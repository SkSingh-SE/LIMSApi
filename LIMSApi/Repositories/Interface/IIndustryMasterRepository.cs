using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Repositories.Interface
{
    public interface IIndustryMasterRepository
    {
        Task AddIndustry(IndustryMaster model);
        Task UpdateIndustry(IndustryMaster model);
        Task DeleteIndustry(IndustryMaster model);
        Task<IndustryMaster> GetIndustryById(long id);
        Task<PagedResponse<object>> GetAllIndustrys(PageFilter filter);

        Task<List<DropdwonSelector>> GetIndustryDropdown(string? searchTerm, int pageNo, int pageSize);
        Task<bool> ExistsByName(string name);
        Task<bool> ExistsByNameAndNotId(string name, long Id);
    }
}
