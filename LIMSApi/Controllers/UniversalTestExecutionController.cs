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
    public class UniversalTestExecutionController : ControllerBase
    {
        private readonly IUniversalTestExecutionService _executionService;
        private readonly IBranchContext _branchContext;

        public UniversalTestExecutionController(
            IUniversalTestExecutionService executionService,
            IBranchContext branchContext)
        {
            _executionService = executionService;
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

        [HttpGet("{testExecutionId}")]
        [RequirePermission(Permissions.Testing.Read)]
        public async Task<IActionResult> GetExecutionById(long testExecutionId)
        {
            var ctx = GetContext();
            if (ctx.userId == 0 || ctx.branchId == 0)
            {
                return Unauthorized("Branch or user context missing.");
            }

            var execution = await _executionService.GetExecutionByIdAsync(testExecutionId, ctx.branchId, ctx.organizationId, ctx.canViewAll);
            if (execution == null)
            {
                return NotFound(new { success = false, message = "Execution not found or access denied for current branch." });
            }

            return Ok(new { success = true, data = execution });
        }

        [HttpGet("by-group/{universalTestGroupId}")]
        [RequirePermission(Permissions.Testing.Read)]
        public async Task<IActionResult> GetExecutionByGroupId(long universalTestGroupId)
        {
            var ctx = GetContext();
            if (ctx.userId == 0 || ctx.branchId == 0)
            {
                return Unauthorized("Branch or user context missing.");
            }

            var execution = await _executionService.GetExecutionByGroupIdAsync(universalTestGroupId, ctx.branchId, ctx.organizationId, ctx.canViewAll);
            if (execution == null)
            {
                return NotFound(new { success = false, message = "No execution found for this test group." });
            }

            return Ok(new { success = true, data = execution });
        }

        [HttpPost("start/{universalTestGroupId}")]
        [RequirePermission(Permissions.Testing.Perform)]
        public async Task<IActionResult> StartExecution(long universalTestGroupId, [FromQuery] bool isRetest = false)
        {
            var ctx = GetContext();
            if (ctx.userId == 0 || ctx.branchId == 0)
            {
                return Unauthorized("Branch or user context missing.");
            }

            try
            {
                var execution = await _executionService.StartExecutionAsync(universalTestGroupId, ctx.userId, ctx.branchId, ctx.organizationId, isRetest);
                return Ok(new { success = true, data = execution, executionId = execution.ID });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("save-observations/{testExecutionId}")]
        [RequirePermission(Permissions.Testing.SaveResult)]
        public async Task<IActionResult> SaveObservations(long testExecutionId, [FromBody] TestExecutionSaveDto data)
        {
            var ctx = GetContext();
            if (ctx.userId == 0 || ctx.branchId == 0)
            {
                return Unauthorized("Branch or user context missing.");
            }

            try
            {
                var execution = await _executionService.SaveObservationsAsync(testExecutionId, data, ctx.userId, ctx.branchId, ctx.organizationId);
                return Ok(new { success = true, data = execution, executionId = execution.ID });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("complete/{testExecutionId}")]
        [RequirePermission(Permissions.Testing.Perform)]
        public async Task<IActionResult> CompleteExecution(long testExecutionId)
        {
            var ctx = GetContext();
            if (ctx.userId == 0 || ctx.branchId == 0)
            {
                return Unauthorized("Branch or user context missing.");
            }

            try
            {
                var execution = await _executionService.CompleteExecutionAsync(testExecutionId, ctx.userId, ctx.branchId, ctx.organizationId);
                return Ok(new { success = true, data = execution, executionId = execution.ID });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("verify/{testExecutionId}")]
        [RequirePermission(Permissions.Testing.VerifyResult)]
        public async Task<IActionResult> VerifyExecution(long testExecutionId, [FromBody] ExecutionActionDto dto)
        {
            var ctx = GetContext();
            if (ctx.userId == 0 || ctx.branchId == 0)
            {
                return Unauthorized("Branch or user context missing.");
            }

            try
            {
                var execution = await _executionService.VerifyExecutionAsync(testExecutionId, dto, ctx.userId, ctx.branchId, ctx.organizationId);
                return Ok(new { success = true, data = execution, executionId = execution.ID });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("approve/{testExecutionId}")]
        [RequirePermission(Permissions.Reporting.Approve)]
        public async Task<IActionResult> ApproveExecution(long testExecutionId, [FromBody] ExecutionActionDto dto)
        {
            var ctx = GetContext();
            if (ctx.userId == 0 || ctx.branchId == 0)
            {
                return Unauthorized("Branch or user context missing.");
            }

            try
            {
                var execution = await _executionService.ApproveExecutionAsync(testExecutionId, dto, ctx.userId, ctx.branchId, ctx.organizationId);
                return Ok(new { success = true, data = execution, executionId = execution.ID });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("reject/{testExecutionId}")]
        [RequirePermission(Permissions.Testing.VerifyResult)]
        public async Task<IActionResult> RejectExecution(long testExecutionId, [FromBody] ExecutionActionDto dto)
        {
            var ctx = GetContext();
            if (ctx.userId == 0 || ctx.branchId == 0)
            {
                return Unauthorized("Branch or user context missing.");
            }

            try
            {
                var execution = await _executionService.RejectExecutionAsync(testExecutionId, dto, ctx.userId, ctx.branchId, ctx.organizationId);
                return Ok(new { success = true, data = execution, executionId = execution.ID });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPut("{testExecutionId}/config")]
        [RequirePermission(Permissions.Testing.Perform)]
        public async Task<IActionResult> UpdateConfiguration(long testExecutionId, [FromBody] TestExecutionConfigSnapshotDto updatedConfig)
        {
            var ctx = GetContext();
            if (ctx.userId == 0 || ctx.branchId == 0)
            {
                return Unauthorized("Branch or user context missing.");
            }

            try
            {
                var execution = await _executionService.UpdateConfigurationAsync(testExecutionId, updatedConfig, ctx.userId, ctx.branchId, ctx.organizationId);
                return Ok(new { success = true, data = execution, message = "Configuration updated successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("{testExecutionId}/calculations-dag")]
        [RequirePermission(Permissions.Testing.Read)]
        public async Task<IActionResult> GetCalculationTrace(long testExecutionId)
        {
            var ctx = GetContext();
            if (ctx.userId == 0 || ctx.branchId == 0)
            {
                return Unauthorized("Branch or user context missing.");
            }

            try
            {
                var trace = await _executionService.GetCalculationTraceAsync(testExecutionId, ctx.branchId, ctx.organizationId);
                return Ok(new { success = true, data = trace });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("{testExecutionId}/results-overview")]
        [RequirePermission(Permissions.Testing.Read)]
        public async Task<IActionResult> GetResultsOverview(long testExecutionId)
        {
            var ctx = GetContext();
            if (ctx.userId == 0 || ctx.branchId == 0)
            {
                return Unauthorized("Branch or user context missing.");
            }

            try
            {
                var overview = await _executionService.GetResultsOverviewAsync(testExecutionId, ctx.branchId, ctx.organizationId);
                return Ok(new { success = true, data = overview });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("{testExecutionId}/nabl-scope")]
        [RequirePermission(Permissions.Testing.Read)]
        public async Task<IActionResult> GetNablScopeSummary(long testExecutionId)
        {
            var ctx = GetContext();
            if (ctx.userId == 0 || ctx.branchId == 0)
            {
                return Unauthorized("Branch or user context missing.");
            }

            try
            {
                var summary = await _executionService.GetNablScopeSummaryAsync(testExecutionId, ctx.branchId, ctx.organizationId);
                return Ok(new { success = true, data = summary });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("formula/preview")]
        [RequirePermission(Permissions.Testing.Read)]
        public async Task<IActionResult> PreviewFormula([FromBody] FormulaPreviewRequestDto request)
        {
            try
            {
                var result = await _executionService.PreviewFormulaAsync(request);
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("{testExecutionId}/attachment")]
        [RequirePermission(Permissions.Testing.Perform)]
        public async Task<IActionResult> AddAttachment(long testExecutionId, [FromBody] ExecutionAttachmentUploadDto uploadDto)
        {
            var ctx = GetContext();
            if (ctx.userId == 0 || ctx.branchId == 0)
            {
                return Unauthorized("Branch or user context missing.");
            }

            try
            {
                var execution = await _executionService.AddAttachmentAsync(testExecutionId, uploadDto, ctx.userId, ctx.branchId, ctx.organizationId);
                return Ok(new { success = true, data = execution, message = "Attachment added successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("report-pdf/{testExecutionId}")]
        [HttpGet("{testExecutionId}/report-pdf")]
        [RequirePermission(Permissions.Reporting.Read)]
        public async Task<IActionResult> GenerateReportPdf(long testExecutionId)
        {
            var ctx = GetContext();
            if (ctx.userId == 0 || ctx.branchId == 0)
            {
                return Unauthorized("Branch or user context missing.");
            }

            try
            {
                var pdfBytes = await _executionService.GenerateReportPdfAsync(testExecutionId, ctx.branchId, ctx.organizationId);
                return File(pdfBytes, "application/pdf", $"Universal_Test_Report_{testExecutionId}.pdf");
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
}
