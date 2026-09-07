using System.Collections.Generic;
using System.Threading.Tasks;
using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Repositories.Interface
{
    public interface ISpecificationVersionRepository
    {
        Task<PagedResponse<SpecificationVersionListItemDto>> GetPagedVersionsAsync(SpecificationVersionFilterDto filter, string companyCode);
        Task<SpecificationVersion?> GetVersionByIdAsync(long id, string companyCode);
        Task<SpecificationVersion?> GetVersionWithParametersByIdAsync(long id, string companyCode);
        Task<bool> ExistsVersionAsync(long specId, string version, long? excludeId = null);
        Task<SpecificationHeader?> GetParentSpecificationAsync(long specId, string companyCode);
        Task<SpecificationVersion> CreateVersionAsync(SpecificationVersion version);
        Task<SpecificationVersion> UpdateVersionAsync(SpecificationVersion version);
        Task UnsetDefaultVersionsAsync(long specId, long? exceptVersionId = null);
        Task SetDefaultVersionAsync(long specId, long versionId);
        Task<List<SpecificationVersionParameterDto>> GetVersionParametersAsync(long versionId, string companyCode);
        Task SaveVersionParametersAsync(long versionId, List<SpecificationVersionParameter> parameters);
        Task<List<SpecificationVersionDropdownDto>> GetDropdownBySpecificationIdAsync(long specId, string companyCode, bool includeAll = false);
        Task<bool> HasDownstreamReferencesAsync(long versionId);
    }
}
