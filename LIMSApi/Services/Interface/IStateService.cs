using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Services.Interface
{
    public interface IStateService
    {
        Task CreateState(StateMaster state);
        Task ModifyState(StateMaster state);
        Task RemoveState(long id);
        Task<StateMaster> GetStateDetails(long id);
        Task<PagedResponse<object>> FetchStates(PageFilter filter);
    }
}
