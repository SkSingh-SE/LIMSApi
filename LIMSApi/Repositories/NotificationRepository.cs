using System.Linq.Dynamic.Core;
using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly LIMSContext _context;
        private LoggedInUserDTO loggedInUser;

        public NotificationRepository(LIMSContext context)
        {
            _context = context;
            loggedInUser = LoggedInUserProvider.CurrentUser;
        }

        public async Task AddNotificationAsync(Notification notification)
        {
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Notification>> GetUnreadNotificationsAsync(long userId)
        {
            return await _context.Notifications
                .AsNoTracking()
                .Where(n => n.UserID == userId && !n.IsRead)
                .OrderByDescending(n => n.CreatedOn)
                .ToListAsync();
        }

        public async Task<List<Notification>> GetAllNotificationsAsync(long userId)
        {
            return await _context.Notifications
                .AsNoTracking()
                .Where(n => n.UserID == userId)
                .OrderByDescending(n => n.CreatedOn)
                .ToListAsync();
        }

        public async Task MarkAsReadAsync(long notificationId, long userId)
        {
            // Ensure user can only mark their own notification
            var notif = await _context.Notifications
                .FirstOrDefaultAsync(n => n.ID == notificationId && n.UserID == userId);

            if (notif == null) return;

            if (!notif.IsRead)
            {
                notif.IsRead = true;
                await _context.SaveChangesAsync();
            }
        }
        public async Task MarkAllReadAsync(long userId)
        {
            // Ensure user can only mark their own notification
            var notif = await _context.Notifications
                .Where(n => n.UserID == userId && !n.IsRead).ToListAsync();

            if (notif == null) return;

            foreach (var n in notif)
            {
                n.IsRead = true;
            }

            await _context.SaveChangesAsync();
        }

        public async Task<int> GetUnreadCountAsync(long userId)
        {
            return await _context.Notifications
                .AsNoTracking()
                .Where(n => n.UserID == userId && !n.IsRead)
                .CountAsync();
        }

        public async Task MarkAllAsReadAsync(long userId)
        {
            var unread = await _context.Notifications
                .Where(n => n.UserID == userId && !n.IsRead)
                .ToListAsync();

            if (!unread.Any()) return;

            foreach (var n in unread)
                n.IsRead = true;

            await _context.SaveChangesAsync();
        }

        // -------------------- PUSH SUBSCRIPTIONS --------------------

        public async Task AddOrUpdateSubscriptionAsync(UserPushSubscription subscription)
        {
            var existing = await _context.UserPushSubscriptions
                .FirstOrDefaultAsync(s => s.Endpoint == subscription.Endpoint && s.UserId == subscription.UserId);

            if (existing == null)
            {
                _context.UserPushSubscriptions.Add(subscription);
            }
            else
            {
                existing.P256DH = subscription.P256DH;
                existing.Auth = subscription.Auth;
            }

            await _context.SaveChangesAsync();
        }

        public async Task<List<UserPushSubscription>> GetUserSubscriptionsAsync(long userId)
        {
            return await _context.UserPushSubscriptions
                .Where(s => s.UserId == userId)
                .ToListAsync();
        }

        public async Task RemoveSubscriptionAsync(string endpoint)
        {
            var sub = await _context.UserPushSubscriptions.FirstOrDefaultAsync(s => s.Endpoint == endpoint);
            if (sub != null)
            {
                _context.UserPushSubscriptions.Remove(sub);
                await _context.SaveChangesAsync();
            }
        }
    }
}
