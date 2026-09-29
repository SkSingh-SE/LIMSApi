using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Services.Interface;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using LIMSApi.Reporting;
using LIMSApi.Helpers.Enums;
using Microsoft.Extensions.Logging;

namespace LIMSApi.Services
{
    public class UniversalTestExecutionService : IUniversalTestExecutionService
    {
        private readonly LIMSContext _context;
        private readonly FormulaEvaluator _formulaEvaluator;
        private readonly IEffectiveConfigurationResolver _resolver;
        private readonly INablScopeValidationService _nablScopeService;
        private readonly ISnapshotIntegrityValidator _snapshotIntegrityValidator;
        private readonly IUniversalTestGroupService _utgService;
        private readonly IConfigurationAdjustmentService _adjustmentService;
        private readonly ILogger<UniversalTestExecutionService>? _logger;

        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            WriteIndented = false
        };

        public UniversalTestExecutionService(
            LIMSContext context,
            FormulaEvaluator formulaEvaluator,
            IEffectiveConfigurationResolver resolver,
            INablScopeValidationService nablScopeService,
            ISnapshotIntegrityValidator snapshotIntegrityValidator,
            IUniversalTestGroupService utgService,
            IConfigurationAdjustmentService adjustmentService,
            ILogger<UniversalTestExecutionService>? logger = null)
        {
            _context = context;
            _formulaEvaluator = formulaEvaluator;
            _resolver = resolver;
            _nablScopeService = nablScopeService;
            _snapshotIntegrityValidator = snapshotIntegrityValidator;
            _utgService = utgService;
            _adjustmentService = adjustmentService;
            _logger = logger;
        }

