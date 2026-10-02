namespace LIMSApi.Dtos
{
    public class CrmConsumptionHeaderDto
    {
        public long Id { get; set; }
        public long? ReferenceMaterialId { get; set; }
        public string? FormNo { get; set; }
        public string? DocumentNo { get; set; }
        public string? IssueNo { get; set; }
        public string? RevNo { get; set; }
        public DateTime RecordDate { get; set; }
        public decimal OpeningQuantity { get; set; }
        public decimal TotalConsumed { get; set; }
        public decimal RemainingQuantity { get; set; }
        public string? Notes { get; set; }
        public string? PreparedBy { get; set; }
        public string? ReviewedBy { get; set; }
        public string? ApprovedBy { get; set; }
    }
}
