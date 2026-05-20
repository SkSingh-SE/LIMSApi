using LIMSApi.Data;
using LIMSApi.Models;
using LIMSApi.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Helpers
{
    /// <summary>
    /// Shared pricing engine — single source of truth for parameter→config matching and rate lookup.
    /// Used by PriceCalculationService, ProformaInvoiceRepository, and TestPriceCalculationService.
    /// Incorporates: strict name matching, FY filtering, correct SizeLoad direction, rounding.
    /// </summary>
    public class PricingEngine : IPricingEngine
    {
        private readonly LIMSContext _db;
        private readonly IFinancialYearService _fyService;
        private readonly ILogger<PricingEngine> _logger;

        private static readonly char[] NameSeparators = { ' ', '-', '_' };

        private static readonly HashSet<string> RangeSelectionTypes = new(StringComparer.OrdinalIgnoreCase)
        {
            "HoursRange", "SizeRange", "WeightRange", "TempratureRange", "LoadRange"
        };

        public PricingEngine(LIMSContext db, IFinancialYearService fyService, ILogger<PricingEngine> logger)
        {
            _db = db;
            _fyService = fyService;
            _logger = logger;
        }

        /// <inheritdoc />
        public bool StrictMatchesName(string paramNameLower, string configNameLower, string? configAliasLower)
        {
            // 1. Exact match
            if (paramNameLower == configNameLower)
                return true;

            // 2. Alias exact match (comma-separated)
            if (!string.IsNullOrWhiteSpace(configAliasLower))
            {
                var aliases = configAliasLower.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(a => a.Trim());
                if (aliases.Any(alias => alias == paramNameLower))
                    return true;
            }

            // 3. Short names (≤2 chars like chemical elements C, Mn, Si) — only exact/alias
            if (paramNameLower.Length <= 2 || configNameLower.Length <= 2)
                return false;

            // 4. Token-based match (split by space/hyphen/underscore, match exact tokens)
            var paramTokens = paramNameLower.Split(NameSeparators, StringSplitOptions.RemoveEmptyEntries);
            var configTokens = configNameLower.Split(NameSeparators, StringSplitOptions.RemoveEmptyEntries);

            if (paramTokens.Any(pt => pt.Length >= 3 && configTokens.Contains(pt)))
                return true;

            return false;
        }

        /// <inheritdoc />
        public Task<decimal?> ExtractParameterValueForConfigAsync(
            InvoiceCaseConfiguration config,
            List<SpecificationLine> specificationLines,
            ICollection<TestResultParameter>? testResultParameters,
            string parameterType)
        {
            var configNameLower = config.Name.ToLower().Trim();
            var configAliasLower = config.AliasName?.ToLower().Trim();

            // First, try TestResultParameter (actual test results) — only billable ones
            if (testResultParameters != null && testResultParameters.Any())
            {
                var billableParams = testResultParameters.Where(trp => trp.IsBillable).ToList();
                var parameterIds = billableParams.Select(trp => trp.ParameterID).Distinct().ToList();
                var parameterMasters = _db.ParameterMasters
                    .Where(pm => parameterIds.Contains(pm.ID))
                    .ToDictionary(pm => pm.ID);

                foreach (var trp in billableParams)
                {
                    if (!parameterMasters.TryGetValue(trp.ParameterID, out var paramMaster))
                        continue;

                    var paramName = paramMaster.Name?.ToLower().Trim() ?? "";
                    var paramAlias = paramMaster.AliasName?.ToLower().Trim() ?? "";
                    var trpParamName = trp.ParameterName?.ToLower().Trim() ?? "";

                    // Strict matching: try each name variant against config
                    bool matched = StrictMatchesName(paramName, configNameLower, configAliasLower)
                                || (!string.IsNullOrEmpty(paramAlias) && StrictMatchesName(paramAlias, configNameLower, configAliasLower))
                                || (!string.IsNullOrEmpty(trpParamName) && StrictMatchesName(trpParamName, configNameLower, configAliasLower));

                    if (matched && trp.Value.HasValue)
                    {
                        _logger.LogDebug("Matched parameter '{ParamName}' to config '{ConfigName}' via strict matching",
                            trpParamName, config.Name);
                        return Task.FromResult<decimal?>(trp.Value.Value);
                    }
                }
            }

            // Fallback: try SpecificationLine
            foreach (var specLine in specificationLines)
            {
                if (specLine.Parameter == null) continue;

                var paramName = specLine.Parameter.Name?.ToLower().Trim() ?? "";
                var paramAlias = specLine.Parameter.AliasName?.ToLower().Trim() ?? "";

                bool matched = StrictMatchesName(paramName, configNameLower, configAliasLower)
                            || (!string.IsNullOrEmpty(paramAlias) && StrictMatchesName(paramAlias, configNameLower, configAliasLower));

                if (matched)
                {
                    if (specLine.MinValue.HasValue)
                        return Task.FromResult<decimal?>(specLine.MinValue.Value);
                    if (specLine.MaxValue.HasValue)
                        return Task.FromResult<decimal?>(specLine.MaxValue.Value);
                }
            }

            return Task.FromResult<decimal?>(null);
        }

        /// <inheritdoc />
        public async Task<(decimal Rate, long ConfigId)> MatchConfigAndGetRateAsync(
            long laboratoryTestId,
            InvoiceCaseConfiguration config,
            decimal usedValue,
            DateTime inwardDate)
        {
            // Date-effective InvoiceCase resolution: pick the version with the greatest
            // EffectiveFrom ≤ the sample inward date, falling back to the earliest version.
            var versions = await _db.InvoiceCases
                .Where(x => x.LaboratoryTestID == laboratoryTestId && x.IsActive)
                .Include(x => x.InvoiceCasePrices)
                .Include(x => x.FinancialYearEntity)
                .ToListAsync();

            var invoiceCase = PriceVersionResolver.Resolve(versions, v => v.EffectiveFrom, inwardDate);

            if (invoiceCase == null)
                throw new Exception($"No invoice case found for LaboratoryTest {laboratoryTestId}");

            if (invoiceCase.EffectiveFrom.Date > inwardDate.Date)
                _logger.LogWarning("Sample inward date {InwardDate:d} precedes all InvoiceCase versions for LabTest {LabTestId}. Using earliest version (FY {FY}).",
                    inwardDate, laboratoryTestId, invoiceCase.FinancialYearEntity?.Year);

            var configPrice = invoiceCase.InvoiceCasePrices
                .FirstOrDefault(p => p.InvoiceCaseConfigID == config.ID);

            if (configPrice == null)
                throw new Exception($"Configuration {config.ID} not found in invoice case prices");

            var isRangeType = RangeSelectionTypes.Contains(config.SelectionType);

            if (isRangeType)
            {
                if (string.IsNullOrWhiteSpace(config.Start) || string.IsNullOrWhiteSpace(config.End))
                    throw new Exception($"Configuration {config.ID} is a range type but Start or End is missing");

                var startValue = decimal.Parse(config.Start);
                var endValue = decimal.Parse(config.End);

                if (usedValue >= startValue && usedValue <= endValue)
                {
                    _logger.LogDebug("Range match: {UsedValue} in [{Start}-{End}] → Price {Price}",
                        usedValue, startValue, endValue, configPrice.Price);
                    return (configPrice.Price, config.ID);
                }
            }
            else
            {
                // Slab match: find nearest slab >= usedValue
                var allConfigsForType = await _db.LaboratoryTestInvoiceCase
                    .Where(lt => lt.LabTestID == laboratoryTestId)
                    .Include(lt => lt.InvoiceCaseConfiguration)
                    .Where(lt => lt.InvoiceCaseConfiguration != null
                                && lt.InvoiceCaseConfiguration.IsActive
                                && lt.InvoiceCaseConfiguration.SelectionType == config.SelectionType)
                    .Select(lt => lt.InvoiceCaseConfiguration!)
                    .Where(c => !string.IsNullOrWhiteSpace(c.Value))
                    .ToListAsync();

                var matchingConfig = allConfigsForType
                    .Where(c => decimal.TryParse(c.Value, out var val) && val >= usedValue)
                    .OrderBy(c => decimal.Parse(c.Value))
                    .FirstOrDefault();

                if (matchingConfig != null)
                {
                    var matchingPrice = invoiceCase.InvoiceCasePrices
                        .FirstOrDefault(p => p.InvoiceCaseConfigID == matchingConfig.ID);

                    if (matchingPrice != null)
                    {
                        _logger.LogDebug("Slab match: {UsedValue} → slab {SlabValue} → Price {Price}",
                            usedValue, matchingConfig.Value, matchingPrice.Price);
                        return (matchingPrice.Price, matchingConfig.ID);
                    }
                }
            }

            throw new Exception($"No pricing slab found for value {usedValue} under SelectionType {config.SelectionType}");
        }
    }
}
