using LIMSApi.Dtos;
using LIMSApi.ServiceWORepo;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LIMSApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportTemplatesController : ControllerBase
    {
        private readonly IReportTemplateService _templateService;

        public ReportTemplatesController(IReportTemplateService templateService)
        {
            _templateService = templateService;
        }

        // -------------------------------------------------
        // GET: /api/report-templates?testType=Chemical
        // -------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> GetTemplates([FromQuery] string? testType)
        {
            var templates = await _templateService.GetTemplatesAsync(testType);
            return Ok(templates);
        }

        // -------------------------------------------------
        // GET: /api/report-templates/{id}
        // -------------------------------------------------
        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetTemplate(long id)
        {
            var template = await _templateService.GetTemplateByIdAsync(id);
            if (template == null)
                return NotFound();

            return Ok(template);
        }

        // -------------------------------------------------
        // POST: /api/report-templates
        // -------------------------------------------------
        [HttpPost]
        public async Task<IActionResult> CreateTemplate([FromBody] ReportTemplateDto dto)
        {
            var id = await _templateService.CreateTemplateAsync(dto);
            return Ok(new { TemplateId = id });
        }

        // -------------------------------------------------
        // PUT: /api/report-templates/{id}
        // -------------------------------------------------
        [HttpPut("{id:long}")]
        public async Task<IActionResult> UpdateTemplate(long id, [FromBody] ReportTemplateDto dto)
        {
            await _templateService.UpdateTemplateAsync(id, dto);
            return NoContent();
        }

        // -------------------------------------------------
        // DELETE: /api/report-templates/{id}
        // -------------------------------------------------
        [HttpDelete("{id:long}")]
        public async Task<IActionResult> DeleteTemplate(long id)
        {
            await _templateService.DeleteTemplateAsync(id);
            return NoContent();
        }

        // -------------------------------------------------
        // POST: /api/report-templates/{id}/clone
        // -------------------------------------------------
        [HttpPost("{id:long}/clone")]
        public async Task<IActionResult> CloneTemplate(long id)
        {
            var newId = await _templateService.CloneTemplateAsync(id);
            return Ok(new { TemplateId = newId });
        }
    }
}
