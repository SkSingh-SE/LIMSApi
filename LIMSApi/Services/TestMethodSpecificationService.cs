using System;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Helpers.Enums;
using Microsoft.AspNetCore.Http;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Services
{
    public class TestMethodSpecificationService : ITestMethodSpecificationService
    {
        private readonly ITestMethodSpecificationRepository _TestMethodSpecificationRepository;
        private readonly ILogger<TestMethodSpecificationService> _logger;
        private LoggedInUserDTO loggedInUser;
        private readonly IFileUploadService _uploadService;
        private readonly string _pdfFolderPath;

        public TestMethodSpecificationService(ITestMethodSpecificationRepository TestMethodSpecificationRepo, ILogger<TestMethodSpecificationService> logger, IFileUploadService uploadService, IConfiguration configuration, IWebHostEnvironment env)
        {
            _TestMethodSpecificationRepository = TestMethodSpecificationRepo;
            _logger = logger;
            loggedInUser = LoggedInUserProvider.CurrentUser;
            _uploadService = uploadService;

            var configuredPath = configuration["ImportSettings:TestMethodSpecPdfFolder"] ?? string.Empty;
            // Resolve relative paths against ContentRootPath (project directory)
            if (!string.IsNullOrWhiteSpace(configuredPath) && !Path.IsPathRooted(configuredPath))
            {
                _pdfFolderPath = Path.Combine(env.ContentRootPath, configuredPath);
            }
            else
            {
                _pdfFolderPath = configuredPath;
            }
        }

        public async Task CreateTestMethodSpecification(TestMethodSpecification model)
        {
            if (model.StandardOrganizationID <= 0)
                throw new ArgumentException("Standard Organization is required!");

            if (string.IsNullOrWhiteSpace(model.TestMethodStandard))
                throw new ArgumentException("Test Method Standard is required!");

            if (string.IsNullOrWhiteSpace(model.Name))
                throw new ArgumentException("TestMethodSpecification name should not be empty!");

            bool exists = await _TestMethodSpecificationRepository.ExistsByOrgAndStandard(model.StandardOrganizationID, model.TestMethodStandard);
            if (exists)
            {
                throw new InvalidOperationException($"Test Method Specification for Standard '{model.TestMethodStandard}' under the selected Organization already exists!");
            }

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

            if (model.StandardOrganizationID <= 0)
                throw new ArgumentException("Standard Organization is required!");

            if (string.IsNullOrWhiteSpace(model.TestMethodStandard))
                throw new ArgumentException("Test Method Standard is required!");

            bool exists = await _TestMethodSpecificationRepository.ExistsByOrgAndStandardAndNotId(model.StandardOrganizationID, model.TestMethodStandard, model.ID);
            if (exists)
            {
                throw new InvalidOperationException($"Test Method Specification for Standard '{model.TestMethodStandard}' under the selected Organization already exists!");
            }

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

        public async Task<List<DropdwonSelector>> GetTestMethodSpecificationVersionDropdown(string? searchTerm, int pageNo, int pageSize, long metalId = 0)
        {
            return await _TestMethodSpecificationRepository.GetTestMethodSpecificationVersionDropdown(searchTerm, pageNo, pageSize, metalId);
        }

        // ── PDF Matching Helpers (optional folder scan) ─────────────────────
        private static readonly Dictionary<string, string> OrgFolderMap = new(StringComparer.OrdinalIgnoreCase)
        {
            { "ANSI/NACE (AMPP)", "NACE" },
            { "NACE (AMPP)", "NACE" },
            // Default: use org name as folder name
        };

        /// <summary>Map Excel org name to folder name under Updated Std List.</summary>
        private static string MapOrgToFolder(string orgName)
        {
            var trimmed = orgName.Trim();
            return OrgFolderMap.TryGetValue(trimmed, out var mapped) ? mapped : trimmed;
        }

        /// <summary>
        /// Resolve the PDF file for an import row.
        /// Priority: 1) If client sent PdfFileName (Excel column G), look for that exact file.
        ///           2) Fallback: auto-generate candidates from org + standard info.
        /// Returns (foundPdfFileName, fullFilePath) or (null, null).
        /// </summary>
        private (string? fileName, string? filePath) ResolvePdfFile(ImportTestMethodSpecItemDto item)
        {
            if (string.IsNullOrWhiteSpace(_pdfFolderPath) || !Directory.Exists(_pdfFolderPath))
                return (null, null);

            var orgFolder = MapOrgToFolder(item.StandardOrganization);
            var searchDir = Path.Combine(_pdfFolderPath, orgFolder);
            if (!Directory.Exists(searchDir))
                return (null, null);

            // 1) Use the exact filename from Excel column G if provided
            if (!string.IsNullOrWhiteSpace(item.PdfFileName))
            {
                var exactPath = Path.Combine(searchDir, item.PdfFileName);
                if (File.Exists(exactPath))
                    return (item.PdfFileName, exactPath);

                // Not found with exact name — try a prefix-based search anyway
                var prefixNoExt = Path.GetFileNameWithoutExtension(item.PdfFileName);
                var fallbackFiles = Directory.GetFiles(searchDir, "*.pdf");
                var match = fallbackFiles.FirstOrDefault(f =>
                    Path.GetFileNameWithoutExtension(f).Equals(prefixNoExt, StringComparison.OrdinalIgnoreCase));
                if (match != null)
                    return (Path.GetFileName(match), match);

                return (null, null); // file specified but not found
            }

            // 2) Fallback: auto-generate candidates from org + standard + version/year
            var orgAbbr = MapOrgToFolder(item.StandardOrganization);
            var stdPart = item.TestMethodStandard.Replace("/", "-").Replace(" ", "-");
            while (stdPart.Contains("--")) stdPart = stdPart.Replace("--", "-");
            var prefix = $"{orgAbbr}-{stdPart}";

            var candidates = new List<string>();
            if (!string.IsNullOrWhiteSpace(item.Version))
            {
                var verDigits = new string(item.Version.Where(char.IsDigit).ToArray());
                if (verDigits.Length > 0)
                {
                    candidates.Add($"{prefix}-{verDigits}.pdf");
                    if (!string.IsNullOrWhiteSpace(item.Year) && item.Year.Length >= 2)
                        candidates.Add($"{prefix}-{item.Year[^2..]}.pdf");
                }
            }
            if (!string.IsNullOrWhiteSpace(item.Year))
                candidates.Add($"{prefix}-{item.Year}.pdf");
            candidates.Add($"{prefix}.pdf");

            foreach (var candidate in candidates.Distinct())
            {
                var exactPath = Path.Combine(searchDir, candidate);
                if (File.Exists(exactPath))
                    return (candidate, exactPath);
            }

            // Fallback: prefix-based wildcard search
            var allFiles = Directory.GetFiles(searchDir, "*.pdf");
            var wildMatch = allFiles.FirstOrDefault(f =>
                Path.GetFileNameWithoutExtension(f).StartsWith(prefix, StringComparison.OrdinalIgnoreCase));
            if (wildMatch != null)
                return (Path.GetFileName(wildMatch), wildMatch);

            return (null, null);
        }

        public async Task<List<ImportValidationResultDto>> ValidateImport(List<ImportTestMethodSpecItemDto> items)
        {
            _logger.LogInformation("[ValidateImport] User={EmployeeId} | Rows={RowCount}", loggedInUser.EmployeeID, items.Count);

            var orgs = await _TestMethodSpecificationRepository.GetAllStandardOrganizations();
            var orgMap = orgs.ToDictionary(x => x.Name!.Trim().ToLower(), x => x.Id);

            var results = new List<ImportValidationResultDto>();

            foreach (var item in items)
            {
                var r = new ImportValidationResultDto
                {
                    RowNumber = item.RowNumber,
                    StandardOrganization = item.StandardOrganization,
                    TestMethodStandard = item.TestMethodStandard,
                    Part = item.Part,
                    OfficialTitle = item.OfficialTitle,
                    Version = item.Version,
                    Year = item.Year,
                };

                var orgKey = item.StandardOrganization.Trim().ToLower();
                if (orgMap.TryGetValue(orgKey, out var orgId))
                {
                    r.StandardOrganizationID = orgId;
                }
                else
                {
                    r.Status = "error";
                    r.Messages.Add($"Standard Organization '{item.StandardOrganization}' not found in the system.");
                }

                if (string.IsNullOrWhiteSpace(item.TestMethodStandard))
                {
                    r.Status = "error";
                    r.Messages.Add("Test Method Standard is required.");
                }

                if (r.StandardOrganizationID > 0 && !string.IsNullOrWhiteSpace(item.TestMethodStandard))
                {
                    var exists = await _TestMethodSpecificationRepository.ExistsByOrgAndStandard(
                        r.StandardOrganizationID.Value, item.TestMethodStandard.Trim());
                    r.Exists = exists;
                    if (exists)
                    {
                        r.Status = "error";
                        r.Messages.Add($"Already exists: '{item.StandardOrganization} {item.TestMethodStandard}'.");
                    }
                }

                if (string.IsNullOrWhiteSpace(item.OfficialTitle) && string.IsNullOrWhiteSpace(item.TestMethodStandard))
                {
                    r.Status = "error";
                    r.Messages.Add("Both Official Title and Test Method Standard are empty.");
                }

                // PDF match check (optional — info only if folder is configured)
                if (!string.IsNullOrWhiteSpace(_pdfFolderPath) && Directory.Exists(_pdfFolderPath))
                {
                    var (pdfName, pdfPath) = ResolvePdfFile(item);
                    r.PdfFileName = pdfName;
                    r.PdfFound = pdfPath != null;
                    if (pdfPath != null)
                        r.Messages.Add($"PDF found: {pdfName}");
                }

                results.Add(r);
            }

            return results;
        }

        public async Task<BulkImportResultDto> BulkImport(List<ImportTestMethodSpecItemDto> items)
        {
            _logger.LogInformation(
                "[BulkImport-Start] User={EmployeeId} Company={CompanyCode} | Rows={RowCount} PdfFolder={PdfFolder}",
                loggedInUser.EmployeeID, loggedInUser.CompanyCode, items.Count, _pdfFolderPath);

            var orgs = await _TestMethodSpecificationRepository.GetAllStandardOrganizations();
            var orgMap = orgs.ToDictionary(x => x.Name!.Trim().ToLower(), x => x.Id);

            // Track (item, pdfFileName, pdfFilePath) for PDF upload after save
            var pendingPdfUploads = new List<(ImportTestMethodSpecItemDto item, string fileName, string filePath)>();

            var specs = new List<TestMethodSpecification>();
            var errors = new List<string>();
            int skipped = 0;
            int pdfMatched = 0;

            foreach (var item in items)
            {
                var orgKey = item.StandardOrganization.Trim().ToLower();
                if (!orgMap.TryGetValue(orgKey, out var orgId))
                {
                    skipped++;
                    errors.Add($"Row {item.RowNumber}: Organization '{item.StandardOrganization}' not found.");
                    continue;
                }

                if (string.IsNullOrWhiteSpace(item.TestMethodStandard))
                {
                    skipped++;
                    errors.Add($"Row {item.RowNumber}: Test Method Standard is empty.");
                    continue;
                }

                var exists = await _TestMethodSpecificationRepository.ExistsByOrgAndStandard(
                    orgId, item.TestMethodStandard.Trim());
                if (exists)
                {
                    skipped++;
                    errors.Add($"Row {item.RowNumber}: '{item.StandardOrganization} {item.TestMethodStandard}' already exists.");
                    continue;
                }

                var name = !string.IsNullOrWhiteSpace(item.OfficialTitle)
                    ? item.OfficialTitle.Trim()
                    : $"{item.StandardOrganization} {item.TestMethodStandard}{(string.IsNullOrEmpty(item.Part) ? "" : " " + item.Part)}";

                var displayTitle = $"{item.StandardOrganization} {item.TestMethodStandard}"
                    + (string.IsNullOrEmpty(item.Part) ? "" : " " + item.Part)
                    + (string.IsNullOrEmpty(item.Version) ? "" : " " + item.Version)
                    + (string.IsNullOrEmpty(item.Year) ? "" : " (" + item.Year + ")");

                var versionYear = !string.IsNullOrWhiteSpace(item.Year) ? item.Year : null;
                var versionLabel = !string.IsNullOrWhiteSpace(item.Version) ? item.Version : (versionYear ?? "1.0");

                var spec = new TestMethodSpecification
                {
                    StandardOrganizationID = orgId,
                    TestMethodStandard = item.TestMethodStandard.Trim(),
                    Part = item.Part?.Trim(),
                    Name = name,
                    DisplayTitle = displayTitle,
                    IsActive = true,
                    CompanyCode = loggedInUser.CompanyCode,
                    CreatedBy = loggedInUser.EmployeeID,
                    CreatedOn = DateTime.UtcNow,
                    Versions = new List<TestMethodSpecificationVersion>
                    {
                        new TestMethodSpecificationVersion
                        {
                            Version = versionLabel,
                            Year = versionYear,
                            Status = VersionStatus.Active,
                            IsDefault = true,
                            EffectiveDate = DateTime.UtcNow,
                            CreatedBy = loggedInUser.EmployeeID,
                            CreatedOn = DateTime.UtcNow,
                        }
                    }
                };

                // Store PDF info in pending list (not on spec entity)
                var (pdfFileName, pdfFilePath) = ResolvePdfFile(item);
                if (pdfFilePath != null)
                {
                    pdfMatched++;
                    pendingPdfUploads.Add((item, pdfFileName!, pdfFilePath));
                }

                specs.Add(spec);
            }

            int importedCount = 0;
            int pdfUploaded = 0;

            if (specs.Any())
            {
                await _TestMethodSpecificationRepository.AddRangeAsync(specs);
                importedCount = specs.Count;
                _logger.LogInformation(
                    "[BulkImport] User={EmployeeId} Company={CompanyCode} | TotalRows={Total} Imported={Imported} Skipped={Skipped} PdfMatched={PdfMatched} PdfUploaded={PdfUploaded} Errors={ErrorCount}",
                    loggedInUser.EmployeeID, loggedInUser.CompanyCode,
                    items.Count, importedCount, skipped, pdfMatched, 0, errors.Count);

                // Second pass: match saved specs to pending PDF uploads by org+standard
                foreach (var spec in specs)
                {
                    if (spec.ID == 0) continue;
                    var version = spec.Versions.FirstOrDefault();
                    if (version == null) continue;

                    // Find matching pending PDF by org (StandardOrganizationID) + standard
                    var pending = pendingPdfUploads.FirstOrDefault(p =>
                    {
                        var orgKey = p.item.StandardOrganization.Trim().ToLower();
                        return orgMap.TryGetValue(orgKey, out var oid) && oid == spec.StandardOrganizationID
                            && p.item.TestMethodStandard.Trim() == spec.TestMethodStandard;
                    });
                    if (pending == default) continue;

                    var (_, pdfFileName, pdfFilePath) = pending;

                    try
                    {
                        var fileInfo = new FileInfo(pdfFilePath);
                        // Skip files > 10 MB silently (log only)
                        if (fileInfo.Length > FileUploadValidator.MaxFileSizeBytes)
                        {
                            _logger.LogInformation("Skipping PDF '{FileName}' ({Size:F1} MB) — exceeds 10 MB limit.", pdfFileName, fileInfo.Length / (1024.0 * 1024));
                            continue;
                        }

                        var fileBytes = await File.ReadAllBytesAsync(pdfFilePath);
                        using var stream = new MemoryStream(fileBytes);
                        var formFile = new FormFile(stream, 0, fileBytes.Length, "file", pdfFileName)
                        {
                            Headers = new HeaderDictionary(),
                            ContentType = "application/octet-stream"
                        };

                        var uploaded = await _uploadService.UploadFileAsync(formFile, FileType.Other, null, $"import-{spec.ID}");

                        if (uploaded != null)
                        {
                            await _TestMethodSpecificationRepository.UpdateVersionFileRef(
                                version.ID, uploaded.FilePath, uploaded.OriginalFileName, uploaded.ID);
                            pdfUploaded++;
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to upload PDF '{FileName}' for spec {SpecId}.", pdfFileName, spec.ID);
                        errors.Add($"Row '{spec.DisplayTitle}': PDF upload failed - {ex.Message}");
                    }
                }
            }

            var result = new BulkImportResultDto
            {
                TotalRows = items.Count,
                Imported = importedCount,
                Skipped = skipped,
                Errors = errors,
                PdfMatched = pdfMatched,
                PdfUploaded = pdfUploaded,
            };

            _logger.LogInformation(
                "[BulkImport-Done] User={EmployeeId} | Total={Total} Imported={Imported} Skipped={Skipped} PdfMatched={PdfMatched} PdfUploaded={PdfUploaded}",
                loggedInUser.EmployeeID, result.TotalRows, result.Imported, result.Skipped, result.PdfMatched, result.PdfUploaded);

            return result;
        }

        public async Task<List<DropdwonSelector>> GetAllStandardOrganizations()
        {
            return await _TestMethodSpecificationRepository.GetAllStandardOrganizations();
        }
    }
}
