using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Services.Interface
{
    public interface IHeatTreatmentCategoryService
    {
        Task CreateHeatTreatmentCategory(HeatTreatmentCategoryMaster model);
        Task ModifyHeatTreatmentCategory(HeatTreatmentCategoryMaster model);
        Task RemoveHeatTreatmentCategory(long id);
        Task<HeatTreatmentCategoryMaster> GetHeatTreatmentCategoryDetails(long id);
        Task<PagedResponse<object>> FetchHeatTreatmentCategoryList(PageFilter filter);

        Task<List<DropdwonSelector>> GetHeatTreatmentCategoryDropdown(string? searchTerm, int pageNo, int pageSize);
    }
}
