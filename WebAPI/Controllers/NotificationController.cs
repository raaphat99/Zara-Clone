using DataAccess.EFCore.Repositories;
using Domain.Interfaces;
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
    public class NotificationController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public NotificationController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [Authorize]
      [HttpGet]
        public async Task<ActionResult<IEnumerable<NotificationDTO>>> GetNotifications()
        {
            string userId = User.FindFirst(JwtRegisteredClaimNames.Sid).Value;
            var user = await _unitOfWork.Users.GetByIdAsync(userId);

            if (user == null)
                return NotFound("User not found!");
            var notifications = await _unitOfWork.Notifications.GetAll()
                .Where(n => n.UserId == userId)
                .ToListAsync();

            if (notifications == null || !notifications.Any())
            {
                return NotFound("No notifications found for this user.");
            }

            var notificationDtos = notifications.Select(n => new NotificationDTO
            {
                id=n.Id,
                userId = n.UserId,
                message = n.Message,
                isRead = n.IsRead,
                created = n.Created
            }).ToList();

            return Ok(notificationDtos);
        }

        // PUT: api/notification/mark-read/{notificationId}
        [HttpPut("mark-read/{notificationId}")]
        public async Task<IActionResult> MarkNotificationAsRead(int notificationId)
        {
            var notification = await _unitOfWork.Notifications.GetByIdAsync(notificationId);

            if (notification == null)
            {
                return NotFound("Notification not found.");
            }

            notification.IsRead = true; 

            await _unitOfWork.Complete(); 

            return NoContent();
        }

        // DELETE: api/notification/{notificationId}
        [HttpDelete("{notificationId}")]
        public async Task<IActionResult> DeleteNotification(int notificationId)
        {
            var notification = await _unitOfWork.Notifications.GetByIdAsync(notificationId);

            if (notification == null)
            {
                return NotFound("Notification not found.");
            }

            _unitOfWork.Notifications.Remove(notification); 
            await _unitOfWork.Complete(); 

            return Ok("Notification deleted.");
        }
    }
}

