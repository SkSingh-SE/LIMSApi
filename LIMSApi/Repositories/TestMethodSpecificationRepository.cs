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
            return await GetTestMethodsByMetalClassification(0, searchTerm, pageNo, pageSize);
        }

        public async Task<List<DropdwonSelector>> GetTestMethodsByMetalClassification(long metalClassificationId, string? searchTerm, int pageNo = 0, int pageSize = 20)
        {
            if (pageNo < 0) pageNo = 0;

            // 1. Exact ID lookup for instantaneous single-item rebind (matches VersionID or TestMethodSpecificationID)
            if (!string.IsNullOrWhiteSpace(searchTerm) && FilterHelper.IsExactIdSearch(searchTerm, out long exactId))
            {
                var exactVersionMatch = await (from v in _context.TestMethodSpecificationVersions
                                               join tms in _context.TestMethodSpecifications on v.TestMethodSpecificationID equals tms.ID
                                               join so in _context.StandardOrganizationMasters on tms.StandardOrganizationID equals so.ID into soGroup
                                               from so in soGroup.DefaultIfEmpty()
                                               where (v.ID == exactId || tms.ID == exactId) && tms.IsActive && tms.CompanyCode == loggedInUser.CompanyCode
                                               orderby (v.ID == exactId ? 0 : (v.IsDefault ? 1 : 2))
                                               select new DropdwonSelector
                                               {
                                                   Id = v.ID,
                                                   Name = (tms.DisplayTitle ?? tms.Name) + " (" + v.Version + (!string.IsNullOrEmpty(v.Year) ? $" - {v.Year}" : "") + ")",
                                                   Level = 2,
                                                   Selectable = true,
                                                   NodeType = "Version",
                                                   ParentId = tms.ID,
                                                   IsHeader = false,
                                                   IsChild = true,
                                                   AdditionalValues = new Dictionary<string, object>
                                                   {
                                                       { "testMethodSpecificationId", tms.ID },
                                                       { "testMethodSpecificationName", tms.Name },
                                                       { "displayTitle", tms.DisplayTitle ?? tms.Name },
                                                       { "testMethodStandard", tms.TestMethodStandard },
                                                       { "versionId", v.ID },
                                                       { "versionName", v.Version },
                                                       { "year", v.Year ?? "" },
                                                       { "isDefault", v.IsDefault },
                                                       { "standardOrgId", so != null ? so.ID : 0 },
                                                       { "standardOrgName", so != null ? so.Name : "" }
                                                   }
                                               }).FirstOrDefaultAsync();

                if (exactVersionMatch != null)
                {
                    return new List<DropdwonSelector> { exactVersionMatch };
                }

                // Fallback for unversioned master test method spec matching exact ID
                var unversionedMatch = await (from tms in _context.TestMethodSpecifications
                                              join so in _context.StandardOrganizationMasters on tms.StandardOrganizationID equals so.ID into soGroup
                                              from so in soGroup.DefaultIfEmpty()
                                              where tms.ID == exactId && tms.IsActive && tms.CompanyCode == loggedInUser.CompanyCode
                                              select new DropdwonSelector
                                              {
                                                  Id = tms.ID,
                                                  Name = tms.DisplayTitle ?? tms.Name,
                                                  Level = 1,
                                                  Selectable = true,
                                                  NodeType = "TestMethodStandard",
                                                  IsHeader = false,
                                                  IsChild = false,
                                                  AdditionalValues = new Dictionary<string, object>
                                                  {
                                                      { "testMethodSpecificationId", tms.ID },
                                                      { "testMethodSpecificationName", tms.Name },
                                                      { "displayTitle", tms.DisplayTitle ?? tms.Name },
                                                      { "versionId", 0 },
                                                      { "standardOrgId", so != null ? so.ID : 0 },
                                                      { "standardOrgName", so != null ? so.Name : "" }
                                                  }
                                              }).FirstOrDefaultAsync();

                if (unversionedMatch != null)
                {
                    return new List<DropdwonSelector> { unversionedMatch };
                }
            }

            // 2. 3-Tier Hierarchy Query: StandardOrganization (Level 0) -> TestMethodStandard (Level 1) -> Version (Level 2 Leaf)
            var query = from tms in _context.TestMethodSpecifications
                        join so in _context.StandardOrganizationMasters on tms.StandardOrganizationID equals so.ID into soGroup
                        from so in soGroup.DefaultIfEmpty()
                        join v in _context.TestMethodSpecificationVersions on tms.ID equals v.TestMethodSpecificationID into vGroup
                        from v in vGroup.DefaultIfEmpty()
                        where tms.IsActive && !tms.IsDisabled && tms.CompanyCode == loggedInUser.CompanyCode
                              && (v == null || v.Status == VersionStatus.Active || v.Status == VersionStatus.Superseded)
                        select new
                        {
                            TMSID = tms.ID,
                            TMSName = tms.Name,
                            TMSStandard = tms.TestMethodStandard,
                            TMSDisplayTitle = tms.DisplayTitle,
                            StandardOrgID = so != null ? so.ID : 0,
                            StandardOrgName = so != null ? so.Name : "Other Standards",
                            VersionID = (long?)(v != null ? v.ID : (long?)null),
                            VersionName = v != null ? v.Version : null,
                            VersionYear = v != null ? v.Year : null,
                            VersionStatus = (VersionStatus?)(v != null ? v.Status : (VersionStatus?)null),
                            IsDefault = v != null && v.IsDefault,
                            IsMetalMatch = metalClassificationId > 0 && (_context.TestMethodSpecificationMetalClassifications
                                            .Any(m => m.TestMethodSpecificationID == tms.ID && m.MetalClassificationID == metalClassificationId)
                                     || !_context.TestMethodSpecificationMetalClassifications
                                            .Any(m => m.TestMethodSpecificationID == tms.ID))
                        };

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var search = searchTerm.Trim();
                query = query.Where(x => (x.TMSName != null && x.TMSName.Contains(search))
                                      || (x.TMSStandard != null && x.TMSStandard.Contains(search))
                                      || (x.TMSDisplayTitle != null && x.TMSDisplayTitle.Contains(search))
                                      || (x.StandardOrgName != null && x.StandardOrgName.Contains(search))
                                      || (x.VersionName != null && x.VersionName.Contains(search))
                                      || (x.VersionYear != null && x.VersionYear.Contains(search)));
            }

            var rawData = await query
                .OrderBy(x => metalClassificationId > 0 ? (x.IsMetalMatch ? 0 : 1) : 0)
                .ThenBy(x => x.StandardOrgName)
                .ThenBy(x => x.TMSStandard)
                .ThenBy(x => x.TMSName)
                .ThenBy(x => x.IsDefault ? 0 : (x.VersionStatus == VersionStatus.Active ? 1 : 2))
                .ThenBy(x => x.VersionName)
                .ToListAsync();

            var result = new List<DropdwonSelector>();

            if (rawData.Count > 0)
            {
                // Group by Standard Organization (Level 0)
                var orgGroups = rawData.GroupBy(x => new { x.StandardOrgID, x.StandardOrgName });

                foreach (var orgGroup in orgGroups)
                {
                    // 1. Level 0: Standard Organization Header (Non-selectable)
                    result.Add(new DropdwonSelector
                    {
                        Id = 0,
                        Name = orgGroup.Key.StandardOrgName,
                        Level = 0,
                        Selectable = false,
                        NodeType = "Organization",
                        IsHeader = true,
                        IsChild = false,
                        AdditionalValues = new Dictionary<string, object>
                        {
                            { "standardOrgId", orgGroup.Key.StandardOrgID },
                            { "standardOrgName", orgGroup.Key.StandardOrgName }
                        }
                    });

                    // 2. Level 1: Test Method Standard Intermediate Header (or Leaf if no versions)
                    var specGroups = orgGroup.GroupBy(x => new
                    {
                        x.TMSID,
                        x.TMSName,
                        x.TMSStandard,
                        DisplayTitle = !string.IsNullOrEmpty(x.TMSDisplayTitle)
                            ? x.TMSDisplayTitle
                            : (!string.IsNullOrEmpty(x.TMSStandard) ? (x.TMSName + " (" + x.TMSStandard + ")") : x.TMSName)
                    });

                    foreach (var specGroup in specGroups)
                    {
                        var versions = specGroup.Where(x => x.VersionID != null).ToList();

                        if (versions.Count == 0)
                        {
                            // Unversioned specification: Selectable Level 1 Leaf
                            result.Add(new DropdwonSelector
                            {
                                Id = specGroup.Key.TMSID,
                                Name = specGroup.Key.DisplayTitle,
                                Level = 1,
                                Selectable = true,
                                NodeType = "TestMethodStandard",
                                ParentId = orgGroup.Key.StandardOrgID,
                                IsHeader = false,
                                IsChild = true,
                                AdditionalValues = new Dictionary<string, object>
                                {
                                    { "testMethodSpecificationId", specGroup.Key.TMSID },
                                    { "testMethodSpecificationName", specGroup.Key.TMSName },
                                    { "displayTitle", specGroup.Key.DisplayTitle },
                                    { "versionId", 0 },
                                    { "standardOrgId", orgGroup.Key.StandardOrgID },
                                    { "standardOrgName", orgGroup.Key.StandardOrgName }
                                }
                            });
                        }
                        else
                        {
                            // Versioned specification: Non-selectable Level 1 Header
                            result.Add(new DropdwonSelector
                            {
                                Id = 0,
                                Name = specGroup.Key.DisplayTitle,
                                Level = 1,
                                Selectable = false,
                                NodeType = "TestMethodStandard",
                                ParentId = orgGroup.Key.StandardOrgID,
                                IsHeader = true,
                                IsChild = true,
                                AdditionalValues = new Dictionary<string, object>
                                {
                                    { "standardOrgId", orgGroup.Key.StandardOrgID },
                                    { "testMethodSpecificationId", specGroup.Key.TMSID },
                                    { "testMethodSpecificationName", specGroup.Key.TMSName },
                                    { "displayTitle", specGroup.Key.DisplayTitle }
                                }
                            });

                            // 3. Level 2: Versions (Selectable Leaf - Active and Superseded)
                            foreach (var v in versions)
                            {
                                var versionLabel = v.VersionName 
                                    + (!string.IsNullOrEmpty(v.VersionYear) ? $" - {v.VersionYear}" : "") 
                                    + (v.IsDefault ? " ★" : "")
                                    + (v.VersionStatus == VersionStatus.Superseded ? " [Superseded]" : "");

                                result.Add(new DropdwonSelector
                                {
                                    Id = v.VersionID!.Value,
                                    Name = versionLabel,
                                    Level = 2,
                                    Selectable = true,
                                    NodeType = "Version",
                                    ParentId = specGroup.Key.TMSID,
                                    IsHeader = false,
                                    IsChild = true,
                                    AdditionalValues = new Dictionary<string, object>
                                    {
                                        { "testMethodSpecificationId", specGroup.Key.TMSID },
                                        { "testMethodSpecificationName", specGroup.Key.TMSName },
                                        { "displayTitle", specGroup.Key.DisplayTitle },
                                        { "testMethodStandard", specGroup.Key.TMSStandard },
                                        { "versionId", v.VersionID.Value },
                                        { "versionName", v.VersionName ?? "" },
                                        { "year", v.VersionYear ?? "" },
                                        { "status", v.VersionStatus?.ToString() ?? "" },
                                        { "isSuperseded", v.VersionStatus == VersionStatus.Superseded },
                                        { "isDefault", v.IsDefault },
                                        { "standardOrgId", orgGroup.Key.StandardOrgID },
                                        { "standardOrgName", orgGroup.Key.StandardOrgName }
                                    }
                                });
                            }
                        }
                    }
                }
            }

            return result;
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

        public async Task<List<DropdwonSelector>> GetTestMethodSpecificationVersionDropdown(string? searchTerm, int pageNo = 0, int pageSize = 20, long metalId = 0)
        {
            if (pageNo < 0) pageNo = 0;

            // 1. Exact ID lookup for instantaneous single-item rebind
            if (!string.IsNullOrWhiteSpace(searchTerm) && FilterHelper.IsExactIdSearch(searchTerm, out long exactId))
            {
                var exactRaw = await (from v in _context.TestMethodSpecificationVersions
                                      join s in _context.TestMethodSpecifications on v.TestMethodSpecificationID equals s.ID
                                      join so in _context.StandardOrganizationMasters on s.StandardOrganizationID equals so.ID into soGroup
                                      from so in soGroup.DefaultIfEmpty()
                                      where v.ID == exactId && (v.Status == VersionStatus.Active || v.Status == VersionStatus.Superseded) && s.IsActive && !s.IsDisabled
                                      select new
                                      {
                                          v.ID,
                                          v.Version,
                                          v.Year,
                                          v.Status,
                                          v.IsDefault,
                                          SpecID = s.ID,
                                          SpecName = s.Name,
                                          s.TestMethodStandard,
                                          s.Part,
                                          s.DisplayTitle,
                                          StandardOrgID = so != null ? so.ID : 0,
                                          StandardOrgName = so != null ? so.Name : ""
                                      }).FirstOrDefaultAsync();

                if (exactRaw != null)
                {
                    var dispTitle = !string.IsNullOrEmpty(exactRaw.DisplayTitle)
                        ? exactRaw.DisplayTitle
                        : (!string.IsNullOrEmpty(exactRaw.Part)
                            ? $"{exactRaw.TestMethodStandard} ({exactRaw.Part}) : {exactRaw.SpecName}"
                            : $"{exactRaw.TestMethodStandard} : {exactRaw.SpecName}");

                    var versionLabel = exactRaw.Version 
                        + (!string.IsNullOrEmpty(exactRaw.Year) ? $" ({exactRaw.Year})" : "")
                        + (exactRaw.IsDefault ? " ★" : "")
                        + (exactRaw.Status == VersionStatus.Superseded ? " [Superseded]" : "");

                    var exactMatch = new DropdwonSelector
                    {
                        Id = exactRaw.ID,
                        Name = versionLabel,
                        Level = 2,
                        Selectable = true,
                        NodeType = "Version",
                        ParentId = exactRaw.SpecID,
                        IsHeader = false,
                        IsChild = true,
                        AdditionalValues = new Dictionary<string, object>
                        {
                            { "testMethodSpecificationId", exactRaw.SpecID },
                            { "testMethodSpecificationName", exactRaw.SpecName },
                            { "testMethodStandard", exactRaw.TestMethodStandard },
                            { "displayTitle", dispTitle },
                            { "versionId", exactRaw.ID },
                            { "version", exactRaw.Version },
                            { "year", exactRaw.Year ?? "" },
                            { "status", exactRaw.Status.ToString() },
                            { "isSuperseded", exactRaw.Status == VersionStatus.Superseded },
                            { "isDefault", exactRaw.IsDefault },
                            { "standardOrgId", exactRaw.StandardOrgID },
                            { "standardOrgName", exactRaw.StandardOrgName },
                            { "TestMethodSpecificationID", exactRaw.SpecID },
                            { "TestMethodStandard", exactRaw.TestMethodStandard },
                            { "Name", exactRaw.SpecName },
                            { "Version", exactRaw.Version }
                        }
                    };

                    return new List<DropdwonSelector> { exactMatch };
                }
            }

            // 2. 3-Tier Hierarchy Query: StandardOrganization (Level 0) -> TestMethodSpecification (Level 1) -> Version (Level 2 Leaf)
            var query = from v in _context.TestMethodSpecificationVersions
                        join s in _context.TestMethodSpecifications on v.TestMethodSpecificationID equals s.ID
                        join so in _context.StandardOrganizationMasters on s.StandardOrganizationID equals so.ID into soGroup
                        from so in soGroup.DefaultIfEmpty()
                        where (v.Status == VersionStatus.Active || v.Status == VersionStatus.Superseded)
                           && s.IsActive
                           && !s.IsDisabled
                           && s.CompanyCode == loggedInUser.CompanyCode
                        select new
                        {
                            VersionID = v.ID,
                            VersionName = v.Version,
                            VersionYear = v.Year,
                            VersionStatus = v.Status,
                            IsDefault = v.IsDefault,
                            CreatedOn = v.CreatedOn,
                            SpecHeaderID = s.ID,
                            SpecName = s.Name,
                            TestMethodStandard = s.TestMethodStandard,
                            SpecPart = s.Part,
                            SpecDisplayTitle = s.DisplayTitle,
                            StandardOrgID = so != null ? so.ID : 0,
                            StandardOrgName = so != null ? so.Name : "Other Standards",
                            IsMetalMatch = metalId > 0 && s.MetalClassifications.Any(m => m.MetalClassificationID == metalId)
                        };

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var search = searchTerm.Trim();
                query = query.Where(x => (x.VersionName != null && x.VersionName.Contains(search))
                                      || (x.SpecName != null && x.SpecName.Contains(search))
                                      || (x.TestMethodStandard != null && x.TestMethodStandard.Contains(search))
                                      || (x.SpecPart != null && x.SpecPart.Contains(search))
                                      || (x.SpecDisplayTitle != null && x.SpecDisplayTitle.Contains(search))
                                      || (x.StandardOrgName != null && x.StandardOrgName.Contains(search)));
            }

            var rawData = await query
                .OrderBy(x => metalId > 0 ? (x.IsMetalMatch ? 0 : 1) : 0)
                .ThenBy(x => x.StandardOrgName)
                .ThenBy(x => x.TestMethodStandard)
                .ThenBy(x => x.SpecName)
                .ThenBy(x => x.IsDefault ? 0 : (x.VersionStatus == VersionStatus.Active ? 1 : 2))
                .ThenByDescending(x => x.CreatedOn)
                .ToListAsync();

            var result = new List<DropdwonSelector>();

            if (rawData.Count > 0)
            {
                // Group by Standard Organization (Level 0)
                var orgGroups = rawData.GroupBy(x => new { x.StandardOrgID, x.StandardOrgName });

                foreach (var orgGroup in orgGroups)
                {
                    // 1. Level 0: Standard Organization Header (Non-selectable)
                    result.Add(new DropdwonSelector
                    {
                        Id = 0,
                        Name = orgGroup.Key.StandardOrgName,
                        Level = 0,
                        Selectable = false,
                        NodeType = "Organization",
                        IsHeader = true,
                        IsChild = false,
                        AdditionalValues = new Dictionary<string, object>
                        {
                            { "standardOrgId", orgGroup.Key.StandardOrgID },
                            { "standardOrgName", orgGroup.Key.StandardOrgName }
                        }
                    });

                    // 2. Level 1: Test Method Specification Intermediate Header (Non-selectable)
                    var specGroups = orgGroup.GroupBy(x => new
                    {
                        x.SpecHeaderID,
                        x.SpecName,
                        x.TestMethodStandard,
                        DisplayTitle = !string.IsNullOrEmpty(x.SpecDisplayTitle)
                            ? x.SpecDisplayTitle
                            : (!string.IsNullOrEmpty(x.SpecPart)
                                ? $"{x.TestMethodStandard} ({x.SpecPart}) : {x.SpecName}"
                                : $"{x.TestMethodStandard} : {x.SpecName}")
                    });

                    foreach (var specGroup in specGroups)
                    {
                        result.Add(new DropdwonSelector
                        {
                            Id = 0,
                            Name = specGroup.Key.DisplayTitle,
                            Level = 1,
                            Selectable = false,
                            NodeType = "TestMethodSpecification",
                            ParentId = orgGroup.Key.StandardOrgID,
                            IsHeader = true,
                            IsChild = true,
                            AdditionalValues = new Dictionary<string, object>
                            {
                                { "standardOrgId", orgGroup.Key.StandardOrgID },
                                { "testMethodSpecificationId", specGroup.Key.SpecHeaderID },
                                { "testMethodStandard", specGroup.Key.TestMethodStandard },
                                { "name", specGroup.Key.SpecName },
                                { "displayTitle", specGroup.Key.DisplayTitle }
                            }
                        });

                        // 3. Level 2: Version Leaf (Selectable - Active and Superseded)
                        foreach (var v in specGroup)
                        {
                            var versionLabel = v.VersionName 
                                + (!string.IsNullOrEmpty(v.VersionYear) ? $" ({v.VersionYear})" : "")
                                + (v.IsDefault ? " ★" : "")
                                + (v.VersionStatus == VersionStatus.Superseded ? " [Superseded]" : "");

                            result.Add(new DropdwonSelector
                            {
                                Id = v.VersionID,
                                Name = versionLabel,
                                Level = 2,
                                Selectable = true,
                                NodeType = "Version",
                                ParentId = specGroup.Key.SpecHeaderID,
                                IsHeader = false,
                                IsChild = true,
                                AdditionalValues = new Dictionary<string, object>
                                {
                                    { "testMethodSpecificationId", specGroup.Key.SpecHeaderID },
                                    { "testMethodSpecificationName", specGroup.Key.SpecName },
                                    { "testMethodStandard", specGroup.Key.TestMethodStandard },
                                    { "displayTitle", specGroup.Key.DisplayTitle },
                                    { "versionId", v.VersionID },
                                    { "version", v.VersionName },
                                    { "year", v.VersionYear ?? "" },
                                    { "status", v.VersionStatus.ToString() },
                                    { "isSuperseded", v.VersionStatus == VersionStatus.Superseded },
                                    { "isDefault", v.IsDefault },
                                    { "standardOrgId", orgGroup.Key.StandardOrgID },
                                    { "standardOrgName", orgGroup.Key.StandardOrgName },
                                    { "TestMethodSpecificationID", specGroup.Key.SpecHeaderID },
                                    { "TestMethodStandard", specGroup.Key.TestMethodStandard },
                                    { "Name", specGroup.Key.SpecName },
                                    { "Version", v.VersionName }
                                }
                            });
                        }
                    }
                }
            }

            return result;
        }
    }
}
