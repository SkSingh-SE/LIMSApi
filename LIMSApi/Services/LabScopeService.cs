using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;

namespace LIMSApi.Services
{
    public class LabScopeService : ILabScopeService
    {
        private readonly ILabScopeRepository _labScopeRepository;
        private readonly ILogger<LabScopeService> _logger;

        public LabScopeService(ILabScopeRepository labScopeRepo, ILogger<LabScopeService> logger)
        {
            _labScopeRepository = labScopeRepo;
            _logger = logger;
        }

        public async Task CreateLabScope(LabScopeMaster model)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
                throw new ArgumentException("LabScope name should not be empty!");

            bool exists = await _labScopeRepository.ExistsByName(model.Name);
            if (exists)
                throw new InvalidOperationException("LabScope already exists!");

            await _labScopeRepository.AddLabScope(model);
            _logger.LogInformation("LabScope '{LabScopeName}' created successfully.", model.Name);
        }

        public async Task ModifyLabScope(LabScopeMaster model)
        {
            if (model.ID == 0)
                throw new ArgumentException("LabScope ID should not be empty!");

            bool exists = await _labScopeRepository.ExistsByNameAndNotId(model.Name, model.ID);
            if (exists)
                throw new InvalidOperationException("Same LabScope already exists!");

            var existingLabScope = await _labScopeRepository.GetLabScopeById(model.ID);
            if (existingLabScope == null)
                throw new InvalidOperationException("LabScope not found!");

            existingLabScope.Name = model.Name;;
            existingLabScope.Description = model.Description;
            existingLabScope.TestMethodID = model.TestMethodID;
            existingLabScope.ModifiedOn = DateTime.UtcNow;

            await _labScopeRepository.UpdateLabScope(existingLabScope);
            _logger.LogInformation("LabScope '{LabScopeName}' updated successfully.", model.Name);
        }

        public async Task RemoveLabScope(long id)
        {
            var existingLabScope = await _labScopeRepository.GetLabScopeById(id);
            if (existingLabScope == null)
                throw new InvalidOperationException("LabScope not found!");

            existingLabScope.IsActive = false;
            existingLabScope.ModifiedOn = DateTime.UtcNow;

            await _labScopeRepository.UpdateLabScope(existingLabScope);
            _logger.LogInformation("LabScope with ID '{LabScopeId}' deleted successfully.", id);
        }

        public async Task<LabScopeMaster> GetLabScopeDetails(long id)
        {
            var classification = await _labScopeRepository.GetLabScopeById(id);
            if (classification == null)
                throw new InvalidOperationException("LabScope not found!");

            return classification;
        }

        public async Task<PagedResponse<object>> FetchLabScopeList(PageFilter filter)
        {
            return await _labScopeRepository.GetAllLabScopes(filter);
        }

        public async Task<List<DropdwonSelector>> GetLabScopeDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            return await _labScopeRepository.GetLabScopeDropdown(searchTerm, pageNo, pageSize);
        }
    }
}
