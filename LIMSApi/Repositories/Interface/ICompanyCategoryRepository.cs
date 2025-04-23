using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Repositories.Interface
{
    public interface ICompanyCategoryRepository
    {
        Task AddCustomerType(CompanyCategoryMaster model);
        Task UpdateCustomerType(CompanyCategoryMaster model);
        Task DeleteCustomerType(CompanyCategoryMaster model);
        Task<CompanyCategoryMaster> GetCustomerTypeById(long id);
        Task<PagedResponse<object>> GetAllCustomerTypes(PageFilter filter);

        Task<List<DropdwonSelector>> GetCustomerTypeDropdown(string? searchTerm, int pageNo, int pageSize);
        Task<bool> ExistsByName(string name);
        Task<bool> ExistsByNameAndNotId(string name, long Id);
    }
}
