using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Repositories.Interface
{
    public interface ITestTypeRepository
    {
        Task AddTestType(TestTypeMaster model);
        Task UpdateTestType(TestTypeMaster model);
        Task DeleteTestType(long id);
        Task<TestTypeMaster> GetTestTypeById(long id);
        Task<PagedResponse<object>> GetAllTestTypes(PageFilter filter);

        Task<List<DropdwonSelector>> GetTestTypeDropdown(string? searchTerm, int pageNo, int pageSize);
        Task<bool> ExistsByName(string name);
        Task<bool> ExistsByNameAndNotId(string name, long Id);
    }
}
