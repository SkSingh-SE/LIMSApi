using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Repositories.Interface
{
    public interface ITPIMasterRepository
    {
        Task AddTPI(TPIMaster model);
        Task UpdateTPI(TPIMaster model);
        Task DeleteTPI(TPIMaster model);
        Task<TPIMaster> GetTPIById(long id);
        Task<PagedResponse<object>> GetAllTPIs(PageFilter filter);

        Task<List<DropdwonSelector>> GetTPIDropdown(string? searchTerm, int pageNo, int pageSize);
        Task<bool> ExistsByName(string name);
        Task<bool> ExistsByNameAndNotId(string name, long Id);
    }
}
