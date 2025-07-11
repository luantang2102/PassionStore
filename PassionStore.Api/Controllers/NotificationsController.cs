using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PassionStore.Api.Controllers.Base;
using PassionStore.Api.Extensions;
using PassionStore.Application.DTOs.Notifications;
using PassionStore.Application.Helpers.Params;
using PassionStore.Application.Interfaces;
using PassionStore.Application.Paginations;

namespace PassionStore.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : BaseApiController
    {
        private readonly INotificationService _notificationService;

        public NotificationsController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetNotifications([FromQuery] NotificationParams @params)
        {
            var userId = User.GetUserId();
            var notifications = await _notificationService.GetNotificationsAsync(userId, @params);
            Response.AddPaginationHeader(notifications.Metadata);
            return Ok(notifications);
        }


        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateNotification([FromBody] NotificationRequest notificationRequest)
        {
            var notification = await _notificationService.CreateNotificationAsync(
                Guid.Parse(notificationRequest.UserId),
                Guid.Parse(notificationRequest.ObjectId),
                notificationRequest.ObjectType,
                notificationRequest.Content
            );
            return CreatedAtAction(nameof(GetNotifications), new { id = notification.Id }, notification);
        }

        [HttpPut("{id}/read")]
        [Authorize]
        public async Task<IActionResult> MarkNotificationAsRead(Guid id)
        {
            var userId = User.GetUserId();
            await _notificationService.MarkNotificationAsReadAsync(id, userId);
            return NoContent();
        }
    }
}