using LIMSApi.Dtos;
using LIMSApi.ServiceWORepo;
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
            return Ok(report);
        }

    }
}
