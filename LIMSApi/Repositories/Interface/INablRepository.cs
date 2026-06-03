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

    }
}
