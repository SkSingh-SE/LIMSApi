using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Repositories.Interface
{
    public interface IMeasurementUncertaintyRepository
    {
        Task AddUncertainty(MeasurementUncertaintyMaster model);
        Task UpdateUncertainty(MeasurementUncertaintyMaster model);
        Task<MeasurementUncertaintyMaster?> GetUncertaintyById(long id);
        Task<PagedResponse<object>> GetPagedUncertainties(MeasurementUncertaintyListRequest request);
        Task<List<DropdwonSelector>> GetUncertaintyDropdown(string? searchTerm, int pageNo, int pageSize);
        Task<bool> ExistsByCode(string code);
        Task<bool> ExistsByCodeAndNotId(string code, long id);
        Task<bool> ExistsCombination(long? laboratoryTestId, long? parameterId, long? methodId, long? versionId);
        Task<bool> ExistsCombinationAndNotId(long id, long? laboratoryTestId, long? parameterId, long? methodId, long? versionId);
        Task<List<MeasurementUncertaintyMaster>> GetActiveForScope(string companyCode, long? laboratoryTestId, long? methodId, long? versionId);
    }
}
