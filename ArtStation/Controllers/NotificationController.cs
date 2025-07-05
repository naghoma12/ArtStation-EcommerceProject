using ArtStation.Core.Helper;
using ArtStation.Core.Repository.Contract;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
        [HttpPost("SendNotification")]
        public async Task<IActionResult> SendNotification([FromQuery] int userId, [FromBody] MessageRequest request)
        {
            var language = Request.Headers["Accept-Language"].ToString();
            if (string.IsNullOrWhiteSpace(language) || (language != "en" && language != "ar"))
                language = "en";

            if (userId <= 0 || request == null || string.IsNullOrWhiteSpace(request.DeviceToken))
            {
                return BadRequest("Invalid input data.");
            }

            var result = await _notificationRepository.SendNotification(language, userId, request);
            return Ok(new { Message = "Notification sent successfully.", Result = result });
        }
    }
}
