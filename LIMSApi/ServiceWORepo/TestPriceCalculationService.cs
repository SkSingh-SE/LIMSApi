using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Services.Interface;
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
        private readonly IFinancialYearService _fyService;

        public TestPriceCalculationService(LIMSContext db, ILogger<TestPriceCalculationService> logger, IFinancialYearService fyService)
        {
            _db = db;
            _logger = logger;
            _fyService = fyService;
        }

        /// <summary>
        /// Get InvoiceCase for a lab test with FY filtering.
        /// Tries current FY first, falls back to any active case if no FY match.
        /// </summary>
        private async Task<InvoiceCase?> GetInvoiceCaseForTestAsync(long laboratoryTestId)
        {
            var currentFY = await _fyService.GetCurrentFinancialYearAsync();

            // Try current FY first
            var invoiceCase = await _db.InvoiceCases
                .Where(ic => ic.LaboratoryTestID == laboratoryTestId && ic.IsActive && ic.FinancialYear == currentFY)
                .Include(ic => ic.InvoiceCasePrices)
                    .ThenInclude(p => p.Configuration)
                .FirstOrDefaultAsync();

            if (invoiceCase != null) return invoiceCase;

            // Fallback: any active case (log warning)
            invoiceCase = await _db.InvoiceCases
                .Where(ic => ic.LaboratoryTestID == laboratoryTestId && ic.IsActive)
                .Include(ic => ic.InvoiceCasePrices)
                    .ThenInclude(p => p.Configuration)
                .FirstOrDefaultAsync();

            if (invoiceCase != null)
                _logger.LogWarning("No InvoiceCase for FY {FY} and LabTest {LabTestId}. Using fallback FY {FallbackFY}.",
                    currentFY, laboratoryTestId, invoiceCase.FinancialYear);

            return invoiceCase;
        }

        /// <inheritdoc />
        public async Task<PriceSummaryDto> CalculateTestPrice(long headerId)
        {
            var header = await _db.TestResultHeaders
                .Include(h => h.Parameters)
                .FirstOrDefaultAsync(h => h.ID == headerId);

            if (header == null)
                throw new Exception($"TestResultHeader {headerId} not found");

            // Get InvoiceCase for this LaboratoryTest (FY-filtered with fallback)
            var invoiceCase = await GetInvoiceCaseForTestAsync(header.LaboratoryTestID);

            decimal calculatedTotal = 0;
            string? message = null;

            if (invoiceCase == null)
            {
                // Look up the test name for a helpful message
                var labTest = await _db.LaboratoryTests.FirstOrDefaultAsync(t => t.ID == header.LaboratoryTestID);
                var testName = labTest?.Name ?? $"LaboratoryTestID {header.LaboratoryTestID}";
                message = $"No pricing configuration found for test '{testName}'. Please set up Invoice Case in Configuration > Invoice Case.";
                _logger.LogWarning("No InvoiceCase found for LaboratoryTestID {LabTestId} (Header {HeaderId})", header.LaboratoryTestID, headerId);
            }
            else if (!invoiceCase.InvoiceCasePrices.Any())
            {
                message = $"Invoice Case (FY: {invoiceCase.FinancialYear}) exists but has no price entries configured. Please add price tiers in Configuration > Invoice Case.";
                _logger.LogWarning("InvoiceCase {CaseId} has no prices for Header {HeaderId}", invoiceCase.ID, headerId);
            }
            else
            {
                var breakdown = BuildPriceBreakdown(header.Parameters, invoiceCase.InvoiceCasePrices);
                calculatedTotal = breakdown.Sum(b => b.Amount);

                if (calculatedTotal == 0)
                {
                    message = "Price calculation returned 0. Parameters may not match any configured pricing tier. Check Invoice Case price configuration.";
                }
            }

            // Update header
            header.CalculatedPrice = calculatedTotal;
            await _db.SaveChangesAsync();

            _logger.LogInformation(
                "Calculated test price for Header {HeaderId}: {Price}",
                headerId, calculatedTotal);

            var summary = await GetPriceSummary(headerId);
            if (message != null)
                summary.Message = message;
            return summary;
        }

        /// <inheritdoc />
        public async Task<List<PriceBreakdownDto>> GetPriceBreakdown(long headerId)
        {
            var header = await _db.TestResultHeaders
                .Include(h => h.Parameters)
                .FirstOrDefaultAsync(h => h.ID == headerId);

            if (header == null)
                throw new Exception($"TestResultHeader {headerId} not found");

            var invoiceCase = await GetInvoiceCaseForTestAsync(header.LaboratoryTestID);

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
            var invoiceCase = await GetInvoiceCaseForTestAsync(header.LaboratoryTestID);

            string? message = null;

            if (invoiceCase == null)
            {
                var labTest = await _db.LaboratoryTests.FirstOrDefaultAsync(t => t.ID == header.LaboratoryTestID);
                var testName = labTest?.Name ?? $"LaboratoryTestID {header.LaboratoryTestID}";
                message = $"No pricing configuration found for test '{testName}'. Please set up Invoice Case in Configuration > Invoice Case.";
            }
            else if (!invoiceCase.InvoiceCasePrices.Any())
            {
                message = $"Invoice Case (FY: {invoiceCase.FinancialYear}) exists but has no price entries configured. Please add price tiers in Configuration > Invoice Case.";
            }
            else
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
                Breakdown = breakdown,
                Message = message
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
                // --- GROUP 2a: Quantity-based pricing (PerIndent, Location, FieldWise) ---
                var quantitySelectionTypes = new[] { "PerIndent", "Indent", "Location", "NoOfLocations", "FieldWise", "EachField" };
                var quantityPrices = dimensionalPrices.Where(p =>
                    quantitySelectionTypes.Contains(p.Configuration!.SelectionType, StringComparer.OrdinalIgnoreCase)).ToList();

                foreach (var qPrice in quantityPrices)
                {
                    decimal? qty = FindParameterValueForDimension(billableParams, qPrice.Configuration!);
                    if (qty == null || qty <= 0) qty = 1;

                    breakdown.Add(new PriceBreakdownDto
                    {
                        ParameterId = 0,
                        ParameterName = $"{qPrice.Name ?? qPrice.Configuration!.Name} x {qty}",
                        UnitPrice = qPrice.Price,
                        Quantity = (int)qty.Value,
                        Amount = qPrice.Price * qty.Value
                    });
                }

                // --- GROUP 2b: Days-based slab pricing ---
                var daysSelectionTypes = new[] { "Days", "Duration" };
                var daysPrices = dimensionalPrices.Where(p =>
                    daysSelectionTypes.Contains(p.Configuration!.SelectionType, StringComparer.OrdinalIgnoreCase)).ToList();

                if (daysPrices.Any())
                {
                    decimal? daysValue = FindParameterValueForDimension(billableParams, daysPrices.First().Configuration!);
                    if (daysValue != null)
                    {
                        // Slab match: find nearest slab >= days value
                        var matched = daysPrices
                            .Where(p => decimal.TryParse(p.Configuration!.Value, out var v) && v >= daysValue)
                            .OrderBy(p => decimal.Parse(p.Configuration!.Value))
                            .FirstOrDefault();

                        if (matched != null)
                        {
                            var unit = matched.Configuration?.Unit ?? "days";
                            breakdown.Add(new PriceBreakdownDto
                            {
                                ParameterId = 0,
                                ParameterName = $"{matched.Configuration!.Name} ({daysValue} {unit})".Trim(),
                                UnitPrice = matched.Price,
                                Quantity = 1,
                                Amount = matched.Price
                            });
                        }
                    }
                }

                // --- GROUP 2c: Size/Load combo (two-dimensional lookup) ---
                var sizeLoadTypes = new[] { "SizeLoad", "SizeAndLoad" };
                var sizeLoadPrices = dimensionalPrices.Where(p =>
                    sizeLoadTypes.Contains(p.Configuration!.SelectionType, StringComparer.OrdinalIgnoreCase)).ToList();

                if (sizeLoadPrices.Any())
                {
                    // First dimension: use Start/End fields for size range
                    // Second dimension: use Value field for load threshold
                    // Both must match for a price entry to apply
                    decimal? sizeValue = FindParameterValueByName(billableParams, "size", "diameter", "width", "thickness");
                    decimal? loadValue = FindParameterValueByName(billableParams, "load", "force", "capacity");

                    if (sizeValue != null && loadValue != null)
                    {
                        var matched = sizeLoadPrices.FirstOrDefault(p =>
                        {
                            var cfg = p.Configuration!;
                            bool sizeMatch = decimal.TryParse(cfg.Start, out var s) &&
                                             decimal.TryParse(cfg.End, out var e) &&
                                             sizeValue >= s && sizeValue <= e;
                            bool loadMatch = decimal.TryParse(cfg.Value, out var lv) && loadValue >= lv;
                            return sizeMatch && loadMatch;
                        });

                        if (matched != null)
                        {
                            var unit = matched.Configuration?.Unit ?? "";
                            breakdown.Add(new PriceBreakdownDto
                            {
                                ParameterId = 0,
                                ParameterName = $"{matched.Configuration!.Name} (Size:{sizeValue}, Load:{loadValue} {unit})".Trim(),
                                UnitPrice = matched.Price,
                                Quantity = 1,
                                Amount = matched.Price
                            });
                        }
                    }
                }

                // --- GROUP 2d: Fixed + Algorithm pricing ---
                var fixedAlgoTypes = new[] { "FixedWithAlgorithm" };
                var fixedAlgoPrices = dimensionalPrices.Where(p =>
                    fixedAlgoTypes.Contains(p.Configuration!.SelectionType, StringComparer.OrdinalIgnoreCase)).ToList();

                foreach (var fap in fixedAlgoPrices)
                {
                    var basePrice = fap.Price;
                    decimal additional = 0;

                    // If config Value contains a formula, evaluate it using parameter values
                    var formula = fap.Configuration!.Value;
                    if (!string.IsNullOrWhiteSpace(formula) && !decimal.TryParse(formula, out _))
                    {
                        var variables = new Dictionary<string, double>();
                        foreach (var param in billableParams)
                        {
                            if (param.Value.HasValue && !string.IsNullOrWhiteSpace(param.ParameterName))
                            {
                                var key = param.ParameterName.Trim().Replace(" ", "_");
                                variables[key] = (double)param.Value.Value;
                            }
                        }

                        var evaluator = new Helpers.FormulaEvaluator();
                        var result = evaluator.Evaluate(formula, variables);
                        if (result.HasValue)
                            additional = (decimal)result.Value;
                    }

                    var total = basePrice + additional;
                    breakdown.Add(new PriceBreakdownDto
                    {
                        ParameterId = 0,
                        ParameterName = additional > 0
                            ? $"{fap.Name ?? fap.Configuration!.Name} (Base:{basePrice} + Calc:{additional})"
                            : fap.Name ?? fap.Configuration!.Name ?? "Fixed Fee",
                        UnitPrice = total,
                        Quantity = 1,
                        Amount = total
                    });
                }

                // --- GROUP 2e: Standard range/slab pricing (remaining dimensional prices) ---
                var handledTypes = quantitySelectionTypes
                    .Concat(daysSelectionTypes)
                    .Concat(sizeLoadTypes)
                    .Concat(fixedAlgoTypes)
                    .ToArray();

                var remainingDimensionalPrices = dimensionalPrices.Where(p =>
                    !handledTypes.Contains(p.Configuration!.SelectionType, StringComparer.OrdinalIgnoreCase)).ToList();

                var groups = remainingDimensionalPrices.GroupBy(p => p.Configuration!.SelectionType);

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
        /// Find parameter value by searching for any of the given name keywords in parameter names.
        /// Used for multi-dimensional pricing (e.g., SizeLoad) where we need to locate
        /// specific parameters by well-known names rather than by config Name/AliasName.
        /// </summary>
        private decimal? FindParameterValueByName(
            List<TestResultParameter> billableParams,
            params string[] keywords)
        {
            foreach (var param in billableParams)
            {
                var paramName = (param.ParameterName ?? "").Trim().ToLower();
                if (string.IsNullOrWhiteSpace(paramName)) continue;

                if (keywords.Any(k => paramName.Contains(k.ToLower())))
                {
                    if (param.Value.HasValue) return param.Value.Value;
                }
            }
            return null;
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

                // Strict matching: exact, then token-based (no loose Contains)
                bool matched = configNames.Any(cn => cn == paramName); // Exact match

                if (!matched && paramName.Length > 2)
                {
                    // Token-based match for names with 3+ chars
                    var separators = new[] { ' ', '-', '_' };
                    var paramTokens = paramName.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                    matched = configNames.Any(cn =>
                    {
                        var cnTokens = cn.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                        return paramTokens.Any(pt => pt.Length >= 3 && cnTokens.Contains(pt));
                    });
                }

                if (matched)
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
        /// Matching priority: 1) Exact match, 2) Alias exact match, 3) Token match (3+ chars only).
        /// Short names (≤2 chars, e.g. chemical elements C, Mn, Si) only match via exact or alias.
        /// Always returns parent Config ID reference — no confusion about which config matched.
        /// </summary>
        private bool MatchesName(string paramNameLower, string priceName, string priceAlias)
        {
            var priceNameLower = (priceName ?? "").Trim().ToLower();
            var priceAliasLower = (priceAlias ?? "").Trim().ToLower();

            // 1. Exact match
            if (paramNameLower == priceNameLower)
                return true;

            // 2. Alias exact match (comma-separated)
            if (!string.IsNullOrWhiteSpace(priceAliasLower))
            {
                var aliases = priceAliasLower.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(a => a.Trim());

                if (aliases.Any(alias => alias == paramNameLower))
                    return true;
            }

            // 3. Short names (≤2 chars) — only exact/alias match, skip token matching
            if (paramNameLower.Length <= 2 || priceNameLower.Length <= 2)
                return false;

            // 4. Token-based match (split by space/hyphen/underscore, match exact tokens)
            var separators = new[] { ' ', '-', '_' };
            var paramTokens = paramNameLower.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            var priceTokens = priceNameLower.Split(separators, StringSplitOptions.RemoveEmptyEntries);

            if (paramTokens.Any(pt => pt.Length >= 3 && priceTokens.Contains(pt)))
                return true;

            return false;
        }
    }
}
