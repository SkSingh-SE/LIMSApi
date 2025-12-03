using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Services.Interface
{
    public interface ILaboratoryTestService
    {
        Task CreateTestMethod(LaboratoryTest model);
        Task ModifyTestMethod(LaboratoryTest model);
        Task RemoveTestMethod(long id);
        Task<LaboratoryTest> GetTestMethodDetails(long id);
        Task<PagedResponse<object>> FetchTestMethodList(PageFilter filter);

        Task<List<DropdwonSelector>> GetTestMethodDropdown(string? searchTerm, int pageNo, int pageSize);
        Task<List<object>> GetTestCases(long labTestId);
    }
}
