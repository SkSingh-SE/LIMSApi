using System;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Helpers.Enums;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;

namespace LIMSApi.Services
{
    public class TestMethodSpecificationService : ITestMethodSpecificationService
    {
        private readonly ITestMethodSpecificationRepository _TestMethodSpecificationRepository;
        private readonly ILogger<TestMethodSpecificationService> _logger;
        private LoggedInUserDTO loggedInUser;
        private readonly IFileUploadService _uploadService;

        public TestMethodSpecificationService(ITestMethodSpecificationRepository TestMethodSpecificationRepo, ILogger<TestMethodSpecificationService> logger, IFileUploadService uploadService)
        {
            _TestMethodSpecificationRepository = TestMethodSpecificationRepo;
            _logger = logger;
            loggedInUser = LoggedInUserProvider.CurrentUser;
            _uploadService = uploadService;
        }

        public async Task CreateTestMethodSpecification(TestMethodSpecification model)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
                throw new ArgumentException("TestMethodSpecification name should not be empty!");

            bool exists = await _TestMethodSpecificationRepository.ExistsByName(model.Name);
            if (exists)
                throw new InvalidOperationException("TestMethodSpecification already exists!");

            model.CreatedOn = DateTime.UtcNow;
            model.CreatedBy = loggedInUser.EmployeeID;
            model.CompanyCode = loggedInUser.CompanyCode;

            if (model.Versions.Any())
            {
                var activeCount = model.Versions.Count(v => v.Status == VersionStatus.Active);
                if (activeCount > 1)
                    throw new InvalidOperationException("Only one version can be Active at a time!");

                foreach (var version in model.Versions)
                {
                    version.TestMethodSpecificationID = model.ID;
                    version.CreatedBy = loggedInUser.EmployeeID;
                    version.CreatedOn = DateTime.UtcNow;

                    if (version.Status == VersionStatus.Active && version.EffectiveDate == null)
                    {
                        version.EffectiveDate = DateTime.UtcNow;
                    }

                    if (version.file != null)
                    {
                        var fileUploadResponse = await _uploadService.UploadFileAsync(version.file, FileType.Other, null, version.StandardFile);
                        if (fileUploadResponse == null)
                            throw new InvalidOperationException("File upload failed!");
                        version.StandardFilePath = fileUploadResponse.FilePath;
                        version.StandardFile = fileUploadResponse.OriginalFileName;
                        version.UploadReferenceID = fileUploadResponse.ID;
                    }
                }

                // Pick the default version: explicit flag → Active → first. Then ensure only one has IsDefault.
                var defaultVersion = model.Versions.FirstOrDefault(v => v.IsDefault)
                    ?? model.Versions.FirstOrDefault(v => v.Status == VersionStatus.Active)
                    ?? model.Versions.First();

                foreach (var v in model.Versions)
                    v.IsDefault = v == defaultVersion;
            }

            await _TestMethodSpecificationRepository.AddTestMethodSpecification(model);
            _logger.LogInformation("TestMethodSpecification '{TestMethodSpecificationName}' created successfully.", model.Name);
        }

