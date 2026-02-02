using LIMSApi.Data;
using LIMSApi.Helpers;
using LIMSApi.Helpers.Enums;
using LIMSApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LIMSApi.ServiceWORepo
{
    public class TemplateService
    {
        private readonly LIMSContext _context;
        private readonly ILogger<TemplateService> _logger;

        public TemplateService(LIMSContext context, ILogger<TemplateService> logger)
        {
            _context = context;
            _logger = logger;
        }

        // =====================================================
        // SINGLE SIMPLE FUNCTION (ENUM BASED)
        // =====================================================
        public async Task<string> GetTemplateAsync(
            MessageTemplateKey templateKey,
            NotificationType type,
            object model)
        {
            try
            {
                var template = await _context.MessageTemplates
                    .AsNoTracking()
                    .FirstOrDefaultAsync(t =>
                        t.TemplateKey == templateKey.ToString() &&
                        t.Type == type.ToString() &&
                        t.IsActive);

                if (template != null)
                {
                    return Build(type, template.Body, templateKey, model);
                }

                _logger.LogWarning(
                    "Template '{TemplateKey}' not found for '{Type}', using hardcoded",
                    templateKey, type);

                return Build(type, null, templateKey, model);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Template error for '{TemplateKey}' / '{Type}', using fallback",
                    templateKey, type);

                return Build(type, null, templateKey, model);
            }
        }

        // =====================================================
        // BUILDER ROUTER
        // =====================================================
        private static string Build(
            NotificationType type,
            string? body,
            MessageTemplateKey templateKey,
            object model)
        {
            return type switch
            {
                NotificationType.Email =>
                    !string.IsNullOrWhiteSpace(body)
                        ? EmailTemplateBuilder.Build(body, model)
                        : EmailTemplateBuilder.BuildWithTemplateKey(templateKey, model),

                NotificationType.WhatsApp =>
                    !string.IsNullOrWhiteSpace(body)
                        ? WhatsAppTemplateBuilder.Build(body, model)
                        : WhatsAppTemplateBuilder.BuildWithTemplateKey(templateKey, model),

                _ => "Notification service temporarily unavailable"
            };
        }

        public string GetMessageBody(MessageTemplateKey templateKey)
        {
            return templateKey switch
            {
                MessageTemplateKey.SEND_OTP => @"
                        Dear {UserName},

                        We received a request to enable two-factor authentication on your account.

                        Your one-time verification code is:

                        {OTP}

                        This code is valid for the next 5 minutes.
                        For your security, please do not share this code with anyone.

                        If you did not initiate this request, please contact the system administrator immediately.

                        Regards,
                        Security Team
                        {ApplicationName}
                        ",
                _ => throw new ArgumentOutOfRangeException(nameof(templateKey))
            };
        }

    }
}
