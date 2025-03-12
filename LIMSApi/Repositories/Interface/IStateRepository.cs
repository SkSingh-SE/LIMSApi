using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Repositories.Interface
{
    public interface IStateRepository
    {
        Task AddState(StateMaster state);
        Task UpdateState(StateMaster state);
        Task DeleteState(long id);
        Task<StateMaster> GetStateById(long id);
        Task<PagedResponse<object>> GetAllStates(PageFilter filter);
        Task<bool> ExistsByName(string name);
        Task<bool> ExistsByNameAndNotId(string name, long id);
    }
}
