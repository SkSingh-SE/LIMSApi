using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Repositories.Interface
{
    public interface ITestMethodStandardRepository
    {
        Task AddTestMethodStandard(TestMethodStandard model);
        Task UpdateTestMethodStandard(TestMethodStandard model);
        Task DeleteTestMethodStandard(long id);
        Task<TestMethodStandard> GetTestMethodStandardById(long id);
        Task<PagedResponse<object>> GetAllTestMethodStandards(PageFilter filter);

        Task<List<DropdwonSelector>> GetTestMethodStandardDropdown(string? searchTerm, int pageNo, int pageSize);
        Task<bool> ExistsByName(string name);
        Task<bool> ExistsByNameAndNotId(string name, long Id);
    }
}
