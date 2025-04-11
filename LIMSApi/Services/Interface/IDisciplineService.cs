using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Services.Interface
{
    public interface IDisciplineService
    {
        Task CreateDiscipline(DisciplineMaster model);
        Task ModifyDiscipline(DisciplineMaster model);
        Task RemoveDiscipline(long id);
        Task<DisciplineMaster> GetDisciplineDetails(long id);
        Task<PagedResponse<object>> FetchDisciplineList(PageFilter filter);

        Task<List<DropdwonSelector>> GetDisciplineDropdown(string? searchTerm, int pageNo, int pageSize);
    }
}
