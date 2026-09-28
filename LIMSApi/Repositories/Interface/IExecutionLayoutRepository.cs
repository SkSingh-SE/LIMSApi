using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Repositories.Interface
{
    public interface IExecutionLayoutRepository
    {
        Task AddLayout(ExecutionLayoutMaster model);
        Task UpdateLayout(ExecutionLayoutMaster model);
        Task<ExecutionLayoutMaster?> GetByIdWithStructure(long id);
        Task<ExecutionLayoutMaster?> GetById(long id);
        Task<PagedResponse<object>> GetPagedLayouts(ExecutionLayoutListRequest request);
        Task<List<DropdwonSelector>> GetDropdown(string? searchTerm, int pageNo, int pageSize);
        Task<bool> ExistsByCode(string code);
        Task<bool> ExistsByCodeAndNotId(string code, long id);
        Task<List<ExecutionLayoutMaster>> GetAllActive(string companyCode);
    }
}
