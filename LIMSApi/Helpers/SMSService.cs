using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace LIMSApi.Helpers
{
    public class SMSService
    {
        private readonly IConfiguration _configuration;

        public SMSService(IConfiguration configuration)
        {
            _configuration = configuration;
            TwilioClient.Init(
                _configuration["TwilioSMS:AccountSID"],
                _configuration["TwilioSMS:AuthToken"]
            );
        }

        public async Task<bool> SendSmsAsync(string toPhoneNumber, string message)
        {
            // Normalize to compact E.164 (e.g. "+919876543210"); Twilio rejects spaces.
            var toE164 = PhoneHelper.ToE164(toPhoneNumber);
            if (string.IsNullOrEmpty(toE164))
            {
                Console.WriteLine("SMS Skipped: phone number is empty.");
                return false;
            }

            try
            {
                var smsMessage = await MessageResource.CreateAsync(
                    body: message,
                    from: new Twilio.Types.PhoneNumber(_configuration["TwilioSMS:FromPhoneNumber"]),
                    to: new Twilio.Types.PhoneNumber(toE164)
                );

                Console.WriteLine($"SMS sent with SID: {smsMessage.Sid}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SMS sending failed: {ex.Message}");
                return false;
            }
        }
    }
}
