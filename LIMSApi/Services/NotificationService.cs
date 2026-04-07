using LIMSApi.Data;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _repo;
        private readonly IHubContext<NotificationHub> _hub;
        private readonly IPushNotificationService _pushNotificationService;
        private readonly EmailService _emailService;
        private readonly LIMSContext _db;
        private readonly LoggedInUserDTO _loggedInUser;

        public NotificationService(INotificationRepository repo, IHubContext<NotificationHub> hub, IPushNotificationService pushNotificationService, EmailService emailService, LIMSContext db)
        {
            _repo = repo;
            _hub = hub;
            _pushNotificationService = pushNotificationService;
            _emailService = emailService;
            _db = db;
            _loggedInUser = LoggedInUserProvider.CurrentUser;
        }

        public async Task CreateNotificationAsync(Notification notification)
        {
            await _repo.AddNotificationAsync(notification);

            // Push real-time via SignalR (don't fail if push fails)
            try
            {
                if (notification.UserID != null)
                {
                    await _hub.Clients.User(notification.UserID.ToString())
                        .SendAsync("ReceiveNotification", notification);
                }
            }
            catch { /* SignalR push failed — notification is saved in DB, user will see it on next load */ }

            // Send email if needed (don't fail if email fails)
            try
            {
                if (notification.UserID != null && !string.IsNullOrWhiteSpace(notification.Email))
                {
                    var subject = $"New Notification: {notification.Title}";
                    var body = $"<h3>{notification.Title}</h3><p>{notification.Message}</p>";
                    await _emailService.SendEmailAsync(notification.Email, subject, body);
                }
            }
            catch { /* Email failed — notification is saved in DB */ }

            // Push notification (don't fail if push fails)
            try
            {
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
            catch { /* Web push failed — notification is saved in DB */ }
        }

        public async Task NotifyByRoleAsync(string roleName, string title, string message, string? entityType = null, long? entityId = null)
        {
            var roleId = await _db.RoleMasters
                .Where(r => r.Name == roleName && r.IsActive)
                .Select(r => r.ID)
                .FirstOrDefaultAsync();
            if (roleId == 0) return;

            var employees = await _db.EmployeeMasters
                .Where(e => e.IsActive && e.RoleID == roleId)
                .Select(e => new { e.ID, e.EmailId })
                .ToListAsync();

            foreach (var emp in employees)
            {
                try
                {
                    await CreateNotificationAsync(new Notification
                    {
                        UserID = emp.ID,
                        Email = emp.EmailId,
                        Title = title,
                        Message = message,
                        Type = NotificationType.System,
                        EntityType = entityType,
                        EntityID = entityId
                    });
                }
                catch { /* Skip failed employee, continue notifying others */ }
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
