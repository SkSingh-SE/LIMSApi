namespace LIMSApi.Dtos
{
    public class InvoiceLineItemDto
    {
        public long Id { get; set; }
        public long? ProformaInvoiceHeaderID { get; set; }
        public long? TaxInvoiceID { get; set; }
        public long? SampleInwardID { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public decimal TaxPercent { get; set; }
        public string? Remark { get; set; }
    }
}
