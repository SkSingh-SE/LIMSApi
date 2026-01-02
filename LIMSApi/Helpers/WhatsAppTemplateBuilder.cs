using System;
using System.Text;
using System.Reflection;

namespace LIMSApi.Helpers
{
    public static class WhatsAppTemplateBuilder
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
                    $"WhatsApp template '{templateCode}' not found.")
            };
        }

        // =====================================================
        // TEMPLATE DEFINITIONS (TEXT ONLY)
        // =====================================================

        private static string CaseCreated() =>
@"Dear {{CustomerName}},
Your case {{CaseNo}} has been successfully created.
We will keep you informed as it progresses.
- Laboratory Team";

        private static string PiGenerated() =>
@"Dear {{CustomerName}},
Proforma Invoice {{PINO}} has been generated for case {{CaseNo}}.
Please check your email for details.
- Accounts Team";

        private static string PiPaid() =>
@"Dear {{CustomerName}},
Payment for Proforma Invoice {{PINO}} has been received.
Thank you.
- Accounts Team";

        private static string FinalInvoiceGenerated() =>
@"Dear {{CustomerName}},
Final Invoice {{InvoiceNo}} has been generated for case {{CaseNo}}.
Please check your email.
- Accounts Team";

        private static string CaseClosed() =>
@"Dear {{CustomerName}},
Your case {{CaseNo}} has been successfully closed.
Thank you for choosing our laboratory services.
- Laboratory Team";

        private static string SampleAdded() =>
@"Dear {{CustomerName}},
Sample {{SampleCode}} has been added to case {{CaseNo}}.
We will update you once testing is completed.
- Laboratory Team";

        private static string SingleSampleInvoice() =>
@"Dear {{CustomerName}},
Invoice {{InvoiceNo}} has been generated for sample {{SampleCode}}.
Please check your email.
- Accounts Team";

        private static string SampleReportDelivered() =>
@"Dear {{CustomerName}},
Test report for sample {{SampleCode}} is now available.
Please check your email.
- Laboratory Team";

        private static string FinalInvoicePostTesting() =>
@"Dear {{CustomerName}},
Final invoice {{InvoiceNo}} has been generated after testing for case {{CaseNo}}.
Please check your email.
- Accounts Team";

        private static string AmendmentRequested() =>
@"Dear {{CustomerName}},
Your amendment request for report {{ReportNo}} has been received.
Our team will review and update you.
- Laboratory Team";

        private static string AmendmentDelivered() =>
@"Dear {{CustomerName}},
The amended report for {{ReportNo}} is now available.
Please check your email.
- Laboratory Team";

        private static string FinalReportWithAmendmentLink() =>
@"Dear {{CustomerName}},
Your final test report {{ReportNo}} has been sent to your email.

To request an amendment, use the secure link below:
{{AmendmentLink}}

Link valid for 7 days.
- Laboratory Team";

        private static string PaymentLink() =>
@"Dear {{CustomerName}},
Payment of ₹{{Amount}} is pending for {{ReferenceText}}.

Please complete payment using the link below:
{{PaymentLink}}

Thank you.
- Accounts Team";

        // =====================================================
        // PLACEHOLDER ENGINE (COMMON)
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
