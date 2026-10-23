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

        public PlanController(IPlanService planService)
        {
            _planService = planService;
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
    }
}
