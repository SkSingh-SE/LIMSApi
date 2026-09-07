using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers.Enums;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Repositories
{
    public class SpecificationVersionRepository : ISpecificationVersionRepository
    {
        private readonly LIMSContext _context;

        public SpecificationVersionRepository(LIMSContext context)
        {
            _context = context;
        }

        public async Task<PagedResponse<SpecificationVersionListItemDto>> GetPagedVersionsAsync(SpecificationVersionFilterDto filter, string companyCode)
        {
            var query = _context.SpecificationVersions
                .Include(v => v.SpecificationHeader)
                    .ThenInclude(h => h!.StandardOrganization)
                .Include(v => v.Parameters)
                .Where(v => v.SpecificationHeader != null && v.SpecificationHeader.CompanyCode == companyCode)
                .AsNoTracking()
                .AsQueryable();

            if (filter.SpecificationHeaderID.HasValue && filter.SpecificationHeaderID.Value > 0)
            {
                query = query.Where(v => v.SpecificationHeaderID == filter.SpecificationHeaderID.Value);
            }

            if (!string.IsNullOrWhiteSpace(filter.Version))
            {
                var vTrim = filter.Version.Trim().ToLower();
                query = query.Where(v => v.Version.ToLower().Contains(vTrim));
            }

            if (!string.IsNullOrWhiteSpace(filter.Year))
            {
                var yTrim = filter.Year.Trim().ToLower();
                query = query.Where(v => v.Year != null && v.Year.ToLower().Contains(yTrim));
            }

            if (filter.Status.HasValue)
            {
                query = query.Where(v => v.Status == filter.Status.Value);
            }

            if (filter.IsDefault.HasValue)
            {
                query = query.Where(v => v.IsDefault == filter.IsDefault.Value);
            }

            var totalCount = await query.CountAsync();

            bool isAsc = string.Equals(filter.SortDirection, "asc", StringComparison.OrdinalIgnoreCase);
            switch (filter.SortColumn?.ToLower())
            {
                case "version":
                    query = isAsc ? query.OrderBy(v => v.Version) : query.OrderByDescending(v => v.Version);
                    break;
                case "year":
                    query = isAsc ? query.OrderBy(v => v.Year) : query.OrderByDescending(v => v.Year);
                    break;
                case "status":
                    query = isAsc ? query.OrderBy(v => v.Status) : query.OrderByDescending(v => v.Status);
                    break;
                case "effectivedate":
                    query = isAsc ? query.OrderBy(v => v.EffectiveDate) : query.OrderByDescending(v => v.EffectiveDate);
                    break;
                case "reviewdate":
                    query = isAsc ? query.OrderBy(v => v.ReviewDate) : query.OrderByDescending(v => v.ReviewDate);
                    break;
                case "speccode":
                case "specificationcode":
                    query = isAsc ? query.OrderBy(v => v.SpecificationHeader!.Code) : query.OrderByDescending(v => v.SpecificationHeader!.Code);
                    break;
                case "specname":
                case "specificationname":
                    query = isAsc ? query.OrderBy(v => v.SpecificationHeader!.AliasName) : query.OrderByDescending(v => v.SpecificationHeader!.AliasName);
                    break;
                case "isdefault":
                    query = isAsc ? query.OrderBy(v => v.IsDefault) : query.OrderByDescending(v => v.IsDefault);
                    break;
                default:
                    query = isAsc ? query.OrderBy(v => v.ID) : query.OrderByDescending(v => v.ID);
                    break;
            }

            int pageNo = filter.PageNumber < 1 ? 1 : filter.PageNumber;
            int pageSize = filter.PageSize < 1 ? 10 : filter.PageSize;

            var items = await query
                .Skip((pageNo - 1) * pageSize)
                .Take(pageSize)
                .Select(v => new SpecificationVersionListItemDto
                {
                    ID = v.ID,
                    SpecificationHeaderID = v.SpecificationHeaderID,
                    SpecificationCode = v.SpecificationHeader!.Code ?? string.Empty,
                    SpecificationName = v.SpecificationHeader.AliasName,
                    StandardReference = v.SpecificationHeader.StandardReference,
                    StandardOrganizationName = v.SpecificationHeader.StandardOrganization != null ? v.SpecificationHeader.StandardOrganization.Name : null,
                    Version = v.Version,
                    Year = v.Year,
                    Status = v.Status,
                    EffectiveDate = v.EffectiveDate,
                    SupersededDate = v.SupersededDate,
                    ReviewDate = v.ReviewDate,
                    ChangeReason = v.ChangeReason,
                    IsDefault = v.IsDefault,
                    StandardFile = v.StandardFile,
                    StandardFilePath = v.StandardFilePath,
                    UploadReferenceID = v.UploadReferenceID,
                    ParametersCount = v.Parameters.Count,
                    IsParentActive = v.SpecificationHeader.IsActive
                })
                .ToListAsync();

            return new PagedResponse<SpecificationVersionListItemDto>(items, totalCount, pageNo, pageSize);
        }

        public async Task<SpecificationVersion?> GetVersionByIdAsync(long id, string companyCode)
        {
            return await _context.SpecificationVersions
                .Include(v => v.SpecificationHeader)
                    .ThenInclude(h => h!.StandardOrganization)
                .FirstOrDefaultAsync(v => v.ID == id && v.SpecificationHeader != null && v.SpecificationHeader.CompanyCode == companyCode);
        }

        public async Task<SpecificationVersion?> GetVersionWithParametersByIdAsync(long id, string companyCode)
        {
            return await _context.SpecificationVersions
                .Include(v => v.SpecificationHeader)
                    .ThenInclude(h => h!.StandardOrganization)
                .Include(v => v.Parameters)
                    .ThenInclude(p => p.Parameter)
                        .ThenInclude(pm => pm!.ParameterUnit)
                .FirstOrDefaultAsync(v => v.ID == id && v.SpecificationHeader != null && v.SpecificationHeader.CompanyCode == companyCode);
        }

        public async Task<bool> ExistsVersionAsync(long specId, string version, long? excludeId = null)
        {
            var normalized = version.Trim().ToLower();
            var query = _context.SpecificationVersions
                .Where(v => v.SpecificationHeaderID == specId && v.Version.Trim().ToLower() == normalized);

            if (excludeId.HasValue && excludeId.Value > 0)
            {
                query = query.Where(v => v.ID != excludeId.Value);
            }

            return await query.AnyAsync();
        }

        public async Task<SpecificationHeader?> GetParentSpecificationAsync(long specId, string companyCode)
        {
            return await _context.SpecificationHeaders
                .FirstOrDefaultAsync(m => m.ID == specId && m.CompanyCode == companyCode);
        }

        public async Task<SpecificationVersion> CreateVersionAsync(SpecificationVersion version)
        {
            _context.SpecificationVersions.Add(version);
            await _context.SaveChangesAsync();
            return version;
        }

        public async Task<SpecificationVersion> UpdateVersionAsync(SpecificationVersion version)
        {
            _context.SpecificationVersions.Update(version);
            await _context.SaveChangesAsync();
            return version;
        }

        public async Task UnsetDefaultVersionsAsync(long specId, long? exceptVersionId = null)
        {
            var query = _context.SpecificationVersions
                .Where(v => v.SpecificationHeaderID == specId && v.IsDefault);

            if (exceptVersionId.HasValue && exceptVersionId.Value > 0)
            {
                query = query.Where(v => v.ID != exceptVersionId.Value);
            }

            var defaults = await query.ToListAsync();
            if (defaults.Any())
            {
                foreach (var d in defaults)
                {
                    d.IsDefault = false;
                }
                await _context.SaveChangesAsync();
            }
        }

        public async Task SetDefaultVersionAsync(long specId, long versionId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Step 1: Unset any existing default version for this specification
                var existingDefaults = await _context.SpecificationVersions
                    .Where(v => v.SpecificationHeaderID == specId && v.IsDefault && v.ID != versionId)
                    .ToListAsync();

                foreach (var d in existingDefaults)
                {
                    d.IsDefault = false;
                }

                // Step 2: Set target version as Default
                var target = await _context.SpecificationVersions
                    .FirstOrDefaultAsync(v => v.ID == versionId && v.SpecificationHeaderID == specId);

                if (target != null)
                {
                    target.IsDefault = true;
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<List<SpecificationVersionParameterDto>> GetVersionParametersAsync(long versionId, string companyCode)
        {
            return await _context.SpecificationVersionParameters
                .Include(p => p.SpecificationVersion)
                    .ThenInclude(v => v!.SpecificationHeader)
                .Include(p => p.Parameter)
                    .ThenInclude(pm => pm!.ParameterUnit)
                .Where(p => p.SpecificationVersionID == versionId
                         && p.SpecificationVersion != null
                         && p.SpecificationVersion.SpecificationHeader != null
                         && p.SpecificationVersion.SpecificationHeader.CompanyCode == companyCode)
                .OrderBy(p => p.SortOrder)
                .Select(p => new SpecificationVersionParameterDto
                {
                    ID = p.ID,
                    ParameterID = p.ParameterID,
                    ParameterCode = p.Parameter != null ? p.Parameter.Code : null,
                    ParameterName = p.Parameter != null ? p.Parameter.Name : null,
                    UnitID = p.Parameter != null ? p.Parameter.ParameterUnitID : null,
                    UnitName = p.Parameter != null && p.Parameter.ParameterUnit != null ? p.Parameter.ParameterUnit.Name : null,
                    UnitSymbol = p.Parameter != null && p.Parameter.ParameterUnit != null ? p.Parameter.ParameterUnit.Symbol : null,
                    SortOrder = p.SortOrder,
                    Comment = p.Comment
                })
                .ToListAsync();
        }

        public async Task SaveVersionParametersAsync(long versionId, List<SpecificationVersionParameter> parameters)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var existing = await _context.SpecificationVersionParameters
                    .Where(p => p.SpecificationVersionID == versionId)
                    .ToListAsync();

                _context.SpecificationVersionParameters.RemoveRange(existing);
                await _context.SaveChangesAsync();

                if (parameters.Any())
                {
                    foreach (var p in parameters)
                    {
                        p.ID = 0;
                        p.SpecificationVersionID = versionId;
                    }
                    await _context.SpecificationVersionParameters.AddRangeAsync(parameters);
                    await _context.SaveChangesAsync();
                }

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<List<SpecificationVersionDropdownDto>> GetDropdownBySpecificationIdAsync(long specId, string companyCode, bool includeAll = false)
        {
            return await _context.SpecificationVersions
                .Where(v => v.SpecificationHeaderID == specId && v.SpecificationHeader != null && v.SpecificationHeader.CompanyCode == companyCode && (includeAll || v.Status == VersionStatus.Active))
                .OrderByDescending(v => v.IsDefault)
                .ThenByDescending(v => v.ID)
                .Select(v => new SpecificationVersionDropdownDto
                {
                    ID = v.ID,
                    Version = v.Version,
                    Year = v.Year,
                    IsDefault = v.IsDefault,
                    Status = v.Status
                })
                .ToListAsync();
        }

        public async Task<bool> HasDownstreamReferencesAsync(long versionId)
        {
            // Reserved for downstream universal executions or line configurations
            return await Task.FromResult(false);
        }
    }
}
