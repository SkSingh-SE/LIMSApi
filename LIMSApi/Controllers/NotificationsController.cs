using LIMSApi.Models;
using LIMSApi.Services;
using LIMSApi.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Twilio.TwiML.Voice;

namespace LIMSApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _service;
        private readonly IPushNotificationService _pushNotificationService;
        public NotificationsController(INotificationService service, IPushNotificationService pushNotificationService)
        {
            _service = service;
            _pushNotificationService = pushNotificationService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(Notification notification)
        {
            await _service.CreateNotificationAsync(notification);
            return Ok();
        }

        [HttpGet("unread")]
        public async Task<IActionResult> GetUnread()
        {
            var notifications = await _service.GetUnreadAsync();
            return Ok(notifications);
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var notifications = await _service.GetAllAsync();
            return Ok(notifications);
        }

        [HttpPatch("mark-as-read/{id}")]
        public async Task<IActionResult> MarkAsRead(long id)
        {
            await _service.MarkAsReadAsync(id);
            return Ok(new
            {
                status = "success",
                message = $"Notification marked successfully."
            });
        }
        [HttpGet("mark-all-read")]
        public async Task<IActionResult> MarkAllRead()
        {
            await _service.MarkAllReadAsync();
            return Ok(new
            {
                status = "success",
                message = $"Notification marked successfully."
            });
        }
        [HttpGet("vapid-public-key")]
        [AllowAnonymous] // public key can be shared
        public IActionResult GetVapidPublicKey()
        {
            var publicKey = _pushNotificationService.GetPublicKey();
            return Ok(new { publicKey });
        }

        [HttpPost("subscribe")]
        public async Task<IActionResult> Subscribe( UserPushSubscription dto)
        {
            await _service.AddOrUpdateSubscriptionAsync(dto);

            return Ok(new { success = true });
        }

        [HttpPost("unsubscribe")]
        public async Task<IActionResult> Unsubscribe(UserPushSubscription dto)
        {
            await _service.RemoveSubscriptionAsync(dto.Endpoint);

            return Ok(new { success = true });
        }


    }

}
