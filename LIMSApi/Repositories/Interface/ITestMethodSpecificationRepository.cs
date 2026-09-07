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
        Task<List<DropdwonSelector>> GetTestMethodSpecificationVersionDropdown(string? searchTerm, int pageNo, int pageSize, long metalId = 0);
        Task<bool> ExistsByOrgAndStandard(long orgId, string testMethodStandard);
        Task<bool> ExistsByOrgAndStandardAndNotId(long orgId, string testMethodStandard, long excludeId);
        Task<List<DropdwonSelector>> GetAllStandardOrganizations();
        Task AddRangeAsync(List<TestMethodSpecification> specs);
        Task UpdateVersionFileRef(long versionId, string filePath, string originalFileName, long uploadRefId);

        // Screen 06: Test Method Master contracts
        Task<TestMethodSpecification?> GetTestMethodEntityById(long id);
        Task<bool> ExistsByCode(string code);
        Task<bool> ExistsByCodeAndNotId(string code, long id);
        Task<PagedResponse<TestMethodListItemDto>> GetTestMethodList(PageFilter filter, string? codeFilter, string? nameFilter, long? techniqueId, string? statusFilter);
        Task<TestMethodDetailDto?> GetTestMethodDetailById(long id);
        Task ToggleTestMethodStatus(long id);
        Task<List<TestMethodDropdownDto>> GetActiveTestMethodDropdown();
    }
}
