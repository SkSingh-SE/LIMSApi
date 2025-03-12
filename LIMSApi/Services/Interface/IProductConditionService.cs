using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Services.Interface
{
    public interface IProductConditionService
    {
        Task CreateProductCondition(ProductConditionMaster model);
        Task ModifyProductCondition(ProductConditionMaster model);
        Task RemoveProductCondition(long id);
        Task<ProductConditionMaster> GetProductConditionDetails(long id);
        Task<PagedResponse<object>> FetchProductConditionList(PageFilter filter);

        Task<List<DropdwonSelector>> GetProductConditionDropdown(string? searchTerm, int pageNo, int pageSize);
    }
}
