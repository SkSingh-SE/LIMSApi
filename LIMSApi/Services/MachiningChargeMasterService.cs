using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;

namespace LIMSApi.Services
{
    public class MachiningChargeMasterService : IMachiningChargeMasterService
    {
        private readonly IMachiningChargeMasterRepository _repository;
        private readonly ILogger<MachiningChargeMasterService> _logger;
        private readonly IFileUploadService _uploadService;
        private LoggedInUserDTO loggedInUser;

        public MachiningChargeMasterService(
            IMachiningChargeMasterRepository repository,
            ILogger<MachiningChargeMasterService> logger,
            IFileUploadService uploadService)
        {
            _repository = repository;
            _logger = logger;
            _uploadService = uploadService;
            loggedInUser = LoggedInUserProvider.CurrentUser;
        }

        public async Task CreateMachiningChargeMaster(MachiningChargeMaster model)
        {
            if (string.IsNullOrWhiteSpace(model.SpecimenSize))
                throw new ArgumentException("Specimen Size should not be empty!");

            ValidateVersions(model.Versions);

            bool exists = await _repository.ExistsBySpecimenSizeAndTest(
                model.SpecimenSize, model.LaboratoryTestID, model.TestMethodStandardID);
            if (exists)
                throw new InvalidOperationException("A record with the same Specimen Size, Laboratory Test, and Test Method Standard already exists!");

            if (model.file != null)
            {
                var fileUploadResponse = await _uploadService.UploadFileAsync(model.file, FileType.Other, null, model.FileName);
                if (fileUploadResponse == null)
                    throw new InvalidOperationException("File upload failed!");
                model.DrawingFilePath = fileUploadResponse.FilePath;
                model.FileName = fileUploadResponse.OriginalFileName;
                model.UploadReferenceID = fileUploadResponse.ID;
            }

            model.CreatedOn = DateTime.UtcNow;
            model.CreatedBy = loggedInUser.EmployeeID;
            model.CompanyCode = loggedInUser.CompanyCode;

            foreach (var version in model.Versions)
            {
                version.ID = 0;
                version.CreatedOn = DateTime.UtcNow;
                version.CreatedBy = loggedInUser.EmployeeID;
                version.ModifiedOn = null;
                version.ModifiedBy = null;
                version.CompanyCode = loggedInUser.CompanyCode;
                version.IsActive = true;
            }

            await _repository.AddMachiningChargeMaster(model);
            _logger.LogInformation("MachiningChargeMaster '{SpecimenSize}' created successfully.", model.SpecimenSize);
        }

        public async Task ModifyMachiningChargeMaster(MachiningChargeMaster model)
        {
            if (model.ID == 0)
                throw new ArgumentException("MachiningChargeMaster ID should not be empty!");

            ValidateVersions(model.Versions);

            bool exists = await _repository.ExistsBySpecimenSizeAndTestAndNotId(
                model.SpecimenSize, model.LaboratoryTestID, model.TestMethodStandardID, model.ID);
            if (exists)
                throw new InvalidOperationException("A record with the same Specimen Size, Laboratory Test, and Test Method Standard already exists!");

            var existing = await _repository.GetMachiningChargeMasterById(model.ID);
            if (existing == null)
                throw new InvalidOperationException("MachiningChargeMaster not found!");

            existing.LaboratoryTestID = model.LaboratoryTestID;
            existing.TestMethodStandardID = model.TestMethodStandardID;
            existing.SpecimenRawMaterialSize = model.SpecimenRawMaterialSize;
            existing.SpecimenSize = model.SpecimenSize;
            existing.Remark = model.Remark;
            existing.ModifiedOn = DateTime.UtcNow;
            existing.ModifiedBy = loggedInUser.EmployeeID;

            if (model.file != null)
            {
                var fileUploadResponse = await _uploadService.UploadFileAsync(model.file, FileType.Other, null, model.FileName);
                if (fileUploadResponse == null)
                    throw new InvalidOperationException("File upload failed!");
                if (existing.UploadReferenceID != null)
                    await _uploadService.RemoveFileAsync((long)existing.UploadReferenceID);
                existing.DrawingFilePath = fileUploadResponse.FilePath;
                existing.FileName = fileUploadResponse.OriginalFileName;
                existing.UploadReferenceID = fileUploadResponse.ID;
            }
            else if (string.IsNullOrEmpty(model.FileName) && existing.UploadReferenceID != null)
            {
                await _uploadService.RemoveFileAsync((long)existing.UploadReferenceID);
                existing.DrawingFilePath = null;
                existing.FileName = null;
                existing.UploadReferenceID = null;
            }

            SyncVersions(existing, model.Versions);

            await _repository.SaveChangesAsync();
            _logger.LogInformation("MachiningChargeMaster '{SpecimenSize}' updated successfully.", model.SpecimenSize);
        }

