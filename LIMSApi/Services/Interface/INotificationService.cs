using LIMSApi.Models;

namespace LIMSApi.Services.Interface
{
    public interface INotificationService
    {
        Task CreateNotificationAsync(Notification notification);
        Task<List<Notification>> GetUnreadAsync();
        Task<List<Notification>> GetAllAsync();
        Task MarkAsReadAsync(long notificationId);
        Task MarkAllReadAsync();

        // Push subscription management
        Task AddOrUpdateSubscriptionAsync(UserPushSubscription subscription);
        Task RemoveSubscriptionAsync(string endpoint);

    }

}
