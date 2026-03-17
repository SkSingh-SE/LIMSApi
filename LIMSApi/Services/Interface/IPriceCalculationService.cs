using LIMSApi.Dtos;

namespace LIMSApi.Services.Interface
{
    /// <summary>
    /// PriceCalculationService: Generic price calculation and ChargeEvent generation
    /// Invoked AFTER testing or partial testing
    /// </summary>
    public interface IPriceCalculationService
    {
        /// <summary>
        /// Calculate prices for a case and create ChargeEvents with DRAFT status
        /// Returns detailed result with per-test success/failure info
        /// </summary>
        Task<PriceCalculationResultDto> CalculateAndCreateChargeEventsAsync(long inwardId);

        /// <summary>
        /// Validate pricing without saving — dry run returning what would succeed/fail
        /// </summary>
        Task<PriceCalculationResultDto> ValidatePricingAsync(long inwardId);

        /// <summary>
        /// Get total amount from DRAFT ChargeEvents for a case
        /// </summary>
        Task<decimal> GetDraftTotalAsync(long inwardId);

        /// <summary>
        /// Get all ChargeEvents for a case
        /// </summary>
        Task<List<Models.ChargeEvent>> GetChargeEventsAsync(long inwardId, string? status = null);

        /// <summary>
        /// Create price snapshot: Move DRAFT ChargeEvents to SNAPSHOT
        /// Sets BillingStatus = PRICE_SNAPSHOT
        /// Must be called before invoice generation
        /// </summary>
        Task CreatePriceSnapshotAsync(long inwardId);
    }
}
