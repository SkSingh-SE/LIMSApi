using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Helpers.Enums;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LIMSApi.Services
{
    public class SpecificationVersionService : ISpecificationVersionService
    {
        private readonly ISpecificationVersionRepository _versionRepo;
        private readonly IFileUploadService _uploadService;
        private readonly LIMSContext _context;
        private readonly ILogger<SpecificationVersionService> _logger;

        public SpecificationVersionService(
            ISpecificationVersionRepository versionRepo,
            IFileUploadService uploadService,
            LIMSContext context,
            ILogger<SpecificationVersionService> logger)
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

        public async Task<PagedResponse<SpecificationVersionListItemDto>> GetPagedVersionsAsync(SpecificationVersionFilterDto filter)
        {
            var companyCode = GetCurrentCompanyCode();
            return await _versionRepo.GetPagedVersionsAsync(filter, companyCode);
        }

        public async Task<SpecificationVersionDetailDto> GetVersionDetailsAsync(long id)
        {
            var companyCode = GetCurrentCompanyCode();
            var version = await _versionRepo.GetVersionWithParametersByIdAsync(id, companyCode);
            if (version == null)
            {
                throw new KeyNotFoundException($"Specification Version with ID {id} was not found.");
            }

            var detail = new SpecificationVersionDetailDto
            {
                ID = version.ID,
                SpecificationHeaderID = version.SpecificationHeaderID,
                SpecificationCode = version.SpecificationHeader?.Code ?? string.Empty,
                SpecificationName = version.SpecificationHeader?.AliasName ?? string.Empty,
                StandardReference = version.SpecificationHeader?.StandardReference,
                StandardOrganizationName = version.SpecificationHeader?.StandardOrganization?.Name,
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
                IsParentActive = version.SpecificationHeader?.IsActive ?? false,
                CreatedOn = version.CreatedOn,
                CreatedBy = version.CreatedBy,
                ModifiedOn = version.ModifiedOn,
                ModifiedBy = version.ModifiedBy,
                CompanyCode = version.CompanyCode,
                Parameters = version.Parameters
                    .OrderBy(p => p.SortOrder)
                    .ThenBy(p => p.ID)
                    .Select(p => new SpecificationVersionParameterDto
                    {
                        ID = p.ID,
                        ParameterID = p.ParameterID,
                        ParameterCode = p.Parameter?.Code,
                        ParameterName = p.Parameter?.Name,
                        UnitID = p.Parameter?.ParameterUnitID,
                        UnitName = p.Parameter?.ParameterUnit?.Name,
                        UnitSymbol = p.Parameter?.ParameterUnit?.Symbol,
                        SortOrder = p.SortOrder,
                        Comment = p.Comment
                    })
                    .ToList()
            };

            return detail;
        }

        public async Task<SpecificationVersionListItemDto> CreateVersionAsync(SpecificationVersionCreateDto dto)
        {
            var companyCode = GetCurrentCompanyCode();

            // 1. Parent Specification validation
            var parent = await _versionRepo.GetParentSpecificationAsync(dto.SpecificationHeaderID, companyCode);
            if (parent == null)
            {
                throw new KeyNotFoundException("Parent Specification Master was not found or does not belong to your organization.");
            }

            if (!parent.IsActive)
            {
                throw new InvalidOperationException("Cannot create a version for an inactive Specification Master.");
            }

            // 2. Version validation
            dto.Version = dto.Version?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(dto.Version))
            {
                throw new ArgumentException("Version / Edition is required.");
            }

            if (await _versionRepo.ExistsVersionAsync(dto.SpecificationHeaderID, dto.Version))
            {
                throw new InvalidOperationException($"Version '{dto.Version}' already exists for specification '{parent.Code ?? parent.AliasName}'.");
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
                var uploadResult = await _uploadService.UploadFileAsync(dto.File, FileType.Test, yearInt, "SpecificationVersion");
                standardFile = uploadResult.OriginalFileName;
                standardFilePath = uploadResult.FilePath;
                uploadRefId = uploadResult.ID;
            }

            // 6. Transactional default handling
            if (dto.IsDefault)
            {
                await _versionRepo.UnsetDefaultVersionsAsync(dto.SpecificationHeaderID);
            }

            var version = new SpecificationVersion
            {
                SpecificationHeaderID = dto.SpecificationHeaderID,
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
                CreatedOn = DateTime.UtcNow,
                CompanyCode = companyCode
            };

            await _versionRepo.CreateVersionAsync(version);

            return new SpecificationVersionListItemDto
            {
                ID = version.ID,
                SpecificationHeaderID = parent.ID,
                SpecificationCode = parent.Code ?? string.Empty,
                SpecificationName = parent.AliasName,
                StandardReference = parent.StandardReference,
                StandardOrganizationName = parent.StandardOrganization?.Name,
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

        public async Task<SpecificationVersionListItemDto> UpdateVersionAsync(long id, SpecificationVersionUpdateDto dto)
        {
            var companyCode = GetCurrentCompanyCode();
            var existing = await _versionRepo.GetVersionByIdAsync(id, companyCode);
            if (existing == null)
            {
                throw new KeyNotFoundException($"Specification Version with ID {id} was not found.");
            }

            var parent = existing.SpecificationHeader;
            if (parent == null)
            {
                throw new KeyNotFoundException("Parent Specification was not found.");
            }

            // 1. Parent active check on activation
            if (!parent.IsActive && dto.Status == VersionStatus.Active && existing.Status != VersionStatus.Active)
            {
                throw new InvalidOperationException("Cannot activate a version when its parent Specification is inactive.");
            }

            // 2. Version validation
            dto.Version = dto.Version?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(dto.Version))
            {
                throw new ArgumentException("Version / Edition is required.");
            }

            if (await _versionRepo.ExistsVersionAsync(existing.SpecificationHeaderID, dto.Version, id))
            {
                throw new InvalidOperationException($"Version '{dto.Version}' already exists for this specification.");
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

            // Prevent casual reactivation from Superseded or Withdrawn without controlled action
            if ((existing.Status == VersionStatus.Superseded || existing.Status == VersionStatus.Withdrawn) && dto.Status == VersionStatus.Active)
            {
                if (!parent.IsActive)
                {
                    throw new InvalidOperationException("Cannot reactivate version because the parent specification is inactive.");
                }
            }

            // 6. Transactional default handling
            if (dto.IsDefault && !existing.IsDefault)
            {
                await _versionRepo.UnsetDefaultVersionsAsync(existing.SpecificationHeaderID, id);
            }

            // 7. File upload handling
            if (dto.File != null && dto.File.Length > 0)
            {
                int? yearInt = int.TryParse(dto.Year, out var y) ? y : DateTime.UtcNow.Year;
                var uploadResult = await _uploadService.UploadFileAsync(dto.File, FileType.Test, yearInt, "SpecificationVersion");
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
            existing.ModifiedBy = GetCurrentUserId();
            existing.ModifiedOn = DateTime.UtcNow;

            await _versionRepo.UpdateVersionAsync(existing);

            return new SpecificationVersionListItemDto
            {
                ID = existing.ID,
                SpecificationHeaderID = parent.ID,
                SpecificationCode = parent.Code ?? string.Empty,
                SpecificationName = parent.AliasName,
                StandardReference = parent.StandardReference,
                StandardOrganizationName = parent.StandardOrganization?.Name,
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

        public async Task<bool> ActivateVersionAsync(long id)
        {
            var companyCode = GetCurrentCompanyCode();
            var version = await _versionRepo.GetVersionByIdAsync(id, companyCode);
            if (version == null)
            {
                throw new KeyNotFoundException($"Specification Version with ID {id} was not found.");
            }

            var parent = version.SpecificationHeader;
            if (parent == null || !parent.IsActive)
            {
                throw new InvalidOperationException("Cannot activate version because the parent Specification Master is inactive.");
            }

            if (version.Status == VersionStatus.Active)
            {
                return true; // Already active
            }

            version.Status = VersionStatus.Active;
            if (!version.EffectiveDate.HasValue)
            {
                version.EffectiveDate = DateTime.UtcNow.Date;
            }
            version.ModifiedBy = GetCurrentUserId();
            version.ModifiedOn = DateTime.UtcNow;

            await _versionRepo.UpdateVersionAsync(version);
            return true;
        }

        public async Task<bool> SupersedeVersionAsync(long id, DateTime? supersededDate = null)
        {
            var companyCode = GetCurrentCompanyCode();
            var version = await _versionRepo.GetVersionByIdAsync(id, companyCode);
            if (version == null)
            {
                throw new KeyNotFoundException($"Specification Version with ID {id} was not found.");
            }

            if (version.Status != VersionStatus.Active)
            {
                throw new InvalidOperationException("Only an Active version can be superseded.");
            }

            version.Status = VersionStatus.Superseded;
            version.SupersededDate = supersededDate ?? DateTime.UtcNow.Date;
            version.IsDefault = false; // Superseded version cannot remain default
            version.ModifiedBy = GetCurrentUserId();
            version.ModifiedOn = DateTime.UtcNow;

            await _versionRepo.UpdateVersionAsync(version);
            return true;
        }

        public async Task<bool> WithdrawVersionAsync(long id)
        {
            var companyCode = GetCurrentCompanyCode();
            var version = await _versionRepo.GetVersionByIdAsync(id, companyCode);
            if (version == null)
            {
                throw new KeyNotFoundException($"Specification Version with ID {id} was not found.");
            }

            version.Status = VersionStatus.Withdrawn;
            version.IsDefault = false; // Withdrawn version cannot remain default
            version.ModifiedBy = GetCurrentUserId();
            version.ModifiedOn = DateTime.UtcNow;

            await _versionRepo.UpdateVersionAsync(version);
            return true;
        }

        public async Task<bool> SetDefaultVersionAsync(long id)
        {
            var companyCode = GetCurrentCompanyCode();
            var version = await _versionRepo.GetVersionByIdAsync(id, companyCode);
            if (version == null)
            {
                throw new KeyNotFoundException($"Specification Version with ID {id} was not found.");
            }

            if (version.Status != VersionStatus.Active)
            {
                throw new InvalidOperationException("Only an Active version can be set as default.");
            }

            var parent = version.SpecificationHeader;
            if (parent == null || !parent.IsActive)
            {
                throw new InvalidOperationException("Cannot set as default because the parent Specification is inactive.");
            }

            await _versionRepo.SetDefaultVersionAsync(version.SpecificationHeaderID, id);
            return true;
        }

        public async Task<List<SpecificationVersionParameterDto>> GetVersionParametersAsync(long id)
        {
            var companyCode = GetCurrentCompanyCode();
            return await _versionRepo.GetVersionParametersAsync(id, companyCode);
        }

        public async Task<bool> SaveVersionParametersAsync(long id, SaveSpecificationVersionParametersDto dto)
        {
            var companyCode = GetCurrentCompanyCode();
            var version = await _versionRepo.GetVersionByIdAsync(id, companyCode);
            if (version == null)
            {
                throw new KeyNotFoundException($"Specification Version with ID {id} was not found.");
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

                    if (p.SortOrder < 1)
                    {
                        throw new ArgumentException("Sort Order must be at least 1.");
                    }

                    if (!string.IsNullOrWhiteSpace(p.Comment) && p.Comment.Length > 1000)
                    {
                        throw new ArgumentException("Comment cannot exceed 1000 characters.");
                    }
                }

                var entities = dto.Parameters.Select(p => new SpecificationVersionParameter
                {
                    SpecificationVersionID = id,
                    ParameterID = p.ParameterID,
                    SortOrder = p.SortOrder,
                    Comment = string.IsNullOrWhiteSpace(p.Comment) ? null : p.Comment.Trim()
                }).ToList();

                await _versionRepo.SaveVersionParametersAsync(id, entities);
            }
            else
            {
                await _versionRepo.SaveVersionParametersAsync(id, new List<SpecificationVersionParameter>());
            }

            return true;
        }

        public async Task<List<SpecificationVersionDropdownDto>> GetDropdownBySpecificationIdAsync(long specId, bool includeAll = false)
        {
            var companyCode = GetCurrentCompanyCode();
            return await _versionRepo.GetDropdownBySpecificationIdAsync(specId, companyCode, includeAll);
        }

        public async Task<bool> DeleteVersionAsync(long id)
        {
            var companyCode = GetCurrentCompanyCode();
            var version = await _versionRepo.GetVersionByIdAsync(id, companyCode);
            if (version == null)
            {
                throw new KeyNotFoundException($"Specification Version with ID {id} was not found.");
            }

            if (version.Status == VersionStatus.Active)
            {
                throw new InvalidOperationException("Active Specification Versions cannot be deleted.");
            }

            if (version.Status == VersionStatus.Superseded)
            {
                throw new InvalidOperationException("Superseded Specification Versions cannot be deleted.");
            }

            // Check if referenced by UniversalTestGroups (planned or executed test)
            var isReferencedByUtg = await _context.UniversalTestGroups.AnyAsync(utg => utg.SpecificationVersionID == id);
            if (isReferencedByUtg)
            {
                throw new InvalidOperationException("Cannot delete this Specification Version because it is referenced by a planned or executed test.");
            }

            // Check if referenced by configured specification requirements
            var hasConfiguredLines = await _context.SpecificationLines.AnyAsync(sl => sl.SpecificationVersionID == id);
            if (hasConfiguredLines)
            {
                throw new InvalidOperationException("Cannot delete this Specification Version because it has configured specification requirements.");
            }

            // Check if referenced by test method mappings
            var hasMethodRef = await _context.LaboratoryTestAnalysisTypeMethods.AnyAsync(m => m.TestMethodSpecificationVersionID == id)
                || await _context.LaboratoryTestSubGroupMethods.AnyAsync(m => m.TestMethodSpecificationVersionID == id)
                || await _context.LabScopeSpecifications.AnyAsync(m => m.TestMethodSpecificationVersionID == id)
                || await _context.TestMethodSpecificationParameters.AnyAsync(m => m.TestMethodSpecificationVersionID == id);
            if (hasMethodRef)
            {
                throw new InvalidOperationException("Cannot delete this Specification Version because it is referenced by test method mappings.");
            }

            // Remove associated mapped parameters if any
            var parameters = await _context.SpecificationVersionParameters
                .Where(p => p.SpecificationVersionID == id)
                .ToListAsync();
            if (parameters.Any())
            {
                _context.SpecificationVersionParameters.RemoveRange(parameters);
            }

            // Delete standard file if uploaded
            if (version.UploadReferenceID.HasValue)
            {
                try
                {
                    await _uploadService.RemoveFileAsync(version.UploadReferenceID.Value);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to delete physical file {FilePath} for version {VersionId}", version.StandardFilePath, id);
                }
            }

            _context.SpecificationVersions.Remove(version);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
