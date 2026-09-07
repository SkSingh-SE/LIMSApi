using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Helpers.Enums;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Services
{
    public class TestMethodVersionService : ITestMethodVersionService
    {
        private readonly ITestMethodVersionRepository _versionRepo;
        private readonly IFileUploadService _uploadService;
        private readonly LIMSContext _context;
        private readonly ILogger<TestMethodVersionService> _logger;

        public TestMethodVersionService(
            ITestMethodVersionRepository versionRepo,
            IFileUploadService uploadService,
            LIMSContext context,
            ILogger<TestMethodVersionService> logger)
        {
            _versionRepo = versionRepo;
            _uploadService = uploadService;
            _context = context;
            _logger = logger;
        }

        private static string GetCurrentCompanyCode()
        {
            var user = LoggedInUserProvider.CurrentUser;
            return string.IsNullOrWhiteSpace(user?.CompanyCode) ? "LIMS" : user.CompanyCode;
        }

        private static long GetCurrentUserId()
        {
            return LoggedInUserProvider.CurrentUser?.UserId ?? 1;
        }

        public async Task<PagedResponse<TestMethodVersionListItemDto>> GetPagedVersionsAsync(TestMethodVersionFilterDto filter)
        {
            var companyCode = GetCurrentCompanyCode();
            return await _versionRepo.GetPagedVersionsAsync(filter, companyCode);
        }

        public async Task<TestMethodVersionDetailDto> GetVersionDetailsAsync(long id)
        {
            var companyCode = GetCurrentCompanyCode();
            var version = await _versionRepo.GetVersionWithParametersByIdAsync(id, companyCode);
            if (version == null)
            {
                throw new KeyNotFoundException($"Test Method Version with ID {id} was not found.");
            }

            var detail = new TestMethodVersionDetailDto
            {
                ID = version.ID,
                TestMethodSpecificationID = version.TestMethodSpecificationID,
                TestMethodCode = version.TestMethodSpecification?.Code ?? version.TestMethodSpecification?.TestMethodStandard ?? string.Empty,
                TestMethodName = version.TestMethodSpecification?.Name ?? version.TestMethodSpecification?.DisplayTitle ?? string.Empty,
                Version = version.Version,
                Year = version.Year,
                Status = version.Status,
                EffectiveDate = version.EffectiveDate,
                SupersededDate = version.SupersededDate,
                ReviewDate = version.ReviewDate,
                ChangeReason = version.ChangeReason,
                IsDefault = version.IsDefault,
                StandardFile = version.StandardFile,
                StandardFilePath = version.StandardFilePath,
                UploadReferenceID = version.UploadReferenceID,
                ParametersCount = version.Parameters.Count,
                IsParentActive = version.TestMethodSpecification?.IsActive ?? false,
                CreatedOn = version.CreatedOn,
                CreatedBy = version.CreatedBy,
                Parameters = version.Parameters
                    .OrderBy(p => p.SortOrder)
                    .ThenBy(p => p.ID)
                    .Select(p => new TestMethodVersionParameterDto
                    {
                        ID = p.ID,
                        ParameterID = p.ParameterID,
                        ParameterCode = p.Parameter?.Code ?? string.Empty,
                        ParameterName = p.Parameter?.Name ?? string.Empty,
                        UnitID = p.Parameter?.ParameterUnit?.ID ?? p.ParameterUnitID,
                        UnitName = p.Parameter?.ParameterUnit?.Name,
                        UnitSymbol = p.Parameter?.ParameterUnit?.Symbol,
                        ParameterUnitEquivalentID = p.ParameterUnitEquivalentID,
                        SortOrder = p.SortOrder,
                        Comment = p.Comment
                    })
                    .ToList()
            };

            return detail;
        }

        public async Task<TestMethodVersionListItemDto> CreateVersionAsync(TestMethodVersionCreateDto dto)
        {
            var companyCode = GetCurrentCompanyCode();

            // 1. Parent Test Method validation
            var parent = await _versionRepo.GetParentMethodAsync(dto.TestMethodSpecificationID, companyCode);
            if (parent == null)
            {
                throw new KeyNotFoundException("Parent Test Method was not found or does not belong to your organization.");
            }

            if (!parent.IsActive)
            {
                throw new InvalidOperationException("Cannot create a version for an inactive Test Method.");
            }

            // 2. Version validation
            dto.Version = dto.Version?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(dto.Version))
            {
                throw new ArgumentException("Version / Edition is required.");
            }

            if (await _versionRepo.ExistsVersionAsync(dto.TestMethodSpecificationID, dto.Version))
            {
                throw new InvalidOperationException("Version already exists for this test method.");
            }

            // 3. Date validations
            if (dto.EffectiveDate.HasValue && dto.SupersededDate.HasValue && dto.SupersededDate < dto.EffectiveDate)
            {
                throw new ArgumentException("Superseded Date cannot be earlier than Effective Date.");
            }

            if (dto.EffectiveDate.HasValue && dto.ReviewDate.HasValue && dto.ReviewDate < dto.EffectiveDate)
            {
                throw new ArgumentException("Periodic Review Date cannot be earlier than Effective Date.");
            }

            // 4. Default rule validation
            if (dto.IsDefault && dto.Status != VersionStatus.Active)
            {
                throw new InvalidOperationException("Only an Active version can be set as default.");
            }

            // 5. File upload handling
            string? standardFile = null;
            string? standardFilePath = null;
            long? uploadRefId = null;

            if (dto.File != null && dto.File.Length > 0)
            {
                int? yearInt = int.TryParse(dto.Year, out var y) ? y : DateTime.UtcNow.Year;
                var uploadResult = await _uploadService.UploadFileAsync(dto.File, FileType.Test, yearInt, "TestMethodVersion");
                standardFile = uploadResult.OriginalFileName;
                standardFilePath = uploadResult.FilePath;
                uploadRefId = uploadResult.ID;
            }

            // 6. Transactional default handling
            if (dto.IsDefault)
            {
                await _versionRepo.UnsetDefaultVersionsAsync(dto.TestMethodSpecificationID);
            }

            var version = new TestMethodSpecificationVersion
            {
                TestMethodSpecificationID = dto.TestMethodSpecificationID,
                Version = dto.Version,
                Year = string.IsNullOrWhiteSpace(dto.Year) ? null : dto.Year.Trim(),
                Status = dto.Status,
                EffectiveDate = dto.EffectiveDate,
                SupersededDate = dto.SupersededDate,
                ReviewDate = dto.ReviewDate,
                ChangeReason = string.IsNullOrWhiteSpace(dto.ChangeReason) ? null : dto.ChangeReason.Trim(),
                IsDefault = dto.IsDefault,
                StandardFile = standardFile,
                StandardFilePath = standardFilePath,
                UploadReferenceID = uploadRefId,
                CreatedBy = GetCurrentUserId(),
                CreatedOn = DateTime.UtcNow
            };

            await _versionRepo.CreateVersionAsync(version);

            return new TestMethodVersionListItemDto
            {
                ID = version.ID,
                TestMethodSpecificationID = parent.ID,
                TestMethodCode = parent.Code ?? parent.TestMethodStandard ?? string.Empty,
                TestMethodName = parent.Name ?? parent.DisplayTitle ?? string.Empty,
                Version = version.Version,
                Year = version.Year,
                Status = version.Status,
                EffectiveDate = version.EffectiveDate,
                SupersededDate = version.SupersededDate,
                ReviewDate = version.ReviewDate,
                ChangeReason = version.ChangeReason,
                IsDefault = version.IsDefault,
                StandardFile = version.StandardFile,
                StandardFilePath = version.StandardFilePath,
                UploadReferenceID = version.UploadReferenceID,
                ParametersCount = 0,
                IsParentActive = parent.IsActive
            };
        }

        public async Task<TestMethodVersionListItemDto> UpdateVersionAsync(long id, TestMethodVersionUpdateDto dto)
        {
            var companyCode = GetCurrentCompanyCode();
            var existing = await _versionRepo.GetVersionByIdAsync(id, companyCode);
            if (existing == null)
            {
                throw new KeyNotFoundException($"Test Method Version with ID {id} was not found.");
            }

            var parent = existing.TestMethodSpecification;
            if (parent == null)
            {
                throw new KeyNotFoundException("Parent Test Method was not found.");
            }

            // 1. Parent active check on activation
            if (!parent.IsActive && dto.Status == VersionStatus.Active && existing.Status != VersionStatus.Active)
            {
                throw new InvalidOperationException("Cannot activate a version when its parent Test Method is inactive.");
            }

            // 2. Version validation
            dto.Version = dto.Version?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(dto.Version))
            {
                throw new ArgumentException("Version / Edition is required.");
            }

            if (await _versionRepo.ExistsVersionAsync(existing.TestMethodSpecificationID, dto.Version, id))
            {
                throw new InvalidOperationException("Version already exists for this test method.");
            }

            // 3. Date validations
            if (dto.EffectiveDate.HasValue && dto.SupersededDate.HasValue && dto.SupersededDate < dto.EffectiveDate)
            {
                throw new ArgumentException("Superseded Date cannot be earlier than Effective Date.");
            }

            if (dto.EffectiveDate.HasValue && dto.ReviewDate.HasValue && dto.ReviewDate < dto.EffectiveDate)
            {
                throw new ArgumentException("Periodic Review Date cannot be earlier than Effective Date.");
            }

            // 4. Default rule validation
            if (dto.IsDefault && dto.Status != VersionStatus.Active)
            {
                throw new InvalidOperationException("Only an Active version can be set as default.");
            }

            // If status moves away from Active, it can no longer be default
            if (dto.Status != VersionStatus.Active && dto.IsDefault)
            {
                dto.IsDefault = false;
            }

            // 5. Lifecycle transitions check
            if (existing.Status == VersionStatus.Active && dto.Status == VersionStatus.Superseded)
            {
                if (!dto.SupersededDate.HasValue)
                {
                    dto.SupersededDate = DateTime.UtcNow.Date;
                }
            }

            // 6. Transactional default handling
            if (dto.IsDefault && !existing.IsDefault)
            {
                await _versionRepo.UnsetDefaultVersionsAsync(existing.TestMethodSpecificationID, id);
            }

            // 7. File upload handling
            if (dto.File != null && dto.File.Length > 0)
            {
                int? yearInt = int.TryParse(dto.Year, out var y) ? y : DateTime.UtcNow.Year;
                var uploadResult = await _uploadService.UploadFileAsync(dto.File, FileType.Test, yearInt, "TestMethodVersion");
                existing.StandardFile = uploadResult.OriginalFileName;
                existing.StandardFilePath = uploadResult.FilePath;
                existing.UploadReferenceID = uploadResult.ID;
            }

            existing.Version = dto.Version;
            existing.Year = string.IsNullOrWhiteSpace(dto.Year) ? null : dto.Year.Trim();
            existing.Status = dto.Status;
            existing.EffectiveDate = dto.EffectiveDate;
            existing.SupersededDate = dto.SupersededDate;
            existing.ReviewDate = dto.ReviewDate;
            existing.ChangeReason = string.IsNullOrWhiteSpace(dto.ChangeReason) ? null : dto.ChangeReason.Trim();
            existing.IsDefault = dto.IsDefault;

            await _versionRepo.UpdateVersionAsync(existing);

            return new TestMethodVersionListItemDto
            {
                ID = existing.ID,
                TestMethodSpecificationID = parent.ID,
                TestMethodCode = parent.Code ?? parent.TestMethodStandard ?? string.Empty,
                TestMethodName = parent.Name ?? parent.DisplayTitle ?? string.Empty,
                Version = existing.Version,
                Year = existing.Year,
                Status = existing.Status,
                EffectiveDate = existing.EffectiveDate,
                SupersededDate = existing.SupersededDate,
                ReviewDate = existing.ReviewDate,
                ChangeReason = existing.ChangeReason,
                IsDefault = existing.IsDefault,
                StandardFile = existing.StandardFile,
                StandardFilePath = existing.StandardFilePath,
                UploadReferenceID = existing.UploadReferenceID,
                ParametersCount = existing.Parameters.Count,
                IsParentActive = parent.IsActive
            };
        }

        public async Task<bool> SetDefaultVersionAsync(long id)
        {
            var companyCode = GetCurrentCompanyCode();
            var version = await _versionRepo.GetVersionByIdAsync(id, companyCode);
            if (version == null)
            {
                throw new KeyNotFoundException($"Test Method Version with ID {id} was not found.");
            }

            if (version.Status != VersionStatus.Active)
            {
                throw new InvalidOperationException("Only an Active version can be set as default.");
            }

            var parent = version.TestMethodSpecification;
            if (parent == null || !parent.IsActive)
            {
                throw new InvalidOperationException("Cannot set as default because the parent Test Method is inactive.");
            }

            await _versionRepo.SetDefaultVersionAsync(version.TestMethodSpecificationID, id);
            return true;
        }

        public async Task<List<TestMethodVersionParameterDto>> GetVersionParametersAsync(long id)
        {
            var companyCode = GetCurrentCompanyCode();
            return await _versionRepo.GetVersionParametersAsync(id, companyCode);
        }

        public async Task<bool> SaveVersionParametersAsync(long id, SaveVersionParametersDto dto)
        {
            var companyCode = GetCurrentCompanyCode();
            var version = await _versionRepo.GetVersionByIdAsync(id, companyCode);
            if (version == null)
            {
                throw new KeyNotFoundException($"Test Method Version with ID {id} was not found.");
            }

            if (dto.Parameters != null && dto.Parameters.Any())
            {
                // Validate duplicate parameters
                var duplicateParams = dto.Parameters
                    .GroupBy(p => p.ParameterID)
                    .Where(g => g.Count() > 1)
                    .Select(g => g.Key)
                    .ToList();

                if (duplicateParams.Any())
                {
                    throw new ArgumentException("Duplicate parameters cannot be mapped to the same version.");
                }

                // Validate duplicate sort orders
                var duplicateSortOrders = dto.Parameters
                    .GroupBy(p => p.SortOrder)
                    .Where(g => g.Count() > 1)
                    .Select(g => g.Key)
                    .ToList();

                if (duplicateSortOrders.Any())
                {
                    throw new ArgumentException("Each parameter must have a unique Sort Order.");
                }

                // Validate all ParameterIDs exist and are active
                var paramIds = dto.Parameters.Select(p => p.ParameterID).ToList();
                var validParams = await _context.ParameterMasters
                    .Where(pm => paramIds.Contains(pm.ID))
                    .ToDictionaryAsync(pm => pm.ID);

                foreach (var p in dto.Parameters)
                {
                    if (!validParams.ContainsKey(p.ParameterID))
                    {
                        throw new KeyNotFoundException($"Parameter with ID {p.ParameterID} does not exist.");
                    }

                    if (!validParams[p.ParameterID].IsActive)
                    {
                        throw new InvalidOperationException($"Parameter '{validParams[p.ParameterID].Name}' is inactive and cannot be mapped.");
                    }

                    if (p.SortOrder < 0)
                    {
                        throw new ArgumentException("Sort Order cannot be negative.");
                    }

                    if (!string.IsNullOrWhiteSpace(p.Comment) && p.Comment.Length > 1000)
                    {
                        throw new ArgumentException("Comment cannot exceed 1000 characters.");
                    }
                }

                var entities = dto.Parameters.Select(p => new TestMethodSpecificationParameter
                {
                    TestMethodSpecificationVersionID = id,
                    ParameterID = p.ParameterID,
                    ParameterUnitID = validParams[p.ParameterID].ParameterUnitID,
                    ParameterUnitEquivalentID = p.ParameterUnitEquivalentID,
                    SortOrder = p.SortOrder,
                    Comment = string.IsNullOrWhiteSpace(p.Comment) ? null : p.Comment.Trim()
                }).ToList();

                await _versionRepo.SaveVersionParametersAsync(id, entities);
            }
            else
            {
                await _versionRepo.SaveVersionParametersAsync(id, new List<TestMethodSpecificationParameter>());
            }

            return true;
        }

        public async Task<List<TestMethodVersionDropdownDto>> GetDropdownByMethodIdAsync(long methodId)
        {
            var companyCode = GetCurrentCompanyCode();
            return await _versionRepo.GetDropdownByMethodIdAsync(methodId, companyCode);
        }
    }
}
