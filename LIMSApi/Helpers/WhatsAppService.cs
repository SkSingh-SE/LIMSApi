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
            // Stored value already carries the country code (e.g. "+91 9876543210").
            // Normalize to compact E.164 (e.g. "+919876543210") — Twilio rejects spaces
            // and the country code must NOT be hardcoded anymore.
            var toE164 = PhoneHelper.ToE164(toPhoneNumber);
            if (string.IsNullOrEmpty(toE164))
            {
                Console.WriteLine("WhatsApp Message Skipped: phone number is empty.");
                return false;
            }

            try
            {
                var whatsappMessage = await MessageResource.CreateAsync(
                    body: message,
                    from: new Twilio.Types.PhoneNumber(_configuration["TwilioWhatsAPP:FromWhatsAppNumber"]),
                    to: new Twilio.Types.PhoneNumber($"whatsapp:{toE164}")
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
            var toE164 = PhoneHelper.ToE164(toPhoneNumber);
            if (string.IsNullOrEmpty(toE164))
            {
                Console.WriteLine("WhatsApp Message Skipped: phone number is empty.");
                return false;
            }

            try
            {
                var messageOptions = new CreateMessageOptions(new PhoneNumber($"whatsapp:{toE164}"))
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