        public async Task ModifyTestMethodSpecification(TestMethodSpecification model)
        {
            if (model.ID == 0)
                throw new ArgumentException("TestMethodSpecification ID should not be empty!");

            bool exists = await _TestMethodSpecificationRepository.ExistsByNameAndNotId(model.Name, model.ID);
            if (exists)
                throw new InvalidOperationException("Same TestMethodSpecification already exists!");

            var existingTestMethodSpecification = await _TestMethodSpecificationRepository.GetTestMethodSpecificationById(model.ID);
            if (existingTestMethodSpecification == null)
                throw new InvalidOperationException("TestMethodSpecification not found!");

            existingTestMethodSpecification.Name = model.Name;
            existingTestMethodSpecification.Part = model.Part;
            existingTestMethodSpecification.DisplayTitle = model.DisplayTitle;
            existingTestMethodSpecification.StandardOrganizationID = model.StandardOrganizationID;
            existingTestMethodSpecification.TestMethodStandard = model.TestMethodStandard;
            existingTestMethodSpecification.IsDisabled = model.IsDisabled;
            existingTestMethodSpecification.LinkedStandard = model.LinkedStandard;
            existingTestMethodSpecification.FormulaExpression = model.FormulaExpression;
            existingTestMethodSpecification.DefaultParameters = model.DefaultParameters;
            existingTestMethodSpecification.ModifiedOn = DateTime.UtcNow;
            existingTestMethodSpecification.ModifiedBy = loggedInUser.EmployeeID;

            // Sync metal classifications (replace set)
            existingTestMethodSpecification.MetalClassifications.Clear();
            foreach (var mc in model.MetalClassifications)
            {
                existingTestMethodSpecification.MetalClassifications.Add(new TestMethodSpecificationMetalClassification
                {
                    TestMethodSpecificationID = existingTestMethodSpecification.ID,
                    MetalClassificationID = mc.MetalClassificationID
                });
            }

            var activeCount = model.Versions.Count(v => v.Status == VersionStatus.Active);
            if (activeCount > 1)
                throw new InvalidOperationException("Only one version can be Active at a time!");

            var updatedVersionIds = model.Versions.Select(v => v.ID).ToHashSet();

            var toRemove = existingTestMethodSpecification.Versions
                .Where(v => !updatedVersionIds.Contains(v.ID))
                .ToList();
            foreach (var item in toRemove)
            {
                existingTestMethodSpecification.Versions.Remove(item);
            }

            foreach (var versionModel in model.Versions)
            {
                if (versionModel.file != null)
                {
                    var fileUploadResponse = await _uploadService.UploadFileAsync(versionModel.file, FileType.Other, null, versionModel.StandardFile);
                    if (fileUploadResponse == null)
                        throw new InvalidOperationException("File upload failed!");
                    versionModel.StandardFilePath = fileUploadResponse.FilePath;
                    versionModel.StandardFile = fileUploadResponse.OriginalFileName;
                    versionModel.UploadReferenceID = fileUploadResponse.ID;
                }
                var existingVersion = existingTestMethodSpecification.Versions.FirstOrDefault(v => v.ID == versionModel.ID);

                if (existingVersion != null)
                {
                    // Lifecycle transition: activating a version that was not active before
                    if (versionModel.Status == VersionStatus.Active && existingVersion.Status != VersionStatus.Active)
                    {
                        var currentActive = existingTestMethodSpecification.Versions
                            .FirstOrDefault(v => v.Status == VersionStatus.Active && v.ID != versionModel.ID);
                        if (currentActive != null)
                        {
                            currentActive.Status = VersionStatus.Superseded;
                            currentActive.SupersededDate = DateTime.UtcNow;
                        }
                        versionModel.EffectiveDate = DateTime.UtcNow;
                    }

                    existingVersion.Version = versionModel.Version;
                    existingVersion.StandardFile = versionModel.StandardFile;
                    existingVersion.StandardFilePath = versionModel.StandardFilePath;
                    existingVersion.Status = versionModel.Status;
                    existingVersion.Year = versionModel.Year;
                    existingVersion.EffectiveDate = versionModel.EffectiveDate;
                    existingVersion.SupersededDate = versionModel.SupersededDate;
                    existingVersion.ReviewDate = versionModel.ReviewDate;
                    existingVersion.ChangeReason = versionModel.ChangeReason;
                    existingVersion.UploadReferenceID = versionModel.UploadReferenceID;
                    existingVersion.IsDefault = versionModel.IsDefault;

                    // Sync this version's parameters (replace set).
                    existingVersion.Parameters.Clear();
                    foreach (var p in BuildVersionParameters(versionModel))
                        existingVersion.Parameters.Add(p);
                }
                else
                {
                    if (versionModel.Status == VersionStatus.Active)
                    {
                        var currentActive = existingTestMethodSpecification.Versions
                            .FirstOrDefault(v => v.Status == VersionStatus.Active);
                        if (currentActive != null)
                        {
                            currentActive.Status = VersionStatus.Superseded;
                            currentActive.SupersededDate = DateTime.UtcNow;
                        }
                        versionModel.EffectiveDate ??= DateTime.UtcNow;
                    }

                    var newVersion = new TestMethodSpecificationVersion
                    {
                        Version = versionModel.Version,
                        StandardFile = versionModel.StandardFile,
                        StandardFilePath = versionModel.StandardFilePath,
                        Status = versionModel.Status,
                        Year = versionModel.Year,
                        EffectiveDate = versionModel.EffectiveDate,
                        SupersededDate = versionModel.SupersededDate,
                        ReviewDate = versionModel.ReviewDate,
                        ChangeReason = versionModel.ChangeReason,
                        UploadReferenceID = versionModel.UploadReferenceID,
                        IsDefault = versionModel.IsDefault,
                        CreatedBy = loggedInUser.EmployeeID,
                        CreatedOn = DateTime.UtcNow,
                        Parameters = BuildVersionParameters(versionModel)
                    };
                    existingTestMethodSpecification.Versions.Add(newVersion);
                }
            }

            // Ensure exactly one version has IsDefault = true.
            EnsureSingleDefault(existingTestMethodSpecification.Versions.ToList());

            // CRITICAL: To avoid violating the unique index on (TestMethodSpecificationID, IsDefault),
            // we must handle the IsDefault updates carefully. First, set all to false except the one we want,
            // then update via the repository with proper ordering.
            var versionToSetDefault = existingTestMethodSpecification.Versions.FirstOrDefault(v => v.IsDefault);
            
            // Clear IsDefault on all versions before saving
            foreach (var v in existingTestMethodSpecification.Versions)
            {
                if (v != versionToSetDefault)
                    v.IsDefault = false;
            }

            // Now ensure the target has IsDefault = true
            if (versionToSetDefault != null)
                versionToSetDefault.IsDefault = true;

            await _TestMethodSpecificationRepository.UpdateTestMethodSpecification(existingTestMethodSpecification);
            _logger.LogInformation("TestMethodSpecification '{TestMethodSpecificationName}' updated successfully.", model.Name);
        }

