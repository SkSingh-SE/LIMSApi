using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Repositories.Interface
{
    public interface ILaboratoryTestLayoutRepository
    {
        Task<List<LaboratoryTestLayout>> GetByTestAsync(long testId, string companyCode);
        Task<LaboratoryTestLayout?> GetByIdAsync(long id, string companyCode);
        Task AddAsync(LaboratoryTestLayout model);
        Task UpdateAsync(LaboratoryTestLayout model);
        Task<bool> ExistsDuplicateAsync(long testId, long layoutId, long? methodId, long? versionId, long excludeId, string companyCode);
        Task<bool> ExistsDefaultInScopeAsync(long testId, long? methodId, long? versionId, long excludeId, string companyCode);
    }
}
