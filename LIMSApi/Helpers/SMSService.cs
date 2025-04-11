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
            try
            {
                var smsMessage = await MessageResource.CreateAsync(
                    body: message,
                    from: new Twilio.Types.PhoneNumber(_configuration["TwilioSMS:FromPhoneNumber"]),
                    to: new Twilio.Types.PhoneNumber(toPhoneNumber)
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
