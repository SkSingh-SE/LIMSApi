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
        Task<List<DropdwonSelector>> GetTestMethodSpecificationsByStandard(long standardId);
        Task<List<DropdwonSelector>> GetTestMethodsByMetalClassification(long metalClassificationId, string? searchTerm, int pageNo, int pageSize);
        Task<int> GetVersionImpactCount(long versionId);
        Task<List<TestMethodSpecificationVersion>> GetVersionsDueForReview(DateTime cutoffDate);
        Task<List<DropdwonSelector>> GetVersionsBySpecId(long specId, bool includeAll = false);
    }
}
