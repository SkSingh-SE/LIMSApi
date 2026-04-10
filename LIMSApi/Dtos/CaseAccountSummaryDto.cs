namespace LIMSApi.Dtos
{
    public class CaseAccountSummaryDto
    {
        public long InwardID { get; set; }
        public string CaseNo { get; set; } = null!;
        public long CustomerId { get; set; }
        public string CustomerName { get; set; } = null!;
        public string CustomerType { get; set; } = null!;
        public string PIStatus { get; set; } = null!;
        public string InvoiceStatus { get; set; } = null!;
        public string BillingStatus { get; set; } = "";
        public string ReportStatus { get; set; } = "";
        public bool HasPendingPayment { get; set; }

        // PI details (null if not generated)
        public ProformaInvoiceSummary? ProformaInvoice { get; set; }

        // Final Invoice details (null if not generated)
        public InvoiceSummary? FinalInvoice { get; set; }

        // Advance payments
        public List<AdvancePaymentSummary> AdvancePayments { get; set; } = new();
    }

    public class ProformaInvoiceSummary
    {
        public long Id { get; set; }
        public string PiNo { get; set; } = string.Empty;
        public DateTime PiDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal SubTotal { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal GrandTotal { get; set; }
        public decimal Amount { get; set; }
    }

    public class InvoiceSummary
    {
        public long InvoiceId { get; set; }
        public string InvoiceNo { get; set; } = string.Empty;
        public DateTime InvoiceDate { get; set; }
        public decimal SubTotal { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal Amount { get; set; }
        public decimal GrandTotal { get; set; }
    }

    public class AdvancePaymentSummary
    {
        public long Id { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string PaymentMode { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }

}
