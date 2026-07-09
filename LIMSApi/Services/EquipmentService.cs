using System.Text.RegularExpressions;
using LIMSApi.Data;
using Microsoft.EntityFrameworkCore;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;

namespace LIMSApi.Services
{
    public class EquipmentService : IEquipmentService
    {
        private readonly IEquipmentRepository _equipmentRepository;
        private readonly ILogger<EquipmentService> _logger;
        private readonly IFileUploadService _uploadService;
        private readonly LIMSContext _context;
        public EquipmentService(IEquipmentRepository equipment, ILogger<EquipmentService> logger, IFileUploadService uploadService, LIMSContext context)
        {
            _equipmentRepository = equipment;
            _logger = logger;
            _uploadService = uploadService;
            _context = context;
        }

        public async Task CreateEquipment(EquipmentMaster model)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
                throw new ArgumentException("Equipment name should not be empty!");

            bool exists = await _equipmentRepository.ExistsByName(model.Name);
            if (exists)
                throw new InvalidOperationException("Equipment already exists!");

            await _equipmentRepository.AddEquipment(model);

            if (model.AnalysisTechniques != null && model.AnalysisTechniques.Any())
            {
                foreach (var t in model.AnalysisTechniques)
                {
                    t.EquipmentID = model.ID;
                    _context.EquipmentAnalysisTechniques.Add(t);
                }
                await _context.SaveChangesAsync();
            }

            _logger.LogInformation("Equipment '{EquipmentName}' created successfully.", model.Name);
        }

        public async Task ModifyEquipment(EquipmentMaster model)
        {
            if (model.ID == 0)
                throw new ArgumentException("Equipment ID should not be empty!");

            bool exists = await _equipmentRepository.ExistsByNameAndNotId(model.Name, model.ID);
            if (exists)
                throw new InvalidOperationException("Same Equipment already exists!");

            var existingEquipment = await _equipmentRepository.GetEquipmentById(model.ID);
            if (existingEquipment == null)
                throw new InvalidOperationException("Equipment not found!");

            existingEquipment.Name = model.Name;
            existingEquipment.EquipmentNo = model.EquipmentNo;
            existingEquipment.DepartmentID = model.DepartmentID;
            existingEquipment.EquipmentTypeID = model.EquipmentTypeID;
            existingEquipment.OEMID = model.OEMID;
            existingEquipment.ModelNo = model.ModelNo;
            existingEquipment.PurchaseDate = model.PurchaseDate;
            existingEquipment.CalibrationRequired = model.CalibrationRequired;
            existingEquipment.MaintenanceRequired = model.MaintenanceRequired;
            existingEquipment.MaintenanceInterval = model.MaintenanceInterval;
            existingEquipment.InternalExternal = model.InternalExternal;
            existingEquipment.IntermediateCheckRequired = model.IntermediateCheckRequired;
            existingEquipment.IntermediateCheckInterval = model.IntermediateCheckInterval;
            existingEquipment.NextCalibrationDueDate = model.NextCalibrationDueDate;
            existingEquipment.NextMaintenanceDueDate = model.NextMaintenanceDueDate;
            existingEquipment.LastCalibrationDate = model.LastCalibrationDate;
            existingEquipment.CalibrationFrequencyDays = model.CalibrationFrequencyDays;
            existingEquipment.MaintenanceSchedule = model.MaintenanceSchedule;
            existingEquipment.ModifiedOn = DateTime.UtcNow;

            await SyncAnalysisTechniques(existingEquipment, model.AnalysisTechniques);

            await _equipmentRepository.UpdateEquipment(existingEquipment);
            _logger.LogInformation("Equipment '{EquipmentName}' updated successfully.", model.Name);
        }

