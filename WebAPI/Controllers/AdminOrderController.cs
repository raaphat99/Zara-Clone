using AutoMapper;
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
        private readonly IMapper _mapper;

        public AdminOrderController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;

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

            var orders = await _unitOfWork.Orders.GetAll()
                .Where(o => o.UserId == userId)
                .ToListAsync();

            if (orders == null || !orders.Any())
            {
                return NotFound();
            }

            var orderDtos = _mapper.Map<List<OrderDTO>>(orders);

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
                return NotFound("Order not found!");
            }

            var orderDetailsDto = _mapper.Map<OrderDetailsDTO>(order);

            return Ok(orderDetailsDto);
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
            var orderDtos = _mapper.Map<List<OrderDTO>>(orders);

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
