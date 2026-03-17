using LIMSApi.Dtos;

namespace LIMSApi.ServiceWORepo
{
    /// <summary>
    /// Test-level price calculation service.
    /// Calculates price for individual TestResultHeader based on parameters × InvoiceCasePrice.
    /// </summary>
    public interface ITestPriceCalculationService
    {
        /// <summary>
        /// Calculate test price based on selected parameters × their price from InvoiceCasePrice.
        /// Updates TestResultHeader.CalculatedPrice.
        /// </summary>
        Task<PriceSummaryDto> CalculateTestPrice(long headerId);

        /// <summary>
        /// Get price breakdown: list of parameter name, unit price, quantity, amount.
        /// </summary>
        Task<List<PriceBreakdownDto>> GetPriceBreakdown(long headerId);

        /// <summary>
        /// Override the calculated price with a manual amount and reason.
        /// </summary>
        Task<PriceSummaryDto> OverridePrice(long headerId, decimal amount, string reason, long overrideById);

        /// <summary>
        /// Get price summary: calculated, override, final, and override info.
        /// </summary>
        Task<PriceSummaryDto> GetPriceSummary(long headerId);
    }
}
