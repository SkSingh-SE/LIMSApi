using System.Text.Json;
using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Helpers.Enums;
using LIMSApi.Models;
using LIMSApi.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Services
{
    public class UniversalPlanService : IUniversalPlanService
    {
        private readonly LIMSContext _context;
        private readonly IEffectiveConfigurationResolver _resolver;
        private readonly IPlanService _planService;
        private readonly IBranchContext _branchContext;
        private readonly ILogger<UniversalPlanService> _logger;

        public UniversalPlanService(
            LIMSContext context,
            IEffectiveConfigurationResolver resolver,
            IPlanService planService,
            IBranchContext branchContext,
            ILogger<UniversalPlanService> logger)
        {
            _context = context;
            _resolver = resolver;
            _planService = planService;
            _branchContext = branchContext;
            _logger = logger;
        }

        public async Task<UniversalPlanWorkspaceDto> GetPlanWorkspaceAsync(long inwardId, long sampleId)
        {
            if (sampleId <= 0)
            {
                throw new ArgumentException("SampleID is required to open Universal Plan Workspace. Implicit sample selection is disabled.");
            }

            var loggedInUser = LoggedInUserProvider.CurrentUser;
            // Screen 14 Part B: CompanyCode is resolved authoritatively from JWT via IBranchContext.
            // If JWT lacks CompanyCode, ResolveTenantContext() will fail loud when the plan is confirmed.
            string tenantCompanyCode = !string.IsNullOrWhiteSpace(loggedInUser?.CompanyCode)
                ? loggedInUser.CompanyCode
                : string.Empty;

            var inward = await _context.SampleInwards
                .Include(i => i.Customer)
                .Include(i => i.Branch)
                .Include(i => i.SampleDetails)
                    .ThenInclude(s => s.ProductMaster)
                .Include(i => i.SampleDetails)
                    .ThenInclude(s => s.Discipline)
                .Include(i => i.SampleDetails)
                    .ThenInclude(s => s.SpecificationGrade)
                        .ThenInclude(g => g!.SpecificationHeader)
                .Include(i => i.SampleDetails)
                    .ThenInclude(s => s.TestPlans)
                        .ThenInclude(tp => tp.UniversalTestGroups.Where(utg => utg.IsActive))
                            .ThenInclude(u => u.LaboratoryTest)
                .Include(i => i.SampleDetails)
                    .ThenInclude(s => s.TestPlans)
                        .ThenInclude(tp => tp.UniversalTestGroups.Where(utg => utg.IsActive))
                            .ThenInclude(u => u.TestMethodSpecification)
                .Include(i => i.SampleDetails)
                    .ThenInclude(s => s.TestPlans)
                        .ThenInclude(tp => tp.UniversalTestGroups.Where(utg => utg.IsActive))
                            .ThenInclude(u => u.TestMethodSpecificationVersion)
                .Include(i => i.SampleDetails)
                    .ThenInclude(s => s.TestPlans)
                        .ThenInclude(tp => tp.UniversalTestGroups.Where(utg => utg.IsActive))
                            .ThenInclude(u => u.SpecificationHeader)
                .Include(i => i.SampleDetails)
                    .ThenInclude(s => s.TestPlans)
                        .ThenInclude(tp => tp.UniversalTestGroups.Where(utg => utg.IsActive))
                            .ThenInclude(u => u.SpecificationVersion)
                .Include(i => i.SampleDetails)
                    .ThenInclude(s => s.TestPlans)
                        .ThenInclude(tp => tp.UniversalTestGroups.Where(utg => utg.IsActive))
                            .ThenInclude(u => u.SpecificationGrade)
                .Include(i => i.SampleDetails)
                    .ThenInclude(s => s.TestPlans)
                        .ThenInclude(tp => tp.UniversalTestGroups.Where(utg => utg.IsActive))
                            .ThenInclude(u => u.Branch)
                .Include(i => i.SampleDetails)
                    .ThenInclude(s => s.TestPlans)
                        .ThenInclude(tp => tp.UniversalTestGroups.Where(utg => utg.IsActive))
                            .ThenInclude(u => u.ExecutionLayout)
                .Include(i => i.SampleDetails)
                    .ThenInclude(s => s.TestPlans)
                        .ThenInclude(tp => tp.UniversalTestGroups.Where(utg => utg.IsActive))
                            .ThenInclude(u => u.Department)
                .Include(i => i.SampleDetails)
                    .ThenInclude(s => s.TestPlans)
                        .ThenInclude(tp => tp.UniversalTestGroups.Where(utg => utg.IsActive))
                            .ThenInclude(u => u.TestExecutions)
                .FirstOrDefaultAsync(i => i.ID == inwardId);

            if (inward == null)
            {
                throw new KeyNotFoundException($"Sample Inward case {inwardId} not found.");
            }

            // Tenant security check
            if (!string.IsNullOrEmpty(inward.CompanyCode) && inward.CompanyCode != tenantCompanyCode)
            {
                throw new UnauthorizedAccessException("Access denied. Case does not belong to your organization.");
            }

            var activeSample = inward.SampleDetails.FirstOrDefault(s => s.ID == sampleId);
            if (activeSample == null)
            {
                throw new KeyNotFoundException($"Sample {sampleId} not found in Inward case {inwardId}.");
            }

            var plan = activeSample.TestPlans.OrderByDescending(tp => tp.Version).FirstOrDefault();
            if (plan == null)
            {
                // Ensure a base test plan exists
                plan = new SampleTestPlan
                {
                    SampleID = activeSample.ID,
                    SampleNo = activeSample.SampleNo,
                    Version = 1,
                    PlanStatus = "Draft"
                };
                _context.TestPlans.Add(plan);
                await _context.SaveChangesAsync();
            }

            var inwardDate = inward.CollectionTime;

            var workspace = new UniversalPlanWorkspaceDto
            {
                InwardID = inward.ID,
                CaseNo = inward.CaseNo,
                SampleID = activeSample.ID,
                SampleNo = activeSample.SampleNo,
                SampleDetails = activeSample.Details,
                SampleDisciplineID = activeSample.DisciplineID,
                SampleDisciplineName = activeSample.Discipline?.Name,
                InwardDate = inwardDate,
                CustomerID = inward.CustomerID,
                CustomerName = inward.Customer?.Name ?? string.Empty,
                ProductMasterID = activeSample.ProductMasterID,
                ProductMasterName = activeSample.ProductMaster?.ProductName ?? activeSample.ProductMaster?.DisplayTitle,
                SpecificationGradeID = activeSample.SpecificationGradeID,
                GradeName = activeSample.SpecificationGrade?.Grade,
                BranchID = inward.BranchID,
                BranchName = inward.Branch?.Name ?? string.Empty,
                SampleTestPlanID = plan.ID,
                PlanStatus = plan.PlanStatus,
                PlanVersion = plan.Version,
                IsPlanLocked = plan.PlanStatus == "Submitted" || plan.PlanStatus == "Approved",
                AvailableSamples = inward.SampleDetails
                    .Where(s => !s.IsCancelled)
                    .Select(s =>
                    {
                        var sPlan = s.TestPlans.OrderByDescending(tp => tp.Version).FirstOrDefault();
                        return new UniversalPlanSampleSummaryDto
                        {
                            SampleID = s.ID,
                            SampleNo = s.SampleNo,
                            SampleDetails = s.Details,
                            ProductMasterID = s.ProductMasterID,
                            ProductName = s.ProductMaster?.ProductName ?? s.ProductMaster?.DisplayTitle,
                            SpecificationGradeID = s.SpecificationGradeID,
                            GradeName = s.SpecificationGrade?.Grade,
                            SampleDisciplineID = s.DisciplineID,
                            SampleDisciplineName = s.Discipline?.Name,
                            Quantity = s.Quantity,
                            PlanStatus = sPlan?.PlanStatus ?? "Draft",
                            PlannedTestCount = sPlan?.UniversalTestGroups.Count(u => u.IsActive && u.Status != "Cancelled") ?? 0,
                            IsSelected = s.ID == activeSample.ID
                        };
                    }).ToList()
            };

            // Available Branches
            workspace.AvailableBranches = await _context.Branches
                .Where(b => b.IsActive)
                .Select(b => new BranchOptionDto
                {
                    ID = b.ID,
                    Name = b.Name,
                    Code = b.Code
                })
                .ToListAsync();

            // Resolve Specification Header from Grade
            long? specHeaderId = activeSample.SpecificationGrade?.SpecificationHeaderID;
            if (specHeaderId.HasValue)
            {
                var specHeader = await _context.SpecificationHeaders.FirstOrDefaultAsync(h => h.ID == specHeaderId.Value);
                if (specHeader != null)
                {
                    workspace.SpecificationHeaderID = specHeader.ID;
                    workspace.SpecificationName = specHeader.DisplayTitle ?? specHeader.AliasName ?? specHeader.Code;
                    workspace.StandardReference = specHeader.StandardReference;

                    // Fetch Available Specification Versions (both Active & Superseded for explicit selection)
                    var specVersions = await _context.SpecificationVersions
                        .Where(v => v.SpecificationHeaderID == specHeader.ID)
                        .OrderByDescending(v => v.IsDefault)
                        .ThenByDescending(v => v.ID)
                        .ToListAsync();

                    workspace.AvailableSpecVersions = specVersions.Select(v => new SpecificationVersionOptionDto
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

                    // Auto-resolve active version on reference date
                    var activeCandidate = specVersions
                        .Where(v => v.Status == VersionStatus.Active &&
                                    (v.EffectiveDate == null || v.EffectiveDate <= inwardDate) &&
                                    (v.SupersededDate == null || v.SupersededDate > inwardDate))
                        .OrderByDescending(v => v.IsDefault)
                        .ThenByDescending(v => v.ID)
                        .FirstOrDefault();

                    if (activeCandidate != null)
                    {
                        workspace.SpecificationVersionID = activeCandidate.ID;
                        workspace.SpecificationVersionName = activeCandidate.Version;
                        workspace.IsSupersededSpecVersion = false;
                    }
                    else if (specVersions.Any())
                    {
                        // Fallback to default or latest
                        var fallback = specVersions.FirstOrDefault(v => v.IsDefault) ?? specVersions.First();
                        workspace.SpecificationVersionID = fallback.ID;
                        workspace.SpecificationVersionName = fallback.Version;
                        workspace.IsSupersededSpecVersion = fallback.Status == VersionStatus.Superseded;
                    }
                }
            }

            // Available Screen 13 Universal Tests (Direct mappings only - Isolated from legacy)
            var universalTests = await _context.LaboratoryTests
                .Include(t => t.Discipline)
                .Include(t => t.LabDepartment)
                .Include(t => t.Parameters.Where(p => p.IsActive))
                .Include(t => t.Methods.Where(m => m.IsActive))
                .Include(t => t.Conditions.Where(c => c.IsActive))
                .Where(t => t.IsActive)
                .OrderBy(t => t.Name)
                .ToListAsync();

            workspace.AvailableTests = universalTests.Select(t => new UniversalTestCardDto
            {
                ID = t.ID,
                Code = t.Code,
                Name = t.Name,
                DisciplineID = t.DisciplineID,
                DisciplineName = t.Discipline?.Name,
                LabDepartmentID = t.LabDepartmentID,
                DepartmentName = t.LabDepartment?.Name,
                ParameterCount = t.Parameters.Count(p => p.IsActive),
                MethodCount = t.Methods.Count(m => m.IsActive),
                ConditionCount = t.Conditions.Count(c => c.IsActive),
                IsReady = t.Parameters.Any(p => p.IsActive) && t.Methods.Any(m => m.IsActive),
                ReadinessMessage = t.Parameters.Any(p => p.IsActive) && t.Methods.Any(m => m.IsActive) ? "READY" : "INCOMPLETE"
            }).ToList();

            var activeDepartments = await _context.DepartmentMasters
                .Where(d => d.IsActive)
                .ToListAsync();

            // Planned Tests from UniversalTestGroup
            workspace.PlannedTests = plan.UniversalTestGroups
                .Where(u => u.IsActive)
                .Select(u =>
                {
                    var resolvedDept = activeDepartments
                        .FirstOrDefault(d => d.BranchID == u.BranchID && u.LaboratoryTest?.DisciplineID != null && d.DisciplineID == u.LaboratoryTest.DisciplineID)
                        ?? u.LaboratoryTest?.LabDepartment;

                    return new PlannedUniversalTestDto
                    {
                        UniversalTestGroupID = u.ID,
                        LaboratoryTestID = u.LaboratoryTestID,
                        LaboratoryTestCode = u.LaboratoryTest?.Code ?? string.Empty,
                        LaboratoryTestName = u.LaboratoryTest?.Name ?? "Universal Test",
                        DisciplineName = u.LaboratoryTest?.Discipline?.Name,
                        TestMethodSpecificationID = u.TestMethodSpecificationID,
                        TestMethodName = u.TestMethodSpecification?.Name,
                        TestMethodSpecificationVersionID = u.TestMethodSpecificationVersionID,
                        TestMethodVersion = u.TestMethodSpecificationVersion?.Version,
                        IsSupersededMethodVersion = u.TestMethodSpecificationVersion?.Status == VersionStatus.Superseded,
                        SpecificationHeaderID = u.SpecificationHeaderID,
                        SpecificationName = u.SpecificationHeader?.DisplayTitle ?? u.SpecificationHeader?.AliasName,
                        SpecificationVersionID = u.SpecificationVersionID,
                        SpecificationVersionName = u.SpecificationVersion?.Version,
                        SpecificationGradeID = u.SpecificationGradeID,
                        GradeName = u.SpecificationGrade?.Grade,
                        BranchID = u.BranchID,
                        BranchName = u.Branch?.Name,
                        DepartmentID = u.DepartmentID ?? resolvedDept?.ID ?? u.LaboratoryTest?.LabDepartmentID,
                        DepartmentName = u.Department?.Name ?? resolvedDept?.Name ?? u.LaboratoryTest?.LabDepartment?.Name,
                        ExecutionLayoutID = u.ExecutionLayoutID,
                        ExecutionLayoutCode = u.ExecutionLayout?.Code,
                        ExecutionLayoutName = u.ExecutionLayout?.Name,
                        RendererType = u.ExecutionLayout?.RendererType,
                        Status = u.Status,
                        TestExecutionID = u.TestExecutions.OrderByDescending(e => e.ID).Select(e => (long?)e.ID).FirstOrDefault(),
                        ExecutionStatus = u.TestExecutions.OrderByDescending(e => e.ID).Select(e => e.Status).FirstOrDefault()
                    };
                }).ToList();

            return workspace;
        }

        public async Task<List<TestMethodVersionOptionDto>> GetMethodVersionsAsync(long methodId)
        {
            var versions = await _context.TestMethodSpecificationVersions
                .Where(v => v.TestMethodSpecificationID == methodId)
                .OrderByDescending(v => v.IsDefault)
                .ThenByDescending(v => v.ID)
                .ToListAsync();

            return versions.Select(v => new TestMethodVersionOptionDto
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
        }

        public async Task<UniversalPlanPreviewResponseDto> PreviewTestConfigurationAsync(UniversalPlanPreviewRequestDto request)
        {
            return await _resolver.ResolveEffectiveConfigurationAsync(request);
        }

        // Screen 14 cascade: persist planner-selected Product/Grade identity onto the sample (Draft only).
        // Returns the effective specification header (grade's header wins; conflicts fail loud).
        private async Task<long?> ApplySampleIdentityAsync(SampleTestPlan plan, long? productMasterId, long? gradeId, long? headerId)
        {
            var sample = plan.SampleDetail
                ?? throw new InvalidOperationException($"Sample Test Plan {plan.ID} has no linked sample detail.");

            if (productMasterId.HasValue)
            {
                if (productMasterId.Value > 0)
                {
                    bool productOk = await _context.ProductMasters.AsNoTracking()
                        .AnyAsync(p => p.ID == productMasterId.Value && p.IsActive);
                    if (!productOk)
                        throw new ArgumentException($"Product Master {productMasterId.Value} not found or inactive. Select an existing active product (Screen 10).");
                    sample.ProductMasterID = productMasterId.Value;
                }
                else
                {
                    sample.ProductMasterID = null;
                }
            }

            long? effectiveHeaderId = (headerId.HasValue && headerId.Value > 0) ? headerId.Value : null;

            if (gradeId.HasValue)
            {
                if (gradeId.Value > 0)
                {
                    var grade = await _context.SpecificationGrades.AsNoTracking()
                        .FirstOrDefaultAsync(g => g.ID == gradeId.Value)
                        ?? throw new ArgumentException($"Specification Grade {gradeId.Value} not found. Select an existing grade (Screen 08/09).");
                    if (effectiveHeaderId.HasValue && effectiveHeaderId.Value != grade.SpecificationHeaderID)
                        throw new ArgumentException($"Selected grade '{grade.Grade}' does not belong to the selected specification standard. Change the grade or the standard so both agree.");
                    sample.SpecificationGradeID = grade.ID;
                    effectiveHeaderId = grade.SpecificationHeaderID;
                }
                else
                {
                    sample.SpecificationGradeID = null;
                }
            }

            if (effectiveHeaderId.HasValue)
            {
                bool headerOk = await _context.SpecificationHeaders.AsNoTracking()
                    .AnyAsync(h => h.ID == effectiveHeaderId.Value && h.IsActive);
                if (!headerOk)
                    throw new ArgumentException($"Specification Standard {effectiveHeaderId.Value} not found or inactive. Select an existing active standard (Screen 08).");
            }

            return effectiveHeaderId;
        }

        // Screen 11 lifecycle: explicit versions must belong to the header and be Active/Superseded.
        // Null version + known header auto-resolves the Active edition on the reference date (parity-safe).
        private async Task<long?> ResolvePinnedVersionIdAsync(long? explicitVersionId, long? headerId, DateTime referenceDate)
        {
            if (!headerId.HasValue || headerId.Value <= 0) return null;

            if (explicitVersionId.HasValue && explicitVersionId.Value > 0)
            {
                var pinned = await _context.SpecificationVersions.AsNoTracking()
                    .FirstOrDefaultAsync(v => v.ID == explicitVersionId.Value);
                if (pinned == null)
                    throw new ArgumentException($"Specification Version {explicitVersionId.Value} not found.");
                if (pinned.SpecificationHeaderID != headerId.Value)
                    throw new ArgumentException($"Specification version '{pinned.Version}' does not belong to the selected specification standard.");
                if (pinned.Status != VersionStatus.Active && pinned.Status != VersionStatus.Superseded)
                    throw new ArgumentException($"Specification version '{pinned.Version}' is {pinned.Status}. Only Active editions (or an explicitly acknowledged Superseded edition) can be pinned into a plan.");
                return pinned.ID;
            }

            var active = await _context.SpecificationVersions.AsNoTracking()
                .Where(v => v.SpecificationHeaderID == headerId.Value
                    && v.Status == VersionStatus.Active
                    && (v.EffectiveDate == null || v.EffectiveDate <= referenceDate)
                    && (v.SupersededDate == null || v.SupersededDate > referenceDate))
                .OrderByDescending(v => v.IsDefault)
                .ThenByDescending(v => v.ID)
                .FirstOrDefaultAsync();
            if (active != null) return active.ID;

            var fallback = await _context.SpecificationVersions.AsNoTracking()
                .Where(v => v.SpecificationHeaderID == headerId.Value
                    && (v.Status == VersionStatus.Active || v.Status == VersionStatus.Superseded))
                .OrderByDescending(v => v.IsDefault)
                .ThenByDescending(v => v.ID)
                .FirstOrDefaultAsync();
            if (fallback != null) return fallback.ID;

            throw new InvalidOperationException("Selected specification standard has no Active version on the inward date. Configure an Active edition in Screen 09 before planning.");
        }

        // Sample-linked header fallback for read-only (locked) confirmations.
        private async Task<long?> ResolveHeaderFromSampleGradeAsync(SampleTestPlan plan)
        {
            var gradeId = plan.SampleDetail?.SpecificationGradeID;
            if (gradeId.HasValue && gradeId.Value > 0)
            {
                var grade = await _context.SpecificationGrades.AsNoTracking()
                    .FirstOrDefaultAsync(g => g.ID == gradeId.Value);
                if (grade != null) return grade.SpecificationHeaderID;
            }
            return null;
        }
        private async Task<(long? GradeId, long? HeaderId, long? VersionId)> ResolveTestRoutingAsync(
            long? gradeId, long? headerId, long? versionId, long? effectiveHeaderId, DateTime referenceDate)
        {
            long? g = (gradeId.HasValue && gradeId.Value > 0) ? gradeId.Value : null;
            long? h = (headerId.HasValue && headerId.Value > 0) ? headerId.Value : effectiveHeaderId;
            if (g.HasValue)
            {
                var grade = await _context.SpecificationGrades.AsNoTracking()
                    .FirstOrDefaultAsync(x => x.ID == g.Value)
                    ?? throw new ArgumentException($"Specification Grade {g.Value} not found.");
                if (h.HasValue && h.Value != grade.SpecificationHeaderID)
                    throw new ArgumentException($"Planned test references grade '{grade.Grade}' which does not belong to its specification standard.");
                h ??= grade.SpecificationHeaderID;
            }
            long? v = await ResolvePinnedVersionIdAsync(
                (versionId.HasValue && versionId.Value > 0) ? versionId.Value : null, h, referenceDate);
            return (g, h, v);
        }

        public async Task<UniversalPlanConfirmResultDto> SaveDraftPlanAsync(UniversalPlanSaveDto dto)
        {
            var loggedInUser = LoggedInUserProvider.CurrentUser;
            // tenantCompanyCode is now resolved per-test via _branchContext.ResolveTenantContext.
            string tenantCompanyCode = !string.IsNullOrWhiteSpace(loggedInUser?.CompanyCode)
                ? loggedInUser.CompanyCode
                : string.Empty;

            var plan = await _context.TestPlans
                .Include(tp => tp.UniversalTestGroups)
                .Include(tp => tp.SampleDetail)
                    .ThenInclude(s => s!.SampleInward)
                .FirstOrDefaultAsync(tp => tp.ID == dto.SampleTestPlanID);

            if (plan == null)
            {
                throw new KeyNotFoundException($"Sample Test Plan {dto.SampleTestPlanID} not found.");
            }

            // Server-side tenant check
            var planCompanyCode = plan.SampleDetail?.SampleInward?.CompanyCode;
            if (!string.IsNullOrEmpty(planCompanyCode) && planCompanyCode != tenantCompanyCode)
            {
                throw new UnauthorizedAccessException("Access denied. Case does not belong to your organization.");
            }

            if (plan.PlanStatus != "Draft")
            {
                throw new InvalidOperationException($"Cannot edit plan {plan.ID} because it is in '{plan.PlanStatus}' status and frozen. Only 'Draft' plans can be edited through planning APIs.");
            }

            plan.PlanStatus = "Draft";

            if (plan.SampleDetail?.SampleInward != null)
            {
                plan.SampleDetail.SampleInward.InwardStatus = InwardStatus.UNDER_PLANNING.ToString();
            }

            // Screen 14 cascade: pin planner-selected identity + lifecycle-checked version.
            long? effectiveHeaderId = await ApplySampleIdentityAsync(plan, dto.ProductMasterID, dto.SpecificationGradeID, dto.SpecificationHeaderID);
            DateTime referenceDate = plan.SampleDetail?.SampleInward?.CollectionTime ?? DateTime.UtcNow;

            var createdIds = new List<long>();

            foreach (var item in dto.Tests)
            {
                UniversalTestGroup? utg = null;
                if (item.UniversalTestGroupID.HasValue && item.UniversalTestGroupID.Value > 0)
                {
                    utg = plan.UniversalTestGroups.FirstOrDefault(u => u.ID == item.UniversalTestGroupID.Value);
                }

                if (utg == null && !item.IsRetest)
                {
                    // Check if an unexecuted group already exists for this test
                    utg = plan.UniversalTestGroups.FirstOrDefault(u => u.LaboratoryTestID == item.LaboratoryTestID &&
                                                                       u.IsActive &&
                                                                       u.Status == "Pending");
                }

                var routed = await ResolveTestRoutingAsync(
                    item.SpecificationGradeID ?? dto.SpecificationGradeID,
                    item.SpecificationHeaderID,
                    item.SpecificationVersionID,
                    effectiveHeaderId, referenceDate);

                var previewReq = new UniversalPlanPreviewRequestDto
                {
                    SampleID = dto.SampleID,
                    LaboratoryTestID = item.LaboratoryTestID,
                    SpecificationHeaderID = routed.HeaderId,
                    SpecificationGradeID = routed.GradeId,
                    SpecificationVersionID = routed.VersionId,
                    TestMethodSpecificationID = item.TestMethodSpecificationID,
                    TestMethodSpecificationVersionID = item.TestMethodSpecificationVersionID,
                    BranchID = item.BranchID ?? dto.BranchID
                };
                var preview = await _resolver.ResolveEffectiveConfigurationAsync(previewReq);

                if (utg != null)
                {
                    // Update existing pending test group with fully pinned routing.
                    utg.LaboratoryTestID = item.LaboratoryTestID;
                    utg.TestMethodSpecificationID = item.TestMethodSpecificationID;
                    utg.TestMethodSpecificationVersionID = item.TestMethodSpecificationVersionID;
                    utg.SpecificationHeaderID = routed.HeaderId;
                    utg.SpecificationGradeID = routed.GradeId;
                    utg.SpecificationVersionID = routed.VersionId;
                    utg.BranchID = item.BranchID ?? dto.BranchID;
                    utg.DepartmentID = item.DepartmentID ?? preview.DepartmentID;
                    utg.ExecutionLayoutID = item.ExecutionLayoutID ?? preview.ExecutionLayoutID;
                    utg.ModifiedOn = DateTime.UtcNow;
                    utg.ModifiedBy = loggedInUser.EmployeeID;
                    createdIds.Add(utg.ID);
                }
                else
                {
                    // Create new UniversalTestGroup (or legitimate retest) with fully pinned routing.
                    long execBranchId = item.BranchID ?? dto.BranchID;

                    // Authoritative tenant resolution (Part B). Never defaults to 7 or 1.
                    TenantContext draftTenant;
                    try
                    {
                        draftTenant = _branchContext.ResolveTenantContext(execBranchId);
                    }
                    catch (InvalidOperationException ex)
                    {
                        throw new InvalidOperationException(
                            $"Cannot save draft UniversalTestGroup for LaboratoryTestID={item.LaboratoryTestID}: {ex.Message}");
                    }

                    long orgId = draftTenant.OrganizationID;
                    string companyCode = draftTenant.CompanyCode;

                    var newUtg = new UniversalTestGroup
                    {
                        SampleTestPlanID = plan.ID,
                        BranchID = execBranchId,
                        OrganizationID = orgId,
                        LaboratoryTestID = item.LaboratoryTestID,
                        TestMethodSpecificationID = item.TestMethodSpecificationID,
                        TestMethodSpecificationVersionID = item.TestMethodSpecificationVersionID,
                        SpecificationHeaderID = routed.HeaderId,
                        SpecificationGradeID = routed.GradeId,
                        SpecificationVersionID = routed.VersionId,
                        DepartmentID = item.DepartmentID ?? preview.DepartmentID,
                        ExecutionLayoutID = item.ExecutionLayoutID ?? preview.ExecutionLayoutID,
                        Status = "Pending",
                        IsActive = true,
                        CreatedOn = DateTime.UtcNow,
                        CreatedBy = loggedInUser.EmployeeID,
                        CompanyCode = companyCode
                    };
                    plan.UniversalTestGroups.Add(newUtg);
                }
            }

            await _context.SaveChangesAsync();

            // Record Plan History audit log
            await _planService.CreatePlanHistoryEntry(
                plan.ID,
                "DraftSaved",
                null,
                null,
                null,
                $"Draft plan saved with {dto.Tests.Count} universal tests."
            );

            return new UniversalPlanConfirmResultDto
            {
                Success = true,
                Message = "Draft plan saved successfully.",
                SampleTestPlanID = plan.ID,
                PlanStatus = plan.PlanStatus,
                CreatedUniversalTestGroupIDs = plan.UniversalTestGroups.Where(u => u.IsActive).Select(u => u.ID).ToList()
            };
        }

        public async Task<UniversalPlanValidationSummaryDto> ValidatePlanAsync(UniversalPlanSaveDto dto)
        {
            var summary = new UniversalPlanValidationSummaryDto();

            if (!dto.Tests.Any())
            {
                summary.TestDefinitionPass = false;
                summary.BlockingErrors.Add("At least one test definition must be selected for planning.");
                return summary;
            }

            foreach (var test in dto.Tests)
            {
                var req = new UniversalPlanPreviewRequestDto
                {
                    SampleID = dto.SampleID,
                    LaboratoryTestID = test.LaboratoryTestID,
                    SpecificationHeaderID = test.SpecificationHeaderID ?? dto.SpecificationHeaderID,
                    SpecificationGradeID = test.SpecificationGradeID ?? dto.SpecificationGradeID,
                    SpecificationVersionID = test.SpecificationVersionID ?? dto.SpecificationVersionID,
                    TestMethodSpecificationID = test.TestMethodSpecificationID,
                    TestMethodSpecificationVersionID = test.TestMethodSpecificationVersionID,
                    BranchID = test.BranchID ?? dto.BranchID
                };

                var preview = await _resolver.ResolveEffectiveConfigurationAsync(req);
                summary.SamplePass &= preview.ValidationSummary.SamplePass;
                summary.ProductGradePass &= preview.ValidationSummary.ProductGradePass;
                summary.SpecificationPass &= preview.ValidationSummary.SpecificationPass;
                summary.SpecificationVersionPass &= preview.ValidationSummary.SpecificationVersionPass;
                summary.TestDefinitionPass &= preview.ValidationSummary.TestDefinitionPass;
                summary.MandatoryParametersPass &= preview.ValidationSummary.MandatoryParametersPass;
                summary.TestMethodPass &= preview.ValidationSummary.TestMethodPass;
                summary.MethodVersionPass &= preview.ValidationSummary.MethodVersionPass;
                summary.BranchPass &= preview.ValidationSummary.BranchPass;
                summary.DepartmentRoutingPass &= preview.ValidationSummary.DepartmentRoutingPass;
                summary.RequiredConditionsPass &= preview.ValidationSummary.RequiredConditionsPass;
                summary.EquipmentPass &= preview.ValidationSummary.EquipmentPass;

                if (!preview.IsConfigurationReady)
                {
                    summary.BlockingErrors.AddRange(preview.ValidationSummary.BlockingErrors);
                }
                summary.Warnings.AddRange(preview.ValidationSummary.Warnings);
            }

            summary.BlockingErrors = summary.BlockingErrors.Distinct().ToList();
            summary.Warnings = summary.Warnings.Distinct().ToList();

            return summary;
        }

        public async Task<UniversalPlanConfirmResultDto> CreateTestGroupsAsync(UniversalPlanConfirmDto dto)
        {
            var loggedInUser = LoggedInUserProvider.CurrentUser;
            // Screen 14 Part B: tenant context must be resolved authoritatively.
            // Defer the actual ResolveTenantContext call until the BranchID is known
            // (after the plan is loaded) so we can use the operating branch.
            string tenantCompanyCode = !string.IsNullOrWhiteSpace(loggedInUser?.CompanyCode)
                ? loggedInUser.CompanyCode
                : string.Empty;

            var plan = await _context.TestPlans
                .Include(tp => tp.UniversalTestGroups)
                .Include(tp => tp.SampleDetail)
                    .ThenInclude(s => s!.SampleInward)
                .FirstOrDefaultAsync(tp => tp.ID == dto.SampleTestPlanID);

            if (plan == null)
            {
                throw new KeyNotFoundException($"Sample Test Plan {dto.SampleTestPlanID} not found.");
            }

            // Server-side tenant security
            var planCompanyCode = plan.SampleDetail?.SampleInward?.CompanyCode;
            if (!string.IsNullOrEmpty(planCompanyCode) && planCompanyCode != tenantCompanyCode)
            {
                throw new UnauthorizedAccessException("Access denied. Case does not belong to your organization.");
            }

            if (!dto.Tests.Any())
            {
                throw new InvalidOperationException("At least one test definition must be selected for planning.");
            }

            if (plan.PlanStatus != "Draft")
            {
                throw new InvalidOperationException($"Cannot confirm or modify plan {plan.ID} because it is in '{plan.PlanStatus}' status and frozen. Only 'Draft' plans can be confirmed.");
            }

            // Screen 14 cascade: planner-selected identity persists while the plan is still Draft.
            long? effectiveHeaderId = await ApplySampleIdentityAsync(plan, dto.ProductMasterID, dto.SpecificationGradeID, dto.SpecificationHeaderID);
            DateTime confirmReferenceDate = plan.SampleDetail?.SampleInward?.CollectionTime ?? DateTime.UtcNow;

            // Validate all tests before confirmation (enforcing all 12 gates)
            var resolvedPreviews = new Dictionary<UniversalPlanTestItemDto, UniversalPlanPreviewResponseDto>();
            foreach (var test in dto.Tests)
            {
                var previewReq = new UniversalPlanPreviewRequestDto
                {
                    SampleID = dto.SampleID,
                    LaboratoryTestID = test.LaboratoryTestID,
                    SpecificationHeaderID = test.SpecificationHeaderID ?? dto.SpecificationHeaderID,
                    SpecificationGradeID = test.SpecificationGradeID ?? dto.SpecificationGradeID,
                    SpecificationVersionID = test.SpecificationVersionID ?? dto.SpecificationVersionID,
                    TestMethodSpecificationID = test.TestMethodSpecificationID,
                    TestMethodSpecificationVersionID = test.TestMethodSpecificationVersionID,
                    BranchID = test.BranchID ?? dto.BranchID
                };

                var preview = await _resolver.ResolveEffectiveConfigurationAsync(previewReq);
                if (!preview.IsConfigurationReady)
                {
                    var errors = string.Join("; ", preview.ValidationSummary.BlockingErrors);
                    throw new InvalidOperationException($"Cannot confirm plan. Test '{preview.LaboratoryTestName}' has blocking errors: {errors}");
                }

                // Attach resolved version IDs if not explicitly given
                test.TestMethodSpecificationID = preview.TestMethodSpecificationID;
                test.TestMethodSpecificationVersionID = preview.TestMethodSpecificationVersionID;
                test.SpecificationHeaderID = preview.SpecificationHeaderID;
                test.SpecificationVersionID = preview.SpecificationVersionID;
                test.DepartmentID = preview.DepartmentID;
                test.ExecutionLayoutID = preview.ExecutionLayoutID;

                resolvedPreviews[test] = preview;
            }

            // Persist UniversalTestGroups with strictly pinned configuration & frozen PlannedConfigurationJson snapshot
            var createdIds = new List<long>();

            foreach (var item in dto.Tests)
            {
                var preview = resolvedPreviews[item];
                UniversalTestGroup? utg = null;
                if (item.UniversalTestGroupID.HasValue && item.UniversalTestGroupID.Value > 0)
                {
                    utg = plan.UniversalTestGroups.FirstOrDefault(u => u.ID == item.UniversalTestGroupID.Value);
                }

                if (utg == null && !item.IsRetest)
                {
                    utg = plan.UniversalTestGroups.FirstOrDefault(u => u.LaboratoryTestID == item.LaboratoryTestID &&
                                                                      u.IsActive &&
                                                                      u.Status == "Pending");
                }

                if (utg != null)
                {
                    var routed = await ResolveTestRoutingAsync(
                        item.SpecificationGradeID ?? dto.SpecificationGradeID,
                        item.SpecificationHeaderID,
                        item.SpecificationVersionID,
                        effectiveHeaderId, confirmReferenceDate);

                    utg.LaboratoryTestID = item.LaboratoryTestID;
                    utg.TestMethodSpecificationID = item.TestMethodSpecificationID;
                    utg.TestMethodSpecificationVersionID = item.TestMethodSpecificationVersionID;
                    utg.SpecificationHeaderID = routed.HeaderId;
                    utg.SpecificationGradeID = routed.GradeId;
                    utg.SpecificationVersionID = routed.VersionId;
                    utg.BranchID = item.BranchID ?? dto.BranchID;
                    utg.DepartmentID = preview.DepartmentID;
                    utg.ExecutionLayoutID = preview.ExecutionLayoutID;
                    utg.Status = "Pending";
                    utg.ModifiedOn = DateTime.UtcNow;
                    utg.ModifiedBy = loggedInUser.EmployeeID;

                    var plannedSnapshot = new PlannedConfigurationSnapshotDto
                    {
                        UniversalTestGroupID = utg.ID,
                        LaboratoryTestID = preview.LaboratoryTestID,
                        LaboratoryTestCode = preview.LaboratoryTestCode,
                        LaboratoryTestName = preview.LaboratoryTestName,
                        DisciplineID = preview.DisciplineID,
                        DisciplineName = preview.DisciplineName,
                        ExecutionLayoutID = preview.ExecutionLayoutID,
                        ExecutionLayoutCode = preview.ExecutionLayoutCode,
                        ExecutionLayoutName = preview.ExecutionLayoutName,
                        RendererType = preview.RendererType,
                        LayoutResolutionLevel = preview.LayoutResolutionLevel,
                        BranchID = preview.BranchID,
                        BranchName = preview.BranchName,
                        DepartmentID = preview.DepartmentID,
                        DepartmentName = preview.DepartmentName,
                        TestMethodSpecificationID = preview.TestMethodSpecificationID,
                        TestMethodCode = preview.TestMethodCode,
                        TestMethodName = preview.TestMethodName,
                        TestMethodStandard = preview.TestMethodStandard,
                        TestMethodSpecificationVersionID = preview.TestMethodSpecificationVersionID,
                        TestMethodVersion = preview.TestMethodVersion,
                        SpecificationHeaderID = preview.SpecificationHeaderID,
                        SpecificationTitle = preview.SpecificationTitle,
                        SpecificationGradeID = preview.SpecificationGradeID,
                        GradeName = preview.GradeName,
                        SpecificationVersionID = preview.SpecificationVersionID,
                        SpecificationVersionNumber = preview.SpecificationVersionNumber,
                        IsStandardlessTest = preview.IsStandardlessTest,
                        Parameters = preview.Parameters,
                        Conditions = preview.Conditions,
                        EquipmentRequirements = preview.Equipment,
                        Tenant = preview.Tenant,
                        FrozenAtUtc = DateTime.UtcNow
                    };
                    utg.PlannedConfigurationJson = JsonSerializer.Serialize(plannedSnapshot);

                    createdIds.Add(utg.ID);
                }
                else
                {
                    long execBranchId = item.BranchID ?? dto.BranchID;

                    // Authoritative tenant resolution (Part B). Never defaults to 7 or 1.
                    TenantContext tenantForItem;
                    try
                    {
                        tenantForItem = _branchContext.ResolveTenantContext(execBranchId);
                    }
                    catch (InvalidOperationException ex)
                    {
                        throw new InvalidOperationException(
                            $"Cannot create UniversalTestGroup for LaboratoryTestID={item.LaboratoryTestID}: {ex.Message}");
                    }

                    long orgId = tenantForItem.OrganizationID;
                    string companyCode = tenantForItem.CompanyCode;

                    var routedNew = await ResolveTestRoutingAsync(
                        item.SpecificationGradeID ?? dto.SpecificationGradeID,
                        item.SpecificationHeaderID,
                        item.SpecificationVersionID,
                        effectiveHeaderId, confirmReferenceDate);

                    var newUtg = new UniversalTestGroup
                    {
                        SampleTestPlanID = plan.ID,
                        BranchID = execBranchId,
                        OrganizationID = orgId,
                        LaboratoryTestID = item.LaboratoryTestID,
                        TestMethodSpecificationID = item.TestMethodSpecificationID,
                        TestMethodSpecificationVersionID = item.TestMethodSpecificationVersionID,
                        SpecificationHeaderID = routedNew.HeaderId,
                        SpecificationGradeID = routedNew.GradeId,
                        SpecificationVersionID = routedNew.VersionId,
                        DepartmentID = preview.DepartmentID,
                        ExecutionLayoutID = preview.ExecutionLayoutID,
                        Status = "Pending",
                        IsActive = true,
                        CreatedOn = DateTime.UtcNow,
                        CreatedBy = loggedInUser.EmployeeID,
                        CompanyCode = companyCode
                    };

                    var plannedSnapshot = new PlannedConfigurationSnapshotDto
                    {
                        UniversalTestGroupID = 0,
                        LaboratoryTestID = preview.LaboratoryTestID,
                        LaboratoryTestCode = preview.LaboratoryTestCode,
                        LaboratoryTestName = preview.LaboratoryTestName,
                        DisciplineID = preview.DisciplineID,
                        DisciplineName = preview.DisciplineName,
                        ExecutionLayoutID = preview.ExecutionLayoutID,
                        ExecutionLayoutCode = preview.ExecutionLayoutCode,
                        ExecutionLayoutName = preview.ExecutionLayoutName,
                        RendererType = preview.RendererType,
                        LayoutResolutionLevel = preview.LayoutResolutionLevel,
                        BranchID = preview.BranchID,
                        BranchName = preview.BranchName,
                        DepartmentID = preview.DepartmentID,
                        DepartmentName = preview.DepartmentName,
                        TestMethodSpecificationID = preview.TestMethodSpecificationID,
                        TestMethodCode = preview.TestMethodCode,
                        TestMethodName = preview.TestMethodName,
                        TestMethodStandard = preview.TestMethodStandard,
                        TestMethodSpecificationVersionID = preview.TestMethodSpecificationVersionID,
                        TestMethodVersion = preview.TestMethodVersion,
                        SpecificationHeaderID = preview.SpecificationHeaderID,
                        SpecificationTitle = preview.SpecificationTitle,
                        SpecificationGradeID = preview.SpecificationGradeID,
                        GradeName = preview.GradeName,
                        SpecificationVersionID = preview.SpecificationVersionID,
                        SpecificationVersionNumber = preview.SpecificationVersionNumber,
                        IsStandardlessTest = preview.IsStandardlessTest,
                        Parameters = preview.Parameters,
                        Conditions = preview.Conditions,
                        EquipmentRequirements = preview.Equipment,
                        Tenant = preview.Tenant,
                        FrozenAtUtc = DateTime.UtcNow
                    };
                    newUtg.PlannedConfigurationJson = JsonSerializer.Serialize(plannedSnapshot);

                    plan.UniversalTestGroups.Add(newUtg);
                }
            }

            plan.PlanStatus = "Submitted";

            if (plan.SampleDetail != null)
            {
                plan.SampleDetail.SampleStatus = SampleStatus.REQUEST_APPROVED.ToString();
                plan.SampleDetail.ModifiedBy = loggedInUser.EmployeeID;
                plan.SampleDetail.ModifiedOn = DateTime.UtcNow;

                if (plan.SampleDetail.SampleInward != null)
                {
                    plan.SampleDetail.SampleInward.InwardStatus = InwardStatus.IN_PROGRESS.ToString();
                    plan.SampleDetail.SampleInward.ModifiedBy = loggedInUser.EmployeeID;
                    plan.SampleDetail.SampleInward.ModifiedOn = DateTime.UtcNow;
                }
            }

            await _context.SaveChangesAsync();

            // Stamp newly generated IDs into PlannedConfigurationJson snapshots
            bool needsResave = false;
            foreach (var u in plan.UniversalTestGroups.Where(x => x.IsActive && !string.IsNullOrEmpty(x.PlannedConfigurationJson)))
            {
                if (u.PlannedConfigurationJson.Contains("\"UniversalTestGroupID\":0"))
                {
                    var snapshotObj = JsonSerializer.Deserialize<PlannedConfigurationSnapshotDto>(u.PlannedConfigurationJson);
                    if (snapshotObj != null && snapshotObj.UniversalTestGroupID == 0)
                    {
                        snapshotObj.UniversalTestGroupID = u.ID;
                        u.PlannedConfigurationJson = JsonSerializer.Serialize(snapshotObj);
                        needsResave = true;
                    }
                }
            }
            if (needsResave)
            {
                await _context.SaveChangesAsync();
            }

            // Record Plan History audit log
            await _planService.CreatePlanHistoryEntry(
                plan.ID,
                "UniversalPlanConfirmed",
                null,
                null,
                null,
                $"Universal plan confirmed with {dto.Tests.Count} planned test groups."
            );

            return new UniversalPlanConfirmResultDto
            {
                Success = true,
                Message = "Universal test groups created and plan confirmed successfully.",
                SampleTestPlanID = plan.ID,
                PlanStatus = plan.PlanStatus,
                CreatedUniversalTestGroupIDs = plan.UniversalTestGroups.Where(u => u.IsActive).Select(u => u.ID).ToList()
            };
        }

        public async Task<List<UniversalPlanSampleSummaryDto>> GetInwardSamplesOverviewAsync(long inwardId)
        {
            var inward = await _context.SampleInwards
                .Include(i => i.SampleDetails)
                    .ThenInclude(s => s.ProductMaster)
                .Include(i => i.SampleDetails)
                    .ThenInclude(s => s.Discipline)
                .Include(i => i.SampleDetails)
                    .ThenInclude(s => s.SpecificationGrade)
                .Include(i => i.SampleDetails)
                    .ThenInclude(s => s.TestPlans)
                        .ThenInclude(tp => tp.UniversalTestGroups.Where(utg => utg.IsActive))
                .FirstOrDefaultAsync(i => i.ID == inwardId);

            if (inward == null)
            {
                throw new KeyNotFoundException($"Sample Inward case {inwardId} not found.");
            }

            return inward.SampleDetails
                .Where(s => !s.IsCancelled)
                .Select(s =>
                {
                    var sPlan = s.TestPlans.OrderByDescending(tp => tp.Version).FirstOrDefault();
                    return new UniversalPlanSampleSummaryDto
                    {
                        SampleID = s.ID,
                        SampleNo = s.SampleNo,
                        SampleDetails = s.Details,
                        ProductMasterID = s.ProductMasterID,
                        ProductName = s.ProductMaster?.ProductName ?? s.ProductMaster?.DisplayTitle,
                        SpecificationGradeID = s.SpecificationGradeID,
                        GradeName = s.SpecificationGrade?.Grade,
                        SampleDisciplineID = s.DisciplineID,
                        SampleDisciplineName = s.Discipline?.Name,
                        Quantity = s.Quantity,
                        PlanStatus = sPlan?.PlanStatus ?? "Draft",
                        PlannedTestCount = sPlan?.UniversalTestGroups.Count(u => u.IsActive && u.Status != "Cancelled") ?? 0,
                        IsSelected = false
                    };
                }).ToList();
        }

        public async Task<UniversalPlanCopyResultDto> CopyPlanToSamplesAsync(UniversalPlanCopyRequestDto dto)
        {
            var loggedInUser = LoggedInUserProvider.CurrentUser;
            // tenantCompanyCode is now resolved per-target via _branchContext.ResolveTenantContext.
            string tenantCompanyCode = !string.IsNullOrWhiteSpace(loggedInUser?.CompanyCode)
                ? loggedInUser.CompanyCode
                : string.Empty;

            var sourceSample = await _context.SampleDetails
                .Include(s => s.SampleInward)
                .Include(s => s.TestPlans)
                    .ThenInclude(tp => tp.UniversalTestGroups.Where(utg => utg.IsActive && utg.Status != "Cancelled"))
                .FirstOrDefaultAsync(s => s.ID == dto.SourceSampleID);

            if (sourceSample == null)
            {
                throw new KeyNotFoundException($"Source sample {dto.SourceSampleID} not found.");
            }

            var sourcePlan = sourceSample.TestPlans.OrderByDescending(tp => tp.Version).FirstOrDefault();
            if (sourcePlan == null || !sourcePlan.UniversalTestGroups.Any())
            {
                throw new InvalidOperationException("Source sample has no active planned tests to copy.");
            }

            var activeSourceTests = sourcePlan.UniversalTestGroups.ToList();
            var result = new UniversalPlanCopyResultDto
            {
                TotalTargets = dto.TargetSampleIDs.Count
            };

            foreach (var targetSampleId in dto.TargetSampleIDs)
            {
                var targetDetail = new UniversalPlanTargetCopyDetailDto
                {
                    SampleID = targetSampleId
                };

                try
                {
                    var targetSample = await _context.SampleDetails
                        .Include(s => s.SampleInward)
                        .Include(s => s.ProductMaster)
                        .Include(s => s.SpecificationGrade)
                            .ThenInclude(g => g!.SpecificationHeader)
                        .Include(s => s.TestPlans)
                            .ThenInclude(tp => tp.UniversalTestGroups.Where(utg => utg.IsActive))
                        .FirstOrDefaultAsync(s => s.ID == targetSampleId);

                    if (targetSample == null || targetSample.IsCancelled)
                    {
                        targetDetail.Success = false;
                        targetDetail.Message = "Target sample not found or cancelled.";
                        result.Details.Add(targetDetail);
                        result.FailedCopies++;
                        continue;
                    }

                    targetDetail.SampleNo = targetSample.SampleNo;

                    // Ensure target has an active SampleTestPlan
                    var targetPlan = targetSample.TestPlans.OrderByDescending(tp => tp.Version).FirstOrDefault();
                    if (targetPlan == null)
                    {
                        targetPlan = new SampleTestPlan
                        {
                            SampleID = targetSample.ID,
                            SampleNo = targetSample.SampleNo,
                            Version = 1,
                            PlanStatus = "Draft"
                        };
                        _context.TestPlans.Add(targetPlan);
                        await _context.SaveChangesAsync();
                    }
                    else if (targetPlan.PlanStatus == "Submitted" || targetPlan.PlanStatus == "Approved")
                    {
                        targetDetail.Success = false;
                        targetDetail.Message = $"Target sample plan is locked ({targetPlan.PlanStatus}).";
                        result.Details.Add(targetDetail);
                        result.FailedCopies++;
                        continue;
                    }

                    int copiedForThisTarget = 0;
                    // Authoritative execution branch resolution (Screen 14 Part B):
                    // caller ExecutionBranchID > target inward BranchID > source inward BranchID > JWT context.
                    // NO silent fallback to a hardcoded branch id.
                    long? execBranchId = null;
                    if (dto.ExecutionBranchID.HasValue && dto.ExecutionBranchID.Value > 0)
                    {
                        execBranchId = dto.ExecutionBranchID.Value;
                    }
                    else if (targetSample.SampleInward?.BranchID is long tInwardBranch && tInwardBranch > 0)
                    {
                        execBranchId = tInwardBranch;
                    }
                    else if (sourceSample.SampleInward?.BranchID is long sInwardBranch && sInwardBranch > 0)
                    {
                        execBranchId = sInwardBranch;
                    }
                    else if (_branchContext.CurrentBranchID is long ctxBranch && ctxBranch > 0)
                    {
                        execBranchId = ctxBranch;
                    }

                    if (!execBranchId.HasValue)
                    {
                        targetDetail.Success = false;
                        targetDetail.Message = "Cannot resolve authoritative execution branch for target sample. Refusing to silently default.";
                        result.Details.Add(targetDetail);
                        result.FailedCopies++;
                        continue;
                    }

                    // Fail-loud tenant resolution (Part B). Never defaults to 7 or 1.
                    TenantContext tenant;
                    try
                    {
                        tenant = _branchContext.ResolveTenantContext(execBranchId.Value);
                    }
                    catch (InvalidOperationException ex)
                    {
                        targetDetail.Success = false;
                        targetDetail.Message = $"Tenant integrity violation: {ex.Message}";
                        result.Details.Add(targetDetail);
                        result.FailedCopies++;
                        continue;
                    }

                    long orgId = tenant.OrganizationID;
                    string companyCode = tenant.CompanyCode;

                    foreach (var srcGroup in activeSourceTests)
                    {
                        // Prevent accidental duplicate of same pending test definition
                        bool alreadyPlanned = targetPlan.UniversalTestGroups.Any(utg =>
                            utg.LaboratoryTestID == srcGroup.LaboratoryTestID &&
                            utg.IsActive &&
                            utg.Status != "Cancelled");

                        if (alreadyPlanned)
                            continue;

                        // Re-resolve target configuration using IEffectiveConfigurationResolver
                        var prevReq = new UniversalPlanPreviewRequestDto
                        {
                            SampleID = targetSample.ID,
                            LaboratoryTestID = srcGroup.LaboratoryTestID,
                            SpecificationHeaderID = targetSample.SpecificationGrade?.SpecificationHeaderID,
                            SpecificationGradeID = targetSample.SpecificationGradeID,
                            SpecificationVersionID = null, // Auto-resolve for target
                            BranchID = execBranchId.Value,
                            ReferenceDate = targetSample.SampleInward?.CollectionTime
                        };

                        var resolvedConfig = await _resolver.ResolveEffectiveConfigurationAsync(prevReq);

                        // Intent-based copy: IsRetest is ALWAYS false on target
                        var newTargetGroup = new UniversalTestGroup
                        {
                            SampleTestPlanID = targetPlan.ID,
                            LaboratoryTestID = srcGroup.LaboratoryTestID,
                            TestMethodSpecificationID = resolvedConfig.TestMethodSpecificationID ?? srcGroup.TestMethodSpecificationID,
                            TestMethodSpecificationVersionID = resolvedConfig.TestMethodSpecificationVersionID ?? srcGroup.TestMethodSpecificationVersionID,
                            SpecificationHeaderID = resolvedConfig.SpecificationHeaderID ?? targetSample.SpecificationGrade?.SpecificationHeaderID,
                            SpecificationVersionID = resolvedConfig.SpecificationVersionID > 0 ? resolvedConfig.SpecificationVersionID : (srcGroup.SpecificationVersionID.HasValue && srcGroup.SpecificationVersionID.Value > 0 ? srcGroup.SpecificationVersionID.Value : null),
                            SpecificationGradeID = targetSample.SpecificationGradeID,
                            BranchID = execBranchId!.Value,
                            OrganizationID = orgId,
                            Status = "Pending",
                            IsActive = true,
                            CreatedBy = loggedInUser.EmployeeID,
                            CreatedOn = DateTime.UtcNow,
                            CompanyCode = companyCode
                        };

                        _context.UniversalTestGroups.Add(newTargetGroup);
                        copiedForThisTarget++;
                    }

                    await _context.SaveChangesAsync();
                    targetDetail.Success = true;
                    targetDetail.CopiedTestCount = copiedForThisTarget;
                    targetDetail.Message = $"Successfully copied {copiedForThisTarget} planned tests.";
                    result.SuccessfulCopies++;
                }
                catch (Exception ex)
                {
                    targetDetail.Success = false;
                    targetDetail.Message = ex.Message;
                    result.FailedCopies++;
                }

                result.Details.Add(targetDetail);
            }

            result.Success = result.SuccessfulCopies > 0;
            result.Message = $"Plan intent copied to {result.SuccessfulCopies} of {result.TotalTargets} samples.";
            return result;
        }
    }
}
