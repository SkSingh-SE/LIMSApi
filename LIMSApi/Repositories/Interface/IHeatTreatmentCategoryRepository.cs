using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Repositories.Interface
{
    public interface IHeatTreatmentCategoryRepository
    {
        Task AddHeatTreatmentCategory(HeatTreatmentCategoryMaster model);
        Task UpdateHeatTreatmentCategory(HeatTreatmentCategoryMaster model);
        Task DeleteHeatTreatmentCategory(long id);
        Task<HeatTreatmentCategoryMaster> GetHeatTreatmentCategoryById(long id);
        Task<PagedResponse<object>> GetAllHeatTreatmentCategories(PageFilter filter);

        Task<List<DropdwonSelector>> GetHeatTreatmentCategoryDropdown(string? searchTerm, int pageNo, int pageSize);
        Task<bool> ExistsByName(string name);
        Task<bool> ExistsByNameAndNotId(string name, long Id);
    }
}
