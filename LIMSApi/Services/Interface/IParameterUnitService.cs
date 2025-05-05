using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Services.Interface
{
    public interface IParameterUnitService
    {
        Task CreateParameterUnit(ParameterUnitMaster model);
        Task ModifyParameterUnit(ParameterUnitMaster model);
        Task RemoveParameterUnit(long id);
        Task<ParameterUnitMaster> GetParameterUnitDetails(long id);
        Task<PagedResponse<object>> FetchParameterUnitList(PageFilter filter);

        Task<List<DropdwonSelector>> GetParameterUnitDropdown(string? searchTerm, int pageNo, int pageSize);
    }
}
