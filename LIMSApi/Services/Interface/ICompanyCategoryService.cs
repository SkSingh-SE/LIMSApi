using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Services.Interface
{
    public interface ICompanyCategoryService
    {
        Task CreateCustomerType(CompanyCategoryMaster model);
        Task ModifyCustomerType(CompanyCategoryMaster model);
        Task RemoveCustomerType(long id);
        Task<CompanyCategoryMaster> GetCustomerTypeDetails(long id);
        Task<PagedResponse<object>> FetchCustomerTypeList(PageFilter filter);

        Task<List<DropdwonSelector>> GetCustomerTypeDropdown(string? searchTerm, int pageNo, int pageSize);
    }
}