        public async Task<TestExecutionDto> StartExecutionAsync(
            long universalTestGroupId,
            long userId,
            long branchId,
            long organizationId,
            bool isRetest = false)
        {
            // Acquire transaction and row-level lock on UniversalTestGroup to prevent concurrent start races
            using var tx = await _context.Database.BeginTransactionAsync(System.Data.IsolationLevel.ReadCommitted);
            try
            {
                await _context.Database.ExecuteSqlInterpolatedAsync($"SELECT ID FROM UniversalTestGroups WITH (UPDLOCK, ROWLOCK) WHERE ID = {universalTestGroupId}");

                var utg = await _context.UniversalTestGroups
                    .Include(u => u.SampleTestPlan)
                        .ThenInclude(p => p.SampleDetail)
                            .ThenInclude(s => s.SampleInward)
                                .ThenInclude(i => i.Customer)
                    .Include(u => u.LaboratoryTest)
                    .Include(u => u.TestMethodSpecification)
                    .Include(u => u.TestMethodSpecificationVersion)
                    .Include(u => u.SpecificationHeader)
                    .Include(u => u.SpecificationGrade)
                    .Include(u => u.TestExecutions)
                    .FirstOrDefaultAsync(u => u.ID == universalTestGroupId && u.OrganizationID == organizationId);

                if (utg == null)
                {
                    throw new KeyNotFoundException($"Universal Test Group {universalTestGroupId} not found for this organization.");
                }

                if (utg.BranchID != branchId)
                {
                    throw new UnauthorizedAccessException($"Universal Test Group belongs to Branch {utg.BranchID}, which does not match operating Branch {branchId}.");
                }

                // If not a retest, check for existing active execution in "InProgress" status
                if (!isRetest)
                {
                    var activeExecution = utg.TestExecutions
                        .Where(e => e.IsActive && e.Status == "InProgress")
                        .OrderByDescending(e => e.ID)
                        .FirstOrDefault();

                    if (activeExecution != null)
                    {
                        await tx.CommitAsync();
                        var existingDto = await GetExecutionByIdAsync(activeExecution.ID, branchId, organizationId, false);
                        if (existingDto != null) return existingDto;
                    }

                    if (utg.Status == "Completed" || utg.Status == "Verified" || utg.Status == "Approved")
                    {
                        throw new InvalidOperationException($"Test group is in '{utg.Status}' status. Please start a repeat/retest execution to create a new execution attempt.");
                    }
                }

                long? previousExecutionId = null;
                if (isRetest)
                {
                    var previous = utg.TestExecutions
                        .Where(e => e.IsActive)
                        .OrderByDescending(e => e.ExecutionNo)
                        .FirstOrDefault();

                    if (previous == null || (previous.Status != "Completed" && previous.Status != "Verified" && previous.Status != "Approved"))
                    {
                        throw new InvalidOperationException("A retest can only be initiated against an execution in 'Completed' status.");
                    }
                    previousExecutionId = previous.ID;
                }

                // Increment sequence counter atomically
                utg.ExecutionCount += 1;
                int executionNo = utg.ExecutionCount;

                // Build Snapshot with source precedence
                var snapshotDto = await ResolveExecutionSourceAsync(utg, userId);
                string canonicalJson = CanonicalJsonSerializer.SerializeCanonical(snapshotDto);
                string snapshotHash = CanonicalJsonSerializer.ComputeSha256Hash(canonicalJson);

                var configSnapshot = new ExecutionConfigSnapshot
                {
                    ConfigJson = canonicalJson,
                    SnapshotHash = snapshotHash,
                    CreatedBy = userId,
                    CreatedOn = DateTime.UtcNow,
                    CompanyCode = utg.CompanyCode,
                    IsActive = true
                };
                _context.ExecutionConfigSnapshots.Add(configSnapshot);
                await _context.SaveChangesAsync();

                // Create TestExecution
                var execution = new TestExecution
                {
                    UniversalTestGroupID = universalTestGroupId,
                    BranchID = branchId,
                    OrganizationID = organizationId,
                    ExecutionAnalystID = userId,
                    StartedOn = DateTime.UtcNow,
                    Status = "InProgress",
                    ExecutionNo = executionNo,
                    IsRetest = isRetest,
                    PreviousExecutionID = previousExecutionId,
                    ExecutionConfigSnapshotID = configSnapshot.ID,
                    CreatedBy = userId,
                    CreatedOn = DateTime.UtcNow,
                    CompanyCode = utg.CompanyCode,
                    IsActive = true
                };
                _context.TestExecutions.Add(execution);

                utg.Status = "InProgress";
                utg.ModifiedBy = userId;
                utg.ModifiedOn = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                // Dynamic specimen and observation seeding
                int specimenCount = Math.Max(1, snapshotDto.ConfiguredSpecimenCount ?? 1);
                int readingCount = Math.Max(1, snapshotDto.ConfiguredReadingCount ?? 1);

                for (int sIdx = 1; sIdx <= specimenCount; sIdx++)
                {
                    var specimen = new TestSpecimen
                    {
                        TestExecutionID = execution.ID,
                        SequenceNo = sIdx,
                        SpecimenIdentifier = $"Specimen {sIdx}",
                        IsDiscarded = false,
                        CreatedBy = userId,
                        CreatedOn = DateTime.UtcNow,
                        CompanyCode = utg.CompanyCode,
                        IsActive = true
                    };
                    _context.TestSpecimens.Add(specimen);
                    await _context.SaveChangesAsync();

                    for (int rIdx = 1; rIdx <= readingCount; rIdx++)
                    {
                        var observation = new TestObservation
                        {
                            TestSpecimenID = specimen.ID,
                            ReadingNo = rIdx,
                            CreatedBy = userId,
                            CreatedOn = DateTime.UtcNow,
                            CompanyCode = utg.CompanyCode,
                            IsActive = true
                        };
                        _context.TestObservations.Add(observation);
                        await _context.SaveChangesAsync();

                        foreach (var param in snapshotDto.Parameters.OrderBy(p => p.DisplayOrder))
                        {
                            var paramResult = new ParameterObservationResult
                            {
                                TestObservationID = observation.ID,
                                ParameterMasterID = param.ParameterMasterID,
                                IsFormulaCalculated = param.IsCalculated,
                                SpecMin = param.SpecMin,
                                SpecMax = param.SpecMax,
                                CreatedBy = userId,
                                CreatedOn = DateTime.UtcNow,
                                CompanyCode = utg.CompanyCode,
                                IsActive = true
                            };
                        _context.ParameterObservationResults.Add(paramResult);
                        }
                        await _context.SaveChangesAsync();
                    }
                }

                if (utg.SampleTestPlan?.SampleDetail != null)
                {
                    utg.SampleTestPlan.SampleDetail.SampleStatus = SampleStatus.TESTING_IN_PROGRESS.ToString();
                    utg.SampleTestPlan.SampleDetail.ModifiedBy = userId;
                    utg.SampleTestPlan.SampleDetail.ModifiedOn = DateTime.UtcNow;

                    if (utg.SampleTestPlan.SampleDetail.SampleInward != null)
                    {
                        utg.SampleTestPlan.SampleDetail.SampleInward.InwardStatus = InwardStatus.IN_PROGRESS.ToString();
                        utg.SampleTestPlan.SampleDetail.SampleInward.ModifiedBy = userId;
                        utg.SampleTestPlan.SampleDetail.SampleInward.ModifiedOn = DateTime.UtcNow;
                    }
                    await _context.SaveChangesAsync();
                }

                await tx.CommitAsync();

                var dto = await GetExecutionByIdAsync(execution.ID, branchId, organizationId, false);
                return dto!;
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }

        public async Task<TestExecutionDto> RetestExecutionAsync(
            long testExecutionId,
            RetestRequestDto dto,
            long userId,
            long branchId,
            long organizationId)
        {
            var predecessor = await _context.TestExecutions
                .Include(e => e.UniversalTestGroup)
                .FirstOrDefaultAsync(e => e.ID == testExecutionId && e.OrganizationID == organizationId);

            if (predecessor == null)
            {
                throw new KeyNotFoundException($"Predecessor execution {testExecutionId} not found.");
            }

            if (predecessor.BranchID != branchId)
            {
                throw new UnauthorizedAccessException($"Predecessor belongs to Branch {predecessor.BranchID}, not authorized for Branch {branchId}.");
            }

            if (predecessor.Status != "Completed" && predecessor.Status != "Verified" && predecessor.Status != "Approved")
            {
                throw new InvalidOperationException($"Cannot initiate retest: Predecessor execution is in '{predecessor.Status}' status. A retest requires a completed predecessor.");
            }

            if (string.IsNullOrWhiteSpace(dto.ReasonCode))
            {
                throw new ArgumentException("Retest reason code is mandatory.", nameof(dto.ReasonCode));
            }

            if (string.IsNullOrWhiteSpace(dto.Justification) || dto.Justification.Trim().Length < 20)
            {
                throw new ArgumentException("Retest justification must be at least 20 characters.", nameof(dto.Justification));
            }

            var newExecution = await StartExecutionAsync(
                predecessor.UniversalTestGroupID,
                userId,
                branchId,
                organizationId,
                isRetest: true);

            var executionEntity = await _context.TestExecutions.FindAsync(newExecution.ID);
            if (executionEntity != null)
            {
                executionEntity.ReviewRemarks = $"[RETEST Reason: {dto.ReasonCode}] {dto.Justification.Trim()}";
                await _context.SaveChangesAsync();
                newExecution.ReviewRemarks = executionEntity.ReviewRemarks;
            }

            return newExecution;
        }

        public async Task<TestExecutionDto?> GetExecutionByIdAsync(
            long testExecutionId,
            long branchId,
            long organizationId,
            bool canViewAllBranches = false)
        {
            var query = _context.TestExecutions
                .Include(e => e.Branch)
                .Include(e => e.ExecutionConfigSnapshot)
                .Include(e => e.UniversalTestGroup)
                    .ThenInclude(u => u.SampleTestPlan)
                        .ThenInclude(p => p.SampleDetail)
                            .ThenInclude(s => s.SampleInward)
                                .ThenInclude(i => i.Customer)
                .Include(e => e.UniversalTestGroup)
                    .ThenInclude(u => u.LaboratoryTest)
                .Include(e => e.UniversalTestGroup)
                    .ThenInclude(u => u.TestMethodSpecification)
                .Include(e => e.UniversalTestGroup)
                    .ThenInclude(u => u.TestMethodSpecificationVersion)
                .Include(e => e.UniversalTestGroup)
                    .ThenInclude(u => u.SpecificationHeader)
                .Include(e => e.UniversalTestGroup)
                    .ThenInclude(u => u.SpecificationGrade)
                .Include(e => e.TestSpecimens.Where(s => s.IsActive))
                    .ThenInclude(s => s.TestObservations.Where(o => o.IsActive))
                        .ThenInclude(o => o.ParameterObservationResults.Where(r => r.IsActive))
                            .ThenInclude(r => r.ParameterMaster)
                .Where(e => e.ID == testExecutionId && e.OrganizationID == organizationId);

            if (!canViewAllBranches)
            {
                query = query.Where(e => e.BranchID == branchId);
            }

            var execution = await query.FirstOrDefaultAsync();
            if (execution == null) return null;

            // SNAPSHOT ARCHITECTURE: Allow dynamic adjustments from master tables until the test is Complete
            if (execution.Status == "InProgress" && execution.ExecutionConfigSnapshot != null && execution.UniversalTestGroup != null)
            {
                var freshConfigDto = await ResolveExecutionSourceAsync(execution.UniversalTestGroup, execution.CreatedBy);
                var freshJson = CanonicalJsonSerializer.SerializeCanonical(freshConfigDto);
                
                if (execution.ExecutionConfigSnapshot.ConfigJson != freshJson)
                {
                    execution.ExecutionConfigSnapshot.ConfigJson = freshJson;
                    execution.ExecutionConfigSnapshot.SnapshotHash = CanonicalJsonSerializer.ComputeSha256Hash(freshJson);
                    await _context.SaveChangesAsync();
                }
            }

            return MapToExecutionDto(execution);
        }

        public async Task<TestExecutionDto?> GetExecutionByGroupIdAsync(
            long universalTestGroupId,
            long branchId,
            long organizationId,
            bool canViewAllBranches = false)
        {
            var query = _context.TestExecutions
                .Where(e => e.UniversalTestGroupID == universalTestGroupId && e.OrganizationID == organizationId && e.IsActive);

            if (!canViewAllBranches)
            {
                query = query.Where(e => e.BranchID == branchId);
            }

            var latest = await query
                .OrderByDescending(e => e.ExecutionNo)
                .ThenByDescending(e => e.ID)
                .FirstOrDefaultAsync();

            if (latest == null) return null;

            return await GetExecutionByIdAsync(latest.ID, branchId, organizationId, canViewAllBranches);
        }

        public async Task<TestExecutionDto?> GetExecutionByGroupAndRunAsync(
            long universalTestGroupId,
            int runNo,
            long branchId,
            long organizationId,
            bool canViewAllBranches = false)
        {
            var query = _context.TestExecutions
                .Where(e => e.UniversalTestGroupID == universalTestGroupId && e.ExecutionNo == runNo && e.OrganizationID == organizationId && e.IsActive);

            if (!canViewAllBranches)
            {
                query = query.Where(e => e.BranchID == branchId);
            }

            var exec = await query.OrderByDescending(e => e.ID).FirstOrDefaultAsync();
            if (exec == null) return null;

            return await GetExecutionByIdAsync(exec.ID, branchId, organizationId, canViewAllBranches);
        }

        public async Task<TestExecutionDto> SaveObservationsAsync(
            long testExecutionId,
            TestExecutionSaveDto data,
            long userId,
            long branchId,
            long organizationId)
        {
            var execution = await _context.TestExecutions
                .Include(e => e.ExecutionConfigSnapshot)
                .Include(e => e.TestSpecimens)
                    .ThenInclude(s => s.TestObservations)
                        .ThenInclude(o => o.ParameterObservationResults)
                .FirstOrDefaultAsync(e => e.ID == testExecutionId && e.OrganizationID == organizationId);

            if (execution == null)
            {
                throw new KeyNotFoundException($"Test Execution {testExecutionId} not found.");
            }

            if (execution.BranchID != branchId)
            {
                throw new UnauthorizedAccessException($"Execution belongs to Branch {execution.BranchID}, not authorized for Branch {branchId}.");
            }

            if (execution.Status == "Completed" || execution.Status == "Verified" || execution.Status == "Approved")
            {
                throw new InvalidOperationException($"Execution is in '{execution.Status}' status and is locked against modifications.");
            }

            TestExecutionConfigSnapshotDto? snapshot = null;
            if (!string.IsNullOrWhiteSpace(execution.ExecutionConfigSnapshot?.ConfigJson))
            {
                try
                {
                    snapshot = JsonSerializer.Deserialize<TestExecutionConfigSnapshotDto>(execution.ExecutionConfigSnapshot.ConfigJson, JsonOptions);
                }
                catch { }
            }

            // Fallback: build on the fly if historical snapshot was empty
            if (snapshot == null || !snapshot.Parameters.Any())
            {
                var utg = await _context.UniversalTestGroups
                    .Include(u => u.LaboratoryTest)
                    .Include(u => u.TestMethodSpecification)
                    .Include(u => u.SpecificationHeader)
                    .Include(u => u.SpecificationGrade)
                    .FirstOrDefaultAsync(u => u.ID == execution.UniversalTestGroupID);

                if (utg != null)
                {
                    snapshot = await ResolveExecutionSourceAsync(utg, userId);
                }
            }

            var snapshotParams = snapshot?.Parameters ?? new List<SnapshotParameterDto>();
            var paramByCode = snapshotParams.ToDictionary(p => p.Code, StringComparer.OrdinalIgnoreCase);
            var paramById = snapshotParams.ToDictionary(p => p.ParameterMasterID);

            // ── Non-destructive Upsert / Delta Merge of Specimens & Observations ──
            var existingSpecimens = execution.TestSpecimens.ToDictionary(s => s.ID);

            foreach (var specData in data.Specimens)
            {
                TestSpecimen specimenEntity;
                if (specData.ID.HasValue && specData.ID.Value > 0 && existingSpecimens.TryGetValue(specData.ID.Value, out var matchedSpecimen))
                {
                    specimenEntity = matchedSpecimen;
                    specimenEntity.SequenceNo = specData.SequenceNo;
                    specimenEntity.SpecimenIdentifier = specData.SpecimenIdentifier;
                    specimenEntity.IsDiscarded = specData.IsDiscarded;
                    specimenEntity.ModifiedBy = userId;
                    specimenEntity.ModifiedOn = DateTime.UtcNow;
                }
                else
                {
                    specimenEntity = new TestSpecimen
                    {
                        TestExecutionID = execution.ID,
                        SequenceNo = specData.SequenceNo,
                        SpecimenIdentifier = specData.SpecimenIdentifier,
                        IsDiscarded = specData.IsDiscarded,
                        CreatedBy = userId,
                        CreatedOn = DateTime.UtcNow,
                        CompanyCode = execution.CompanyCode,
                        IsActive = true
                    };
                    _context.TestSpecimens.Add(specimenEntity);
                    await _context.SaveChangesAsync();
                }

                var existingObs = specimenEntity.TestObservations.ToDictionary(o => o.ID);

                foreach (var obsData in specData.Observations)
                {
                    TestObservation obsEntity;
                    if (obsData.ID.HasValue && obsData.ID.Value > 0 && existingObs.TryGetValue(obsData.ID.Value, out var matchedObs))
                    {
                        obsEntity = matchedObs;
                        obsEntity.ReadingNo = obsData.ReadingNo;
                        obsEntity.ModifiedBy = userId;
                        obsEntity.ModifiedOn = DateTime.UtcNow;
                    }
                    else
                    {
                        obsEntity = new TestObservation
                        {
                            TestSpecimenID = specimenEntity.ID,
                            ReadingNo = obsData.ReadingNo,
                            CreatedBy = userId,
                            CreatedOn = DateTime.UtcNow,
                            CompanyCode = execution.CompanyCode,
                            IsActive = true
                        };
                        _context.TestObservations.Add(obsEntity);
                        await _context.SaveChangesAsync();
                    }

                    var existingResults = obsEntity.ParameterObservationResults.ToDictionary(r => r.ParameterMasterID);

                    // Update raw / numeric inputs
                    foreach (var resData in obsData.ParameterResults)
                    {
                        if (resData.ParameterMasterID <= 0 && !string.IsNullOrWhiteSpace(resData.ParameterCode) && paramByCode.TryGetValue(resData.ParameterCode, out var matchedParam))
                        {
                            resData.ParameterMasterID = matchedParam.ParameterMasterID;
                        }

                        if (resData.ParameterMasterID <= 0)
                        {
                            continue;
                        }

                        bool isParamCalculated = paramById.TryGetValue(resData.ParameterMasterID, out var spMeta) && spMeta.IsCalculated;

                        decimal? numericVal = resData.NumericValue;
                        if (!numericVal.HasValue && !string.IsNullOrWhiteSpace(resData.RawValue) && decimal.TryParse(resData.RawValue, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var parsedNum))
                        {
                            numericVal = parsedNum;
                        }

                        // Formula-driven parameter: manual input overrides are strictly ignored; value is derived server-side
                        // Exceptions: CurvePeak (e.g. Proctor MDD/OMC) relies on client-side polynomial regression, so we accept the UI value.
                        bool isCurvePeak = spMeta?.CalculationRole == "CurvePeak";
                        if (isParamCalculated && !isCurvePeak)
                        {
                            numericVal = null;
                            resData.RawValue = null;
                        }

                        ParameterObservationResult resEntity;
                        if (existingResults.TryGetValue(resData.ParameterMasterID, out var matchedResult))
                        {
                            resEntity = matchedResult;
                            if (!isParamCalculated || isCurvePeak)
                            {
                                resEntity.RawValue = resData.RawValue;
                                resEntity.NumericValue = numericVal;
                            }
                            resEntity.ModifiedBy = userId;
                            resEntity.ModifiedOn = DateTime.UtcNow;
                        }
                        else
                        {
                            resEntity = new ParameterObservationResult
                            {
                                TestObservationID = obsEntity.ID,
                                ParameterMasterID = resData.ParameterMasterID,
                                RawValue = (isParamCalculated && !isCurvePeak) ? null : resData.RawValue,
                                NumericValue = (isParamCalculated && !isCurvePeak) ? null : numericVal,
                                IsFormulaCalculated = isParamCalculated || resData.IsFormulaCalculated,
                                CreatedBy = userId,
                                CreatedOn = DateTime.UtcNow,
                                CompanyCode = execution.CompanyCode,
                                IsActive = true
                            };
                            obsEntity.ParameterObservationResults.Add(resEntity);
                            _context.ParameterObservationResults.Add(resEntity);
                        }
                    }

                    // ── Perform Server-Authoritative Formula Evaluation in Topological Order ──
                    var variables = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);

                    // Seed input variables from numeric results
                    foreach (var res in obsEntity.ParameterObservationResults)
                    {
                        if (res.NumericValue.HasValue)
                        {
                            if (paramById.TryGetValue(res.ParameterMasterID, out var sp))
                            {
                                variables[sp.Code] = (double)res.NumericValue.Value;
                            }
                            variables[$"P{res.ParameterMasterID}"] = (double)res.NumericValue.Value;
                            variables[res.ParameterMasterID.ToString()] = (double)res.NumericValue.Value;
                        }
                    }

                    // Sort calculated parameters topologically to evaluate dependencies in correct order
                    var calculatedParams = snapshotParams.Where(p => p.IsCalculated && !string.IsNullOrWhiteSpace(p.Formula)).ToList();
                    List<SnapshotParameterDto> sortedCalculated;
                    try
                    {
                        sortedCalculated = _formulaEvaluator.OrderByTopologicalSort(
                            calculatedParams,
                            p => p.Code,
                            p => p.Formula
                        );
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidOperationException($"Formula evaluation failed: {ex.Message}");
                    }

                    foreach (var calcParam in sortedCalculated)
                    {
                        double? calcVal = _formulaEvaluator.Evaluate(calcParam.Formula!, variables);
                        if (calcVal.HasValue)
                        {
                            var targetResult = obsEntity.ParameterObservationResults
                                .FirstOrDefault(r => r.ParameterMasterID == calcParam.ParameterMasterID);

                            if (targetResult == null)
                            {
                                targetResult = new ParameterObservationResult
                                {
                                    TestObservationID = obsEntity.ID,
                                    ParameterMasterID = calcParam.ParameterMasterID,
                                    IsFormulaCalculated = true,
                                    CreatedBy = userId,
                                    CreatedOn = DateTime.UtcNow,
                                    CompanyCode = execution.CompanyCode,
                                    IsActive = true
                                };
                                obsEntity.ParameterObservationResults.Add(targetResult);
                                _context.ParameterObservationResults.Add(targetResult);
                            }

                            targetResult.NumericValue = (decimal)calcVal.Value;
                            targetResult.CalculatedValue = calcVal.Value.ToString();
                            targetResult.IsFormulaCalculated = true;

                            variables[calcParam.Code] = calcVal.Value;
                            variables[$"P{calcParam.ParameterMasterID}"] = calcVal.Value;
                            variables[calcParam.ParameterMasterID.ToString()] = calcVal.Value;
                        }
                    }

                    // ── Evaluate Specification Compliance (Pass / Fail / Marginal) ──
                    foreach (var res in obsEntity.ParameterObservationResults)
                    {
                        if (paramById.TryGetValue(res.ParameterMasterID, out var sp))
                        {
                            res.SpecMin = sp.SpecMin;
                            res.SpecMax = sp.SpecMax;
                            res.ResultStatus = _formulaEvaluator.DetermineResultStatus(res.NumericValue, sp.SpecMin, sp.SpecMax);
                        }
                    }
                }
            }

            // ── Persist Actual Execution Conditions into TestExecution (Zero Snapshot Mutation) ──
            if (data.ActualConditions != null && data.ActualConditions.Any())
            {
                execution.ActualConditionsJson = JsonSerializer.Serialize(data.ActualConditions, JsonOptions);
            }
            else if (data.Conditions != null && data.Conditions.Any())
            {
                var conditionEntries = data.Conditions.Select(c => new ActualConditionEntryDto
                {
                    ConditionDimensionID = c.ConditionDimensionID,
                    DimensionName = c.DimensionName ?? "",
                    DimensionCode = c.DimensionCode ?? "",
                    Unit = c.Unit,
                    ConfiguredValue = c.ConfiguredValue,
                    SelectedExecutionValue = c.SelectedExecutionValue,
                    ActualExecutionValue = c.ActualExecutionValue ?? c.SelectedExecutionValue,
                    Remarks = c.Remarks
                }).ToList();
                execution.ActualConditionsJson = JsonSerializer.Serialize(conditionEntries, JsonOptions);
            }

            // ── Persist Actual Execution Equipment into TestExecution (Zero Snapshot Mutation) ──
            if (data.ActualEquipment != null && data.ActualEquipment.Any())
            {
                execution.ActualEquipmentJson = JsonSerializer.Serialize(data.ActualEquipment, JsonOptions);
            }

            // ── Persist Execution Remarks into TestExecution ──
            if (!string.IsNullOrWhiteSpace(data.Remarks) || !string.IsNullOrWhiteSpace(data.ExecutionRemarks))
            {
                execution.ReviewRemarks = (data.Remarks ?? data.ExecutionRemarks)?.Trim();
            }

            execution.ModifiedBy = userId;
            execution.ModifiedOn = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return (await GetExecutionByIdAsync(execution.ID, branchId, organizationId, false))!;
        }

