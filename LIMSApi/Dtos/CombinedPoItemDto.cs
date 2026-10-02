namespace LIMSApi.Dtos
{
    public class CombinedPoItemDto
    {
        public string ItemName { get; set; } = "";
        public decimal? OrderedQty { get; set; }
        public string? Unit { get; set; } = "";
        public decimal? UnitPrice { get; set; }
        public decimal? Amount { get; set; }
        public decimal? ReceivedQty { get; set; }
        public string? BatchNo { get; set; } = "";
        public string? LotNo { get; set; } = "";
        public string? InvoiceNo { get; set; } = "";
        public decimal? ApprovedQty { get; set; }
        public decimal? RejectedQty { get; set; }
        public string? VerificationStatus { get; set; } = "";
        public string? VerificationDone { get; set; } = "";
        public string? VerificationDetails { get; set; } = "";
    }
}
