using System.Security.Claims;
using System.Threading.Tasks;
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
    public class UniversalResultController : ControllerBase
    {
        private readonly IUniversalResultService _results;
        private readonly IBranchContext _branchContext;

        public UniversalResultController(IUniversalResultService results, IBranchContext branchContext)
        {
            _results = results;
            _branchContext = branchContext;
        }

        private (long userId, long branchId, long organizationId, bool canViewAll) GetContext()
        {
            var user = LoggedInUserProvider.CurrentUser;
            long userId = user?.UserId ?? 0;
            if (userId == 0)
            {
                var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                long.TryParse(claim, out userId);
            }
            long branchId = _branchContext.CurrentBranchID ?? user?.BranchID ?? 0;
            long organizationId = _branchContext.CurrentOrganizationID ?? user?.OrganizationID ?? 0;
            return (userId, branchId, organizationId, _branchContext.CanViewAllBranches);
        }

        [HttpGet("by-execution/{testExecutionId}")]
        [RequirePermission(Permissions.UniversalResult.Read)]
        public async Task<IActionResult> GetByExecution(long testExecutionId)
        {
            var ctx = GetContext();
            if (ctx.userId == 0 || ctx.branchId == 0 || ctx.organizationId == 0)
                return Unauthorized("Branch, user, or organization context missing.");
            var result = await _results.GetLatestByExecutionAsync(testExecutionId, ctx.branchId, ctx.organizationId, ctx.canViewAll);
            if (result == null) return Ok(new { success = true, data = (object?)null, message = "No result exists for this execution. Run Evaluate first." });
            return Ok(new { success = true, data = result });
        }

        [HttpGet("{resultId}")]
        [RequirePermission(Permissions.UniversalResult.Read)]
        public async Task<IActionResult> GetById(long resultId)
        {
            var ctx = GetContext();
            if (ctx.userId == 0 || ctx.branchId == 0 || ctx.organizationId == 0)
                return Unauthorized("Branch, user, or organization context missing.");
            var result = await _results.GetByIdAsync(resultId, ctx.branchId, ctx.organizationId, ctx.canViewAll);
            if (result == null) return NotFound(new { success = false, message = "Result not found or access denied." });
            return Ok(new { success = true, data = result });
        }

        [HttpPost("evaluate/{testExecutionId}")]
        [RequirePermission(Permissions.UniversalResult.Evaluate)]
        public async Task<IActionResult> Evaluate(long testExecutionId, [FromBody] EvaluateResultRequestDto? dto)
        {
            var ctx = GetContext();
            if (ctx.userId == 0 || ctx.branchId == 0 || ctx.organizationId == 0)
                return Unauthorized("Branch, user, or organization context missing.");
            var result = await _results.EvaluateAsync(testExecutionId, ctx.userId, ctx.branchId, ctx.organizationId, dto?.Remarks);
            return Ok(new { success = true, data = result });
        }

        [HttpPost("finalize/{resultId}")]
        [RequirePermission(Permissions.UniversalResult.Finalize)]
        public async Task<IActionResult> Finalize(long resultId, [FromBody] FinalizeResultRequestDto dto)
        {
            var ctx = GetContext();
            if (ctx.userId == 0 || ctx.branchId == 0 || ctx.organizationId == 0)
                return Unauthorized("Branch, user, or organization context missing.");
            var result = await _results.FinalizeAsync(resultId, dto.ConcurrencyToken, ctx.userId, ctx.branchId, ctx.organizationId, dto.Remarks);
            return Ok(new { success = true, data = result });
        }

        [HttpGet("phase9-handoff/{testExecutionId}")]
        [RequirePermission(Permissions.UniversalResult.Read)]
        public async Task<IActionResult> Phase9Handoff(long testExecutionId)
        {
            var ctx = GetContext();
            if (ctx.userId == 0 || ctx.branchId == 0 || ctx.organizationId == 0)
                return Unauthorized("Branch, user, or organization context missing.");
            var handoff = await _results.GetPhase9HandoffAsync(testExecutionId, ctx.branchId, ctx.organizationId, ctx.canViewAll);
            return Ok(new { success = true, data = handoff });
        }
    }
}
