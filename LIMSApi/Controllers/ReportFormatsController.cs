using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.ServiceWORepo;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportFormatsController : ControllerBase
    {
        private readonly IReportFormatService _service;
        private readonly IReportAutoGenerationService _autoGenService;
        private readonly LIMSContext _db;

        public ReportFormatsController(IReportFormatService service, IReportAutoGenerationService autoGenService, LIMSContext db)
        {
            _service = service;
            _autoGenService = autoGenService;
            _db = db;
        }

        // POST: /api/ReportFormats/list
        [HttpPost("list")]
        public async Task<IActionResult> List([FromBody] PageFilter filter)
        {
            var result = await _service.GetFormatListAsync(filter);
            return Ok(result);
        }

        // GET: /api/ReportFormats/{id}
        [HttpGet("details/{id:long}")]
        public async Task<IActionResult> Get(long id)
        {
            var result = await _service.GetFormatByIdAsync(id);
            return result == null ? NotFound() : Ok(result);
        }

        // POST: /api/ReportFormats/create
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] ReportFormatDto dto)
        {
            var id = await _service.CreateFormatAsync(dto);
            return Ok(new { message = "Report format created successfully.", id });
        }

        // PUT: /api/ReportFormats/update
        [HttpPut("update")]
        public async Task<IActionResult> Update([FromBody] ReportFormatDto dto)
        {
            await _service.UpdateFormatAsync(dto.Id, dto);
            return Ok(new { message = "Report format updated successfully." });
        }

        // DELETE: /api/ReportFormats/delete/{id}
        [HttpDelete("delete/{id:long}")]
        public async Task<IActionResult> Delete(long id)
        {
            await _service.DeleteFormatAsync(id);
            return Ok(new { message = "Report format deleted successfully." });
        }

        // POST: /api/ReportFormats/clone/{id}
        [HttpPost("clone/{id:long}")]
        public async Task<IActionResult> Clone(long id)
        {
            var newId = await _service.CloneFormatAsync(id);
            return Ok(new { message = "Report format cloned successfully.", id = newId });
        }

        // GET: /api/ReportFormats/section-types
        [HttpGet("section-types")]
        public IActionResult GetSectionTypes()
        {
            return Ok(_service.GetSectionTypes());
        }

        // GET: /api/ReportFormats/dropdown
        [HttpGet("dropdown")]
        public async Task<IActionResult> GetDropdown(
            [FromQuery] string? searchTerm,
            [FromQuery] int pageNo = 0,
            [FromQuery] int pageSize = 20)
        {
            var result = await _service.GetFormatDropdownAsync(searchTerm, pageNo, pageSize);
            return Ok(result);
        }

        // GET: /api/ReportFormats/{id}/mappings
        [HttpGet("{id:long}/mappings")]
        public async Task<IActionResult> GetMappings(long id)
        {
            var result = await _service.GetMappingsForFormatAsync(id);
            return Ok(result);
        }

        // PUT: /api/ReportFormats/{id}/mappings
        [HttpPut("{id:long}/mappings")]
        public async Task<IActionResult> SaveMappings(long id, [FromBody] List<ReportFormatMappingDto> mappings)
        {
            await _service.SaveMappingsAsync(id, mappings);
            return Ok(new { message = "Mappings saved successfully." });
        }

        // GET: /api/ReportFormats/resolve/{sampleId}
        [HttpGet("resolve/{sampleId:long}")]
        public async Task<IActionResult> ResolveFormat(long sampleId)
        {
            var format = await _service.ResolveFormatForSampleAsync(sampleId);
            if (format == null)
                return NotFound(new { message = "No report format found for this sample." });

            return Ok(new { id = format.ID, formatCode = format.FormatCode, formatName = format.FormatName });
        }

        // POST: /api/ReportFormats/preview/{sampleId}?formatId={id}
        [HttpPost("preview/{sampleId:long}")]
        public async Task<IActionResult> PreviewPdf(long sampleId, [FromQuery] long? formatId)
        {
            var pdfBytes = await _autoGenService.GeneratePreviewAsync(sampleId, formatId);
            return File(pdfBytes, "application/pdf", "preview.pdf");
        }

        // POST: /api/ReportFormats/generate/{sampleId}
        [HttpPost("generate/{sampleId:long}")]
        public async Task<IActionResult> GenerateReport(long sampleId)
        {
            await _autoGenService.GenerateAsync(sampleId);
            return Ok(new { message = "Report generated successfully." });
        }

        // GET: /api/ReportFormats/verified-samples — samples where all tests are verified
        [HttpGet("verified-samples")]
        public async Task<IActionResult> GetVerifiedSamples(
            [FromQuery] string? searchTerm,
            [FromQuery] int pageNo = 0,
            [FromQuery] int pageSize = 20)
        {
            var loggedInUser = LoggedInUserProvider.CurrentUser;
            var companyCode = loggedInUser?.CompanyCode ?? "LIMS";

            // Samples in TESTING_VERIFIED or later report statuses
            var verifiedStatuses = new[] {
                "TESTING_VERIFIED", "REPORT_GENERATION_IN_PROGRESS", "REPORT_GENERATED",
                "REPORT_UNDER_REVIEW", "FINAL_REPORT_APPROVED", "REPORT_DISPATCHED"
            };

            var query = from s in _db.SampleDetails
                        join i in _db.SampleInwards on s.InwardID equals i.ID
                        join c in _db.Customers on i.CustomerID equals c.ID
                        where s.IsActive && i.CompanyCode == companyCode
                              && verifiedStatuses.Contains(s.SampleStatus)
                        select new { s.ID, s.SampleNo, i.CaseNo, CustomerName = c.Name, s.SampleStatus };

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                // Rely on SQL Server CI collation — no LOWER() on columns (keeps indexes sargable).
                var term = searchTerm.Trim();
                query = query.Where(x =>
                    x.SampleNo.Contains(term) ||
                    x.CaseNo.Contains(term) ||
                    x.CustomerName.Contains(term));
            }

            var items = await query
                .OrderByDescending(x => x.ID)
                .Skip(pageNo * pageSize)
                .Take(pageSize)
                .Select(x => new { id = x.ID, name = $"{x.SampleNo} | {x.CaseNo} | {x.CustomerName}" })
                .ToListAsync();

            return Ok(items);
        }
    }
}
