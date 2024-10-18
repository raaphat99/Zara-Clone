using Domain.Auth;
using Domain.Enums;
using Domain.Interfaces;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using System.IdentityModel.Tokens.Jwt;
using WebAPI.DTOs;

using WebAPI.Services;


namespace WebAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private const string TrackingPrefix = "#ZA";
        private readonly ProductService _productService;
        public OrderController(IUnitOfWork unitOfWork, ProductService productService)
        {
            _unitOfWork = unitOfWork;
            _productService = productService;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDTO>>> GetOrders()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized();
            }
            string userId = User.FindFirst(JwtRegisteredClaimNames.Sid).Value;
            var user = await _unitOfWork.Users.GetByIdAsync(userId);

            if (user == null)
                return NotFound("User not found!");
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("User ID cannot be null or empty.");
            }

            var orders
                = await _unitOfWork.Orders.GetAll()
                .Where(o => o.UserId == userId)
                .ToListAsync();

            if (orders == null || !orders.Any())
            {
                return NotFound();
            }

            var orderDtos = orders.Select(order => new OrderDTO
            {
                trackingNumber = order.TrackingNumber,
                created = order.Created.ToString(),
                status = order.Status.ToString(),
                totalPrice = order.TotalPrice,
                items = order.OrderItems.Select(item => new OrderItemDTO
                {
                    name = item.ProductVariant.Product.Name,
                    productImage = item.ProductVariant.ProductImage.FirstOrDefault()?.ImageUrl,
                    quantity = item.Quantity,
                    unitPrice = item.UnitPrice
                }).ToList()
            }).ToList();

            return Ok(orderDtos);
        }
        [HttpGet("tracking/{trackingNumber}")]
        public async Task<ActionResult<OrderDetailsDTO>> GetOrderDetails(string trackingNumber)
        {
            string userId = User.FindFirst(JwtRegisteredClaimNames.Sid).Value;
            var user = await _unitOfWork.Users.GetByIdAsync(userId);

            if (user == null)
                return NotFound("User not found!");
            var order = await _unitOfWork.Orders.GetOrderByTrackingNumberAsync(userId, trackingNumber);

            if (order == null)
            {
                return null;
            }

            var orderDTO = new OrderDetailsDTO
            {
                trackingNumber = order.TrackingNumber,
                status = order.Status.ToString(),
                orderDate = order.Created.Value,
                totalPrice = order.TotalPrice,
                shippingCost = order.ShippingMethod.ShippingCost,
                paymentMethod = order.Payment?.PaymentMethod,

                customer = new CustomerDTO
                {
                    name = $"{order.User.Name} {order.User.Surname}",
                    email = order.User.Email,
                    shippingAddress = $"{order.User.Adresses.FirstOrDefault()?.Street}, {order.User.Adresses.FirstOrDefault()?.City}, {order.User.Adresses.FirstOrDefault()?.Country}"
                },

                items = order.OrderItems.Select(item => new OrderItemDTO
                {
                    name = item.ProductVariant.Product.Name,
                    unitPrice = item.UnitPrice,
                    quantity = item.Quantity,
                    subtotal = item.Quantity * item.UnitPrice,
                    productImage = item.ProductVariant.ProductImage.FirstOrDefault()?.ImageUrl,
                    color = item.ProductVariant.ProductColor.ToString(),
                    size = item.ProductVariant.Size.ToString()
                }).ToList()
            };

            return Ok(orderDTO);
        }
        [HttpPut("/{orderId}/status")]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, OrderStatus newStatus)
        {
            string userId = User.FindFirst(JwtRegisteredClaimNames.Sid).Value;
            var user = await _unitOfWork.Users.GetByIdAsync(userId);

            if (user == null)
                return NotFound("User not found!");
            var order = await _unitOfWork.Orders.GetByIdAsync(orderId);

            if (order == null)
            {
                return NotFound("Order not found.");
            }

            if (order.Status == newStatus)
            {
                return BadRequest("Order is already in the specified status.");
            }

            order.Status = newStatus;

            string message = GenerateNotificationMessage(newStatus, order.TrackingNumber);

            var notificationDto = new NotificationDTO
            {
                userId = order.UserId,
                message = message,
                isRead = false,
                created = DateTime.UtcNow
            };

            var notification = new Notification
            {
                UserId = notificationDto.userId,
                Message = notificationDto.message,
                IsRead = notificationDto.isRead,
                Created = notificationDto.created
            };

            await _unitOfWork.Notifications.AddAsync(notification);
            await _unitOfWork.Complete();

            return Ok($"Order status updated to {newStatus} for tracking number {order.TrackingNumber} and notification sent.");
        }
        [HttpPost]
        public async Task<IActionResult> CreateOrder(List<CheckoutDTO> items)
        {
            if (!ModelState.IsValid)
                return BadRequest(new Response { Status = "Error", Message = "Send Valid Data" });

            var userId = User.FindFirst(JwtRegisteredClaimNames.Sid).Value;
            var user = _unitOfWork.Users.FindSingle(u => u.Id == userId);
            if (user == null)
                return Unauthorized(new Response { Status = "Error", Message = "Invalid token or email not found in token" });

            //get tracking number numeric section from data base
            var trackingNumberRecord = await _unitOfWork.TrackingNumbers.GetByIdAsync(1);
            int trackingNumber = trackingNumberRecord.UniversalTrackingNumber;

            double totalAmmount = 0;

            // summate total ammount paid for whole order

            foreach (var item in items)
            {
                totalAmmount += item.Subtotal;
            }

            // create the order in the initial state 

            Order order = new Order
            {
                UserId = userId,
                UserAddressId = items[0].UserAddressId,
                ShippingMethodId = items[0].ShippingMethodId,
                TrackingNumber = $"{TrackingPrefix}{trackingNumber}",  // concatonating numeric section of tracing number to it's contant trademark prefix
                TotalPrice = totalAmmount,
                Created = DateTime.UtcNow,
                Status = OrderStatus.Pending

            };

            // update the numeric section the data base record
            trackingNumberRecord.UniversalTrackingNumber += 1;
            _unitOfWork.TrackingNumbers.Update(trackingNumberRecord);
            await _unitOfWork.Orders.AddAsync(order);
            await _unitOfWork.Complete();


            //create order items after obtaining order Id

            var orderItems = new List<OrderItem>();

            foreach (var item in items)
            {
                OrderItem orderItem = new OrderItem
                {
                    ProductVariantId = item.ProductVariantId,
                    OrderId = order.Id,
                    UnitPrice = item.UnitPrice,
                    Quantity = item.Quantity,
                    Subtotal = item.Subtotal,

                };
                orderItems.Add(orderItem);
            }
            // creating payment record after obtaining order Id 
            Payment payment = new Payment
            {
                OrderId = order.Id,
                Amount = order.TotalPrice,
                Created = DateTime.UtcNow,
                AmountRefunded = 0,
                PaymentMethod = items[0].PaymentMethod,
                PaymentStatus = PaymentStatus.Pending,

            };
            if (items[0].PaymentMethod == "POD")
            {
                

                order.Status = OrderStatus.Shipped; //update order status
                var cart = await _unitOfWork.Carts.FindSingle(c => c.UserId == order.UserId);
                cart.CartItems.Clear();            //clear cart for this user

                foreach (var item in order.OrderItems)
                {
                    await _productService.UpdateStockQuantity(item.ProductVariantId ?? 0, item.Quantity); //update stock quantity
                }

                Notification notification = new Notification   // notify user
                {
                    UserId = order.UserId,
                    Message = $"Good news! Order {order.TrackingNumber} has been shipped.",
                    Created = DateTime.Now,
                    IsRead = false
                };
                await _unitOfWork.OrderItems.AddRangeAsync(orderItems);
                await _unitOfWork.Payments.AddAsync(payment);
                await _unitOfWork.Complete();
                return Redirect("http://localhost:4200/home");
            }

            await _unitOfWork.OrderItems.AddRangeAsync(orderItems);
            await _unitOfWork.Payments.AddAsync(payment);
            await _unitOfWork.Complete();

            // hash code for kashier server call 
            string hash = Kashier.create_hash(order.TotalPrice.ToString(), order.Id.ToString());
            return Ok(hash);

        }
        private string GenerateNotificationMessage(OrderStatus newStatus, string trackingNumber)
        {
            return newStatus switch
            {
                OrderStatus.Pending => $"Order {trackingNumber} is pending. We are processing it.",
                OrderStatus.Shipped => $"Good news! Order {trackingNumber} has been shipped.",
                OrderStatus.Delivered => $"Your order {trackingNumber} has been delivered successfully.",
                OrderStatus.Canceled => $"We regret to inform you that order {trackingNumber} has been canceled.",
                _ => "Unknown order status."
            };
        }

    }

}


