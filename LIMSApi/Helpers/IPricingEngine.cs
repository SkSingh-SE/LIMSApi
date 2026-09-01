using LIMSApi.Models;

namespace LIMSApi.Helpers
{
    public interface IPricingEngine
    {
        /// <summary>
        /// Extract parameter value for a config from SpecificationLine or TestResultParameter.
        /// Uses strict name matching (exact → alias → token, no loose Contains).
        /// </summary>
        Task<decimal?> ExtractParameterValueForConfigAsync(
            InvoiceCaseConfiguration config,
            List<SpecificationLine> specificationLines,
            ICollection<TestResultParameter>? testResultParameters,
            string parameterType);

        /// <summary>
        /// Match configuration against parameter value and get rate.
        /// Resolves the date-effective InvoiceCase version via the sample inward date
        /// (greatest EffectiveFrom ≤ inwardDate; falls back to the earliest version).
        /// </summary>
        Task<(decimal Rate, long ConfigId)> MatchConfigAndGetRateAsync(
            long laboratoryTestId,
            InvoiceCaseConfiguration config,
            decimal usedValue,
            DateTime inwardDate,
            long? analysisTypeId = null);

        /// <summary>
        /// Strict name matching: exact → alias exact → token match (3+ chars).
        /// Short names (≤2 chars) only match via exact/alias. No loose Contains().
        /// Always returns parent Config ID reference.
        /// </summary>
        bool StrictMatchesName(string paramNameLower, string configNameLower, string? configAliasLower);
    }
}
