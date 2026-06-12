using LIMSApi.Dtos;
using LIMSApi.Services;
using LIMSApi.Services.Interface;
using LIMSApi.ServiceWORepo;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

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

        // ─── Form Defaults & Reviewers ──────────────────────────────────

        [HttpGet("form-defaults/{formType}")]
        public async Task<IActionResult> FormDefaults(string formType)
        {
            var defaults = await _service.GetFormDefaults(formType);
            return Ok(defaults);
        }

        [HttpGet("suggested-reviewers")]
        public async Task<IActionResult> SuggestedReviewers()
        {
            var reviewers = await _service.GetSuggestedReviewers();
            return Ok(reviewers);
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
        [HttpGet("{formType}/training-plan-dropdown")]
        public async Task<IActionResult> Trainingplandropdown(string? searchTerm, int pageNo, int pageSize)
        {
            var data = await _service.GetTraningPlanDropdown(searchTerm, pageNo, pageSize);
            return data == null ? NoContent() : Ok(data);
        }
        [HttpGet("{formType}/room-dropdown")]
        public async Task<IActionResult> Roomdropdown(string? searchTearm, int pageNo, int pageSize)
        {
            var data = await _service.Roomdropdown(searchTearm, pageNo, pageSize);
            return data == null ? NoContent() : Ok(data);
        }

        [HttpPost("{formType}/upload-signature")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadSignature(IFormFile logo, CancellationToken cancellationToken = default)
        {
            var uploadedRef = await _service.UploadSignatureAsync(logo, cancellationToken);
            return Ok(uploadedRef);
        }
        [HttpGet("{formType}/next-register-no")]
        public async Task<IActionResult> GetNextRegisterNo()
        {
            var registerNo = await _service.GetNextRegisterNo();
            return Ok(new { registerNo });
        }
        [HttpGet("{formType}/supplierlist")]
        public async Task<IActionResult> Supplierlist(string? searchTearm, int pageNo, int pageSize)
        {
            var data = await _service.Supplierlist(searchTearm, pageNo, pageSize);
            return data == null ? NoContent() : Ok(data);
        }
        [HttpGet("{formType}/allsupplierlist")]
        public async Task<IActionResult> AllSupplierlist(string? searchTearm, int pageNo, int pageSize)
        {
            var data = await _service.AllSupplierlist(searchTearm, pageNo, pageSize);
            return data == null ? NoContent() : Ok(data);
        }
        [HttpGet("{formType}/next-indent-no")]
        public async Task<IActionResult> GetNextIndentNo()
        {
            var piNo = await _service.GetNextIndentNo();
            return Ok(new { piNo });
        }
        [HttpGet("{formType}/next-plan-no")]
        public async Task<IActionResult> GetNextPlanNo()
        {
            var planNo = await _service.GetNextPlanNo();
            return Ok(new { planNo });
        }
        [HttpGet("{formType}/indentNoList")]
        public async Task<IActionResult> IndentNoList(string? searchTearm, int pageNo, int pageSize)
        {
            var data = await _service.IndentNoList(searchTearm, pageNo, pageSize);
            return data == null ? NoContent() : Ok(data);
        }
        [HttpGet("{formType}/approvedSupplierlist")]
        public async Task<IActionResult> ApprovedSupplierlist(string? searchTearm, int pageNo, int pageSize)
        {
            var data = await _service.ApprovedSupplierlist(searchTearm, pageNo, pageSize);
            return data == null ? NoContent() : Ok(data);
        }
        [HttpGet("{formType}/planNoList")]
        public async Task<IActionResult> PlanNoDetailslist(string? searchTearm, int pageNo, int pageSize)
        {
            var data = await _service.PlanNoDetailslist(searchTearm, pageNo, pageSize);
            return data == null ? NoContent() : Ok(data);
        }
        [HttpGet("{formType}/pONoList")]
        public async Task<IActionResult> PONoListDetailslist(string formType, string? searchTearm, int pageNo, int pageSize)
        {
            var data = await _service.PONoListDetailslist(formType, searchTearm, pageNo, pageSize);
            return data == null ? NoContent() : Ok(data);
        }
        [HttpGet("{formType}/supplier-evaluation-details")]
        public async Task<IActionResult> SupplierEvaluationDetails(string supplierName, DateTime? fromDate, DateTime? toDate)
        {
            var data = await _service.SupplierEvaluationDetails(supplierName, fromDate, toDate);
            return data == null ? NoContent() : Ok(data);
        }
        [HttpGet("{formType}/po-items-details")]
        public async Task<IActionResult> PoitemsDetails(string poNo, string supplierName)
        {
            var data = await _service.PoitemsDetails(poNo, supplierName);
            return data == null ? NoContent() : Ok(data);
        }
        [HttpGet("{formType}/receive-items-details")]
        public async Task<IActionResult> ReceivedItemsDetails(string poNo, string supplierName)
        {
            var data = await _service.ReceivedItemsDetails(poNo, supplierName);
            return data == null ? NoContent() : Ok(data);
        }
        [HttpGet("{formType}/inspectionplan-details")]
        public async Task<IActionResult> InspectionPlanDetails(string inspectionPlanNo)
        {
            var data = await _service.InspectionPlanDetails(inspectionPlanNo);
            return data == null ? NoContent() : Ok(data);
        }
        [HttpGet("{formType}/indent-details")]
        public async Task<IActionResult> IndentDetails(string indentNo)
        {
            var data = await _service.IndentDetails(indentNo);
            return data == null ? NoContent() : Ok(data);
        }
        [HttpGet("{formType}/alltestmethodlist")]
        public async Task<IActionResult> Alltestmethodlist(string formType, string? searchTearm, int pageNo, int pageSize)
        {
            var data = await _service.Alltestmethodlist(formType, searchTearm, pageNo, pageSize);
            return data == null ? NoContent() : Ok(data);
        }
        [HttpGet("{formType}/testMethodDetails/{testmethodCode}")]
        public async Task<IActionResult> TestMethodDetails(string testmethodCode)
        {
            var data = await _service.TestMethodDetails(testmethodCode);
            return data == null ? NoContent() : Ok(data);
        }
    }

    // DTO for reject endpoint
    public class RejectRequest
    {
        public string? Remarks { get; set; }
    }
}