        public async Task RemoveMachiningChargeMaster(long id)
        {
            var existing = await _repository.GetMachiningChargeMasterById(id);
            if (existing == null)
                throw new InvalidOperationException("MachiningChargeMaster not found!");

            existing.IsActive = false;
            existing.ModifiedOn = DateTime.UtcNow;
            existing.ModifiedBy = loggedInUser.EmployeeID;
            if (existing.UploadReferenceID != null)
            {
                await _uploadService.RemoveFileAsync((long)existing.UploadReferenceID);
                existing.DrawingFilePath = null;
                existing.FileName = null;
                existing.UploadReferenceID = null;
            }

            await _repository.SaveChangesAsync();
            _logger.LogInformation("MachiningChargeMaster with ID '{Id}' deleted successfully.", id);
        }

        public async Task<MachiningChargeMaster> GetMachiningChargeMasterDetails(long id)
        {
            var entity = await _repository.GetMachiningChargeMasterById(id);
            if (entity == null)
                throw new InvalidOperationException("MachiningChargeMaster not found!");
            return entity;
        }

        public async Task<PagedResponse<object>> FetchMachiningChargeMasterList(PageFilter filter)
        {
            return await _repository.GetAllMachiningChargeMasters(filter);
        }

        public async Task<List<DropdwonSelector>> GetMachiningChargeMasterDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            return await _repository.GetMachiningChargeMasterDropdown(searchTerm, pageNo, pageSize);
        }

        public async Task<List<MachiningChargeMaster>> GetByLabTestAndStandard(long labTestId, long standardId)
        {
            return await _repository.GetByLabTestAndStandard(labTestId, standardId);
        }

        // At least one version required; each must have a Financial Year + valid EffectiveFrom; no duplicate dates.
        private static void ValidateVersions(ICollection<MachiningChargeVersion> versions)
        {
            if (versions == null || versions.Count == 0)
                throw new ArgumentException("At least one price version is required!");
            if (versions.Any(v => !v.FinancialYearId.HasValue))
                throw new ArgumentException("Each price version must have a Financial Year selected!");
            if (versions.Any(v => v.EffectiveFrom == default))
                throw new ArgumentException("Each price version must have an Effective From date!");
            if (versions.GroupBy(v => v.EffectiveFrom.Date).Any(g => g.Count() > 1))
                throw new ArgumentException("Duplicate Effective From date in price versions — each date can appear only once!");
        }

        // Reconciles tracked Versions against incoming list:
        // update matched rows, add new ones, soft-delete removed ones.
        private void SyncVersions(MachiningChargeMaster existing, ICollection<MachiningChargeVersion> incoming)
        {
            var incomingIds = incoming.Where(v => v.ID > 0).Select(v => v.ID).ToHashSet();

            foreach (var stale in existing.Versions.Where(v => v.IsActive && !incomingIds.Contains(v.ID)))
            {
                stale.IsActive = false;
                stale.ModifiedOn = DateTime.UtcNow;
                stale.ModifiedBy = loggedInUser.EmployeeID;
            }

            foreach (var v in incoming)
            {
                if (v.ID > 0)
                {
                    var current = existing.Versions.FirstOrDefault(x => x.ID == v.ID);
                    if (current == null) continue;
                    current.EffectiveFrom = v.EffectiveFrom;
                    current.FinancialYearId = v.FinancialYearId;
                    current.PriceGeneralMetal = v.PriceGeneralMetal;
                    current.PriceHardMetal = v.PriceHardMetal;
                    current.ModifiedOn = DateTime.UtcNow;
                    current.ModifiedBy = loggedInUser.EmployeeID;
                }
                else
                {
                    existing.Versions.Add(new MachiningChargeVersion
                    {
                        EffectiveFrom = v.EffectiveFrom,
                        FinancialYearId = v.FinancialYearId,
                        PriceGeneralMetal = v.PriceGeneralMetal,
                        PriceHardMetal = v.PriceHardMetal,
                        CreatedOn = DateTime.UtcNow,
                        CreatedBy = loggedInUser.EmployeeID,
                        ModifiedOn = null,
                        ModifiedBy = null,
                        CompanyCode = loggedInUser.CompanyCode,
                        IsActive = true
                    });
                }
            }
        }
    }
}
