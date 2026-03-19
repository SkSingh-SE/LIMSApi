using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Services.Interface
{
    public interface ISamplePreparationMasterService
    {
        Task CreateSamplePreparationMaster(SamplePreparationMaster model);
        Task ModifySamplePreparationMaster(SamplePreparationMaster model);
        Task RemoveSamplePreparationMaster(long id);
        Task<SamplePreparationMaster> GetSamplePreparationMasterDetails(long id);
        Task<PagedResponse<object>> FetchSamplePreparationMasterList(PageFilter filter);
        Task<List<DropdwonSelector>> GetSamplePreparationMasterDropdown(string? searchTerm, int pageNo, int pageSize);
        Task<List<SamplePreparationMaster>> GetByLaboratoryTestId(long laboratoryTestId);
    }
}
