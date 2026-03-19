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
        Task<PagedResponse<object>> GetAllChemicalParameters(PageFilter filter);
        Task<PagedResponse<object>> GetAllMechanicalParameters(PageFilter filter);
        Task<PagedResponse<object>> ParameterList(PageFilter filter);

        Task<List<DropdwonSelector>> GetParameterDropdown(string? searchTerm, int pageNo, int pageSize);
        Task<List<DropdwonSelector>> GetChemicalParameterDropdown(string? searchTerm, int pageNo, int pageSize);
        Task<List<DropdwonSelector>> GetMechanicalParameterDropdown(string? searchTerm, int pageNo, int pageSize);
        Task<bool> ExistsByName(string name);
        Task<bool> ExistsByNameAndNotId(string name, long Id);
        Task<bool> ExistsByCode(string code);
        Task<bool> ExistsByCodeAndNotId(string code, long Id);
    }
}
