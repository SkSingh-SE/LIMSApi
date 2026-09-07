using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Middleware;
using LIMSApi.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LIMSApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PlanController : ControllerBase
    {
        private readonly IPlanService _planService;
        private readonly IUniversalPlanService _universalPlanService;

        public PlanController(IPlanService planService, IUniversalPlanService universalPlanService)
        {
            _planService = planService;
            _universalPlanService = universalPlanService;
        }

        [HttpGet("history/{planId}")]
        [RequirePermission(Permissions.Plan.Read)]
        public async Task<IActionResult> GetPlanHistory(long planId)
        {
            var history = await _planService.GetPlanHistory(planId);
            return Ok(history);
        }

        [HttpPost("request-replan/{planId}")]
        [RequirePermission(Permissions.Plan.Update)]
        public async Task<IActionResult> RequestReplan(long planId, [FromBody] ReplanRequestDto dto)
        {
            await _planService.RequestReplan(planId, dto.Reason);
            return Ok(new
            {
                status = "success",
                message = "Replan request submitted successfully."
            });
        }

        [HttpPost("approve-replan/{requestId}")]
        [RequirePermission(Permissions.Plan.Approve)]
        public async Task<IActionResult> ApproveReplan(long requestId, [FromBody] ReplanApprovalDto dto)
        {
            await _planService.ApproveReplan(requestId, dto.Remarks);
            return Ok(new
            {
                status = "success",
                message = "Replan request approved successfully."
            });
        }

        [HttpPost("reject-replan/{requestId}")]
        [RequirePermission(Permissions.Plan.Reject)]
        public async Task<IActionResult> RejectReplan(long requestId, [FromBody] ReplanApprovalDto dto)
        {
            await _planService.RejectReplan(requestId, dto.Remarks);
            return Ok(new
            {
                status = "success",
                message = "Replan request rejected."
            });
        }

        [HttpPost("assign-grade")]
        [RequirePermission(Permissions.Plan.Update)]
        public async Task<IActionResult> AssignGrade([FromBody] AssignGradeDto dto)
        {
            await _planService.AssignGradeAsync(dto);
            return Ok(new
            {
                status = "success",
                message = "Grade assigned successfully and audit logged."
            });
        }

        // ────────────── 6-Tier Decision Engine Cascade Endpoints ──────────────

        [HttpGet("cascade/product-master/{id}")]
        public async Task<IActionResult> GetProductMasterCascade(long id)
        {
            var result = await _planService.GetProductMasterCascadeAsync(id);
            return Ok(result);
        }

        [HttpGet("cascade/product-master/{id}/size/{sizeId}")]
        public async Task<IActionResult> GetProductMasterSizeLimits(long id, long sizeId)
        {
            var result = await _planService.GetProductMasterSizeLimitsAsync(id, sizeId);
            return Ok(result);
        }

        [HttpGet("cascade/metal-classification/{id}")]
        public async Task<IActionResult> GetMetalClassificationCascade(long id)
        {
            var result = await _planService.GetMetalClassificationCascadeAsync(id);
            return Ok(result);
        }

        [HttpGet("cascade/material-spec/{id}")]
        public async Task<IActionResult> GetMaterialSpecCascade(long id)
        {
            var result = await _planService.GetMaterialSpecCascadeAsync(id);
            return Ok(result);
        }

        [HttpGet("cascade/lab-test/{id}")]
        public async Task<IActionResult> GetLabTestCascade(long id)
        {
            var result = await _planService.GetLabTestCascadeAsync(id);
            return Ok(result);
        }

        [HttpGet("cascade/technique/{techniqueId}/metal/{metalId}")]
        public async Task<IActionResult> GetTechniqueAnalysisTypes(long techniqueId, long metalId)
        {
            var result = await _planService.GetTechniqueAnalysisTypesAsync(techniqueId, metalId);
            return Ok(result);
        }

        // ────────────── Screen 14: Universal Test Planning Endpoints ──────────────

        [HttpGet("universal/workspace/{inwardId}")]
        [RequirePermission(Permissions.Plan.Read)]
        public async Task<IActionResult> GetUniversalPlanWorkspace(long inwardId, [FromQuery] long sampleId)
        {
            if (sampleId <= 0)
            {
                return BadRequest(new { message = "SampleID is required to open Universal Plan Workspace. Please specify ?sampleId=." });
            }

            var workspace = await _universalPlanService.GetPlanWorkspaceAsync(inwardId, sampleId);
            return Ok(workspace);
        }

        [HttpGet("universal/inward-samples/{inwardId}")]
        [RequirePermission(Permissions.Plan.Read)]
        public async Task<IActionResult> GetUniversalInwardSamples(long inwardId)
        {
            var samples = await _universalPlanService.GetInwardSamplesOverviewAsync(inwardId);
            return Ok(samples);
        }

        [HttpGet("universal/method-versions/{methodId}")]
        [RequirePermission(Permissions.Plan.Read)]
        public async Task<IActionResult> GetUniversalMethodVersions(long methodId)
        {
            var versions = await _universalPlanService.GetMethodVersionsAsync(methodId);
            return Ok(versions);
        }

        [HttpPost("universal/preview")]
        [RequirePermission(Permissions.Plan.Read)]
        public async Task<IActionResult> PreviewUniversalTestConfiguration([FromBody] UniversalPlanPreviewRequestDto request)
        {
            var preview = await _universalPlanService.PreviewTestConfigurationAsync(request);
            return Ok(preview);
        }

        [HttpPost("universal/save-draft")]
        [RequirePermission(Permissions.Plan.Update)]
        public async Task<IActionResult> SaveUniversalPlanDraft([FromBody] UniversalPlanSaveDto dto)
        {
            var result = await _universalPlanService.SaveDraftPlanAsync(dto);
            return Ok(result);
        }

        [HttpPost("universal/validate")]
        [RequirePermission(Permissions.Plan.Read)]
        public async Task<IActionResult> ValidateUniversalPlan([FromBody] UniversalPlanSaveDto dto)
        {
            var validation = await _universalPlanService.ValidatePlanAsync(dto);
            return Ok(validation);
        }

        [HttpPost("universal/create-test-group")]
        [RequirePermission(Permissions.Plan.Create)]
        public async Task<IActionResult> CreateUniversalTestGroups([FromBody] UniversalPlanConfirmDto dto)
        {
            var result = await _universalPlanService.CreateTestGroupsAsync(dto);
            return Ok(result);
        }

        [HttpPost("universal/copy-plan")]
        [RequirePermission(Permissions.Plan.Create)]
        public async Task<IActionResult> CopyUniversalPlan([FromBody] UniversalPlanCopyRequestDto dto)
        {
            var result = await _universalPlanService.CopyPlanToSamplesAsync(dto);
            return Ok(result);
        }
    }
}
