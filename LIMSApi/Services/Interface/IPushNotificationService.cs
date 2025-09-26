namespace LIMSApi.Services.Interface
{
    public interface IPushNotificationService
    {
        Task SendPushNotificationAsync(long userId, string title, string message);

        Task<string> GetPublicKey();

    }
}
