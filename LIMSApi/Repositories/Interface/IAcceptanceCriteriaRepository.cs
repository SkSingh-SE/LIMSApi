using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Repositories.Interface
{
    public interface IAcceptanceCriteriaRepository
    {
        Task AddAcceptanceCriteria(AcceptanceCriteriaMaster model);
        Task UpdateAcceptanceCriteria(AcceptanceCriteriaMaster model);
        Task<AcceptanceCriteriaMaster?> GetAcceptanceCriteriaById(long id);
        Task<PagedResponse<object>> GetAllAcceptanceCriteria(PageFilter filter);
        Task<List<DropdwonSelector>> GetAcceptanceCriteriaDropdown(string? searchTerm, int pageNo, int pageSize);
        Task<bool> ExistsByCode(string code);
        Task<bool> ExistsByCodeAndNotId(string code, long id);
        Task<bool> ExistsByName(string name);
        Task<bool> ExistsByNameAndNotId(string name, long id);
    }
}
