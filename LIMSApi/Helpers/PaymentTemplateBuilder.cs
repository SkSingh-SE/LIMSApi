using LIMSApi.Helpers.Enums;

namespace LIMSApi.Helpers
{
    public static class PaymentTemplateBuilder
    {
        // =========================
        // EMAIL TEMPLATES
        // =========================
        public static (string Subject, string Body) BuildEmailTemplate(
            PaymentType paymentType,
            string customerName,
            string invoiceNo,
            decimal amount,
            string paymentLink)
        {
            return paymentType switch
            {
                PaymentType.PIInvoice => BuildPIEmail(customerName, invoiceNo, amount, paymentLink),
                PaymentType.Invoice => BuildInvoiceEmail(customerName, invoiceNo, amount, paymentLink),
                PaymentType.AmendmentInvoice => BuildAmendmentEmail(customerName, invoiceNo, amount, paymentLink),
                _ => BuildGenericEmail(customerName, invoiceNo, amount, paymentLink)
            };
        }

        // =========================
        // WHATSAPP / SMS TEMPLATES
        // =========================
        public static string BuildWhatsAppTemplate(
            PaymentType paymentType,
            string customerName,
            string invoiceNo,
            decimal amount,
            string paymentLink)
        {
            return paymentType switch
            {
                PaymentType.PIInvoice => BuildPIWhatsApp(customerName, invoiceNo, amount, paymentLink),
                PaymentType.Invoice => BuildInvoiceWhatsApp(customerName, invoiceNo, amount, paymentLink),
                PaymentType.AmendmentInvoice => BuildAmendmentWhatsApp(customerName, invoiceNo, amount, paymentLink),
                _ => BuildGenericWhatsApp(customerName, invoiceNo, amount, paymentLink)
            };
        }

        // =========================
        // EMAIL VARIANTS
        // =========================
        private static (string, string) BuildPIEmail(string name, string inv, decimal amt, string link)
        {
            return (
                $"Proforma Invoice Payment Required – {inv}",
                $"""
                <p>Dear {name},</p>
                <p>Please complete the advance payment for the Proforma Invoice <b>{inv}</b>.</p>
                <p><b>Amount:</b> ₹{amt}</p>
                <p>
                    <a href="{link}" style="background:#dc3545;color:#fff;padding:10px 16px;text-decoration:none;">
                        Pay Now
                    </a>
                </p>
                <p>Regards,<br/>Accounts Team</p>
                """
            );
        }

        private static (string, string) BuildInvoiceEmail(string name, string inv, decimal amt, string link)
        {
            return (
                $"Payment Required – Invoice {inv}",
                $"""
                <p>Dear {name},</p>
                <p>Your test report is ready. Please complete the payment to release the report.</p>
                <p><b>Invoice:</b> {inv}<br/>
                   <b>Amount:</b> ₹{amt}</p>
                <p>
                    <a href="{link}" style="background:#dc3545;color:#fff;padding:10px 16px;text-decoration:none;">
                        Pay Securely
                    </a>
                </p>
                <p>Regards,<br/>Accounts Team</p>
                """
            );
        }

        private static (string, string) BuildAmendmentEmail(string name, string inv, decimal amt, string link)
        {
            return (
                $"Amendment Charges – {inv}",
                $"""
                <p>Dear {name},</p>
                <p>An amendment has been requested for your report.</p>
                <p><b>Invoice:</b> {inv}<br/>
                   <b>Amendment Charges:</b> ₹{amt}</p>
                <p>
                    <a href="{link}" style="background:#dc3545;color:#fff;padding:10px 16px;text-decoration:none;">
                        Pay Amendment Charges
                    </a>
                </p>
                <p>Regards,<br/>Accounts Team</p>
                """
            );
        }

        private static (string, string) BuildGenericEmail(string name, string inv, decimal amt, string link)
        {
            return (
                $"Payment Required – {inv}",
                $"Dear {name},<br/>Please complete payment of ₹{amt}.<br/><a href='{link}'>Pay Now</a>"
            );
        }

        // =========================
        // WHATSAPP VARIANTS
        // =========================
        private static string BuildPIWhatsApp(string name, string inv, decimal amt, string link) =>
            $"Dear {name},\n\nProforma Invoice {inv}\nAmount: ₹{amt}\n\nPay here:\n{link}\n\n– Accounts Team";

        private static string BuildInvoiceWhatsApp(string name, string inv, decimal amt, string link) =>
            $"Dear {name},\n\nInvoice {inv}\nAmount: ₹{amt}\n\nPay to release report:\n{link}\n\n– Accounts Team";

        private static string BuildAmendmentWhatsApp(string name, string inv, decimal amt, string link) =>
            $"Dear {name},\n\nAmendment Invoice {inv}\nCharges: ₹{amt}\n\nPay here:\n{link}\n\n– Accounts Team";

        private static string BuildGenericWhatsApp(string name, string inv, decimal amt, string link) =>
            $"Dear {name},\nPayment {inv}\nAmount ₹{amt}\n{link}";
    }
}
