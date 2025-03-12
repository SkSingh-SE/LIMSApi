using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Repositories.Interface
{
    public interface IUniversalCodeTypeRepository
    {
        Task AddUniversalCodeType(UniversalCodeTypeMaster model);
        Task UpdateUniversalCodeType(UniversalCodeTypeMaster model);
        Task DeleteUniversalCodeType(long id);
        Task<UniversalCodeTypeMaster> GetUniversalCodeTypeById(long id);
        Task<PagedResponse<object>> GetAllUniversalCodeTypes(PageFilter filter);

        Task<List<DropdwonSelector>> GetUniversalCodeTypeDropdown(string? searchTerm, int pageNo, int pageSize);
        Task<bool> ExistsByName(string name);
        Task<bool> ExistsByNameAndNotId(string name, long id);
    }
}
