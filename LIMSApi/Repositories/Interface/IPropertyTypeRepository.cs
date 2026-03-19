using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Repositories.Interface
{
    public interface IPropertyTypeRepository
    {
        Task AddPropertyType(PropertyTypeMaster model);
        Task UpdatePropertyType(PropertyTypeMaster model);
        Task DeletePropertyType(long id);
        Task<PropertyTypeMaster> GetPropertyTypeById(long id);
        Task<PagedResponse<object>> GetAllPropertyTypes(PageFilter filter);
        Task<List<DropdwonSelector>> GetPropertyTypeDropdown(string? searchTerm, int pageNo, int pageSize);
        Task<bool> ExistsByName(string name);
        Task<bool> ExistsByNameAndNotId(string name, long Id);
    }
}
