using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Services.Interface
{
    public interface IPropertyTypeService
    {
        Task CreatePropertyType(PropertyTypeMaster model);
        Task ModifyPropertyType(PropertyTypeMaster model);
        Task RemovePropertyType(long id);
        Task<PropertyTypeMaster> GetPropertyTypeDetails(long id);
        Task<PagedResponse<object>> FetchPropertyTypeList(PageFilter filter);
        Task<List<DropdwonSelector>> GetPropertyTypeDropdown(string? searchTerm, int pageNo, int pageSize);
    }
}