        public async Task AddEquipmentCalibration(EquipmentCalibration model)
        {
            if (model.EquipmentID == 0)
                throw new ArgumentException("Equipment ID should not be empty!");


            var existingEquipment = await _equipmentRepository.GetEquipmentById(model.EquipmentID);
            if (existingEquipment == null)
                throw new InvalidOperationException("Equipment not found!");

            if (model.File != null)
            {
                var fileUploadResponse = await _uploadService.UploadFileAsync(model.File, FileType.Other, null, model.Certificate);
                if (fileUploadResponse == null)
                    throw new InvalidOperationException("File upload failed!");
                model.CertificatePath = fileUploadResponse.FilePath;
                model.Certificate = fileUploadResponse.OriginalFileName;
                model.UploadReferenceID = fileUploadResponse.ID;

            }
            existingEquipment.NextCalibrationDueDate = model.CalibrationDueDate;
            if (existingEquipment.Calibrations != null && existingEquipment.Calibrations.Any())
            {
                existingEquipment.Calibrations.Add(model);
            }
            else
            {
                existingEquipment.Calibrations = new List<EquipmentCalibration> { model };
            }

            await _equipmentRepository.UpdateEquipment(existingEquipment);
            _logger.LogInformation("Equipment Calibration added successfully.");
        }

        public async Task AddEquipmentMaintenance(EquipmentMaintenance model)
        {
            if (model.EquipmentID == 0)
                throw new ArgumentException("Equipment ID should not be empty!");

            var existingEquipment = await _equipmentRepository.GetEquipmentById(model.EquipmentID);
            if (existingEquipment == null)
                throw new InvalidOperationException("Equipment not found!");

            if (model.File != null)
            {
                var fileUploadResponse = await _uploadService.UploadFileAsync(model.File, FileType.Other, null, model.Certificate);
                if (fileUploadResponse == null)
                    throw new InvalidOperationException("File upload failed!");
                model.CertificatePath = fileUploadResponse.FilePath;
                model.Certificate = fileUploadResponse.OriginalFileName;
                model.UploadReferenceID = fileUploadResponse.ID;

            }
            if (existingEquipment.MaintenanceInterval != null)
            {
                existingEquipment.NextMaintenanceDueDate = CalculateDueDate(model.MaintenanceDate, existingEquipment.MaintenanceInterval);
            }

            if (existingEquipment.Maintenances != null && existingEquipment.Maintenances.Any())
            {
                existingEquipment.Maintenances.Add(model);
            }
            else
            {
                existingEquipment.Maintenances = new List<EquipmentMaintenance> { model };
            }

            await _equipmentRepository.UpdateEquipment(existingEquipment);
            _logger.LogInformation("Equipment Maintainence added successfully.");
        }
        public async Task AddEquipmentSOP(EquipmentSOP model)
        {
            if (model.EquipmentID == 0)
                throw new ArgumentException("Equipment ID should not be empty!");

            var existingEquipment = await _equipmentRepository.GetEquipmentById(model.EquipmentID);
            if (existingEquipment == null)
                throw new InvalidOperationException("Equipment not found!");

            if (model.File != null)
            {
                var fileUploadResponse = await _uploadService.UploadFileAsync(model.File, FileType.Other, null, model.FileName);
                if (fileUploadResponse == null)
                    throw new InvalidOperationException("File upload failed!");
                model.FilePath = fileUploadResponse.FilePath;
                model.FileName = fileUploadResponse.OriginalFileName;
                model.UploadReferenceID = fileUploadResponse.ID;

            }

            if (existingEquipment.SOPs != null && existingEquipment.SOPs.Any())
            {
                existingEquipment.SOPs.Add(model);
            }
            else
            {
                existingEquipment.SOPs = new List<EquipmentSOP> { model };
            }

            await _equipmentRepository.UpdateEquipment(existingEquipment);
            _logger.LogInformation("Equipment Maintainence added successfully.");
        }
        public static DateTime CalculateDueDate(DateTime startDate, string interval)
        {

            var match = Regex.Match(interval, @"(\d+)?\s*(EveryDay|Weekly|Month|Months|Year|Years)", RegexOptions.IgnoreCase);
            if (!match.Success)
                return startDate;

            int value = string.IsNullOrEmpty(match.Groups[1].Value) ? 1 : int.Parse(match.Groups[1].Value);
            string unitRaw = match.Groups[2].Value.ToLower();

            DateTime date = startDate;

            switch (unitRaw)
            {
                case "everyday":
                    date = date.AddDays(1);
                    break;

                case "weekly":
                    date = date.AddDays(7);
                    break;

                case "month":
                case "months":
                    date = date.AddMonths(value);
                    break;

                case "year":
                case "years":
                    date = date.AddYears(value);
                    break;

                default:
                    // No change
                    break;
            }

            return date;
        }
        public async Task ReviewCalibration(long calibrationId)
        {
            var calibration = await _context.Set<EquipmentCalibration>().FirstOrDefaultAsync(c => c.ID == calibrationId);
            if (calibration == null)
                throw new KeyNotFoundException("Calibration record not found!");

            calibration.IsReviewed = true;
            await _context.SaveChangesAsync();
            _logger.LogInformation("Calibration '{CalibrationId}' marked as reviewed.", calibrationId);
        }

