using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;

namespace LIMSApi.Services
{
    public class MakerService : IMakerService
    {
        private readonly IMakerRepository _makerRepository;
        private readonly ILogger<MakerService> _logger;

        public MakerService(IMakerRepository makerRepo, ILogger<MakerService> logger)
        {
            _makerRepository = makerRepo;
            _logger = logger;
        }

        public async Task CreateMaker(MakerMaster model)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
                throw new ArgumentException("Maker name should not be empty!");

            bool exists = await _makerRepository.ExistsByName(model.Name);
            if (exists)
                throw new InvalidOperationException("Maker already exists!");

            await _makerRepository.AddMaker(model);
            _logger.LogInformation("Maker '{MakerName}' created successfully.", model.Name);
        }

        public async Task ModifyMaker(MakerMaster model)
        {
            if (model.ID == 0)
                throw new ArgumentException("Maker ID should not be empty!");

            bool exists = await _makerRepository.ExistsByNameAndNotId(model.Name, model.ID);
            if (exists)
                throw new InvalidOperationException("Same Maker already exists!");

            var existingMaker = await _makerRepository.GetMakerById(model.ID);
            if (existingMaker == null)
                throw new InvalidOperationException("Maker not found!");

            existingMaker.Name = model.Name;
            existingMaker.ModifiedOn = DateTime.UtcNow;

            await _makerRepository.UpdateMaker(existingMaker);
            _logger.LogInformation("Maker '{MakerName}' updated successfully.", model.Name);
        }

        public async Task RemoveMaker(long id)
        {
            var existingMaker = await _makerRepository.GetMakerById(id);
            if (existingMaker == null)
                throw new InvalidOperationException("Maker not found!");

            existingMaker.IsActive = false;
            existingMaker.ModifiedOn = DateTime.UtcNow;

            await _makerRepository.UpdateMaker(existingMaker);
            _logger.LogInformation("Maker with ID '{MakerId}' deleted successfully.", id);
        }

        public async Task<MakerMaster> GetMakerDetails(long id)
        {
            var classification = await _makerRepository.GetMakerById(id);
            if (classification == null)
                throw new InvalidOperationException("Maker not found!");

            return classification;
        }

        public async Task<PagedResponse<object>> FetchMakerList(PageFilter filter)
        {
            return await _makerRepository.GetAllMakers(filter);
        }

        public async Task<List<DropdwonSelector>> GetMakerDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            return await _makerRepository.GetMakerDropdown(searchTerm, pageNo, pageSize);
        }
    }
}
