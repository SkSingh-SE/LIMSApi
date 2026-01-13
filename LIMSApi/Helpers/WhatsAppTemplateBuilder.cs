using System;
using System.Text;
using System.Reflection;
using LIMSApi.Helpers.Enums;

namespace LIMSApi.Helpers
{
    public static class WhatsAppTemplateBuilder
    {
        public static string BuildWithTemplateKey(
            MessageTemplateKey key,
            object model)
        {
            var template = GetTemplate(key);
            return ReplacePlaceholders(template, model);
        }
        public static string Build(string body, object model)
        {
            return ReplacePlaceholders(body, model);
        }

        // =====================================================
        // TEMPLATE REGISTRY (ENUM BASED)
        // =====================================================
        private static string GetTemplate(MessageTemplateKey key)
        {
            return key switch
            {
                MessageTemplateKey.SAMPLE_INWARD_ACK =>
                    "Dear {{CustomerName}}, your sample has been received. Case {{CaseNo}}.",

                MessageTemplateKey.PROFORMA_INVOICE_SENT =>
                    "Proforma Invoice {{PINO}} has been generated.",

                MessageTemplateKey.PAYMENT_LINK =>
                    "Payment of ₹{{Amount}} pending. Pay here: {{PaymentLink}}",

                MessageTemplateKey.PAYMENT_RECEIVED =>
                    "Payment received successfully. Thank you.",

                MessageTemplateKey.AMENDED_REPORT_READY =>
                    "Your amended report is ready.",

                MessageTemplateKey.CASE_CLOSED =>
                    "Your case {{CaseNo}} has been closed.",

                _ => throw new InvalidOperationException(
                    $"WhatsApp template not defined for {key}")
            };
        }

        // =====================================================
        // PLACEHOLDER ENGINE
        // =====================================================
        private static string ReplacePlaceholders(string template, object model)
        {
            if (model == null)
                return template;

            var result = new StringBuilder(template);
            var props = model.GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var prop in props)
            {
                var placeholder = $"{{{{{prop.Name}}}}}";
                var value = prop.GetValue(model)?.ToString() ?? string.Empty;
                result.Replace(placeholder, value);
            }

            return result.ToString();
        }
    }
}
