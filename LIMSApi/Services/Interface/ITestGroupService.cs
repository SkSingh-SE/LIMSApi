using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Services.Interface
{
    public interface ITestGroupService
    {
        Task CreateTestGroup(TestGroup model);
        Task ModifyTestGroup(TestGroup model);
        Task RemoveTestGroup(long id);
        Task<TestGroup> GetTestGroupDetails(long id);
        Task<PagedResponse<object>> FetchTestGroupList(PageFilter filter);

        Task<List<DropdwonSelector>> GetTestGroupDropdown(string? searchTerm, int pageNo, int pageSize);
    }
}
