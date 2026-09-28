using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Repositories.Interface
{
    public interface ILabScopeRepository
    {
        Task AddLabScope(LabScopeMaster model);
        Task UpdateLabScope(LabScopeMaster model);
        Task<LabScopeMaster?> GetLabScopeById(long id);
    }
}