        public async Task RemoveEquipment(long id)
        {
            var existingEquipment = await _equipmentRepository.GetEquipmentById(id);
            if (existingEquipment == null)
                throw new InvalidOperationException("Equipment not found!");

            await DeleteValidationHelper.ValidateDeleteAsync<EquipmentMaster>(_context, id, "Equipment");

            existingEquipment.IsActive = false;
            existingEquipment.ModifiedOn = DateTime.UtcNow;

            await _equipmentRepository.UpdateEquipment(existingEquipment);
            _logger.LogInformation("Equipment with ID '{EquipmentId}' deleted successfully.", id);
        }

        public async Task RemoveEquipmentSOP(EquipmentSOP sop)
        {
            var existingEquipment = await _equipmentRepository.GetEquipmentById(sop.EquipmentID);
            if (existingEquipment == null)
                throw new InvalidOperationException("Equipment not found!");

            var removeSOP = existingEquipment.SOPs?.Where(s => s.ID == sop.ID).FirstOrDefault();
            if (removeSOP != null)
            {
                if(removeSOP.UploadReferenceID != null)
                {
                     await _uploadService.RemoveFileAsync(removeSOP.UploadReferenceID.Value);
                }
                existingEquipment.SOPs?.Remove(removeSOP);
            }

            await _equipmentRepository.UpdateEquipment(existingEquipment);
            _logger.LogInformation("Equipment SOP '{sopId}' deleted successfully.", sop.ID);
        }

        public async Task<EquipmentMaster> GetEquipmentDetails(long id)
        {
            var classification = await _equipmentRepository.GetEquipmentById(id);
            if (classification == null)
                throw new InvalidOperationException("Equipment not found!");

            return classification;
        }

        public async Task<PagedResponse<object>> FetchEquipmentList(PageFilter filter)
        {
            return await _equipmentRepository.GetAllEquipments(filter);
        }

        public async Task<List<DropdwonSelector>> GetEquipmentDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            return await _equipmentRepository.GetEquipmentDropdown(searchTerm, pageNo, pageSize);
        }

        private async Task SyncAnalysisTechniques(EquipmentMaster existing, ICollection<EquipmentAnalysisTechnique>? incoming)
        {
            var existingJunctions = await _context.Set<EquipmentAnalysisTechnique>()
                .Where(j => j.EquipmentID == existing.ID)
                .ToListAsync();
            _context.Set<EquipmentAnalysisTechnique>().RemoveRange(existingJunctions);

            if (incoming != null)
            {
                foreach (var t in incoming)
                {
                    t.EquipmentID = existing.ID;
                    t.ID = 0;
                    _context.Set<EquipmentAnalysisTechnique>().Add(t);
                }
            }
            await _context.SaveChangesAsync();
        }
    }
}
