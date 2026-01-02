

using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace LIMSApi.Helpers
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;
        public EmailService(IConfiguration configuration)
        {
            this._configuration = configuration;
        }

        public async Task<bool> SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            {
                var email = new MimeMessage();
                email.From.Add(new MailboxAddress(
                    _configuration["SmtpSettings:SenderName"],
                    _configuration["SmtpSettings:SenderEmail"]
                ));
                email.To.Add(new MailboxAddress("", toEmail));
                email.Subject = subject;

                var bodyBuilder = new BodyBuilder { HtmlBody = body };
                email.Body = bodyBuilder.ToMessageBody();

                using var smtp = new SmtpClient();
                 smtp.Connect(
                    _configuration["SmtpSettings:Server"],
                    int.Parse((_configuration["SmtpSettings:Port"]) ?? "457"),
                    SecureSocketOptions.StartTls
                );

                await smtp.AuthenticateAsync(
                    _configuration["SmtpSettings:Username"],
                    _configuration["SmtpSettings:Password"]
                );

                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Email sending failed: {ex.Message}");
                return false;
            }
        }
        public async Task<bool> SendEmailWithAttachment(
    string toEmail,
    string subject,
    string body,
    string attachmentPath,
    string attachmentName)
        {
            try
            {
                var email = new MimeMessage();
                email.From.Add(new MailboxAddress(
                    _configuration["SmtpSettings:SenderName"],
                    _configuration["SmtpSettings:SenderEmail"]
                ));
                email.To.Add(MailboxAddress.Parse(toEmail));
                email.Subject = subject;

                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = body
                };

                if (!string.IsNullOrWhiteSpace(attachmentPath) && File.Exists(attachmentPath))
                {
                    bodyBuilder.Attachments.Add(
                        attachmentName,
                        File.ReadAllBytes(attachmentPath),
                        ContentType.Parse("application/pdf")
                    );
                }

                email.Body = bodyBuilder.ToMessageBody();

                using var smtp = new SmtpClient();
                await smtp.ConnectAsync(
                    _configuration["SmtpSettings:Server"],
                     int.Parse((_configuration["SmtpSettings:Port"]) ?? "457"),
                    SecureSocketOptions.StartTls
                );

                await smtp.AuthenticateAsync(
                    _configuration["SmtpSettings:Username"],
                    _configuration["SmtpSettings:Password"]
                );

                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Email sending failed: {ex.Message}");
                return false;
            }
        }

    }
}
