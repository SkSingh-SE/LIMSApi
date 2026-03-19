using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Services.Interface
{
    public interface IProductTestGroupService
    {
        Task CreateProductTestGroup(ProductTestGroup model);
        Task ModifyProductTestGroup(ProductTestGroup model);
        Task RemoveProductTestGroup(long id);
        Task<ProductTestGroup> GetProductTestGroupDetails(long id);
        Task<PagedResponse<object>> FetchProductTestGroupList(PageFilter filter);
        Task<List<DropdwonSelector>> GetProductTestGroupDropdown(string? searchTerm, int pageNo, int pageSize);
        Task<List<object>> GetByProductSpecificationId(long productSpecId);
    }
}
