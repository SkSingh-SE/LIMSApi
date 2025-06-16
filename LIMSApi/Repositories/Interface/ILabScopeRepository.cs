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

    }
}
