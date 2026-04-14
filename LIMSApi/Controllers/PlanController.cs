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
    }
}