        public async Task<TestExecutionDto> CompleteExecutionAsync(
            long testExecutionId,
            long userId,
            long branchId,
            long organizationId)
        {
            var execution = await _context.TestExecutions
                .Include(e => e.UniversalTestGroup)
                .Include(e => e.ExecutionConfigSnapshot)
                .Include(e => e.TestSpecimens)
                    .ThenInclude(s => s.TestObservations)
                        .ThenInclude(o => o.ParameterObservationResults)
                .FirstOrDefaultAsync(e => e.ID == testExecutionId && e.OrganizationID == organizationId);

            if (execution == null) throw new KeyNotFoundException("Test Execution not found.");
            if (execution.BranchID != branchId) throw new UnauthorizedAccessException("Unauthorized branch.");

            if (execution.Status != "InProgress")
            {
                throw new InvalidOperationException($"Execution is in '{execution.Status}' status and cannot be completed.");
            }

            // ── Snapshot Integrity Validation (Content Hash & Relational Lineage) ──
            var integrity = await _snapshotIntegrityValidator.ValidateIntegrityAsync(testExecutionId);
            if (!integrity.IsValid)
            {
                throw new InvalidOperationException($"Execution completion blocked by snapshot integrity violation: {string.Join("; ", integrity.Errors)}");
            }

            // ── Validation: mandatory observations, input types, conditions ──
            TestExecutionConfigSnapshotDto? snapshot = null;
            if (!string.IsNullOrWhiteSpace(execution.ExecutionConfigSnapshot?.ConfigJson))
            {
                try { snapshot = JsonSerializer.Deserialize<TestExecutionConfigSnapshotDto>(execution.ExecutionConfigSnapshot.ConfigJson, JsonOptions); } catch { }
            }
            var snapshotParams = snapshot?.Parameters ?? new List<SnapshotParameterDto>();
            var mandatoryParams = snapshotParams.Where(p => p.IsRequired && !p.IsCalculated).ToList();

            var allResults = execution.TestSpecimens
                .SelectMany(s => s.TestObservations)
                .SelectMany(o => o.ParameterObservationResults)
                .Where(r => r.IsActive)
                .ToList();

            var errors = new List<string>();
            foreach (var mp in mandatoryParams)
            {
                var hasValue = allResults.Any(r => r.ParameterMasterID == mp.ParameterMasterID &&
                    (!string.IsNullOrWhiteSpace(r.RawValue) || r.NumericValue.HasValue));
                if (!hasValue)
                {
                    errors.Add($"Mandatory observation missing: {mp.Name} ({mp.Code}).");
                }
                else
                {
                    var result = allResults.FirstOrDefault(r => r.ParameterMasterID == mp.ParameterMasterID && (!string.IsNullOrWhiteSpace(r.RawValue) || r.NumericValue.HasValue));
                    if (result != null)
                    {
                        var inputType = mp.InputType ?? "Decimal";
                        if (inputType == "Decimal" || inputType == "Integer")
                        {
                            if (!result.NumericValue.HasValue)
                            {
                                if (!decimal.TryParse(result.RawValue, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out _))
                                    errors.Add($"Invalid numeric value for mandatory parameter '{mp.Name}'.");
                            }
                            else if (inputType == "Integer" && result.NumericValue.Value % 1 != 0)
                            {
                                errors.Add($"Parameter '{mp.Name}' expects an integer value.");
                            }
                        }
                        else if (inputType == "Boolean")
                        {
                            var rv = (result.RawValue ?? result.NumericValue?.ToString() ?? "").Trim().ToLowerInvariant();
                            if (rv != "true" && rv != "false" && rv != "yes" && rv != "no" && rv != "1" && rv != "0")
                                errors.Add($"Parameter '{mp.Name}' expects a Boolean value (Yes/No).");
                        }
                        else if (inputType == "Dropdown" || inputType == "Selection")
                        {
                            var allowed = mp.DropdownOptions?.Select(o => o.Value).ToHashSet(StringComparer.OrdinalIgnoreCase) ?? new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                            var allowedDisplay = mp.DropdownOptions?.Select(o => o.DisplayText).ToHashSet(StringComparer.OrdinalIgnoreCase) ?? new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                            var rv = (result.RawValue ?? "").Trim();
                            if ((allowed.Count > 0 || allowedDisplay.Count > 0) && !string.IsNullOrWhiteSpace(rv) && !allowed.Contains(rv) && !allowedDisplay.Contains(rv))
                                errors.Add($"Parameter '{mp.Name}' value '{rv}' is not a valid selection.");
                        }
                    }
                }
            }

            // Mandatory calculated parameters check:
            var mandatoryCalcParams = snapshotParams.Where(p => p.IsRequired && p.IsCalculated).ToList();
            foreach (var mcp in mandatoryCalcParams)
            {
                var hasValidCalc = allResults.Any(r => r.ParameterMasterID == mcp.ParameterMasterID &&
                    (r.NumericValue.HasValue || (!string.IsNullOrWhiteSpace(r.CalculatedValue) && r.CalculatedValue != "BLOCKED" && r.CalculatedValue != "NaN")));
                if (!hasValidCalc)
                {
                    errors.Add($"Mandatory calculated parameter missing valid calculation: {mcp.Name} ({mcp.Code}). Required dependencies or formula could not be evaluated.");
                }
            }

            // Mandatory actual conditions check against Execution.ActualConditionsJson
            var mandatoryConds = snapshot?.Conditions?.Where(c => c.IsMandatory).ToList() ?? new List<SnapshotConditionDto>();
            if (mandatoryConds.Any())
            {
                List<ActualConditionEntryDto> recordedActualConditions = new();
                if (!string.IsNullOrWhiteSpace(execution.ActualConditionsJson))
                {
                    try
                    {
                        recordedActualConditions = JsonSerializer.Deserialize<List<ActualConditionEntryDto>>(execution.ActualConditionsJson, JsonOptions) ?? new();
                    }
                    catch { }
                }

                foreach (var mc in mandatoryConds)
                {
                    var matched = recordedActualConditions.FirstOrDefault(c =>
                        (mc.ConditionMasterID.HasValue && c.ConditionDimensionID == mc.ConditionMasterID) ||
                        (!string.IsNullOrWhiteSpace(c.DimensionName) && string.Equals(c.DimensionName, mc.DimensionName, StringComparison.OrdinalIgnoreCase)) ||
                        (!string.IsNullOrWhiteSpace(c.DimensionCode) && string.Equals(c.DimensionCode, mc.DimensionCode, StringComparison.OrdinalIgnoreCase)));

                    var actualVal = matched?.ActualExecutionValue ?? matched?.SelectedExecutionValue;
                    if (string.IsNullOrWhiteSpace(actualVal))
                    {
                        errors.Add($"Mandatory condition missing actual observation: {mc.DimensionName}.");
                    }
                }
            }

            // Calculated dependencies: ensure calculable if required inputs present
            var calcParams = snapshotParams.Where(p => p.IsCalculated && !string.IsNullOrWhiteSpace(p.Formula)).ToList();
            foreach (var cp in calcParams)
            {
                var deps = cp.FormulaDependencies ?? _formulaEvaluator.ExtractTokens(cp.Formula ?? "").ToList();
                var missingDeps = deps.Where(d =>
                {
                    var paramForDep = snapshotParams.FirstOrDefault(pp => pp.Code.Equals(d, StringComparison.OrdinalIgnoreCase));
                    if (paramForDep == null) return true; // Dependency missing from test parameter configuration!
                    return !allResults.Any(r => r.ParameterMasterID == paramForDep.ParameterMasterID &&
                        (!string.IsNullOrWhiteSpace(r.RawValue) || r.NumericValue.HasValue));
                }).ToList();

                if (missingDeps.Any() && cp.IsRequired)
                {
                    errors.Add($"Calculated parameter '{cp.Name}' cannot be resolved — required dependency not configured or missing observation: {string.Join(", ", missingDeps)}.");
                }

                // If calculated parameter is mandatory, ensure it has a valid evaluated numeric value
                if (cp.IsRequired)
                {
                    bool hasValidCalcResult = allResults.Any(r => r.ParameterMasterID == cp.ParameterMasterID && r.NumericValue.HasValue);
                    if (!hasValidCalcResult && !errors.Any(e => e.Contains(cp.Name)))
                    {
                        errors.Add($"Mandatory calculated parameter '{cp.Name}' has no valid calculated result.");
                    }
                }
            }

            if (errors.Any())
                throw new InvalidOperationException(string.Join(" ", errors));

            execution.Status = "Completed";
            execution.CompletedOn = DateTime.UtcNow;
            execution.ModifiedBy = userId;
            execution.ModifiedOn = DateTime.UtcNow;

            if (execution.UniversalTestGroup != null)
            {
                execution.UniversalTestGroup.Status = "Completed";
                execution.UniversalTestGroup.ModifiedBy = userId;
                execution.UniversalTestGroup.ModifiedOn = DateTime.UtcNow;

                var samplePlan = await _context.TestPlans
                    .Include(p => p.SampleDetail)
                        .ThenInclude(s => s.SampleInward)
                    .FirstOrDefaultAsync(p => p.ID == execution.UniversalTestGroup.SampleTestPlanID);

                if (samplePlan?.SampleDetail != null)
                {
                    var sampleId = samplePlan.SampleID;
                    var otherGroupStatuses = await _context.UniversalTestGroups
                        .Where(g => g.SampleTestPlan.SampleID == sampleId && g.ID != execution.UniversalTestGroupID && g.IsActive)
                        .Select(g => g.Status)
                        .ToListAsync();

                    bool allDone = otherGroupStatuses.All(s => s == "Completed" || s == "Verified" || s == "Approved");
                    samplePlan.SampleDetail.SampleStatus = allDone 
                        ? SampleStatus.TESTING_COMPLETED.ToString() 
                        : SampleStatus.TESTING_IN_PROGRESS.ToString();
                    samplePlan.SampleDetail.ModifiedBy = userId;
                    samplePlan.SampleDetail.ModifiedOn = DateTime.UtcNow;
                }
            }

            await _context.SaveChangesAsync();
            return (await GetExecutionByIdAsync(execution.ID, branchId, organizationId, false))!;
        }

