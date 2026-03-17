using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Models;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.ServiceWORepo
{
    /// <summary>
    /// Test-level price calculation service.
    /// Calculates price for individual TestResultHeader based on parameters × InvoiceCasePrice.
    /// </summary>
    public class TestPriceCalculationService : ITestPriceCalculationService
    {
        private readonly LIMSContext _db;
        private readonly ILogger<TestPriceCalculationService> _logger;

        public TestPriceCalculationService(LIMSContext db, ILogger<TestPriceCalculationService> logger)
        {
            _db = db;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task<PriceSummaryDto> CalculateTestPrice(long headerId)
        {
            var header = await _db.TestResultHeaders
                .Include(h => h.Parameters)
                .FirstOrDefaultAsync(h => h.ID == headerId);

            if (header == null)
                throw new Exception($"TestResultHeader {headerId} not found");

            // Get InvoiceCase for this LaboratoryTest
            var invoiceCase = await _db.InvoiceCases
                .Where(ic => ic.LaboratoryTestID == header.LaboratoryTestID && ic.IsActive)
                .Include(ic => ic.InvoiceCasePrices)
                    .ThenInclude(p => p.Configuration)
                .FirstOrDefaultAsync();

            decimal calculatedTotal = 0;

            if (invoiceCase != null && invoiceCase.InvoiceCasePrices.Any())
            {
                var breakdown = BuildPriceBreakdown(header.Parameters, invoiceCase.InvoiceCasePrices);
                calculatedTotal = breakdown.Sum(b => b.Amount);
            }

            // Update header
            header.CalculatedPrice = calculatedTotal;
            await _db.SaveChangesAsync();

            _logger.LogInformation(
                "Calculated test price for Header {HeaderId}: {Price}",
                headerId, calculatedTotal);

            return await GetPriceSummary(headerId);
        }

        /// <inheritdoc />
        public async Task<List<PriceBreakdownDto>> GetPriceBreakdown(long headerId)
        {
            var header = await _db.TestResultHeaders
                .Include(h => h.Parameters)
                .FirstOrDefaultAsync(h => h.ID == headerId);

            if (header == null)
                throw new Exception($"TestResultHeader {headerId} not found");

            var invoiceCase = await _db.InvoiceCases
                .Where(ic => ic.LaboratoryTestID == header.LaboratoryTestID && ic.IsActive)
                .Include(ic => ic.InvoiceCasePrices)
                    .ThenInclude(p => p.Configuration)
                .FirstOrDefaultAsync();

            if (invoiceCase == null || !invoiceCase.InvoiceCasePrices.Any())
                return new List<PriceBreakdownDto>();

            return BuildPriceBreakdown(header.Parameters, invoiceCase.InvoiceCasePrices);
        }

        /// <inheritdoc />
        public async Task<PriceSummaryDto> OverridePrice(long headerId, decimal amount, string reason, long overrideById)
        {
            var header = await _db.TestResultHeaders
                .FirstOrDefaultAsync(h => h.ID == headerId);

            if (header == null)
                throw new Exception($"TestResultHeader {headerId} not found");

            header.OverridePrice = amount;
            header.OverrideReason = reason;
            header.OverrideById = overrideById;
            header.PriceOverridden = true;

            await _db.SaveChangesAsync();

            _logger.LogInformation(
                "Price overridden for Header {HeaderId}: Override={Amount}, Reason={Reason}",
                headerId, amount, reason);

            return await GetPriceSummary(headerId);
        }

        /// <inheritdoc />
        public async Task<PriceSummaryDto> GetPriceSummary(long headerId)
        {
            var header = await _db.TestResultHeaders
                .Include(h => h.Parameters)
                .FirstOrDefaultAsync(h => h.ID == headerId);

            if (header == null)
                throw new Exception($"TestResultHeader {headerId} not found");

            var calculatedPrice = header.CalculatedPrice ?? 0;
            var overridePrice = header.OverridePrice;
            var isOverridden = header.PriceOverridden;
            var finalPrice = isOverridden && overridePrice.HasValue ? overridePrice.Value : calculatedPrice;

            // Get override user name if applicable
            string? overrideByName = null;
            if (header.OverrideById.HasValue)
            {
                var user = await _db.UserMasters.FirstOrDefaultAsync(u => u.ID == header.OverrideById.Value);
                overrideByName = user?.UserName;
            }

            // Build breakdown
            var breakdown = new List<PriceBreakdownDto>();
            var invoiceCase = await _db.InvoiceCases
                .Where(ic => ic.LaboratoryTestID == header.LaboratoryTestID && ic.IsActive)
                .Include(ic => ic.InvoiceCasePrices)
                    .ThenInclude(p => p.Configuration)
                .FirstOrDefaultAsync();

            if (invoiceCase != null && invoiceCase.InvoiceCasePrices.Any())
            {
                breakdown = BuildPriceBreakdown(header.Parameters, invoiceCase.InvoiceCasePrices);
            }

            return new PriceSummaryDto
            {
                HeaderId = headerId,
                CalculatedPrice = calculatedPrice,
                OverridePrice = overridePrice,
                FinalPrice = finalPrice,
                IsOverridden = isOverridden,
                OverrideReason = header.OverrideReason,
                OverrideByName = overrideByName,
                Breakdown = breakdown
            };
        }

        /// <summary>
        /// Build price breakdown using 4-group priority:
        /// 1. Element count pricing (chemical tests — count of billable parameters as slab value)
        /// 2. Dimensional pricing (range/slab — Load, Temperature, Size, Hours, etc.)
        /// 3. Name-based matching (parameter name vs InvoiceCasePrice name/alias)
        /// 4. Flat-rate fallback (single InvoiceCasePrice entry used as flat fee)
        /// Only IsBillable parameters participate in pricing.
        /// </summary>
        private List<PriceBreakdownDto> BuildPriceBreakdown(
            ICollection<TestResultParameter> parameters,
            ICollection<InvoiceCasePrice> prices)
        {
            var breakdown = new List<PriceBreakdownDto>();

            // Only billable parameters participate in pricing
            var billableParams = parameters.Where(p => p.IsBillable).ToList();

            // GROUP 1: Element count pricing (auto-detect for chemical tests)
            var elementPrices = prices.Where(p =>
                p.Configuration != null &&
                string.Equals(p.Configuration.SelectionType, "Element", StringComparison.OrdinalIgnoreCase)).ToList();

            if (elementPrices.Any())
            {
                var elementCount = billableParams.Count;
                // Slab match: find nearest slab >= elementCount
                var matched = elementPrices
                    .Where(p => decimal.TryParse(p.Configuration!.Value, out var v) && v >= elementCount)
                    .OrderBy(p => decimal.Parse(p.Configuration!.Value))
                    .FirstOrDefault();

                if (matched != null)
                {
                    breakdown.Add(new PriceBreakdownDto
                    {
                        ParameterId = 0,
                        ParameterName = $"Chemical Analysis ({elementCount} elements)",
                        UnitPrice = matched.Price,
                        Quantity = 1,
                        Amount = matched.Price
                    });
                }

                // When Element pricing is used, skip name-based matching to avoid double-charge
                if (breakdown.Any()) return breakdown;
                return BuildFlatRateFallback(prices);
            }

            // GROUP 2: Dimensional pricing (prices with Config that has a SelectionType)
            var dimensionalPrices = prices.Where(p =>
                p.Configuration != null &&
                !string.IsNullOrWhiteSpace(p.Configuration.SelectionType)).ToList();

            if (dimensionalPrices.Any())
            {
                var groups = dimensionalPrices.GroupBy(p => p.Configuration!.SelectionType);

                foreach (var group in groups)
                {
                    var selectionType = group.Key!;
                    var isRange = selectionType.EndsWith("Range", StringComparison.OrdinalIgnoreCase);

                    // Find the parameter value that matches this dimension
                    decimal? paramValue = FindParameterValueForDimension(billableParams, group.First().Configuration!);
                    if (paramValue == null) continue;

                    InvoiceCasePrice? matched = null;
                    if (isRange)
                    {
                        // Range: find where paramValue falls between Start and End
                        matched = group.FirstOrDefault(p =>
                        {
                            var cfg = p.Configuration!;
                            return decimal.TryParse(cfg.Start, out var s) &&
                                   decimal.TryParse(cfg.End, out var e) &&
                                   paramValue >= s && paramValue <= e;
                        });
                    }
                    else
                    {
                        // Slab: find nearest slab >= paramValue
                        matched = group
                            .Where(p => decimal.TryParse(p.Configuration!.Value, out var v) && v >= paramValue)
                            .OrderBy(p => decimal.Parse(p.Configuration!.Value))
                            .FirstOrDefault();
                    }

                    if (matched != null)
                    {
                        var unit = matched.Configuration?.Unit ?? "";
                        breakdown.Add(new PriceBreakdownDto
                        {
                            ParameterId = 0,
                            ParameterName = $"{matched.Configuration!.Name} ({paramValue} {unit})".Trim(),
                            UnitPrice = matched.Price,
                            Quantity = 1,
                            Amount = matched.Price
                        });
                    }
                }
            }

            // GROUP 3: Name-based matching (prices WITHOUT dimensional config)
            var flatPrices = prices.Where(p =>
                p.Configuration == null ||
                string.IsNullOrWhiteSpace(p.Configuration.SelectionType)).ToList();

            foreach (var param in billableParams)
            {
                var paramNameLower = (param.ParameterName ?? "").Trim().ToLower();
                if (string.IsNullOrWhiteSpace(paramNameLower))
                    continue;

                var matchedPrice = flatPrices.FirstOrDefault(p =>
                    MatchesName(paramNameLower, p.Name, p.AliasName));

                if (matchedPrice != null)
                {
                    breakdown.Add(new PriceBreakdownDto
                    {
                        ParameterId = param.ParameterID,
                        ParameterName = param.ParameterName,
                        UnitPrice = matchedPrice.Price,
                        Quantity = 1,
                        Amount = matchedPrice.Price
                    });
                }
            }

            // GROUP 4: Flat-rate fallback
            if (!breakdown.Any())
            {
                breakdown = BuildFlatRateFallback(prices);
            }

            return breakdown;
        }

        /// <summary>
        /// Find parameter value for a dimensional pricing configuration.
        /// Matches config Name/AliasName against parameter names and returns the parameter's value.
        /// </summary>
        private decimal? FindParameterValueForDimension(
            List<TestResultParameter> billableParams,
            InvoiceCaseConfiguration config)
        {
            var configNames = new List<string> { config.Name.ToLower() };
            if (!string.IsNullOrWhiteSpace(config.AliasName))
            {
                configNames.AddRange(config.AliasName
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(a => a.Trim().ToLower()));
            }

            foreach (var param in billableParams)
            {
                var paramName = (param.ParameterName ?? "").Trim().ToLower();
                if (string.IsNullOrWhiteSpace(paramName)) continue;

                if (configNames.Any(cn => cn == paramName || cn.Contains(paramName) || paramName.Contains(cn)))
                {
                    if (param.Value.HasValue) return param.Value.Value;
                }
            }
            return null;
        }

        /// <summary>
        /// Flat-rate fallback: if exactly 1 InvoiceCasePrice exists, use it as a flat fee.
        /// </summary>
        private List<PriceBreakdownDto> BuildFlatRateFallback(ICollection<InvoiceCasePrice> prices)
        {
            if (prices.Count == 1)
            {
                var singlePrice = prices.First();
                return new List<PriceBreakdownDto>
                {
                    new PriceBreakdownDto
                    {
                        ParameterId = 0,
                        ParameterName = singlePrice.Name ?? "Test Fee",
                        UnitPrice = singlePrice.Price,
                        Quantity = 1,
                        Amount = singlePrice.Price
                    }
                };
            }
            return new List<PriceBreakdownDto>();
        }

        /// <summary>
        /// Check if a parameter name matches an InvoiceCasePrice Name or AliasName (case-insensitive).
        /// </summary>
        private bool MatchesName(string paramNameLower, string priceName, string priceAlias)
        {
            var priceNameLower = (priceName ?? "").Trim().ToLower();
            var priceAliasLower = (priceAlias ?? "").Trim().ToLower();

            // Exact match
            if (paramNameLower == priceNameLower)
                return true;

            // Check alias (comma-separated)
            if (!string.IsNullOrWhiteSpace(priceAliasLower))
            {
                var aliases = priceAliasLower.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(a => a.Trim());

                if (aliases.Any(alias => alias == paramNameLower))
                    return true;
            }

            // Partial match (contains)
            if (priceNameLower.Contains(paramNameLower) || paramNameLower.Contains(priceNameLower))
                return true;

            return false;
        }
    }
}
