namespace LIMSApi.Services.Interface
{
    public interface IPushNotificationService
    {
        Task SendPushNotificationAsync(long userId, string title, string message, string entityType, long entityId);

        Task<string> GetPublicKey();

    }
}
