using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Repositories.Interface
{
    public interface ITestGroupRepository
    {
        Task AddTestGroup(TestGroup model);
        Task UpdateTestGroup(TestGroup model);
        Task DeleteTestGroup(TestGroup model);
        Task<TestGroup> GetTestGroupById(long id);
        Task<PagedResponse<object>> GetAllTestGroups(PageFilter filter);

        Task<List<DropdwonSelector>> GetTestGroupDropdown(string? searchTerm, int pageNo, int pageSize);
        Task<bool> ExistsByName(string name);
        Task<bool> ExistsByNameAndNotId(string name, long Id);
    }
}
