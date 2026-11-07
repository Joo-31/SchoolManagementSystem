using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementAPI.DTOs.Requests;
using SchoolManagementAPI.Services.Interfaces;
using System.Security.Claims;

namespace SchoolManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationsController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        // GET: api/notifications (للمستخدمين)
        [HttpGet]
        public IActionResult GetMyNotifications()
        {
            // بنجيب الـ Role بتاع المستخدم الحالي من التوكن
            var role = User.FindFirst(ClaimTypes.Role)?.Value ?? "Student";
            var notifications = _notificationService.GetForRole(role);
            return Ok(notifications);
        }

        // POST: api/notifications (للأدمن بس)
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Create([FromBody] CreateNotificationDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Title) || string.IsNullOrWhiteSpace(dto.Message))
                return BadRequest("Title and Message are required.");

            _notificationService.Create(dto);
            return Ok(new { message = "Notification sent successfully!" });
        }

        // DELETE: api/notifications/5 (للأدمن بس)
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            _notificationService.Delete(id);
            return Ok();
        }
    }
}