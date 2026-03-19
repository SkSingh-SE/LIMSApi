using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Repositories.Interface
{
    public interface ISamplePreparationMasterRepository
    {
        Task AddSamplePreparationMaster(SamplePreparationMaster model);
        Task UpdateSamplePreparationMaster(SamplePreparationMaster model);
        Task DeleteSamplePreparationMaster(SamplePreparationMaster model);
        Task<SamplePreparationMaster?> GetSamplePreparationMasterById(long id);
        Task<PagedResponse<object>> GetAllSamplePreparationMasters(PageFilter filter);
        Task<List<DropdwonSelector>> GetSamplePreparationMasterDropdown(string? searchTerm, int pageNo, int pageSize);
        Task<bool> ExistsBySpecimenAndDimensions(string specimenType, string? dimensions, long? testMethodStandardID, long? laboratoryTestID);
        Task<bool> ExistsBySpecimenAndDimensionsAndNotId(string specimenType, string? dimensions, long? testMethodStandardID, long? laboratoryTestID, long id);
        Task<List<SamplePreparationMaster>> GetByLaboratoryTestId(long laboratoryTestId);
    }
}
