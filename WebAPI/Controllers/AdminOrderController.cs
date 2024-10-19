using Domain.Enums;
using Domain.Interfaces;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using WebAPI.DTOs;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminOrderController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public AdminOrderController(IUnitOfWork unitOfWork)
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
                    name = item.ProductVariant?.Product?.Name ?? "Unknown Product",
                    productImage = item.ProductVariant?.ProductImage?.FirstOrDefault()?.ImageUrl ?? "default-image-url.jpg",
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
                shippingCost = order.ShippingMethod?.ShippingCost ?? 0,
                paymentMethod = order.Payment?.PaymentMethod ?? "Payment method not available",
                customer = new CustomerDTO
                {
                    name = $"{order.User?.Name} {order.User?.Surname}",
                    email = order.User?.Email,
                    shippingAddress = order.User?.Adresses?.FirstOrDefault() != null
                  ? $"{order.User.Adresses.FirstOrDefault()?.Street}, {order.User.Adresses.FirstOrDefault()?.City}, {order.User.Adresses.FirstOrDefault()?.Country}"
                      : "Address not available"
                },



                items = order.OrderItems?.Select(item => new OrderItemDTO
                {
                    name = item.ProductVariant?.Product?.Name ?? "Unknown",
                    unitPrice = item.UnitPrice,
                    quantity = item.Quantity,
                    subtotal = item.Quantity * item.UnitPrice,
                    productImage = item.ProductVariant?.ProductImage?.FirstOrDefault()?.ImageUrl ?? "Image not available",
                    color = item.ProductVariant != null ? item.ProductVariant.ProductColor.ToString() : "N/A",
                    size = item.ProductVariant?.Size?.ToString() ?? "N/A"
                }).ToList()

            };

            return Ok(orderDTO);
        }
        [HttpGet("all")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<OrderDTO>>> GetAllOrders()
        {
            var orders = await _unitOfWork.Orders.GetAll().ToListAsync();

            if (orders == null || !orders.Any())
            {
                return NotFound("No orders found.");
            }
            var orderDtos = orders.Select(order => new OrderDTO
            {
                id = order.Id,
                trackingNumber = order.TrackingNumber,
                created = order.Created.ToString(),
                status = order.Status.ToString(),
                totalPrice = order.TotalPrice,
                items = order.OrderItems.Select(item => new OrderItemDTO
                {
                    name = item.ProductVariant?.Product?.Name ?? "Unknown Product",
                    productImage = item.ProductVariant?.ProductImage?.FirstOrDefault()?.ImageUrl ?? "default-image-url.jpg",
                    quantity = item.Quantity,
                    unitPrice = item.UnitPrice
                }).ToList(),
                customerName = $"{order.User?.Name ?? "Unknown"} {order.User?.Surname ?? "Customer"}",
            }).ToList();

            return Ok(orderDtos);
        }
        [HttpDelete("{orderId}")]
        public async Task<IActionResult> DeleteOrder(int orderId)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(orderId);

            if (order == null)
            {
                return NotFound("Order not found.");
            }

            var payments = await _unitOfWork.Payments.FindAsync(p => p.OrderId == orderId);
            _unitOfWork.Payments.RemoveRange(payments);

            var orderItems = await _unitOfWork.OrderItems.FindAsync(oi => oi.OrderId == orderId);
            _unitOfWork.OrderItems.RemoveRange(orderItems);

            _unitOfWork.Orders.Remove(order);

            await _unitOfWork.Complete();

            return NoContent();
        }


        [HttpPut("{orderId}/status")]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, string newStatus)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(orderId);

            if (order == null)
            {
                return NotFound("Order not found.");
            }

            if (!Enum.TryParse<OrderStatus>(newStatus, true, out var parsedStatus))
            {
                return BadRequest("Invalid status value.");
            }

            if (order.Status == parsedStatus)
            {
                return BadRequest("Order is already in the specified status.");
            }

            order.Status = parsedStatus;

            string message = GenerateNotificationMessage(parsedStatus, order.TrackingNumber);

            var notification = new Notification
            {
                UserId = order.UserId,
                Message = message,
                IsRead = false,
                Created = DateTime.UtcNow
            };
            await _unitOfWork.Notifications.AddAsync(notification);

            await _unitOfWork.Complete();
            return NoContent();
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
