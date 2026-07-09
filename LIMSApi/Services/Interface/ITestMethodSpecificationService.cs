using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Services.Interface
{
    public interface ITestMethodSpecificationService
    {
        Task CreateTestMethodSpecification(TestMethodSpecification model);
        Task ModifyTestMethodSpecification(TestMethodSpecification model);
        Task RemoveTestMethodSpecification(long id);
        Task EnableDisableTestMethodSpecification(long id);
        Task<TestMethodSpecification> GetTestMethodSpecificationDetails(long id);
        Task<PagedResponse<object>> FetchTestMethodSpecificationList(PageFilter filter);

        Task<List<DropdwonSelector>> GetTestMethodSpecificationDropdown(string? searchTerm, int pageNo, int pageSize);
        Task<List<DropdwonSelector>> GetTestMethodsByStandard(long standardId);
        Task<List<DropdwonSelector>> GetTestMethodsByMetalClassification(long metalClassificationId, string? searchTerm, int pageNo, int pageSize);
        Task ActivateVersion(long specId, long versionId);
        Task SetDefaultVersion(long specId, long versionId);
        Task WithdrawVersion(long specId, long versionId, string reason);
        Task<int> GetVersionImpactCount(long versionId);
        Task<List<DropdwonSelector>> GetVersionsBySpecId(long specId, bool includeAll = false);
        Task<List<DropdwonSelector>> GetTestMethodSpecificationVersionDropdown(string? searchTerm, int pageNo, int pageSize);
    }
}
