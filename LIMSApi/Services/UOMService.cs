using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;

namespace LIMSApi.Services
{
    public class UOMService : IUOMService
    {
        private readonly IUOMRepository _uomRepository;
        private readonly ILogger<UOMService> _logger;

        public UOMService(IUOMRepository uomRepo, ILogger<UOMService> logger)
        {
            _uomRepository = uomRepo;
            _logger = logger;
        }

        public async Task CreateUOM(UOMMaster model)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
                throw new ArgumentException("UOM name should not be empty!");

            bool exists = await _uomRepository.ExistsByName(model.Name);
            if (exists)
                throw new InvalidOperationException("UOM already exists!");

            await _uomRepository.AddUOM(model);
            _logger.LogInformation("UOM '{UOMName}' created successfully.", model.Name);
        }

        public async Task ModifyUOM(UOMMaster model)
        {
            if (model.ID == 0)
                throw new ArgumentException("UOM ID should not be empty!");

            bool exists = await _uomRepository.ExistsByNameAndNotId(model.Name, model.ID);
            if (exists)
                throw new InvalidOperationException("Same UOM already exists!");

            var existingUOM = await _uomRepository.GetUOMById(model.ID);
            if (existingUOM == null)
                throw new InvalidOperationException("UOM not found!");

            existingUOM.Name = model.Name;
            existingUOM.ModifiedOn = DateTime.UtcNow;

            await _uomRepository.UpdateUOM(existingUOM);
            _logger.LogInformation("UOM '{UOMName}' updated successfully.", model.Name);
        }

        public async Task RemoveUOM(long id)
        {
            var existingUOM = await _uomRepository.GetUOMById(id);
            if (existingUOM == null)
                throw new InvalidOperationException("UOM not found!");

            existingUOM.IsActive = false;
            existingUOM.ModifiedOn = DateTime.UtcNow;

            await _uomRepository.UpdateUOM(existingUOM);
            _logger.LogInformation("UOM with ID '{UOMId}' deleted successfully.", id);
        }

        public async Task<UOMMaster> GetUOMDetails(long id)
        {
            var classification = await _uomRepository.GetUOMById(id);
            if (classification == null)
                throw new InvalidOperationException("UOM not found!");

            return classification;
        }

        public async Task<PagedResponse<object>> FetchUOMList(PageFilter filter)
        {
            return await _uomRepository.GetAllUOMs(filter);
        }

        public async Task<List<DropdwonSelector>> GetUOMDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            return await _uomRepository.GetUOMDropdown(searchTerm, pageNo, pageSize);
        }
    }
}
