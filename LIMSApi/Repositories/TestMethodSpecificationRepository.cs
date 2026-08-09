using System.Linq.Dynamic.Core;
using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Helpers.Enums;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Repositories
{
    public class TestMethodSpecificationRepository : ITestMethodSpecificationRepository
    {
        private readonly LIMSContext _context;
        private LoggedInUserDTO loggedInUser;

        public TestMethodSpecificationRepository(LIMSContext context)
        {
            _context = context;
            loggedInUser = LoggedInUserProvider.CurrentUser;
        }

        public async Task AddTestMethodSpecification(TestMethodSpecification model)
        {
            await _context.TestMethodSpecifications.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteTestMethodSpecification(long id)
        {
            var existingTestMethodSpecification = await _context.TestMethodSpecifications.FirstOrDefaultAsync(x => x.ID == id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);
            if (existingTestMethodSpecification != null)
            {
                existingTestMethodSpecification.IsActive = false;
                existingTestMethodSpecification.ModifiedOn = DateTime.UtcNow;
                _context.TestMethodSpecifications.Update(existingTestMethodSpecification);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<TestMethodSpecification?> GetTestMethodSpecificationById(long id)
        {
            var spec = await _context.TestMethodSpecifications
                .Include(x => x.Versions).ThenInclude(v => v.Parameters).ThenInclude(p => p.Parameter)
                .Include(x => x.MetalClassifications).ThenInclude(m => m.MetalClassification)
                .FirstOrDefaultAsync(x => x.ID == id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);

            if (spec != null)
            {
                // Sort in-memory: Active first, then Draft, Superseded, Withdrawn; then by EffectiveDate DESC
                spec.Versions = spec.Versions
                    .OrderBy(v => v.Status == VersionStatus.Active ? 0 :
                                  v.Status == VersionStatus.Draft ? 1 :
                                  v.Status == VersionStatus.Superseded ? 2 : 3)
                    .ThenByDescending(v => v.EffectiveDate)
                    .ToList();
            }
            return spec;
        }

        public async Task UpdateTestMethodSpecification(TestMethodSpecification model)
        {
            var specId = model.ID;
            var defaultVersionId = model.Versions.FirstOrDefault(v => v.IsDefault)?.ID ?? 0;

            // To avoid unique constraint violation on (TestMethodSpecificationID, IsDefault),
            // we must clear IsDefault on all other versions first before updating.
            // This prevents EF from trying to set multiple versions as default simultaneously.
            if (defaultVersionId > 0)
            {
                // Clear IsDefault on all versions for this spec except the one we're setting as default
                await _context.TestMethodSpecificationVersions
                    .Where(v => v.TestMethodSpecificationID == specId && v.ID != defaultVersionId && v.IsDefault)
                    .ExecuteUpdateAsync(s => s.SetProperty(v => v.IsDefault, false));
            }

            // Now update the specification and its versions
            _context.TestMethodSpecifications.Update(model);
            await _context.SaveChangesAsync();
        }

        public async Task<PagedResponse<object>> GetAllTestMethodSpecifications(PageFilter filter)
        {
            var _query = from c in _context.TestMethodSpecifications
                         join s in _context.StandardOrganizationMasters on c.StandardOrganizationID equals s.ID
                         where c.IsActive && c.CompanyCode == loggedInUser.CompanyCode
                         let defaultVersion = c.Versions.FirstOrDefault(v => v.IsDefault)
                         select new
                         {
                             c.ID,
                             c.Name,
                             c.DisplayTitle,
                             c.StandardOrganizationID,
                             StandardOrganizationName = s.Name,
                             c.TestMethodStandard,
                             DefaultVersion = defaultVersion != null ? defaultVersion.Version : "",
                             DefaultVersionYear = defaultVersion != null ? defaultVersion.Year : (string?)null,
                             c.IsDisabled,
                             c.CreatedBy,
                             c.CreatedOn,
                             c.ModifiedOn,
                         };

            _query = _query.AsQueryable().ApplyFilters(filter.Filter);

            if (!string.IsNullOrWhiteSpace(filter.searchTerm))
            {
                var search = filter.searchTerm.Trim();
                _query = _query.Where(x => (x.Name != null && x.Name.Contains(search))
                                     || (x.StandardOrganizationName != null && x.StandardOrganizationName.Contains(search))
                                     || (x.TestMethodStandard != null && x.TestMethodStandard.Contains(search))
                                     || (x.DefaultVersion != null && x.DefaultVersion.Contains(search))
                                     || (x.DefaultVersionYear != null && x.DefaultVersionYear.ToString()!.Contains(search))
                                     || (x.IsDisabled ? "disabled" : "active").Contains(search));
            }

            if (filter.SortByColumn != null)
            {
                _query = _query.OrderBy($"{filter.SortByColumn} {(filter.SortOrder == "asc" ? "ascending" : "descending")}");
            }

            return await _query.Cast<object>().ToPagedAsync(filter);
        }

        public async Task<List<DropdwonSelector>> GetTestMethodSpecificationDropdown(string? searchTerm, int pageNo = 0, int pageSize = 20)
        {
            if (pageNo < 0) pageNo = 0;

            var _query = from a in _context.TestMethodSpecifications where a.IsActive && a.CompanyCode == loggedInUser.CompanyCode select a;

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                if (FilterHelper.IsExactIdSearch(searchTerm, out long exactId))
                {
                    _query = _query.Where(x => x.ID == exactId);
                }
                else
                {
                    var search = searchTerm.Trim();
                    _query = _query.Where(x => (x.Name != null && x.Name.Contains(search)));
                }
            }

            var skip = pageNo * pageSize;

            var data = await (_query.Skip(skip).Take(pageSize).Select(x => new DropdwonSelector
            {
                Id = x.ID,
                Name = x.Name,
            })).ToListAsync();

            return data;
        }

        public async Task<bool> ExistsByName(string name)
        {
            return await _context.TestMethodSpecifications.AnyAsync(x => x.Name == name && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);
        }

        public async Task<bool> ExistsByNameAndNotId(string name, long Id)
        {
            return await _context.TestMethodSpecifications.AnyAsync(x => x.Name == name && x.ID != Id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);
        }

        public Task<List<DropdwonSelector>> GetTestMethodSpecificationsByStandard(long standardId)
        {
            var _query = from a in _context.TestMethodSpecifications
                         where a.IsActive && a.CompanyCode == loggedInUser.CompanyCode && a.StandardOrganizationID == standardId
                         select new DropdwonSelector
                         {
                             Id = a.ID,
                             Name = a.Name,
                         };
            return _query.ToListAsync();
        }

        public async Task<List<DropdwonSelector>> GetTestMethodsByMetalClassification(long metalClassificationId, string? searchTerm, int pageNo = 0, int pageSize = 20)
        {
            if (pageNo < 0) pageNo = 0;

            // Specs explicitly linked to this metal classification, plus specs with no metal-classification
            // link at all (treated as universal — applicable to any classification).
            var _query = from a in _context.TestMethodSpecifications
                         where a.IsActive && !a.IsDisabled && a.CompanyCode == loggedInUser.CompanyCode
                            && (a.MetalClassifications.Any(m => m.MetalClassificationID == metalClassificationId)
                                || !a.MetalClassifications.Any())
                         select a;

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                if (FilterHelper.IsExactIdSearch(searchTerm, out long exactId))
                {
                    _query = _query.Where(x => x.ID == exactId);
                }
                else
                {
                    var search = searchTerm.Trim();
                    _query = _query.Where(x => x.Name != null && x.Name.Contains(search));
                }
            }

            var skip = pageNo * pageSize;

            return await _query.OrderBy(x => x.Name).Skip(skip).Take(pageSize).Select(x => new DropdwonSelector
            {
                Id = x.ID,
                Name = !string.IsNullOrEmpty(x.DisplayTitle) ? x.DisplayTitle : x.Name,
            }).ToListAsync();
        }

        public async Task<int> GetVersionImpactCount(long versionId)
        {
            var labScopeCount = await _context.LabScopeSpecifications
                .CountAsync(l => l.TestMethodSpecificationVersionID == versionId);
            return labScopeCount;
        }

        public async Task<List<TestMethodSpecificationVersion>> GetVersionsDueForReview(DateTime cutoffDate)
        {
            return await _context.TestMethodSpecificationVersions
                .Include(v => v.TestMethodSpecification)
                .Where(v => v.Status == VersionStatus.Active
                    && v.ReviewDate != null
                    && v.ReviewDate <= cutoffDate
                    && v.ReviewDate >= DateTime.UtcNow)
                .ToListAsync();
        }

        public async Task<List<DropdwonSelector>> GetVersionsBySpecId(long specId, bool includeAll = false)
        {
            var query = _context.TestMethodSpecificationVersions
                .Where(v => v.TestMethodSpecificationID == specId);

            if (!includeAll)
                query = query.Where(v => v.Status == VersionStatus.Active);

            return await query
                .OrderBy(v => v.IsDefault ? 0 :
                              v.Status == VersionStatus.Active ? 1 :
                              v.Status == VersionStatus.Draft ? 2 :
                              v.Status == VersionStatus.Superseded ? 3 : 4)
                .Select(v => new DropdwonSelector
                {
                    Id = v.ID,
                    Name = v.Version + (v.Year != null ? " (" + v.Year + ")" : "")
                         + (v.IsDefault ? " ★" : "")
                         + (v.Status != VersionStatus.Active ? " [" + v.Status + "]" : ""),
                })
                .ToListAsync();
        }

        public async Task<bool> ExistsByOrgAndStandard(long orgId, string testMethodStandard, string? part)
        {
            return await _context.TestMethodSpecifications
                .AnyAsync(x => x.StandardOrganizationID == orgId
                            && x.TestMethodStandard == testMethodStandard
                            && x.Part == part
                            && x.IsActive
                            && x.CompanyCode == loggedInUser.CompanyCode);
        }

        public async Task<List<DropdwonSelector>> GetAllStandardOrganizations()
        {
            return await _context.StandardOrganizationMasters
                .Where(x => x.IsActive && x.CompanyCode == loggedInUser.CompanyCode)
                .OrderBy(x => x.Name)
                .Select(x => new DropdwonSelector
                {
                    Id = x.ID,
                    Name = x.Name,
                })
                .ToListAsync();
        }

        public async Task AddRangeAsync(List<TestMethodSpecification> specs)
        {
            await _context.TestMethodSpecifications.AddRangeAsync(specs);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateVersionFileRef(long versionId, string filePath, string originalFileName, long uploadRefId)
        {
            var version = await _context.TestMethodSpecificationVersions.FindAsync(versionId);
            if (version != null)
            {
                version.StandardFilePath = filePath;
                version.StandardFile = originalFileName;
                version.UploadReferenceID = uploadRefId;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<DropdwonSelector>> GetTestMethodSpecificationVersionDropdown(string? searchTerm, int pageNo = 0, int pageSize = 20)
        {
            if (pageNo < 0) pageNo = 0;

            var query = _context.TestMethodSpecificationVersions
                .Include(v => v.TestMethodSpecification)
                .Where(v => v.Status == VersionStatus.Active 
                    && v.TestMethodSpecification != null 
                    && v.TestMethodSpecification.IsActive 
                    && v.TestMethodSpecification.CompanyCode == loggedInUser.CompanyCode);

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var search = searchTerm.Trim().ToLower();
                query = query.Where(v => (v.TestMethodSpecification.Name != null && v.TestMethodSpecification.Name.Contains(search)) 
                    || (v.Version != null && v.Version.Contains(search)));
            }

            var skip = pageNo * pageSize;

            return await query
                .OrderBy(v => v.TestMethodSpecification.Name)
                .ThenByDescending(v => v.CreatedOn)
                .Skip(skip)
                .Take(pageSize)
                .Select(v => new DropdwonSelector
                {
                    Id = v.ID,
                    Name = v.TestMethodSpecification.Name + " (" + v.Version + ")",
                    AdditionalValues = new Dictionary<string, object>
                    {
                        { "TestMethodStandard", v.TestMethodSpecification.TestMethodStandard },
                        { "Name", v.TestMethodSpecification.Name },
                        { "Version", v.Version },
                        { "TestMethodSpecificationID", v.TestMethodSpecificationID }
                    }
                })
                .ToListAsync();
        }
    }
}
