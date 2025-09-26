using LIMSApi.Models;

namespace LIMSApi.Repositories.Interface
{
    public interface INotificationRepository
    {
        Task AddNotificationAsync(Notification notification);
        Task<List<Notification>> GetUnreadNotificationsAsync(long userId);
        Task<List<Notification>> GetAllNotificationsAsync(long userId);
        Task MarkAsReadAsync(long notificationId, long userId);
        Task MarkAllReadAsync(long userId);

        // Push subscriptions
        Task AddOrUpdateSubscriptionAsync(UserPushSubscription subscription);
        Task<List<UserPushSubscription>> GetUserSubscriptionsAsync(long userId);
        Task RemoveSubscriptionAsync(string endpoint);
    }
}
