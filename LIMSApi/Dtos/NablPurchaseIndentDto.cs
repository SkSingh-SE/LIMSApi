namespace LIMSApi.Dtos
{
    public class NablPurchaseIndentDto
    {
        public long Id { get; set; }
        public string? Description { get; set; }
        public string? TechnicalSpecification { get; set; }
        public string? IndentorName { get; set; }
        public string? UnitOfMeasure { get; set; }
        public string? PurchaseIndentNo { get; set; }
        public int? Quantity { get; set; }
        public DateTime? ExpectedDate { get; set; }
        public string? Priority { get; set; }
    }
}
