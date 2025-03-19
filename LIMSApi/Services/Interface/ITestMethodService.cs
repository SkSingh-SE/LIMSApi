using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Services.Interface
{
    public interface ITestMethodService
    {
        Task CreateTestMethod(TestMethodMaster model);
        Task ModifyTestMethod(TestMethodMaster model);
        Task RemoveTestMethod(long id);
        Task<TestMethodMaster> GetTestMethodDetails(long id);
        Task<PagedResponse<object>> FetchTestMethodList(PageFilter filter);

        Task<List<DropdwonSelector>> GetTestMethodDropdown(string? searchTerm, int pageNo, int pageSize);
    }
}
