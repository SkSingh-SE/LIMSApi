using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;

namespace LIMSApi.Services
{
    public class SamplePreparationMasterService : ISamplePreparationMasterService
    {
        private readonly ISamplePreparationMasterRepository _repository;
        private readonly ILogger<SamplePreparationMasterService> _logger;
        private LoggedInUserDTO loggedInUser;

        public SamplePreparationMasterService(ISamplePreparationMasterRepository repository, ILogger<SamplePreparationMasterService> logger)
        {
            _repository = repository;
            _logger = logger;
            loggedInUser = LoggedInUserProvider.CurrentUser;
        }

        public async Task CreateSamplePreparationMaster(SamplePreparationMaster model)
        {
            if (string.IsNullOrWhiteSpace(model.SpecimenType))
                throw new ArgumentException("Specimen Type should not be empty!");

            bool exists = await _repository.ExistsBySpecimenAndDimensions(model.SpecimenType, model.Dimensions, model.TestMethodStandardID, model.LaboratoryTestID);
            if (exists)
                throw new InvalidOperationException("Sample Preparation Master with same Specimen Type, Dimensions, Test Method Standard, and Laboratory Test already exists!");

            model.CreatedOn = DateTime.UtcNow;
            model.CreatedBy = loggedInUser.EmployeeID;
            model.CompanyCode = loggedInUser.CompanyCode;

            await _repository.AddSamplePreparationMaster(model);
            _logger.LogInformation("SamplePreparationMaster '{SpecimenType}' created successfully.", model.SpecimenType);
        }

        public async Task ModifySamplePreparationMaster(SamplePreparationMaster model)
        {
            if (model.ID == 0)
                throw new ArgumentException("SamplePreparationMaster ID should not be empty!");

            bool exists = await _repository.ExistsBySpecimenAndDimensionsAndNotId(model.SpecimenType, model.Dimensions, model.TestMethodStandardID, model.LaboratoryTestID, model.ID);
            if (exists)
                throw new InvalidOperationException("Sample Preparation Master with same Specimen Type, Dimensions, Test Method Standard, and Laboratory Test already exists!");

            var existing = await _repository.GetSamplePreparationMasterById(model.ID);
            if (existing == null)
                throw new InvalidOperationException("SamplePreparationMaster not found!");

            existing.TestMethodStandardID = model.TestMethodStandardID;
            existing.LaboratoryTestID = model.LaboratoryTestID;
            existing.SpecimenType = model.SpecimenType;
            existing.Dimensions = model.Dimensions;
            existing.MaterialType = model.MaterialType;
            existing.Charges = model.Charges;
            existing.Remark = model.Remark;
            existing.ModifiedOn = DateTime.UtcNow;
            existing.ModifiedBy = loggedInUser.EmployeeID;

            await _repository.UpdateSamplePreparationMaster(existing);
            _logger.LogInformation("SamplePreparationMaster '{SpecimenType}' updated successfully.", model.SpecimenType);
        }

        public async Task RemoveSamplePreparationMaster(long id)
        {
            var existing = await _repository.GetSamplePreparationMasterById(id);
            if (existing == null)
                throw new InvalidOperationException("SamplePreparationMaster not found!");

            existing.IsActive = false;
            existing.ModifiedOn = DateTime.UtcNow;
            existing.ModifiedBy = loggedInUser.EmployeeID;

            await _repository.DeleteSamplePreparationMaster(existing);
            _logger.LogInformation("SamplePreparationMaster with ID '{Id}' deleted successfully.", id);
        }

        public async Task<SamplePreparationMaster> GetSamplePreparationMasterDetails(long id)
        {
            var entity = await _repository.GetSamplePreparationMasterById(id);
            if (entity == null)
                throw new InvalidOperationException("SamplePreparationMaster not found!");

            return entity;
        }

        public async Task<PagedResponse<object>> FetchSamplePreparationMasterList(PageFilter filter)
        {
            return await _repository.GetAllSamplePreparationMasters(filter);
        }

        public async Task<List<DropdwonSelector>> GetSamplePreparationMasterDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            return await _repository.GetSamplePreparationMasterDropdown(searchTerm, pageNo, pageSize);
        }

        public async Task<List<SamplePreparationMaster>> GetByLaboratoryTestId(long laboratoryTestId)
        {
            return await _repository.GetByLaboratoryTestId(laboratoryTestId);
        }
    }
}
