using System.Collections.Generic;
using System.Threading.Tasks;
using LIMSApi.Dtos;

namespace LIMSApi.Services.Interface
{
    public interface ISpecificationRequirementService
    {
        Task<SpecificationRequirementContextDto> GetContextAsync(long specId, long versionId, long gradeId, string companyCode);
        Task<List<SpecificationRequirementItemDto>> GetRequirementsAsync(long versionId, long gradeId, string companyCode);
        Task<SpecificationRequirementItemDto?> GetRequirementByIdAsync(long id, string companyCode);
        Task<long> CreateRequirementAsync(SaveSpecificationRequirementDto dto, string companyCode, long userId);
        Task UpdateRequirementAsync(long id, SaveSpecificationRequirementDto dto, string companyCode, long userId);
        Task DeleteRequirementAsync(long id, string companyCode);
        Task<int> CopyVersionRequirementsAsync(CopyVersionRequirementsDto dto, string companyCode, long userId);
        Task<bool> ActivateSpecificationVersionAsync(long versionId, string companyCode, long userId);
    }
}
