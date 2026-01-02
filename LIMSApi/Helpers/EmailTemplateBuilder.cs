using System;
using System.Text;
using System.Reflection;

namespace LIMSApi.Helpers
{
    public static class EmailTemplateBuilder
    {
        public static string Build(string templateCode, object model)
        {
            var template = GetTemplate(templateCode);
            return ReplacePlaceholders(template, model);
        }

        // =====================================================
        // TEMPLATE REGISTRY (HARDCODED – REPLACE LATER)
        // =====================================================
        private static string GetTemplate(string templateCode)
        {
            return templateCode switch
            {
                "CASE_CREATED" => CaseCreated(),
                "PI_GENERATED" => PiGenerated(),
                "PI_PAID" => PiPaid(),
                "FINAL_INVOICE_GENERATED" => FinalInvoiceGenerated(),
                "CASE_CLOSED" => CaseClosed(),
                "SAMPLE_ADDED" => SampleAdded(),
                "SINGLE_SAMPLE_INVOICE" => SingleSampleInvoice(),
                "SAMPLE_REPORT_DELIVERED" => SampleReportDelivered(),
                "FINAL_INVOICE_POST_TESTING" => FinalInvoicePostTesting(),
                "AMENDMENT_REQUESTED" => AmendmentRequested(),
                "AMENDMENT_DELIVERED" => AmendmentDelivered(),
                "FINAL_REPORT_WITH_AMENDMENT_LINK" => FinalReportWithAmendmentLink(),
                "PAYMENT_LINK" => PaymentLink(),

                _ => throw new InvalidOperationException(
                    $"Email template '{templateCode}' not found.")
            };
        }

        // =====================================================
        // TEMPLATE DEFINITIONS
        // =====================================================

        private static string CaseCreated() => @"
<p>Dear {{CustomerName}},</p>
<p>Your case <strong>{{CaseNo}}</strong> has been successfully created.</p>
<p>We will keep you informed as the case progresses.</p>
<p>Regards,<br/>Laboratory Team</p>";

        private static string PiGenerated() => @"
<p>Dear {{CustomerName}},</p>
<p>A Proforma Invoice <strong>{{PINO}}</strong> has been generated for case <strong>{{CaseNo}}</strong>.</p>
<p>Please find the invoice attached.</p>
<p>Regards,<br/>Accounts Team</p>";

        private static string PiPaid() => @"
<p>Dear {{CustomerName}},</p>
<p>We have received payment for Proforma Invoice <strong>{{PINO}}</strong>.</p>
<p>Thank you for your cooperation.</p>
<p>Regards,<br/>Accounts Team</p>";

        private static string FinalInvoiceGenerated() => @"
<p>Dear {{CustomerName}},</p>
<p>The final invoice <strong>{{InvoiceNo}}</strong> has been generated for case <strong>{{CaseNo}}</strong>.</p>
<p>Please find the invoice attached.</p>
<p>Regards,<br/>Accounts Team</p>";

        private static string CaseClosed() => @"
<p>Dear {{CustomerName}},</p>
<p>Your case <strong>{{CaseNo}}</strong> has been successfully closed.</p>
<p>Thank you for choosing our laboratory services.</p>
<p>Regards,<br/>Laboratory Team</p>";

        private static string SampleAdded() => @"
<p>Dear {{CustomerName}},</p>
<p>A new sample <strong>{{SampleCode}}</strong> has been added to case <strong>{{CaseNo}}</strong>.</p>
<p>We will notify you once testing is completed.</p>
<p>Regards,<br/>Laboratory Team</p>";

        private static string SingleSampleInvoice() => @"
<p>Dear {{CustomerName}},</p>
<p>An invoice <strong>{{InvoiceNo}}</strong> has been generated for sample <strong>{{SampleCode}}</strong>.</p>
<p>Please find the invoice attached.</p>
<p>Regards,<br/>Accounts Team</p>";

        private static string SampleReportDelivered() => @"
<p>Dear {{CustomerName}},</p>
<p>The test report for sample <strong>{{SampleCode}}</strong> is now available.</p>
<p>Please find the report attached.</p>
<p>Regards,<br/>Laboratory Team</p>";

        private static string FinalInvoicePostTesting() => @"
<p>Dear {{CustomerName}},</p>
<p>The final invoice <strong>{{InvoiceNo}}</strong> has been generated after completion of testing for case <strong>{{CaseNo}}</strong>.</p>
<p>Please find the invoice attached.</p>
<p>Regards,<br/>Accounts Team</p>";

        private static string AmendmentRequested() => @"
<p>Dear {{CustomerName}},</p>
<p>Your amendment request for report <strong>{{ReportNo}}</strong> has been received.</p>
<p>Our team will review the request and update you shortly.</p>
<p>Regards,<br/>Laboratory Team</p>";

        private static string AmendmentDelivered() => @"
<p>Dear {{CustomerName}},</p>
<p>The amended report for <strong>{{ReportNo}}</strong> has been completed.</p>
<p>Please find the amended report attached.</p>
<p>Regards,<br/>Laboratory Team</p>";

        private static string FinalReportWithAmendmentLink() => @"
<p>Dear {{CustomerName}},</p>

<p>
Please find attached the final test report
<strong>{{ReportNo}}</strong>.
</p>

<p>
If you require any amendments (typographical corrections,
clarifications, or regulatory wording changes), you may submit
a request using the secure link below:
</p>

<p>
<a href=""{{AmendmentLink}}"">Request Report Amendment</a>
</p>

<p>
This link is valid for 7 days.
</p>

<p>
Regards,<br/>
Laboratory Team
</p>";

        private static string PaymentLink() => @"
<p>Dear {{CustomerName}},</p>

<p>
Payment of <strong>₹{{Amount}}</strong> is pending for
<strong>{{ReferenceText}}</strong>.
</p>

<p>
Please complete the payment using the link below:
</p>

<p>
<a href=""{{PaymentLink}}"">Pay Now</a>
</p>

<p>
Regards,<br/>
Accounts Team
</p>";


        // =====================================================
        // PLACEHOLDER ENGINE (COMMON FOR ALL TEMPLATES)
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
