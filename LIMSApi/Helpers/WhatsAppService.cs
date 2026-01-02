using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace LIMSApi.Helpers
{
    public class WhatsAppService
    {
        private readonly IConfiguration _configuration;

        public WhatsAppService(IConfiguration configuration)
        {
            _configuration = configuration;
            TwilioClient.Init(
                _configuration["TwilioWhatsAPP:AccountSID"],
                _configuration["TwilioWhatsAPP:AuthToken"]
            );
        }
        public async Task<bool> SendWhatsAppMessageAsync(string toPhoneNumber, string message)
        {
            try
            {
                var whatsappMessage = await MessageResource.CreateAsync(
                    body: message,
                    from: new Twilio.Types.PhoneNumber(_configuration["TwilioWhatsAPP:FromWhatsAppNumber"]),
                    to: new Twilio.Types.PhoneNumber($"whatsapp:+91{toPhoneNumber}")
                );

                Console.WriteLine($"WhatsApp Message Sent - SID: {whatsappMessage.Sid}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"WhatsApp Message Failed: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> SendWhatsAppMessageAsync(string toPhoneNumber, string message, List<string> mediaUrls)
        {
            try
            {
                var messageOptions = new CreateMessageOptions(new PhoneNumber($"whatsapp:{toPhoneNumber}"))
                {
                    From = new PhoneNumber(_configuration["TwilioWhatsAPP:FromWhatsAppNumber"]),
                    Body = message
                };

                if (mediaUrls != null && mediaUrls.Count > 0)
                {
                    foreach (var mediaUrl in mediaUrls)
                    {
                        messageOptions.MediaUrl.Add(new Uri(mediaUrl));
                    }
                }

                var whatsappMessage = await MessageResource.CreateAsync(messageOptions);
                Console.WriteLine($"WhatsApp Message Sent - SID: {whatsappMessage.Sid}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"WhatsApp Message Failed: {ex.Message}");
                return false;
            }
        }
    }
}
