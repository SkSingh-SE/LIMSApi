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
    public class UniversalReportController : ControllerBase
    {
        private readonly IUniversalReportService _reports;
        private readonly IBranchContext _branchContext;

        public UniversalReportController(IUniversalReportService reports, IBranchContext branchContext)
        {
            _reports = reports;
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

        [HttpGet("formats")]
        [RequirePermission(Permissions.UniversalReport.Read)]
        public async Task<IActionResult> GetFormats()
        {
            var ctx = GetContext();
            var user = LoggedInUserProvider.CurrentUser;
            var companyCode = user?.CompanyCode ?? "LIMS";
            var list = await _reports.GetAvailableFormatsAsync(ctx.organizationId, companyCode);
            return Ok(new { success = true, data = list });
        }

        [HttpGet("preview/{testExecutionId}")]
        [RequirePermission(Permissions.UniversalReport.Read)]
        public async Task<IActionResult> Preview(long testExecutionId, [FromQuery] string? formatCode = null)
        {
            var ctx = GetContext();
            if (ctx.userId == 0 || ctx.branchId == 0 || ctx.organizationId == 0)
                return Unauthorized("Branch, user, or organization context missing.");
            var dto = await _reports.PreviewAsync(testExecutionId, ctx.userId, ctx.branchId, ctx.organizationId, formatCode, ctx.canViewAll);
            return Ok(new { success = true, data = dto });
        }

        [HttpGet("by-execution/{testExecutionId}")]
        [RequirePermission(Permissions.UniversalReport.Read)]
        public async Task<IActionResult> ListByExecution(long testExecutionId)
        {
            var ctx = GetContext();
            if (ctx.userId == 0 || ctx.branchId == 0 || ctx.organizationId == 0)
                return Unauthorized("Branch, user, or organization context missing.");
            var list = await _reports.ListByExecutionAsync(testExecutionId, ctx.branchId, ctx.organizationId, ctx.canViewAll);
            return Ok(new { success = true, data = list });
        }

        [HttpGet("{reportId}")]
        [RequirePermission(Permissions.UniversalReport.Read)]
        public async Task<IActionResult> GetById(long reportId)
        {
            var ctx = GetContext();
            if (ctx.userId == 0 || ctx.branchId == 0 || ctx.organizationId == 0)
                return Unauthorized("Branch, user, or organization context missing.");
            var dto = await _reports.GetByIdAsync(reportId, ctx.branchId, ctx.organizationId, ctx.canViewAll);
            if (dto == null) return NotFound(new { success = false, message = "Report not found or access denied." });
            return Ok(new { success = true, data = dto });
        }

        [HttpPost("generate/{testExecutionId}")]
        [RequirePermission(Permissions.UniversalReport.Generate)]
        public async Task<IActionResult> Generate(long testExecutionId, [FromBody] GenerateUniversalReportRequestDto? dto)
        {
            var ctx = GetContext();
            if (ctx.userId == 0 || ctx.branchId == 0 || ctx.organizationId == 0)
                return Unauthorized("Branch, user, or organization context missing.");
            var result = await _reports.GenerateAsync(testExecutionId, ctx.userId, ctx.branchId, ctx.organizationId, dto?.Remarks, dto?.ReportFormatCode);
            return Ok(new { success = true, data = result });
        }

        [HttpPost("{reportId}/release")]
        [RequirePermission(Permissions.UniversalReport.Release)]
        public async Task<IActionResult> Release(long reportId, [FromBody] ReleaseUniversalReportRequestDto? dto)
        {
            var ctx = GetContext();
            if (ctx.userId == 0 || ctx.branchId == 0 || ctx.organizationId == 0)
                return Unauthorized("Branch, user, or organization context missing.");
            var result = await _reports.ReleaseAsync(reportId, ctx.userId, ctx.branchId, ctx.organizationId, dto?.Remarks);
            return Ok(new { success = true, data = result });
        }

        [HttpPost("{reportId}/reissue")]
        [RequirePermission(Permissions.UniversalReport.Reissue)]
        public async Task<IActionResult> Reissue(long reportId, [FromBody] ReissueUniversalReportRequestDto? dto)
        {
            var ctx = GetContext();
            if (ctx.userId == 0 || ctx.branchId == 0 || ctx.organizationId == 0)
                return Unauthorized("Branch, user, or organization context missing.");
            var result = await _reports.ReissueAsync(reportId, ctx.userId, ctx.branchId, ctx.organizationId, dto?.Reason);
            return Ok(new { success = true, data = result });
        }

        [HttpPost("{reportId}/void")]
        [RequirePermission(Permissions.UniversalReport.Release)]
        public async Task<IActionResult> Void(long reportId, [FromBody] ReissueUniversalReportRequestDto? dto)
        {
            var ctx = GetContext();
            if (ctx.userId == 0 || ctx.branchId == 0 || ctx.organizationId == 0)
                return Unauthorized("Branch, user, or organization context missing.");
            var result = await _reports.VoidAsync(reportId, ctx.userId, ctx.branchId, ctx.organizationId, dto?.Reason);
            return Ok(new { success = true, data = result });
        }

        [HttpGet("{reportId}/download")]
        [RequirePermission(Permissions.UniversalReport.Read)]
        public async Task<IActionResult> Download(long reportId)
        {
            var ctx = GetContext();
            if (ctx.userId == 0 || ctx.branchId == 0 || ctx.organizationId == 0)
                return Unauthorized("Branch, user, or organization context missing.");
            var (bytes, reportNo, fileName) = await _reports.GeneratePdfBytesAsync(reportId, ctx.branchId, ctx.organizationId, ctx.canViewAll);
            return File(bytes, "application/pdf", fileName);
        }
    }
}
