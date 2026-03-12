using Cocorra.BLL.Services.NotificationService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Cocorra.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationsController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet("my-notifications")]
        public async Task<IActionResult> GetMyNotifications()
        {
            var currentUserIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(currentUserIdString, out Guid currentUserId)) return Unauthorized();

            var result = await _notificationService.GetMyNotificationsAsync(currentUserId);
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpPut("read-notification/{notificationId:guid}")]
        public async Task<IActionResult> MarkNotificationRead(Guid notificationId)
        {
            var currentUserIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(currentUserIdString, out Guid currentUserId)) return Unauthorized();

            var result = await _notificationService.MarkNotificationAsReadAsync(notificationId, currentUserId);
            return StatusCode((int)result.StatusCode, result);
        }
    }
}