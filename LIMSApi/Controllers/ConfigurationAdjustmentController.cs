using System;
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
    public class ConfigurationAdjustmentController : ControllerBase
    {
        private readonly IConfigurationAdjustmentService _adjustmentService;
        private readonly IBranchContext _branchContext;

        public ConfigurationAdjustmentController(
            IConfigurationAdjustmentService adjustmentService,
            IBranchContext branchContext)
        {
            _adjustmentService = adjustmentService;
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
            long organizationId = _branchContext.CurrentOrganizationID ?? user?.OrganizationID ?? 1;
            bool canViewAll = _branchContext.CanViewAllBranches;

            return (userId, branchId, organizationId, canViewAll);
        }

        [HttpGet("by-test-group/{universalTestGroupId}")]
        [RequirePermission(Permissions.ConfigurationAdjustment.View)]
        public async Task<IActionResult> GetByTestGroupId(long universalTestGroupId)
        {
            var ctx = GetContext();
            if (ctx.userId == 0 || ctx.branchId == 0)
            {
                return Unauthorized(new { success = false, message = "Branch or user context missing." });
            }

            var result = await _adjustmentService.GetActiveAdjustmentAsync(universalTestGroupId);
            if (result == null)
            {
                return Ok(new { success = true, data = (object?)null, message = "No active configuration adjustment." });
            }

            return Ok(new { success = true, data = result });
        }

        [HttpGet("{id}")]
        [RequirePermission(Permissions.ConfigurationAdjustment.View)]
        public async Task<IActionResult> GetById(long id)
        {
            var ctx = GetContext();
            if (ctx.userId == 0 || ctx.branchId == 0)
            {
                return Unauthorized(new { success = false, message = "Branch or user context missing." });
            }

            var result = await _adjustmentService.GetByIdAsync(id);
            if (result == null)
            {
                return NotFound(new { success = false, message = "Configuration adjustment not found." });
            }

            return Ok(new { success = true, data = result });
        }

        [HttpGet("history/{universalTestGroupId}")]
        [RequirePermission(Permissions.ConfigurationAdjustment.View)]
        public async Task<IActionResult> GetHistory(long universalTestGroupId)
        {
            var ctx = GetContext();
            if (ctx.userId == 0 || ctx.branchId == 0)
            {
                return Unauthorized(new { success = false, message = "Branch or user context missing." });
            }

            var result = await _adjustmentService.GetHistoryAsync(universalTestGroupId);
            return Ok(new { success = true, data = result });
        }

        [HttpGet("difference-audit/{universalTestGroupId}")]
        [RequirePermission(Permissions.ConfigurationAdjustment.View)]
        public async Task<IActionResult> GetDifferenceAudit(long universalTestGroupId, [FromQuery] string? section = null, [FromQuery] string? changeType = null)
        {
            var ctx = GetContext();
            if (ctx.userId == 0 || ctx.branchId == 0)
            {
                return Unauthorized(new { success = false, message = "Branch or user context missing." });
            }

            var audit = await _adjustmentService.GetComprehensiveDifferenceAuditAsync(universalTestGroupId);
            return Ok(new { success = true, data = audit });
        }

        [HttpGet("adjusted-configuration/{universalTestGroupId}")]
        [RequirePermission(Permissions.ConfigurationAdjustment.View)]
        public async Task<IActionResult> GetAdjustedConfiguration(long universalTestGroupId, [FromQuery] bool includeUnapproved = false)
        {
            var ctx = GetContext();
            if (ctx.userId == 0 || ctx.branchId == 0)
            {
                return Unauthorized(new { success = false, message = "Branch or user context missing." });
            }

            try
            {
                var result = await _adjustmentService.GetAdjustedConfigurationAsync(universalTestGroupId, includeUnapproved);
                return Ok(new { success = true, data = result });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }

        }

        [HttpPost("draft")]
        [RequirePermission(Permissions.ConfigurationAdjustment.Create)]
        public async Task<IActionResult> SaveDraft([FromBody] ConfigurationAdjustmentDraftDto dto)
        {
            var ctx = GetContext();
            if (ctx.userId == 0 || ctx.branchId == 0)
            {
                return Unauthorized(new { success = false, message = "Branch or user context missing." });
            }

            try
            {
                var result = await _adjustmentService.SaveDraftAsync(dto);
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("validate")]
        [RequirePermission(Permissions.ConfigurationAdjustment.Create)]
        public async Task<IActionResult> ValidateAdjustment([FromBody] ConfigurationAdjustmentDraftDto dto)
        {
            var ctx = GetContext();
            if (ctx.userId == 0 || ctx.branchId == 0)
            {
                return Unauthorized(new { success = false, message = "Branch or user context missing." });
            }

            var result = await _adjustmentService.ValidateAdjustmentAsync(dto.UniversalTestGroupID, dto.Items);
            return Ok(new { success = true, data = result });
        }

        [HttpPost("apply")]
        [RequirePermission(Permissions.ConfigurationAdjustment.Apply)]
        public async Task<IActionResult> ApplyAdjustment([FromBody] ApplyAdjustmentRequestDto dto)
        {
            var ctx = GetContext();
            if (ctx.userId == 0 || ctx.branchId == 0)
            {
                return Unauthorized(new { success = false, message = "Branch or user context missing." });
            }

            try
            {
                var result = await _adjustmentService.ApplyAdjustmentAsync(dto);
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("approve")]
        [RequirePermission(Permissions.ConfigurationAdjustment.Approve)]
        public async Task<IActionResult> ApproveAdjustment([FromBody] ApproveAdjustmentRequestDto dto)
        {
            var ctx = GetContext();
            if (ctx.userId == 0 || ctx.branchId == 0)
            {
                return Unauthorized(new { success = false, message = "Branch or user context missing." });
            }

            try
            {
                var result = await _adjustmentService.ApproveAdjustmentAsync(dto);
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("reject")]
        [RequirePermission(Permissions.ConfigurationAdjustment.Approve)]
        public async Task<IActionResult> RejectAdjustment([FromBody] RejectAdjustmentRequestDto dto)
        {
            var ctx = GetContext();
            if (ctx.userId == 0 || ctx.branchId == 0)
            {
                return Unauthorized(new { success = false, message = "Branch or user context missing." });
            }

            try
            {
                var result = await _adjustmentService.RejectAdjustmentAsync(dto);
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
        [HttpGet("deviation-options/{universalTestGroupId}")]
        [RequirePermission(Permissions.ConfigurationAdjustment.View)]
        public async Task<IActionResult> GetDeviationOptions(long universalTestGroupId, [FromQuery] string category = "Equipment")
        {
            var ctx = GetContext();
            if (ctx.userId == 0 || ctx.branchId == 0)
            {
                return Unauthorized(new { success = false, message = "Branch or user context missing." });
            }

            try
            {
                var result = await _adjustmentService.GetDeviationLookupOptionsAsync(universalTestGroupId, category);
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("request-deviation")]
        [RequirePermission(Permissions.ConfigurationAdjustment.Create)]
        public async Task<IActionResult> RequestDeviation([FromBody] ExecutionDeviationRequestDto dto)
        {
            var ctx = GetContext();
            if (ctx.userId == 0 || ctx.branchId == 0)
            {
                return Unauthorized(new { success = false, message = "Branch or user context missing." });
            }

            try
            {
                var result = await _adjustmentService.RequestExecutionDeviationAsync(dto);
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
}
