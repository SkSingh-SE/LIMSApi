using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Services
{
    public class MetalClassificationService : IMetalClassificationService
    {
        private readonly IMetalClassificationRepository _MetalClassificationRepository;
        private readonly ILogger<MetalClassificationService> _logger;
        private readonly LIMSContext _context;

        public MetalClassificationService(IMetalClassificationRepository MetalClassificationRepo, ILogger<MetalClassificationService> logger, LIMSContext context)
        {
            _MetalClassificationRepository = MetalClassificationRepo;
            _logger = logger;
            _context = context;
        }

        public async Task CreateMetalClassification(MetalClassificationMaster model)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
                throw new ArgumentException("MetalClassification name should not be empty!");

            bool exists = await _MetalClassificationRepository.ExistsByName(model.Name);
            if (exists)
                throw new InvalidOperationException("Metal Classification Name already exists!");

            if (!string.IsNullOrWhiteSpace(model.Code))
            {
                bool codeExists = await _MetalClassificationRepository.ExistsByCode(model.Code);
                if (codeExists)
                    throw new InvalidOperationException("Metal Classification Code already exists!");
            }

            await _MetalClassificationRepository.AddMetalClassification(model);
            _logger.LogInformation("MetalClassification '{MetalClassificationName}' created successfully.", model.Name);
        }

        public async Task ModifyMetalClassification(MetalClassificationMaster model)
        {
            if (model.ID == 0)
                throw new ArgumentException("MetalClassification ID should not be empty!");

            bool exists = await _MetalClassificationRepository.ExistsByNameAndNotId(model.Name, model.ID);
            if (exists)
                throw new InvalidOperationException("Metal Classification Name already exists!");

            if (!string.IsNullOrWhiteSpace(model.Code))
            {
                bool codeExists = await _MetalClassificationRepository.ExistsByCodeAndNotId(model.Code, model.ID);
                if (codeExists)
                    throw new InvalidOperationException("Metal Classification Code already exists!");
            }

            var existingMetalClassification = await _MetalClassificationRepository.GetMetalClassificationById(model.ID);
            if (existingMetalClassification == null)
                throw new InvalidOperationException("MetalClassification not found!");

            existingMetalClassification.Name = model.Name;
            existingMetalClassification.Code = model.Code;
            existingMetalClassification.ParentID = model.ParentID;
            existingMetalClassification.HasChemicalParams = model.HasChemicalParams;
            existingMetalClassification.HasMechanicalParams = model.HasMechanicalParams;
            existingMetalClassification.SortOrder = model.SortOrder;
            existingMetalClassification.MetalType = model.MetalType;
            existingMetalClassification.ModifiedOn = DateTime.UtcNow;

            // Replace parameters via DbContext to avoid EF NoAction cascade issue.
            // Parameters.Clear() + reassignment loses EF tracking; instead delete/insert explicitly.
            var existingParams = await _context.Set<MetalClassificationParameter>()
                .Where(p => p.MetalClassificationID == model.ID)
                .ToListAsync();
            _context.Set<MetalClassificationParameter>().RemoveRange(existingParams);

            if (model.Parameters != null)
            {
                foreach (var p in model.Parameters)
                {
                    p.MetalClassificationID = model.ID;
                    _context.Set<MetalClassificationParameter>().Add(p);
                }
            }

            await _MetalClassificationRepository.UpdateMetalClassification(existingMetalClassification);
            _logger.LogInformation("MetalClassification '{MetalClassificationName}' updated successfully.", model.Name);
        }

        public async Task RemoveMetalClassification(long id)
        {
            var existingMetalClassification = await _MetalClassificationRepository.GetMetalClassificationById(id);
            if (existingMetalClassification == null)
                throw new InvalidOperationException("MetalClassification not found!");

            // Check self-referencing hierarchy
            bool hasChildren = await _context.MetalClassificationMasters.AnyAsync(m => m.ParentID == id && m.IsActive);
            if (hasChildren)
                throw new InvalidOperationException("Cannot delete: Metal Classification has child classifications.");

            // Generic dependency validation across all referencing tables
            await DeleteValidationHelper.ValidateDeleteAsync<MetalClassificationMaster>(_context, id, "Metal Classification", existingMetalClassification.Name);

            existingMetalClassification.IsActive = false;
            existingMetalClassification.ModifiedOn = DateTime.UtcNow;

            await _MetalClassificationRepository.UpdateMetalClassification(existingMetalClassification);
            _logger.LogInformation("MetalClassification with ID '{MetalClassificationId}' deleted successfully.", id);
        }

        public async Task<MetalClassificationMaster> GetMetalClassificationDetails(long id)
        {
            var classification = await _MetalClassificationRepository.GetMetalClassificationById(id);
            if (classification == null)
                throw new InvalidOperationException("MetalClassification not found!");

            return classification;
        }

        public async Task<PagedResponse<object>> FetchMetalClassificationList(PageFilter filter)
        {
            return await _MetalClassificationRepository.GetAllMetalClassifications(filter);
        }

        public async Task<List<DropdwonSelector>> GetMetalClassificationDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            return await _MetalClassificationRepository.GetMetalClassificationDropdown(searchTerm, pageNo, pageSize);
        }
        public async Task<List<ParameterMaster>> GetParameterByMetalId(long id)
        {
            return await _MetalClassificationRepository.GetParameterByMetalId(id);
        }
    }
}
