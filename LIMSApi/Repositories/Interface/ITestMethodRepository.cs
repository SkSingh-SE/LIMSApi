using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Repositories.Interface
{
    public interface ITestMethodRepository
    {
        Task AddTestMethod(TestMethodMaster model);
        Task UpdateTestMethod(TestMethodMaster model);
        Task DeleteTestMethod(long id);
        Task<TestMethodMaster> GetTestMethodById(long id);
        Task<PagedResponse<object>> GetAllTestMethods(PageFilter filter);

        Task<List<DropdwonSelector>> GetTestMethodDropdown(string? searchTerm, int pageNo, int pageSize);
        Task<bool> ExistsByName(string name);
        Task<bool> ExistsByNameAndNotId(string name, long Id);
    }
}
