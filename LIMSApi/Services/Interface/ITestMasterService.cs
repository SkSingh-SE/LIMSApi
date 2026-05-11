using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Services.Interface
{
    public interface ITestMasterService
    {
        Task CreateTestMaster(TestMaster model);
        Task ModifyTestMaster(TestMaster model);
        Task RemoveTestMaster(long id);
        Task<TestMaster> GetTestMasterDetails(long id);
        Task<PagedResponse<object>> FetchTestMasterList(PageFilter filter);

        Task<List<DropdwonSelector>> GetTestMasterDropdown(string? searchTerm, int pageNo, int pageSize);
        Task<List<DropdwonSelector>> GetGeneralTestMasterDropdown(string? searchTerm, int pageNo, int pageSize);
        Task<List<DropdwonSelector>> GetChemicalTestMasterDropdown(string? searchTerm, int pageNo, int pageSize);
    }
}
