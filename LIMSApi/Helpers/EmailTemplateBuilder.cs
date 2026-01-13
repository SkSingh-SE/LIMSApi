using System;
using System.Text;
using System.Reflection;
using LIMSApi.Helpers.Enums;

namespace LIMSApi.Helpers
{
    public static class EmailTemplateBuilder
    {
        public static string Build(string body, object model)
        {
            return ReplacePlaceholders(body, model);
        }

        public static string BuildWithTemplateKey(
            MessageTemplateKey key,
            object model)
        {
            var template = GetTemplate(key);
            return ReplacePlaceholders(template, model);
        }

        // =====================================================
        // TEMPLATE REGISTRY (ENUM BASED)
        // =====================================================
        private static string GetTemplate(MessageTemplateKey key)
        {
            return key switch
            {
                MessageTemplateKey.SAMPLE_INWARD_ACK => SampleInwardAck(),
                MessageTemplateKey.SAMPLE_INWARD_INCOMPLETE_REMINDER => SampleInwardIncompleteReminder(),
                MessageTemplateKey.SAMPLE_INWARD_UPDATED => SampleInwardUpdated(),

                MessageTemplateKey.PROFORMA_INVOICE_SENT => ProformaInvoiceSent(),
                MessageTemplateKey.FINAL_INVOICE_GENERATED => FinalInvoiceGenerated(),
                MessageTemplateKey.FINAL_INVOICE_POST_TESTING => FinalInvoicePostTesting(),

                MessageTemplateKey.PAYMENT_LINK => PaymentLink(),
                MessageTemplateKey.PAYMENT_RECEIVED => PaymentReceived(),

                MessageTemplateKey.CUSTOMER_AMENDMENT_REQUESTED => CustomerAmendmentRequested(),
                MessageTemplateKey.AMENDED_REPORT_READY => AmendedReportReady(),

                MessageTemplateKey.CASE_CLOSED => CaseClosed(),

                _ => throw new InvalidOperationException(
                    $"Email template not defined for {key}")
            };
        }

        // =====================================================
        // EMAIL TEMPLATES
        // =====================================================
        private static string SampleInwardAck() => @"
<p>Dear {{CustomerName}},</p>
<p>Your sample has been received successfully.</p>
<p>Case No: <strong>{{CaseNo}}</strong></p>
<p>Regards,<br/>Laboratory Team</p>";

        private static string SampleInwardIncompleteReminder() => @"
<p>Dear {{CustomerName}},</p>
<p>Your sample inward is incomplete for case <strong>{{CaseNo}}</strong>.</p>
<p>Please submit pending details.</p>
<p>Regards,<br/>Laboratory Team</p>";

        private static string SampleInwardUpdated() => @"
<p>Dear {{CustomerName}},</p>
<p>Your sample details have been updated for case <strong>{{CaseNo}}</strong>.</p>
<p>Regards,<br/>Laboratory Team</p>";

        private static string ProformaInvoiceSent() => @"
<p>Dear {{CustomerName}},</p>
<p>Proforma Invoice <strong>{{PINO}}</strong> has been generated.</p>
<p>Regards,<br/>Accounts Team</p>";

        private static string FinalInvoiceGenerated() => @"
<p>Dear {{CustomerName}},</p>
<p>Final Invoice <strong>{{InvoiceNo}}</strong> has been generated.</p>
<p>Regards,<br/>Accounts Team</p>";

        private static string FinalInvoicePostTesting() => @"
<p>Dear {{CustomerName}},</p>
<p>Final Invoice <strong>{{InvoiceNo}}</strong> generated post testing.</p>
<p>Regards,<br/>Accounts Team</p>";

        private static string PaymentLink() => @"
<p>Dear {{CustomerName}},</p>
<p>Payment of ₹{{Amount}} is pending.</p>
<p><a href=""{{PaymentLink}}"">Pay Now</a></p>
<p>Regards,<br/>Accounts Team</p>";

        private static string PaymentReceived() => @"
<p>Dear {{CustomerName}},</p>
<p>Payment received successfully.</p>
<p>Regards,<br/>Accounts Team</p>";

        private static string CustomerAmendmentRequested() => @"
<p>Dear {{CustomerName}},</p>
<p>Your amendment request has been received.</p>
<p>Regards,<br/>Laboratory Team</p>";

        private static string AmendedReportReady() => @"
<p>Dear {{CustomerName}},</p>
<p>Your amended report is ready.</p>
<p>Regards,<br/>Laboratory Team</p>";

        private static string CaseClosed() => @"
<p>Dear {{CustomerName}},</p>
<p>Your case <strong>{{CaseNo}}</strong> has been closed.</p>
<p>Regards,<br/>Laboratory Team</p>";

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
