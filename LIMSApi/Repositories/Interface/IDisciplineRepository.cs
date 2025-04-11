using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Repositories.Interface
{
    public interface IDisciplineRepository
    {
        Task AddDiscipline(DisciplineMaster model);
        Task UpdateDiscipline(DisciplineMaster model);
        Task DeleteDiscipline(DisciplineMaster model);
        Task<DisciplineMaster> GetDisciplineById(long id);
        Task<PagedResponse<object>> GetAllDisciplines(PageFilter filter);

        Task<List<DropdwonSelector>> GetDisciplineDropdown(string? searchTerm, int pageNo, int pageSize);
        Task<bool> ExistsByName(string name);
        Task<bool> ExistsByNameAndNotId(string name, long Id);
    }
}
