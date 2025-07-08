using ArtStation.Core.Helper;
using ArtStation.Core.Repository.Contract;
using ArtStation.Core.Resources;
using FirebaseAdmin.Messaging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ArtStation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationRepository _notificationRepository;

        public NotificationController(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }
        //[HttpPost("SendNotification")]
        //public async Task<IActionResult> SendNotification([FromQuery] int userId, [FromBody] MessageRequest request)
        //{
        //    var language = Request.Headers["Accept-Language"].ToString();
        //    if (string.IsNullOrWhiteSpace(language) || (language != "en" && language != "ar"))
        //        language = "en";

        //    if (userId <= 0 || request == null || string.IsNullOrWhiteSpace(request.DeviceToken))
        //    {
        //        return BadRequest("Invalid input data.");
        //    }

        //    var result = await _notificationRepository.SendNotification(language, userId, request);
        //    return Ok(new { Message = "Notification sent successfully.", Result = result });
        //}

        [HttpGet("UnreadCount")]
        public async Task<IActionResult> UnreadCount(string token)
        {

            var userId = Utility.GetUserId(token);
            if (userId <= 0)
            {
                return BadRequest("Invalid user ID.");
            }
            var unreadCount = await _notificationRepository.IsUnReadNotification(userId);
            return Ok(new
            {
                Message = unreadCount > 0 ? ControllerMessages.NotiCount : ControllerMessages.NoNotiCount,
                UnreadCount = unreadCount
            });
        }

        [HttpGet("GetNotifications")]
        public async Task<IActionResult> GetNotifications(string? token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return BadRequest("Token is required.");
            }
            var userId = Utility.GetUserId(token);
            if (userId <= 0)
            {
                return BadRequest("Invalid user ID.");
            }
            var language = Request.Headers["Accept-Language"].ToString();
            if (string.IsNullOrWhiteSpace(language) || (language != "en" && language != "ar"))
                language = "en";
            var notifications = await _notificationRepository.GetNotifications(language, userId);
            if (notifications == null || !notifications.Any())
            {
                return Ok(new { Message = ControllerMessages.NoNotiFound });
            }

            return Ok(new { Message = ControllerMessages.NotiFound, Notifications = notifications });
        }

        [HttpPost("MarkAsRead")]
        public async Task<IActionResult> MarkAsRead(string? token)
        {
            try
            {
                var userId = Utility.GetUserId(token);
                if (userId <= 0)
                {
                    return BadRequest("Invalid user ID.");
                }
                await _notificationRepository.MarkAllNotificationAsRead(userId);
                return Ok(new { Message = ControllerMessages.NotiMarkedAsRead });

            }
            catch
            {
                return BadRequest(new
                {
                    Message = ControllerMessages.NotiMarkedAsReadFailed
                });
            }
        }

        [HttpDelete("DeleteNotification/{notificationId}")]
        public async Task<IActionResult> DeleteNotification(int notificationId, string token)
        {
            try
            {
                var userId = Utility.GetUserId(token);
                if (userId <= 0)
                {
                    return BadRequest("Invalid user ID.");
                }
                await _notificationRepository.DeleteNotification(userId, notificationId);
                return Ok(new { Message = ControllerMessages.NotiDeleted });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Message = $"{ControllerMessages.NotiDeleteFailed} {ex.Message.ToString() ?? ex.InnerException?.Message.ToString()}"
                });
            }
        }
        [HttpDelete("DeleteAllNotifications")]
        public async Task<IActionResult> DeleteAllNotifications(string token)
        {
            try
            {
                var userId = Utility.GetUserId(token);
                if (userId <= 0)
                {
                    return BadRequest("Invalid user ID.");
                }
                await _notificationRepository.DeleteAllNotification(userId);
                return Ok(new { Message = ControllerMessages.NotiDeleted });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Message = $"{ControllerMessages.NotiDeleteFailed} {ex.Message.ToString() ?? ex.InnerException?.Message.ToString()}"
                });
            }
        }

    }
}
