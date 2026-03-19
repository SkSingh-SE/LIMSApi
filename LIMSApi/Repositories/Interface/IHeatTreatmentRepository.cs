using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Repositories.Interface
{
    public interface IHeatTreatmentRepository
    {
        Task AddHeatTreatment(HeatTreatmentMaster model);
        Task UpdateHeatTreatment(HeatTreatmentMaster model);
        Task DeleteHeatTreatment(long id);
        Task<HeatTreatmentMaster> GetHeatTreatmentById(long id);
        Task<PagedResponse<object>> GetAllHeatTreatments(PageFilter filter);

        Task<List<DropdwonSelector>> GetHeatTreatmentDropdown(string? searchTerm, int pageNo, int pageSize);
        Task<bool> ExistsByName(string name);
        Task<bool> ExistsByNameAndNotId(string name, long Id);
        Task<bool> ExistsByCode(string code);
        Task<bool> ExistsByCodeAndNotId(string code, long Id);
    }
}
