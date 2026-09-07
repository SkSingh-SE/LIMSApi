using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Repositories.Interface
{
    public interface ITestMethodVersionRepository
    {
        Task<PagedResponse<TestMethodVersionListItemDto>> GetPagedVersionsAsync(TestMethodVersionFilterDto filter, string companyCode);
        Task<TestMethodSpecificationVersion?> GetVersionByIdAsync(long id, string companyCode);
        Task<TestMethodSpecificationVersion?> GetVersionWithParametersByIdAsync(long id, string companyCode);
        Task<bool> ExistsVersionAsync(long methodId, string version, long? excludeId = null);
        Task<TestMethodSpecification?> GetParentMethodAsync(long methodId, string companyCode);
        Task<TestMethodSpecificationVersion> CreateVersionAsync(TestMethodSpecificationVersion version);
        Task<TestMethodSpecificationVersion> UpdateVersionAsync(TestMethodSpecificationVersion version);
        Task UnsetDefaultVersionsAsync(long methodId, long? exceptVersionId = null);
        Task SetDefaultVersionAsync(long methodId, long versionId);
        Task<List<TestMethodVersionParameterDto>> GetVersionParametersAsync(long versionId, string companyCode);
        Task SaveVersionParametersAsync(long versionId, List<TestMethodSpecificationParameter> parameters);
        Task<List<TestMethodVersionDropdownDto>> GetDropdownByMethodIdAsync(long methodId, string companyCode);
    }
}
