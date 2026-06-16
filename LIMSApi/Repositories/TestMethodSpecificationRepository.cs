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
            _context.TestMethodSpecifications.Update(model);
            await _context.SaveChangesAsync();
        }

        public async Task<PagedResponse<object>> GetAllTestMethodSpecifications(PageFilter filter)
        {
            var _query = from c in _context.TestMethodSpecifications
                         join s in _context.StandardOrganizationMasters on c.StandardOrganizationID equals s.ID
                         where c.IsActive && c.CompanyCode == loggedInUser.CompanyCode
                         let activeVersion = c.Versions.FirstOrDefault(v => v.Status == VersionStatus.Active)
                         select new
                         {
                             c.ID,
                             c.Name,
                             c.DisplayTitle,
                             c.StandardOrganizationID,
                             StandardOrganizationName = s.Name,
                             c.TestMethodStandard,
                             CurrentVersion = activeVersion != null ? activeVersion.Version : "",
                             CurrentVersionYear = activeVersion != null ? activeVersion.Year : (string?)null,
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
                                     || (x.CurrentVersion != null && x.CurrentVersion.Contains(search))
                                     || (x.CurrentVersionYear != null && x.CurrentVersionYear.ToString()!.Contains(search))
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
            var productSpecCount = await _context.ProductSpecifications
                .CountAsync(p => p.TestMethodSpecificationVersionID == versionId && p.IsActive);
            var labScopeCount = await _context.LabScopeSpecifications
                .CountAsync(l => l.TestMethodSpecificationVersionID == versionId);
            return productSpecCount + labScopeCount;
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

        public async Task<List<DropdwonSelector>> GetActiveVersionsBySpecId(long specId)
        {
            return await _context.TestMethodSpecificationVersions
                .Where(v => v.TestMethodSpecificationID == specId && v.Status == VersionStatus.Active)
                .Select(v => new DropdwonSelector
                {
                    Id = v.ID,
                    Name = v.Version + (v.Year != null ? " (" + v.Year.ToString() + ")" : ""),
                })
                .ToListAsync();
        }
    }
}
