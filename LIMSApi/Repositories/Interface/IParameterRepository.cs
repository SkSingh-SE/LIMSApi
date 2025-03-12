using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Repositories.Interface
{
    public interface IParameterRepository
    {
        Task AddParameter(ParameterMaster model);
        Task UpdateParameter(ParameterMaster model);
        Task DeleteParameter(long id);
        Task<ParameterMaster> GetParameterById(long id);
        Task<PagedResponse<object>> GetAllParameters(PageFilter filter);

        Task<List<DropdwonSelector>> GetParameterDropdown(string? searchTerm, int pageNo, int pageSize);
        Task<bool> ExistsByName(string name);
        Task<bool> ExistsByNameAndNotId(string name, long Id);
    }
}
