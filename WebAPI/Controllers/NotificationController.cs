using DataAccess.EFCore.Repositories;
using Domain.Auth;
using Domain.Interfaces;
using Domain.Models;
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
                id = n.Id,
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
        [HttpPost]
        public async Task<IActionResult> NotifyUser([FromBody]NotifyDTO notification)
        {
            var user = await _unitOfWork.Users.FindSingle(u => u.Id == notification.userId);
            if (user == null)
                return NotFound();
            Notification note = new Notification
            {
                UserId = notification.userId,
                IsRead = false,
                Message = notification.message,
                Created = DateTime.Now,

            };
            await _unitOfWork.Notifications.AddAsync(note);
            await _unitOfWork.Complete();
            return Ok(new Response {Status="success", Message = "user notified successfully" });
        }
        [HttpPost("notify-all")]
        public async Task<IActionResult> NotifyAll([FromBody] NotifyDTO notification)
        {
            if(notification.message==null)
                return NotFound("empty message");
            var users = _unitOfWork.Users.GetAll();
            List<Notification> notes = new List<Notification>();
            foreach (var user in users)
            {
                Notification note = new Notification
                {
                    UserId = user.Id,
                    IsRead = false,
                    Message = notification.message,
                    Created = DateTime.Now,

                };
                notes.Add(note);
            }
         await   _unitOfWork.Notifications.AddRangeAsync(notes);
            await _unitOfWork.Complete();
            return Ok(new Response {Status = "success",Message="All users notified successfully"});
        }
    }
}

