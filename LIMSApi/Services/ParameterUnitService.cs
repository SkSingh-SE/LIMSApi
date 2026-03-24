using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;

namespace LIMSApi.Services
{
    public class ParameterUnitService : IParameterUnitService
    {
        private readonly IParameterUnitRepository _ParameterUnitRepository;
        private readonly ILogger<ParameterUnitService> _logger;
        private readonly LIMSContext _context;

        public ParameterUnitService(IParameterUnitRepository ParameterUnitRepo, ILogger<ParameterUnitService> logger, LIMSContext context)
        {
            _ParameterUnitRepository = ParameterUnitRepo;
            _logger = logger;
            _context = context;
        }

        public async Task CreateParameterUnit(ParameterUnitMaster model)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
                throw new ArgumentException("ParameterUnit name should not be empty!");

            bool exists = await _ParameterUnitRepository.ExistsByName(model.Name);
            if (exists)
                throw new InvalidOperationException("ParameterUnit already exists!");

            await _ParameterUnitRepository.AddParameterUnit(model);
            _logger.LogInformation("ParameterUnit '{ParameterUnitName}' created successfully.", model.Name);
        }

        public async Task ModifyParameterUnit(ParameterUnitMaster model)
        {
            if (model.ID == 0)
                throw new ArgumentException("ParameterUnit ID should not be empty!");

            bool exists = await _ParameterUnitRepository.ExistsByNameAndNotId(model.Name, model.ID);
            if (exists)
                throw new InvalidOperationException("Same ParameterUnit already exists!");

            var existingParameterUnit = await _ParameterUnitRepository.GetParameterUnitById(model.ID);
            if (existingParameterUnit == null)
                throw new InvalidOperationException("ParameterUnit not found!");

            existingParameterUnit.Name = model.Name;
            existingParameterUnit.ConversaionFactor = model.ConversaionFactor;
            existingParameterUnit.SimilarUnit1 = model.SimilarUnit1;
            existingParameterUnit.ConversionFactor1 = model.ConversionFactor1;
            existingParameterUnit.SimilarUnit2 = model.SimilarUnit2;
            existingParameterUnit.ConversionFactor2 = model.ConversionFactor2;
            existingParameterUnit.SimilarUnit3 = model.SimilarUnit3;
            existingParameterUnit.ConversionFactor3 = model.ConversionFactor3;
            existingParameterUnit.ModifiedOn = DateTime.UtcNow;

            await _ParameterUnitRepository.UpdateParameterUnit(existingParameterUnit);
            _logger.LogInformation("ParameterUnit '{ParameterUnitName}' updated successfully.", model.Name);
        }

        public async Task RemoveParameterUnit(long id)
        {
            var existingParameterUnit = await _ParameterUnitRepository.GetParameterUnitById(id);
            if (existingParameterUnit == null)
                throw new InvalidOperationException("ParameterUnit not found!");

            await DeleteValidationHelper.ValidateDeleteAsync<ParameterUnitMaster>(_context, id, "Parameter Unit");

            existingParameterUnit.IsActive = false;
            existingParameterUnit.ModifiedOn = DateTime.UtcNow;

            await _ParameterUnitRepository.UpdateParameterUnit(existingParameterUnit);
            _logger.LogInformation("ParameterUnit with ID '{ParameterUnitId}' deleted successfully.", id);
        }

        public async Task<ParameterUnitMaster> GetParameterUnitDetails(long id)
        {
            var classification = await _ParameterUnitRepository.GetParameterUnitById(id);
            if (classification == null)
                throw new InvalidOperationException("ParameterUnit not found!");

            return classification;
        }

        public async Task<PagedResponse<object>> FetchParameterUnitList(PageFilter filter)
        {
            return await _ParameterUnitRepository.GetAllParameterUnits(filter);
        }

        public async Task<List<DropdwonSelector>> GetParameterUnitDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            return await _ParameterUnitRepository.GetParameterUnitDropdown(searchTerm, pageNo, pageSize);
        }
    }
}
