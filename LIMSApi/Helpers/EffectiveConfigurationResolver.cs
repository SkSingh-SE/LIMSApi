using System.Text.Json;
using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers.Enums;
using LIMSApi.Models;
using LIMSApi.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Helpers
{
    public class EffectiveConfigurationResolver : IEffectiveConfigurationResolver
    {
        private readonly LIMSContext _context;
        private readonly FormulaEvaluator _formulaEvaluator;
        private readonly IBranchContext _branchContext;
        private readonly ILaboratoryTestLayoutService _layoutService;
        private readonly IExecutionLayoutService _executionLayoutService;

        public EffectiveConfigurationResolver(
            LIMSContext context,
            FormulaEvaluator formulaEvaluator,
            IBranchContext branchContext,
            ILaboratoryTestLayoutService layoutService,
            IExecutionLayoutService executionLayoutService)
        {
            _context = context;
            _formulaEvaluator = formulaEvaluator;
            _branchContext = branchContext;
            _layoutService = layoutService;
            _executionLayoutService = executionLayoutService;
        }

        public async Task<UniversalPlanPreviewResponseDto> ResolveEffectiveConfigurationAsync(UniversalPlanPreviewRequestDto request)
        {
            var response = new UniversalPlanPreviewResponseDto();
            var val = response.ValidationSummary;
            var refDate = request.ReferenceDate ?? DateTime.UtcNow;

            // 1. Sample & Product/Grade Gate
            SampleDetail? sample = null;
            if (request.SampleID > 0)
            {
                sample = await _context.SampleDetails
                    .Include(s => s.SampleInward)
                    .Include(s => s.ProductMaster)
                    .Include(s => s.SpecificationGrade)
                        .ThenInclude(g => g!.SpecificationHeader)
                    .FirstOrDefaultAsync(s => s.ID == request.SampleID);
            }

            if (sample == null)
            {
                val.SamplePass = false;
                val.SampleMessage = $"Sample with ID {request.SampleID} not found.";
                val.BlockingErrors.Add(val.SampleMessage);
            }
            else
            {
                val.SamplePass = true;
                refDate = sample.SampleInward?.CollectionTime ?? sample.SampleInward?.CreatedOn ?? refDate;
            }

            // Authoritative Specification Policy Gate:
            // Standardless execution is allowed ONLY if the Laboratory Test has an explicit authoritative STANDARDLESS policy.
            // Unknown Sample (SampleDetail.IsUnknownSample) is sample identity/context only and MUST NOT grant Standardless execution.
            // By default, ISO 17025 laboratory testing requires an authoritative specification (SPECIFICATION_REQUIRED).
            bool isExplicitlyStandardless = false;

            // Product & Grade
            long? gradeId = request.SpecificationGradeID ?? sample?.SpecificationGradeID;
            long? specHeaderId = request.SpecificationHeaderID;

            if (gradeId.HasValue && gradeId.Value > 0)
            {
                var grade = await _context.SpecificationGrades
                    .Include(g => g.SpecificationHeader)
                    .FirstOrDefaultAsync(g => g.ID == gradeId.Value);

                if (grade != null)
                {
                    response.SpecificationGradeID = grade.ID;
                    response.GradeName = grade.Grade;
                    specHeaderId ??= grade.SpecificationHeaderID;
                    val.ProductGradePass = true;
                }
                else
                {
                    val.ProductGradePass = false;
                    val.ProductGradeMessage = $"Specification Grade {gradeId.Value} not found.";
                    val.BlockingErrors.Add(val.ProductGradeMessage);
                }
            }
            else if (isExplicitlyStandardless)
            {
                val.ProductGradePass = true;
                val.ProductGradeMessage = "N/A — No Product Grade linked (Explicitly Standardless / Unknown Sample).";
            }
            else if (!specHeaderId.HasValue || specHeaderId.Value == 0)
            {
                val.ProductGradePass = false;
                val.ProductGradeMessage = "SPECIFICATION_REQUIRED: Valid specification and applicable Product Grade are required before test execution.";
                val.BlockingErrors.Add(val.ProductGradeMessage);
            }
            else
            {
                // Check if the specification has grades configured
                bool specHasGrades = await _context.SpecificationGrades.AnyAsync(g => g.SpecificationHeaderID == specHeaderId.Value);
                if (specHasGrades)
                {
                    val.ProductGradePass = false;
                    val.ProductGradeMessage = "Applicable Product Grade is required for this specification before test execution.";
                    val.BlockingErrors.Add(val.ProductGradeMessage);
                }
                else
                {
                    val.ProductGradePass = true;
                    val.ProductGradeMessage = "Specification has no grade breakdown (General / Ungraded Specification).";
                }
            }

            // Specification Header
            if (specHeaderId.HasValue && specHeaderId.Value > 0)
            {
                var header = await _context.SpecificationHeaders.FirstOrDefaultAsync(h => h.ID == specHeaderId.Value);
                if (header != null && header.IsActive)
                {
                    response.SpecificationHeaderID = header.ID;
                    response.SpecificationTitle = header.DisplayTitle ?? header.AliasName ?? header.Code;
                    val.SpecificationPass = true;
                }
                else if (header != null && !header.IsActive)
                {
                    val.SpecificationPass = false;
                    val.SpecificationMessage = $"Specification Header '{header.Code}' ({header.ID}) is inactive.";
                    val.BlockingErrors.Add(val.SpecificationMessage);
                }
                else
                {
                    val.SpecificationPass = false;
                    val.SpecificationMessage = $"Specification Header {specHeaderId.Value} not found.";
                    val.BlockingErrors.Add(val.SpecificationMessage);
                }
            }
            else if (isExplicitlyStandardless)
            {
                val.SpecificationPass = true;
                val.SpecificationMessage = "N/A — Explicitly Standardless / Unknown Sample.";
            }
            else
            {
                val.SpecificationPass = false;
                val.SpecificationMessage = "SPECIFICATION_REQUIRED: Valid specification is required before test execution.";
                val.BlockingErrors.Add(val.SpecificationMessage);
            }

            // 2. Specification Version Gate (Strict Fail Loud on zero / ambiguity; explicit superseded override permitted)
            long resolvedSpecVersionId = 0;
            if (specHeaderId.HasValue && specHeaderId.Value > 0)
            {
                var allSpecVersions = await _context.SpecificationVersions
                    .Where(v => v.SpecificationHeaderID == specHeaderId.Value)
                    .OrderByDescending(v => v.IsDefault)
                    .ThenByDescending(v => v.ID)
                    .ToListAsync();

                if (request.SpecificationVersionID.HasValue && request.SpecificationVersionID.Value > 0)
                {
                    var requestedVer = allSpecVersions.FirstOrDefault(v => v.ID == request.SpecificationVersionID.Value);
                    if (requestedVer == null)
                    {
                        val.SpecificationVersionPass = false;
                        val.SpecificationVersionMessage = $"Requested Specification Version {request.SpecificationVersionID.Value} does not belong to Specification Header {specHeaderId.Value}.";
                        val.BlockingErrors.Add(val.SpecificationVersionMessage);
                    }
                    else
                    {
                        resolvedSpecVersionId = requestedVer.ID;
                        response.SpecificationVersionID = requestedVer.ID;
                        response.SpecificationVersionNumber = requestedVer.Version;
                        response.IsSupersededSpecVersion = requestedVer.Status == VersionStatus.Superseded;
                        val.SpecificationVersionPass = true;
                    }
                }
                else
                {
                    // Auto-resolve active version on reference date
                    var activeCandidateVersions = allSpecVersions
                        .Where(v => v.Status == VersionStatus.Active &&
                                    (v.EffectiveDate == null || v.EffectiveDate <= refDate) &&
                                    (v.SupersededDate == null || v.SupersededDate > refDate))
                        .ToList();

                    if (activeCandidateVersions.Count == 0)
                    {
                        val.SpecificationVersionPass = false;
                        val.SpecificationVersionMessage = $"No active specification version is effective for reference date {refDate:yyyy-MM-dd} under Specification '{response.SpecificationTitle}'.";
                        val.BlockingErrors.Add(val.SpecificationVersionMessage);
                    }
                    else if (activeCandidateVersions.Count == 1)
                    {
                        var v = activeCandidateVersions[0];
                        resolvedSpecVersionId = v.ID;
                        response.SpecificationVersionID = v.ID;
                        response.SpecificationVersionNumber = v.Version;
                        response.IsSupersededSpecVersion = false;
                        val.SpecificationVersionPass = true;
                    }
                    else
                    {
                        var defaultVer = activeCandidateVersions.FirstOrDefault(v => v.IsDefault);
                        if (defaultVer != null && activeCandidateVersions.Count(v => v.IsDefault) == 1)
                        {
                            resolvedSpecVersionId = defaultVer.ID;
                            response.SpecificationVersionID = defaultVer.ID;
                            response.SpecificationVersionNumber = defaultVer.Version;
                            response.IsSupersededSpecVersion = false;
                            val.SpecificationVersionPass = true;
                        }
                        else
                        {
                            val.SpecificationVersionPass = false;
                            val.SpecificationVersionMessage = $"Ambiguous specification versions: {activeCandidateVersions.Count} active versions are effective for reference date {refDate:yyyy-MM-dd}. Exactly one must be designated as default, or an explicit version must be selected.";
                            val.BlockingErrors.Add(val.SpecificationVersionMessage);
                        }
                    }
                }
            }
            else if (isExplicitlyStandardless)
            {
                val.SpecificationVersionPass = true;
                val.SpecificationVersionMessage = "N/A — Specification Version not required for explicitly standardless test.";
            }
            else
            {
                val.SpecificationVersionPass = false;
                val.SpecificationVersionMessage = "SPECIFICATION_REQUIRED: Valid active specification version is required before test execution.";
                val.BlockingErrors.Add(val.SpecificationVersionMessage);
            }

            // 3. Laboratory Test Definition Gate (Screen 13 Direct Mappings Only)
            var test = await _context.LaboratoryTests
                .Include(t => t.Discipline)
                .Include(t => t.LabDepartment)
                .Include(t => t.Parameters.Where(p => p.IsActive))
                    .ThenInclude(p => p.Parameter)
                        .ThenInclude(pm => pm!.ParameterUnit)
                .Include(t => t.Methods.Where(m => m.IsActive))
                    .ThenInclude(m => m.TestMethodSpecification)
                .Include(t => t.Conditions.Where(c => c.IsActive))
                    .ThenInclude(c => c.ConditionMaster)
                        .ThenInclude(cm => cm.ParameterUnit)
                .FirstOrDefaultAsync(t => t.ID == request.LaboratoryTestID);

            if (test == null)
            {
                val.TestDefinitionPass = false;
                val.TestDefinitionMessage = $"Laboratory Test Definition {request.LaboratoryTestID} not found.";
                val.BlockingErrors.Add(val.TestDefinitionMessage);
                return response;
            }

            if (!test.IsActive)
            {
                val.TestDefinitionPass = false;
                val.TestDefinitionMessage = $"Laboratory Test '{test.Name}' is inactive.";
                val.BlockingErrors.Add(val.TestDefinitionMessage);
            }
            else
            {
                val.TestDefinitionPass = true;
            }

            response.LaboratoryTestID = test.ID;
            response.LaboratoryTestCode = test.Code;
            response.LaboratoryTestName = test.Name;
            response.DisciplineID = test.DisciplineID;
            response.DisciplineName = test.Discipline?.Name;

            // 4. Test Method & Method Version Gate (Screen 13 Direct Methods)
            var supportedMethods = test.Methods.Where(m => m.IsActive).ToList();
            LaboratoryTestMethod? selectedMethodMapping = null;

            if (request.TestMethodSpecificationID.HasValue && request.TestMethodSpecificationID.Value > 0)
            {
                selectedMethodMapping = supportedMethods.FirstOrDefault(m => m.TestMethodSpecificationID == request.TestMethodSpecificationID.Value);
            }
            else
            {
                selectedMethodMapping = supportedMethods.FirstOrDefault(m => m.IsDefault) ?? supportedMethods.FirstOrDefault();
            }

            if (selectedMethodMapping == null)
            {
                val.TestMethodPass = false;
                val.TestMethodMessage = $"No supported test method is configured for Laboratory Test '{test.Name}'.";
                val.BlockingErrors.Add(val.TestMethodMessage);
            }
            else
            {
                val.TestMethodPass = true;
                response.TestMethodSpecificationID = selectedMethodMapping.TestMethodSpecificationID;
                response.TestMethodCode = selectedMethodMapping.TestMethodSpecification?.Code;
                response.TestMethodName = selectedMethodMapping.TestMethodSpecification?.Name;
                response.TestMethodStandard = selectedMethodMapping.TestMethodSpecification?.TestMethodStandard;

                // Load all versions for this method (for preview dropdown)
                var allMethodVersions = await _context.TestMethodSpecificationVersions
                    .Where(v => v.TestMethodSpecificationID == selectedMethodMapping.TestMethodSpecificationID)
                    .OrderByDescending(v => v.IsDefault)
                    .ThenByDescending(v => v.ID)
                    .ToListAsync();

                response.AvailableMethodVersions = allMethodVersions.Select(v => new TestMethodVersionOptionDto
                {
                    ID = v.ID,
                    Version = v.Version,
                    Year = v.Year,
                    Status = v.Status.ToString(),
                    IsDefault = v.IsDefault,
                    IsActive = v.Status == VersionStatus.Active,
                    IsSuperseded = v.Status == VersionStatus.Superseded,
                    EffectiveDate = v.EffectiveDate,
                    SupersededDate = v.SupersededDate
                }).ToList();

                // Method Version Resolution
                if (request.TestMethodSpecificationVersionID.HasValue && request.TestMethodSpecificationVersionID.Value > 0)
                {
                    var requestedMv = allMethodVersions.FirstOrDefault(v => v.ID == request.TestMethodSpecificationVersionID.Value);
                    if (requestedMv == null)
                    {
                        val.MethodVersionPass = false;
                        val.MethodVersionMessage = $"Requested Method Version {request.TestMethodSpecificationVersionID.Value} not found for method '{response.TestMethodName}'.";
                        val.BlockingErrors.Add(val.MethodVersionMessage);
                    }
                    else
                    {
                        response.TestMethodSpecificationVersionID = requestedMv.ID;
                        response.TestMethodVersion = requestedMv.Version;
                        response.IsSupersededMethodVersion = requestedMv.Status == VersionStatus.Superseded;
                        val.MethodVersionPass = true;
                    }
                }
                else
                {
                    // Auto-resolve active method version on reference date
                    var activeMethodCandidates = allMethodVersions
                        .Where(v => v.Status == VersionStatus.Active &&
                                    (v.EffectiveDate == null || v.EffectiveDate <= refDate) &&
                                    (v.SupersededDate == null || v.SupersededDate > refDate))
                        .ToList();

                    if (activeMethodCandidates.Count == 0)
                    {
                        val.MethodVersionPass = false;
                        val.MethodVersionMessage = $"No valid active method version is effective for Test Method '{response.TestMethodName}' on reference date {refDate:yyyy-MM-dd}.";
                        val.BlockingErrors.Add(val.MethodVersionMessage);
                    }
                    else if (activeMethodCandidates.Count == 1)
                    {
                        var mv = activeMethodCandidates[0];
                        response.TestMethodSpecificationVersionID = mv.ID;
                        response.TestMethodVersion = mv.Version;
                        response.IsSupersededMethodVersion = false;
                        val.MethodVersionPass = true;
                    }
                    else
                    {
                        var defaultMv = activeMethodCandidates.FirstOrDefault(v => v.IsDefault);
                        if (defaultMv != null && activeMethodCandidates.Count(v => v.IsDefault) == 1)
                        {
                            response.TestMethodSpecificationVersionID = defaultMv.ID;
                            response.TestMethodVersion = defaultMv.Version;
                            response.IsSupersededMethodVersion = false;
                            val.MethodVersionPass = true;
                        }
                        else
                        {
                            val.MethodVersionPass = false;
                            val.MethodVersionMessage = $"Ambiguous method versions: {activeMethodCandidates.Count} active versions are effective for Test Method '{response.TestMethodName}' on reference date {refDate:yyyy-MM-dd}. Exactly one must be designated as default, or an explicit version must be selected.";
                            val.BlockingErrors.Add(val.MethodVersionMessage);
                        }
                    }
                }
            }

            // 5. Branch & Department Routing Gate (BranchID + DisciplineID -> Department)
            // Authoritative branch resolution: caller BranchID > Sample.Inward.BranchID.
            // NO silent fallback to any default branch id.
            long? resolvedBranchId = null;
            if (request.BranchID > 0)
            {
                resolvedBranchId = request.BranchID;
            }
            else if (sample?.SampleInward?.BranchID is long sampleBranchId && sampleBranchId > 0)
            {
                resolvedBranchId = sampleBranchId;
            }
            else if (_branchContext.CurrentBranchID is long ctxBranchId && ctxBranchId > 0)
            {
                resolvedBranchId = ctxBranchId;
            }

            if (!resolvedBranchId.HasValue)
            {
                val.BranchPass = false;
                val.BranchMessage = "No authoritative execution branch could be resolved from the request, sample inward, or authenticated user context.";
                val.BlockingErrors.Add(val.BranchMessage);
            }
            else
            {
                long branchId = resolvedBranchId.Value;
                var branch = await _context.Branches.FirstOrDefaultAsync(b => b.ID == branchId);

                if (branch == null || !branch.IsActive)
                {
                    val.BranchPass = false;
                    val.BranchMessage = $"Execution Branch {branchId} is invalid or inactive.";
                    val.BlockingErrors.Add(val.BranchMessage);
                }
                else
                {
                    val.BranchPass = true;
                    response.BranchID = branch.ID;
                    response.BranchName = branch.Name;

                    // Resolve tenant context authoritatively
                    try
                    {
                        var tenant = _branchContext.ResolveTenantContext(branch.ID);
                        response.Tenant = new TenantContextDto
                        {
                            OrganizationID = tenant.OrganizationID,
                            BranchID = tenant.BranchID,
                            CompanyCode = tenant.CompanyCode
                        };
                    }
                    catch (InvalidOperationException)
                    {
                        // Resolver must remain a pure configuration resolver; do not throw here.
                    }

                    // Department Resolution — Strict match on BranchID and DisciplineID only (Correction 10)
                    DepartmentMaster? dept = null;
                    if (test.DisciplineID.HasValue)
                    {
                        dept = await _context.DepartmentMasters
                            .FirstOrDefaultAsync(d => d.BranchID == branch.ID && d.DisciplineID == test.DisciplineID.Value && d.IsActive);
                    }

                    if (dept != null)
                    {
                        response.DepartmentID = dept.ID;
                        response.DepartmentName = dept.Name;
                        response.DepartmentRoutingSource = $"Resolved from Branch ({branch.Name}) + Discipline ({response.DisciplineName ?? "General"})";
                        val.DepartmentRoutingPass = true;
                    }
                    else
                    {
                        val.DepartmentRoutingPass = false;
                        val.DepartmentRoutingMessage = $"No active laboratory department found for Branch '{branch.Name}' and Discipline '{response.DisciplineName ?? "General"}'. Strict department discipline routing is required.";
                        val.BlockingErrors.Add(val.DepartmentRoutingMessage);
                    }
                }
            }

            // Layout Resolution Gate (Phase 2 -> Phase 3)
            try
            {
                var layoutRes = await _layoutService.ResolveEffectiveLayoutAsync(
                    test.ID,
                    response.TestMethodSpecificationID,
                    response.TestMethodSpecificationVersionID);

                if (layoutRes != null && layoutRes.ResolutionLevel != "Unassigned")
                {
                    response.ExecutionLayoutID = layoutRes.ExecutionLayoutID;
                    response.ExecutionLayoutCode = layoutRes.LayoutCode;
                    response.ExecutionLayoutName = layoutRes.LayoutName;
                    response.RendererType = layoutRes.RendererType;
                    response.LayoutResolutionLevel = layoutRes.ResolutionLevel;

                    if (layoutRes.ExecutionLayoutID.HasValue)
                    {
                        try
                        {
                            response.ExecutionLayout = await _executionLayoutService.GetLayoutDetails(layoutRes.ExecutionLayoutID.Value);
                        }
                        catch (Exception ex)
                        {
                            val.Warnings.Add($"Failed to load execution layout details: {ex.Message}");
                        }
                    }
                }
                else
                {
                    response.ExecutionLayoutID = null;
                    response.ExecutionLayoutCode = null;
                    response.ExecutionLayoutName = null;
                    response.RendererType = null;
                    response.LayoutResolutionLevel = "Unassigned";
                    response.ExecutionLayout = null;
                }
            }
            catch (InvalidOperationException ex)
            {
                // Ambiguous layout configuration blocks planning
                val.BlockingErrors.Add(ex.Message);
            }

            // 6. Parameters & Specification Requirements Gate
            // Screen 14 Part C: classify each parameter as RESOLVED / NOT_CONFIGURED / MANDATORY_MISSING / etc.
            // Matching is strictly by ParameterID — never by display name, code, or alias.
            List<SpecificationLine> specLines = new();
            if (resolvedSpecVersionId > 0 && gradeId.HasValue)
            {
                specLines = await _context.SpecificationLines
                    .Include(sl => sl.Parameter)
                        .ThenInclude(p => p!.ParameterUnit)
                    .Include(sl => sl.ParameterUnit)
                    .Where(sl => sl.SpecificationVersionID == resolvedSpecVersionId &&
                                 sl.SpecificationGradeID == gradeId.Value &&
                                 (sl.LaboratoryTestID == test.ID || sl.LaboratoryTestID == null))
                    .ToListAsync();
            }

            // Detect ambiguous lines (same parameter appears more than once for the resolved scope)
            var ambiguousParamIds = specLines
                .GroupBy(l => l.ParameterID)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToHashSet();

            // Sibling lines for VERSION_MISMATCH / GRADE_MISMATCH diagnostics
            var anyVersionSpecLines = new Dictionary<long, SpecificationLine>();
            var anyGradeSpecLines = new Dictionary<long, SpecificationLine>();
            if (specHeaderId.HasValue && specHeaderId.Value > 0)
            {
                var siblingLines = await _context.SpecificationLines
                    .Include(sl => sl.Parameter)
                    .Where(sl => sl.SpecificationVersion.SpecificationHeaderID == specHeaderId.Value)
                    .ToListAsync();
                foreach (var sl in siblingLines)
                {
                    if (!sl.ParameterID.HasValue) continue;
                    long pid = sl.ParameterID.Value;
                    if (!anyVersionSpecLines.ContainsKey(pid))
                    {
                        anyVersionSpecLines[pid] = sl;
                    }
                    if (gradeId.HasValue && sl.SpecificationGradeID != gradeId.Value
                        && !anyGradeSpecLines.ContainsKey(pid))
                    {
                        anyGradeSpecLines[pid] = sl;
                    }
                }
            }

            var specLinesByParamId = specLines
                .Where(l => l.ParameterID.HasValue)
                .GroupBy(l => l.ParameterID!.Value)
                .ToDictionary(g => g.Key, g => g.First());
            bool allMandatoryParamsSatisfied = true;
            bool isStandardless = isExplicitlyStandardless;
            response.IsStandardlessTest = isStandardless;

            foreach (var tp in test.Parameters.Where(p => p.IsActive).OrderBy(p => p.DisplayOrder))
            {
                var pm = tp.Parameter;
                if (pm == null) continue;

                specLinesByParamId.TryGetValue(pm.ID, out var specLine);

                string reqText = "-";
                decimal? minVal = specLine?.LowerLimitDecimalValue ?? specLine?.MinValue;
                decimal? maxVal = specLine?.UpperLimitDecimalValue ?? specLine?.MaxValue;
                decimal? minTol = specLine?.MinTolerance;
                decimal? maxTol = specLine?.MaxTolerance;
                string? formula = specLine?.Equation ?? pm.Formula;
                string? note = specLine?.Notes;
                string? acceptance = specLine?.TestCondition;

                if (minVal.HasValue && maxVal.HasValue)
                {
                    reqText = $"{minVal.Value} – {maxVal.Value}";
                }
                else if (minVal.HasValue)
                {
                    reqText = $"≥ {minVal.Value}";
                }
                else if (maxVal.HasValue)
                {
                    reqText = $"≤ {maxVal.Value}";
                }
                else if (!string.IsNullOrWhiteSpace(acceptance))
                {
                    reqText = acceptance;
                }

                // Unit authority (Screen 14 Part D): SpecLine.ParameterUnit (if set) > ParameterMaster.ParameterUnit.
                string? unit = null;
                if (specLine?.ParameterUnit != null)
                {
                    unit = !string.IsNullOrWhiteSpace(specLine.ParameterUnit.Symbol)
                        ? specLine.ParameterUnit.Symbol
                        : specLine.ParameterUnit.Name;
                }
                if (string.IsNullOrWhiteSpace(unit) && pm.ParameterUnit != null)
                {
                    unit = !string.IsNullOrWhiteSpace(pm.ParameterUnit.Symbol)
                        ? pm.ParameterUnit.Symbol
                        : pm.ParameterUnit.Name;
                }

                bool hasReq = specLine != null;
                SpecificationResolutionStatus status;
                string? reason = null;

                if (isStandardless)
                {
                    status = SpecificationResolutionStatus.SPECIFICATION_NOT_APPLICABLE;
                    reason = "Standardless test (Specification Header/Version are both NULL). Parameter requirements are N/A.";
                }
                else if (ambiguousParamIds.Contains(pm.ID))
                {
                    status = SpecificationResolutionStatus.AMBIGUOUS_CONFIGURATION;
                    reason = $"Multiple SpecificationLines reference Parameter {pm.Code} for Specification Grade {gradeId} in Version {response.SpecificationVersionNumber}.";
                    allMandatoryParamsSatisfied = false;
                }
                else if (specLine != null)
                {
                    status = SpecificationResolutionStatus.RESOLVED;
                }
                else if (anyGradeSpecLines.TryGetValue(pm.ID, out var gradeLine))
                {
                    status = SpecificationResolutionStatus.GRADE_MISMATCH;
                    reason = $"Requirement for '{pm.Code}' is configured for a different grade (GradeID={gradeLine.SpecificationGradeID}) in this specification, not for the current Grade (ID={gradeId}).";
                    if (tp.IsMandatory) allMandatoryParamsSatisfied = false;
                }
                else if (anyVersionSpecLines.TryGetValue(pm.ID, out var versionLine))
                {
                    status = SpecificationResolutionStatus.VERSION_MISMATCH;
                    reason = $"Requirement for '{pm.Code}' exists in another Specification Version (ID={versionLine.SpecificationVersionID}) of the same header, not in Version {response.SpecificationVersionNumber}.";
                    if (tp.IsMandatory) allMandatoryParamsSatisfied = false;
                }
                else if (tp.IsMandatory)
                {
                    status = SpecificationResolutionStatus.MANDATORY_MISSING;
                    reason = $"No requirement configured for {pm.Code} + Grade {response.GradeName} + Spec Version {response.SpecificationVersionNumber}. Mandatory test parameter blocks planning.";
                    allMandatoryParamsSatisfied = false;
                }
                else
                {
                    status = SpecificationResolutionStatus.NOT_CONFIGURED;
                    reason = $"No requirement configured for {pm.Code} + Grade {response.GradeName} + Spec Version {response.SpecificationVersionNumber}. Optional parameter — planning remains valid.";
                }

                if (tp.IsMandatory && !hasReq && !isStandardless && specHeaderId.HasValue && specHeaderId.Value > 0)
                {
                    val.Warnings.Add($"Mandatory parameter '{pm.Name}' has no configured requirement in Specification Version {response.SpecificationVersionNumber}.");
                }

                response.Parameters.Add(new PreviewParameterDto
                {
                    ParameterID = pm.ID,
                    ParameterCode = !string.IsNullOrWhiteSpace(pm.Code) ? pm.Code : $"PARAM_{pm.ID}",
                    ParameterName = pm.Name,
                    ParameterUnit = string.IsNullOrWhiteSpace(unit) ? null : unit,
                    InputType = pm.InputType ?? "Decimal",
                    IsMandatory = tp.IsMandatory,
                    IsReportable = tp.IsReportable,
                    RequirementText = reqText,
                    MinValue = minVal,
                    MaxValue = maxVal,
                    MinTolerance = minTol,
                    MaxTolerance = maxTol,
                    AcceptanceCriteria = acceptance,
                    Equation = formula,
                    Note = note,
                    HasRequirement = hasReq,
                    ResolutionStatus = status,
                    ResolutionReason = reason,
                    SpecificationLineID = specLine?.ID,
                    SourceSpecificationVersionID = specLine?.SpecificationVersionID,
                    Status = tp.IsMandatory ? "Required" : "Optional"
                });
            }

            if (isStandardless)
            {
                val.MandatoryParametersPass = true;
                val.MandatoryParametersMessage = "N/A — Standardless test without specification requirements.";
            }
            else
            {
                val.MandatoryParametersPass = allMandatoryParamsSatisfied;
                if (!allMandatoryParamsSatisfied)
                {
                    val.MandatoryParametersMessage = "One or more mandatory test parameters do not have configured specification requirements. See parameter ResolutionReason for details.";
                    val.BlockingErrors.Add(val.MandatoryParametersMessage);
                }
            }

            // 7. Conditions Gate
            var specLineIds = specLines.Select(s => s.ID).ToList();
            var lineConditions = await _context.SpecificationLineConditions
                .Include(c => c.ConditionMaster)
                    .ThenInclude(cm => cm.ParameterUnit)
                .Where(c => specLineIds.Contains(c.SpecificationLineID))
                .ToListAsync();

            var lineCondsByMasterId = lineConditions.ToDictionary(c => c.ConditionMasterID, c => c);
            bool allMandatoryConditionsSatisfied = true;

            foreach (var tc in test.Conditions.Where(c => c.IsActive).OrderBy(c => c.DisplayOrder))
            {
                var cm = tc.ConditionMaster;
                if (cm == null) continue;

                lineCondsByMasterId.TryGetValue(cm.ID, out var lineCond);

                string confVal = lineCond?.Value1 ?? "-";
                if (!string.IsNullOrWhiteSpace(lineCond?.Value2))
                {
                    confVal = $"{lineCond.Value1} – {lineCond.Value2}";
                }
                string unit = cm.ParameterUnit?.Symbol ?? cm.ParameterUnit?.Name ?? "";
                if (!string.IsNullOrWhiteSpace(unit) && confVal != "-")
                {
                    confVal = $"{confVal} {unit}";
                }

                bool hasConf = lineCond != null && !string.IsNullOrWhiteSpace(confVal) && confVal != "-";
                if (tc.IsMandatory && !hasConf)
                {
                    allMandatoryConditionsSatisfied = false;
                    if (specHeaderId.HasValue && specHeaderId.Value > 0)
                    {
                        val.Warnings.Add($"Mandatory condition '{cm.Name}' has no configured value in specification requirements.");
                    }
                }

                response.Conditions.Add(new PreviewConditionDto
                {
                    ConditionMasterID = cm.ID,
                    ConditionCode = !string.IsNullOrWhiteSpace(cm.Code) ? cm.Code : $"COND_{cm.ID}",
                    ConditionName = cm.Name,
                    Category = cm.Category ?? "",
                    ParameterUnit = unit,
                    IsMandatory = tc.IsMandatory,
                    ConfiguredValue = confVal,
                    HasConfiguration = hasConf,
                    Status = tc.IsMandatory ? "Required" : "Optional"
                });
            }

            if (!specHeaderId.HasValue || specHeaderId.Value <= 0)
            {
                val.RequiredConditionsPass = true;
                val.RequiredConditionsMessage = "N/A — Standardless test without specification conditions.";
            }
            else
            {
                val.RequiredConditionsPass = allMandatoryConditionsSatisfied;
                if (!allMandatoryConditionsSatisfied)
                {
                    val.RequiredConditionsMessage = "One or more mandatory test conditions do not have configured values.";
                    val.BlockingErrors.Add(val.RequiredConditionsMessage);
                }
            }

            // 8. Equipment Requirements & Calibration Gate (Gate 12)
            string tenantCo = response.Tenant?.CompanyCode ?? string.Empty;
            var activeRequirements = await _context.EquipmentRequirementMasters
                .Include(r => r.EquipmentType)
                .AsNoTracking()
                .Where(r => r.IsActive
                    && (string.IsNullOrEmpty(tenantCo) || r.CompanyCode == tenantCo)
                    && r.LaboratoryTestID == test.ID
                    && (r.TestMethodSpecificationID == null || r.TestMethodSpecificationID == response.TestMethodSpecificationID))
                .OrderBy(r => r.DisplayOrder).ThenBy(r => r.Code)
                .ToListAsync();

            if (activeRequirements.Any())
            {
                var branchEquipments = await _context.EquipmentMasters
                    .Include(e => e.Calibrations)
                    .Include(e => e.EquipmentType)
                    .AsNoTracking()
                    .Where(e => e.IsActive && e.BranchID == response.BranchID && (string.IsNullOrEmpty(tenantCo) || e.CompanyCode == tenantCo))
                    .ToListAsync();

                bool allMandatoryEquipValid = true;
                var now = DateTime.UtcNow;

                foreach (var req in activeRequirements)
                {
                    var matchingEqList = branchEquipments
                        .Where(e => e.EquipmentTypeID == req.EquipmentTypeID)
                        .ToList();

                    if (req.EquipmentID.HasValue)
                    {
                        matchingEqList = matchingEqList.Where(e => e.ID == req.EquipmentID.Value).ToList();
                    }

                    Models.EquipmentMaster? validSelected = null;
                    string bestStatus = "Not Configured";
                    DateTime? bestDue = null;
                    string? blockReason = null;

                    var evaluatedEqList = new List<(Models.EquipmentMaster Eq, string CalStatus, DateTime? DueDate, bool IsValid, string? Reason)>();

                    foreach (var eq in matchingEqList)
                    {
                        var latestCal = eq.Calibrations?.OrderByDescending(c => c.CalibrationDate).FirstOrDefault();
                        var dueDate = latestCal?.CalibrationDueDate ?? eq.NextCalibrationDueDate;
                        string calStatus;
                        bool isValid = true;
                        string? reason = null;

                        if (!eq.CalibrationRequired)
                        {
                            calStatus = "Not Required";
                        }
                        else if (!dueDate.HasValue)
                        {
                            calStatus = "Not Configured";
                            isValid = false;
                            reason = "Calibration validity is not configured for this equipment.";
                        }
                        else if (dueDate.Value < now)
                        {
                            calStatus = "Expired";
                            isValid = false;
                            reason = $"Calibration expired on {dueDate.Value:dd MMM yyyy}.";
                        }
                        else
                        {
                            calStatus = "Valid";
                        }

                        evaluatedEqList.Add((eq, calStatus, dueDate, isValid, reason));
                    }

                    var validCandidate = evaluatedEqList.FirstOrDefault(x => x.IsValid);
                    if (validCandidate.Eq != null)
                    {
                        validSelected = validCandidate.Eq;
                        bestStatus = validCandidate.CalStatus;
                        bestDue = validCandidate.DueDate;
                    }
                    else if (evaluatedEqList.Any())
                    {
                        var first = evaluatedEqList[0];
                        validSelected = first.Eq;
                        bestStatus = first.CalStatus;
                        bestDue = first.DueDate;
                        blockReason = first.Reason;
                    }
                    else
                    {
                        bestStatus = "Not Configured";
                        blockReason = $"No active equipment of type '{req.EquipmentType?.Name ?? req.Code}' found in Branch '{response.BranchName}'.";
                    }

                    bool reqPassed = validCandidate.Eq != null;
                    if (req.IsMandatory && !reqPassed)
                    {
                        allMandatoryEquipValid = false;
                    }

                    response.Equipment.Add(new PreviewEquipmentDto
                    {
                        EquipmentRequirementMasterID = req.ID,
                        RequirementName = req.Name,
                        EquipmentTypeID = req.EquipmentTypeID,
                        EquipmentTypeName = req.EquipmentType?.Name,
                        IsMandatory = req.IsMandatory,
                        EquipmentID = validSelected?.ID,
                        EquipmentName = validSelected?.Name,
                        EquipmentCode = validSelected?.EquipmentNo,
                        SerialNumber = validSelected?.ModelNo,
                        CalibrationStatus = bestStatus,
                        CalibrationDueDate = bestDue,
                        IsValidForExecution = reqPassed,
                        Message = blockReason
                    });
                }

                val.EquipmentPass = allMandatoryEquipValid;
                if (!allMandatoryEquipValid)
                {
                    val.EquipmentMessage = "One or more mandatory equipment requirements lack valid calibrated instruments in the operating branch.";
                    val.BlockingErrors.Add(val.EquipmentMessage);
                }
            }
            else
            {
                val.EquipmentPass = true;
                val.EquipmentMessage = "N/A — No equipment requirements configured for this test.";
            }

            response.IsConfigurationReady = val.AllPassed;

            // Planning-time NABL scope coverage (same LabScopeMaster semantics as execution;
            // no observed values yet → coverage only: WithinScope / NotAccredited).
            if (!isStandardless && response.Tenant != null)
            {
                var effectiveScopeIds = await _context.LabScopeMasters
                    .Where(ls => ls.IsActive && ls.CompanyCode == response.Tenant.CompanyCode
                        && ls.LaboratoryTestID == request.LaboratoryTestID
                        && (ls.BranchID == null || ls.BranchID == response.BranchID)
                        && (ls.ValidFrom == null || refDate >= ls.ValidFrom)
                        && (ls.ValidUntil == null || refDate <= ls.ValidUntil))
                    .Select(ls => ls.ID)
                    .ToListAsync();
                HashSet<long> coveredParamIds = new();
                if (effectiveScopeIds.Any())
                {
                    coveredParamIds = (await _context.LabScopeSpecifications
                        .Where(s => effectiveScopeIds.Contains(s.LabScopeID))
                        .SelectMany(s => s.Parameters)
                        .Select(p => p.ParameterID)
                        .ToListAsync()).ToHashSet();
                }
                foreach (var pp in response.Parameters)
                    pp.ScopeStatus = coveredParamIds.Contains(pp.ParameterID) ? "WithinScope" : "NotAccredited";
            }

            // Option C Fallback: Unmapped Parameters (Screen 14 / ISO 17025 execution layout)
            if (response.ExecutionLayout != null && response.ExecutionLayout.Sections.Any())
            {
                var referencedParamIds = response.ExecutionLayout.Sections
                    .SelectMany(s => s.Items)
                    .Where(it => (string.Equals(it.ReferenceType, "ParameterMaster", StringComparison.OrdinalIgnoreCase)
                               || string.Equals(it.ReferenceType, "GraphXAxis", StringComparison.OrdinalIgnoreCase)
                               || string.Equals(it.ReferenceType, "GraphYAxis", StringComparison.OrdinalIgnoreCase)
                               || string.Equals(it.ReferenceType, "GraphSeries", StringComparison.OrdinalIgnoreCase))
                               && it.ReferenceID.HasValue)
                    .Select(it => it.ReferenceID!.Value)
                    .ToHashSet();

                response.UnmappedParameters = response.Parameters
                    .Where(p => !referencedParamIds.Contains(p.ParameterID))
                    .ToList();
            }
            else
            {
                response.UnmappedParameters = response.Parameters.ToList();
            }

            return response;
        }

        public async Task<TestExecutionConfigSnapshotDto> ResolveSnapshotDtoAsync(UniversalTestGroup utg)
        {
            var previewReq = new UniversalPlanPreviewRequestDto
            {
                SampleID = utg.SampleTestPlan?.SampleID ?? 0,
                LaboratoryTestID = utg.LaboratoryTestID,
                SpecificationHeaderID = utg.SpecificationHeaderID,
                SpecificationVersionID = utg.SpecificationVersionID,
                SpecificationGradeID = utg.SpecificationGradeID,
                TestMethodSpecificationID = utg.TestMethodSpecificationID,
                TestMethodSpecificationVersionID = utg.TestMethodSpecificationVersionID,
                BranchID = utg.BranchID,
                ReferenceDate = utg.CreatedOn != default ? utg.CreatedOn : DateTime.UtcNow
            };

            var preview = await ResolveEffectiveConfigurationAsync(previewReq);

            var snapshot = new TestExecutionConfigSnapshotDto
            {
                LaboratoryTestID = preview.LaboratoryTestID,
                LaboratoryTestName = preview.LaboratoryTestName,
                LaboratoryTestCode = preview.LaboratoryTestCode,
                TestMethodSpecificationID = preview.TestMethodSpecificationID,
                TestMethodStandard = preview.TestMethodStandard,
                TestMethodName = preview.TestMethodName,
                TestMethodVersion = preview.TestMethodVersion,
                SpecificationHeaderID = preview.SpecificationHeaderID,
                SpecificationTitle = preview.SpecificationTitle,
                SpecificationGradeID = preview.SpecificationGradeID,
                GradeName = preview.GradeName,
                SnapshotDateUtc = DateTime.UtcNow,
                DefaultAggregateType = "Average",
                AggregateScope = "Observation"
            };

            int order = 1;
            // Preload ParameterMasters for precision and dropdown options
            var paramIds = preview.Parameters.Select(pp => pp.ParameterID).ToList();
            var paramMasters = await _context.ParameterMasters
                .Include(pm => pm.ParameterUnit)
                .Include(pm => pm.DropdownOptions.Where(o => o.IsActive))
                .Where(pm => paramIds.Contains(pm.ID))
                .ToDictionaryAsync(pm => pm.ID);

            // Preload Parameter-level Measurement Uncertainty (ISO 17025 Parameter MU)
            var muTenant = preview.Tenant?.CompanyCode ?? utg.CompanyCode;
            var paramMuMasters = await _context.MeasurementUncertaintyMasters
                .Include(mu => mu.ParameterUnit)
                .AsNoTracking()
                .Where(mu => mu.IsActive
                    && (string.IsNullOrEmpty(muTenant) || mu.CompanyCode == muTenant)
                    && mu.ParameterID != null
                    && paramIds.Contains(mu.ParameterID.Value)
                    && (mu.LaboratoryTestID == null || mu.LaboratoryTestID == utg.LaboratoryTestID)
                    && (mu.TestMethodSpecificationID == null || mu.TestMethodSpecificationID == preview.TestMethodSpecificationID)
                    && (mu.TestMethodSpecificationVersionID == null || mu.TestMethodSpecificationVersionID == preview.TestMethodSpecificationVersionID))
                .OrderByDescending(mu => (mu.LaboratoryTestID != null ? 1 : 0)
                    + (mu.TestMethodSpecificationID != null ? 1 : 0)
                    + (mu.TestMethodSpecificationVersionID != null ? 1 : 0))
                .ThenBy(mu => mu.DisplayOrder)
                .ThenBy(mu => mu.Code)
                .ToListAsync();

            var paramMuByParamId = paramMuMasters
                .GroupBy(mu => mu.ParameterID!.Value)
                .ToDictionary(g => g.Key, g => g.First());

            // Preload ToleranceMasters for parameters lacking explicit SpecificationLine tolerance
            var candidateToleranceMasters = await _context.ToleranceMasters
                .AsNoTracking()
                .Where(t => t.IsActive
                    && (string.IsNullOrEmpty(muTenant) || t.CompanyCode == muTenant)
                    && t.ParameterID != null
                    && paramIds.Contains(t.ParameterID.Value)
                    && (t.SpecificationHeaderID == null || t.SpecificationHeaderID == preview.SpecificationHeaderID))
                .ToListAsync();

            foreach (var p in preview.Parameters)
            {
                paramMasters.TryGetValue(p.ParameterID, out var pmMaster);
                var dropdownOpts = pmMaster?.DropdownOptions
                    .OrderBy(o => o.DisplayOrder)
                    .Select(o => new SnapshotDropdownOptionDto
                    {
                        ID = o.ID,
                        DisplayText = o.DisplayText,
                        Value = o.Value ?? o.DisplayText,
                        IsDefault = o.IsDefault,
                        DisplayOrder = o.DisplayOrder
                    }).ToList() ?? new List<SnapshotDropdownOptionDto>();

                // Deterministic Tolerance Resolution
                decimal? effMinTol = p.MinTolerance;
                decimal? effMaxTol = p.MaxTolerance;
                string tolSource = "NONE";
                string tolType = "Absolute";
                long? tolMasterId = null;
                decimal? appliedTol = null;

                if (effMinTol.HasValue || effMaxTol.HasValue)
                {
                    tolSource = "SPECIFICATION_LINE";
                    appliedTol = effMaxTol ?? effMinTol;
                }
                else
                {
                    decimal? nominal = p.MinValue.HasValue && p.MaxValue.HasValue ? (p.MinValue.Value + p.MaxValue.Value) / 2m
                        : p.MinValue ?? p.MaxValue;

                    var matchedTols = candidateToleranceMasters
                        .Where(t => t.ParameterID == p.ParameterID
                            && (!t.SpecificationHeaderID.HasValue || t.SpecificationHeaderID == preview.SpecificationHeaderID)
                            && (!t.ValueRangeStart.HasValue || !nominal.HasValue || nominal.Value >= t.ValueRangeStart.Value)
                            && (!t.ValueRangeEnd.HasValue || !nominal.HasValue || nominal.Value <= t.ValueRangeEnd.Value))
                        .OrderByDescending(t => t.SpecificationHeaderID.HasValue ? 2 : 1)
                        .ToList();

                    var topGroup = matchedTols
                        .GroupBy(t => t.SpecificationHeaderID.HasValue ? 2 : 1)
                        .OrderByDescending(g => g.Key)
                        .FirstOrDefault()?.ToList();

                    if (topGroup != null && topGroup.Count > 1)
                    {
                        var distinctTols = topGroup.Select(t => (t.Tolerance, t.ToleranceType)).Distinct().Count();
                        if (distinctTols > 1)
                        {
                            throw new InvalidOperationException($"Ambiguous tolerance configuration for parameter '{p.ParameterName}' ({p.ParameterCode}): Multiple conflicting ToleranceMaster records match nominal {nominal}. Resolve tolerance master overlap before execution.");
                        }
                    }

                    var chosenTol = topGroup?.FirstOrDefault();
                    if (chosenTol != null)
                    {
                        tolSource = "TOLERANCE_MASTER";
                        tolMasterId = chosenTol.ID;
                        tolType = chosenTol.ToleranceType ?? "Absolute";
                        decimal tolVal = Math.Abs(chosenTol.Tolerance);
                        if (string.Equals(tolType, "Percentage", StringComparison.OrdinalIgnoreCase) && nominal.HasValue)
                        {
                            tolVal = Math.Abs(nominal.Value * (tolVal / 100m));
                        }
                        effMinTol = tolVal;
                        effMaxTol = tolVal;
                        appliedTol = tolVal;
                    }
                }

                // Requirement Pattern Effective Limit Computation with Sign Normalization
                decimal? effectiveMin = null;
                decimal? effectiveMax = null;

                decimal absLowerTol = effMinTol.HasValue ? Math.Abs(effMinTol.Value) : 0m;
                decimal absUpperTol = effMaxTol.HasValue ? Math.Abs(effMaxTol.Value) : 0m;

                if (p.MinValue.HasValue && p.MaxValue.HasValue)
                {
                    effectiveMin = p.MinValue.Value - absLowerTol;
                    effectiveMax = p.MaxValue.Value + absUpperTol;
                }
                else if (p.MinValue.HasValue)
                {
                    effectiveMin = p.MinValue.Value - absLowerTol;
                    effectiveMax = null;
                }
                else if (p.MaxValue.HasValue)
                {
                    effectiveMin = null;
                    effectiveMax = p.MaxValue.Value + absUpperTol;
                }

                // Parameter-Level Measurement Uncertainty (ISO 17025)
                decimal? paramCombinedUncertainty = null;
                decimal? paramExpandedUncertainty = null;
                decimal? paramCoverageFactor = null;
                long? muMasterId = null;
                string muSource = "NONE";

                if (paramMuByParamId.TryGetValue(p.ParameterID, out var pMu))
                {
                    paramCombinedUncertainty = pMu.CombinedUncertainty;
                    paramExpandedUncertainty = pMu.ExpandedUncertainty ?? (pMu.CombinedUncertainty.HasValue ? pMu.CombinedUncertainty.Value * pMu.CoverageFactor : null);
                    paramCoverageFactor = pMu.CoverageFactor;
                    muMasterId = pMu.ID;
                    muSource = "PARAMETER_MASTER";
                }

                var paramDto = new SnapshotParameterDto
                {
                    ParameterMasterID = p.ParameterID,
                    Code = p.ParameterCode,
                    Name = p.ParameterName,
                    Unit = p.ParameterUnit ?? "Unitless",
                    Symbol = pmMaster?.Symbol,
                    InputType = pmMaster?.InputType ?? p.InputType ?? "Decimal",
                    DecimalPrecision = pmMaster?.DecimalPrecision ?? 2,
                    ParameterType = pmMaster?.CalculationRole ?? (!string.IsNullOrWhiteSpace(p.Equation) ? "Calculated" : "Input"),
                    CalculationRole = pmMaster?.CalculationRole,
                    IsCalculated = !string.IsNullOrWhiteSpace(p.Equation) || pmMaster?.IsCalculated == true,
                    Formula = p.Equation ?? pmMaster?.Formula,
                    SpecMin = p.MinValue,
                    SpecMax = p.MaxValue,
                    MinTolerance = effMinTol,
                    MaxTolerance = effMaxTol,
                    EffectiveMin = effectiveMin,
                    EffectiveMax = effectiveMax,
                    ToleranceSource = tolSource,
                    ToleranceType = tolType,
                    ToleranceMasterID = tolMasterId,
                    AppliedTolerance = appliedTol,
                    ParameterCombinedUncertainty = paramCombinedUncertainty,
                    ParameterExpandedUncertainty = paramExpandedUncertainty,
                    ParameterCoverageFactor = paramCoverageFactor,
                    MeasurementUncertaintyMasterID = muMasterId,
                    MUSource = muSource,
                    AcceptanceCriteria = p.AcceptanceCriteria ?? "Within specification range",
                    DisplayOrder = order++,
                    IsRequired = p.IsMandatory,
                    IsReportable = p.IsReportable,
                    AggregateType = "Average",
                    DropdownOptions = dropdownOpts
                };

                if (!string.IsNullOrWhiteSpace(paramDto.Formula))
                {
                    try
                    {
                        paramDto.FormulaDependencies = _formulaEvaluator.ExtractTokens(paramDto.Formula).ToList();
                    }
                    catch { }
                }

                snapshot.Parameters.Add(paramDto);
            }

            foreach (var c in preview.Conditions)
            {
                snapshot.Conditions.Add(new SnapshotConditionDto
                {
                    ConditionMasterID = c.ConditionMasterID,
                    DimensionName = c.ConditionName,
                    Unit = c.ParameterUnit ?? "",
                    ConfiguredValue1 = c.ConfiguredValue,
                    IsMandatory = c.IsMandatory
                });
            }

            // Equipment & Calibration — match by DepartmentID (authoritative) ONLY.
            // Removed all name-based string matchers (Casagrande, LaboratoryTestName) per Screen 14 Part F.
            // Phase 1D: active EquipmentRequirementMasters for (test, method) attach frozen
            // requirement references to matching type rows; dept listing is preserved when
            // no requirement is configured (zero regression for unconfigured tests).
            var branchEquipment = await _context.EquipmentMasters
                .Include(e => e.Calibrations)
                .Where(e => e.IsActive && e.BranchID == utg.BranchID)
                .ToListAsync();

            List<Models.EquipmentMaster> matchedEquipment;
            if (preview.DepartmentID.HasValue && preview.DepartmentID.Value > 0)
            {
                matchedEquipment = branchEquipment
                    .Where(e => e.DepartmentID == preview.DepartmentID.Value)
                    .ToList();
            }
            else
            {
                matchedEquipment = new List<Models.EquipmentMaster>();
            }

            var activeRequirements = await _context.EquipmentRequirementMasters
                .AsNoTracking()
                .Where(r => r.IsActive
                    && r.CompanyCode == utg.CompanyCode
                    && r.LaboratoryTestID == utg.LaboratoryTestID
                    && (r.TestMethodSpecificationID == null || r.TestMethodSpecificationID == utg.TestMethodSpecificationID))
                .OrderBy(r => r.DisplayOrder).ThenBy(r => r.Code)
                .ToListAsync();

            foreach (var eq in matchedEquipment)
            {
                var latestCal = eq.Calibrations.OrderByDescending(cal => cal.CalibrationDate).FirstOrDefault();
                var req = activeRequirements.FirstOrDefault(r => r.EquipmentTypeID == eq.EquipmentTypeID);
                snapshot.Equipment.Add(new SnapshotEquipmentDto
                {
                    EquipmentID = eq.ID,
                    Name = eq.Name,
                    Model = eq.ModelNo,
                    CalibrationNo = latestCal?.Certificate ?? eq.EquipmentNo,
                    CalibratedOn = latestCal?.CalibrationDate,
                    ValidUpto = latestCal?.CalibrationDueDate,
                    EquipmentRequirementID = req?.ID,
                    RequirementCode = req?.Code,
                    IsMandatory = req?.IsMandatory ?? true
                });
            }

            // Factors & Conversions — Phase 1E: resolve from FactorConversionMasters.
            // Scope: active enterprise factors whose input parameter participates in this test
            // and whose (test, method) applicability matches (null = global). Unit conversions
            // (ParameterUnitMaster/Equivalents) and formulas (FormulaEvaluator/SpecificationLine)
            // are owned elsewhere and never duplicated here. Zero configured factors →
            // snapshot list stays empty (identical legacy behavior).
            var snapshotParamIds = snapshot.Parameters.Select(p => p.ParameterMasterID).ToList();
            if (snapshotParamIds.Any())
            {
                var activeFactors = await _context.FactorConversionMasters
                    .AsNoTracking()
                    .Where(f => f.IsActive
                        && f.CompanyCode == utg.CompanyCode
                        && snapshotParamIds.Contains(f.InputParameterID)
                        && (f.LaboratoryTestID == null || f.LaboratoryTestID == utg.LaboratoryTestID)
                        && (f.TestMethodSpecificationID == null || f.TestMethodSpecificationID == utg.TestMethodSpecificationID)
                        && (f.TestMethodSpecificationVersionID == null || f.TestMethodSpecificationVersionID == utg.TestMethodSpecificationVersionID))
                    .OrderBy(f => f.DisplayOrder).ThenBy(f => f.Code)
                    .ToListAsync();

                // Review rule: never auto-apply multiple matching factors to one parameter.
                // Most-specific-wins per input parameter: version-scoped > method-scoped
                // > test-scoped > global. Broader records coexist but lose to narrower ones.
                var winningFactors = activeFactors
                    .GroupBy(f => f.InputParameterID)
                    .Select(g => g
                        .OrderByDescending(f =>
                            f.TestMethodSpecificationVersionID != null ? 3 :
                            f.TestMethodSpecificationID != null ? 2 :
                            f.LaboratoryTestID != null ? 1 : 0)
                        .ThenBy(f => f.DisplayOrder)
                        .ThenBy(f => f.Code)
                        .First())
                    .OrderBy(f => f.DisplayOrder).ThenBy(f => f.Code)
                    .ToList();

                foreach (var fc in winningFactors)
                {
                    snapshot.Factors.Add(new SnapshotFactorDto
                    {
                        FactorType = fc.FactorType switch
                        {
                            "DIVISION" => "Division Factor",
                            "ADDITIVE_OFFSET" => "Additive Offset",
                            "SUBTRACTIVE_OFFSET" => "Subtractive Offset",
                            _ => "Multiplication Factor"
                        },
                        FactorName = fc.Name,
                        Value = fc.FactorValue,
                        AppliedOn = fc.AppliedOn,
                        Description = fc.Description,
                        FactorConversionID = fc.ID,
                        FactorCode = fc.Code,
                        InputParameterID = fc.InputParameterID,
                        OutputParameterID = fc.OutputParameterID,
                        IsMandatory = fc.IsMandatory
                    });
                }
            }

            // Measurement Uncertainty (MU) — Phase 1F: fresh master first (FK-precise,
            // CompanyCode-scoped, most-specific-wins), legacy NABL string-match as fallback.
            // Snapshot holds test/method-level configuration only; parameter-specific rows are
            // consumed per-parameter at result/report time (Phase 5/6), never in this singular slot.
            SnapshotMeasurementUncertaintyDto? frozenMu = null;
            muTenant = preview.Tenant?.CompanyCode ?? utg.CompanyCode;
            if (!string.IsNullOrWhiteSpace(muTenant))
            {
                var methodIdForMu = snapshot.TestMethodSpecificationID;
                long? versionIdForMu = null;
                var candidates = await _context.MeasurementUncertaintyMasters
                    .Include(x => x.ParameterUnit)
                    .Where(x => x.IsActive
                        && x.CompanyCode == muTenant
                        && x.ParameterID == null
                        && (x.LaboratoryTestID == null || x.LaboratoryTestID == utg.LaboratoryTestID)
                        && (x.TestMethodSpecificationID == null || x.TestMethodSpecificationID == methodIdForMu)
                        && (x.TestMethodSpecificationVersionID == null || x.TestMethodSpecificationVersionID == versionIdForMu))
                    .OrderByDescending(x => (x.LaboratoryTestID != null ? 1 : 0)
                        + (x.TestMethodSpecificationID != null ? 1 : 0)
                        + (x.TestMethodSpecificationVersionID != null ? 1 : 0))
                    .ThenBy(x => x.DisplayOrder)
                    .ThenBy(x => x.Code)
                    .ToListAsync();

                var best = candidates.FirstOrDefault();
                if (best != null)
                {
                    frozenMu = new SnapshotMeasurementUncertaintyDto
                    {
                        UncertaintyType = best.UncertaintyType == "EXPANDED" ? "Expanded Uncertainty (k=2)"
                            : best.UncertaintyType == "COMBINED" ? "Combined Uncertainty"
                            : "Standard Uncertainty",
                        Value = best.ExpandedUncertainty ?? (best.CombinedUncertainty.HasValue
                            ? Math.Round(best.CombinedUncertainty.Value * best.CoverageFactor, 6)
                            : null),
                        CoverageFactor = best.CoverageFactor,
                        Unit = best.ParameterUnit != null ? best.ParameterUnit.Symbol : null,
                        Basis = best.Basis,
                        Remarks = "As per ISO 17025",
                        MeasurementUncertaintyMasterID = best.ID,
                        MasterCode = best.Code
                    };
                }
            }

            if (frozenMu != null)
            {
                snapshot.MeasurementUncertainty = frozenMu;
            }
            else
            {
                // Legacy fallback (preserved): NABL per-study rows matched by method/test strings.
                NablMeasurementUncertainty? mu = null;
                if (!string.IsNullOrWhiteSpace(snapshot.TestMethodStandard))
                {
                    mu = await _context.NablMeasurementUncertainties
                        .Where(u => u.IsActive && u.TestMethod == snapshot.TestMethodStandard)
                        .FirstOrDefaultAsync();
                }
                if (mu == null && !string.IsNullOrWhiteSpace(snapshot.LaboratoryTestName))
                {
                    mu = await _context.NablMeasurementUncertainties
                        .Where(u => u.IsActive && u.TestParameter == snapshot.LaboratoryTestName)
                        .FirstOrDefaultAsync();
                }

                if (mu != null)
                {
                    snapshot.MeasurementUncertainty = new SnapshotMeasurementUncertaintyDto
                    {
                        UncertaintyType = mu.UncertaintyType ?? "Expanded Uncertainty (k=2)",
                        Value = mu.ExpandedUncertainty ?? 2.50m,
                        CoverageFactor = mu.CoverageFactor ?? 2.0m,
                        Unit = mu.Unit ?? "%",
                        Basis = "Type B",
                        Remarks = "As per ISO 17025",
                        MeasurementUncertaintyMasterID = null,
                        MasterCode = null
                    };
                }
                else
                {
                    // No authoritative MU configured — leave snapshot field as null
                    // so the execution engine can show "Not Configured" rather than fake values.
                    snapshot.MeasurementUncertainty = null;
                }
            }

            // Acceptance Criteria (Phase 1B precedence — behavior unchanged):
            // Master default (AcceptanceCriteriaMaster, via test configuration link in Phase 2)
            //   -> Case-specific override (SampleInward.DecisionRule, backward compatible)
            //   -> Built-in default ("All Parameters Must Pass").
            // Frozen master reference fields stay null until Phase 2 links test configuration
            // to AcceptanceCriteriaMaster; historical snapshots must keep whatever is frozen here.
            var decisionRule = utg.SampleTestPlan?.SampleDetail?.SampleInward?.DecisionRule;
            snapshot.AcceptanceCriteria = new SnapshotAcceptanceCriteriaDto
            {
                DecisionRule = !string.IsNullOrWhiteSpace(decisionRule) ? decisionRule : "All Parameters Must Pass",
                OverallDecision = "Pass if all parameters within spec range",
                RoundingRule = "Round to nearest",
                RoundingPrecision = 0.01m,
                AcceptanceCriteriaID = null,
                AcceptanceCriteriaCode = null,
                AcceptanceCriteriaName = null
            };

            // Lab Scope freeze reference (Phase 1C): record WHICH scope applied at planning time.
            // Execution-time verdict freeze belongs to Phase 5/6 — never re-resolve live scope for history.
            if (preview.Tenant != null)
            {
                var scopeRefDate = DateTime.UtcNow;
                var candidates = await _context.LabScopeMasters
                    .Where(ls => ls.IsActive && ls.CompanyCode == preview.Tenant.CompanyCode
                        && ls.LaboratoryTestID == preview.LaboratoryTestID
                        && (ls.BranchID == null || ls.BranchID == preview.BranchID)
                        && (ls.ValidFrom == null || scopeRefDate >= ls.ValidFrom)
                        && (ls.ValidUntil == null || scopeRefDate <= ls.ValidUntil))
                    .ToListAsync();
                    
                var branchSpecific = candidates.Where(ls => ls.BranchID.HasValue && ls.BranchID.Value == preview.BranchID).ToList();
                var globalSpecific = candidates.Where(ls => !ls.BranchID.HasValue).ToList();
                var effectiveCandidates = branchSpecific.Any() ? branchSpecific : globalSpecific;
                
                long? effectiveScopeId = null;
                if (effectiveCandidates.Count == 1)
                {
                    effectiveScopeId = effectiveCandidates.First().ID;
                }
                
                snapshot.Scope = new SnapshotScopeDto
                {
                    LabScopeID = effectiveScopeId,
                    ScopeStatus = effectiveScopeId.HasValue ? "Referenced" : "NoEffectiveScope",
                    CheckedOnUtc = scopeRefDate,
                    Note = effectiveCandidates.Count > 1 
                        ? "Ambiguous scopes found at planning; NoEffectiveScope applied." 
                        : "Planning-time scope reference; execution verdict freeze is Phase 5/6."
                };

                // Phase 1C: Freeze Parameter-Level Scope
                if (effectiveScopeId.HasValue)
                {
                    var specsQuery = _context.LabScopeSpecifications
                        .Include(s => s.Parameters)
                        .Where(s => s.LabScopeID == effectiveScopeId.Value);

                    if (preview.TestMethodSpecificationID.HasValue && preview.TestMethodSpecificationID.Value > 0)
                    {
                        specsQuery = specsQuery.Where(s => s.TestMethodSpecificationID == preview.TestMethodSpecificationID.Value);
                    }

                    if (preview.TestMethodSpecificationVersionID.HasValue && preview.TestMethodSpecificationVersionID.Value > 0)
                    {
                        specsQuery = specsQuery.Where(s => s.TestMethodSpecificationVersionID == preview.TestMethodSpecificationVersionID.Value || s.TestMethodSpecificationVersionID == null);
                    }

                    var sortedSpecs = await specsQuery.ToListAsync();
                    sortedSpecs = sortedSpecs.OrderByDescending(s => s.TestMethodSpecificationVersionID.HasValue).ToList();

                    foreach (var param in snapshot.Parameters)
                    {
                        foreach (var spec in sortedSpecs)
                        {
                            var scopeParam = spec.Parameters.FirstOrDefault(p => p.ParameterID == param.ParameterMasterID);
                            if (scopeParam != null)
                            {
                                param.NablScopeLowerLimit = scopeParam.LowerLimitValue ?? (decimal.TryParse(scopeParam.LowerLimit?.Trim(), out var l) ? l : null);
                                param.NablScopeUpperLimit = scopeParam.UpperLimitValue ?? (decimal.TryParse(scopeParam.UpperLimit?.Trim(), out var u) ? u : null);
                                param.IsUnderISO = scopeParam.IsUnderISO;
                                param.LabScopeSpecParamId = scopeParam.ID;
                                break;
                            }
                        }
                    }
                }
                
                // Phase 1C: Freeze Accreditation
                var cert = await _context.NablAccreditations
                    .Where(n => n.IsActive && n.CompanyCode == preview.Tenant.CompanyCode
                        && n.OrganizationId == preview.Tenant.OrganizationID
                        && (n.BranchID == null || n.BranchID == preview.BranchID))
                    .OrderByDescending(n => n.BranchID.HasValue ? 1 : 0)
                    .ThenByDescending(n => n.ExpiryDate)
                    .FirstOrDefaultAsync();

                if (cert != null)
                {
                    snapshot.Accreditation = new SnapshotAccreditationDto
                    {
                        AccreditationID = cert.Id,
                        CertificateNumber = cert.CertificateNumber,
                        ValidFrom = cert.IssueDate,
                        ValidTo = cert.ExpiryDate,
                        BranchID = cert.BranchID,
                        LogoPath = cert.LogoPath,
                        AccreditationStatus = (cert.IssueDate == default || cert.IssueDate <= scopeRefDate) &&
                                              (cert.ExpiryDate == default || cert.ExpiryDate >= scopeRefDate) 
                                              ? "Active" : "Expired"
                    };
                }
            }

            // Execution Layout Freeze (Phase B.1 / Phase B.3 — ISO 17025 layout contract)
            snapshot.ExecutionLayoutID = preview.ExecutionLayoutID;
            snapshot.ExecutionLayoutCode = preview.ExecutionLayoutCode;
            snapshot.ExecutionLayoutName = preview.ExecutionLayoutName;
            snapshot.RendererType = preview.RendererType ?? "ObservationMatrix";
            snapshot.LayoutResolutionLevel = preview.LayoutResolutionLevel;
            snapshot.ExecutionLayout = preview.ExecutionLayout;
            snapshot.UnmappedParameterIDs = preview.UnmappedParameters.Select(p => p.ParameterID).ToList();
            snapshot.SpecimenMode = "Single";
            snapshot.ObservationMode = "Multiple";
            snapshot.ConfiguredReadingCount = 1;

            // Phase C.4: Dynamic Execution Layout-Driven Compliance
            // ---------------------------------------------------
            // The execution layout structurally separates 'Observations' (intermediate trial data)
            // from final 'Parameters' (reportable outcomes). Parameters explicitly mapped in an 
            // Observation section are marked as non-reportable in the snapshot. This strictly 
            // isolates them from the final compliance table and overall decision engine without 
            // affecting their formula dependencies.
            if (snapshot.ExecutionLayout != null && snapshot.ExecutionLayout.Sections != null)
            {
                var obsSectionParamIds = snapshot.ExecutionLayout.Sections
                    .Where(s => s.SectionType == "Observations")
                    .SelectMany(s => s.Items ?? new List<Dtos.ExecutionLayoutItemDto>())
                    .Where(i => i.ReferenceType == "ParameterMaster" && i.ReferenceID.HasValue)
                    .Select(i => i.ReferenceID.Value)
                    .ToHashSet();

                if (obsSectionParamIds.Count > 0)
                {
                    foreach (var p in snapshot.Parameters)
                    {
                        if (obsSectionParamIds.Contains(p.ParameterMasterID))
                        {
                            p.IsReportable = false;
                        }
                    }
                }
            }

            return snapshot;
        }
    }
}