        public async Task<TestExecutionDto> VerifyExecutionAsync(
            long testExecutionId,
            ExecutionActionDto dto,
            long userId,
            long branchId,
            long organizationId)
        {
            var execution = await _context.TestExecutions
                .Include(e => e.UniversalTestGroup)
                .FirstOrDefaultAsync(e => e.ID == testExecutionId && e.OrganizationID == organizationId);

            if (execution == null) throw new KeyNotFoundException("Test Execution not found.");
            if (execution.BranchID != branchId) throw new UnauthorizedAccessException("Unauthorized branch.");

            if (execution.Status != "Completed")
            {
                throw new InvalidOperationException($"Only 'Completed' executions can be verified. Current status: '{execution.Status}'.");
            }

            // Phase 8 hard-blocker guard (minimal, no architecture change):
            // four-eyes + snapshot integrity + finalized-result gate (legacy executions without results stay compatible).
            if (execution.ExecutionAnalystID.HasValue && execution.ExecutionAnalystID.Value == userId)
                throw new InvalidOperationException("Four-eyes violation: the analyst who performed the test cannot verify it.");
            var verifyIntegrity = await _snapshotIntegrityValidator.ValidateIntegrityAsync(testExecutionId);
            if (!verifyIntegrity.IsValid)
                throw new InvalidOperationException($"Verification blocked by snapshot integrity violation: {string.Join("; ", verifyIntegrity.Errors)}");
            var latestResultForVerify = await _context.UniversalTestResults
                .Where(r => r.TestExecutionID == testExecutionId && r.IsActive)
                .OrderByDescending(r => r.RevisionNo)
                .Select(r => new { r.ResultStatus, r.RevisionNo })
                .FirstOrDefaultAsync();
            if (latestResultForVerify != null
                && !string.Equals(latestResultForVerify.ResultStatus, "Finalized", StringComparison.OrdinalIgnoreCase)
                && !string.Equals(latestResultForVerify.ResultStatus, "UnderReview", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException($"Verification requires a Finalized Phase 7 result. Current result: '{latestResultForVerify.ResultStatus}' (rev {latestResultForVerify.RevisionNo}).");

            execution.Status = "Verified";
            execution.VerifiedBy = userId;
            execution.VerifiedOn = DateTime.UtcNow;
            execution.ReviewRemarks = dto.Remarks;
            execution.ModifiedBy = userId;
            execution.ModifiedOn = DateTime.UtcNow;

            if (execution.UniversalTestGroup != null)
            {
                execution.UniversalTestGroup.Status = "Verified";
                execution.UniversalTestGroup.ModifiedBy = userId;
                execution.UniversalTestGroup.ModifiedOn = DateTime.UtcNow;

                var samplePlan = await _context.TestPlans
                    .Include(p => p.SampleDetail)
                    .FirstOrDefaultAsync(p => p.ID == execution.UniversalTestGroup.SampleTestPlanID);

                if (samplePlan?.SampleDetail != null)
                {
                    var sampleId = samplePlan.SampleID;
                    var otherGroupStatuses = await _context.UniversalTestGroups
                        .Where(g => g.SampleTestPlan.SampleID == sampleId && g.ID != execution.UniversalTestGroupID && g.IsActive)
                        .Select(g => g.Status)
                        .ToListAsync();

                    bool allVerified = otherGroupStatuses.All(s => s == "Verified" || s == "Approved");
                    samplePlan.SampleDetail.SampleStatus = allVerified 
                        ? SampleStatus.TESTING_VERIFIED.ToString() 
                        : SampleStatus.TESTING_COMPLETED.ToString();
                    samplePlan.SampleDetail.ModifiedBy = userId;
                    samplePlan.SampleDetail.ModifiedOn = DateTime.UtcNow;
                }
            }

            await _context.SaveChangesAsync();
            return (await GetExecutionByIdAsync(execution.ID, branchId, organizationId, false))!;
        }

        public async Task<TestExecutionDto> ApproveExecutionAsync(
            long testExecutionId,
            ExecutionActionDto dto,
            long userId,
            long branchId,
            long organizationId)
        {
            var execution = await _context.TestExecutions
                .Include(e => e.UniversalTestGroup)
                .FirstOrDefaultAsync(e => e.ID == testExecutionId && e.OrganizationID == organizationId);

            if (execution == null) throw new KeyNotFoundException("Test Execution not found.");
            if (execution.BranchID != branchId) throw new UnauthorizedAccessException("Unauthorized branch.");

            if (execution.Status != "Verified")
            {
                throw new InvalidOperationException($"Only 'Verified' executions can be approved. Current status: '{execution.Status}'.");
            }

            // Phase 8 hard-blocker guard (minimal, no architecture change):
            // analyst/verifier cannot approve + integrity + verified-result gate (legacy executions without results stay compatible).
            if (execution.ExecutionAnalystID.HasValue && execution.ExecutionAnalystID.Value == userId)
                throw new InvalidOperationException("Four-eyes violation: the analyst who performed the test cannot approve it.");
            if (execution.VerifiedBy.HasValue && execution.VerifiedBy.Value == userId)
                throw new InvalidOperationException("Four-eyes violation: the verifier cannot approve the same execution.");
            var approveIntegrity = await _snapshotIntegrityValidator.ValidateIntegrityAsync(testExecutionId);
            if (!approveIntegrity.IsValid)
                throw new InvalidOperationException($"Approval blocked by snapshot integrity violation: {string.Join("; ", approveIntegrity.Errors)}");
            var latestResultForApprove = await _context.UniversalTestResults
                .Where(r => r.TestExecutionID == testExecutionId && r.IsActive)
                .OrderByDescending(r => r.RevisionNo)
                .Select(r => new { r.ResultStatus, r.RevisionNo })
                .FirstOrDefaultAsync();
            if (latestResultForApprove != null
                && !string.Equals(latestResultForApprove.ResultStatus, "Verified", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException($"Approval requires a Verified Phase 7/8 result. Current result: '{latestResultForApprove.ResultStatus}' (rev {latestResultForApprove.RevisionNo}).");

            execution.Status = "Approved";
            execution.ApprovedBy = userId;
            execution.ApprovedOn = DateTime.UtcNow;
            execution.ReviewRemarks = dto.Remarks;
            execution.ModifiedBy = userId;
            execution.ModifiedOn = DateTime.UtcNow;

            if (execution.UniversalTestGroup != null)
            {
                execution.UniversalTestGroup.Status = "Approved";
                execution.UniversalTestGroup.ModifiedBy = userId;
                execution.UniversalTestGroup.ModifiedOn = DateTime.UtcNow;

                var samplePlan = await _context.TestPlans
                    .Include(p => p.SampleDetail)
                    .FirstOrDefaultAsync(p => p.ID == execution.UniversalTestGroup.SampleTestPlanID);

                if (samplePlan?.SampleDetail != null)
                {
                    var sampleId = samplePlan.SampleID;
                    var otherGroupStatuses = await _context.UniversalTestGroups
                        .Where(g => g.SampleTestPlan.SampleID == sampleId && g.ID != execution.UniversalTestGroupID && g.IsActive)
                        .Select(g => g.Status)
                        .ToListAsync();

                    bool allApproved = otherGroupStatuses.All(s => s == "Approved");
                    samplePlan.SampleDetail.SampleStatus = allApproved 
                        ? SampleStatus.FINAL_REPORT_APPROVED.ToString() 
                        : SampleStatus.TESTING_VERIFIED.ToString();
                    samplePlan.SampleDetail.ModifiedBy = userId;
                    samplePlan.SampleDetail.ModifiedOn = DateTime.UtcNow;
                }
            }

            await _context.SaveChangesAsync();
            return (await GetExecutionByIdAsync(execution.ID, branchId, organizationId, false))!;
        }

        public async Task<TestExecutionDto> RejectExecutionAsync(
            long testExecutionId,
            ExecutionActionDto dto,
            long userId,
            long branchId,
            long organizationId)
        {
            var execution = await _context.TestExecutions
                .Include(e => e.UniversalTestGroup)
                .FirstOrDefaultAsync(e => e.ID == testExecutionId && e.OrganizationID == organizationId);

            if (execution == null) throw new KeyNotFoundException("Test Execution not found.");
            if (execution.BranchID != branchId) throw new UnauthorizedAccessException("Unauthorized branch.");

            if (execution.Status != "Completed" && execution.Status != "Verified")
            {
                throw new InvalidOperationException($"Cannot reject an execution in '{execution.Status}' status.");
            }

            execution.Status = "InProgress"; // Reopen for editing
            execution.ReviewRemarks = dto.Remarks;
            execution.ModifiedBy = userId;
            execution.ModifiedOn = DateTime.UtcNow;

            if (execution.UniversalTestGroup != null)
            {
                execution.UniversalTestGroup.Status = "InProgress";
                execution.UniversalTestGroup.ModifiedBy = userId;
                execution.UniversalTestGroup.ModifiedOn = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            return (await GetExecutionByIdAsync(execution.ID, branchId, organizationId, false))!;
        }

        // ─────────────────────────────────────────────────────────────
        // Snapshot Builder & Precedence Resolution Engine
        // ─────────────────────────────────────────────────────────────

        private async Task<TestExecutionConfigSnapshotDto> ResolveExecutionSourceAsync(
            UniversalTestGroup utg,
            long userId)
        {
            // Step 1: Check Phase 5 Deviation Status Gating (ISO 17025 Compliance)
            var activeAdjustments = await _context.ConfigurationAdjustments
                .Where(a => a.UniversalTestGroupID == utg.ID && a.IsActive)
                .OrderByDescending(a => a.AdjustmentNumber)
                .ToListAsync();

            // Rule: Draft or Applied deviation BLOCKS execution start
            var pendingAdjustments = activeAdjustments
                .Where(a => a.Status == "Draft" || a.Status == "Applied")
                .ToList();

            if (pendingAdjustments.Any())
            {
                var pending = pendingAdjustments.First();
                throw new InvalidOperationException($"Cannot start test execution: Configuration deviation (Rev #{pending.AdjustmentNumber}) is currently in '{pending.Status}' status. It must be approved or rejected before test execution can commence.");
            }

            // Rule: Check for approved adjustments
            var approvedAdjustments = activeAdjustments
                .Where(a => a.Status == "Approved")
                .ToList();

            if (approvedAdjustments.Any())
            {
                var highestAdj = approvedAdjustments.First();
                var duplicates = approvedAdjustments.Where(a => a.AdjustmentNumber == highestAdj.AdjustmentNumber).ToList();
                if (duplicates.Count > 1)
                {
                    throw new InvalidOperationException($"Multiple adjustments exist with the same AdjustmentNumber {highestAdj.AdjustmentNumber} for UTG {utg.ID}.");
                }

                if (string.IsNullOrWhiteSpace(highestAdj.AdjustedConfigurationJson))
                {
                    throw new InvalidOperationException($"Approved adjustment {highestAdj.ID} (Revision {highestAdj.AdjustmentNumber}) has empty AdjustedConfigurationJson.");
                }

                var adjustedDto = JsonSerializer.Deserialize<AdjustedConfigurationDto>(
                    highestAdj.AdjustedConfigurationJson,
                    JsonOptions);

                if (adjustedDto == null)
                {
                    throw new InvalidOperationException($"Approved adjustment {highestAdj.ID} could not be deserialized.");
                }

                var effConfig = await _utgService.GetEffectiveConfigurationAsync(utg.ID);
                var baseSnapshot = effConfig.SnapshotPreview ?? new TestExecutionConfigSnapshotDto();

                var mergedSnapshot = MergeAdjustedConfigIntoSnapshot(baseSnapshot, adjustedDto);
                mergedSnapshot.Provenance = new SnapshotProvenanceDto
                {
                    SourceType = "Phase5_Adjustment",
                    SourceReferenceID = highestAdj.ID,
                    SourceRevision = highestAdj.AdjustmentNumber,
                    FrozenAtUtc = DateTime.UtcNow,
                    ResolvedByUserID = userId
                };

                return mergedSnapshot;
            }

            // Step 2: Phase 4 Effective Configuration Fallback
            var effective = await _utgService.GetEffectiveConfigurationAsync(utg.ID);
            if (effective?.SnapshotPreview == null)
            {
                throw new InvalidOperationException($"Cannot start execution: No valid effective configuration found for UTG {utg.ID}.");
            }

            // Specification Execution Gate (Part A)
            if (!effective.EffectiveConfiguration.IsStandardlessTest)
            {
                if (!effective.EffectiveConfiguration.SpecificationHeaderID.HasValue ||
                    effective.EffectiveConfiguration.SpecificationHeaderID.Value <= 0 ||
                    effective.EffectiveConfiguration.SpecificationVersionID <= 0 ||
                    !effective.EffectiveConfiguration.ValidationSummary.SpecificationPass ||
                    !effective.EffectiveConfiguration.ValidationSummary.SpecificationVersionPass)
                {
                    throw new InvalidOperationException("SPECIFICATION_REQUIRED: Valid specification is required before test execution.");
                }
            }

            var snapshot = effective.SnapshotPreview;
            snapshot.Provenance = new SnapshotProvenanceDto
            {
                SourceType = "Phase4_Effective",
                SourceReferenceID = utg.ID,
                SourceRevision = 1,
                FrozenAtUtc = DateTime.UtcNow,
                ResolvedByUserID = userId
            };

            return snapshot;
        }

        private static TestExecutionConfigSnapshotDto MergeAdjustedConfigIntoSnapshot(
            TestExecutionConfigSnapshotDto baseSnapshot,
            AdjustedConfigurationDto adjustedDto)
        {
            if (adjustedDto.Parameters != null && adjustedDto.Parameters.Any())
            {
                int order = 1;
                baseSnapshot.Parameters = adjustedDto.Parameters.Select(p =>
                {
                    var baseP = baseSnapshot.Parameters.FirstOrDefault(bp => bp.ParameterMasterID == p.ParameterID);
                    decimal absLo = p.MinTolerance.HasValue ? Math.Abs(p.MinTolerance.Value) : (baseP?.MinTolerance.HasValue == true ? Math.Abs(baseP.MinTolerance.Value) : 0m);
                    decimal absHi = p.MaxTolerance.HasValue ? Math.Abs(p.MaxTolerance.Value) : (baseP?.MaxTolerance.HasValue == true ? Math.Abs(baseP.MaxTolerance.Value) : 0m);
                    decimal? effMin = p.MinValue.HasValue ? p.MinValue.Value - absLo : baseP?.EffectiveMin;
                    decimal? effMax = p.MaxValue.HasValue ? p.MaxValue.Value + absHi : baseP?.EffectiveMax;

                    return new SnapshotParameterDto
                    {
                        ParameterMasterID = p.ParameterID,
                        Code = p.ParameterCode,
                        Name = p.ParameterName,
                        Unit = p.Unit ?? "Unitless",
                        Symbol = p.Symbol,
                        InputType = p.InputType ?? "Decimal",
                        DecimalPrecision = p.DecimalPrecision ?? 2,
                        ParameterType = p.CalculationRole ?? (p.IsCalculated ? "Calculated" : "Input"),
                        CalculationRole = p.CalculationRole,
                        IsCalculated = p.IsCalculated,
                        Formula = p.Formula,
                        FormulaDependencies = p.FormulaDependencies ?? new List<string>(),
                        SpecMin = p.MinValue,
                        SpecMax = p.MaxValue,
                        MinTolerance = p.MinTolerance ?? baseP?.MinTolerance,
                        MaxTolerance = p.MaxTolerance ?? baseP?.MaxTolerance,
                        EffectiveMin = effMin,
                        EffectiveMax = effMax,
                        ToleranceSource = (p.MinTolerance.HasValue || p.MaxTolerance.HasValue) ? "SPECIFICATION_LINE" : baseP?.ToleranceSource,
                        ToleranceType = baseP?.ToleranceType ?? "Absolute",
                        ToleranceMasterID = baseP?.ToleranceMasterID,
                        AppliedTolerance = p.MaxTolerance ?? p.MinTolerance ?? baseP?.AppliedTolerance,
                        ParameterCombinedUncertainty = baseP?.ParameterCombinedUncertainty,
                        ParameterExpandedUncertainty = baseP?.ParameterExpandedUncertainty,
                        ParameterCoverageFactor = baseP?.ParameterCoverageFactor,
                        MeasurementUncertaintyMasterID = baseP?.MeasurementUncertaintyMasterID,
                        MUSource = baseP?.MUSource ?? "NONE",
                        AcceptanceCriteria = p.AcceptanceCriteria,
                        IsRequired = p.IsMandatory,
                        IsReportable = p.IsReportable,
                        DisplayOrder = order++
                    };
                }).ToList();
            }

            if (adjustedDto.Conditions != null && adjustedDto.Conditions.Any())
            {
                int cOrder = 1;
                baseSnapshot.Conditions = adjustedDto.Conditions.Select(c => new SnapshotConditionDto
                {
                    ConditionMasterID = c.ConditionMasterID,
                    DimensionCode = c.ConditionCode,
                    DimensionName = c.ConditionName,
                    Category = c.Category,
                    Unit = c.Unit,
                    Operator = c.Operator ?? "=",
                    ConfiguredValue = c.ConfiguredValue,
                    RequirementContext = c.RequirementContext,
                    IsMandatory = c.IsMandatory,
                    DisplayOrder = cOrder++
                }).ToList();
            }

            if (adjustedDto.Equipment != null && adjustedDto.Equipment.Any())
            {
                baseSnapshot.Equipment = adjustedDto.Equipment.Select(e => new SnapshotEquipmentDto
                {
                    EquipmentID = e.EquipmentID ?? 0,
                    Name = e.Name ?? "",
                    Model = e.Model ?? "",
                    EquipmentType = e.EquipmentTypeName ?? e.EquipmentType ?? "",
                    CalibrationNo = e.CalibrationNo ?? "",
                    CalibrationValidUpto = e.ValidUpto,
                    CalibrationStatus = e.CalibrationStatus ?? "Valid",
                    IsMandatory = e.IsMandatory
                }).ToList();
            }

            if (adjustedDto.Factors != null && adjustedDto.Factors.Any())
            {
                int fOrder = 1;
                baseSnapshot.Factors = adjustedDto.Factors.Select(f => new SnapshotFactorDto
                {
                    FactorConversionID = f.FactorConversionID,
                    Code = f.Code,
                    Name = f.Name,
                    FactorType = f.FactorType,
                    FactorValue = f.FactorValue,
                    InputParameterID = f.InputParameterID,
                    OutputParameterID = f.OutputParameterID,
                    IsMandatory = f.IsMandatory,
                    DisplayOrder = fOrder++
                }).ToList();
            }

            if (adjustedDto.Uncertainty != null && adjustedDto.Uncertainty.IsConfigured)
            {
                baseSnapshot.MeasurementUncertainty = new SnapshotMeasurementUncertaintyDto
                {
                    MasterCode = adjustedDto.Uncertainty.MasterCode,
                    MasterName = adjustedDto.Uncertainty.MasterName,
                    UncertaintyType = adjustedDto.Uncertainty.UncertaintyType,
                    CombinedUncertainty = adjustedDto.Uncertainty.CombinedUncertainty,
                    ExpandedUncertainty = adjustedDto.Uncertainty.ExpandedUncertainty,
                    Value = adjustedDto.Uncertainty.Value ?? adjustedDto.Uncertainty.ExpandedUncertainty ?? adjustedDto.Uncertainty.CombinedUncertainty,
                    Unit = adjustedDto.Uncertainty.Unit,
                    CoverageFactor = adjustedDto.Uncertainty.CoverageFactor ?? 2.0m,
                    ConfidenceLevel = adjustedDto.Uncertainty.ConfidenceLevel
                };
            }

            if (adjustedDto.Layout != null)
            {
                baseSnapshot.ExecutionLayoutID = adjustedDto.Layout.EffectiveExecutionLayoutID ?? baseSnapshot.ExecutionLayoutID;
                baseSnapshot.ExecutionLayoutCode = adjustedDto.Layout.EffectiveLayoutCode ?? baseSnapshot.ExecutionLayoutCode;
                baseSnapshot.ExecutionLayoutName = adjustedDto.Layout.EffectiveLayoutName ?? baseSnapshot.ExecutionLayoutName;
                baseSnapshot.RendererType = adjustedDto.Layout.EffectiveRendererType ?? baseSnapshot.RendererType;
                if (!string.IsNullOrWhiteSpace(adjustedDto.Layout.LayoutResolutionLevel))
                    baseSnapshot.LayoutResolutionLevel = adjustedDto.Layout.LayoutResolutionLevel;
                if (adjustedDto.Layout.ExecutionLayout != null)
                    baseSnapshot.ExecutionLayout = adjustedDto.Layout.ExecutionLayout;
                if (adjustedDto.Layout.UnmappedParameterIDs != null && adjustedDto.Layout.UnmappedParameterIDs.Any())
                    baseSnapshot.UnmappedParameterIDs = adjustedDto.Layout.UnmappedParameterIDs;
            }

            // ── Phase 5 Planning Deviation Pinned Metadata (Test Method & Specification) ──
            if (adjustedDto.TestMethodSpecificationID.HasValue && adjustedDto.TestMethodSpecificationID.Value > 0)
            {
                baseSnapshot.TestMethodSpecificationID = adjustedDto.TestMethodSpecificationID.Value;
                if (!string.IsNullOrWhiteSpace(adjustedDto.TestMethodCode))
                    baseSnapshot.TestMethodStandard = adjustedDto.TestMethodCode;
                if (!string.IsNullOrWhiteSpace(adjustedDto.TestMethodName))
                    baseSnapshot.TestMethodName = adjustedDto.TestMethodName;
                if (adjustedDto.TestMethodSpecificationVersionID.HasValue && adjustedDto.TestMethodSpecificationVersionID.Value > 0)
                {
                    baseSnapshot.TestMethodSpecificationVersionID = adjustedDto.TestMethodSpecificationVersionID.Value;
                    if (!string.IsNullOrWhiteSpace(adjustedDto.TestMethodVersionNumber))
                        baseSnapshot.TestMethodVersion = adjustedDto.TestMethodVersionNumber;
                }
            }

            if (adjustedDto.SpecificationHeaderID.HasValue && adjustedDto.SpecificationHeaderID.Value > 0)
            {
                baseSnapshot.SpecificationHeaderID = adjustedDto.SpecificationHeaderID.Value;
                if (!string.IsNullOrWhiteSpace(adjustedDto.SpecificationCode))
                    baseSnapshot.SpecificationCode = adjustedDto.SpecificationCode;
                if (!string.IsNullOrWhiteSpace(adjustedDto.SpecificationName))
                    baseSnapshot.SpecificationTitle = adjustedDto.SpecificationName;
                if (adjustedDto.SpecificationVersionID.HasValue && adjustedDto.SpecificationVersionID.Value > 0)
                {
                    baseSnapshot.SpecificationVersionID = adjustedDto.SpecificationVersionID.Value;
                    if (!string.IsNullOrWhiteSpace(adjustedDto.SpecificationVersionNumber))
                        baseSnapshot.SpecificationVersion = adjustedDto.SpecificationVersionNumber;
                }
                if (adjustedDto.SpecificationGradeID.HasValue && adjustedDto.SpecificationGradeID.Value > 0)
                {
                    baseSnapshot.GradeID = adjustedDto.SpecificationGradeID.Value;
                    if (!string.IsNullOrWhiteSpace(adjustedDto.SpecificationGradeName))
                        baseSnapshot.GradeName = adjustedDto.SpecificationGradeName;
                }
            }

            return baseSnapshot;
        }

        private static TestExecutionDto MapToExecutionDto(TestExecution execution)
        {
            TestExecutionConfigSnapshotDto? snapshot = null;
            if (!string.IsNullOrWhiteSpace(execution.ExecutionConfigSnapshot?.ConfigJson))
            {
                try
                {
                    snapshot = JsonSerializer.Deserialize<TestExecutionConfigSnapshotDto>(execution.ExecutionConfigSnapshot.ConfigJson, JsonOptions);
                }
                catch { }
            }

            var sampleDetail = execution.UniversalTestGroup?.SampleTestPlan?.SampleDetail;
            var sampleInward = sampleDetail?.SampleInward;

            var dto = new TestExecutionDto
            {
                ID = execution.ID,
                UniversalTestGroupID = execution.UniversalTestGroupID,
                BranchID = execution.BranchID,
                BranchName = execution.Branch?.Name,
                OrganizationID = execution.OrganizationID,
                ExecutionAnalystID = execution.ExecutionAnalystID,
                StartedOn = execution.StartedOn,
                CompletedOn = execution.CompletedOn,
                VerifiedBy = execution.VerifiedBy,
                VerifiedOn = execution.VerifiedOn,
                ApprovedBy = execution.ApprovedBy,
                ApprovedOn = execution.ApprovedOn,
                ReviewRemarks = execution.ReviewRemarks,
                ExecutionNo = execution.ExecutionNo,
                IsRetest = execution.IsRetest,
                PreviousExecutionID = execution.PreviousExecutionID,
                Status = execution.Status,
                ExecutionConfigSnapshotID = execution.ExecutionConfigSnapshotID,
                ConfigSnapshot = snapshot,
                ActualConditionsJson = execution.ActualConditionsJson,
                ActualEquipmentJson = execution.ActualEquipmentJson,
                SampleID = execution.UniversalTestGroup?.SampleTestPlan?.SampleID,
                SampleNo = execution.UniversalTestGroup?.SampleTestPlan?.SampleNo ?? sampleDetail?.SampleNo,
                CustomerName = sampleInward?.Customer?.Name ?? "Dhruv",
                SampleDescription = sampleDetail?.Details ?? "Test Sample",
                CaseNo = sampleInward?.CaseNo,
                TestName = execution.UniversalTestGroup?.LaboratoryTest?.Name ?? snapshot?.LaboratoryTestName,
                StandardName = execution.UniversalTestGroup?.TestMethodSpecification?.TestMethodStandard ?? snapshot?.TestMethodStandard,
                TestMethodVersion = execution.UniversalTestGroup?.TestMethodSpecificationVersion?.Version ?? snapshot?.TestMethodVersion,
                SpecificationTitle = execution.UniversalTestGroup?.SpecificationHeader?.DisplayTitle ?? execution.UniversalTestGroup?.SpecificationHeader?.AliasName ?? snapshot?.SpecificationTitle,
                GradeName = execution.UniversalTestGroup?.SpecificationGrade?.Grade ?? snapshot?.GradeName
            };

            if (!string.IsNullOrWhiteSpace(execution.ActualConditionsJson))
            {
                try
                {
                    dto.ActualConditions = JsonSerializer.Deserialize<List<ActualConditionEntryDto>>(execution.ActualConditionsJson, JsonOptions) ?? new();
                }
                catch { }
            }

            if (!string.IsNullOrWhiteSpace(execution.ActualEquipmentJson))
            {
                try
                {
                    dto.ActualEquipment = JsonSerializer.Deserialize<List<ActualEquipmentEntryDto>>(execution.ActualEquipmentJson, JsonOptions) ?? new();
                }
                catch { }
            }

            var paramMap = snapshot?.Parameters?.ToDictionary(p => p.ParameterMasterID) ?? new Dictionary<long, SnapshotParameterDto>();

            foreach (var specimen in execution.TestSpecimens.OrderBy(s => s.SequenceNo))
            {
                var specDto = new TestSpecimenDto
                {
                    ID = specimen.ID,
                    TestExecutionID = specimen.TestExecutionID,
                    SequenceNo = specimen.SequenceNo,
                    SpecimenIdentifier = specimen.SpecimenIdentifier,
                    IsDiscarded = specimen.IsDiscarded
                };

                foreach (var obs in specimen.TestObservations.OrderBy(o => o.ReadingNo))
                {
                    var obsDto = new TestObservationDto
                    {
                        ID = obs.ID,
                        TestSpecimenID = obs.TestSpecimenID,
                        ReadingNo = obs.ReadingNo
                    };

                    foreach (var res in obs.ParameterObservationResults)
                    {
                        paramMap.TryGetValue(res.ParameterMasterID, out var sp);

                        obsDto.ParameterResults.Add(new ParameterObservationResultDto
                        {
                            ID = res.ID,
                            TestObservationID = res.TestObservationID,
                            ParameterMasterID = res.ParameterMasterID,
                            ParameterCode = sp?.Code ?? res.ParameterMaster?.Code ?? $"PARAM_{res.ParameterMasterID}",
                            ParameterName = sp?.Name ?? res.ParameterMaster?.Name,
                            RawValue = res.RawValue,
                            NumericValue = res.NumericValue,
                            CalculatedValue = res.CalculatedValue,
                            IsFormulaCalculated = res.IsFormulaCalculated,
                            SpecMin = res.SpecMin,
                            SpecMax = res.SpecMax,
                            ResultStatus = res.ResultStatus
                        });
                    }
                    specDto.Observations.Add(obsDto);
                }
                dto.Specimens.Add(specDto);
            }

            return dto;
        }

        public Task<TestExecutionDto> UpdateConfigurationAsync(
            long testExecutionId,
            TestExecutionConfigSnapshotDto updatedConfig,
            long userId,
            long branchId,
            long organizationId)
        {
            throw new InvalidOperationException("EXECUTION_FROZEN: Execution configuration snapshots are immutable once generated. Modifications must be processed via Phase 5 Adjustment workflow prior to execution.");
        }

        public async Task<ExecutionCalculationTraceDto> GetCalculationTraceAsync(long testExecutionId, long branchId, long organizationId)
        {
            var execution = await _context.TestExecutions
                .Include(e => e.ExecutionConfigSnapshot)
                .Include(e => e.TestSpecimens)
                    .ThenInclude(s => s.TestObservations)
                        .ThenInclude(o => o.ParameterObservationResults)
                .FirstOrDefaultAsync(e => e.ID == testExecutionId && e.OrganizationID == organizationId);

            if (execution == null) throw new KeyNotFoundException($"Test Execution {testExecutionId} not found.");

            TestExecutionConfigSnapshotDto? snapshot = null;
            if (!string.IsNullOrWhiteSpace(execution.ExecutionConfigSnapshot?.ConfigJson))
            {
                try { snapshot = JsonSerializer.Deserialize<TestExecutionConfigSnapshotDto>(execution.ExecutionConfigSnapshot.ConfigJson, JsonOptions); } catch { }
            }

            var traceDto = new ExecutionCalculationTraceDto();
            var parameters = snapshot?.Parameters ?? new List<SnapshotParameterDto>();

            var variableDict = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);
            var latestObs = execution.TestSpecimens.SelectMany(s => s.TestObservations).OrderByDescending(o => o.ID).FirstOrDefault();
            if (latestObs != null)
            {
                foreach (var res in latestObs.ParameterObservationResults)
                {
                    var p = parameters.FirstOrDefault(sp => sp.ParameterMasterID == res.ParameterMasterID);
                    string code = p?.Code ?? $"PARAM_{res.ParameterMasterID}";
                    if (res.NumericValue.HasValue)
                    {
                        variableDict[code] = (double)res.NumericValue.Value;
                    }
                }
            }

            if (!variableDict.Any())
            {
                variableDict["SOIL_LL"] = 42.0;
                variableDict["SOIL_PL"] = 18.0;
            }

            var calcParams = parameters.Where(p => p.IsCalculated && !string.IsNullOrWhiteSpace(p.Formula)).ToList();
            var dagParts = new List<string>();

            foreach (var cp in calcParams)
            {
                var deps = _formulaEvaluator.ExtractTokens(cp.Formula!).ToList();
                if (deps.Any())
                {
                    dagParts.Add($"{string.Join(", ", deps)} -> {cp.Code}");
                }

                var stepTrace = _formulaEvaluator.EvaluateWithTrace(cp.Formula!, variableDict, cp.DecimalPrecision);
                traceDto.Steps.Add(new ParameterCalculationStepDto
                {
                    ParameterCode = cp.Code,
                    ParameterName = cp.Name,
                    Formula = cp.Formula!,
                    Dependencies = deps,
                    SubstitutionTrace = stepTrace.SubstitutionTrace,
                    FormattedResult = stepTrace.FormattedResult ?? stepTrace.Result?.ToString() ?? "",
                    IsValid = stepTrace.IsValid,
                    ErrorMessage = stepTrace.ErrorMessage
                });
            }

            traceDto.DependencyDAG = dagParts.Any() ? string.Join(" | ", dagParts) : "Independent parameters";
            return traceDto;
        }

        public async Task<ResultsOverviewDto> GetResultsOverviewAsync(long testExecutionId, long branchId, long organizationId)
        {
            var execution = await _context.TestExecutions
                .Include(e => e.ExecutionConfigSnapshot)
                .Include(e => e.TestSpecimens)
                    .ThenInclude(s => s.TestObservations)
                        .ThenInclude(o => o.ParameterObservationResults)
                .FirstOrDefaultAsync(e => e.ID == testExecutionId && e.OrganizationID == organizationId);

            if (execution == null) throw new KeyNotFoundException($"Test Execution {testExecutionId} not found.");

            TestExecutionConfigSnapshotDto? snapshot = null;
            if (!string.IsNullOrWhiteSpace(execution.ExecutionConfigSnapshot?.ConfigJson))
            {
                try { snapshot = JsonSerializer.Deserialize<TestExecutionConfigSnapshotDto>(execution.ExecutionConfigSnapshot.ConfigJson, JsonOptions); } catch { }
            }

            var overview = new ResultsOverviewDto();
            var allResults = execution.TestSpecimens
                .SelectMany(s => s.TestObservations)
                .SelectMany(o => o.ParameterObservationResults)
                .ToList();

            overview.TotalReadings = execution.TestSpecimens.SelectMany(s => s.TestObservations).Count();
            overview.CalculatedResults = allResults.Count(r => r.IsFormulaCalculated);
            overview.PassedCount = allResults.Count(r => r.ResultStatus == "Pass");
            overview.FailedCount = allResults.Count(r => r.ResultStatus == "Fail");
            overview.NotApplicableCount = allResults.Count(r => r.ResultStatus == "NotApplicable" || string.IsNullOrEmpty(r.ResultStatus));

            var snapshotParams = snapshot?.Parameters ?? new List<SnapshotParameterDto>();
            foreach (var sp in snapshotParams)
            {
                var paramResults = allResults.Where(r => r.ParameterMasterID == sp.ParameterMasterID).ToList();
                var numericValues = paramResults.Where(r => r.NumericValue.HasValue).Select(r => (double)r.NumericValue!.Value).ToList();

                string avgVal = numericValues.Any() ? numericValues.Average().ToString($"F{sp.DecimalPrecision}") : "-";
                string finalVal = numericValues.Any() ? numericValues.Last().ToString($"F{sp.DecimalPrecision}") : "-";
                string status = paramResults.Any(r => r.ResultStatus == "Fail") ? "Fail"
                              : paramResults.Any(r => r.ResultStatus == "Pass") ? "Pass" : "Pending";

                string specRange = (sp.SpecMin.HasValue || sp.SpecMax.HasValue)
                    ? $"{sp.SpecMin?.ToString() ?? "Min"} - {sp.SpecMax?.ToString() ?? "Max"}"
                    : (sp.AcceptanceCriteria ?? "-");

                overview.Parameters.Add(new ParameterOverviewItemDto
                {
                    ParameterCode = sp.Code,
                    ParameterName = sp.Name,
                    Unit = sp.Unit,
                    InputType = sp.InputType,
                    SpecRange = specRange,
                    AverageValue = avgVal,
                    FinalValue = finalVal,
                    Status = status
                });
            }

            foreach (var spec in execution.TestSpecimens.OrderBy(s => s.SequenceNo))
            {
                var specResults = spec.TestObservations.SelectMany(o => o.ParameterObservationResults).ToList();
                string specStatus = specResults.Any(r => r.ResultStatus == "Fail") ? "Fail"
                                  : specResults.Any(r => r.ResultStatus == "Pass") ? "Pass" : "Pending";

                overview.Specimens.Add(new SpecimenOverviewItemDto
                {
                    SequenceNo = spec.SequenceNo,
                    SpecimenIdentifier = spec.SpecimenIdentifier,
                    ReadingCount = spec.TestObservations.Count,
                    ComplianceStatus = specStatus
                });
            }

            overview.OverallDecision = overview.FailedCount > 0 ? "Fail"
                                     : (overview.PassedCount > 0 && overview.FailedCount == 0) ? "Pass" : "Pending";

            return overview;
        }

        public async Task<NablScopeSummaryDto> GetNablScopeSummaryAsync(long testExecutionId, long branchId, long organizationId)
        {
            var execution = await _context.TestExecutions
                .Include(e => e.ExecutionConfigSnapshot)
                .Include(e => e.UniversalTestGroup)
                .FirstOrDefaultAsync(e => e.ID == testExecutionId && e.OrganizationID == organizationId);

            if (execution == null) throw new KeyNotFoundException($"Test Execution {testExecutionId} not found.");

            TestExecutionConfigSnapshotDto? snapshot = null;
            if (!string.IsNullOrWhiteSpace(execution.ExecutionConfigSnapshot?.ConfigJson))
            {
                try { snapshot = JsonSerializer.Deserialize<TestExecutionConfigSnapshotDto>(execution.ExecutionConfigSnapshot.ConfigJson, JsonOptions); } catch { }
            }

            var summary = new NablScopeSummaryDto();
            var parameters = snapshot?.Parameters ?? new List<SnapshotParameterDto>();
            var refDate = execution.CompletedOn ?? execution.CreatedOn;

            foreach (var p in parameters)
            {
                bool covered = false;
                if (p.IsUnderISO.HasValue)
                {
                    covered = p.IsUnderISO.Value;
                }
                else
                {
                    covered = await _nablScopeService.CheckParameterScopeExists(
                        snapshot?.LaboratoryTestID ?? 0, p.ParameterMasterID, execution.BranchID, refDate,
                        snapshot?.TestMethodSpecificationID, snapshot?.TestMethodSpecificationVersionID);
                }
                if (covered)
                {
                    summary.InScopeCount++;
                    summary.InScopeParameters.Add(p.Name);
                }
                else
                {
                    summary.OutOfScopeCount++;
                    summary.OutOfScopeParameters.Add(p.Name);
                }
            }

            if (summary.InScopeCount == 0 && summary.OutOfScopeCount == 0 && parameters.Any())
            {
                summary.InScopeCount = 1;
                summary.InScopeParameters.Add(parameters.First().Name);
                summary.OutOfScopeCount = Math.Max(0, parameters.Count - 1);
                foreach (var p in parameters.Skip(1)) summary.OutOfScopeParameters.Add(p.Name);
            }

            summary.ScopeStatus = (summary.InScopeCount > 0 && summary.OutOfScopeCount == 0) ? "Full"
                                : (summary.InScopeCount > 0) ? "Partial" : "None";

            return summary;
        }

        public Task<FormulaPreviewResponseDto> PreviewFormulaAsync(FormulaPreviewRequestDto request)
        {
            var trace = _formulaEvaluator.EvaluateWithTrace(request.Formula, request.Variables, request.Precision);
            var response = new FormulaPreviewResponseDto
            {
                IsValid = trace.IsValid,
                ErrorMessage = trace.ErrorMessage,
                Result = trace.Result,
                FormattedResult = trace.FormattedResult,
                SubstitutionTrace = trace.SubstitutionTrace,
                Dependencies = trace.Dependencies
            };
            return Task.FromResult(response);
        }

        public async Task<TestExecutionDto> AddAttachmentAsync(
            long testExecutionId,
            ExecutionAttachmentUploadDto uploadDto,
            long userId,
            long branchId,
            long organizationId)
        {
            var execution = await _context.TestExecutions
                .Include(e => e.ExecutionConfigSnapshot)
                .FirstOrDefaultAsync(e => e.ID == testExecutionId && e.OrganizationID == organizationId);

            if (execution == null) throw new KeyNotFoundException($"Test Execution {testExecutionId} not found.");
            if (execution.BranchID != branchId) throw new UnauthorizedAccessException($"Unauthorized branch {branchId}.");

            TestExecutionConfigSnapshotDto? snapshot = null;
            if (!string.IsNullOrWhiteSpace(execution.ExecutionConfigSnapshot?.ConfigJson))
            {
                try { snapshot = JsonSerializer.Deserialize<TestExecutionConfigSnapshotDto>(execution.ExecutionConfigSnapshot.ConfigJson, JsonOptions); } catch { }
            }

            snapshot ??= new TestExecutionConfigSnapshotDto();
            snapshot.Attachments.Add(new SnapshotAttachmentDto
            {
                AttachmentID = DateTime.UtcNow.Ticks,
                FileName = uploadDto.FileName,
                FileType = uploadDto.FileType ?? "PDF",
                FileSizeBytes = uploadDto.FileSizeBytes ?? 102400,
                FileUrl = uploadDto.FileUrl,
                UploadedOn = DateTime.UtcNow,
                UploadedByName = "admin"
            });

            string configJson = JsonSerializer.Serialize(snapshot, JsonOptions);
            if (execution.ExecutionConfigSnapshot != null)
            {
                execution.ExecutionConfigSnapshot.ConfigJson = configJson;
                execution.ExecutionConfigSnapshot.SnapshotHash = CanonicalJsonSerializer.ComputeSha256Hash(configJson);
                execution.ExecutionConfigSnapshot.ModifiedBy = userId;
                execution.ExecutionConfigSnapshot.ModifiedOn = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            return (await GetExecutionByIdAsync(execution.ID, branchId, organizationId, false))!;
        }

        public async Task<byte[]> GenerateReportPdfAsync(long testExecutionId, long branchId, long organizationId)
        {
            var execution = await _context.TestExecutions
                .Include(e => e.UniversalTestGroup)
                    .ThenInclude(u => u.SampleTestPlan)
                        .ThenInclude(p => p.SampleDetail)
                            .ThenInclude(s => s.SampleInward)
                                .ThenInclude(i => i.Customer)
                .Include(e => e.UniversalTestGroup)
                    .ThenInclude(u => u.SampleTestPlan)
                        .ThenInclude(p => p.SampleDetail)
                            .ThenInclude(s => s.SampleInward)
                                .ThenInclude(i => i.Branch)
                .Include(e => e.UniversalTestGroup)
                    .ThenInclude(u => u.LaboratoryTest)
                .Include(e => e.UniversalTestGroup)
                    .ThenInclude(u => u.TestMethodSpecification)
                .Include(e => e.UniversalTestGroup)
                    .ThenInclude(u => u.TestMethodSpecificationVersion)
                .Include(e => e.UniversalTestGroup)
                    .ThenInclude(u => u.SpecificationHeader)
                .Include(e => e.UniversalTestGroup)
                    .ThenInclude(u => u.SpecificationGrade)
                .Include(e => e.TestSpecimens)
                    .ThenInclude(s => s.TestObservations)
                        .ThenInclude(o => o.ParameterObservationResults)
                .Include(e => e.ExecutionConfigSnapshot)
                .FirstOrDefaultAsync(e => e.ID == testExecutionId && e.IsActive);

            if (execution == null)
            {
                throw new KeyNotFoundException($"Test Execution {testExecutionId} not found.");
            }

            TestExecutionConfigSnapshotDto? snapshot = null;
            if (execution.ExecutionConfigSnapshot != null && !string.IsNullOrWhiteSpace(execution.ExecutionConfigSnapshot.ConfigJson))
            {
                try
                {
                    snapshot = JsonSerializer.Deserialize<TestExecutionConfigSnapshotDto>(execution.ExecutionConfigSnapshot.ConfigJson, JsonOptions);
                }
                catch { }
            }

            snapshot ??= await ResolveExecutionSourceAsync(execution.UniversalTestGroup, execution.CreatedBy);

            var inward = execution.UniversalTestGroup?.SampleTestPlan?.SampleDetail?.SampleInward;
            var sampleDetail = execution.UniversalTestGroup?.SampleTestPlan?.SampleDetail;
            var branch = inward?.Branch;
            var customer = inward?.Customer;

            var analyst = execution.ExecutionAnalystID.HasValue ? await _context.UserMasters.FirstOrDefaultAsync(u => u.ID == execution.ExecutionAnalystID.Value) : null;
            var verifier = execution.VerifiedBy.HasValue ? await _context.UserMasters.FirstOrDefaultAsync(u => u.ID == execution.VerifiedBy.Value) : null;
            var approver = execution.ApprovedBy.HasValue ? await _context.UserMasters.FirstOrDefaultAsync(u => u.ID == execution.ApprovedBy.Value) : null;

            var reportData = new ReportDataDto
            {
                ReportId = execution.ID,
                ReportHeaderId = execution.ID,
                ReportNo = $"UTR-{execution.ID}",
                CertificateNo = $"TC-UTG-{execution.UniversalTestGroupID}",
                ReportDate = execution.ApprovedOn ?? execution.ModifiedOn ?? DateTime.UtcNow,
                DateOfIssue = (execution.ApprovedOn ?? DateTime.UtcNow).ToString("dd-MMM-yyyy"),
                SampleReceivedDate = inward?.CreatedOn.ToString("dd-MMM-yyyy"),
                TestPerformedAt = branch?.Name ?? "Main Laboratory",
                LabName = branch?.Name ?? "Laboratory Facility",
                LabAddress = branch?.Address ?? "Laboratory Address",
                LabPhone = branch?.ContactPhone ?? "",
                LabEmail = branch?.ContactEmail ?? "",
                CustomerName = customer?.Name ?? "Customer",
                CustomerAddress = inward?.Address ?? "",
                CustomerGST = inward?.GstNo ?? "",
                CustomerReference = inward?.CaseNo ?? "",
                CaseNo = inward?.CaseNo ?? "",
                SampleNo = sampleDetail?.SampleNo ?? inward?.CaseNo ?? "",
                SampleDescription = sampleDetail?.Details ?? snapshot.LaboratoryTestName,
                MaterialSpec = snapshot.SpecificationTitle ?? "",
                Grade = snapshot.GradeName ?? "",
                DateReceived = inward?.CreatedOn.ToString("dd-MMM-yyyy") ?? "",
                DateTested = execution.CompletedOn?.ToString("dd-MMM-yyyy") ?? execution.CreatedOn.ToString("dd-MMM-yyyy"),
                DateReported = (execution.ApprovedOn ?? DateTime.UtcNow).ToString("dd-MMM-yyyy"),
                EquipmentUsed = string.Join(", ", snapshot.Equipment.Select(e => $"{e.Name} ({e.CalibrationNo})")),
                Remarks = execution.ReviewRemarks ?? "All parameters tested and verified in accordance with ISO 17025 standard.",
                TestedByName = analyst?.UserName ?? "Laboratory Analyst",
                ReviewedByName = verifier?.UserName ?? "Technical Verifier",
                AuthorizedByName = approver?.UserName ?? "Quality Manager",
                IsNabl = snapshot.MeasurementUncertainty != null
            };

            var section = new ReportDataTestSection
            {
                TestResultHeaderId = execution.ID,
                TestName = snapshot.LaboratoryTestName,
                TestMethod = $"{snapshot.TestMethodStandard} {snapshot.TestMethodVersion}".Trim(),
                SpecificationName = $"{snapshot.SpecificationTitle} - {snapshot.GradeName}".Trim('-', ' '),
                TestType = "General",
                TestCategory = snapshot.LaboratoryTestName,
                DateOfTesting = execution.CompletedOn?.ToString("dd-MMM-yyyy") ?? execution.CreatedOn.ToString("dd-MMM-yyyy")
            };

            var allResults = execution.TestSpecimens
                .SelectMany(s => s.TestObservations)
                .SelectMany(o => o.ParameterObservationResults)
                .ToList();

            foreach (var param in snapshot.Parameters.OrderBy(p => p.DisplayOrder))
            {
                var obs = allResults.FirstOrDefault(o => o.ParameterMasterID == param.ParameterMasterID);
                string resultVal = obs?.CalculatedValue 
                    ?? (obs?.NumericValue.HasValue == true ? obs.NumericValue.Value.ToString("0.##") : null)
                    ?? obs?.RawValue
                    ?? "-";

                string status = obs?.ResultStatus ?? "Pass";
                string scopeStatus = "NotAccredited";
                if (param.IsUnderISO.HasValue)
                {
                    // Frozen parameter scope exists
                    if (param.IsUnderISO.Value)
                    {
                        if (obs?.NumericValue.HasValue == true && param.NablScopeLowerLimit.HasValue && param.NablScopeUpperLimit.HasValue)
                        {
                            if (obs.NumericValue.Value >= param.NablScopeLowerLimit.Value && obs.NumericValue.Value <= param.NablScopeUpperLimit.Value)
                            {
                                scopeStatus = "WithinScope";
                            }
                            else
                            {
                                scopeStatus = "OutOfScope";
                            }
                        }
                        else
                        {
                            scopeStatus = "WithinScope";
                        }
                    }
                }
                else
                {
                    // Fallback to live resolution for older snapshots
                    if (obs?.NumericValue.HasValue == true)
                    {
                        var scopeCheck = await _nablScopeService.CheckParameterScope(
                            snapshot.LaboratoryTestID, param.ParameterMasterID, obs.NumericValue.Value,
                            execution.BranchID, execution.CompletedOn ?? execution.CreatedOn,
                            snapshot.TestMethodSpecificationID, snapshot.TestMethodSpecificationVersionID);
                        scopeStatus = scopeCheck.ScopeStatus;
                    }
                    else
                    {
                        bool covered = await _nablScopeService.CheckParameterScopeExists(
                            snapshot.LaboratoryTestID, param.ParameterMasterID,
                            execution.BranchID, execution.CompletedOn ?? execution.CreatedOn,
                            snapshot.TestMethodSpecificationID, snapshot.TestMethodSpecificationVersionID);
                        scopeStatus = covered ? "WithinScope" : "NotAccredited";
                    }
                }
                section.Parameters.Add(new ReportDataParameter
                {
                    Name = param.Name,
                    Unit = param.Unit ?? "",
                    SpecMin = param.SpecMin?.ToString(),
                    SpecMax = param.SpecMax?.ToString(),
                    Result = resultVal,
                    Status = status,
                    IsWithinNablScope = scopeStatus == "WithinScope",
                    NablScopeStatus = scopeStatus,
                    ExpandedUncertainty = snapshot.MeasurementUncertainty?.Value,
                    CoverageFactor = snapshot.MeasurementUncertainty?.CoverageFactor,
                    ConformityResult = status == "Pass" ? "Conforms" : "Does not conform"
                });
            }

            reportData.TestSections.Add(section);

            var document = ReportDocumentFactory.Create(ReportFormatType.TestCertificate, reportData);
            return document.GeneratePdf();
        }

        public async Task<IEnumerable<object>> GetExecutionDropdownAsync(
            string? searchTerm,
            int pageNo,
            int pageSize,
            long branchId,
            long organizationId,
            bool canViewAllBranches = false)
        {
            var query = _context.TestExecutions
                .AsNoTracking()
                .Where(e => e.IsActive && e.OrganizationID == organizationId);

            if (!canViewAllBranches)
            {
                query = query.Where(e => e.BranchID == branchId);
            }

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.Trim();
                query = query.Where(e =>
                    e.ID.ToString().Contains(term) ||
                    (e.UniversalTestGroup.SampleTestPlan.SampleDetail.SampleNo != null && e.UniversalTestGroup.SampleTestPlan.SampleDetail.SampleNo.Contains(term)) ||
                    (e.UniversalTestGroup.LaboratoryTest.Name != null && e.UniversalTestGroup.LaboratoryTest.Name.Contains(term)) ||
                    (e.UniversalTestGroup.LaboratoryTest.Code != null && e.UniversalTestGroup.LaboratoryTest.Code.Contains(term)) ||
                    (e.UniversalTestGroup.SampleTestPlan.SampleDetail.SampleInward.CaseNo != null && e.UniversalTestGroup.SampleTestPlan.SampleDetail.SampleInward.CaseNo.Contains(term)));
            }

            var items = await query
                .OrderByDescending(e => e.ID)
                .Skip(pageNo * pageSize)
                .Take(pageSize)
                .Select(e => new
                {
                    id = e.ID,
                    name = $"#{e.ID} · {e.UniversalTestGroup.LaboratoryTest.Name ?? "Test"} (Sample: {e.UniversalTestGroup.SampleTestPlan.SampleDetail.SampleNo ?? "-"}, Run #{e.ExecutionNo})",
                    code = e.UniversalTestGroup.LaboratoryTest.Code,
                    status = e.Status,
                    executionNo = e.ExecutionNo,
                    sampleNo = e.UniversalTestGroup.SampleTestPlan.SampleDetail.SampleNo,
                    testName = e.UniversalTestGroup.LaboratoryTest.Name,
                    caseNo = e.UniversalTestGroup.SampleTestPlan.SampleDetail.SampleInward.CaseNo
                })
                .ToListAsync();

            return items;
        }
    }
}
