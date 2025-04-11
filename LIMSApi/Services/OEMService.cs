using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;

namespace LIMSApi.Services
{
    public class OEMService : IOEMService
    {
        private readonly IOEMRepository _oemRepository;
        private readonly ILogger<OEMService> _logger;

        public OEMService(IOEMRepository oemRepo, ILogger<OEMService> logger)
        {
            _oemRepository = oemRepo;
            _logger = logger;
        }

        public async Task CreateOEM(OEMMaster model)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
                throw new ArgumentException("OEM name should not be empty!");

            bool exists = await _oemRepository.ExistsByName(model.Name);
            if (exists)
                throw new InvalidOperationException("OEM already exists!");
            await _oemRepository.AddOEM(model);
            _logger.LogInformation("OEM '{OEMName}' created successfully.", model.Name);
        }

        public async Task ModifyOEM(OEMMaster model)
        {
            if (model.ID == 0)
                throw new ArgumentException("OEM ID should not be empty!");

            bool exists = await _oemRepository.ExistsByNameAndNotId(model.Name, model.ID);
            if (exists)
                throw new InvalidOperationException("Same OEM already exists!");

            var existingOEM = await _oemRepository.GetOEMById(model.ID);
            if (existingOEM == null)
                throw new InvalidOperationException("OEM not found!");

            existingOEM.Name = model.Name;
            existingOEM.ContactPerson1 = model.ContactPerson1;
            existingOEM.ContactPerson2 = model.ContactPerson2;
            existingOEM.ContactPerson3 = model.ContactPerson3;
            existingOEM.ContactNo1 = model.ContactNo1;
            existingOEM.ContactNo2 = model.ContactNo2;
            existingOEM.ContactNo3 = model.ContactNo3;
            existingOEM.EmailId1 = model.EmailId1;
            existingOEM.EmailId2 = model.EmailId2;
            existingOEM.EmailId3 = model.EmailId3;
            existingOEM.AgreementFilePath = model.AgreementFilePath;

            existingOEM.ModifiedOn = DateTime.UtcNow;

            await _oemRepository.UpdateOEM(existingOEM);
            _logger.LogInformation("OEM '{OEMName}' updated successfully.", model.Name);
        }

        public async Task RemoveOEM(long id)
        {
            var existingOEM = await _oemRepository.GetOEMById(id);
            if (existingOEM == null)
                throw new InvalidOperationException("OEM not found!");

            existingOEM.IsActive = false;
            existingOEM.ModifiedOn = DateTime.UtcNow;

            await _oemRepository.UpdateOEM(existingOEM);
            _logger.LogInformation("OEM with ID '{OEMId}' deleted successfully.", id);
        }

        public async Task<OEMMaster> GetOEMDetails(long id)
        {
            var classification = await _oemRepository.GetOEMById(id);
            if (classification == null)
                throw new InvalidOperationException("OEM not found!");

            return classification;
        }

        public async Task<PagedResponse<object>> FetchOEMList(PageFilter filter)
        {
            return await _oemRepository.GetAllOEMs(filter);
        }

        public async Task<List<DropdwonSelector>> GetOEMDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            return await _oemRepository.GetOEMDropdown(searchTerm, pageNo, pageSize);
        }
    }
}
