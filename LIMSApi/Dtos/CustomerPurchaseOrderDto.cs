namespace LIMSApi.Dtos
{
    public class CustomerPurchaseOrderDto
    {
        public long Id { get; set; }
        public long CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string PONumber { get; set; } = string.Empty;
        public DateTime PODate { get; set; }
        public DateTime? ValidUntil { get; set; }
        public decimal POAmount { get; set; }
        public decimal UtilizedAmount { get; set; }
        public decimal RemainingAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Terms { get; set; }
        public string? FilePath { get; set; }
        public string? FileName { get; set; }
    }

    public class CreateCustomerPurchaseOrderDto
    {
        public long CustomerId { get; set; }
        public string PONumber { get; set; } = string.Empty;
        public DateTime PODate { get; set; }
        public DateTime? ValidUntil { get; set; }
        public decimal POAmount { get; set; }
        public string? Terms { get; set; }
        public string? FilePath { get; set; }
        public string? FileName { get; set; }
        public long? UploadReferenceID { get; set; }
    }

    public class POUtilizationDto
    {
        public long POId { get; set; }
        public string PONumber { get; set; } = string.Empty;
        public decimal POAmount { get; set; }
        public decimal UtilizedAmount { get; set; }
        public decimal RemainingAmount { get; set; }
        public List<POInvoiceLineDto> Invoices { get; set; } = new();
    }

    public class POInvoiceLineDto
    {
        public long InvoiceId { get; set; }
        public string InvoiceNo { get; set; } = string.Empty;
        public DateTime InvoiceDate { get; set; }
        public decimal Amount { get; set; }
    }
}
