using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;
using Microsoft.AspNetCore.SignalR;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using NuGet.Protocol.Core.Types;

namespace LIMSApi.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _repo;
        private readonly IHubContext<NotificationHub> _hub;
        private readonly IPushNotificationService _pushNotificationService;
        private readonly EmailService _emailService;
        private readonly LoggedInUserDTO _loggedInUser;

        public NotificationService(INotificationRepository repo, IHubContext<NotificationHub> hub, IPushNotificationService pushNotificationService, EmailService emailService)
        {
            _repo = repo;
            _hub = hub;
            _pushNotificationService = pushNotificationService;
            _emailService = emailService;
            _loggedInUser = LoggedInUserProvider.CurrentUser;
        }

        public async Task CreateNotificationAsync(Notification notification)
        {
            await _repo.AddNotificationAsync(notification);

            // Push real-time via SignalR
            if(notification.UserID != null)
            {
                await _hub.Clients.User(notification.UserID.ToString())
                .SendAsync("ReceiveNotification", notification);
            }
            // Send email if needed
            if (notification.UserID != null && !string.IsNullOrWhiteSpace(notification.Email))
            {
                var subject = $"New Notification: {notification.Title}";
                var body = $"<h3>{notification.Title}</h3><p>{notification.Message}</p>";

                await _emailService.SendEmailAsync(notification.Email, subject, body);
            }

            // Push notification
            if (notification.UserID != null && notification.UserID.HasValue)
            {
                await _pushNotificationService.SendPushNotificationAsync(
                    notification.UserID.Value,
                    notification.Title,
                    notification.Message,
                    notification.EntityType,
                    notification.EntityID ?? 0
                );
            }
        }

        public async Task<List<Notification>> GetUnreadAsync()
        {
            var userId = _loggedInUser.EmployeeID;
            if (userId == null)
            {
                throw new InvalidOperationException("User ID not found in token.");
            }
            return await _repo.GetUnreadNotificationsAsync(userId);
        }

        public async Task<List<Notification>> GetAllAsync()
        {
            var userId = _loggedInUser.EmployeeID;
            if (userId == null)
            {
                throw new InvalidOperationException("User ID not found in token.");
            }
            return await _repo.GetAllNotificationsAsync(userId);
        }

        public async Task MarkAsReadAsync(long notificationId)
        {
            var userId = _loggedInUser.EmployeeID;
            if (userId == null)
            {
                throw new InvalidOperationException("User ID not found in token.");
            }
            await _repo.MarkAsReadAsync(notificationId, userId);
        }

        public async Task MarkAllReadAsync()
        {
            var userId = _loggedInUser.EmployeeID;
            if (userId == null)
            {
                throw new InvalidOperationException("User ID not found in token.");
            }
            await _repo.MarkAllReadAsync(userId);
        }

        // -------------------- PUSH SUBSCRIPTIONS --------------------

        public async Task AddOrUpdateSubscriptionAsync(UserPushSubscription subscription)
        {
            subscription.UserId = _loggedInUser.EmployeeID;
            await _repo.AddOrUpdateSubscriptionAsync(subscription);
        }

        public async Task RemoveSubscriptionAsync(string endpoint)
        {
            await _repo.RemoveSubscriptionAsync(endpoint);
        }
    }

}
