using System.Text.Json;
using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Services.Interface
{
    public interface INablService
    {
        Task<PagedResponse<object>> FetchList(string formType, PageFilter filter);
        Task<object?> GetDetails(string formType, long id);
        Task<object?> GetByDesignationId(string formType, long designationId);
        Task<long> Save(string formType, JsonElement body);
        Task Remove(string formType, long id);
        Task<string> GetNextRegisterNo();
        Task<string> GetNextIndentNo();
        Task<string> GetNextPlanNo();

        // Workflow
        Task Submit(string formType, long id);
        Task Review(string formType, long id);
        Task Approve(string formType, long id);
        Task Reject(string formType, long id, string? remarks);

        // History & Audit
        Task<List<object>> GetRevisionHistory(string formType, long id);
        Task<List<object>> GetAuditLog(string formType, long id);

        // Form Defaults & Reviewers
        Task<object> GetFormDefaults(string formType);
        Task<object> GetSuggestedReviewers();
        Task<List<DropdwonSelector>> GetTraningPlanDropdown(string? searchTerm, int pageNo, int pageSize);
        Task<List<DropdwonSelector>> Roomdropdown(string? searchTerm, int pageNo, int pageSize);
        Task<List<DropdwonSelector>> Supplierlist(string? searchTerm, int pageNo, int pageSize);
        Task<List<DropdwonSelector>> AllSupplierlist(string? searchTerm, int pageNo, int pageSize);
        Task<List<DropdwonSelector>> IndentNoList(string? searchTerm, int pageNo, int pageSize);
        Task<List<DropdwonSelector>> ApprovedSupplierlist(string? searchTerm, int pageNo, int pageSize);
        Task<List<DropdwonSelector>> PlanNoDetailslist(string? searchTerm, int pageNo, int pageSize);
        Task<List<DropdwonSelector>> PONoListDetailslist(string? formType, string? searchTerm, int pageNo, int pageSize);
        Task<SupplierEvaluationDetailsDto> SupplierEvaluationDetails(string supplierName, DateTime? fromDate, DateTime? toDate);
        Task<List<Items>> PoitemsDetails(string poNo, string supplierName);
        Task<List<CombinedPoItemDto>> ReceivedItemsDetails(string poNo, string supplierName);
        Task<List<InspectionParameters>> InspectionPlanDetails(string inspectionPlanNo);
        Task<NablPurchaseIndentDto> IndentDetails(string indentNo);

        Task<UploadFile> UploadSignatureAsync(IFormFile file, CancellationToken cancellationToken = default);
    }
}