        // Builds version-level parameter entities from a version DTO/model.
        private static List<TestMethodSpecificationParameter> BuildVersionParameters(TestMethodSpecificationVersion versionModel)
        {
            return (versionModel.Parameters ?? new List<TestMethodSpecificationParameter>())
                .Where(p => p.ParameterID > 0)
                .Select((p, idx) => new TestMethodSpecificationParameter
                {
                    ParameterID = p.ParameterID,
                    ParameterUnitID = p.ParameterUnitID,
                    ParameterUnitEquivalentID = p.ParameterUnitEquivalentID,
                    Comment = p.Comment,
                    SortOrder = p.SortOrder != 0 ? p.SortOrder : idx
                }).ToList();
        }

        /// <summary>
        /// Ensures exactly one version in the collection has IsDefault = true.
        /// Priority: version already flagged → Active → first.
        /// </summary>
        private static void EnsureSingleDefault(List<TestMethodSpecificationVersion> versions)
        {
            if (!versions.Any()) return;

            var currentDefault = versions.FirstOrDefault(v => v.IsDefault);
            if (currentDefault != null)
            {
                // Clear all others, keep the flagged one.
                foreach (var v in versions.Where(v => v != currentDefault))
                    v.IsDefault = false;
                return;
            }

            // No version flagged — pick one.
            var fallback = versions.FirstOrDefault(v => v.Status == VersionStatus.Active) ?? versions.First();
            fallback.IsDefault = true;
        }

        public async Task SetDefaultVersion(long specId, long versionId)
        {
            var spec = await _TestMethodSpecificationRepository.GetTestMethodSpecificationById(specId);
            if (spec == null)
                throw new InvalidOperationException("TestMethodSpecification not found!");

            var target = spec.Versions.FirstOrDefault(v => v.ID == versionId);
            if (target == null)
                throw new InvalidOperationException("Version not found!");

            // Toggle IsDefault: set target to true, all others to false.
            foreach (var v in spec.Versions)
                v.IsDefault = v.ID == versionId;

            spec.ModifiedOn = DateTime.UtcNow;
            spec.ModifiedBy = loggedInUser.EmployeeID;

            await _TestMethodSpecificationRepository.UpdateTestMethodSpecification(spec);
            _logger.LogInformation("Default version '{VersionId}' set for TestMethodSpecification '{SpecId}'.", versionId, specId);
        }

        public async Task RemoveTestMethodSpecification(long id)
        {
            var existingTestMethodSpecification = await _TestMethodSpecificationRepository.GetTestMethodSpecificationById(id);
            if (existingTestMethodSpecification == null)
                throw new InvalidOperationException("TestMethodSpecification not found!");

            existingTestMethodSpecification.IsActive = false;
            existingTestMethodSpecification.ModifiedOn = DateTime.UtcNow;
            existingTestMethodSpecification.ModifiedBy = loggedInUser.EmployeeID;

            await _TestMethodSpecificationRepository.UpdateTestMethodSpecification(existingTestMethodSpecification);
            _logger.LogInformation("TestMethodSpecification with ID '{TestMethodSpecificationId}' deleted successfully.", id);
        }

        public async Task EnableDisableTestMethodSpecification(long id)
        {
            var existingTestMethodSpecification = await _TestMethodSpecificationRepository.GetTestMethodSpecificationById(id);
            if (existingTestMethodSpecification == null)
                throw new InvalidOperationException("TestMethodSpecification not found!");

            existingTestMethodSpecification.IsDisabled = !existingTestMethodSpecification.IsDisabled;
            existingTestMethodSpecification.ModifiedOn = DateTime.UtcNow;
            existingTestMethodSpecification.ModifiedBy = loggedInUser.EmployeeID;

            await _TestMethodSpecificationRepository.UpdateTestMethodSpecification(existingTestMethodSpecification);
            _logger.LogInformation("TestMethodSpecification with ID '{TestMethodSpecificationId}' {Action} successfully.", id, existingTestMethodSpecification.IsDisabled ? "disabled" : "enabled");
        }

