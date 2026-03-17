using System.Text.Json;
using LIMSApi.Dtos;
using LIMSApi.Services.Interface;
using LIMSApi.ServiceWORepo;
using Microsoft.AspNetCore.Mvc;

namespace LIMSApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NablController : ControllerBase
    {
        private readonly INablService _service;
        private readonly INablAuditService _auditService;

        public NablController(INablService service, INablAuditService auditService)
        {
            _service = service;
            _auditService = auditService;
        }

        // ─── CRUD ────────────────────────────────────────────────────────

        [HttpPost("{formType}/list")]
        public async Task<IActionResult> List(string formType, PageFilter filter)
        {
            return Ok(await _service.FetchList(formType, filter));
        }

        [HttpGet("{formType}/details/{id}")]
        public async Task<IActionResult> Details(string formType, long id)
        {
            var entity = await _service.GetDetails(formType, id);
            return entity == null ? NoContent() : Ok(entity);
        }

        [HttpGet("{formType}/details-by-designation/{designationId}")]
        public async Task<IActionResult> DetailsByDesignation(string formType, long designationId)
        {
            var entity = await _service.GetByDesignationId(formType, designationId);
            return entity == null ? NoContent() : Ok(entity);
        }

        [HttpPost("{formType}/save")]
        public async Task<IActionResult> Save(string formType, [FromBody] JsonElement body)
        {
            var id = await _service.Save(formType, body);
            return Ok(new { message = $"{formType} saved successfully", id });
        }

        [HttpDelete("{formType}/delete/{id}")]
        public async Task<IActionResult> Delete(string formType, long id)
        {
            await _service.Remove(formType, id);
            return Ok(new { message = $"{formType} deleted successfully" });
        }

        // ─── Workflow ────────────────────────────────────────────────────

        [HttpPost("{formType}/submit/{id}")]
        public async Task<IActionResult> Submit(string formType, long id)
        {
            await _service.Submit(formType, id);
            return Ok(new { message = $"{formType} submitted successfully" });
        }

        [HttpPost("{formType}/review/{id}")]
        public async Task<IActionResult> Review(string formType, long id)
        {
            await _service.Review(formType, id);
            return Ok(new { message = $"{formType} reviewed successfully" });
        }

        [HttpPost("{formType}/approve/{id}")]
        public async Task<IActionResult> Approve(string formType, long id)
        {
            await _service.Approve(formType, id);
            return Ok(new { message = $"{formType} approved successfully" });
        }

        [HttpPost("{formType}/reject/{id}")]
        public async Task<IActionResult> Reject(string formType, long id, [FromBody] RejectRequest? request)
        {
            await _service.Reject(formType, id, request?.Remarks);
            return Ok(new { message = $"{formType} rejected successfully" });
        }

        // ─── History & Audit ─────────────────────────────────────────────

        [HttpGet("{formType}/history/{id}")]
        public async Task<IActionResult> History(string formType, long id)
        {
            var history = await _service.GetRevisionHistory(formType, id);
            return Ok(history);
        }

        [HttpGet("{formType}/audit-log/{id}")]
        public async Task<IActionResult> AuditLog(string formType, long id)
        {
            var logs = await _service.GetAuditLog(formType, id);
            return Ok(logs);
        }

        // ─── Dashboard & Audit Package ─────────────────────────────────

        [HttpGet("dashboard")]
        public async Task<IActionResult> Dashboard()
        {
            var data = await _auditService.GetNablDashboard();
            return Ok(data);
        }

        [HttpGet("audit-summary")]
        public async Task<IActionResult> AuditSummary()
        {
            var summary = await _auditService.GetAuditSummary();
            return Ok(summary);
        }

        [HttpPost("audit-package")]
        public async Task<IActionResult> AuditPackage([FromBody] AuditPackageRequest request)
        {
            var pdf = await _auditService.GenerateAuditPackage(request.FormTypes, request.From, request.To);
            return File(pdf, "application/pdf", $"NABL_Audit_Package_{DateTime.UtcNow:yyyyMMdd_HHmmss}.pdf");
        }
    }

    // DTO for reject endpoint
    public class RejectRequest
    {
        public string? Remarks { get; set; }
    }
}
