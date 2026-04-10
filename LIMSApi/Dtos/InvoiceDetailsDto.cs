namespace LIMSApi.Dtos
{
    public class InvoiceDetailsDto
    {
        // Invoice header
        public long InvoiceId { get; set; }
        public string InvoiceNo { get; set; } = "";
        public DateTime InvoiceDate { get; set; }
        public string Status { get; set; } = "";
        public string SACCode { get; set; } = "998346";
        public string? PaymentTerms { get; set; }
        public DateTime? PaymentDueDate { get; set; }
        public string? PdfPath { get; set; }

        // Customer
        public long CustomerId { get; set; }
        public string CustomerName { get; set; } = "";
        public string? CustomerAddress { get; set; }
        public string? CustomerGSTIN { get; set; }
        public string? CustomerState { get; set; }

        // Case
        public long InwardId { get; set; }
        public string CaseNo { get; set; } = "";

        // Financial
        public decimal SubTotal { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TaxableAmount { get; set; }
        public decimal CGST { get; set; }
        public decimal SGST { get; set; }
        public decimal IGST { get; set; }
        public decimal GrandTotal { get; set; }
        public decimal AdvanceAdjusted { get; set; }
        public decimal BalancePayable { get; set; }

        // PO
        public string? PurchaseOrderNo { get; set; }

        // Line items
        public List<InvoiceChargeLineDto> ChargeLines { get; set; } = new();
        public List<InvoiceAdHocLineDto> AdHocLines { get; set; } = new();
    }

    public class InvoiceChargeLineDto
    {
        public long Id { get; set; }
        public string ChargeType { get; set; } = "";
        public string Description { get; set; } = "";
        public decimal Amount { get; set; }
    }

    public class InvoiceAdHocLineDto
    {
        public long Id { get; set; }
        public string Description { get; set; } = "";
        public decimal Amount { get; set; }
        public decimal TaxPercent { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
