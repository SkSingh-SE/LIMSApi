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
        /// <summary>
        /// Informational message when price could not be calculated (e.g., no Invoice Case configured)
        /// </summary>
        public string? Message { get; set; }
        public string? SelectedPricingType { get; set; }
        public string? DefaultPricingType { get; set; }
        public List<string> AvailablePricingTypes { get; set; } = new();
        public SampleDimensionsDto? SampleDimensions { get; set; }
        public string? PricingDimensionValue { get; set; }
    }

    /// <summary>
    /// DTO for overriding price on a test result header
    /// </summary>
    public class SetPricingTypeDto
    {
        public string? PricingType { get; set; }
    }

    public class PriceOverrideDto
    {
        public decimal Amount { get; set; }
        public string Reason { get; set; } = string.Empty;
    }

    // ── Smart Pricing Recommendation DTOs ──

    public class SampleDimensionsDto
    {
        public decimal? Thickness { get; set; }
        public decimal? Diameter { get; set; }
        public decimal? Width { get; set; }
        public decimal? Length { get; set; }
    }

    public class PricingTypeRecommendationDto
    {
        public string PricingType { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public int Score { get; set; }
        public string Status { get; set; } = "available";  // ready, needs_input, no_match
        public string Reason { get; set; } = string.Empty;
        public bool IsRecommended { get; set; }
        public decimal? AutoDetectedValue { get; set; }
        public string? ValueSource { get; set; }
        public string? RequiredInput { get; set; }
        public string? InputHint { get; set; }
        public string? Unit { get; set; }
        public int TierCount { get; set; }
        public List<PricingTierPreviewDto> Tiers { get; set; } = new();
    }

    public class PricingTierPreviewDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Value { get; set; }
        public string? Start { get; set; }
        public string? End { get; set; }
        public decimal Price { get; set; }
    }

    public class PricingRecommendationDto
    {
        public long HeaderId { get; set; }
        public string TestName { get; set; } = string.Empty;
        public int BillableParamCount { get; set; }
        public List<PricingTypeRecommendationDto> Recommendations { get; set; } = new();
        public SampleDimensionsDto? SampleDimensions { get; set; }
        public string? CurrentSelectedType { get; set; }
        public string? DefaultPricingType { get; set; }
    }

    public class SetPricingWithValueDto
    {
        public string? PricingType { get; set; }
        public string? DimensionValue { get; set; }  // "12" or "12|500" for SizeLoad
    }
}
