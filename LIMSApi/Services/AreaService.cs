using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;

namespace LIMSApi.Services
{
    public class AreaService : IAreaService
    {
        private readonly IAreaRepository _areaRepository;
        private readonly ILogger<AreaService> _logger;

        public AreaService(IAreaRepository areaRepository, ILogger<AreaService> logger)
        {
            _areaRepository = areaRepository;
            _logger = logger;
        }

        public async Task CreateArea(AreaMaster area)
        {
            if (string.IsNullOrWhiteSpace(area.Name))
                throw new ArgumentException("Area name should not be empty!");

            bool exists = await _areaRepository.ExistsByName(area.Name);
            if (exists)
                throw new InvalidOperationException("Area already exists!");

            await _areaRepository.AddArea(area);
            _logger.LogInformation("Area '{AreaName}' created successfully.", area.Name);
        }

        public async Task ModifyArea(AreaMaster area)
        {
            if (area.ID == 0)
                throw new ArgumentException("Area ID should not be empty!");

            bool exists = await _areaRepository.ExistsByNameAndNotId(area.Name, area.ID);
            if (exists)
                throw new InvalidOperationException("Same Area already exists!");

            var existingArea = await _areaRepository.GetAreaById(area.ID);
            if (existingArea == null)
                throw new InvalidOperationException("Area not found!");

            existingArea.Name = area.Name;
            existingArea.Code = area.Code;
            existingArea.ModifiedOn = DateTime.UtcNow;

            await _areaRepository.UpdateArea(existingArea);
            _logger.LogInformation("Area '{AreaName}' updated successfully.", area.Name);
        }

        public async Task RemoveArea(long id)
        {
            var existingArea = await _areaRepository.GetAreaById(id);
            if (existingArea == null)
                throw new InvalidOperationException("Area not found!");

            existingArea.IsActive = false;
            existingArea.ModifiedOn = DateTime.UtcNow;

            await _areaRepository.UpdateArea(existingArea);
            _logger.LogInformation("Area with ID '{AreaId}' deleted successfully.", id);
        }

        public async Task<AreaMaster> GetAreaDetails(long id)
        {
            var area = await _areaRepository.GetAreaById(id);
            if (area == null)
                throw new InvalidOperationException("Area not found!");

            return area;
        }

        public async Task<PagedResponse<AreaMaster>> FetchAreas(PageFilter filter)
        {
            return await _areaRepository.GetAllAreas(filter);
        }

        public async Task<List<DropdwonSelector>> GetAreaWithPincode(string? searchTerm, int pageNo, int pageSize)
        {
            return await _areaRepository.GetAreaWithPincode(searchTerm, pageNo, pageSize);
        }
    }
}
