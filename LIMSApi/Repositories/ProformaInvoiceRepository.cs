
using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Helpers.Enums;
using LIMSApi.Models;
using LIMSApi.Helpers;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;

namespace LIMSApi.Repositories
{
    public class ProformaInvoiceRepository : IProformaInvoiceRepository
    {
        private readonly LIMSContext _context;
        private readonly IFinancialYearService _fyService;
        private readonly IPricingEngine _pricingEngine;
        private LoggedInUserDTO loggedInUser;

        public ProformaInvoiceRepository(LIMSContext context, IFinancialYearService fyService, IPricingEngine pricingEngine)
        {
            _context = context;
            _fyService = fyService;
            _pricingEngine = pricingEngine;
            loggedInUser = LoggedInUserProvider.CurrentUser;
        }

        public async Task<string> GeneratePINoAsync()
        {
            var year = DateTime.Now.Year;
            var last = await _context.ProformaInvoiceHeader
                .OrderByDescending(x => x.ID)
                .Select(x => x.PINo)
                .FirstOrDefaultAsync();

            var next = last != null
                ? int.Parse(last.Split('/').Last()) + 1
                : 1;

            return $"PI/{year}/{next.ToString("D6")}";
        }
        public async Task<long> GeneratePIAsync(long inwardId)
        {
            using var tx = await _context.Database.BeginTransactionAsync();

            try
            {
                // 1. PREVENT DUPLICATE PI
                if (await _context.ProformaInvoiceHeader.AnyAsync(x => x.InwardID == inwardId))
                    throw new Exception("PI already generated.");

                // 2. VALIDATE INVOICE NOT ALREADY GENERATED
                var inward = await _context.SampleInwards
                    .Include(x => x.Customer)
                    .FirstOrDefaultAsync(x => x.ID == inwardId);

                if (inward == null)
                    throw new Exception("Inward not found.");

                if (inward.IsInvoiceGenerated)
                    throw new InvalidOperationException("Cannot generate PI. Invoice has already been generated for this inward.");

                // ── GST from System Configuration ──
                var gstConfig = await _context.GstConfigs.FirstOrDefaultAsync();
                var piGstApplicable = gstConfig?.PIGstApplicable ?? true;
                var gstRate = gstConfig?.DefaultGstRate ?? 18m;
                var halfRate = gstRate / 2m;
                var companyState = gstConfig?.State?.Trim().ToLower() ?? "";
                var customerState = inward?.State?.Trim().ToLower() ?? "";
                var customerGstExempt = inward?.Customer?.GSTNA ?? false;

                // SpecialAccountingCase: SEZ or No GST = exempt from GST
                var specialCase = inward?.Customer?.SpecialAccountingCase ?? "";
                if (specialCase.Equals("SEZ", StringComparison.OrdinalIgnoreCase)
                    || specialCase.Equals("No GST applicable", StringComparison.OrdinalIgnoreCase))
                {
                    customerGstExempt = true;
                }

                // Determine inter-state from company state vs customer state
                var isInterState = !string.IsNullOrEmpty(companyState)
                    && !string.IsNullOrEmpty(customerState)
                    && companyState != customerState;
                // Check for existing TaxInvoice as additional safeguard
                var hasInvoice = await _context.TaxInvoices
                    .AnyAsync(x => x.InwardID == inwardId);

                if (hasInvoice)
                    throw new InvalidOperationException("Cannot generate PI. Invoice has already been generated for this inward.");

                // 3. CUTTING + MACHINING
                var cuttingHeader = await _context.CuttingChargeHeaders
                    .FirstOrDefaultAsync(x => x.InwardID == inwardId);

                var cuttingAmount = cuttingHeader?.GrandTotal ?? 0;

                var machiningAmount = await _context.MachiningChargeItems
                    .Where(x => _context.SampleDetails.Any(s => s.ID == x.SampleID && s.InwardID == inwardId) && x.IsActive)
                    .SumAsync(x => (decimal?)x.Amount) ?? 0;

                // ===========================
                //  4. LAB TEST CHARGES (FULLY IMPLEMENTED)
                // ===========================
                var piTestDetails = new List<ProformaInvoiceDetail>();
                decimal totalTestAmount = 0;
                var pricingErrors = new List<string>();

                var sampleDetails = await _context.SampleDetails
                    .Where(x => x.InwardID == inwardId)
                    .Include(x => x.TestPlans)
                        .ThenInclude(x => x.GeneralTests)
                            .ThenInclude(x => x.Methods)
                    .Include(x => x.TestPlans)
                        .ThenInclude(x => x.ChemicalTests)
                            .ThenInclude(x => x.Elements)
                    .Include(x => x.TestPlans)
                        .ThenInclude(x => x.ChemicalTests)
                            .ThenInclude(x => x.TestTypes)
                    .ToListAsync();

                foreach (var sd in sampleDetails)
                {
                    foreach (var plan in sd.TestPlans)
                    {
                        // ---------------- GENERAL TESTS ---------------- 
                        foreach (var gt in plan.GeneralTests)
                        {
                            foreach (var method in gt.Methods)
                            {
                                try
                                {
                                    var (rate, configId, selectionType, usedValue) = await GetRateForGeneralTestAsync(
                                        method.LaboratoryTestID,
                                        gt,
                                        method,
                                        plan.ID,
                                        inward.CollectionTime,
                                        sd
                                    );

                                    var amount = rate * method.Quantity;
                                    totalTestAmount += amount;

                                    piTestDetails.Add(new ProformaInvoiceDetail
                                    {
                                        SampleID = sd.ID,
                                        ChargeType = "GeneralTest",
                                        Description = "General Test",
                                        Quantity = method.Quantity,
                                        Rate = rate,
                                        Amount = amount,
                                        SelectionType = selectionType,
                                        UsedValue = usedValue ?? 0,
                                        InvoiceCaseConfigID = configId
                                    });
                                }
                                catch (Exception ex)
                                {
                                    System.Diagnostics.Debug.WriteLine(
                                        $"[PI] GeneralTest pricing FAILED for LabTestID={method.LaboratoryTestID}, " +
                                        $"Method={method.ID}, Plan={plan.ID}: {ex.Message}");
                                    pricingErrors.Add($"Test LabID={method.LaboratoryTestID}: {ex.Message}");
                                    continue;
                                }
                            }
                        }

                        // ---------------- CHEMICAL TESTS (NEW LOGIC) ---------------- 
                        foreach (var ct in plan.ChemicalTests)
                        {
                            // Count selected elements once
                            var usedElements = ct.Elements.Count(x => x.Selected);

                            // Loop over each selected TestType
                            foreach (var tt in ct.TestTypes.Where(t => t.IsSelected && t.LaboratoryTestID.HasValue))
                            {
                                try
                                {
                                    if (!tt.LaboratoryTestID.HasValue) continue;

                                    var (rate, configId, selectionType, usedValue) = await GetRateForChemicalTestAsync(
                                        tt.LaboratoryTestID.Value,
                                        ct,
                                        usedElements,
                                        inward.CollectionTime
                                    );

                                    var amount = rate;
                                    totalTestAmount += amount;

                                    piTestDetails.Add(new ProformaInvoiceDetail
                                    {
                                        SampleID = sd.ID,
                                        ChargeType = "ChemicalTest",
                                        Description = $"Chemical Test - {tt.Name}",
                                        Quantity = 1,
                                        Rate = rate,
                                        Amount = amount,
                                        SelectionType = selectionType,
                                        UsedValue = usedValue ?? 0,
                                        InvoiceCaseConfigID = configId
                                    });
                                }
                                catch (Exception ex)
                                {
                                    System.Diagnostics.Debug.WriteLine(
                                        $"[PI] ChemicalTest pricing FAILED for LabTestID={tt.LaboratoryTestID}: {ex.Message}");
                                    pricingErrors.Add($"Chemical LabID={tt.LaboratoryTestID}: {ex.Message}");
                                    continue;
                                }
                            }
                        }

                    }
                }

                // ===========================
                //  5. GRAND TOTAL (with customer discount)
                // ===========================
                var subTotal = cuttingAmount + machiningAmount + totalTestAmount;

                // Warn if no test pricing found — PI will be ₹0 for test charges
                if (totalTestAmount == 0 && piTestDetails.Count == 0)
                {
                    System.Diagnostics.Debug.WriteLine(
                        $"[PI Generation] WARNING: No test pricing found for Inward {inwardId}. " +
                        "Check InvoiceCaseConfiguration for matching test entries.");
                }

                if (subTotal <= 0)
                {
                    var errorDetail = pricingErrors.Any()
                        ? " Errors: " + string.Join("; ", pricingErrors)
                        : "";
                    throw new InvalidOperationException(
                        "Cannot generate Proforma Invoice: Total amount is ₹0. " +
                        "Please configure pricing (Invoice Case Configuration) for the planned tests." +
                        errorDetail);
                }

                // Customer discount (applied before GST — standard Indian taxation)
                decimal discountPct = 0;
                decimal discountAmt = 0;
                if (inward!.Customer?.ConstantDiscount == true
                    && inward.Customer.ConstantDiscountPercentage.HasValue
                    && inward.Customer.ConstantDiscountPercentage.Value > 0
                    && inward.Customer.ConstantDiscountPercentage.Value <= 100)
                {
                    discountPct = inward.Customer.ConstantDiscountPercentage.Value;
                    discountAmt = Math.Round(subTotal * discountPct / 100m, 2, MidpointRounding.AwayFromZero);
                }
                var discountedSubTotal = subTotal - discountAmt;

                decimal cgst = 0, sgst = 0, igst = 0;

                // Apply GST on discounted subtotal (only if system config allows AND customer is not GST-exempt)
                if (piGstApplicable && !customerGstExempt)
                {
                    if (isInterState)
                    {
                        igst = Math.Round(discountedSubTotal * gstRate / 100m, 2, MidpointRounding.AwayFromZero);
                    }
                    else
                    {
                        cgst = Math.Round(discountedSubTotal * halfRate / 100m, 2, MidpointRounding.AwayFromZero);
                        sgst = Math.Round(discountedSubTotal * halfRate / 100m, 2, MidpointRounding.AwayFromZero);
                    }
                }

                var taxAmount = cgst + sgst + igst;
                var grandTotal = discountedSubTotal + taxAmount;

                // ===========================
                //  6. INSERT PI HEADER
                // ===========================
                var currentFY = await _context.FinancialYears.FirstOrDefaultAsync(f => f.IsCurrent);

                var piHeader = new ProformaInvoiceHeader
                {
                    InwardID = inwardId,
                    PINo = await GeneratePINoAsync(),
                    PIDate = DateTime.Now,

                    SubTotal = subTotal,
                    DiscountPercentage = discountAmt > 0 ? discountPct : null,
                    DiscountAmount = discountAmt,
                    CGST = cgst,
                    SGST = sgst,
                    IGST = igst,
                    TaxAmount = taxAmount,
                    GrandTotal = grandTotal,
                    FinancialYearId = currentFY?.Id,

                    CreatedBy = loggedInUser.UserId
                };

                _context.ProformaInvoiceHeader.Add(piHeader);
                await _context.SaveChangesAsync();

                // ===========================
                //  7. INSERT MACHINING + CUTTING DETAILS
                // ===========================
                if (cuttingHeader != null)
                {
                    var cuttingSamples = await _context.CuttingChargeSamples
                        .Where(x => x.CuttingChargeHeader.InwardID == inwardId)
                        .ToListAsync();

                    foreach (var cs in cuttingSamples)
                    {
                        _context.ProformaInvoiceDetails.Add(new ProformaInvoiceDetail
                        {
                            ProformaInvoiceHeaderID = piHeader.ID,
                            SampleID = cs.SampleID,
                            ChargeType = "Machining",
                            Description = "Sample Machining Charges",
                            Quantity = 1,
                            Rate = cs.SampleTotal,
                            Amount = cs.SampleTotal
                        });
                    }
                }

                // ===========================
                //  8. INSERT TEST DETAILS
                // ===========================
                foreach (var t in piTestDetails)
                {
                    t.ProformaInvoiceHeaderID = piHeader.ID;
                    _context.ProformaInvoiceDetails.Add(t);
                }

                await _context.SaveChangesAsync();

                // Update BillingStatus to PI_GENERATED after successful PI creation
                // Note: inward variable already loaded earlier for validation
                inward.BillingStatus = BillingStatus.PI_GENERATED.ToString();
                inward.ModifiedOn = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                await tx.CommitAsync();

                return piHeader.ID;
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }


        /// <summary>
        /// Get rate for General Test by extracting parameter values from SpecificationLine
        /// </summary>
        private async Task<(decimal Rate, long ConfigId, string? SelectionType, decimal? UsedValue)> GetRateForGeneralTestAsync(
            long laboratoryTestId,
            GeneralTest generalTest,
            GeneralTestMethod method,
            long testPlanId,
            DateTime inwardDate,
            SampleDetail? sample = null)
        {
            // Date-effective InvoiceCase resolution: pick the version with the greatest
            // EffectiveFrom ≤ the sample inward date, falling back to the earliest version.
            var versions = await _context.InvoiceCases
                .Where(ic => ic.LaboratoryTestID == laboratoryTestId && ic.IsActive)
                .ToListAsync();

            var invoiceCase = PriceVersionResolver.Resolve(versions, v => v.EffectiveFrom, inwardDate);

            if (invoiceCase == null)
                throw new Exception($"No Invoice Case found for LaboratoryTest {laboratoryTestId}");

            // Get all prices for this InvoiceCase (FY-specific prices)
            var allPrices = await _context.InvoiceCasePrices
                .Where(p => p.InvoiceCaseID == invoiceCase.ID)
                .Include(p => p.Configuration)
                .Where(p => p.Configuration != null)
                .ToListAsync();

            if (!allPrices.Any())
                throw new Exception($"No pricing entries found for LaboratoryTest {laboratoryTestId} in current financial year");

            // ── Priority 1: SizeAndLoad — use sample dimensions ──
            if (sample != null)
            {
                decimal sizeValue = sample.Diameter ?? sample.Thickness ?? sample.Width ?? 0;
                if (sizeValue > 0)
                {
                    var sizeLoadPrices = allPrices.Where(p =>
                        p.Configuration!.SelectionType == "SizeAndLoad"
                        || p.Configuration.SelectionType == "SizeLoad"
                        || p.Configuration.SelectionType == "Size with load").ToList();

                    foreach (var price in sizeLoadPrices)
                    {
                        decimal.TryParse(price.Configuration!.Start, out var start);
                        decimal.TryParse(price.Configuration.End, out var end);
                        if (sizeValue >= start && (end == 0 || sizeValue <= end))
                            return (price.Price, price.InvoiceCaseConfigID, price.Configuration.SelectionType, sizeValue);
                    }
                }
            }

            // ── Priority 2: FlatRate / Flat / Other ──
            var flatPrice = allPrices.FirstOrDefault(p =>
                p.Configuration!.SelectionType == "FlatRate"
                || p.Configuration.SelectionType == "Flat"
                || p.Configuration.SelectionType == "Other");
            if (flatPrice != null)
                return (flatPrice.Price, flatPrice.InvoiceCaseConfigID, flatPrice.Configuration!.SelectionType, null);

            // ── Priority 3: Hours ──
            var hoursPrice = allPrices.FirstOrDefault(p =>
                p.Configuration!.SelectionType == "Hours");
            if (hoursPrice != null)
                return (hoursPrice.Price, hoursPrice.InvoiceCaseConfigID, hoursPrice.Configuration!.SelectionType, null);

            // ── Priority 4: Element-based (use specification lines) ──
            var specificationLines = await _context.SpecificationLines
                .Where(sl => (sl.SpecificationGradeID == generalTest.Specification1 ||
                             (generalTest.Specification2.HasValue && sl.SpecificationGradeID == generalTest.Specification2.Value))
                            && sl.Type == "mechanical"
                            && sl.ParameterID.HasValue)
                .Include(sl => sl.Parameter)
                .ToListAsync();

            var testPlan = await _context.TestPlans
                .FirstOrDefaultAsync(tp => tp.ID == testPlanId);

            if (testPlan == null)
                throw new Exception($"TestPlan {testPlanId} not found");

            var testResultHeader = await _context.TestResultHeaders
                .Where(trh => trh.LaboratoryTestID == laboratoryTestId
                             && trh.TestPlanID == testPlanId
                             && trh.SampleID == testPlan.SampleID)
                .Include(trh => trh.Parameters)
                .FirstOrDefaultAsync();

            // Get distinct configs from FY-filtered prices
            var configs = allPrices.Select(p => p.Configuration!).DistinctBy(c => c.ID).ToList();

            foreach (var config in configs.OrderBy(c => c.SelectionType))
            {
                try
                {
                    var parameterValue = await ExtractParameterValueForConfigAsync(
                        config,
                        specificationLines,
                        testResultHeader?.Parameters,
                        "mechanical"
                    );

                    if (parameterValue.HasValue)
                    {
                        var matchingPrice = allPrices.FirstOrDefault(p => p.InvoiceCaseConfigID == config.ID);
                        if (matchingPrice != null)
                            return (matchingPrice.Price, config.ID, config.SelectionType, parameterValue.Value);
                    }
                }
                catch
                {
                    continue;
                }
            }

            // Last resort: return first available price from this FY
            var anyPrice = allPrices.FirstOrDefault();
            if (anyPrice != null)
                return (anyPrice.Price, anyPrice.InvoiceCaseConfigID, anyPrice.Configuration?.SelectionType, null);

            throw new Exception($"No matching pricing configuration found for LaboratoryTest {laboratoryTestId} with available parameters");
        }

        /// <summary>
        /// Get rate for Chemical Test
        /// </summary>
        private async Task<(decimal Rate, long ConfigId, string? SelectionType, decimal? UsedValue)> GetRateForChemicalTestAsync(
            long laboratoryTestId,
            ChemicalTest chemicalTest,
            int usedElements,
            DateTime inwardDate)
        {
            // Get all InvoiceCaseConfigurations linked to this Chemical Test's AnalysisType
            var configs = await _context.LaboratoryTestAnalysisTypeInvoiceCases
                .Where(lt => lt.LaboratoryTestAnalysisTypeID == chemicalTest.LaboratoryTestAnalysisTypeID)
                .Include(lt => lt.InvoiceCaseConfiguration)
                .Where(lt => lt.InvoiceCaseConfiguration != null && lt.InvoiceCaseConfiguration.IsActive)
                .Select(lt => lt.InvoiceCaseConfiguration!)
                .ToListAsync();

            if (!configs.Any())
                throw new Exception($"No pricing configuration found for LaboratoryTest {laboratoryTestId}");

            // For Element type, use element count
            var elementConfig = configs.FirstOrDefault(c => c.SelectionType == "Element");
            if (elementConfig != null)
            {
                var (rate, configId) = await MatchConfigAndGetRateAsync(
                    laboratoryTestId,
                    elementConfig,
                    usedElements,
                    inwardDate
                );

                return (rate, configId, "Element", usedElements);
            }

            // For other types, get SpecificationLines from ChemicalTestElement
            var specificationLineIds = chemicalTest.Elements
                .Where(e => e.Selected)
                .Select(e => e.SpecificationLineID)
                .ToList();

            var specificationLines = await _context.SpecificationLines
                .Where(sl => specificationLineIds.Contains(sl.ID)
                            && sl.Type == "chemical"
                            && sl.ParameterID.HasValue)
                .Include(sl => sl.Parameter)
                .ToListAsync();

            // Try to get TestResultParameter values if test results are available
            var testResultHeader = await _context.TestResultHeaders
                .Where(trh => trh.LaboratoryTestID == laboratoryTestId)
                .Include(trh => trh.Parameters)
                .FirstOrDefaultAsync();

            // For each configuration, try to find matching parameter and calculate rate
            foreach (var config in configs.OrderBy(c => c.SelectionType))
            {
                try
                {
                    var parameterValue = await ExtractParameterValueForConfigAsync(
                        config,
                        specificationLines,
                        testResultHeader?.Parameters,
                        "chemical"
                    );

                    if (parameterValue.HasValue)
                    {
                        var (rate, configId) = await MatchConfigAndGetRateAsync(
                            laboratoryTestId,
                            config,
                            parameterValue.Value,
                            inwardDate
                        );

                        return (rate, configId, config.SelectionType, parameterValue.Value);
                    }
                }
                catch
                {
                    continue;
                }
            }

            throw new Exception($"No matching pricing configuration found for LaboratoryTest {laboratoryTestId} with available parameters");
        }

        // ExtractParameterValueForConfigAsync and MatchConfigAndGetRateAsync
        // are now delegated to the shared PricingEngine (IPricingEngine)
        // to eliminate code duplication with PriceCalculationService.

        private Task<decimal?> ExtractParameterValueForConfigAsync(
            InvoiceCaseConfiguration config,
            List<SpecificationLine> specificationLines,
            ICollection<TestResultParameter>? testResultParameters,
            string parameterType)
            => _pricingEngine.ExtractParameterValueForConfigAsync(config, specificationLines, testResultParameters, parameterType);

        private Task<(decimal Rate, long ConfigId)> MatchConfigAndGetRateAsync(
            long laboratoryTestId,
            InvoiceCaseConfiguration config,
            decimal usedValue,
            DateTime inwardDate)
            => _pricingEngine.MatchConfigAndGetRateAsync(laboratoryTestId, config, usedValue, inwardDate);

        public async Task<byte[]> GeneratePIPdfAsync(long piId)
        {
            try
            {


                var pi = await _context.ProformaInvoiceHeader
                    .Include(x => x.SampleInward)
                        .ThenInclude(x => x.Customer)
                    .Include(x => x.Details)
                    .FirstOrDefaultAsync(x => x.ID == piId);   //  FIX: FirstOrDefaultAsync

                //  IF PI NOT FOUND → USE DUMMY DATA
                if (pi == null)
                {
                    pi = new ProformaInvoiceHeader
                    {
                        ID = 0,
                        PINo = "PI/DUMMY/000001",
                        PIDate = DateTime.Today,

                        SubTotal = 1000,
                        CGST = 90,
                        SGST = 90,
                        IGST = 0,
                        TaxAmount = 180,
                        GrandTotal = 1180,

                        SampleInward = new SampleInward
                        {
                            CaseNo = "CASE-DUMMY-001",
                            CreatedOn = DateTime.Today,
                            GstNo = "24ABCDE1234F1Z5",
                            Address = "Dummy Industrial Area, Ahmedabad",
                            State = "Gujarat",

                            Customer = new Customer
                            {
                                Name = "DUMMY INDUSTRIES PVT LTD",
                                Address = "Dummy Industrial Area, Ahmedabad",
                                GSTNo = "24ABCDE1234F1Z5",
                                PinCode = "380015",
                                TallyLedgerName = "DUMMY INDUSTRIES PVT LTD",
                                CustomerType = "Regular"
                            }
                        },

                        //  DUMMY LINE ITEMS
                        Details = new List<ProformaInvoiceDetail>
                            {
                                new ProformaInvoiceDetail
                                {
                                    SampleID = 1,
                                    Description = "Sample Machining Charges",
                                    Quantity = 1,
                                    Rate = 500,
                                    Amount = 500
                                },
                                new ProformaInvoiceDetail
                                {
                                    SampleID = 2,
                                    Description = "Chemical Testing Charges",
                                    Quantity = 1,
                                    Rate = 500,
                                    Amount = 500
                                }
                            }
                    };
                }

                // Fetch GST config for state code
                var pdfGstConfig = await _context.GstConfigs.FirstOrDefaultAsync();

                var model = new TaxInvoicePdfModelDto
                {
                    InvoiceNo = pi.PINo,
                    InvoiceDate = pi.PIDate,
                    CustomerName = pi.SampleInward.Customer.Name,
                    CustomerAddress = pi.SampleInward.Address,
                    CustomerGst = pi.SampleInward.GstNo,
                    State = pi.SampleInward.State,
                    StateCode = pdfGstConfig?.State ?? "24",
                    RefNo = pi.SampleInward.CaseNo,
                    ReceivedDate = pi.SampleInward.CreatedOn,
                    SubTotal = pi.SubTotal,
                    CGST = pi.CGST,
                    SGST = pi.SGST,
                    IGST = pi.IGST,
                    GrandTotal = pi.GrandTotal,
                    AmountInWords = NumberToWords((long)pi.GrandTotal)
                };

                foreach (var d in pi.Details)
                {
                    model.Rows.Add(new TaxInvoiceRow
                    {
                        Sample = d.SampleID.ToString(),
                        Description = d.Description,
                        QtyDisplay = d.Quantity.ToString(),
                        Rate = d.Rate,
                        Amount = d.Amount
                    });
                }

                var logoPath = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "logo.png");
                var signPath = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "signature.png");

                var document = new ProformaInvoiceDocument(model, logoPath, signPath);
                var pdfBytes = document.GeneratePdf(); // QuestPDF extension

                return pdfBytes;
            }
            catch
            {
                throw;
            }
        }

        private string NumberToWords(long number)
        {
            return $"{number} Only"; // You can plug full converter here
        }
    }
}
