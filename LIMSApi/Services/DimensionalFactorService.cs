using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Services
{
    public class DimensionalFactorService : IDimensionalFactorService
    {
        private readonly IDimensionalFactorRepository _dimensionalRepository;
        private readonly ILogger<DimensionalFactorService> _logger;
        private readonly LIMSContext _context;

        public DimensionalFactorService(IDimensionalFactorRepository dimensionalRepo, ILogger<DimensionalFactorService> logger, LIMSContext context)
        {
            _dimensionalRepository = dimensionalRepo;
            _logger = logger;
            _context = context;
        }

        public async Task CreateDimensionalFactor(DimensionalFactorMaster model)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
                throw new ArgumentException("DimensionalFactor name should not be empty!");

            bool exists = await _dimensionalRepository.ExistsByName(model.Name);
            if (exists)
                throw new InvalidOperationException("Dimensional Factor Name already exists!");

            if (!string.IsNullOrWhiteSpace(model.Code))
            {
                bool codeExists = await _dimensionalRepository.ExistsByCode(model.Code);
                if (codeExists)
                    throw new InvalidOperationException("Dimensional Factor Code already exists!");
            }

            await _dimensionalRepository.AddDimensionalFactor(model);
            _logger.LogInformation("DimensionalFactor '{DimensionalFactorName}' created successfully.", model.Name);
        }

        public async Task ModifyDimensionalFactor(DimensionalFactorMaster model)
        {
            if (model.ID == 0)
                throw new ArgumentException("DimensionalFactor ID should not be empty!");

            bool exists = await _dimensionalRepository.ExistsByNameAndNotId(model.Name, model.ID);
            if (exists)
                throw new InvalidOperationException("Dimensional Factor Name already exists!");

            if (!string.IsNullOrWhiteSpace(model.Code))
            {
                bool codeExists = await _dimensionalRepository.ExistsByCodeAndNotId(model.Code, model.ID);
                if (codeExists)
                    throw new InvalidOperationException("Dimensional Factor Code already exists!");
            }

            var existingDimensionalFactor = await _dimensionalRepository.GetDimensionalFactorById(model.ID);
            if (existingDimensionalFactor == null)
                throw new InvalidOperationException("DimensionalFactor not found!");

            existingDimensionalFactor.Name = model.Name;
            existingDimensionalFactor.Code = model.Code;
            existingDimensionalFactor.ParameterUnitID = (model.ParameterUnitID == null || model.ParameterUnitID == 0) ? null : model.ParameterUnitID;
            existingDimensionalFactor.Instrument = model.Instrument;
            existingDimensionalFactor.ToleranceType = model.ToleranceType;
            existingDimensionalFactor.DefaultTestMethodID = (model.DefaultTestMethodID == null || model.DefaultTestMethodID == 0) ? null : model.DefaultTestMethodID;

            // Update ApplicableForms junction
            existingDimensionalFactor.ApplicableForms?.Clear();
            if (model.ApplicableForms != null)
                foreach (var form in model.ApplicableForms)
                    existingDimensionalFactor.ApplicableForms.Add(form);

            existingDimensionalFactor.ModifiedOn = DateTime.UtcNow;

            await _dimensionalRepository.UpdateDimensionalFactor(existingDimensionalFactor);
            _logger.LogInformation("DimensionalFactor '{DimensionalFactorName}' updated successfully.", model.Name);
        }

        public async Task RemoveDimensionalFactor(long id)
        {
            var existingDimensionalFactor = await _dimensionalRepository.GetDimensionalFactorById(id);
            if (existingDimensionalFactor == null)
                throw new InvalidOperationException("DimensionalFactor not found!");

            bool hasSpecLines = await _context.SpecificationLines.AnyAsync(s => s.DimensionalFactorID == id);
            if (hasSpecLines)
                throw new InvalidOperationException("Cannot delete: Dimensional Factor is linked to Material Specifications.");

            existingDimensionalFactor.IsActive = false;
            existingDimensionalFactor.ModifiedOn = DateTime.UtcNow;

            await _dimensionalRepository.UpdateDimensionalFactor(existingDimensionalFactor);
            _logger.LogInformation("DimensionalFactor with ID '{DimensionalFactorId}' deleted successfully.", id);
        }

        public async Task<DimensionalFactorMaster> GetDimensionalFactorDetails(long id)
        {
            var classification = await _dimensionalRepository.GetDimensionalFactorById(id);
            if (classification == null)
                throw new InvalidOperationException("DimensionalFactor not found!");

            return classification;
        }

        public async Task<PagedResponse<object>> FetchDimensionalFactorList(PageFilter filter)
        {
            return await _dimensionalRepository.GetAllDimensionalFactors(filter);
        }

        public async Task<List<DropdwonSelector>> GetDimensionalFactorDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            return await _dimensionalRepository.GetDimensionalFactorDropdown(searchTerm, pageNo, pageSize);
        }
    }
}
