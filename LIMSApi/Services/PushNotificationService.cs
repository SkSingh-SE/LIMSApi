using Lib.Net.Http.WebPush.Authentication;
using Lib.Net.Http.WebPush;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;
using System;

namespace LIMSApi.Services
{
    public class PushNotificationService : IPushNotificationService
    {
        private readonly INotificationRepository _repository;
        private readonly PushServiceClient _pushClient;
        private readonly string _subject;

        public PushNotificationService(INotificationRepository repository, IConfiguration config)
        {
            _repository = repository;

            _subject = config["Vapid:Subject"] ?? "mailto:admin@yourdomain.com";

            var vapidDetails = new VapidAuthentication(config["Vapid:PublicKey"], config["Vapid:PrivateKey"])
            {
                Subject = _subject
            };

            _pushClient = new PushServiceClient
            {
                DefaultAuthentication = vapidDetails
            };
        }

        public Task<string> GetPublicKey()
        {
            return _pushClient.DefaultAuthentication is VapidAuthentication vapid
                ? Task.FromResult(vapid.PublicKey)
                : Task.FromResult<string>(null);
        }

        public async Task SendPushNotificationAsync(long userId, string title, string message, string entityType, long entityId)
        {
            var subscriptions = await _repository.GetUserSubscriptionsAsync(userId);

            if (!subscriptions.Any()) return;

            var payload = System.Text.Json.JsonSerializer.Serialize(new
            {
                title,
                message,
                timestamp = DateTime.UtcNow,
                url = GenerateEntityUrl(entityType, entityId)
            });

            foreach (var sub in subscriptions)
            {
                try
                {
                    var pushSubscription = new PushSubscription
                    {
                        Endpoint = sub.Endpoint,
                        Keys = new Dictionary<string, string>
                        {
                            { "p256dh", sub.P256DH },
                            { "auth", sub.Auth }
                        }
                    };

                    await _pushClient.RequestPushMessageDeliveryAsync(pushSubscription, new PushMessage(payload));
                }
                catch (Exception ex)
                {
                    // Remove invalid subscription (expired/denied)
                    await _repository.RemoveSubscriptionAsync(sub.Endpoint);
                    Console.WriteLine($"Push failed for {sub.Endpoint}: {ex.Message}");
                }
            }
        }
        public string GenerateEntityUrl(string entityType, long entityId)
        {
            return entityType.ToLower() switch
            {
                "request of review" => "/sample/review",
                _ => "/"
            };
        }


    }
}
