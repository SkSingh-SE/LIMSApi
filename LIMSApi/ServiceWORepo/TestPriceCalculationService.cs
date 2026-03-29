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
                .Include(h => h.Sample)
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
                var labTest = await _db.LaboratoryTests.FirstOrDefaultAsync(t => t.ID == header.LaboratoryTestID);
                var testName = labTest?.Name ?? "Test";

                // Respect pricing type: user override > invoice case default > auto-detect
                var pricingType = header.SelectedPricingType ?? invoiceCase.DefaultPricingType;
                var pricesToUse = FilterPricesByType(invoiceCase.InvoiceCasePrices, pricingType);

                var breakdown = BuildPriceBreakdown(header.Parameters, pricesToUse, testName, header.Sample, header.PricingDimensionValue);
                calculatedTotal = breakdown.Sum(b => b.Amount);

                if (calculatedTotal == 0)
                {
                    var selectedType = header.SelectedPricingType ?? invoiceCase.DefaultPricingType ?? "Auto-detect";
                    var availableTypes = GetAvailablePricingTypes(invoiceCase);
                    var suggestion = availableTypes.Any()
                        ? $"Available types: {string.Join(", ", availableTypes.Select(t => GetDisplayName(t)))}. Try using 'Smart Pricing' to find the best match."
                        : "No pricing tiers configured.";
                    message = $"Price is ₹0 for '{GetDisplayName(selectedType)}' pricing. No matching tier found. {suggestion}";
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
                .Include(h => h.Sample)
                .FirstOrDefaultAsync(h => h.ID == headerId);

            if (header == null)
                throw new Exception($"TestResultHeader {headerId} not found");

            var invoiceCase = await GetInvoiceCaseForTestAsync(header.LaboratoryTestID);

            if (invoiceCase == null || !invoiceCase.InvoiceCasePrices.Any())
                return new List<PriceBreakdownDto>();

            return BuildPriceBreakdown(header.Parameters, invoiceCase.InvoiceCasePrices, "Test", header.Sample, header.PricingDimensionValue);
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
                .Include(h => h.Sample)
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
                var labTest = await _db.LaboratoryTests.FirstOrDefaultAsync(t => t.ID == header.LaboratoryTestID);
                var testName = labTest?.Name ?? "Test";

                // Determine pricing type: user override > invoice case default > auto-detect
                var pricingType = header.SelectedPricingType ?? invoiceCase.DefaultPricingType;

                var pricesToUse = FilterPricesByType(invoiceCase.InvoiceCasePrices, pricingType);

                breakdown = BuildPriceBreakdown(header.Parameters, pricesToUse, testName, header.Sample, header.PricingDimensionValue);
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
                Message = message,
                SelectedPricingType = header.SelectedPricingType,
                DefaultPricingType = invoiceCase?.DefaultPricingType,
                AvailablePricingTypes = GetAvailablePricingTypes(invoiceCase),
                PricingDimensionValue = header.PricingDimensionValue,
                SampleDimensions = header.Sample != null ? new SampleDimensionsDto
                {
                    Thickness = header.Sample.Thickness,
                    Diameter = header.Sample.Diameter,
                    Width = header.Sample.Width,
                    Length = header.Sample.Length
                } : null
            };
        }

        private List<string> GetAvailablePricingTypes(InvoiceCase? invoiceCase)
        {
            if (invoiceCase?.InvoiceCasePrices == null || !invoiceCase.InvoiceCasePrices.Any())
                return new List<string>();

            var types = new HashSet<string>();
            foreach (var p in invoiceCase.InvoiceCasePrices)
            {
                if (p.Configuration != null && !string.IsNullOrEmpty(p.Configuration.SelectionType))
                    types.Add(p.Configuration.SelectionType);
                else
                    types.Add("FlatRate");
            }
            return types.ToList();
        }

        private ICollection<InvoiceCasePrice> FilterPricesByType(ICollection<InvoiceCasePrice> allPrices, string? pricingType)
        {
            if (string.IsNullOrEmpty(pricingType)) return allPrices;

            var filtered = pricingType switch
            {
                "Element" => allPrices
                    .Where(p => p.Configuration != null &&
                        string.Equals(p.Configuration.SelectionType, "Element", StringComparison.OrdinalIgnoreCase)).ToList(),
                "FlatRate" => allPrices
                    .Where(p => p.InvoiceCaseConfigID == 0
                        || p.Configuration == null
                        || string.IsNullOrEmpty(p.Configuration.SelectionType)
                        || string.Equals(p.Configuration.SelectionType, "Other", StringComparison.OrdinalIgnoreCase)).ToList(),
                _ => allPrices
                    .Where(p => p.Configuration != null &&
                        string.Equals(p.Configuration.SelectionType, pricingType, StringComparison.OrdinalIgnoreCase)).ToList()
            };

            return filtered.Any() ? filtered : allPrices;
        }

        public async Task SetPricingTypeAsync(long headerId, string? pricingType)
        {
            var header = await _db.TestResultHeaders.FirstOrDefaultAsync(h => h.ID == headerId)
                ?? throw new KeyNotFoundException($"TestResultHeader {headerId} not found");
            header.SelectedPricingType = pricingType;
            await _db.SaveChangesAsync();
        }

        public async Task<PriceSummaryDto> SetPricingTypeWithValueAsync(long headerId, string? pricingType, string? dimensionValue)
        {
            var header = await _db.TestResultHeaders.FirstOrDefaultAsync(h => h.ID == headerId)
                ?? throw new KeyNotFoundException($"TestResultHeader {headerId} not found");
            header.SelectedPricingType = pricingType;
            header.PricingDimensionValue = dimensionValue;
            await _db.SaveChangesAsync();
            return await CalculateTestPrice(headerId);
        }

        public async Task<PricingRecommendationDto> GetPricingRecommendation(long headerId)
        {
            var header = await _db.TestResultHeaders
                .Include(h => h.Parameters)
                .Include(h => h.Sample)
                .FirstOrDefaultAsync(h => h.ID == headerId)
                ?? throw new KeyNotFoundException($"TestResultHeader {headerId} not found");

            var labTest = await _db.LaboratoryTests.FirstOrDefaultAsync(t => t.ID == header.LaboratoryTestID);
            var invoiceCase = await GetInvoiceCaseForTestAsync(header.LaboratoryTestID);

            var result = new PricingRecommendationDto
            {
                HeaderId = headerId,
                TestName = labTest?.Name ?? "Test",
                BillableParamCount = header.Parameters.Count(p => p.IsBillable),
                CurrentSelectedType = header.SelectedPricingType,
                DefaultPricingType = invoiceCase?.DefaultPricingType,
                SampleDimensions = header.Sample != null ? new SampleDimensionsDto
                {
                    Thickness = header.Sample.Thickness,
                    Diameter = header.Sample.Diameter,
                    Width = header.Sample.Width,
                    Length = header.Sample.Length
                } : null
            };

            if (invoiceCase == null || !invoiceCase.InvoiceCasePrices.Any())
                return result;

            var billableParams = header.Parameters.Where(p => p.IsBillable).ToList();
            var availableTypes = GetAvailablePricingTypes(invoiceCase);

            foreach (var pricingType in availableTypes)
            {
                var rec = ScorePricingType(pricingType, invoiceCase, billableParams, header.Sample);
                result.Recommendations.Add(rec);
            }

            result.Recommendations = result.Recommendations.OrderByDescending(r => r.Score).ToList();
            if (result.Recommendations.Any())
                result.Recommendations.First().IsRecommended = true;

            return result;
        }

        private PricingTypeRecommendationDto ScorePricingType(
            string pricingType, InvoiceCase invoiceCase,
            List<TestResultParameter> billableParams, SampleDetail? sample)
        {
            var prices = FilterPricesByType(invoiceCase.InvoiceCasePrices, pricingType);
            var rec = new PricingTypeRecommendationDto
            {
                PricingType = pricingType,
                DisplayName = GetDisplayName(pricingType),
                TierCount = prices.Count,
                Tiers = prices.Select(p => new PricingTierPreviewDto
                {
                    Name = p.Name ?? "",
                    Value = p.Configuration?.Value,
                    Start = p.Configuration?.Start,
                    End = p.Configuration?.End,
                    Price = p.Price
                }).ToList()
            };

            int score = 0;
            var reasons = new List<string>();

            switch (pricingType)
            {
                case "Element":
                    var count = billableParams.Count;
                    if (count > 0) { score += 30; reasons.Add($"{count} billable parameters"); rec.AutoDetectedValue = count; rec.ValueSource = "Parameter count"; }
                    break;

                case "Size": case "SizeRange":
                    var sz = sample?.Diameter ?? sample?.Thickness ?? sample?.Width;
                    if (sz.HasValue) { score += 40; reasons.Add($"Sample dimension {sz}mm"); rec.AutoDetectedValue = sz; rec.ValueSource = "Sample"; }
                    else { var pSz = FindParameterValueByName(billableParams, "size", "diameter", "width", "thickness"); if (pSz.HasValue) { score += 25; rec.AutoDetectedValue = pSz; rec.ValueSource = "Parameter"; } }
                    rec.RequiredInput = "Size"; rec.InputHint = "Specimen size in mm"; rec.Unit = "mm";
                    break;

                case "Weight": case "WeightRange":
                    var wt = FindParameterValueByName(billableParams, "weight", "load", "force", "capacity");
                    if (wt.HasValue) { score += 35; reasons.Add($"Load {wt}kN"); rec.AutoDetectedValue = wt; rec.ValueSource = "Parameter"; }
                    rec.RequiredInput = "Weight/Load"; rec.InputHint = "Load in kN"; rec.Unit = "kN";
                    break;

                case "Hours": case "HoursRange":
                    var hr = FindParameterValueByName(billableParams, "hours", "duration", "time", "hr");
                    if (hr.HasValue) { score += 35; reasons.Add($"Duration {hr}hr"); rec.AutoDetectedValue = hr; rec.ValueSource = "Parameter"; }
                    rec.RequiredInput = "Hours"; rec.InputHint = "Duration in hours"; rec.Unit = "hr";
                    break;

                case "Temprature": case "TempratureRange":
                    var tmp = FindParameterValueByName(billableParams, "temperature", "temp");
                    if (tmp.HasValue) { score += 35; reasons.Add($"Temperature {tmp}°C"); rec.AutoDetectedValue = tmp; rec.ValueSource = "Parameter"; }
                    rec.RequiredInput = "Temperature"; rec.InputHint = "Test temperature"; rec.Unit = "°C";
                    break;

                case "SizeLoad": case "SizeAndLoad":
                    var szC = sample?.Diameter ?? sample?.Thickness ?? sample?.Width;
                    var ldC = FindParameterValueByName(billableParams, "load", "force", "capacity");
                    if (szC.HasValue && ldC.HasValue) { score += 45; reasons.Add($"Size {szC}mm + Load {ldC}kN"); rec.AutoDetectedValue = szC; rec.ValueSource = "Sample + Parameter"; }
                    else if (szC.HasValue) { score += 20; reasons.Add($"Size {szC}mm found, load needed"); rec.AutoDetectedValue = szC; rec.ValueSource = "Sample"; rec.Status = "needs_input"; }
                    else { rec.Status = "needs_input"; }
                    rec.RequiredInput = "Size + Load"; rec.InputHint = "Size (mm) and Load (kN)"; rec.Unit = "mm / kN";
                    break;

                case "SpectroCombination":
                    var specialElements = new[] { "N", "B", "Ca", "Nb", "Ti", "V", "Al" };
                    var found = billableParams.Where(p => specialElements.Any(e => string.Equals(p.ParameterName?.Trim(), e, StringComparison.OrdinalIgnoreCase))).Select(p => p.ParameterName?.Trim()).ToList();
                    var combo = found.Any() ? "Full + " + string.Join(" + ", found) : "Full";
                    score += 35; reasons.Add($"Detected: {combo}"); rec.AutoDetectedValue = null; rec.ValueSource = combo;
                    break;

                case "FlatRate": case "Other":
                    score += 10; reasons.Add("Fixed price");
                    break;

                default:
                    score += 5; reasons.Add(pricingType);
                    break;
            }

            if (!string.IsNullOrEmpty(invoiceCase.DefaultPricingType) &&
                string.Equals(pricingType, invoiceCase.DefaultPricingType, StringComparison.OrdinalIgnoreCase))
            { score += 15; reasons.Add("Default"); }

            if (prices.Any()) { score += 5; reasons.Add($"{prices.Count} tier(s)"); }

            rec.Score = score;
            rec.Reason = string.Join(" | ", reasons);
            if (string.IsNullOrEmpty(rec.Status)) rec.Status = rec.AutoDetectedValue.HasValue ? "ready" : (rec.RequiredInput != null ? "needs_input" : "ready");
            return rec;
        }

        private static string GetDisplayName(string type) => type switch
        {
            "Element" => "Parameter Count",
            "Hours" => "Hours", "HoursRange" => "Hours Range",
            "Size" => "Size", "SizeRange" => "Size Range",
            "Weight" => "Weight/Load", "WeightRange" => "Weight Range",
            "Temprature" => "Temperature", "TempratureRange" => "Temperature Range",
            "SizeLoad" => "Size + Load", "SizeAndLoad" => "Size + Load Range",
            "SpectroCombination" => "Spectro Combination",
            "FlatRate" => "Flat Rate", "Other" => "Other",
            _ => type
        };

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
            ICollection<InvoiceCasePrice> prices,
            string testName = "Test",
            SampleDetail? sample = null,
            string? dimensionOverride = null)
        {
            var breakdown = new List<PriceBreakdownDto>();

            // Parse dimension override: "12" for single value, "12|500" for SizeLoad (size|load)
            decimal? dimSize = null, dimLoad = null;
            if (!string.IsNullOrEmpty(dimensionOverride))
            {
                var parts = dimensionOverride.Split('|');
                if (parts.Length >= 1 && decimal.TryParse(parts[0].Trim(), out var p1)) dimSize = p1;
                if (parts.Length >= 2 && decimal.TryParse(parts[1].Trim(), out var p2)) dimLoad = p2;
            }

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

                // If exact slab not found, use the highest available slab
                if (matched == null)
                {
                    matched = elementPrices
                        .Where(p => decimal.TryParse(p.Configuration!.Value, out _))
                        .OrderByDescending(p => decimal.Parse(p.Configuration!.Value))
                        .FirstOrDefault();
                }

                if (matched != null)
                {
                    breakdown.Add(new PriceBreakdownDto
                    {
                        ParameterId = 0,
                        ParameterName = $"{testName} ({elementCount} parameters)",
                        UnitPrice = matched.Price,
                        Quantity = 1,
                        Amount = matched.Price
                    });
                    return breakdown;
                }
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
                    // Second dimension: use Value field for load threshold/range
                    // SizeLoad: Value = max load capacity (single number)
                    // SizeAndLoad: Value = "minLoad-maxLoad" (range string)
                    decimal? sizeValue = dimSize
                        ?? sample?.Diameter ?? sample?.Thickness ?? sample?.Width
                        ?? FindParameterValueByName(billableParams, "size", "diameter", "width", "thickness");
                    decimal? loadValue = dimLoad
                        ?? FindParameterValueByName(billableParams, "load", "force", "capacity");

                    if (sizeValue != null && loadValue != null)
                    {
                        var matched = sizeLoadPrices.FirstOrDefault(p =>
                        {
                            var cfg = p.Configuration!;
                            bool sizeMatch = decimal.TryParse(cfg.Start, out var s) &&
                                             decimal.TryParse(cfg.End, out var e) &&
                                             sizeValue >= s && sizeValue <= e;

                            bool loadMatch;
                            if (cfg.SelectionType.Equals("SizeAndLoad", StringComparison.OrdinalIgnoreCase))
                            {
                                // SizeAndLoad: Value = "minLoad-maxLoad" → range check
                                var parts = cfg.Value.Split('-');
                                loadMatch = parts.Length == 2 &&
                                            decimal.TryParse(parts[0].Trim(), out var minL) &&
                                            decimal.TryParse(parts[1].Trim(), out var maxL) &&
                                            loadValue >= minL && loadValue <= maxL;
                            }
                            else
                            {
                                // SizeLoad: Value = max load capacity → threshold check
                                loadMatch = decimal.TryParse(cfg.Value, out var lv) && loadValue <= lv;
                            }

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
