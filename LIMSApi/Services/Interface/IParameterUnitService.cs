using System.Collections.Generic;
using System.Threading.Tasks;
using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Services.Interface
{
    public interface IParameterUnitService
    {
        Task CreateParameterUnit(ParameterUnitCreateDto dto);
        Task ModifyParameterUnit(ParameterUnitUpdateDto dto);
        Task RemoveParameterUnit(long id);
        Task<bool> ToggleParameterUnitStatus(long id);
        Task<ParameterUnitDetailDto> GetParameterUnitDetails(long id);
        Task<PagedResponse<object>> FetchParameterUnitList(PageFilter filter);

        Task<List<DropdwonSelector>> GetParameterUnitDropdown(string? searchTerm, int pageNo, int pageSize);
        Task<List<GroupedUnitDropdownOption>> GetGroupedParameterUnitDropdown(string? searchTerm, int pageNo, int pageSize);
        Task<List<string>> GetQuantityTypes();

        // Selectable unit options for a parameter: base unit + its normalized child equivalents.
        Task<List<EquivalentUnitOption>> GetEquivalentUnits(long unitId);
    }
}
