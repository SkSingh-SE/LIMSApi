using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;

namespace LIMSApi.Services
{
    public class CalibrationAgencyService : ICalibrationAgencyService
    {
        private readonly ICalibrationAgencyRepository _oemRepository;
        private readonly ILogger<CalibrationAgencyService> _logger;

        public CalibrationAgencyService(ICalibrationAgencyRepository oemRepo, ILogger<CalibrationAgencyService> logger)
        {
            _oemRepository = oemRepo;
            _logger = logger;
        }

        public async Task CreateCalibrationAgency(CalibrationAgencyMaster model)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
                throw new ArgumentException("CalibrationAgency name should not be empty!");

            bool exists = await _oemRepository.ExistsByName(model.Name);
            if (exists)
                throw new InvalidOperationException("CalibrationAgency already exists!");
            await _oemRepository.AddCalibrationAgency(model);
            _logger.LogInformation("CalibrationAgency '{CalibrationAgencyName}' created successfully.", model.Name);
        }

        public async Task ModifyCalibrationAgency(CalibrationAgencyMaster model)
        {
            if (model.ID == 0)
                throw new ArgumentException("CalibrationAgency ID should not be empty!");

            bool exists = await _oemRepository.ExistsByNameAndNotId(model.Name, model.ID);
            if (exists)
                throw new InvalidOperationException("Same CalibrationAgency already exists!");

            var existingCalibrationAgency = await _oemRepository.GetCalibrationAgencyById(model.ID);
            if (existingCalibrationAgency == null)
                throw new InvalidOperationException("CalibrationAgency not found!");

            existingCalibrationAgency.Name = model.Name;
            existingCalibrationAgency.ContactPerson1 = model.ContactPerson1;
            existingCalibrationAgency.ContactPerson2 = model.ContactPerson2;
            existingCalibrationAgency.ContactPerson3 = model.ContactPerson3;
            existingCalibrationAgency.ContactNo1 = model.ContactNo1;
            existingCalibrationAgency.ContactNo2 = model.ContactNo2;
            existingCalibrationAgency.ContactNo3 = model.ContactNo3;
            existingCalibrationAgency.EmailId1 = model.EmailId1;
            existingCalibrationAgency.EmailId2 = model.EmailId2;
            existingCalibrationAgency.EmailId3 = model.EmailId3;
            existingCalibrationAgency.AgreementFilePath = model.AgreementFilePath;

            existingCalibrationAgency.ModifiedOn = DateTime.UtcNow;

            await _oemRepository.UpdateCalibrationAgency(existingCalibrationAgency);
            _logger.LogInformation("CalibrationAgency '{CalibrationAgencyName}' updated successfully.", model.Name);
        }

        public async Task RemoveCalibrationAgency(long id)
        {
            var existingCalibrationAgency = await _oemRepository.GetCalibrationAgencyById(id);
            if (existingCalibrationAgency == null)
                throw new InvalidOperationException("CalibrationAgency not found!");

            existingCalibrationAgency.IsActive = false;
            existingCalibrationAgency.ModifiedOn = DateTime.UtcNow;

            await _oemRepository.UpdateCalibrationAgency(existingCalibrationAgency);
            _logger.LogInformation("CalibrationAgency with ID '{CalibrationAgencyId}' deleted successfully.", id);
        }

        public async Task<CalibrationAgencyMaster> GetCalibrationAgencyDetails(long id)
        {
            var classification = await _oemRepository.GetCalibrationAgencyById(id);
            if (classification == null)
                throw new InvalidOperationException("CalibrationAgency not found!");

            return classification;
        }

        public async Task<PagedResponse<object>> FetchCalibrationAgencyList(PageFilter filter)
        {
            return await _oemRepository.GetAllCalibrationAgencys(filter);
        }

        public async Task<List<DropdwonSelector>> GetCalibrationAgencyDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            return await _oemRepository.GetCalibrationAgencyDropdown(searchTerm, pageNo, pageSize);
        }
    }
}
