using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Services
{
    public class HeatTreatmentService : IHeatTreatmentService
    {
        private readonly IHeatTreatmentRepository _heatTreatmentRepository;
        private readonly ILogger<HeatTreatmentService> _logger;
        private readonly LIMSContext _context;

        public HeatTreatmentService(IHeatTreatmentRepository heatTreatmentRepo, ILogger<HeatTreatmentService> logger, LIMSContext context)
        {
            _heatTreatmentRepository = heatTreatmentRepo;
            _logger = logger;
            _context = context;
        }

        public async Task CreateHeatTreatment(HeatTreatmentMaster model)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
                throw new ArgumentException("HeatTreatment name should not be empty!");

            bool exists = await _heatTreatmentRepository.ExistsByName(model.Name);
            if (exists)
                throw new InvalidOperationException("Heat Treatment Name already exists!");

            if (!string.IsNullOrWhiteSpace(model.Code))
            {
                bool codeExists = await _heatTreatmentRepository.ExistsByCode(model.Code);
                if (codeExists)
                    throw new InvalidOperationException("Heat Treatment Code already exists!");
            }

            await _heatTreatmentRepository.AddHeatTreatment(model);
            _logger.LogInformation("HeatTreatment '{HeatTreatmentName}' created successfully.", model.Name);
        }

        public async Task ModifyHeatTreatment(HeatTreatmentMaster model)
        {
            if (model.ID == 0)
                throw new ArgumentException("HeatTreatment ID should not be empty!");

            bool exists = await _heatTreatmentRepository.ExistsByNameAndNotId(model.Name, model.ID);
            if (exists)
                throw new InvalidOperationException("Heat Treatment Name already exists!");

            if (!string.IsNullOrWhiteSpace(model.Code))
            {
                bool codeExists = await _heatTreatmentRepository.ExistsByCodeAndNotId(model.Code, model.ID);
                if (codeExists)
                    throw new InvalidOperationException("Heat Treatment Code already exists!");
            }

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

            // Sync Stages
            if (model.Stages != null)
            {
                var incomingIds = model.Stages.Where(s => s.ID > 0).Select(s => s.ID).ToHashSet();
                foreach (var existingStage in existingHeatTreatment.Stages.Where(s => s.IsActive))
                {
                    if (!incomingIds.Contains(existingStage.ID))
                    {
                        existingStage.IsActive = false;
                        existingStage.ModifiedOn = DateTime.UtcNow;
                    }
                }

                foreach (var incomingStage in model.Stages)
                {
                    if (incomingStage.ID > 0)
                    {
                        var target = existingHeatTreatment.Stages.FirstOrDefault(s => s.ID == incomingStage.ID);
                        if (target != null)
                        {
                            target.StageNumber = incomingStage.StageNumber;
                            target.StageName = incomingStage.StageName;
                            target.TempRangeMin = incomingStage.TempRangeMin;
                            target.TempRangeMax = incomingStage.TempRangeMax;
                            target.SoakingDescription = incomingStage.SoakingDescription;
                            target.CoolingMediumID = (incomingStage.CoolingMediumID == null || incomingStage.CoolingMediumID == 0) ? null : incomingStage.CoolingMediumID;
                            target.IsActive = true;
                            target.ModifiedOn = DateTime.UtcNow;
                        }
                    }
                    else
                    {
                        incomingStage.HeatTreatmentID = model.ID;
                        incomingStage.CoolingMediumID = (incomingStage.CoolingMediumID == null || incomingStage.CoolingMediumID == 0) ? null : incomingStage.CoolingMediumID;
                        incomingStage.IsActive = true;
                        incomingStage.CreatedOn = DateTime.UtcNow;
                        existingHeatTreatment.Stages.Add(incomingStage);
                    }
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

            await DeleteValidationHelper.ValidateDeleteAsync<HeatTreatmentMaster>(_context, id, "Heat Treatment", existingHeatTreatment.Name);

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