        public async Task<TestMethodSpecification> GetTestMethodSpecificationDetails(long id)
        {
            var classification = await _TestMethodSpecificationRepository.GetTestMethodSpecificationById(id);
            if (classification == null)
                throw new InvalidOperationException("TestMethodSpecification not found!");

            return classification;
        }

        public async Task<PagedResponse<object>> FetchTestMethodSpecificationList(PageFilter filter)
        {
            return await _TestMethodSpecificationRepository.GetAllTestMethodSpecifications(filter);
        }

        public async Task<List<DropdwonSelector>> GetTestMethodSpecificationDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            return await _TestMethodSpecificationRepository.GetTestMethodSpecificationDropdown(searchTerm, pageNo, pageSize);
        }

        public async Task<List<DropdwonSelector>> GetTestMethodsByStandard(long standardId)
        {
            return await _TestMethodSpecificationRepository.GetTestMethodSpecificationsByStandard(standardId);
        }

        public async Task<List<DropdwonSelector>> GetTestMethodsByMetalClassification(long metalClassificationId, string? searchTerm, int pageNo, int pageSize)
        {
            return await _TestMethodSpecificationRepository.GetTestMethodsByMetalClassification(metalClassificationId, searchTerm, pageNo, pageSize);
        }

        public async Task ActivateVersion(long specId, long versionId)
        {
            var spec = await _TestMethodSpecificationRepository.GetTestMethodSpecificationById(specId);
            if (spec == null)
                throw new InvalidOperationException("TestMethodSpecification not found!");

            var targetVersion = spec.Versions.FirstOrDefault(v => v.ID == versionId);
            if (targetVersion == null)
                throw new InvalidOperationException("Version not found!");

            if (targetVersion.Status != VersionStatus.Draft && targetVersion.Status != VersionStatus.Withdrawn)
                throw new InvalidOperationException("Only Draft or Withdrawn versions can be activated!");

            var currentActive = spec.Versions.FirstOrDefault(v => v.Status == VersionStatus.Active);
            if (currentActive != null)
            {
                currentActive.Status = VersionStatus.Superseded;
                currentActive.SupersededDate = DateTime.UtcNow;
            }

            targetVersion.Status = VersionStatus.Active;
            targetVersion.EffectiveDate = DateTime.UtcNow;

            // Activating a version also makes it the default selection.
            foreach (var v in spec.Versions)
                v.IsDefault = v.ID == versionId;

            spec.ModifiedOn = DateTime.UtcNow;
            spec.ModifiedBy = loggedInUser.EmployeeID;

            await _TestMethodSpecificationRepository.UpdateTestMethodSpecification(spec);
            _logger.LogInformation("Version '{VersionId}' activated for TestMethodSpecification '{SpecId}'.", versionId, specId);
        }

        public async Task WithdrawVersion(long specId, long versionId, string reason)
        {
            var spec = await _TestMethodSpecificationRepository.GetTestMethodSpecificationById(specId);
            if (spec == null)
                throw new InvalidOperationException("TestMethodSpecification not found!");

            var targetVersion = spec.Versions.FirstOrDefault(v => v.ID == versionId);
            if (targetVersion == null)
                throw new InvalidOperationException("Version not found!");

            if (targetVersion.Status == VersionStatus.Withdrawn)
                throw new InvalidOperationException("Version is already withdrawn!");

            targetVersion.Status = VersionStatus.Withdrawn;
            targetVersion.ChangeReason = reason;
            targetVersion.SupersededDate = DateTime.UtcNow;

            spec.ModifiedOn = DateTime.UtcNow;
            spec.ModifiedBy = loggedInUser.EmployeeID;

            await _TestMethodSpecificationRepository.UpdateTestMethodSpecification(spec);
            _logger.LogInformation("Version '{VersionId}' withdrawn.", versionId);
        }

        public async Task<int> GetVersionImpactCount(long versionId)
        {
            return await _TestMethodSpecificationRepository.GetVersionImpactCount(versionId);
        }

        public async Task<List<DropdwonSelector>> GetVersionsBySpecId(long specId, bool includeAll = false)
        {
            return await _TestMethodSpecificationRepository.GetVersionsBySpecId(specId, includeAll);
        }

        public async Task<List<DropdwonSelector>> GetTestMethodSpecificationVersionDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            return await _TestMethodSpecificationRepository.GetTestMethodSpecificationVersionDropdown(searchTerm, pageNo, pageSize);
        }
    }
}
