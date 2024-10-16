using Domain.Enums;
using Domain.Interfaces;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe.Climate;
using System.IdentityModel.Tokens.Jwt;
using WebAPI.DTOs;

namespace WebAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
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
                status=order.Status.ToString(),
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


