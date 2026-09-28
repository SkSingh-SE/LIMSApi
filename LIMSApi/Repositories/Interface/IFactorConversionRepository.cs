using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Repositories.Interface
{
    public interface IFactorConversionRepository
    {
        Task AddFactor(FactorConversionMaster model);
        Task UpdateFactor(FactorConversionMaster model);
        Task<FactorConversionMaster?> GetFactorById(long id);
        Task<PagedResponse<object>> GetPagedFactors(FactorConversionListRequest request);
        Task<List<DropdwonSelector>> GetFactorDropdown(string? searchTerm, int pageNo, int pageSize);
        Task<bool> ExistsByCode(string code);
        Task<bool> ExistsByCodeAndNotId(string code, long id);
        Task<bool> ExistsCombination(long inputParameterId, string factorType, long? laboratoryTestId, long? methodId, long? versionId);
        Task<bool> ExistsCombinationAndNotId(long id, long inputParameterId, string factorType, long? laboratoryTestId, long? methodId, long? versionId);
        Task<List<FactorConversionMaster>> GetActiveForTest(long laboratoryTestId, long? methodId, List<long> parameterIds);
    }
}
