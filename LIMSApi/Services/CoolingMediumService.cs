using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;

namespace LIMSApi.Services
{
    public class CoolingMediumService : ICoolingMediumService
    {
        private readonly ICoolingMediumRepository _repo;
        private readonly ILogger<CoolingMediumService> _logger;

        public CoolingMediumService(ICoolingMediumRepository repo, ILogger<CoolingMediumService> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        public async Task CreateCoolingMedium(CoolingMediumMaster model)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
                throw new ArgumentException("CoolingMedium name should not be empty!");
            bool exists = await _repo.ExistsByName(model.Name);
            if (exists) throw new InvalidOperationException("CoolingMedium already exists!");
            await _repo.AddCoolingMedium(model);
            _logger.LogInformation("CoolingMedium '{Name}' created successfully.", model.Name);
        }

        public async Task ModifyCoolingMedium(CoolingMediumMaster model)
        {
            if (model.ID == 0) throw new ArgumentException("CoolingMedium ID should not be empty!");
            bool exists = await _repo.ExistsByNameAndNotId(model.Name, model.ID);
            if (exists) throw new InvalidOperationException("Same CoolingMedium already exists!");
            var existing = await _repo.GetCoolingMediumById(model.ID);
            if (existing == null) throw new InvalidOperationException("CoolingMedium not found!");
            existing.Name = model.Name;
            existing.ModifiedOn = DateTime.UtcNow;
            await _repo.UpdateCoolingMedium(existing);
            _logger.LogInformation("CoolingMedium '{Name}' updated successfully.", model.Name);
        }

        public async Task RemoveCoolingMedium(long id)
        {
            var existing = await _repo.GetCoolingMediumById(id);
            if (existing == null) throw new InvalidOperationException("CoolingMedium not found!");
            existing.IsActive = false;
            existing.ModifiedOn = DateTime.UtcNow;
            await _repo.UpdateCoolingMedium(existing);
            _logger.LogInformation("CoolingMedium with ID '{Id}' deleted successfully.", id);
        }

        public async Task<CoolingMediumMaster> GetCoolingMediumDetails(long id)
        {
            var item = await _repo.GetCoolingMediumById(id);
            if (item == null) throw new InvalidOperationException("CoolingMedium not found!");
            return item;
        }

        public async Task<PagedResponse<object>> FetchCoolingMediumList(PageFilter filter) => await _repo.GetAllCoolingMediums(filter);
        public async Task<List<DropdwonSelector>> GetCoolingMediumDropdown(string? searchTerm, int pageNo, int pageSize) => await _repo.GetCoolingMediumDropdown(searchTerm, pageNo, pageSize);
    }
}
