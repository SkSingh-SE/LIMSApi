using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Services.Interface
{
    public interface IUniversalCodeTypeService
    {
        Task CreateUniversalCodeType(UniversalCodeTypeMaster model);
        Task ModifyUniversalCodeType(UniversalCodeTypeMaster model);
        Task RemoveUniversalCodeType(long id);
        Task<UniversalCodeTypeMaster> GetUniversalCodeTypeDetails(long id);
        Task<PagedResponse<object>> FetchUniversalCodeTypeList(PageFilter filter);

        Task<List<DropdwonSelector>> GetUniversalCodeTypeDropdown(string? searchTerm, int pageNo, int pageSize);
    }
}
