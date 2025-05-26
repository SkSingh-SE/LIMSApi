using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Repositories.Interface
{
    public interface ITestMethodSpecificationRepository
    {
        Task AddTestMethodSpecification(TestMethodSpecification model);
        Task UpdateTestMethodSpecification(TestMethodSpecification model);
        Task DeleteTestMethodSpecification(long id);
        Task<TestMethodSpecification> GetTestMethodSpecificationById(long id);
        Task<PagedResponse<object>> GetAllTestMethodSpecifications(PageFilter filter);

        Task<List<DropdwonSelector>> GetTestMethodSpecificationDropdown(string? searchTerm, int pageNo, int pageSize);
        Task<bool> ExistsByName(string name);
        Task<bool> ExistsByNameAndNotId(string name, long Id);
    }
}
