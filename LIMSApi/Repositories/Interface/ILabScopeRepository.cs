using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Repositories.Interface
{
    public interface ILabScopeRepository
    {
        Task AddLabScope(LabScopeMaster model);
        Task UpdateLabScope(LabScopeMaster model);
        Task DeleteLabScope(long id);
        Task<LabScopeMaster> GetLabScopeById(long id);
        Task<PagedResponse<object>> GetAllLabScopes(PageFilter filter);

        Task<List<DropdwonSelector>> GetLabScopeDropdown(string? searchTerm, int pageNo, int pageSize);
        Task<bool> ExistsByName(string name);
        Task<bool> ExistsByNameAndNotId(string name, long Id);
    }
}
