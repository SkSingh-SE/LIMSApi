using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Services
{
    public class UniversalResultService : IUniversalResultService
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            WriteIndented = false
        };

        private static readonly HashSet<string> FinalStates = new(StringComparer.OrdinalIgnoreCase)
            { "Finalized", "UnderReview", "Verified", "Approved", "Rejected", "ReworkRequired" };

        private readonly LIMSContext _context;
        private readonly FormulaEvaluator _formulaEvaluator;
        private readonly ISnapshotIntegrityValidator _integrity;

        public UniversalResultService(
            LIMSContext context,
            FormulaEvaluator formulaEvaluator,
            ISnapshotIntegrityValidator integrity)
        {
            _context = context;
            _formulaEvaluator = formulaEvaluator;
            _integrity = integrity;
        }

        public async Task<UniversalResultDetailDto?> GetLatestByExecutionAsync(long testExecutionId, long branchId, long organizationId, bool canViewAllBranches = false)
        {
            var q = _context.UniversalTestResults
                .Include(r => r.Parameters)
                .Include(r => r.Findings)
                .Include(r => r.Audits)
                .Where(r => r.TestExecutionID == testExecutionId && r.OrganizationID == organizationId && r.IsActive);
            if (!canViewAllBranches)
                q = q.Where(r => r.BranchID == branchId);
            var entity = await q.OrderByDescending(r => r.RevisionNo).FirstOrDefaultAsync();
            if (entity == null) return null;
            return await MapDetailAsync(entity);
        }

        public async Task<UniversalResultDetailDto?> GetByIdAsync(long resultId, long branchId, long organizationId, bool canViewAllBranches = false)
        {
            var q = _context.UniversalTestResults
                .Include(r => r.Parameters)
                .Include(r => r.Findings)
                .Include(r => r.Audits)
                .Where(r => r.ID == resultId && r.OrganizationID == organizationId && r.IsActive);
            if (!canViewAllBranches)
                q = q.Where(r => r.BranchID == branchId);
            var entity = await q.FirstOrDefaultAsync();
            if (entity == null) return null;
            return await MapDetailAsync(entity);
        }

        public async Task<UniversalResultDetailDto> EvaluateAsync(long testExecutionId, long userId, long branchId, long organizationId, string? remarks = null)
        {
            var execution = await _context.TestExecutions
                .Include(e => e.ExecutionConfigSnapshot)
                .Include(e => e.UniversalTestGroup)
                    .ThenInclude(u => u.SampleTestPlan)
                        .ThenInclude(p => p.SampleDetail)
                            .ThenInclude(s => s!.SampleInward!)
                .Include(e => e.UniversalTestGroup).ThenInclude(u => u.LaboratoryTest)
                .Include(e => e.TestSpecimens.Where(s => s.IsActive))
                    .ThenInclude(s => s.TestObservations.Where(o => o.IsActive))
                        .ThenInclude(o => o.ParameterObservationResults.Where(r => r.IsActive))
                .FirstOrDefaultAsync(e => e.ID == testExecutionId && e.OrganizationID == organizationId && e.IsActive)
                ?? throw new KeyNotFoundException($"Test Execution {testExecutionId} not found.");

            if (execution.BranchID != branchId)
                throw new UnauthorizedAccessException($"Execution belongs to Branch {execution.BranchID}, not authorized for Branch {branchId}.");

            if (execution.Status != "Completed" && execution.Status != "Verified" && execution.Status != "Approved" && execution.Status != "InProgress")
                throw new InvalidOperationException($"Cannot evaluate result while execution is '{execution.Status}'.");

            var integrity = await _integrity.ValidateIntegrityAsync(testExecutionId);
            if (!integrity.IsValid)
                throw new InvalidOperationException($"Snapshot integrity violation: {string.Join("; ", integrity.Errors)}");

            if (execution.ExecutionConfigSnapshot == null || string.IsNullOrWhiteSpace(execution.ExecutionConfigSnapshot.ConfigJson))
                throw new InvalidOperationException("Frozen execution snapshot is missing; cannot evaluate historical result.");

            var snapshot = JsonSerializer.Deserialize<TestExecutionConfigSnapshotDto>(
                execution.ExecutionConfigSnapshot.ConfigJson, JsonOptions)
                ?? throw new InvalidOperationException("Frozen execution snapshot could not be deserialized.");

            var utg = execution.UniversalTestGroup;
            bool isStandardless = utg.SpecificationHeaderID == null && utg.SpecificationVersionID == null;

            // Check for Post-Execution Approved Configuration Adjustment (Part C)
            var approvedAdjustment = await _context.ConfigurationAdjustments
                .Include(a => a.Items)
                .Where(a => a.UniversalTestGroupID == utg.ID && a.Status == "Approved" && a.IsActive)
                .OrderByDescending(a => a.AdjustmentNumber)
                .FirstOrDefaultAsync();

            AdjustedConfigurationDto? adjustedConfig = null;
            if (approvedAdjustment != null && !string.IsNullOrWhiteSpace(approvedAdjustment.AdjustedConfigurationJson))
            {
                try
                {
                    adjustedConfig = JsonSerializer.Deserialize<AdjustedConfigurationDto>(
                        approvedAdjustment.AdjustedConfigurationJson, JsonOptions);
                }
                catch { }
            }

            var maxRev = await _context.UniversalTestResults
                .IgnoreQueryFilters()
                .Where(r => r.TestExecutionID == testExecutionId)
                .Select(r => (int?)r.RevisionNo)
                .MaxAsync() ?? 0;

            var latest = await _context.UniversalTestResults
                .Where(r => r.TestExecutionID == testExecutionId && r.IsActive)
                .OrderByDescending(r => r.RevisionNo)
                .FirstOrDefaultAsync();

            UniversalTestResult result;
            bool isNewRevision;
            if (latest == null || FinalStates.Contains(latest.ResultStatus))
            {
                result = new UniversalTestResult
                {
                    TestExecutionID = execution.ID,
                    UniversalTestGroupID = utg.ID,
                    BranchID = execution.BranchID,
                    OrganizationID = execution.OrganizationID,
                    RevisionNo = maxRev + 1,
                    ResultStatus = "Calculated",
                    OverallDecision = "NOT_EVALUATED",
                    SnapshotHash = execution.ExecutionConfigSnapshot.SnapshotHash,
                    ExecutionConfigSnapshotID = execution.ExecutionConfigSnapshotID,
                    CompanyCode = execution.CompanyCode,
                    IsActive = true,
                    CreatedBy = userId,
                    CreatedOn = DateTime.UtcNow,
                    ModifiedBy = userId,
                    ModifiedOn = DateTime.UtcNow,
                    ConcurrencyToken = Guid.NewGuid().ToString("N")
                };
                _context.UniversalTestResults.Add(result);
                await _context.SaveChangesAsync();
                isNewRevision = true;
            }
            else
            {
                result = latest;
                result.SnapshotHash = execution.ExecutionConfigSnapshot.SnapshotHash;
                result.ModifiedBy = userId;
                result.ModifiedOn = DateTime.UtcNow;
                isNewRevision = false;
                var oldParams = _context.UniversalTestResultParameters.Where(p => p.UniversalTestResultID == result.ID);
                _context.UniversalTestResultParameters.RemoveRange(oldParams);
                await _context.SaveChangesAsync();
            }

            var observations = execution.TestSpecimens
                .Where(s => !s.IsDiscarded)
                .SelectMany(s => s.TestObservations)
                .ToList();
            var allRows = observations.SelectMany(o => o.ParameterObservationResults).ToList();

            string canonicalRule = CanonicalDecisionRule(snapshot.AcceptanceCriteria?.DecisionRule);
            string? legacyInwardRule = utg.SampleTestPlan?.SampleDetail?.SampleInward?.DecisionRule;
            bool legacyMou = string.Equals(legacyInwardRule, "With MOU consideration", StringComparison.OrdinalIgnoreCase);
            string effectiveRule = canonicalRule;
            bool ruleConflict = false;
            if (canonicalRule == "ALL_REQUIRED_PASS" && legacyMou)
            {
                effectiveRule = "WITH_MOU_GUARD";
                ruleConflict = true;
            }

            decimal? testLevelU = snapshot.MeasurementUncertainty?.ExpandedUncertainty
                ?? snapshot.MeasurementUncertainty?.Value;
            decimal testK = snapshot.MeasurementUncertainty?.CoverageFactor ?? 2.0m;
            decimal? testUc = snapshot.MeasurementUncertainty?.CombinedUncertainty;

            var orderedSnapshotParams = snapshot.Parameters.OrderBy(p => p.DisplayOrder).ToList();
            var aggregated = new Dictionary<long, (decimal? numeric, string? raw)>();
            var rawTexts = new Dictionary<long, List<string>>();
            foreach (var sp in orderedSnapshotParams)
            {
                var rows = allRows.Where(r => r.ParameterMasterID == sp.ParameterMasterID).ToList();
                var numerics = rows.Where(r => r.NumericValue.HasValue).Select(r => (double)r.NumericValue!.Value).ToList();
                decimal? agg = null;
                if (numerics.Any())
                {
                    var calc = _formulaEvaluator.CalculateAggregate(numerics, sp.AggregateType ?? "Average");
                    if (calc.HasValue) agg = (decimal)calc.Value;
                }
                else if (rows.Any(r => !string.IsNullOrWhiteSpace(r.RawValue)) && decimal.TryParse(rows.First(r => !string.IsNullOrWhiteSpace(r.RawValue)).RawValue,
                    System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var parsed))
                {
                    agg = parsed;
                }
                string? raw = rows.FirstOrDefault(r => !string.IsNullOrWhiteSpace(r.RawValue))?.RawValue
                    ?? (agg.HasValue ? agg.Value.ToString(System.Globalization.CultureInfo.InvariantCulture) : null);
                aggregated[sp.ParameterMasterID] = (agg, raw);
                rawTexts[sp.ParameterMasterID] = rows.Where(r => !string.IsNullOrWhiteSpace(r.RawValue)).Select(r => r.RawValue!).ToList();
            }

            var factored = new Dictionary<long, (decimal? value, string? code, string? op)>();
            var factorNotes = new Dictionary<long, string>();
            foreach (var sp in orderedSnapshotParams)
            {
                var (agg, _) = aggregated[sp.ParameterMasterID];
                var match = snapshot.Factors
                    .Where(f => f.InputParameterID == sp.ParameterMasterID && IsMeasuredStage(f.AppliedOn, out _))
                    .OrderBy(f => f.DisplayOrder)
                    .FirstOrDefault();
                if (match != null && agg.HasValue && string.Equals(sp.InputType ?? "Decimal", "Decimal", StringComparison.OrdinalIgnoreCase))
                {
                    var canonicalType = CanonicalFactorType(match.FactorType, out var legacyType);
                    var legacyStage = IsMeasuredStage(match.AppliedOn, out var legacyApplied);
                    if (canonicalType == "DIVISION" && match.FactorValue == 0)
                        throw new InvalidOperationException($"Frozen factor {match.FactorCode} is DIVISION by zero for {sp.Code}.");
                    var out_ = FactorConversionService.ApplyTransformation(canonicalType, agg.Value, match.FactorValue);
                    factored[sp.ParameterMasterID] = (out_, match.FactorCode ?? match.FactorName, FactorConversionService.DescribeOperation(canonicalType, match.FactorValue));
                    if (legacyType || legacyApplied)
                        factorNotes[sp.ParameterMasterID] = $"Legacy factor text '{match.FactorType}/{match.AppliedOn}' interpreted as {canonicalType}/Measured Value (frozen).";
                }
                else
                {
                    factored[sp.ParameterMasterID] = (agg, null, null);
                }
            }

            var variables = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);
            foreach (var sp in orderedSnapshotParams)
            {
                var fv = factored[sp.ParameterMasterID].value;
                if (fv.HasValue)
                {
                    variables[sp.Code] = (double)fv.Value;
                    variables[$"P{sp.ParameterMasterID}"] = (double)fv.Value;
                    variables[sp.ParameterMasterID.ToString()] = (double)fv.Value;
                }
            }

            var calcParams = orderedSnapshotParams.Where(p => p.IsCalculated && !string.IsNullOrWhiteSpace(p.Formula)).ToList();
            List<SnapshotParameterDto> sortedCalc;
            try
            {
                sortedCalc = _formulaEvaluator.OrderByTopologicalSort(calcParams, p => p.Code, p => p.Formula);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Frozen formula dependency error: {ex.Message}");
            }

            var calcValues = new Dictionary<long, (decimal? value, string? trace, string? formula)>();
            var traceSteps = new List<object>();
            foreach (var cp in sortedCalc)
            {
                var trace = _formulaEvaluator.EvaluateWithTrace(cp.Formula!, variables, cp.DecimalPrecision);
                if (trace.IsValid && trace.Result.HasValue)
                {
                    var dec = (decimal)trace.Result.Value;
                    calcValues[cp.ParameterMasterID] = (dec, trace.SubstitutionTrace, cp.Formula);
                    variables[cp.Code] = trace.Result.Value;
                    variables[$"P{cp.ParameterMasterID}"] = trace.Result.Value;
                    variables[cp.ParameterMasterID.ToString()] = trace.Result.Value;
                    var fmatch = snapshot.Factors
                        .Where(f => f.InputParameterID == cp.ParameterMasterID && IsMeasuredStage(f.AppliedOn, out _))
                        .OrderBy(f => f.DisplayOrder).FirstOrDefault();
                    if (fmatch != null)
                    {
                        var fCanonical = CanonicalFactorType(fmatch.FactorType, out var fLegacyType);
                        IsMeasuredStage(fmatch.AppliedOn, out var fLegacyApplied);
                        var post = FactorConversionService.ApplyTransformation(fCanonical, dec, fmatch.FactorValue);
                        calcValues[cp.ParameterMasterID] = (post, trace.SubstitutionTrace + $" | factor {FactorConversionService.DescribeOperation(fCanonical, fmatch.FactorValue)} → {post}", cp.Formula);
                        variables[cp.Code] = (double)post;
                        variables[$"P{cp.ParameterMasterID}"] = (double)post;
                        variables[cp.ParameterMasterID.ToString()] = (double)post;
                        factored[cp.ParameterMasterID] = (post, fmatch.FactorCode ?? fmatch.FactorName, FactorConversionService.DescribeOperation(fCanonical, fmatch.FactorValue));
                        if (fLegacyType || fLegacyApplied)
                            factorNotes[cp.ParameterMasterID] = $"Legacy factor text '{fmatch.FactorType}/{fmatch.AppliedOn}' interpreted as {fCanonical}/Measured Value (frozen).";
                    }
                    else
                    {
                        factored[cp.ParameterMasterID] = (dec, null, null);
                    }
                }
                else
                {
                    calcValues[cp.ParameterMasterID] = (null, trace.SubstitutionTrace ?? trace.ErrorMessage, cp.Formula);
                }
                traceSteps.Add(new { parameterCode = cp.Code, formula = cp.Formula, trace = calcValues[cp.ParameterMasterID].trace });
            }

            bool isInformationalRule = effectiveRule == "INFORMATIONAL";
            var paramEntities = new List<UniversalTestResultParameter>();
            foreach (var sp in orderedSnapshotParams)
            {
                var (agg, raw) = aggregated[sp.ParameterMasterID];
                var (fval, fcode, fop) = factored[sp.ParameterMasterID];
                bool isCalc = sp.IsCalculated;
                decimal? calcVal = null;
                string? trace = null;
                if (isCalc && calcValues.TryGetValue(sp.ParameterMasterID, out var cv))
                {
                    calcVal = cv.value;
                    trace = cv.trace;
                    fval = cv.value;
                    var fm = snapshot.Factors.FirstOrDefault(f => f.InputParameterID == sp.ParameterMasterID && IsMeasuredStage(f.AppliedOn, out _));
                    if (fm != null) { fcode = fm.FactorCode ?? fm.FactorName; fop = FactorConversionService.DescribeOperation(CanonicalFactorType(fm.FactorType, out _), fm.FactorValue); }
                }

                string inputType = (sp.InputType ?? "Decimal").Trim();
                bool isExplicitTextual = inputType.Equals("Text", StringComparison.OrdinalIgnoreCase)
                    || inputType.Equals("Dropdown", StringComparison.OrdinalIgnoreCase)
                    || inputType.Equals("Selection", StringComparison.OrdinalIgnoreCase)
                    || inputType.Equals("Boolean", StringComparison.OrdinalIgnoreCase)
                    || inputType.Equals("Date", StringComparison.OrdinalIgnoreCase)
                    || inputType.Equals("DateTime", StringComparison.OrdinalIgnoreCase);

                bool isNumericType = !isExplicitTextual
                    || sp.SpecMin.HasValue
                    || sp.SpecMax.HasValue
                    || sp.SpecTarget.HasValue
                    || fval.HasValue;

                decimal? complianceValue = null;
                string? displayValue = null;
                string? reportedValue = null;
                if (isNumericType && fval.HasValue)
                {
                    complianceValue = Math.Round(fval.Value, sp.DecimalPrecision, MidpointRounding.AwayFromZero);
                    displayValue = complianceValue.Value.ToString($"F{sp.DecimalPrecision}", System.Globalization.CultureInfo.InvariantCulture);
                    var repFactor = snapshot.Factors
                        .Where(f => f.InputParameterID == sp.ParameterMasterID && !IsMeasuredStage(f.AppliedOn, out _))
                        .OrderBy(f => f.DisplayOrder).FirstOrDefault();
                    if (repFactor != null)
                    {
                        var repCanonical = CanonicalFactorType(repFactor.FactorType, out _);
                        var rep = FactorConversionService.ApplyTransformation(repCanonical, complianceValue.Value, repFactor.FactorValue);
                        reportedValue = Math.Round(rep, sp.DecimalPrecision, MidpointRounding.AwayFromZero)
                            .ToString($"F{sp.DecimalPrecision}", System.Globalization.CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        reportedValue = displayValue;
                    }
                }
                else if (!isNumericType)
                {
                    displayValue = raw;
                    reportedValue = raw;
                }

                string cvSource;
                if (isCalc) cvSource = "CALCULATED";
                else if (!string.IsNullOrEmpty(fcode)) cvSource = "FACTORED";
                else if (rawTexts.TryGetValue(sp.ParameterMasterID, out var rList) && rList.Count > 1) cvSource = "AGGREGATED";
                else cvSource = "OBSERVATION";

                string reqStatus;
                if (isStandardless)
                    reqStatus = "SPECIFICATION_NOT_APPLICABLE";
                else if (!sp.SpecMin.HasValue && !sp.SpecMax.HasValue && !sp.SpecTarget.HasValue)
                    reqStatus = sp.IsRequired ? "NOT_CONFIGURED" : "SPECIFICATION_NOT_APPLICABLE";
                else
                    reqStatus = "RESOLVED";

                string verdict;
                string? note = null;
                bool guardApplied = false;

                decimal? nominalLo = sp.SpecMin;
                decimal? nominalHi = sp.SpecMax;
                decimal? effLo = sp.EffectiveMin;
                decimal? effHi = sp.EffectiveMax;

                if (!effLo.HasValue && !effHi.HasValue)
                {
                    decimal absLowerTol = sp.MinTolerance.HasValue ? Math.Abs(sp.MinTolerance.Value) : 0m;
                    decimal absUpperTol = sp.MaxTolerance.HasValue ? Math.Abs(sp.MaxTolerance.Value) : 0m;

                    if (!string.IsNullOrWhiteSpace(sp.ComparisonCriteria) && sp.ComparisonCriteria.Equals("TargetTolerance", StringComparison.OrdinalIgnoreCase) && sp.SpecTarget.HasValue)
                    {
                        effLo = sp.SpecTarget.Value - absLowerTol;
                        effHi = sp.SpecTarget.Value + absUpperTol;
                        note = $"Target {sp.SpecTarget} ± tol [{effLo},{effHi}] (frozen).";
                    }
                    else if (nominalLo.HasValue && nominalHi.HasValue)
                    {
                        effLo = nominalLo.Value - absLowerTol;
                        effHi = nominalHi.Value + absUpperTol;
                    }
                    else if (nominalLo.HasValue)
                    {
                        effLo = nominalLo.Value - absLowerTol;
                        effHi = null;
                    }
                    else if (nominalHi.HasValue)
                    {
                        effLo = null;
                        effHi = nominalHi.Value + absUpperTol;
                    }
                }

                decimal? lo = effLo ?? nominalLo;
                decimal? hi = effHi ?? nominalHi;

                if (adjustedConfig != null)
                {
                    var adjReq = adjustedConfig.Requirements?.FirstOrDefault(r => r.ParameterID == sp.ParameterMasterID);
                    var adjParam = adjustedConfig.Parameters?.FirstOrDefault(p => p.ParameterID == sp.ParameterMasterID);
                    if (adjReq != null && (adjReq.Min.HasValue || adjReq.Max.HasValue))
                    {
                        lo = adjReq.Min;
                        hi = adjReq.Max;
                    }
                    else if (adjParam != null && (adjParam.MinValue.HasValue || adjParam.MaxValue.HasValue))
                    {
                        lo = adjParam.MinValue;
                        hi = adjParam.MaxValue;
                    }
                }

                decimal? paramU = sp.ParameterExpandedUncertainty;
                decimal? paramUc = sp.ParameterCombinedUncertainty;
                decimal? paramK = sp.ParameterCoverageFactor;
                string muSource = sp.MUSource ?? (paramU.HasValue ? "PARAMETER_MASTER" : "NONE");

                if (isInformationalRule || !sp.IsReportable)
                {
                    verdict = "INFORMATIONAL";
                    note = "Excluded from overall decision (informational / non-reportable).";
                }
                else if (!isNumericType)
                {
                    if (string.IsNullOrWhiteSpace(raw))
                        verdict = sp.IsRequired ? "INVALID" : "NOT_EVALUATED";
                    else if (inputType.Equals("Dropdown", StringComparison.OrdinalIgnoreCase) || inputType.Equals("Selection", StringComparison.OrdinalIgnoreCase))
                    {
                        var allowed = (sp.DropdownOptions ?? new List<SnapshotDropdownOptionDto>())
                            .SelectMany(o => new[] { o.Value, o.DisplayText })
                            .Where(s => !string.IsNullOrWhiteSpace(s))
                            .ToHashSet(StringComparer.OrdinalIgnoreCase);
                        verdict = allowed.Count == 0 || allowed.Contains(raw.Trim()) ? "PASS" : "FAIL";
                    }
                    else if (inputType.Equals("Boolean", StringComparison.OrdinalIgnoreCase))
                    {
                        var rv = raw.Trim().ToLowerInvariant();
                        verdict = (rv == "true" || rv == "false" || rv == "yes" || rv == "no" || rv == "1" || rv == "0" || rv == "pass" || rv == "fail") ? "PASS" : "INVALID";
                    }
                    else
                    {
                        verdict = "PASS";
                    }
                }
                else
                {
                    if (!complianceValue.HasValue)
                    {
                        verdict = sp.IsRequired ? "INVALID" : "NOT_EVALUATED";
                        if (verdict == "INVALID") note = isCalc ? "Calculated value could not be resolved from frozen formula." : "Mandatory numeric observation missing.";
                    }
                    else if (reqStatus == "NOT_CONFIGURED" || reqStatus == "SPECIFICATION_NOT_APPLICABLE")
                    {
                        verdict = reqStatus == "SPECIFICATION_NOT_APPLICABLE" ? "INFORMATIONAL" : "NOT_CONFIGURED";
                        if (verdict == "INFORMATIONAL") note = "Standardless observation recorded without conformity verdict.";
                    }
                    else
                    {
                        var baseVerdict = _formulaEvaluator.DetermineResultStatus(complianceValue, lo, hi) ?? "NOT_EVALUATED";
                        verdict = baseVerdict.ToUpperInvariant() switch
                        {
                            "PASS" => "PASS",
                            "FAIL" => "FAIL",
                            "MARGINAL" => effectiveRule == "MARGINAL_PERMITTED" ? "MARGINAL" : "PASS",
                            _ => "NOT_EVALUATED"
                        };
                        if (effectiveRule == "WITH_MOU_GUARD" && paramU.HasValue && (lo.HasValue || hi.HasValue))
                        {
                            guardApplied = true;
                            bool withinMin = !lo.HasValue || complianceValue.Value >= (lo.Value + paramU.Value);
                            bool withinMax = !hi.HasValue || complianceValue.Value <= (hi.Value - paramU.Value);
                            verdict = (withinMin && withinMax) ? "PASS" : "FAIL";
                            string loStr = lo.HasValue ? $"≥ {(lo.Value + paramU.Value).ToString(System.Globalization.CultureInfo.InvariantCulture)}" : "";
                            string hiStr = hi.HasValue ? $"≤ {(hi.Value - paramU.Value).ToString(System.Globalization.CultureInfo.InvariantCulture)}" : "";
                            string rangeStr = lo.HasValue && hi.HasValue ? $"[{(lo.Value + paramU.Value).ToString(System.Globalization.CultureInfo.InvariantCulture)}, {(hi.Value - paramU.Value).ToString(System.Globalization.CultureInfo.InvariantCulture)}]" : (lo.HasValue ? loStr : hiStr);
                            note = ((note ?? "") + $" Guard-band U={paramU} (k={paramK ?? 2}): {rangeStr} (frozen).").Trim();
                        }
                    }
                }

                if (factorNotes.TryGetValue(sp.ParameterMasterID, out var fnote))
                    note = ((note ?? "") + " " + fnote).Trim();

                paramEntities.Add(new UniversalTestResultParameter
                {
                    UniversalTestResultID = result.ID,
                    ParameterMasterID = sp.ParameterMasterID,
                    ParameterCode = sp.Code,
                    ParameterName = sp.Name,
                    InputType = inputType,
                    Unit = sp.Unit,
                    DecimalPrecision = sp.DecimalPrecision,
                    IsCalculated = isCalc,
                    IsMandatory = sp.IsRequired,
                    IsReportable = sp.IsReportable,
                    DisplayOrder = sp.DisplayOrder,
                    RawValue = raw,
                    RawNumericValue = agg,
                    AppliedFactorCode = fcode,
                    AppliedFactorOperation = fop,
                    FactoredValue = isNumericType ? fval : null,
                    Formula = isCalc ? sp.Formula : null,
                    SubstitutionTrace = trace,
                    CalculatedValue = calcVal,
                    ComplianceValue = complianceValue,
                    DisplayValue = displayValue,
                    ReportedValue = reportedValue,
                    SpecMin = nominalLo,
                    SpecMax = nominalHi,
                    SpecTarget = sp.SpecTarget,
                    MinTolerance = sp.MinTolerance,
                    MaxTolerance = sp.MaxTolerance,
                    EffectiveMin = lo,
                    EffectiveMax = hi,
                    ToleranceSource = sp.ToleranceSource,
                    ToleranceType = sp.ToleranceType,
                    AppliedTolerance = sp.AppliedTolerance,
                    ComplianceValueSource = cvSource,
                    RequirementStatus = reqStatus,
                    Verdict = verdict,
                    CombinedUncertainty = paramUc,
                    ExpandedUncertainty = paramU,
                    CoverageFactor = paramK,
                    MUSource = muSource,
                    GuardBandApplied = guardApplied,
                    EvaluationNote = note,
                    CompanyCode = execution.CompanyCode,
                    IsActive = true,
                    CreatedBy = userId,
                    CreatedOn = DateTime.UtcNow,
                    ModifiedBy = userId,
                    ModifiedOn = DateTime.UtcNow
                });
            }

            _context.UniversalTestResultParameters.AddRange(paramEntities);
            await _context.SaveChangesAsync();

            var counted = paramEntities.Where(p => p.Verdict != "INFORMATIONAL").ToList();
            string overall;
            if (counted.Any(p => p.Verdict == "INVALID"))
                overall = "INVALID";
            else if (counted.Any(p => p.Verdict == "FAIL"))
                overall = "FAIL";
            else if (!counted.Any())
                overall = isStandardless || paramEntities.All(p => p.Verdict == "INFORMATIONAL") ? "INFORMATIONAL" : "NOT_CONFIGURED";
            else if (counted.All(p => p.Verdict == "NOT_CONFIGURED"))
                overall = "NOT_CONFIGURED";
            else if (counted.All(p => p.Verdict == "NOT_EVALUATED"))
                overall = "NOT_EVALUATED";
            else if (counted.Any(p => p.Verdict == "MARGINAL"))
                overall = "MARGINAL";
            else if (counted.Any(p => p.Verdict == "NOT_CONFIGURED"))
                overall = "NOT_CONFIGURED";
            else
                overall = "PASS";

            result.OverallDecision = overall;
            result.ResultStatus = "Calculated";
            result.DecisionRule = effectiveRule;
            result.AcceptanceCriteriaCode = snapshot.AcceptanceCriteria?.AcceptanceCriteriaCode;
            result.CalculationTraceJson = JsonSerializer.Serialize(traceSteps, JsonOptions);

            result.ComplianceSummaryJson = JsonSerializer.Serialize(new
            {
                executionSnapshotSpecification = new
                {
                    specificationHeaderID = snapshot.SpecificationHeaderID,
                    specificationTitle = snapshot.SpecificationTitle,
                    gradeName = snapshot.GradeName,
                    isStandardless
                },
                effectiveComplianceSpecification = approvedAdjustment != null ? new
                {
                    adjustmentID = approvedAdjustment.ID,
                    adjustmentNumber = approvedAdjustment.AdjustmentNumber,
                    approvedBy = approvedAdjustment.ApprovedBy,
                    approvedOn = approvedAdjustment.ApprovedOn,
                    reason = approvedAdjustment.OverallReason,
                    isAdjusted = true
                } : null,
                decisionRule = effectiveRule,
                canonicalSnapshotRule = snapshot.AcceptanceCriteria?.DecisionRule,
                legacyInwardRule,
                ruleConflictResolvedToSnapshot = ruleConflict,
                expandedUncertainty = testLevelU,
                coverageFactor = testK,
                combinedUncertainty = testUc,
                overall,
                counts = new
                {
                    pass = counted.Count(p => p.Verdict == "PASS"),
                    fail = counted.Count(p => p.Verdict == "FAIL"),
                    marginal = counted.Count(p => p.Verdict == "MARGINAL"),
                    invalid = counted.Count(p => p.Verdict == "INVALID"),
                    notConfigured = counted.Count(p => p.Verdict == "NOT_CONFIGURED"),
                    informational = paramEntities.Count(p => p.Verdict == "INFORMATIONAL")
                },
                rounding = $"Round(compliance, DecimalPrecision, AwayFromZero); display == compliance (legacy parity).",
                marginalNote = "MARGINAL = legacy 5% boundary from FormulaEvaluator.DetermineResultStatus (presentation boundary, not ISO rule)."
            }, JsonOptions);
            result.ModifiedBy = userId;
            result.ModifiedOn = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            await AddAuditAsync(result.ID, execution.ID, result.RevisionNo, userId,
                isNewRevision ? "ResultGenerated" : "ResultRegenerated",
                execution.ExecutionConfigSnapshot.SnapshotHash,
                $"Evaluation from frozen snapshot; overall={overall}; rule={effectiveRule}; params={paramEntities.Count}; remarks={remarks}");

            foreach (var p in paramEntities)
            {
                await AddAuditAsync(result.ID, execution.ID, result.RevisionNo, userId,
                    "ParameterEvaluated", result.SnapshotHash,
                    $"{p.ParameterCode}: raw={p.RawValue} factored={p.FactoredValue} calc={p.CalculatedValue} compliance={p.ComplianceValue} verdict={p.Verdict} req={p.RequirementStatus}");
            }

            await _context.SaveChangesAsync();
            return (await GetByIdAsync(result.ID, branchId, organizationId, true))!;
        }

        public async Task<UniversalResultDetailDto> FinalizeAsync(long resultId, string concurrencyToken, long userId, long branchId, long organizationId, string? remarks = null)
        {
            var result = await _context.UniversalTestResults
                .Include(r => r.Parameters)
                .FirstOrDefaultAsync(r => r.ID == resultId && r.OrganizationID == organizationId && r.IsActive)
                ?? throw new KeyNotFoundException($"Result {resultId} not found.");
            if (result.BranchID != branchId)
                throw new UnauthorizedAccessException($"Result belongs to Branch {result.BranchID}, not authorized for Branch {branchId}.");
            if (result.ResultStatus != "Calculated" && result.ResultStatus != "Draft")
                throw new InvalidOperationException($"Only Calculated/Draft results can be finalized. Current: '{result.ResultStatus}'.");
            if (!string.Equals(result.ConcurrencyToken, concurrencyToken, StringComparison.Ordinal))
                throw new InvalidOperationException("Stale result revision. Reload and retry.");

            var integrity = await _integrity.ValidateIntegrityAsync(result.TestExecutionID);
            if (!integrity.IsValid)
                throw new InvalidOperationException($"Finalization blocked by snapshot integrity violation: {string.Join("; ", integrity.Errors)}");

            var counted = result.Parameters.Where(p => p.Verdict != "INFORMATIONAL").ToList();
            if (counted.Any(p => p.Verdict == "INVALID"))
                throw new InvalidOperationException("Cannot finalize: one or more mandatory parameters are INVALID (missing value or formula failure).");
            if (!result.Parameters.Any())
                throw new InvalidOperationException("Cannot finalize an empty result.");

            var blocking = await _context.UniversalReviewFindings
                .AnyAsync(f => f.UniversalTestResultID == result.ID && f.IsActive && f.IsBlocking && f.Status == "Open");
            if (blocking)
                throw new InvalidOperationException("Cannot finalize: blocking review findings are still open.");

            result.ResultStatus = "Finalized";
            result.FinalizedBy = userId;
            result.FinalizedOn = DateTime.UtcNow;
            if (!string.IsNullOrWhiteSpace(remarks)) result.ReviewRemarks = remarks.Trim();
            result.ConcurrencyToken = Guid.NewGuid().ToString("N");
            result.ModifiedBy = userId;
            result.ModifiedOn = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            await AddAuditAsync(result.ID, result.TestExecutionID, result.RevisionNo, userId,
                "ResultFinalized", result.SnapshotHash, $"Overall={result.OverallDecision}; remarks={remarks}");
            await _context.SaveChangesAsync();
            return (await GetByIdAsync(result.ID, branchId, organizationId, true))!;
        }

        public async Task<Phase9HandoffDto> GetPhase9HandoffAsync(long testExecutionId, long branchId, long organizationId, bool canViewAllBranches = false)
        {
            var latest = await GetLatestByExecutionAsync(testExecutionId, branchId, organizationId, canViewAllBranches)
                ?? throw new KeyNotFoundException("No result exists for this execution.");
            if (latest.ResultStatus != "Approved")
                throw new InvalidOperationException($"Phase 9 handoff requires an Approved result. Current: '{latest.ResultStatus}'.");
            return new Phase9HandoffDto
            {
                TestExecutionID = testExecutionId,
                UniversalTestResultID = latest.ID,
                ApprovedRevisionNo = latest.RevisionNo,
                OverallDecision = latest.OverallDecision,
                SnapshotHash = latest.SnapshotHash,
                ExecutionConfigSnapshotID = latest.ExecutionConfigSnapshotID,
                ApprovedBy = latest.ApprovedBy,
                ApprovedOn = latest.ApprovedOn,
                ReportableParameters = latest.Parameters.Where(p => p.IsReportable).ToList()
            };
        }

        private static string CanonicalFactorType(string? frozen, out bool isLegacyText)
        {
            isLegacyText = false;
            if (string.IsNullOrWhiteSpace(frozen)) return "MULTIPLICATION";
            var v = frozen.Trim().ToUpperInvariant();
            if (v.Contains("DIVISION")) return "DIVISION";
            if (v.Contains("SUBTRACTIVE")) return "SUBTRACTIVE_OFFSET";
            if (v.Contains("ADDITIVE")) return "ADDITIVE_OFFSET";
            if (v.Contains("OFFSET"))
            {
                isLegacyText = true;
                return "ADDITIVE_OFFSET";
            }
            if (v.Contains("MULTIPLICATION") || v.Contains("MULTIPL")) return "MULTIPLICATION";
            isLegacyText = true;
            return "MULTIPLICATION";
        }

        private static bool IsMeasuredStage(string? appliedOn, out bool isLegacyText)
        {
            isLegacyText = false;
            if (string.IsNullOrWhiteSpace(appliedOn)) { isLegacyText = true; return true; }
            var v = appliedOn.Trim().ToUpperInvariant();
            if (v.Contains("REPORTED")) return false;
            if (!v.Contains("MEASURED")) isLegacyText = true;
            return true;
        }

        private static string CanonicalDecisionRule(string? frozen)
        {
            if (string.IsNullOrWhiteSpace(frozen)) return "ALL_REQUIRED_PASS";
            var v = frozen.Trim().ToUpperInvariant();
            if (v.Contains("MOU") || v.Contains("GUARD")) return "WITH_MOU_GUARD";
            if (v.Contains("INFORMATIONAL")) return "INFORMATIONAL";
            if (v.Contains("MARGINAL")) return "MARGINAL_PERMITTED";
            return "ALL_REQUIRED_PASS";
        }

        private async Task AddAuditAsync(long resultId, long executionId, int revision, long actorId, string eventType, string? hash, string? details)
        {
            var employeeId = await _context.UserMasters.AsNoTracking()
                .Where(u => u.ID == actorId).Select(u => u.EmployeeID).FirstOrDefaultAsync();
            var name = employeeId.HasValue
                ? await _context.EmployeeMasters.AsNoTracking()
                    .Where(e => e.ID == employeeId.Value).Select(e => e.Name).FirstOrDefaultAsync()
                : null;
            _context.UniversalResultAudits.Add(new UniversalResultAudit
            {
                UniversalTestResultID = resultId,
                TestExecutionID = executionId,
                RevisionNo = revision,
                EventType = eventType,
                ActorID = actorId,
                ActorName = name ?? $"User {actorId}",
                EventOn = DateTime.UtcNow,
                SnapshotHash = hash,
                DetailsJson = details,
                CreatedBy = actorId,
                CreatedOn = DateTime.UtcNow,
                CompanyCode = "LIMS",
                IsActive = true
            });
        }

        private async Task<UniversalResultDetailDto> MapDetailAsync(UniversalTestResult entity)
        {
            var exec = await _context.TestExecutions.AsNoTracking()
                .Include(e => e.ExecutionConfigSnapshot)
                .Include(e => e.UniversalTestGroup).ThenInclude(u => u.LaboratoryTest)
                .Include(e => e.UniversalTestGroup).ThenInclude(u => u.SpecificationGrade)
                .Include(e => e.UniversalTestGroup).ThenInclude(u => u.SampleTestPlan).ThenInclude(p => p.SampleDetail).ThenInclude(s => s!.SampleInward)
                .FirstOrDefaultAsync(e => e.ID == entity.TestExecutionID);

            TestExecutionConfigSnapshotDto? snapDto = null;
            if (exec?.ExecutionConfigSnapshot != null && !string.IsNullOrWhiteSpace(exec.ExecutionConfigSnapshot.ConfigJson))
            {
                try { snapDto = JsonSerializer.Deserialize<TestExecutionConfigSnapshotDto>(exec.ExecutionConfigSnapshot.ConfigJson, JsonOptions); } catch { }
            }

            var branchName = await _context.Branches.AsNoTracking()
                .Where(b => b.ID == entity.BranchID).Select(b => b.Name).FirstOrDefaultAsync();

            return new UniversalResultDetailDto
            {
                ID = entity.ID,
                TestExecutionID = entity.TestExecutionID,
                UniversalTestGroupID = entity.UniversalTestGroupID,
                BranchID = entity.BranchID,
                BranchName = branchName,
                OrganizationID = entity.OrganizationID,
                RevisionNo = entity.RevisionNo,
                ResultStatus = entity.ResultStatus,
                OverallDecision = entity.OverallDecision,
                SnapshotHash = entity.SnapshotHash,
                ExecutionConfigSnapshotID = entity.ExecutionConfigSnapshotID,
                DecisionRule = entity.DecisionRule,
                AcceptanceCriteriaCode = entity.AcceptanceCriteriaCode,
                ConcurrencyToken = entity.ConcurrencyToken,
                FinalizedBy = entity.FinalizedBy,
                FinalizedOn = entity.FinalizedOn,
                ReviewerID = entity.ReviewerID,
                VerifiedBy = entity.VerifiedBy,
                VerifiedOn = entity.VerifiedOn,
                ApprovedBy = entity.ApprovedBy,
                ApprovedOn = entity.ApprovedOn,
                ReviewRemarks = entity.ReviewRemarks,
                ApprovalRemarks = entity.ApprovalRemarks,
                SampleNo = exec?.UniversalTestGroup?.SampleTestPlan?.SampleDetail?.SampleNo,
                CaseNo = exec?.UniversalTestGroup?.SampleTestPlan?.SampleDetail?.SampleInward?.CaseNo,
                TestName = exec?.UniversalTestGroup?.LaboratoryTest?.Name ?? snapDto?.TestMethodName,
                TestMethodName = snapDto?.TestMethodName ?? exec?.UniversalTestGroup?.LaboratoryTest?.Name,
                SpecificationTitle = snapDto?.SpecificationTitle,
                GradeName = snapDto?.GradeName ?? exec?.UniversalTestGroup?.SpecificationGrade?.Grade,
                ExecutionNo = exec?.ExecutionNo ?? 0,
                ExecutionStatus = exec?.Status ?? string.Empty,
                Parameters = entity.Parameters.OrderBy(p => p.DisplayOrder).Select(p =>
                {
                    decimal? margin = null;
                    string? marginText = null;
                    decimal? effLo = p.EffectiveMin ?? p.SpecMin;
                    decimal? effHi = p.EffectiveMax ?? p.SpecMax;
                    if (p.ComplianceValue.HasValue)
                    {
                        if (effLo.HasValue && effHi.HasValue)
                        {
                            if (p.ComplianceValue.Value < effLo.Value)
                            {
                                margin = p.ComplianceValue.Value - effLo.Value;
                                marginText = $"{margin:F2} below Min";
                            }
                            else if (p.ComplianceValue.Value > effHi.Value)
                            {
                                margin = p.ComplianceValue.Value - effHi.Value;
                                marginText = $"+{margin:F2} above Max";
                            }
                            else
                            {
                                var distToMin = p.ComplianceValue.Value - effLo.Value;
                                var distToMax = effHi.Value - p.ComplianceValue.Value;
                                margin = Math.Min(distToMin, distToMax);
                                marginText = distToMin <= distToMax ? $"+{distToMin:F2} above Min" : $"-{distToMax:F2} below Max";
                            }
                        }
                        else if (effLo.HasValue)
                        {
                            margin = p.ComplianceValue.Value - effLo.Value;
                            marginText = margin >= 0 ? $"+{margin:F2} above Min" : $"{margin:F2} below Min";
                        }
                        else if (effHi.HasValue)
                        {
                            margin = effHi.Value - p.ComplianceValue.Value;
                            marginText = margin >= 0 ? $"-{margin:F2} below Max" : $"+{(-margin):F2} above Max";
                        }
                    }

                    return new UniversalResultParameterDto
                    {
                        ID = p.ID,
                        ParameterMasterID = p.ParameterMasterID,
                        ParameterCode = p.ParameterCode,
                        ParameterName = p.ParameterName,
                        InputType = p.InputType,
                        Unit = p.Unit,
                        DecimalPrecision = p.DecimalPrecision,
                        IsCalculated = p.IsCalculated,
                        IsMandatory = p.IsMandatory,
                        IsReportable = p.IsReportable,
                        DisplayOrder = p.DisplayOrder,
                        RawValue = p.RawValue,
                        RawNumericValue = p.RawNumericValue,
                        AppliedFactorCode = p.AppliedFactorCode,
                        AppliedFactorOperation = p.AppliedFactorOperation,
                        FactoredValue = p.FactoredValue,
                        Formula = p.Formula,
                        SubstitutionTrace = p.SubstitutionTrace,
                        CalculatedValue = p.CalculatedValue,
                        ComplianceValue = p.ComplianceValue,
                        DisplayValue = p.DisplayValue,
                        ReportedValue = p.ReportedValue,
                        SpecMin = p.SpecMin,
                        SpecMax = p.SpecMax,
                        SpecTarget = p.SpecTarget,
                        MinTolerance = p.MinTolerance,
                        MaxTolerance = p.MaxTolerance,
                        EffectiveMin = p.EffectiveMin,
                        EffectiveMax = p.EffectiveMax,
                        ToleranceSource = p.ToleranceSource,
                        ToleranceType = p.ToleranceType,
                        AppliedTolerance = p.AppliedTolerance,
                        ComplianceValueSource = p.ComplianceValueSource,
                        RequirementStatus = p.RequirementStatus,
                        Verdict = p.Verdict,
                        CombinedUncertainty = p.CombinedUncertainty,
                        ExpandedUncertainty = p.ExpandedUncertainty,
                        CoverageFactor = p.CoverageFactor,
                        MUSource = p.MUSource,
                        GuardBandApplied = p.GuardBandApplied,
                        EvaluationNote = p.EvaluationNote,
                        ComplianceMargin = margin,
                        MarginText = marginText
                    };
                }).ToList(),
                Findings = entity.Findings.Where(f => f.IsActive).Select(f => new UniversalReviewFindingDto
                {
                    ID = f.ID,
                    UniversalTestResultID = f.UniversalTestResultID,
                    TestExecutionID = f.TestExecutionID,
                    FindingType = f.FindingType,
                    Description = f.Description,
                    Severity = f.Severity,
                    Status = f.Status,
                    IsBlocking = f.IsBlocking,
                    Resolution = f.Resolution,
                    ResolvedBy = f.ResolvedBy,
                    ResolvedOn = f.ResolvedOn
                }).ToList(),
                Audits = entity.Audits.OrderBy(a => a.EventOn).Select(a => new UniversalResultAuditDto
                {
                    ID = a.ID,
                    EventType = a.EventType,
                    ActorID = a.ActorID,
                    ActorName = a.ActorName,
                    EventOn = a.EventOn,
                    SnapshotHash = a.SnapshotHash,
                    RevisionNo = a.RevisionNo,
                    DetailsJson = a.DetailsJson
                }).ToList(),
                CalculationTraceJson = entity.CalculationTraceJson,
                ComplianceSummaryJson = entity.ComplianceSummaryJson
            };
        }
    }
}
