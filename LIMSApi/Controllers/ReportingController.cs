using LIMSApi.Dtos;
using LIMSApi.Helpers.Enums;
using LIMSApi.Middleware;
using LIMSApi.Reporting;
using LIMSApi.ServiceWORepo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LIMSApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportingController : ControllerBase
    {
        private readonly IReportService _service;

        public ReportingController(IReportService reportingService)
        {
            _service = reportingService;
        }


        // ---------------------------------------------------------
        // 2. GET REPORT BY ID (with blocks)
        // ---------------------------------------------------------
        [HttpGet("{id:long}")]
        public async Task<IActionResult> Get(long id)
        {
            var result = await _service.GetReportAsync(id);
            if (result == null)
                return NotFound($"Report {id} not found");

            return Ok(result);
        }


        // =============================================================
        // Dashboard List
        // =============================================================
        [HttpPost("list")]
        public async Task<IActionResult> List(PageFilter filter)
        {
            return Ok(await _service.GetReportDashboardList(filter));
        }

        // =============================================================
        // Perform Workflow Action
        [HttpPost("perform-action")]
        // [RequirePermission("REPORT_APPROVE")] // TODO: re-enable after permission setup
        public async Task<IActionResult> PerformAction([FromBody] WorkflowActionRequestDto dto)
        {
            if (dto == null)
                return BadRequest("Invalid request");
            var result = await _service.PerformAction(dto);
            return Ok(new { Success = result, Message = result ? "Action performed successfully" : "Action failed" });
        }

        // =============================================================
        // Report Preview
        [HttpGet("preview/{reportHeaderId:long}")]
        public async Task<IActionResult> GetReportPreview(long reportHeaderId)
        {
            var result = await _service.GetReportPreviewAsync(reportHeaderId);
            if (result == null)
                return NotFound($"Report preview for header {reportHeaderId} not found");
            return Ok(result);
        }

        [HttpGet("generate-pdf/{sampleId}")]
        public async Task<IActionResult> GeneratePDFSampleWise(long sampleId)
        {
            var report = await _service.GeneratePdfForSampleAsync(sampleId);
            return Ok(new
            {
                success = true,
                message = "Report Generated Successfully"
            });
        }

        // =============================================================
        // Generate Enhanced PDF (professional layout)
        // =============================================================
        [HttpGet("{id:long}/generate-pdf")]
        public async Task<IActionResult> GenerateEnhancedPdf(long id)
        {
            var filePath = await _service.GenerateEnhancedPdfAsync(id);

            if (!System.IO.File.Exists(filePath))
                return NotFound("Generated PDF file not found on disk.");

            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
            return File(fileBytes, "application/pdf", Path.GetFileName(filePath));
        }

        // =============================================================
        // Preview Report Data (for frontend preview before PDF)
        // =============================================================
        [HttpGet("{id:long}/preview-data")]
        public async Task<IActionResult> GetReportDataPreview(long id)
        {
            var reportData = await _service.BuildReportDataAsync(id);
            if (reportData == null)
                return NotFound($"Report data for header {id} not found");
            return Ok(reportData);
        }

        // ---------------------------------------------------------
        // PUBLIC: Verify Report (QR Code Scan — no auth required)
        // ---------------------------------------------------------
        [HttpGet("verify/{reportNo}")]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyReport(string reportNo)
        {
            var result = await _service.GetReportVerificationAsync(reportNo);
            if (result == null)
                return NotFound(new { message = "Report not found" });
            return Ok(result);
        }

        // ---------------------------------------------------------
        // REPORT FORMATS
        // ---------------------------------------------------------
        [HttpGet("formats")]
        public IActionResult GetAvailableFormats()
        {
            return Ok(ReportDocumentFactory.GetAvailableFormats());
        }

        [HttpGet("{id:long}/generate/{formatType}")]
        public async Task<IActionResult> GenerateByFormat(long id, ReportFormatType formatType, [FromQuery] string? watermark = null)
        {
            var pdfBytes = await _service.GenerateReportByFormatAsync(id, formatType, watermark);
            return File(pdfBytes, "application/pdf", $"Report-{id}-{formatType}.pdf");
        }

        [HttpPost("request-amendment")]
        public async Task<IActionResult> RequestAmendment([FromForm] AmendmentRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Reason))
                return BadRequest("Amendment reason is required.");

            if (request.File == null || request.File.Length == 0)
                return BadRequest("Supporting document is required.");

            await _service.RequestAmendmentAsync(request.ReportHeaderId,request.Reason,request.File);

            return Ok(
                new
                {
                    success = true,
                    message = "Amendment requested successfully."
                });
        }
    }
}
