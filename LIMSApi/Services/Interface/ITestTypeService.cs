using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Services.Interface
{
    public interface ITestTypeService
    {
        Task CreateTestType(TestTypeMaster model);
        Task ModifyTestType(TestTypeMaster model);
        Task RemoveTestType(long id);
        Task<TestTypeMaster> GetTestTypeDetails(long id);
        Task<PagedResponse<object>> FetchTestTypeList(PageFilter filter);

        Task<List<DropdwonSelector>> GetTestTypeDropdown(string? searchTerm, int pageNo, int pageSize);
    }
}
