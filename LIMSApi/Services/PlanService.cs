using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Services.Interface;
using LIMSApi.ServiceWORepo;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace LIMSApi.Services
{
    public class PlanService : IPlanService
    {
        private readonly LIMSContext _context;
        private readonly ILogger<PlanService> _logger;
        private readonly ITestAutoSuggestService _autoSuggestService;
        private readonly LoggedInUserDTO loggedInUser;

        public PlanService(LIMSContext context, ILogger<PlanService> logger, ITestAutoSuggestService autoSuggestService)
        {
            _context = context;
            _logger = logger;
            _autoSuggestService = autoSuggestService;
            loggedInUser = LoggedInUserProvider.CurrentUser;
        }

        public async Task<List<PlanHistory>> GetPlanHistory(long planId)
        {
            return await _context.PlanHistories
                .Where(h => h.PlanId == planId)
                .OrderByDescending(h => h.ChangedAt)
                .ToListAsync();
        }

        public async Task RequestReplan(long planId, string reason)
        {
            var plan = await _context.TestPlans.FirstOrDefaultAsync(p => p.ID == planId);
            if (plan == null)
                throw new Exception($"Plan with ID {planId} not found.");

            if (plan.PlanStatus != "Approved")
                throw new InvalidOperationException("Replan can only be requested for approved plans.");

            var replanRequest = new ReplanRequest
            {
                PlanId = planId,
                RequestedById = loggedInUser.EmployeeID,
                RequestedByName = loggedInUser.Name,
                RequestedAt = DateTime.UtcNow,
                Reason = reason,
                Status = "Pending"
            };

            _context.ReplanRequests.Add(replanRequest);

            plan.PlanStatus = "ReplanRequested";

            // Create history entry
            await CreatePlanHistoryEntry(
                planId,
                "ReplanRequested",
                null,
                null,
                null,
                $"Replan requested. Reason: {reason}"
            );

            await _context.SaveChangesAsync();
            _logger.LogInformation("Replan requested for plan {PlanId} by {UserName}", planId, loggedInUser.Name);
        }

        public async Task ApproveReplan(long requestId, string? remarks)
        {
            var request = await _context.ReplanRequests.FirstOrDefaultAsync(r => r.Id == requestId);
            if (request == null)
                throw new Exception($"Replan request with ID {requestId} not found.");

            if (request.Status != "Pending")
                throw new InvalidOperationException("Only pending replan requests can be approved.");

            var plan = await _context.TestPlans.FirstOrDefaultAsync(p => p.ID == request.PlanId);
            if (plan == null)
                throw new Exception($"Plan with ID {request.PlanId} not found.");

            // Approve the request
            request.Status = "Approved";
            request.ApprovedById = loggedInUser.EmployeeID;
            request.ApprovedByName = loggedInUser.Name;
            request.ApprovedAt = DateTime.UtcNow;
            request.ApprovalRemarks = remarks;

            // Update plan: increment version, reset status, increment replan count
            plan.Version += 1;
            plan.ReplanCount += 1;
            plan.PlanStatus = "Draft";
            plan.ApprovedById = null;
            plan.ApprovedByName = null;
            plan.ApprovedAt = null;

            // Create history entry
            await CreatePlanHistoryEntry(
                plan.ID,
                "ReplanApproved",
                null,
                null,
                JsonSerializer.Serialize(new[]
                {
                    new { field = "Version", oldValue = (plan.Version - 1).ToString(), newValue = plan.Version.ToString() },
                    new { field = "PlanStatus", oldValue = "ReplanRequested", newValue = "Draft" },
                    new { field = "ReplanCount", oldValue = (plan.ReplanCount - 1).ToString(), newValue = plan.ReplanCount.ToString() }
                }),
                $"Replan approved. {(string.IsNullOrWhiteSpace(remarks) ? "" : $"Remarks: {remarks}")}"
            );

            await _context.SaveChangesAsync();
            _logger.LogInformation("Replan approved for plan {PlanId}, new version {Version}", plan.ID, plan.Version);
        }

        public async Task RejectReplan(long requestId, string? remarks)
        {
            var request = await _context.ReplanRequests.FirstOrDefaultAsync(r => r.Id == requestId);
            if (request == null)
                throw new Exception($"Replan request with ID {requestId} not found.");

            if (request.Status != "Pending")
                throw new InvalidOperationException("Only pending replan requests can be rejected.");

            var plan = await _context.TestPlans.FirstOrDefaultAsync(p => p.ID == request.PlanId);
            if (plan == null)
                throw new Exception($"Plan with ID {request.PlanId} not found.");

            // Reject the request
            request.Status = "Rejected";
            request.ApprovedById = loggedInUser.EmployeeID;
            request.ApprovedByName = loggedInUser.Name;
            request.ApprovedAt = DateTime.UtcNow;
            request.ApprovalRemarks = remarks;

            // Restore plan status back to Approved
            plan.PlanStatus = "Approved";

            // Create history entry
            await CreatePlanHistoryEntry(
                plan.ID,
                "ReplanRejected",
                null,
                null,
                null,
                $"Replan request rejected. {(string.IsNullOrWhiteSpace(remarks) ? "" : $"Remarks: {remarks}")}"
            );

            await _context.SaveChangesAsync();
            _logger.LogInformation("Replan rejected for plan {PlanId}", plan.ID);
        }

        public async Task CreatePlanHistoryEntry(long planId, string changeType, string? previousDataJson, string? newDataJson, string? fieldChangesJson, string? remarks)
        {
            var plan = await _context.TestPlans.AsNoTracking().FirstOrDefaultAsync(p => p.ID == planId);
            if (plan == null) return;

            var history = new PlanHistory
            {
                PlanId = planId,
                Version = plan.Version,
                ChangeType = changeType,
                ChangedById = loggedInUser.EmployeeID,
                ChangedByName = loggedInUser.Name,
                ChangedAt = DateTime.UtcNow,
                PreviousDataJson = previousDataJson,
                NewDataJson = newDataJson,
                FieldChangesJson = fieldChangesJson,
                Remarks = remarks
            };

            _context.PlanHistories.Add(history);
        }

        public async Task AssignGradeAsync(AssignGradeDto dto)
        {
            var specGrade = await _context.SpecificationGrades
                .FirstOrDefaultAsync(g => g.ID == dto.SpecificationGradeID);

            if (specGrade == null)
                throw new ArgumentException($"Specification Grade with ID {dto.SpecificationGradeID} not found.");

            var specHeader = await _context.SpecificationHeaders
                .AsNoTracking()
                .FirstOrDefaultAsync(h => h.ID == specGrade.SpecificationHeaderID);

            var sample = await _context.SampleDetails
                .Include(s => s.TestPlans)
                .FirstOrDefaultAsync(s => s.ID == dto.SampleID);

            if (sample != null)
            {
                sample.AssignedGradeID = dto.SpecificationGradeID;
                sample.AssignedGradeNote = dto.Notes;
                sample.IsUnknownSample = false;
                sample.ModifiedOn = DateTime.UtcNow;

                foreach (var plan in sample.TestPlans)
                {
                    await CreatePlanHistoryEntry(
                        plan.ID,
                        "GradeAssigned",
                        "Unknown Sample",
                        $"{specGrade.Grade} (Spec: {specHeader?.DisplayTitle ?? specHeader?.AliasName ?? "Standard"})",
                        JsonSerializer.Serialize(new { AssignedGradeID = dto.SpecificationGradeID, Grade = specGrade.Grade, Notes = dto.Notes }),
                        dto.Notes ?? "Post-Testing Grade Assigned"
                    );
                }
                await _context.SaveChangesAsync();
                _logger.LogInformation("Assigned Grade {GradeID} to Sample {SampleID}", dto.SpecificationGradeID, dto.SampleID);
            }
        }

        // ────────────── 6-Tier Decision Engine Cascade Implementation ──────────────

        public async Task<object> GetProductMasterCascadeAsync(long productMasterId)
        {
            var pm = await _context.ProductMasters
                .AsNoTracking()
                .Include(p => p.ProductSizeMaster)
                .Include(p => p.MetalClassifications)
                    .ThenInclude(m => m.MetalClassification)
                .Include(p => p.Versions)
                    .ThenInclude(v => v.Grades)
                .FirstOrDefaultAsync(p => p.ID == productMasterId);

            if (pm == null)
                return new { success = false, message = "Product Master not found" };

            var primaryMetal = pm.MetalClassifications.FirstOrDefault()?.MetalClassification;
            var activeVersion = pm.Versions.FirstOrDefault(v => v.IsActiveVersion) ?? pm.Versions.FirstOrDefault();
            var primaryGrade = activeVersion?.Grades.FirstOrDefault();

            var availableSizes = await _context.ProductSizeMasters
                .AsNoTracking()
                .Where(s => s.IsActive)
                .Select(s => new { s.ID, s.DisplayName, s.SizeType, s.MinValue, s.MaxValue })
                .ToListAsync();

            return new
            {
                success = true,
                productMasterID = pm.ID,
                productName = pm.ProductName,
                displayTitle = pm.DisplayTitle,
                gradePrefix = pm.GradePrefix,
                gradeValue = pm.GradeValue,
                productSizeMasterID = pm.ProductSizeMasterID,
                productSizeDisplayName = pm.ProductSizeMaster?.DisplayName,
                metalClassificationID = primaryMetal?.ID,
                metalClassificationName = primaryMetal?.Name,
                specificationGradeID = primaryGrade?.SpecificationGradeID,
                availableSizes
            };
        }

        public async Task<object> GetProductMasterSizeLimitsAsync(long productMasterId, long sizeId)
        {
            var sizeMaster = await _context.ProductSizeMasters
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.ID == sizeId);

            var specLines = await _context.SpecificationLines
                .AsNoTracking()
                .Include(l => l.Parameter)
                .Include(l => l.ParameterUnit)
                .Where(l => l.ProductSizeMasterID == sizeId)
                .Select(l => new
                {
                    l.ID,
                    parameterID = l.ParameterID,
                    parameterName = l.Parameter != null ? l.Parameter.Name : "",
                    l.MinValue,
                    l.MaxValue,
                    unit = l.ParameterUnit != null ? l.ParameterUnit.Name : (l.Parameter != null && l.Parameter.ParameterUnit != null ? l.Parameter.ParameterUnit.Name : "")
                })
                .ToListAsync();

            return new
            {
                success = true,
                productMasterID = productMasterId,
                sizeID = sizeId,
                sizeDisplayName = sizeMaster?.DisplayName,
                parameters = specLines
            };
        }

        public async Task<object> GetMetalClassificationCascadeAsync(long metalClassificationId)
        {
            var metal = await _context.MetalClassificationMasters
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == metalClassificationId);

            var validTechniques = await _context.MetalClassificationAnalysisTechniques
                .AsNoTracking()
                .Include(t => t.AnalysisTechnique)
                .Where(t => t.MetalClassificationID == metalClassificationId && t.AnalysisTechnique != null && t.AnalysisTechnique.IsActive)
                .Select(t => new
                {
                    t.AnalysisTechnique!.ID,
                    t.AnalysisTechnique.Name,
                    t.AnalysisTechnique.Code,
                    t.AnalysisTechnique.AliasNames
                })
                .ToListAsync();

            var compatibleMethods = await _context.TestMethodSpecifications
                .AsNoTracking()
                .Include(m => m.MetalClassifications)
                .Where(m => !m.IsDisabled && m.IsActive && m.MetalClassifications.Any(mc => mc.MetalClassificationID == metalClassificationId))
                .Select(m => new
                {
                    m.ID,
                    m.Name,
                    m.TestMethodStandard,
                    m.DisplayTitle
                })
                .ToListAsync();

            return new
            {
                success = true,
                metalClassificationID = metalClassificationId,
                metalClassificationName = metal?.Name,
                validTechniques,
                compatibleMethods
            };
        }

        public async Task<object> GetMaterialSpecCascadeAsync(long materialSpecId)
        {
            var suggestedTests = await _autoSuggestService.GetSuggestedTestsBySpecification(materialSpecId);

            var elementLimits = await _context.SpecificationLines
                .AsNoTracking()
                .Include(l => l.Parameter)
                .Include(l => l.ParameterUnit)
                .Where(l => l.SpecificationGradeID == materialSpecId)
                .Select(l => new
                {
                    l.ID,
                    parameterID = l.ParameterID,
                    parameterName = l.Parameter != null ? l.Parameter.Name : "",
                    l.MinValue,
                    l.MaxValue,
                    unit = l.ParameterUnit != null ? l.ParameterUnit.Name : (l.Parameter != null && l.Parameter.ParameterUnit != null ? l.Parameter.ParameterUnit.Name : "")
                })
                .ToListAsync();

            return new
            {
                success = true,
                materialSpecId,
                suggestedTests,
                elementLimits
            };
        }

        public async Task<object> GetLabTestCascadeAsync(long labTestId)
        {
            var labTest = await _context.LaboratoryTests
                .AsNoTracking()
                .Include(t => t.SubGroups)
                    .ThenInclude(sg => sg.TestMethods)
                        .ThenInclude(m => m.TestMethodSpecification)
                .FirstOrDefaultAsync(t => t.ID == labTestId);

            if (labTest == null)
                return new { success = false, message = "Lab Test not found" };

            var compatibleMethods = labTest.SubGroups
                .SelectMany(sg => sg.TestMethods)
                .Where(tm => tm.TestMethodSpecification != null && !tm.TestMethodSpecification.IsDisabled)
                .Select(tm => new
                {
                    tm.TestMethodSpecification!.ID,
                    tm.TestMethodSpecification.Name,
                    tm.TestMethodSpecification.TestMethodStandard,
                    tm.TestMethodSpecification.DisplayTitle,
                    tm.TestMethodSpecification.FormulaExpression
                })
                .GroupBy(m => m.ID)
                .Select(g => g.First())
                .ToList();

            return new
            {
                success = true,
                labTestID = labTest.ID,
                labTestName = labTest.Name,
                isChemical = labTest.IsChemicalTest,
                isMechanical = labTest.IsMechanical,
                subGroups = labTest.SubGroups.Select(sg => new { sg.ID, sg.Name, sg.ReportTestName }),
                compatibleMethods
            };
        }

        public async Task<object> GetTechniqueAnalysisTypesAsync(long techniqueId, long metalId)
        {
            var analysisTypesQuery = _context.Set<LaboratoryTestAnalysisType>()
                .AsNoTracking()
                .Include(a => a.AllowedTechniques)
                    .ThenInclude(t => t.AnalysisTechnique)
                .Include(a => a.Parameters)
                    .ThenInclude(p => p.Parameter)
                        .ThenInclude(p => p!.ParameterUnit)
                .Include(a => a.TestMethods)
                    .ThenInclude(m => m.TestMethodSpecification)
                .Where(a => a.IsActive && a.AllowedTechniques.Any(at => at.AnalysisTechniqueID == techniqueId));

            if (metalId > 0)
            {
                analysisTypesQuery = analysisTypesQuery.Where(a => a.MetalClassificationID == metalId || a.MetalClassificationID == null);
            }

            var analysisTypes = await analysisTypesQuery.ToListAsync();

            var result = analysisTypes.Select(at => new
            {
                at.ID,
                at.Name,
                at.LaboratoryTestSubGroupID,
                at.MetalClassificationID,
                parameters = at.Parameters.Select(p => new
                {
                    p.ID,
                    parameterID = p.ParameterID,
                    parameterName = p.Parameter?.Name,
                    parameterUnit = p.Parameter?.ParameterUnit?.Name ?? "",
                    p.IsMandatory,
                    p.IsReportable
                }),
                compatibleMethods = at.TestMethods
                    .Where(tm => tm.TestMethodSpecification != null && !tm.TestMethodSpecification.IsDisabled)
                    .Select(tm => new
                    {
                        tm.TestMethodSpecification!.ID,
                        tm.TestMethodSpecification.Name,
                        tm.TestMethodSpecification.TestMethodStandard,
                        tm.TestMethodSpecification.DisplayTitle
                    })
                    .GroupBy(m => m.ID)
                    .Select(g => g.First())
            }).ToList();

            return new
            {
                success = true,
                techniqueID = techniqueId,
                metalClassificationID = metalId,
                analysisTypes = result
            };
        }
    }
}
