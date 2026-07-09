using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Helpers.Enums;
using LIMSApi.Models;
using LIMSApi.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Services
{
    /// <summary>
    /// PriceCalculationService: Generic price calculation and ChargeEvent generation
    /// Extracts pricing logic from ProformaInvoiceRepository and creates ChargeEvents
    /// </summary>
    public class PriceCalculationService : IPriceCalculationService
    {
        private readonly LIMSContext _db;
        private readonly ILogger<PriceCalculationService> _logger;
        private readonly IFinancialYearService _fyService;
        private readonly IPricingEngine _pricingEngine;

        public PriceCalculationService(LIMSContext db, ILogger<PriceCalculationService> logger, IFinancialYearService fyService, IPricingEngine pricingEngine)
        {
            _db = db;
            _logger = logger;
            _fyService = fyService;
            _pricingEngine = pricingEngine;
        }

        /// <summary>
        /// Calculate prices for a case and create ChargeEvents with DRAFT status
        /// Sets BillingStatus = PRICE_DRAFTED
        /// Returns detailed result with per-test success/failure info
        /// </summary>
        public async Task<PriceCalculationResultDto> CalculateAndCreateChargeEventsAsync(long inwardId, bool confirmed = false)
        {
            if (!confirmed)
            {
                var mismatch = await CheckFinancialYearMismatchAsync(inwardId);
                if (mismatch != null)
                    return mismatch;
            }

            var result = await CalculatePricingInternalAsync(inwardId, dryRun: false);
            return result;
        }

        /// <summary>
        /// Returns a RequiresConfirmation result when the sample's inward FY differs from the
        /// current default FY (older price list will be applied). Returns null when there is no
        /// mismatch, or when FY data is missing (calculation proceeds normally).
        /// </summary>
        private async Task<PriceCalculationResultDto?> CheckFinancialYearMismatchAsync(long inwardId)
        {
            var inward = await _db.SampleInwards
                .Include(x => x.FinancialYear)
                .FirstOrDefaultAsync(x => x.ID == inwardId);

            if (inward?.FinancialYearId == null)
                return null;

            var currentFy = await _db.FinancialYears.FirstOrDefaultAsync(f => f.IsCurrent);
            if (currentFy == null || inward.FinancialYearId == currentFy.Id)
                return null;

            var inwardFy = inward.FinancialYear?.Year ?? "(unknown)";
            return new PriceCalculationResultDto
            {
                InwardId = inwardId,
                CaseNo = inward.CaseNo ?? "",
                RequiresConfirmation = true,
                InwardFY = inwardFy,
                CurrentFY = currentFy.Year,
                ConfirmationMessage = $"Sample is from FY {inwardFy} — the {inwardFy} price list will be applied " +
                                      $"instead of the current FY {currentFy.Year}. Proceed?"
            };
        }

        /// <summary>
        /// Validate pricing without saving — dry run that returns what would succeed/fail
        /// </summary>
        public async Task<PriceCalculationResultDto> ValidatePricingAsync(long inwardId)
        {
            return await CalculatePricingInternalAsync(inwardId, dryRun: true);
        }

        /// <summary>
        /// Internal pricing logic shared by Calculate and Validate
        /// </summary>
        private async Task<PriceCalculationResultDto> CalculatePricingInternalAsync(long inwardId, bool dryRun)
        {
            var pricingResult = new PriceCalculationResultDto { InwardId = inwardId };

            var inward = await _db.SampleInwards
                .Include(x => x.Customer)
                .FirstOrDefaultAsync(x => x.ID == inwardId);

            if (inward == null)
                throw new Exception("Case not found");

            pricingResult.CaseNo = inward.CaseNo ?? "";

            if (!dryRun)
            {
                // Block recalculation if events already locked (SNAPSHOT or INVOICED)
                var hasLockedEvents = await _db.ChargeEvents
                    .AnyAsync(x => x.InwardID == inwardId &&
                        (x.Status == ChargeEventStatus.SNAPSHOT.ToString() ||
                         x.Status == ChargeEventStatus.INVOICED.ToString()));

                if (hasLockedEvents)
                {
                    pricingResult.Warnings.Add("Cannot recalculate — ChargeEvents already in SNAPSHOT/INVOICED status.");
                    _logger.LogWarning("Cannot recalculate for Case {CaseNo}: ChargeEvents already in SNAPSHOT/INVOICED status.", inward.CaseNo);
                    return pricingResult;
                }

                // Delete existing DRAFTs for fresh recalculation
                var existingDrafts = await _db.ChargeEvents
                    .Where(x => x.InwardID == inwardId && x.Status == ChargeEventStatus.DRAFT.ToString())
                    .ToListAsync();

                if (existingDrafts.Any())
                {
                    _db.ChargeEvents.RemoveRange(existingDrafts);
                    pricingResult.Warnings.Add($"Recalculating: removed {existingDrafts.Count} existing DRAFT events.");
                    _logger.LogInformation("Recalculating for Case {CaseNo}: removed {Count} existing DRAFT ChargeEvents.", inward.CaseNo, existingDrafts.Count);
                }
            }

            var chargeEvents = new List<ChargeEvent>();

            // 1. CUTTING CHARGES
            var cuttingHeader = await _db.CuttingChargeHeaders
                .Include(x => x.Samples)
                .FirstOrDefaultAsync(x => x.InwardID == inwardId);

            if (cuttingHeader != null && cuttingHeader.GrandTotal > 0)
            {
                chargeEvents.Add(new ChargeEvent
                {
                    InwardID = inwardId,
                    ChargeType = "Cutting",
                    Description = "Sample Cutting Charges",
                    Quantity = 1,
                    Rate = cuttingHeader.GrandTotal,
                    Amount = cuttingHeader.GrandTotal,
                    Status = ChargeEventStatus.DRAFT.ToString(),
                    CreatedOn = DateTime.UtcNow
                });
                pricingResult.TestResults.Add(new PriceLineResultDto
                {
                    ChargeType = "Cutting",
                    TestName = "Sample Cutting Charges",
                    Success = true,
                    Amount = cuttingHeader.GrandTotal
                });
                pricingResult.SuccessCount++;
            }

            // 2. MACHINING / PREPARATION CHARGES
            var machiningCharges = await _db.SampleDetails
                .Where(x => x.InwardID == inwardId && (x.MachiningRequired || x.OtherPreparation))
                .ToListAsync();

            foreach (var sample in machiningCharges)
            {
                var totalPrep = sample.MachiningAmount + sample.OtherPreparationCharge;
                if (totalPrep > 0)
                {
                    chargeEvents.Add(new ChargeEvent
                    {
                        InwardID = inwardId,
                        SampleID = sample.ID,
                        ChargeType = "Preparation",
                        Description = $"Sample Preparation - {sample.SampleNo}",
                        Quantity = 1,
                        Rate = totalPrep,
                        Amount = totalPrep,
                        Status = ChargeEventStatus.DRAFT.ToString(),
                        CreatedOn = DateTime.UtcNow
                    });
                    pricingResult.TestResults.Add(new PriceLineResultDto
                    {
                        SampleId = sample.ID,
                        SampleNo = sample.SampleNo ?? "",
                        ChargeType = "Preparation",
                        TestName = $"Sample Preparation - {sample.SampleNo}",
                        Success = true,
                        Amount = totalPrep
                    });
                    pricingResult.SuccessCount++;
                }
            }

            // 2b. CUSTOM PREPARATION CHARGES (MachiningChargeItems — free-text manual entries per sample)
            var customPrepCharges = await _db.MachiningChargeItems
                .Where(x => _db.SampleDetails.Any(s => s.ID == x.SampleID && s.InwardID == inwardId) && x.IsActive)
                .ToListAsync();

            foreach (var item in customPrepCharges)
            {
                if (item.Amount > 0)
                {
                    chargeEvents.Add(new ChargeEvent
                    {
                        InwardID = inwardId,
                        SampleID = item.SampleID,
                        ChargeType = "CustomPreparation",
                        Description = $"Custom Preparation - {item.Description}",
                        Quantity = 1,
                        Rate = item.Amount,
                        Amount = item.Amount,
                        Status = ChargeEventStatus.DRAFT.ToString(),
                        CreatedOn = DateTime.UtcNow
                    });
                    pricingResult.TestResults.Add(new PriceLineResultDto
                    {
                        SampleId = item.SampleID,
                        ChargeType = "CustomPreparation",
                        TestName = $"Custom Preparation - {item.Description}",
                        Success = true,
                        Amount = item.Amount
                    });
                    pricingResult.SuccessCount++;
                }
            }

            // 3. LAB TEST CHARGES
            var sampleDetails = await _db.SampleDetails
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
                    // GENERAL TESTS
                    foreach (var gt in plan.GeneralTests)
                    {
                        foreach (var method in gt.Methods)
                        {
                            try
                            {
                                var calcResult = await GetRateForGeneralTestAsync(
                                    method.LaboratoryTestID,
                                    gt,
                                    method,
                                    plan.ID,
                                    inward.CollectionTime
                                );
                                var rate = calcResult.Rate;
                                var configId = calcResult.ConfigId;
                                var selectionType = calcResult.SelectionType;
                                var usedValue = calcResult.UsedValue;

                                var amount = rate * method.Quantity;

                                chargeEvents.Add(new ChargeEvent
                                {
                                    InwardID = inwardId,
                                    SampleID = sd.ID,
                                    ChargeType = "Test",
                                    Description = $"General Test - {selectionType ?? "Default"}",
                                    Quantity = method.Quantity,
                                    Rate = rate,
                                    Amount = amount,
                                    Status = ChargeEventStatus.DRAFT.ToString(),
                                    SelectionType = selectionType,
                                    UsedValue = usedValue ?? 0,
                                    InvoiceCaseConfigID = configId,
                                    CreatedOn = DateTime.UtcNow
                                });
                                pricingResult.TestResults.Add(new PriceLineResultDto
                                {
                                    SampleId = sd.ID,
                                    SampleNo = sd.SampleNo ?? "",
                                    ChargeType = "General",
                                    TestName = $"General Test - {selectionType ?? "Default"} (Method {method.ID})",
                                    Success = true,
                                    Amount = amount
                                });
                                pricingResult.SuccessCount++;
                            }
                            catch (Exception ex)
                            {
                                var reason = ex.Message;
                                _logger.LogWarning(ex, "Failed to calculate rate for GeneralTest method {MethodID}: {Reason}", method.ID, reason);
                                pricingResult.TestResults.Add(new PriceLineResultDto
                                {
                                    SampleId = sd.ID,
                                    SampleNo = sd.SampleNo ?? "",
                                    ChargeType = "General",
                                    TestName = $"General Test (Method {method.ID})",
                                    Success = false,
                                    FailureReason = reason
                                });
                                pricingResult.FailureCount++;
                                pricingResult.Errors.Add($"Sample {sd.SampleNo} - General Test Method {method.ID}: {reason}");
                            }
                        }
                    }

                    // CHEMICAL TESTS
                    foreach (var ct in plan.ChemicalTests)
                    {
                        var usedElements = ct.Elements.Count(x => x.Selected);

                        foreach (var tt in ct.TestTypes.Where(t => t.IsSelected && t.LaboratoryTestID.HasValue))
                        {
                            try
                            {
                                var calcResult = await GetRateForChemicalTestAsync(
                                    tt.LaboratoryTestID!.Value,
                                    ct,
                                    usedElements,
                                    inward.CollectionTime
                                );
                                var rate = calcResult.Rate;
                                var configId = calcResult.ConfigId;
                                var selectionType = calcResult.SelectionType;
                                var usedValue = calcResult.UsedValue;

                                chargeEvents.Add(new ChargeEvent
                                {
                                    InwardID = inwardId,
                                    SampleID = sd.ID,
                                    ChargeType = "Test",
                                    Description = $"Chemical Test - {tt.Name}",
                                    Quantity = 1,
                                    Rate = rate,
                                    Amount = rate,
                                    Status = ChargeEventStatus.DRAFT.ToString(),
                                    SelectionType = selectionType,
                                    UsedValue = usedValue ?? 0,
                                    InvoiceCaseConfigID = configId,
                                    CreatedOn = DateTime.UtcNow
                                });
                                pricingResult.TestResults.Add(new PriceLineResultDto
                                {
                                    SampleId = sd.ID,
                                    SampleNo = sd.SampleNo ?? "",
                                    ChargeType = "Chemical",
                                    TestName = $"Chemical Test - {tt.Name}",
                                    Success = true,
                                    Amount = rate
                                });
                                pricingResult.SuccessCount++;
                            }
                            catch (Exception ex)
                            {
                                var reason = ex.Message;
                                _logger.LogWarning(ex, "Failed to calculate rate for ChemicalTest type {TestTypeID}: {Reason}", tt.ID, reason);
                                pricingResult.TestResults.Add(new PriceLineResultDto
                                {
                                    SampleId = sd.ID,
                                    SampleNo = sd.SampleNo ?? "",
                                    ChargeType = "Chemical",
                                    TestName = $"Chemical Test - {tt.Name}",
                                    Success = false,
                                    FailureReason = reason
                                });
                                pricingResult.FailureCount++;
                                pricingResult.Errors.Add($"Sample {sd.SampleNo} - Chemical Test {tt.Name}: {reason}");
                            }
                        }
                    }
                }
            }

            pricingResult.TotalAmount = chargeEvents.Sum(x => x.Amount);

            // ── MINIMUM CHARGE CHECK ──
            // Separate minimums for test cases vs prep-only cases
            if (pricingResult.TotalAmount > 0)
            {
                var hasTestCharges = chargeEvents.Any(e => e.ChargeType == "Test");
                var hasPrepCharges = chargeEvents.Any(e =>
                    e.ChargeType == "Cutting" || e.ChargeType == "Preparation" || e.ChargeType == "CustomPreparation");
                var isPrepOnly = !hasTestCharges && hasPrepCharges;
                var configKey = isPrepOnly ? "MINIMUM_CHARGE_PREP" : "MINIMUM_CHARGE_TEST";

                var minConfig = await _db.Configurations
                    .FirstOrDefaultAsync(c => c.KeyName == configKey && c.GroupName == "BILLING" && c.IsActive);

                if (minConfig != null && decimal.TryParse(minConfig.Value, out var minCharge) && minCharge > 0)
                {
                    if (pricingResult.TotalAmount < minCharge)
                    {
                        var adjustment = Math.Round(minCharge - pricingResult.TotalAmount, 2, MidpointRounding.AwayFromZero);
                        chargeEvents.Add(new ChargeEvent
                        {
                            InwardID = inwardId,
                            ChargeType = "MinimumChargeAdjustment",
                            Description = $"Minimum charge adjustment (Min: {minCharge:0.00})",
                            Quantity = 1,
                            Rate = adjustment,
                            Amount = adjustment,
                            Status = ChargeEventStatus.DRAFT.ToString(),
                            CreatedOn = DateTime.UtcNow
                        });
                        pricingResult.TotalAmount = minCharge;
                        pricingResult.Warnings.Add($"Minimum charge of {minCharge:0.00} applied (type: {(isPrepOnly ? "Prep-Only" : "Test")}). Adjustment: +{adjustment:0.00}");
                        pricingResult.SuccessCount++;
                        _logger.LogInformation("Minimum charge applied for Case {CaseNo}: {ConfigKey}={MinCharge}, Adjustment={Adjustment}",
                            inward.CaseNo, configKey, minCharge, adjustment);
                    }
                }
            }

            if (!dryRun)
            {
                // Save all ChargeEvents
                if (chargeEvents.Any())
                {
                    await _db.ChargeEvents.AddRangeAsync(chargeEvents);
                }

                // Update inward billing status and total
                inward.BillingStatus = BillingStatus.PRICE_DRAFTED.ToString();
                inward.TotalTestCharges = pricingResult.TotalAmount;

                await _db.SaveChangesAsync();
            }

            if (pricingResult.HasFailures)
            {
                _logger.LogWarning("Price calculation for Case {CaseNo}: {SuccessCount} succeeded, {FailureCount} failed. Total: {Total}",
                    inward.CaseNo, pricingResult.SuccessCount, pricingResult.FailureCount, pricingResult.TotalAmount);
            }
            else
            {
                _logger.LogInformation("Created {Count} ChargeEvents for Case {CaseNo} with total {Total}",
                    chargeEvents.Count, inward.CaseNo, pricingResult.TotalAmount);
            }

            return pricingResult;
        }

        /// <summary>
        /// Get rate for General Test by extracting parameter values from SpecificationLine
        /// </summary>
        private async Task<(decimal Rate, long ConfigId, string? SelectionType, decimal? UsedValue)> GetRateForGeneralTestAsync(
            long laboratoryTestId,
            GeneralTest generalTest,
            GeneralTestMethod method,
            long testPlanId,
            DateTime inwardDate)
        {
            // Get all InvoiceCaseConfigurations linked to this General Test's SubGroup
            var configs = await _db.LaboratoryTestSubGroupInvoiceCases
                .Where(lt => lt.LaboratoryTestSubGroupID == generalTest.LaboratoryTestSubGroupID)
                .Include(lt => lt.InvoiceCaseConfiguration)
                .Where(lt => lt.InvoiceCaseConfiguration != null && lt.InvoiceCaseConfiguration.IsActive)
                .Select(lt => lt.InvoiceCaseConfiguration!)
                .ToListAsync();

            if (!configs.Any())
                throw new Exception($"No pricing configuration found for LaboratoryTest {laboratoryTestId}");

            // Get sample ID from the test plan - get it from the generalTest's SampleTestPlan
            var testPlan = await _db.TestPlans
                .FirstOrDefaultAsync(tp => tp.ID == testPlanId);
            
            if (testPlan == null)
                throw new Exception($"TestPlan {testPlanId} not found");

            // Try to get TestResultParameter values if test results are available
            var testResultHeader = await _db.TestResultHeaders
                .Where(trh => trh.LaboratoryTestID == laboratoryTestId 
                             && trh.TestPlanID == testPlanId
                             && trh.SampleID == testPlan.SampleID)
                .Include(trh => trh.Parameters)
                .FirstOrDefaultAsync();

            var ecfConfigs = configs.Where(c => c.SelectionType == "ElementCountFormula").ToList();
            if (ecfConfigs.Any())
            {
                var selectedElementParamIds = testResultHeader?.Parameters
                    .Where(p => p.IsBillable)
                    .Select(p => p.ParameterID)
                    .ToList() ?? new List<long>();

                if (!selectedElementParamIds.Any())
                {
                    // Get SpecificationLines for this GeneralTest (mechanical type) to resolve fallback parameter IDs
                    var specLines = await _db.SpecificationLines
                        .Where(sl => (sl.SpecificationGradeID == generalTest.Specification1 || 
                                     (generalTest.Specification2.HasValue && sl.SpecificationGradeID == generalTest.Specification2.Value))
                                    && sl.Type == "mechanical"
                                    && sl.ParameterID.HasValue)
                        .ToListAsync();

                    selectedElementParamIds = specLines
                        .Select(sl => sl.ParameterID!.Value)
                        .Distinct()
                        .ToList();
                }

                return await CalculateElementCountFormulaRateAsync(
                    laboratoryTestId,
                    ecfConfigs,
                    selectedElementParamIds,
                    selectedElementParamIds.Count,
                    inwardDate
                );
            }

            // Get SpecificationLines for this GeneralTest (mechanical type)
            var specificationLines = await _db.SpecificationLines
                .Where(sl => (sl.SpecificationGradeID == generalTest.Specification1 || 
                             (generalTest.Specification2.HasValue && sl.SpecificationGradeID == generalTest.Specification2.Value))
                            && sl.Type == "mechanical"
                            && sl.ParameterID.HasValue)
                .Include(sl => sl.Parameter)
                .ToListAsync();

            // For each configuration, try to find matching parameter and calculate rate
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
                        var (rate, configId) = await MatchConfigAndGetRateAsync(
                            laboratoryTestId,
                            config,
                            parameterValue.Value,
                            inwardDate
                        );

                        return (rate, configId, config.SelectionType, parameterValue.Value);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogDebug(ex, "Failed to match config {ConfigId} with SelectionType {SelectionType}",
                        config.ID, config.SelectionType);
                    continue;
                }
            }

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
            var configs = await _db.LaboratoryTestAnalysisTypeInvoiceCases
                .Where(lt => lt.LaboratoryTestAnalysisTypeID == chemicalTest.LaboratoryTestAnalysisTypeID)
                .Include(lt => lt.InvoiceCaseConfiguration)
                .Where(lt => lt.InvoiceCaseConfiguration != null && lt.InvoiceCaseConfiguration.IsActive)
                .Select(lt => lt.InvoiceCaseConfiguration!)
                .ToListAsync();

            if (!configs.Any())
                throw new Exception($"No pricing configuration found for LaboratoryTest {laboratoryTestId}");

            var ecfConfigs = configs.Where(c => c.SelectionType == "ElementCountFormula").ToList();
            if (ecfConfigs.Any())
            {
                var selectedElementParamIds = chemicalTest.Elements
                    .Where(e => e.Selected && e.ParameterID > 0)
                    .Select(e => e.ParameterID)
                    .ToList();

                return await CalculateElementCountFormulaRateAsync(
                    laboratoryTestId,
                    ecfConfigs,
                    selectedElementParamIds,
                    usedElements,
                    inwardDate,
                    chemicalTest.LaboratoryTestAnalysisTypeID
                );
            }

            // For Element type, use element count
            var elementConfig = configs.FirstOrDefault(c => c.SelectionType == "Element");
            if (elementConfig != null)
            {
                var (rate, configId) = await MatchConfigAndGetRateAsync(
                    laboratoryTestId,
                    elementConfig,
                    usedElements,
                    inwardDate,
                    chemicalTest.LaboratoryTestAnalysisTypeID
                );

                return (rate, configId, "Element", usedElements);
            }

            // For other types, get SpecificationLines from ChemicalTestElement
            var specificationLineIds = chemicalTest.Elements
                .Where(e => e.Selected)
                .Select(e => e.SpecificationLineID)
                .ToList();

            var specificationLines = await _db.SpecificationLines
                .Where(sl => specificationLineIds.Contains(sl.ID)
                            && sl.Type == "chemical"
                            && sl.ParameterID.HasValue)
                .Include(sl => sl.Parameter)
                .ToListAsync();

            // Try to get TestResultParameter values if test results are available
            var testResultHeader = await _db.TestResultHeaders
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
                            inwardDate,
                            chemicalTest.LaboratoryTestAnalysisTypeID
                        );

                        return (rate, configId, config.SelectionType, parameterValue.Value);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogDebug(ex, "Failed to match config {ConfigId} with SelectionType {SelectionType}",
                        config.ID, config.SelectionType);
                    continue;
                }
            }

            throw new Exception($"No matching pricing configuration found for LaboratoryTest {laboratoryTestId} with available parameters");
        }

        // ExtractParameterValueForConfigAsync and MatchConfigAndGetRateAsync
        // are now delegated to the shared PricingEngine (IPricingEngine)
        // to eliminate code duplication with ProformaInvoiceRepository.

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
            DateTime inwardDate,
            long? analysisTypeId = null)
            => _pricingEngine.MatchConfigAndGetRateAsync(laboratoryTestId, config, usedValue, inwardDate, analysisTypeId);

        public async Task<decimal> GetDraftTotalAsync(long inwardId)
        {
            return await _db.ChargeEvents
                .Where(x => x.InwardID == inwardId && x.Status == ChargeEventStatus.DRAFT.ToString())
                .SumAsync(x => x.Amount);
        }

        public async Task<List<Models.ChargeEvent>> GetChargeEventsAsync(long inwardId, string? status = null)
        {
            var query = _db.ChargeEvents
                .Where(x => x.InwardID == inwardId);

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(x => x.Status == status);
            }

            return await query.ToListAsync();
        }

        public async Task CreatePriceSnapshotAsync(long inwardId)
        {
            var inward = await _db.SampleInwards
                .FirstOrDefaultAsync(x => x.ID == inwardId);

            if (inward == null)
                throw new Exception("Case not found");

            // VALIDATION: Price snapshot can only be created after all test results are APPROVED (FINAL_REPORT_APPROVED)
            var samples = await _db.SampleDetails
                .Where(s => s.InwardID == inwardId && s.IsActive)
                .ToListAsync();

            if (!samples.Any())
                throw new Exception("No samples found for this inward.");

            // Check if all samples have reached report approval stage
            // Accept FINAL_REPORT_APPROVED and statuses that come after it
            var approvedStatuses = new HashSet<string>
            {
                "FINAL_REPORT_APPROVED", "REPORT_DISPATCHED",
                "ADVANCE_PAYMENT_COMPLETED",
                "PAYMENT_PENDING", "PAYMENT_COMPLETED", "COMPLETED", "CASE_CLOSED"
            };
            var allSamplesApproved = samples
                .Where(s => !s.IsCancelled)   // excluded cancelled samples from completion check
                .All(s => approvedStatuses.Contains(s.SampleStatus ?? ""));

            if (!allSamplesApproved)
            {
                var unapprovedSamples = samples
                    .Where(s => !s.IsCancelled && !approvedStatuses.Contains(s.SampleStatus ?? ""))
                    .Select(s => $"{s.SampleNo} ({s.SampleStatus})")
                    .ToList();

                throw new InvalidOperationException(
                    $"Cannot create price snapshot. Not all samples have completed reporting. " +
                    $"Pending samples: {string.Join(", ", unapprovedSamples)}");
            }

            // Get all DRAFT ChargeEvents (including Customer Amendment ChargeEvents)
            // Note: Internal amendments do NOT create ChargeEvents, so they are not included here
            var draftEvents = await _db.ChargeEvents
                .Where(x => x.InwardID == inwardId && x.Status == ChargeEventStatus.DRAFT.ToString())
                .ToListAsync();

            if (!draftEvents.Any())
                throw new Exception("No DRAFT ChargeEvents found. Price calculation must be done first.");

            // Move to SNAPSHOT
            foreach (var evt in draftEvents)
            {
                evt.Status = ChargeEventStatus.SNAPSHOT.ToString();
                evt.SnapshotDate = DateTime.UtcNow;
                evt.ModifiedOn = DateTime.UtcNow;
            }

            // Update billing status
            inward.BillingStatus = BillingStatus.PRICE_SNAPSHOT.ToString();
            inward.ModifiedOn = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            _logger.LogInformation("Created price snapshot for Case {CaseNo}: {Count} ChargeEvents moved to SNAPSHOT",
                inward.CaseNo, draftEvents.Count);
        }

        private async Task<(decimal Rate, long ConfigId, string? SelectionType, decimal? UsedValue)> CalculateElementCountFormulaRateAsync(
            long laboratoryTestId,
            List<InvoiceCaseConfiguration> ecfConfigs,
            List<long> selectedElementParamIds,
            int elementCount,
            DateTime inwardDate,
            long? analysisTypeId = null)
        {
            var versions = await _db.InvoiceCases
                .Where(ic => ic.LaboratoryTestID == laboratoryTestId && ic.AnalysisTypeID == analysisTypeId && ic.IsActive)
                .Include(ic => ic.InvoiceCasePrices)
                .ToListAsync();

            var invoiceCase = PriceVersionResolver.Resolve(versions, v => v.EffectiveFrom, inwardDate);
            if (invoiceCase == null)
                throw new Exception($"No active pricing version found for LaboratoryTest {laboratoryTestId}");

            var baseTiers = ecfConfigs.Where(c => Helpers.ConditionMatcher.IsBaseTier(c.Value)).ToList();
            var overrides = ecfConfigs.Where(c => Helpers.ConditionMatcher.IsOverride(c.Value)).ToList();

            InvoiceCaseConfiguration? matchedBaseConfig = null;
            int baseCountLimit = 0;
            foreach (var bt in baseTiers)
            {
                if (Helpers.ConditionMatcher.MatchesCount(bt.Value, elementCount))
                {
                    matchedBaseConfig = bt;
                    baseCountLimit = Helpers.ConditionMatcher.ParseCountLimit(bt.Value);
                    break;
                }
            }

            decimal basePrice = 0;
            long primaryConfigId = 0;

            if (matchedBaseConfig != null)
            {
                var priceEntry = invoiceCase.InvoiceCasePrices.FirstOrDefault(p => p.InvoiceCaseConfigID == matchedBaseConfig.ID);
                if (priceEntry != null)
                {
                    basePrice = priceEntry.Price;
                    primaryConfigId = matchedBaseConfig.ID;
                }
            }

            decimal baseUnitDeduction = 0;
            var oneElementTier = baseTiers.FirstOrDefault(t => Helpers.ConditionMatcher.ParseCountLimit(t.Value) == 1);
            if (oneElementTier != null)
            {
                var oneElementPriceEntry = invoiceCase.InvoiceCasePrices.FirstOrDefault(p => p.InvoiceCaseConfigID == oneElementTier.ID);
                if (oneElementPriceEntry != null)
                {
                    baseUnitDeduction = oneElementPriceEntry.Price;
                }
            }

            var parameterDetails = await _db.ParameterMasters
                .Where(p => selectedElementParamIds.Contains(p.ID) && p.IsActive)
                .ToListAsync();

            var specialOrSuperParamIds = parameterDetails
                .Where(p => string.Equals(p.ElementType, "special", StringComparison.OrdinalIgnoreCase) ||
                            string.Equals(p.ElementType, "super", StringComparison.OrdinalIgnoreCase))
                .Select(p => p.ID)
                .ToList();

            var sortedSelectedParamIds = selectedElementParamIds.OrderBy(id => {
                bool isOverride = overrides.Any(o => {
                    if (string.IsNullOrEmpty(o.OverrideParameterIDs))
                        return specialOrSuperParamIds.Contains(id);
                    var ids = o.OverrideParameterIDs.Split(',').Select(idStr => long.TryParse(idStr.Trim(), out var pId) ? pId : 0).ToList();
                    return ids.Contains(id);
                });
                return isOverride ? 1 : 0;
            }).ToList();

            decimal subsumedDeductions = 0;
            decimal overrideSurcharges = 0;

            for (int i = 0; i < sortedSelectedParamIds.Count; i++)
            {
                long paramId = sortedSelectedParamIds[i];

                // 1. Check for specific override
                var matchingOverrideConfig = overrides.FirstOrDefault(o => {
                    if (string.IsNullOrEmpty(o.OverrideParameterIDs)) return false;
                    var ids = o.OverrideParameterIDs.Split(',').Select(idStr => long.TryParse(idStr.Trim(), out var pId) ? pId : 0).ToList();
                    return ids.Contains(paramId);
                });

                // 2. Fallback to wildcard override if parameter is special/super
                if (matchingOverrideConfig == null && specialOrSuperParamIds.Contains(paramId))
                {
                    matchingOverrideConfig = overrides.FirstOrDefault(o => string.IsNullOrEmpty(o.OverrideParameterIDs));
                }

                if (matchingOverrideConfig != null)
                {
                    var overridePriceEntry = invoiceCase.InvoiceCasePrices.FirstOrDefault(p => p.InvoiceCaseConfigID == matchingOverrideConfig.ID);
                    if (overridePriceEntry != null)
                    {
                        decimal overridePrice = overridePriceEntry.Price;
                        if (!string.IsNullOrEmpty(overridePriceEntry.ElementPrices))
                        {
                            try
                            {
                                var dict = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, decimal>>(overridePriceEntry.ElementPrices);
                                if (dict != null)
                                {
                                    // 1. Look up specific parameter ID (Case B)
                                    if (dict.TryGetValue(paramId.ToString(), out decimal elementSpecificPrice))
                                    {
                                        overridePrice = elementSpecificPrice;
                                    }
                                    else
                                    {
                                        // Get parameter element type
                                        var pInfo = parameterDetails.FirstOrDefault(p => p.ID == paramId);
                                        string? elType = pInfo?.ElementType?.ToLower()?.Trim();
                                        if (!string.IsNullOrEmpty(elType) && dict.TryGetValue(elType, out decimal typeSpecificPrice))
                                        {
                                            // 2. Look up parameter type: normal, special, super (Case A)
                                            overridePrice = typeSpecificPrice;
                                        }
                                    }
                                }
                            }
                            catch
                            {
                                // fallback to flat rate if JSON deserialization fails
                            }
                        }

                        overrideSurcharges += overridePrice;
                        if (primaryConfigId == 0) primaryConfigId = matchingOverrideConfig.ID;

                        int position = i + 1;
                        if (position <= baseCountLimit)
                        {
                            subsumedDeductions += baseUnitDeduction;
                        }
                    }
                }
            }

            decimal totalRate = basePrice - subsumedDeductions + overrideSurcharges;
            return (totalRate, primaryConfigId, "ElementCountFormula", (decimal)elementCount);
        }
    }
}

