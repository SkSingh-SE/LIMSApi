using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;

namespace LIMSApi.Services
{
    public class HeatTreatmentService : IHeatTreatmentService
    {
        private readonly IHeatTreatmentRepository _heatTreatmentRepository;
        private readonly ILogger<HeatTreatmentService> _logger;

        public HeatTreatmentService(IHeatTreatmentRepository heatTreatmentRepo, ILogger<HeatTreatmentService> logger)
        {
            _heatTreatmentRepository = heatTreatmentRepo;
            _logger = logger;
        }

        public async Task CreateHeatTreatment(HeatTreatmentMaster model)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
                throw new ArgumentException("HeatTreatment name should not be empty!");

            bool exists = await _heatTreatmentRepository.ExistsByName(model.Name);
            if (exists)
                throw new InvalidOperationException("HeatTreatment already exists!");

            await _heatTreatmentRepository.AddHeatTreatment(model);
            _logger.LogInformation("HeatTreatment '{HeatTreatmentName}' created successfully.", model.Name);
        }

        public async Task ModifyHeatTreatment(HeatTreatmentMaster model)
        {
            if (model.ID == 0)
                throw new ArgumentException("HeatTreatment ID should not be empty!");

            bool exists = await _heatTreatmentRepository.ExistsByNameAndNotId(model.Name, model.ID);
            if (exists)
                throw new InvalidOperationException("Same HeatTreatment already exists!");

            var existingHeatTreatment = await _heatTreatmentRepository.GetHeatTreatmentById(model.ID);
            if (existingHeatTreatment == null)
                throw new InvalidOperationException("HeatTreatment not found!");

            existingHeatTreatment.Name = model.Name;
            existingHeatTreatment.Code = model.Code;
            existingHeatTreatment.HeatTreatmentCategoryID = model.HeatTreatmentCategoryID;
            existingHeatTreatment.TempRangeMin = model.TempRangeMin;
            existingHeatTreatment.TempRangeMax = model.TempRangeMax;
            existingHeatTreatment.TempRangeDescription = model.TempRangeDescription;
            existingHeatTreatment.CoolingMediumID = model.CoolingMediumID;

            // Update ApplicableClassifications junction
            existingHeatTreatment.ApplicableClassifications?.Clear();
            if (model.ApplicableClassifications != null)
            {
                foreach (var classification in model.ApplicableClassifications)
                {
                    existingHeatTreatment.ApplicableClassifications.Add(classification);
                }
            }

            existingHeatTreatment.ModifiedOn = DateTime.UtcNow;

            await _heatTreatmentRepository.UpdateHeatTreatment(existingHeatTreatment);
            _logger.LogInformation("HeatTreatment '{HeatTreatmentName}' updated successfully.", model.Name);
        }

        public async Task RemoveHeatTreatment(long id)
        {
            var existingHeatTreatment = await _heatTreatmentRepository.GetHeatTreatmentById(id);
            if (existingHeatTreatment == null)
                throw new InvalidOperationException("HeatTreatment not found!");

            existingHeatTreatment.IsActive = false;
            existingHeatTreatment.ModifiedOn = DateTime.UtcNow;

            await _heatTreatmentRepository.UpdateHeatTreatment(existingHeatTreatment);
            _logger.LogInformation("HeatTreatment with ID '{HeatTreatmentId}' deleted successfully.", id);
        }

        public async Task<HeatTreatmentMaster> GetHeatTreatmentDetails(long id)
        {
            var classification = await _heatTreatmentRepository.GetHeatTreatmentById(id);
            if (classification == null)
                throw new InvalidOperationException("HeatTreatment not found!");

            return classification;
        }

        public async Task<PagedResponse<object>> FetchHeatTreatmentList(PageFilter filter)
        {
            return await _heatTreatmentRepository.GetAllHeatTreatments(filter);
        }

        public async Task<List<DropdwonSelector>> GetHeatTreatmentDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            return await _heatTreatmentRepository.GetHeatTreatmentDropdown(searchTerm, pageNo, pageSize);
        }
    }
}
