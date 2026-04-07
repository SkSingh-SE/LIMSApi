using LIMSApi.Data;
using LIMSApi.Dtos;
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

            existingHeatTreatment.ModifiedOn = DateTime.UtcNow;

            await _heatTreatmentRepository.UpdateHeatTreatment(existingHeatTreatment);
            _logger.LogInformation("HeatTreatment '{HeatTreatmentName}' updated successfully.", model.Name);
        }

        public async Task RemoveHeatTreatment(long id)
        {
            var existingHeatTreatment = await _heatTreatmentRepository.GetHeatTreatmentById(id);
            if (existingHeatTreatment == null)
                throw new InvalidOperationException("HeatTreatment not found!");

            bool hasProductConditions = await _context.ProductConditionMasters.AnyAsync(p => p.LinkedHeatTreatmentID == id && p.IsActive);
            if (hasProductConditions)
                throw new InvalidOperationException("Cannot delete: Heat Treatment is linked to Product Conditions.");

            bool hasSpecLines = await _context.SpecificationLines.AnyAsync(s => s.HeatTreatmentID == id);
            if (hasSpecLines)
                throw new InvalidOperationException("Cannot delete: Heat Treatment is linked to Material Specifications.");

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
