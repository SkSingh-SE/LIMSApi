using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Repositories.Interface
{
    public interface INablRepository
    {
        Task<PagedResponse<object>> GetAll(string formType, PageFilter filter);
        Task<object?> GetById(string formType, long id);
        Task<long> Add(string formType, object model);
        Task Update(string formType, object model);
        Task Delete(string formType, long id);
        Task<List<DropdwonSelector>> GetTraningPlanDropdown(string? searchTerm, int pageNo, int pageSize, long? recordId = null);
        Task<List<DropdwonSelector>> Roomdropdown(string? searchTerm, int pageNo, int pageSize);
        Task<List<DropdwonSelector>> Supplierlist(string? searchTerm, int pageNo, int pageSize, long? recordId = null);
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
        Task<NablTestMethodValidationDto> TestMethodDetails(string testmethodCode);
        Task<List<DropdwonSelector>> GetSupplierDropdown(string? searchTerm, int pageNo, int pageSize);
        Task AddQuantityLog(InventoryQuantityLog dto);
        Task<List<DropdwonSelector>> GetMaterialData(string formType, string type);
        Task<InventoryManagementDto> GetInventoryDetails(string itemCode, string itemName);
        Task<List<DropdwonSelector>> GetEmployeesDropdown(string? searchTerm, int pageNo, int pageSize);
        Task<List<DropdwonSelector>> GetReferenceOptions(string? referenceType);
        Task<List<DropdwonSelector>> GetQcplannoDropdown(string? searchTerm, int pageNo, int pageSize);
        Task<RetestingQcPlanDetailsDto?> QCDetails(long id);
        Task<List<DropdwonSelector>> GetCustomerDropdown(string? searchTerm, int pageNo, int pageSize);
        Task<CustomerFeedbackAnalysisDto?> GetFeedbackDetails(long id);
        Task<List<DropdwonSelector>> GetMeetinglist(string? searchTerm, int pageNo, int pageSize);
        Task<MeetingAgendaDto> GetMeetingDetails(string meetingNo);
        Task<List<PurchaseMaterialVerificationPrintDto>> GetPurchaseMaterialVerificationPrintList();
        Task<List<DropdwonSelector>> Documentlist(string? searchTerm, int pageNo, int pageSize);
        Task<PagedResponse<object>> NcPrintList(PageFilter filter);
        Task<List<DropdwonSelector>> GetAuditorsDropdown(string? searchTerm, int pageNo, int pageSize);
        Task<List<DropdwonSelector>> GetEligibleAuditors(long departmentId, string isoClauseIds, DateTime scheduleDate);
        Task<AuditChecklistDto> GetScheduleSession(long scheduleItemId);
        Task<AuditChecklistNcrDto> GetAuditChecklistNcr(long checklistItemId);
        Task<AuditSummaryDto> GetAuditplan(long auditPlanId);
        Task<List<DropdwonSelector>> GetDocumentsAvailableForReview(string? searchTerm, int pageNo, int pageSize);
        Task<List<MasterDocumentPrintDto>> GetMasterDocumentPrintList();
        Task<List<DropdwonSelector>> GetCompetencereportDropdown(string formType, string? searchTerm, int pageNo, int pageSize, long? recordId = null);
        Task<List<DropdwonSelector>> GetDesignationDetails(string formType, string? searchTerm, int pageNo, int pageSize, long? recordId = null);
        Task<RoleWiseSkill> GetEmployeerolewiseskillsDetails(int employeeId);
        Task<List<DropdwonSelector>> GetRolewiseDesignationsDropdown(string? searchTerm, int pageNo, int pageSize, long? roleWiseSkillsMasterId = null);
        Task<SkillMatrixEmployeeCheckDto> GetcheckSkillmatrixEmployeeDetails(int employeeId);
        Task<List<DropdwonSelector>> Equipmentslist(string? searchTerm, int pageNo, int pageSize, long? recordId = null);
        Task<EquipmentDetailsDto> GetEquipmentDetails(int equipmentId);
        Task<string> GetEmployeeDesignationDetails(int employeeId);
        Task<List<DropdwonSelector>> GetQuestionsetsDropdown(string? searchTerm, int pageNo, int pageSize);
        Task<TrainingPlanDetailsDto> GetTrainingPlanDetails(int planId);
        Task<TrainingEffectivenessDto> EvaluationQuestionDetails(long attendanceId, long trainingPlanId, long questionSetId);
        Task<List<DropdwonSelector>> GetEmployeeDropdown(string formType, string? searchTerm, int pageNo, int pageSize, long? recordId = null, long? trainingPlanId = null);
        Task<TrainingAttendancePrintDto?> GettrainingAttendanceDetails(long id);

    }
}
