using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Services.Interface
{
    public interface ILabScopeService
    {
        Task CreateLabScope(LabScopeMaster model);
        Task ModifyLabScope(LabScopeMaster model);
        Task RemoveLabScope(long id);
        Task<LabScopeMaster> GetLabScopeDetails(long id);
        Task<PagedResponse<object>> FetchLabScopeList(PageFilter filter);

    }
}
