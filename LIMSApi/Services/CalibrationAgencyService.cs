using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;

namespace LIMSApi.Services
{
    public class CalibrationAgencyService : ICalibrationAgencyService
    {
        private readonly ICalibrationAgencyRepository _oemRepository;
        private readonly ILogger<CalibrationAgencyService> _logger;
        private readonly IFileUploadService _uploadService;
        private readonly LIMSContext _context;

        public CalibrationAgencyService(ICalibrationAgencyRepository oemRepo, ILogger<CalibrationAgencyService> logger, IFileUploadService fileUploadService, LIMSContext context)
        {
            _oemRepository = oemRepo;
            _logger = logger;
            _uploadService = fileUploadService;
            _context = context;
        }

        public async Task CreateCalibrationAgency(CalibrationAgencyMaster model)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
                throw new ArgumentException("CalibrationAgency name should not be empty!");

            bool exists = await _oemRepository.ExistsByName(model.Name);
            if (exists)
                throw new InvalidOperationException("CalibrationAgency already exists!");

            if (model.file != null)
            {
                var fileUploadResponse = await _uploadService.UploadFileAsync(model.file, FileType.Other, null, model.FileName);
                if (fileUploadResponse == null)
                    throw new InvalidOperationException("File upload failed!");
                model.AgreementFilePath = fileUploadResponse.FilePath;
                model.FileName = fileUploadResponse.OriginalFileName;
                model.UploadReferenceID = fileUploadResponse.ID;
            }

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
            existingCalibrationAgency.Address = model.Address;

            existingCalibrationAgency.ModifiedOn = DateTime.UtcNow;

            if (model.file != null)
            {
                var fileUploadResponse = await _uploadService.UploadFileAsync(model.file, FileType.Other, null, model.FileName);
                if (fileUploadResponse == null)
                    throw new InvalidOperationException("File upload failed!");
                existingCalibrationAgency.AgreementFilePath = fileUploadResponse.FilePath;
                existingCalibrationAgency.FileName = fileUploadResponse.OriginalFileName;
                existingCalibrationAgency.UploadReferenceID = fileUploadResponse.ID;
            }
            else if (string.IsNullOrEmpty(model.FileName) && existingCalibrationAgency.UploadReferenceID != null)
            {
                await _uploadService.RemoveFileAsync((long)existingCalibrationAgency.UploadReferenceID);
            }

            await _oemRepository.UpdateCalibrationAgency(existingCalibrationAgency);
            _logger.LogInformation("CalibrationAgency '{CalibrationAgencyName}' updated successfully.", model.Name);
        }

        public async Task RemoveCalibrationAgency(long id)
        {
            var existingCalibrationAgency = await _oemRepository.GetCalibrationAgencyById(id);
            if (existingCalibrationAgency == null)
                throw new InvalidOperationException("CalibrationAgency not found!");

            await DeleteValidationHelper.ValidateDeleteAsync<CalibrationAgencyMaster>(_context, id, "Calibration Agency");

            existingCalibrationAgency.IsActive = false;
            existingCalibrationAgency.ModifiedOn = DateTime.UtcNow;
            if (existingCalibrationAgency.UploadReferenceID != null)
            {
                await _uploadService.RemoveFileAsync((long)existingCalibrationAgency.UploadReferenceID);
                existingCalibrationAgency.AgreementFilePath = null;
                existingCalibrationAgency.FileName = null;
                existingCalibrationAgency.UploadReferenceID = null;
            }
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
