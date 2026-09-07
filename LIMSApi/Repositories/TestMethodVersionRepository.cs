using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers.Enums;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Repositories
{
    public class TestMethodVersionRepository : ITestMethodVersionRepository
    {
        private readonly LIMSContext _context;

        public TestMethodVersionRepository(LIMSContext context)
        {
            _context = context;
        }

        public async Task<PagedResponse<TestMethodVersionListItemDto>> GetPagedVersionsAsync(TestMethodVersionFilterDto filter, string companyCode)
        {
            var query = _context.TestMethodSpecificationVersions
                .Include(v => v.TestMethodSpecification)
                .Include(v => v.Parameters)
                .Where(v => v.TestMethodSpecification != null && v.TestMethodSpecification.CompanyCode == companyCode)
                .AsNoTracking()
                .AsQueryable();

            if (filter.TestMethodSpecificationID.HasValue && filter.TestMethodSpecificationID.Value > 0)
            {
                query = query.Where(v => v.TestMethodSpecificationID == filter.TestMethodSpecificationID.Value);
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
                case "methodcode":
                    query = isAsc ? query.OrderBy(v => v.TestMethodSpecification!.Code) : query.OrderByDescending(v => v.TestMethodSpecification!.Code);
                    break;
                case "methodname":
                    query = isAsc ? query.OrderBy(v => v.TestMethodSpecification!.Name) : query.OrderByDescending(v => v.TestMethodSpecification!.Name);
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
                .Select(v => new TestMethodVersionListItemDto
                {
                    ID = v.ID,
                    TestMethodSpecificationID = v.TestMethodSpecificationID,
                    TestMethodCode = v.TestMethodSpecification!.Code ?? v.TestMethodSpecification.TestMethodStandard ?? string.Empty,
                    TestMethodName = v.TestMethodSpecification.Name ?? v.TestMethodSpecification.DisplayTitle ?? string.Empty,
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
                    IsParentActive = v.TestMethodSpecification.IsActive
                })
                .ToListAsync();

            return new PagedResponse<TestMethodVersionListItemDto>(items, totalCount, pageNo, pageSize);
        }

        public async Task<TestMethodSpecificationVersion?> GetVersionByIdAsync(long id, string companyCode)
        {
            return await _context.TestMethodSpecificationVersions
                .Include(v => v.TestMethodSpecification)
                .FirstOrDefaultAsync(v => v.ID == id && v.TestMethodSpecification != null && v.TestMethodSpecification.CompanyCode == companyCode);
        }

        public async Task<TestMethodSpecificationVersion?> GetVersionWithParametersByIdAsync(long id, string companyCode)
        {
            return await _context.TestMethodSpecificationVersions
                .Include(v => v.TestMethodSpecification)
                .Include(v => v.Parameters)
                    .ThenInclude(p => p.Parameter)
                        .ThenInclude(pm => pm!.ParameterUnit)
                .FirstOrDefaultAsync(v => v.ID == id && v.TestMethodSpecification != null && v.TestMethodSpecification.CompanyCode == companyCode);
        }

        public async Task<bool> ExistsVersionAsync(long methodId, string version, long? excludeId = null)
        {
            var normalized = version.Trim().ToLower();
            var query = _context.TestMethodSpecificationVersions
                .Where(v => v.TestMethodSpecificationID == methodId && v.Version.Trim().ToLower() == normalized);

            if (excludeId.HasValue && excludeId.Value > 0)
            {
                query = query.Where(v => v.ID != excludeId.Value);
            }

            return await query.AnyAsync();
        }

        public async Task<TestMethodSpecification?> GetParentMethodAsync(long methodId, string companyCode)
        {
            return await _context.TestMethodSpecifications
                .FirstOrDefaultAsync(m => m.ID == methodId && m.CompanyCode == companyCode);
        }

        public async Task<TestMethodSpecificationVersion> CreateVersionAsync(TestMethodSpecificationVersion version)
        {
            _context.TestMethodSpecificationVersions.Add(version);
            await _context.SaveChangesAsync();
            return version;
        }

        public async Task<TestMethodSpecificationVersion> UpdateVersionAsync(TestMethodSpecificationVersion version)
        {
            _context.TestMethodSpecificationVersions.Update(version);
            await _context.SaveChangesAsync();
            return version;
        }

        public async Task UnsetDefaultVersionsAsync(long methodId, long? exceptVersionId = null)
        {
            var query = _context.TestMethodSpecificationVersions
                .Where(v => v.TestMethodSpecificationID == methodId && v.IsDefault);

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

        public async Task SetDefaultVersionAsync(long methodId, long versionId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Step 1: Unset any existing default version for this method
                var existingDefaults = await _context.TestMethodSpecificationVersions
                    .Where(v => v.TestMethodSpecificationID == methodId && v.IsDefault && v.ID != versionId)
                    .ToListAsync();

                if (existingDefaults.Any())
                {
                    foreach (var d in existingDefaults)
                    {
                        d.IsDefault = false;
                    }
                    await _context.SaveChangesAsync();
                }

                // Step 2: Set the target version as default
                var target = await _context.TestMethodSpecificationVersions
                    .FirstOrDefaultAsync(v => v.ID == versionId && v.TestMethodSpecificationID == methodId);

                if (target != null && !target.IsDefault)
                {
                    target.IsDefault = true;
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


        public async Task<List<TestMethodVersionParameterDto>> GetVersionParametersAsync(long versionId, string companyCode)
        {
            return await _context.TestMethodSpecificationParameters
                .Include(p => p.Parameter)
                    .ThenInclude(pm => pm!.ParameterUnit)
                .Include(p => p.TestMethodSpecificationVersion)
                    .ThenInclude(v => v!.TestMethodSpecification)
                .Where(p => p.TestMethodSpecificationVersionID == versionId 
                         && p.TestMethodSpecificationVersion != null 
                         && p.TestMethodSpecificationVersion.TestMethodSpecification != null 
                         && p.TestMethodSpecificationVersion.TestMethodSpecification.CompanyCode == companyCode)
                .OrderBy(p => p.SortOrder)
                .ThenBy(p => p.ID)
                .Select(p => new TestMethodVersionParameterDto
                {
                    ID = p.ID,
                    ParameterID = p.ParameterID,
                    ParameterCode = p.Parameter != null ? p.Parameter.Code : string.Empty,
                    ParameterName = p.Parameter != null ? p.Parameter.Name : string.Empty,
                    UnitID = p.Parameter != null && p.Parameter.ParameterUnit != null ? p.Parameter.ParameterUnit.ID : p.ParameterUnitID,
                    UnitName = p.Parameter != null && p.Parameter.ParameterUnit != null ? p.Parameter.ParameterUnit.Name : null,
                    UnitSymbol = p.Parameter != null && p.Parameter.ParameterUnit != null ? p.Parameter.ParameterUnit.Symbol : null,
                    ParameterUnitEquivalentID = p.ParameterUnitEquivalentID,
                    SortOrder = p.SortOrder,
                    Comment = p.Comment
                })
                .ToListAsync();
        }

        public async Task SaveVersionParametersAsync(long versionId, List<TestMethodSpecificationParameter> parameters)
        {
            var existing = await _context.TestMethodSpecificationParameters
                .Where(p => p.TestMethodSpecificationVersionID == versionId)
                .ToListAsync();

            if (existing.Any())
            {
                _context.TestMethodSpecificationParameters.RemoveRange(existing);
            }

            if (parameters.Any())
            {
                foreach (var p in parameters)
                {
                    p.TestMethodSpecificationVersionID = versionId;
                    _context.TestMethodSpecificationParameters.Add(p);
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task<List<TestMethodVersionDropdownDto>> GetDropdownByMethodIdAsync(long methodId, string companyCode)
        {
            return await _context.TestMethodSpecificationVersions
                .Include(v => v.TestMethodSpecification)
                .Where(v => v.TestMethodSpecificationID == methodId 
                         && v.Status == VersionStatus.Active
                         && v.TestMethodSpecification != null 
                         && v.TestMethodSpecification.CompanyCode == companyCode)
                .OrderByDescending(v => v.IsDefault)
                .ThenByDescending(v => v.Year)
                .ThenByDescending(v => v.ID)
                .Select(v => new TestMethodVersionDropdownDto
                {
                    ID = v.ID,
                    Version = v.Version,
                    Year = v.Year,
                    IsDefault = v.IsDefault,
                    Status = v.Status
                })
                .ToListAsync();
        }
    }
}
