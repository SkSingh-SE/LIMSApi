namespace LIMSApi.Dtos
{
    /// <summary>
    /// Breakdown of price per parameter for a test result header
    /// </summary>
    public class PriceBreakdownDto
    {
        public long ParameterId { get; set; }
        public string ParameterName { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; } = 1;
        public decimal Amount { get; set; }
    }

    /// <summary>
    /// Summary of calculated and override prices for a test result header
    /// </summary>
    public class PriceSummaryDto
    {
        public long HeaderId { get; set; }
        public decimal CalculatedPrice { get; set; }
        public decimal? OverridePrice { get; set; }
        public decimal FinalPrice { get; set; }
        public bool IsOverridden { get; set; }
        public string? OverrideReason { get; set; }
        public string? OverrideByName { get; set; }
        public List<PriceBreakdownDto> Breakdown { get; set; } = new();
    }

    /// <summary>
    /// DTO for overriding price on a test result header
    /// </summary>
    public class PriceOverrideDto
    {
        public decimal Amount { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}
