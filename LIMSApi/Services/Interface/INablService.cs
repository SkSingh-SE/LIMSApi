using System.Text.Json;
using LIMSApi.Dtos;
using LIMSApi.Migrations;
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
        Task<string> GetNextMaterialNo();

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
        Task<List<DropdwonSelector>> Alltestmethodlist(string formType, string? searchTerm, int pageNo, int pageSize);
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
        Task<NablTestMethodValidationDto> TestMethodDetails(string testmethodCode);
        Task<List<DropdwonSelector>> GetSupplierDropdown(string? searchTerm, int pageNo, int pageSize);
        Task<InventoryQuantityLog> Addquantity(string formType, JsonElement body);
        Task<List<InventoryQuantityLog?>> GetQuantityLogs(string formType, long inventoryId);
        Task<List<DropdwonSelector>> GetMaterialData(string formType, string type);
        Task<InventoryManagementDto> GetInventoryDetails(string itemCode, string itemName);
        Task<List<DropdwonSelector>> GetEmployeesDropdown(string? searchTerm, int pageNo, int pageSize);
        Task<List<DropdwonSelector>> GetReferenceOptions(string referenceType);
        //Task<ReferenceMaterialConsumptionLog> AddConsumption(string formType, JsonElement body);
        Task<string> GetNextQCPlanNo();
        Task<List<DropdwonSelector>> GetQcplannoDropdown(string? searchTerm, int pageNo, int pageSize);
        Task<RetestingQcPlanDetailsDto?> QCDetails(long id);
        Task<List<DropdwonSelector>> GetCustomerDropdown(string? searchTerm, int pageNo, int pageSize);
        Task<CustomerFeedbackAnalysisDto?> GetFeedbackDetails(long id);
        Task<string> GetNextAnalysisNo();
        Task<string> GetNextMeetingNo();
        Task<List<DropdwonSelector>> GetMeetinglist(string? searchTerm, int pageNo, int pageSize);
        Task<MeetingAgendaDto?> GetMeetingDetails(string meetingNo);
        Task<List<PurchaseMaterialVerificationPrintDto>> GetPurchaseMaterialVerificationPrintList();
        Task<string> GetNextNCNo();
        Task<string> GetNextActionNo();
        Task<PagedResponse<object>> NcPrintList(PageFilter filter);
        Task<string> GetNextMUNo();
        Task<long> SaveMasterDocument(JsonElement body, IFormFile? file);
        Task<List<DropdwonSelector>> Documentlist(string? searchTerm, int pageNo, int pageSize);
        Task<string> GetNextrequestNo();
        Task<NablDocumentReview> GetDocumentReviewById(long id);
        Task<NablDocumentChangeRequest> GetDocumentChangeRequestById(long id);
        Task<string> GetNextreviewNo();
        Task<List<DropdwonSelector>> GetAuditorsDropdown(string? searchTerm, int pageNo, int pageSize);
        Task<string> GetNextAuditPlanNo();
        Task<List<DropdwonSelector>> GetEligibleAuditors(long departmentId, string isoClauseIds, DateTime scheduleDate);
        Task<AuditChecklistDto> GetScheduleSession(long scheduleItemId);
        Task<NablAuditChecklist?> GetAuditChecklistById(long id);
        Task<AuditChecklistNcrDto> GetAuditChecklistNcr(long checklistItemId);
        Task<string> GetNextChecklistNo();
        Task<AuditSummaryDto> GetAuditplan(long auditPlanId);
        Task<List<DropdwonSelector>> GetDocumentsAvailableForReview(string? searchTerm, int pageNo, int pageSize);
        Task<List<MasterDocumentPrintDto>> GetMasterDocumentPrintList();
    }
}
