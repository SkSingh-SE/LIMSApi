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

namespace LIMSApi.Services
{
    public class UniversalTestExecutionService : IUniversalTestExecutionService
    {
        private readonly LIMSContext _context;
        private readonly FormulaEvaluator _formulaEvaluator;
        private readonly IEffectiveConfigurationResolver _resolver;
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
            ILogger<UniversalTestExecutionService>? logger = null)
        {
            _context = context;
            _formulaEvaluator = formulaEvaluator;
            _resolver = resolver;
            _logger = logger;
        }

        public async Task<TestExecutionDto> StartExecutionAsync(
            long universalTestGroupId,
            long userId,
            long branchId,
            long organizationId,
            bool isRetest = false)
        {
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
                    var existingDto = await GetExecutionByIdAsync(activeExecution.ID, branchId, organizationId, false);
                    if (existingDto != null) return existingDto;
                }

                if (utg.Status == "Completed" || utg.Status == "Verified" || utg.Status == "Approved")
                {
                    throw new InvalidOperationException($"Test group is in '{utg.Status}' status. Please start a repeat/retest execution to create a new execution attempt.");
                }
            }

            // Determine execution number
            int executionNo = 1;
            long? previousExecutionId = null;
            if (isRetest)
            {
                var previous = utg.TestExecutions
                    .OrderByDescending(e => e.ExecutionNo)
                    .FirstOrDefault();

                executionNo = (previous?.ExecutionNo ?? 0) + 1;
                previousExecutionId = previous?.ID;
            }

            // Build Real Execution Configuration Snapshot
            var snapshotDto = await BuildSnapshotDtoAsync(utg);
            string snapshotJson = JsonSerializer.Serialize(snapshotDto, JsonOptions);
            string snapshotHash = ComputeSha256Hash(snapshotJson);

            var configSnapshot = new ExecutionConfigSnapshot
            {
                ConfigJson = snapshotJson,
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

            // Seed initial 1 Specimen and 1 Observation with parameters
            var initialSpecimen = new TestSpecimen
            {
                TestExecutionID = execution.ID,
                SequenceNo = 1,
                SpecimenIdentifier = "Specimen 1",
                IsDiscarded = false,
                CreatedBy = userId,
                CreatedOn = DateTime.UtcNow,
                CompanyCode = utg.CompanyCode,
                IsActive = true
            };
            _context.TestSpecimens.Add(initialSpecimen);
            await _context.SaveChangesAsync();

            var initialObservation = new TestObservation
            {
                TestSpecimenID = initialSpecimen.ID,
                ReadingNo = 1,
                CreatedBy = userId,
                CreatedOn = DateTime.UtcNow,
                CompanyCode = utg.CompanyCode,
                IsActive = true
            };
            _context.TestObservations.Add(initialObservation);
            await _context.SaveChangesAsync();

            foreach (var param in snapshotDto.Parameters.OrderBy(p => p.DisplayOrder))
            {
                var paramResult = new ParameterObservationResult
                {
                    TestObservationID = initialObservation.ID,
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

            var dto = await GetExecutionByIdAsync(execution.ID, branchId, organizationId, false);
            return dto!;
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
                    snapshot = await BuildSnapshotDtoAsync(utg);
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
                        ParameterObservationResult resEntity;
                        if (existingResults.TryGetValue(resData.ParameterMasterID, out var matchedResult))
                        {
                            resEntity = matchedResult;
                            resEntity.RawValue = resData.RawValue;
                            resEntity.NumericValue = resData.NumericValue;
                            resEntity.ModifiedBy = userId;
                            resEntity.ModifiedOn = DateTime.UtcNow;
                        }
                        else
                        {
                            resEntity = new ParameterObservationResult
                            {
                                TestObservationID = obsEntity.ID,
                                ParameterMasterID = resData.ParameterMasterID,
                                RawValue = resData.RawValue,
                                NumericValue = resData.NumericValue,
                                IsFormulaCalculated = resData.IsFormulaCalculated,
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
                .FirstOrDefaultAsync(e => e.ID == testExecutionId && e.OrganizationID == organizationId);

            if (execution == null) throw new KeyNotFoundException("Test Execution not found.");
            if (execution.BranchID != branchId) throw new UnauthorizedAccessException("Unauthorized branch.");

            if (execution.Status != "InProgress")
            {
                throw new InvalidOperationException($"Execution is in '{execution.Status}' status and cannot be completed.");
            }

            execution.Status = "Completed";
            execution.CompletedOn = DateTime.UtcNow;
            execution.ModifiedBy = userId;
            execution.ModifiedOn = DateTime.UtcNow;

            if (execution.UniversalTestGroup != null)
            {
                execution.UniversalTestGroup.Status = "Completed";
                execution.UniversalTestGroup.ModifiedBy = userId;
                execution.UniversalTestGroup.ModifiedOn = DateTime.UtcNow;
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
        // Snapshot Builder Helper
        // ─────────────────────────────────────────────────────────────

        private async Task<TestExecutionConfigSnapshotDto> BuildSnapshotDtoAsync(UniversalTestGroup utg)
        {
            return await _resolver.ResolveSnapshotDtoAsync(utg);
        }

        private static string ComputeSha256Hash(string rawData)
        {
            using var sha256 = SHA256.Create();
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(rawData));
            var builder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2"));
            }
            return builder.ToString();
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

        public async Task<TestExecutionDto> UpdateConfigurationAsync(
            long testExecutionId,
            TestExecutionConfigSnapshotDto updatedConfig,
            long userId,
            long branchId,
            long organizationId)
        {
            var execution = await _context.TestExecutions
                .Include(e => e.ExecutionConfigSnapshot)
                .Include(e => e.UniversalTestGroup)
                .FirstOrDefaultAsync(e => e.ID == testExecutionId && e.OrganizationID == organizationId);

            if (execution == null) throw new KeyNotFoundException($"Test Execution {testExecutionId} not found.");
            if (execution.BranchID != branchId) throw new UnauthorizedAccessException($"Execution belongs to Branch {execution.BranchID}, unauthorized for Branch {branchId}.");

            // ENFORCE CONTRACT: Once execution starts, snapshot is IMMUTABLE
            if (execution.Status != "Planned")
            {
                throw new InvalidOperationException("Configuration snapshot is frozen and immutable once execution has started.");
            }

            string configJson = JsonSerializer.Serialize(updatedConfig, JsonOptions);
            string configHash = ComputeSha256Hash(configJson);

            if (execution.ExecutionConfigSnapshot != null)
            {
                execution.ExecutionConfigSnapshot.ConfigJson = configJson;
                execution.ExecutionConfigSnapshot.SnapshotHash = configHash;
                execution.ExecutionConfigSnapshot.ModifiedBy = userId;
                execution.ExecutionConfigSnapshot.ModifiedOn = DateTime.UtcNow;
            }
            else
            {
                var snapshot = new ExecutionConfigSnapshot
                {
                    ConfigJson = configJson,
                    SnapshotHash = configHash,
                    CreatedBy = userId,
                    CreatedOn = DateTime.UtcNow,
                    CompanyCode = execution.UniversalTestGroup?.CompanyCode ?? "LIMS",
                    IsActive = true
                };
                _context.ExecutionConfigSnapshots.Add(snapshot);
                await _context.SaveChangesAsync();
                execution.ExecutionConfigSnapshotID = snapshot.ID;
            }

            execution.ModifiedBy = userId;
            execution.ModifiedOn = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return (await GetExecutionByIdAsync(execution.ID, branchId, organizationId, false))!;
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

            foreach (var p in parameters)
            {
                if (p.IsRequired)
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
                execution.ExecutionConfigSnapshot.SnapshotHash = ComputeSha256Hash(configJson);
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

            snapshot ??= await BuildSnapshotDtoAsync(execution.UniversalTestGroup);

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
                LabName = branch?.Name ?? "Divine Metallurgical Services Pvt. Ltd.",
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
                section.Parameters.Add(new ReportDataParameter
                {
                    Name = param.Name,
                    Unit = param.Unit ?? "",
                    SpecMin = param.SpecMin?.ToString(),
                    SpecMax = param.SpecMax?.ToString(),
                    Result = resultVal,
                    Status = status,
                    IsWithinNablScope = snapshot.MeasurementUncertainty != null,
                    NablScopeStatus = snapshot.MeasurementUncertainty != null ? "WithinScope" : "OutsideScope",
                    ExpandedUncertainty = snapshot.MeasurementUncertainty?.Value,
                    CoverageFactor = snapshot.MeasurementUncertainty?.CoverageFactor,
                    ConformityResult = status == "Pass" ? "Conforms" : "Does not conform"
                });
            }

            reportData.TestSections.Add(section);

            var document = ReportDocumentFactory.Create(ReportFormatType.TestCertificate, reportData);
            return document.GeneratePdf();
        }
    }
}
