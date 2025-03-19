using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Services.Interface
{
    public interface ITestMethodStandardService
    {
        Task CreateTestMethodStandard(TestMethodStandard model);
        Task ModifyTestMethodStandard(TestMethodStandard model);
        Task RemoveTestMethodStandard(long id);
        Task<TestMethodStandard> GetTestMethodStandardDetails(long id);
        Task<PagedResponse<object>> FetchTestMethodStandardList(PageFilter filter);

        Task<List<DropdwonSelector>> GetTestMethodStandardDropdown(string? searchTerm, int pageNo, int pageSize);
    }
}
