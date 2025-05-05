using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Repositories.Interface
{
    public interface IParameterUnitRepository
    {
        Task AddParameterUnit(ParameterUnitMaster model);
        Task UpdateParameterUnit(ParameterUnitMaster model);
        Task DeleteParameterUnit(long id);
        Task<ParameterUnitMaster> GetParameterUnitById(long id);
        Task<PagedResponse<object>> GetAllParameterUnits(PageFilter filter);

        Task<List<DropdwonSelector>> GetParameterUnitDropdown(string? searchTerm, int pageNo, int pageSize);
        Task<bool> ExistsByName(string name);
        Task<bool> ExistsByNameAndNotId(string name, long Id);
    }
}
