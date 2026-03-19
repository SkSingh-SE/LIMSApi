using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;

namespace LIMSApi.Services
{
    public class SpecimenOrientationService : ISpecimenOrientationService
    {
        private readonly ISpecimenOrientationRepository _specimenRepository;
        private readonly ILogger<SpecimenOrientationService> _logger;

        public SpecimenOrientationService(ISpecimenOrientationRepository specimenRepository, ILogger<SpecimenOrientationService> logger)
        {
            _specimenRepository = specimenRepository;
            _logger = logger;
        }

        public async Task CreateSpecimenOrientation(SpecimenOrientationMaster model)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
                throw new ArgumentException("SpecimenOrientation name should not be empty!");

            bool exists = await _specimenRepository.ExistsByName(model.Name);
            if (exists)
                throw new InvalidOperationException("SpecimenOrientation already exists!");

            await _specimenRepository.AddSpecimenOrientation(model);
            _logger.LogInformation("SpecimenOrientation '{SpecimenOrientationName}' created successfully.", model.Name);
        }

        public async Task ModifySpecimenOrientation(SpecimenOrientationMaster model)
        {
            if (model.ID == 0)
                throw new ArgumentException("SpecimenOrientation ID should not be empty!");

            bool exists = await _specimenRepository.ExistsByNameAndNotId(model.Name, model.ID);
            if (exists)
                throw new InvalidOperationException("Same SpecimenOrientation already exists!");

            var existingSpecimenOrientation = await _specimenRepository.GetSpecimenOrientationById(model.ID);
            if (existingSpecimenOrientation == null)
                throw new InvalidOperationException("SpecimenOrientation not found!");

            existingSpecimenOrientation.Name = model.Name;
            existingSpecimenOrientation.Code = model.Code;
            existingSpecimenOrientation.SpecimenOrientationCategoryID = model.SpecimenOrientationCategoryID;
            existingSpecimenOrientation.Description = model.Description;

            // Update ApplicableForms junction
            existingSpecimenOrientation.ApplicableForms?.Clear();
            if (model.ApplicableForms != null)
                foreach (var form in model.ApplicableForms)
                    existingSpecimenOrientation.ApplicableForms.Add(form);

            // Update ApplicableClassifications junction
            existingSpecimenOrientation.ApplicableClassifications?.Clear();
            if (model.ApplicableClassifications != null)
                foreach (var cls in model.ApplicableClassifications)
                    existingSpecimenOrientation.ApplicableClassifications.Add(cls);

            existingSpecimenOrientation.ModifiedOn = DateTime.UtcNow;

            await _specimenRepository.UpdateSpecimenOrientation(existingSpecimenOrientation);
            _logger.LogInformation("SpecimenOrientation '{SpecimenOrientationName}' updated successfully.", model.Name);
        }

        public async Task RemoveSpecimenOrientation(long id)
        {
            var existingSpecimenOrientation = await _specimenRepository.GetSpecimenOrientationById(id);
            if (existingSpecimenOrientation == null)
                throw new InvalidOperationException("SpecimenOrientation not found!");

            existingSpecimenOrientation.IsActive = false;
            existingSpecimenOrientation.ModifiedOn = DateTime.UtcNow;

            await _specimenRepository.UpdateSpecimenOrientation(existingSpecimenOrientation);
            _logger.LogInformation("SpecimenOrientation with ID '{SpecimenOrientationId}' deleted successfully.", id);
        }

        public async Task<SpecimenOrientationMaster> GetSpecimenOrientationDetails(long id)
        {
            var classification = await _specimenRepository.GetSpecimenOrientationById(id);
            if (classification == null)
                throw new InvalidOperationException("SpecimenOrientation not found!");

            return classification;
        }

        public async Task<PagedResponse<object>> FetchSpecimenOrientationList(PageFilter filter)
        {
            return await _specimenRepository.GetAllSpecimenOrientations(filter);
        }

        public async Task<List<DropdwonSelector>> GetSpecimenOrientationDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            return await _specimenRepository.GetSpecimenOrientationDropdown(searchTerm, pageNo, pageSize);
        }
    }
}
