using LIMSApi.Data;
using LIMSApi.Dtos;
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

        public PriceCalculationService(LIMSContext db, ILogger<PriceCalculationService> logger)
        {
            _db = db;
            _logger = logger;
        }

        /// <summary>
        /// Calculate prices for a case and create ChargeEvents with DRAFT status
        /// Sets BillingStatus = PRICE_DRAFTED
        /// Returns detailed result with per-test success/failure info
        /// </summary>
        public async Task<PriceCalculationResultDto> CalculateAndCreateChargeEventsAsync(long inwardId)
        {
            var result = await CalculatePricingInternalAsync(inwardId, dryRun: false);
            return result;
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
                                    plan.ID
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
                                    usedElements
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
            long testPlanId)
        {
            // Get all InvoiceCaseConfigurations linked to this LaboratoryTest
            var configs = await _db.LaboratoryTestInvoiceCase
                .Where(lt => lt.LabTestID == laboratoryTestId)
                .Include(lt => lt.InvoiceCaseConfiguration)
                .Where(lt => lt.InvoiceCaseConfiguration != null && lt.InvoiceCaseConfiguration.IsActive)
                .Select(lt => lt.InvoiceCaseConfiguration!)
                .ToListAsync();

            if (!configs.Any())
                throw new Exception($"No pricing configuration found for LaboratoryTest {laboratoryTestId}");

            // Get SpecificationLines for this GeneralTest (mechanical type)
            var specificationLines = await _db.SpecificationLines
                .Where(sl => (sl.SpecificationGradeID == generalTest.Specification1 || 
                             (generalTest.Specification2.HasValue && sl.SpecificationGradeID == generalTest.Specification2.Value))
                            && sl.Type == "mechanical"
                            && sl.ParameterID.HasValue)
                .Include(sl => sl.Parameter)
                .ToListAsync();

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
                            parameterValue.Value
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
            int usedElements)
        {
            // Get all InvoiceCaseConfigurations linked to this LaboratoryTest
            var configs = await _db.LaboratoryTestInvoiceCase
                .Where(lt => lt.LabTestID == laboratoryTestId)
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
                    usedElements
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
                            parameterValue.Value
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
        /// Extract parameter value for a given configuration from SpecificationLine or TestResultParameter
        /// </summary>
        private Task<decimal?> ExtractParameterValueForConfigAsync(
            InvoiceCaseConfiguration config,
            List<SpecificationLine> specificationLines,
            ICollection<TestResultParameter>? testResultParameters,
            string parameterType)
        {
            // Get parameter names/aliases to match
            var configNames = new List<string> { config.Name };
            if (!string.IsNullOrWhiteSpace(config.AliasName))
            {
                configNames.AddRange(config.AliasName.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(a => a.Trim()));
            }

            // First, try to get from TestResultParameter (actual test results) — only billable ones
            if (testResultParameters != null && testResultParameters.Any())
            {
                // Filter only billable parameters for price matching
                var billableParams = testResultParameters.Where(trp => trp.IsBillable).ToList();
                var parameterIds = billableParams.Select(trp => trp.ParameterID).Distinct().ToList();
                var parameterMasters = _db.ParameterMasters
                    .Where(pm => parameterIds.Contains(pm.ID))
                    .ToDictionary(pm => pm.ID);

                foreach (var trp in billableParams)
                {
                    if (!parameterMasters.TryGetValue(trp.ParameterID, out var paramMaster))
                        continue;

                    var paramName = paramMaster.Name?.ToLower() ?? "";
                    var paramAlias = paramMaster.AliasName?.ToLower() ?? "";
                    var trpParamName = trp.ParameterName?.ToLower() ?? "";

                    foreach (var configName in configNames)
                    {
                        var configNameLower = configName.ToLower();
                        if ((paramName.Contains(configNameLower) || configNameLower.Contains(paramName) ||
                             paramAlias.Contains(configNameLower) || configNameLower.Contains(paramAlias) ||
                             trpParamName.Contains(configNameLower) || configNameLower.Contains(trpParamName)) &&
                            trp.Value.HasValue)
                        {
                            return Task.FromResult<decimal?>(trp.Value.Value);
                        }
                    }
                }
            }

            // If not found in test results, try SpecificationLine
            foreach (var specLine in specificationLines)
            {
                if (specLine.Parameter == null) continue;

                var paramName = specLine.Parameter.Name?.ToLower() ?? "";
                var paramAlias = specLine.Parameter.AliasName?.ToLower() ?? "";

                foreach (var configName in configNames)
                {
                    var configNameLower = configName.ToLower();
                    if (paramName.Contains(configNameLower) || configNameLower.Contains(paramName) ||
                        paramAlias.Contains(configNameLower) || configNameLower.Contains(paramAlias))
                    {
                        // For range types, we might need to use MinValue or MaxValue
                        // For single value types, we might need to use a default or calculated value
                        // For now, return MinValue if available, otherwise MaxValue
                        if (specLine.MinValue.HasValue)
                            return Task.FromResult<decimal?>(specLine.MinValue.Value);
                        if (specLine.MaxValue.HasValue)
                            return Task.FromResult<decimal?>(specLine.MaxValue.Value);
                    }
                }
            }

            return Task.FromResult<decimal?>(null);
        }

        /// <summary>
        /// Match configuration against parameter value and get rate
        /// </summary>
        private async Task<(decimal Rate, long ConfigId)> MatchConfigAndGetRateAsync(
            long laboratoryTestId,
            InvoiceCaseConfiguration config,
            decimal usedValue)
        {
            // Get Invoice Case for this Lab Test
            var invoiceCase = await _db.InvoiceCases
                .Where(x => x.LaboratoryTestID == laboratoryTestId && x.IsActive)
                .Include(x => x.InvoiceCasePrices)
                .FirstOrDefaultAsync();

            if (invoiceCase == null)
                throw new Exception($"No invoice case found for LaboratoryTest {laboratoryTestId}");

            // Check if this config is used in the invoice case
            var configPrice = invoiceCase.InvoiceCasePrices
                .FirstOrDefault(p => p.InvoiceCaseConfigID == config.ID);

            if (configPrice == null)
                throw new Exception($"Configuration {config.ID} not found in invoice case prices");

            // Determine if this is a range type or single value type
            var isRangeType = config.SelectionType.EndsWith("Range", StringComparison.OrdinalIgnoreCase);

            if (isRangeType)
            {
                // Range type: check if usedValue falls within Start and End
                if (string.IsNullOrWhiteSpace(config.Start) || string.IsNullOrWhiteSpace(config.End))
                    throw new Exception($"Configuration {config.ID} is a range type but Start or End is missing");

                var startValue = decimal.Parse(config.Start);
                var endValue = decimal.Parse(config.End);

                if (usedValue >= startValue && usedValue <= endValue)
                {
                    return (configPrice.Price, config.ID);
                }
            }
            else
            {
                // Single value type: find the nearest higher or equal slab
                // Get all configs of the same SelectionType
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
                        return (matchingPrice.Price, matchingConfig.ID);
                    }
                }
            }

            throw new Exception($"No pricing slab found for value {usedValue} under SelectionType {config.SelectionType}");
        }

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

            // Check if all samples have FINAL_REPORT_APPROVED status
            var allSamplesApproved = samples.All(s => 
                s.SampleStatus == SampleStatus.FINAL_REPORT_APPROVED.ToString());

            if (!allSamplesApproved)
            {
                var unapprovedSamples = samples
                    .Where(s => s.SampleStatus != SampleStatus.FINAL_REPORT_APPROVED.ToString())
                    .Select(s => s.SampleNo)
                    .ToList();
                
                throw new InvalidOperationException(
                    $"Cannot create price snapshot. Not all samples have been approved (FINAL_REPORT_APPROVED). " +
                    $"Unapproved samples: {string.Join(", ", unapprovedSamples)}");
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
    }
}

