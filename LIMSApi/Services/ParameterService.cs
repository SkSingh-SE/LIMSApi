using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;

namespace LIMSApi.Services
{
    public class ParameterService : IParameterService
    {
        private readonly IParameterRepository _parameterRepository;
        private readonly ILogger<ParameterService> _logger;

        public ParameterService(IParameterRepository parameterRepo, ILogger<ParameterService> logger)
        {
            _parameterRepository = parameterRepo;
            _logger = logger;
        }

        public async Task CreateParameter(ParameterMaster model)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
                throw new ArgumentException("Parameter name should not be empty!");

            bool exists = await _parameterRepository.ExistsByName(model.Name);
            if (exists)
                throw new InvalidOperationException("Parameter already exists!");

            await _parameterRepository.AddParameter(model);
            _logger.LogInformation("Parameter '{ParameterName}' created successfully.", model.Name);
        }

        public async Task ModifyParameter(ParameterMaster model)
        {
            if (model.ID == 0)
                throw new ArgumentException("Parameter ID should not be empty!");

            bool exists = await _parameterRepository.ExistsByNameAndNotId(model.Name, model.ID);
            if (exists)
                throw new InvalidOperationException("Same Parameter already exists!");

            var existingParameter = await _parameterRepository.GetParameterById(model.ID);
            if (existingParameter == null)
                throw new InvalidOperationException("Parameter not found!");

            existingParameter.Name = model.Name;
            existingParameter.ModifiedOn = DateTime.UtcNow;

            await _parameterRepository.UpdateParameter(existingParameter);
            _logger.LogInformation("Parameter '{ParameterName}' updated successfully.", model.Name);
        }

        public async Task RemoveParameter(long id)
        {
            var existingParameter = await _parameterRepository.GetParameterById(id);
            if (existingParameter == null)
                throw new InvalidOperationException("Parameter not found!");

            existingParameter.IsActive = false;
            existingParameter.ModifiedOn = DateTime.UtcNow;

            await _parameterRepository.UpdateParameter(existingParameter);
            _logger.LogInformation("Parameter with ID '{ParameterId}' deleted successfully.", id);
        }

        public async Task<ParameterMaster> GetParameterDetails(long id)
        {
            var classification = await _parameterRepository.GetParameterById(id);
            if (classification == null)
                throw new InvalidOperationException("Parameter not found!");

            return classification;
        }

        public async Task<PagedResponse<object>> FetchParameterList(PageFilter filter)
        {
            return await _parameterRepository.GetAllParameters(filter);
        }

        public async Task<List<DropdwonSelector>> GetParameterDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            return await _parameterRepository.GetParameterDropdown(searchTerm, pageNo, pageSize);
        }
    }
}
