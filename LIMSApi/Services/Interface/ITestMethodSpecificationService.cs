using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Services.Interface
{
    public interface ITestMethodSpecificationService
    {
        Task CreateTestMethodSpecification(TestMethodSpecification model);
        Task ModifyTestMethodSpecification(TestMethodSpecification model);
        Task RemoveTestMethodSpecification(long id);
        Task<TestMethodSpecification> GetTestMethodSpecificationDetails(long id);
        Task<PagedResponse<object>> FetchTestMethodSpecificationList(PageFilter filter);

        Task<List<DropdwonSelector>> GetTestMethodSpecificationDropdown(string? searchTerm, int pageNo, int pageSize);
    }
}
