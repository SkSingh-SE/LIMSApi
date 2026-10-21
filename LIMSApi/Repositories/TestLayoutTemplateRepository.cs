using System.Linq.Dynamic.Core;
using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Repositories
{
    public class TestLayoutTemplateRepository : ITestLayoutTemplateRepository
    {
        private readonly LIMSContext _context;
        private LoggedInUserDTO loggedInUser;

        public TestLayoutTemplateRepository(LIMSContext context)
        {
            _context = context;
            loggedInUser = LoggedInUserProvider.CurrentUser;
        }

        public async Task AddLayoutTemplate(TestLayoutTemplate model)
        {
            await _context.TestLayoutTemplates.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateLayoutTemplate(TestLayoutTemplate model)
        {
            _context.TestLayoutTemplates.Update(model);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteLayoutTemplate(TestLayoutTemplate model)
        {
            _context.TestLayoutTemplates.Update(model);
            await _context.SaveChangesAsync();
        }

        public async Task<TestLayoutTemplate?> GetLayoutTemplateById(long id)
        {
            var entity = await _context.TestLayoutTemplates
                .Include(x => x.LaboratoryTest)
                .Include(x => x.SubGroup)
                .Include(x => x.AnalysisType)
                .Include(x => x.Mappings)
                .FirstOrDefaultAsync(x => x.ID == id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);

            if (entity != null)
            {
                entity.MappedTestIDs = entity.Mappings
                    .Where(m => m.IsActive)
                    .Select(m => m.LaboratoryTestID)
                    .Distinct()
                    .ToList();

                entity.MappedLeafNodes = entity.Mappings
                    .Where(m => m.IsActive)
                    .Select(m => new TestLayoutTemplateMappingItemDto
                    {
                        LaboratoryTestID = m.LaboratoryTestID,
                        LaboratoryTestSubGroupID = m.LaboratoryTestSubGroupID,
                        LaboratoryTestAnalysisTypeID = m.LaboratoryTestAnalysisTypeID,
                        LeafNode = m.LeafNode,
                        UniqueKey = m.LaboratoryTestAnalysisTypeID.HasValue
                            ? $"at_{m.LaboratoryTestAnalysisTypeID.Value}"
                            : (m.LaboratoryTestSubGroupID.HasValue ? $"sg_{m.LaboratoryTestSubGroupID.Value}" : $"lt_{m.LaboratoryTestID}")
                    })
                    .ToList();
            }

            return entity;
        }

        public async Task<TestLayoutTemplate?> GetLayoutByTestHierarchy(long? testId, long? subGroupId, long? analysisTypeId, string? leafNode, long? testMethodId = null, string? testMethodName = null)
        {
            if (testId.HasValue && testId.Value > 0)
            {
                // Strict leaf-level mapped templates
                var mappedTemplateQuery = _context.TestLayoutTemplateMappings
                    .Where(m => m.IsActive && m.CompanyCode == loggedInUser.CompanyCode);

                if (analysisTypeId.HasValue && analysisTypeId.Value > 0)
                {
                    mappedTemplateQuery = mappedTemplateQuery.Where(m => m.LaboratoryTestAnalysisTypeID == analysisTypeId.Value);
                }
                else if (subGroupId.HasValue && subGroupId.Value > 0)
                {
                    mappedTemplateQuery = mappedTemplateQuery.Where(m => m.LaboratoryTestSubGroupID == subGroupId.Value && m.LaboratoryTestAnalysisTypeID == null);
                }
                else
                {
                    mappedTemplateQuery = mappedTemplateQuery.Where(m => m.LaboratoryTestID == testId.Value && m.LaboratoryTestSubGroupID == null && m.LaboratoryTestAnalysisTypeID == null);
                }

                var mappedTemplateIds = await mappedTemplateQuery
                    .Select(m => m.TestLayoutTemplateID)
                    .ToListAsync();

                var query = _context.TestLayoutTemplates
                    .AsNoTracking()
                    .Where(x => x.IsActive && x.CompanyCode == loggedInUser.CompanyCode &&
                               (x.LaboratoryTestID == testId.Value || mappedTemplateIds.Contains(x.ID)));

                // ── TIER 1: EXACT MATCH (Leaf Node + Test Method Specification) ──
                if ((testMethodId.HasValue && testMethodId.Value > 0) || !string.IsNullOrWhiteSpace(testMethodName))
                {
                    var cleanMethodName = testMethodName?.Trim().ToLowerInvariant();
                    var methodExactMatch = await query
                        .Where(x => (testMethodId.HasValue && x.TestMethodSpecificationID == testMethodId.Value) ||
                                    (!string.IsNullOrEmpty(cleanMethodName) && x.TestMethodName != null && x.TestMethodName.Trim().ToLower() == cleanMethodName))
                        .OrderByDescending(x => x.IsDefault)
                        .ThenByDescending(x => x.ID)
                        .FirstOrDefaultAsync();

                    if (methodExactMatch != null) return methodExactMatch;
                }

                // ── TIER 2: MATCH BY LEAF NODE (Generic across methods) ──
                // Level 1: Strict match on AnalysisType (Chemical / 3-Level Leaf node)
                if (analysisTypeId.HasValue && analysisTypeId.Value > 0)
                {
                    var match = await query
                        .Where(x => x.LaboratoryTestAnalysisTypeID == analysisTypeId.Value || mappedTemplateIds.Contains(x.ID))
                        .OrderByDescending(x => x.IsDefault)
                        .ThenByDescending(x => x.ID)
                        .FirstOrDefaultAsync();

                    if (match != null) return match;
                }

                // Level 2: Match on SubGroup (General / 2-Level Leaf node)
                if (subGroupId.HasValue && subGroupId.Value > 0)
                {
                    var match = await query
                        .Where(x => x.LaboratoryTestSubGroupID == subGroupId.Value || mappedTemplateIds.Contains(x.ID))
                        .OrderByDescending(x => x.IsDefault)
                        .ThenByDescending(x => x.ID)
                        .FirstOrDefaultAsync();

                    if (match != null) return match;
                }

                // Level 3: Match on LeafNode string path
                if (!string.IsNullOrWhiteSpace(leafNode))
                {
                    var trimmedLeaf = leafNode.Trim();
                    var match = await query
                        .Where(x => (x.ApplyToLeaf && x.LeafNode != null && x.LeafNode.Trim() == trimmedLeaf) || mappedTemplateIds.Contains(x.ID))
                        .OrderByDescending(x => x.IsDefault)
                        .ThenByDescending(x => x.ID)
                        .FirstOrDefaultAsync();

                    if (match != null) return match;
                }

                // Level 4: Default layout for this Test
                var defaultMatch = await query
                    .Where(x => x.IsDefault)
                    .OrderByDescending(x => x.ID)
                    .FirstOrDefaultAsync();

                if (defaultMatch != null) return defaultMatch;

                // Level 5: Most recent layout for this Test
                var recentMatch = await query
                    .OrderByDescending(x => x.ID)
                    .FirstOrDefaultAsync();

                if (recentMatch != null) return recentMatch;
            }

            // ── TIER 3: GLOBAL DEFAULT FALLBACK ──
            var globalDefault = await _context.TestLayoutTemplates
                .AsNoTracking()
                .Where(x => x.IsActive && x.CompanyCode == loggedInUser.CompanyCode
                         && (x.IsGlobalDefault || x.LaboratoryTestID == null || x.IsDefault))
                .OrderByDescending(x => x.IsGlobalDefault)
                .ThenByDescending(x => x.IsDefault)
                .ThenByDescending(x => x.ID)
                .FirstOrDefaultAsync();

            return globalDefault;
        }

        public async Task<PagedResponse<object>> GetAllLayoutTemplates(PageFilter filter)
        {
            var query = (from t in _context.TestLayoutTemplates
                         where t.IsActive && t.CompanyCode == loggedInUser.CompanyCode
                         select new
                         {
                             t.ID,
                             t.LayoutCode,
                             t.LayoutName,
                             t.Revision,
                             t.Version,
                             t.Status,
                             t.IsDefault,
                             t.IsGlobalDefault,
                             t.LaboratoryTestID,
                             LaboratoryTestName = t.LaboratoryTest != null ? t.LaboratoryTest.Name : (t.IsGlobalDefault ? "Global Default (All Tests)" : "Unassigned"),
                             MappedTestCount = t.Mappings.Count(m => m.IsActive),
                              IsChemicalTest = t.LaboratoryTest != null && t.LaboratoryTest.IsChemicalTest,
                              DepartmentName = (t.LaboratoryTest != null && t.LaboratoryTest.IsChemicalTest) ? "Chemical" : "General",
                             t.LaboratoryTestSubGroupID,
                             SubGroupName = t.SubGroup != null ? t.SubGroup.Name : "",
                             t.LaboratoryTestAnalysisTypeID,
                             AnalysisTypeName = t.AnalysisType != null ? t.AnalysisType.Name : "",
                             t.LeafNode,
                             t.ApplyToLeaf,
                             t.SectionCount,
                             t.ArchetypeSummary,
                             t.CreatedOn,
                             t.ModifiedOn
                         }).AsQueryable().ApplyFilters(filter.Filter);

            if (!string.IsNullOrWhiteSpace(filter.searchTerm))
            {
                var search = filter.searchTerm.Trim();
                query = query.Where(x =>
                    (x.LayoutCode != null && x.LayoutCode.Contains(search))
                    || (x.LayoutName != null && x.LayoutName.Contains(search))
                    || (x.LaboratoryTestName != null && x.LaboratoryTestName.Contains(search))
                    || (x.Revision != null && x.Revision.Contains(search))
                    || (x.ArchetypeSummary != null && x.ArchetypeSummary.Contains(search))
                    || (x.LeafNode != null && x.LeafNode.Contains(search))
                );
            }

            var totalCount = await query.CountAsync();

            if (!string.IsNullOrWhiteSpace(filter.SortByColumn) && !string.IsNullOrWhiteSpace(filter.SortOrder))
            {
                query = query.OrderBy($"{filter.SortByColumn} {filter.SortOrder}");
            }
            else
            {
                query = query.OrderByDescending(x => x.ID);
            }

            var pagedData = await query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync<object>();

            return new PagedResponse<object>(pagedData, totalCount, filter.PageNumber, filter.PageSize);
        }

        public async Task<List<DropdwonSelector>> GetLayoutDropdown(long? testId, string? searchTerm, int pageNo, int pageSize)
        {
            var query = _context.TestLayoutTemplates
                .Where(x => x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);

            if (testId.HasValue && testId.Value > 0)
            {
                query = query.Where(x => x.LaboratoryTestID == testId.Value || x.IsGlobalDefault);
            }

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.Trim();
                query = query.Where(x => x.LayoutName.Contains(term) || x.LayoutCode.Contains(term));
            }

            return await query
                .OrderBy(x => x.LayoutName)
                .Skip((pageNo - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new DropdwonSelector
                {
                    Id = x.ID,
                    Name = $"{x.LayoutName} (Rev {x.Revision}){(x.IsDefault ? " [Default]" : "")}"
                })
                .ToListAsync();
        }

        public async Task ResetDefaultLayouts(long? testId, long? analysisTypeId, long currentId)
        {
            var query = _context.TestLayoutTemplates
                .Where(x => x.IsActive && x.CompanyCode == loggedInUser.CompanyCode
                         && x.ID != currentId && x.IsDefault);

            if (testId.HasValue && testId.Value > 0)
            {
                query = query.Where(x => x.LaboratoryTestID == testId.Value);
                if (analysisTypeId.HasValue && analysisTypeId.Value > 0)
                {
                    query = query.Where(x => x.LaboratoryTestAnalysisTypeID == analysisTypeId.Value);
                }
            }
            else
            {
                query = query.Where(x => x.LaboratoryTestID == null || x.IsGlobalDefault);
            }

            var existingDefaults = await query.ToListAsync();
            foreach (var item in existingDefaults)
            {
                item.IsDefault = false;
                item.ModifiedOn = DateTime.UtcNow;
                item.ModifiedBy = loggedInUser.EmployeeID;
            }

            if (existingDefaults.Count > 0)
            {
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<long>> GetMappedTestIds(long templateId)
        {
            return await _context.TestLayoutTemplateMappings
                .Where(m => m.IsActive && m.CompanyCode == loggedInUser.CompanyCode && m.TestLayoutTemplateID == templateId)
                .Select(m => m.LaboratoryTestID)
                .ToListAsync();
        }

        public async Task SyncMappedTests(long templateId, List<long> testIds)
        {
            var existing = await _context.TestLayoutTemplateMappings
                .Where(m => m.TestLayoutTemplateID == templateId && m.CompanyCode == loggedInUser.CompanyCode)
                .ToListAsync();

            var toKeep = new HashSet<long>(testIds ?? new List<long>());

            foreach (var map in existing)
            {
                if (!toKeep.Contains(map.LaboratoryTestID))
                {
                    map.IsActive = false;
                    map.ModifiedOn = DateTime.UtcNow;
                    map.ModifiedBy = loggedInUser.EmployeeID;
                }
                else
                {
                    map.IsActive = true;
                    toKeep.Remove(map.LaboratoryTestID);
                }
            }

            foreach (var newTestId in toKeep)
            {
                if (newTestId > 0)
                {
                    await _context.TestLayoutTemplateMappings.AddAsync(new TestLayoutTemplateMapping
                    {
                        TestLayoutTemplateID = templateId,
                        LaboratoryTestID = newTestId,
                        IsDefault = true,
                        IsActive = true,
                        CompanyCode = loggedInUser.CompanyCode,
                        CreatedBy = loggedInUser.EmployeeID,
                        CreatedOn = DateTime.UtcNow
                    });
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task<List<TestLayoutTemplateMappingItemDto>> GetMappedLeafNodes(long templateId)
        {
            return await _context.TestLayoutTemplateMappings
                .Where(m => m.IsActive && m.CompanyCode == loggedInUser.CompanyCode && m.TestLayoutTemplateID == templateId)
                .Select(m => new TestLayoutTemplateMappingItemDto
                {
                    LaboratoryTestID = m.LaboratoryTestID,
                    LaboratoryTestSubGroupID = m.LaboratoryTestSubGroupID,
                    LaboratoryTestAnalysisTypeID = m.LaboratoryTestAnalysisTypeID,
                    LeafNode = m.LeafNode,
                    UniqueKey = m.LaboratoryTestAnalysisTypeID.HasValue
                        ? $"at_{m.LaboratoryTestAnalysisTypeID.Value}"
                        : (m.LaboratoryTestSubGroupID.HasValue ? $"sg_{m.LaboratoryTestSubGroupID.Value}" : $"lt_{m.LaboratoryTestID}")
                })
                .ToListAsync();
        }

        public async Task SyncMappedLeafNodes(long templateId, List<TestLayoutTemplateMappingItemDto> leafNodes)
        {
            var existing = await _context.TestLayoutTemplateMappings
                .Where(m => m.TestLayoutTemplateID == templateId && m.CompanyCode == loggedInUser.CompanyCode)
                .ToListAsync();

            var incomingList = leafNodes ?? new List<TestLayoutTemplateMappingItemDto>();

            string GetLeafKey(long testId, long? sgId, long? atId) => $"{testId}_{sgId ?? 0}_{atId ?? 0}";

            var incomingKeys = new HashSet<string>(incomingList.Select(l => GetLeafKey(l.LaboratoryTestID, l.LaboratoryTestSubGroupID, l.LaboratoryTestAnalysisTypeID)));

            foreach (var map in existing)
            {
                var key = GetLeafKey(map.LaboratoryTestID, map.LaboratoryTestSubGroupID, map.LaboratoryTestAnalysisTypeID);
                if (!incomingKeys.Contains(key))
                {
                    map.IsActive = false;
                    map.ModifiedOn = DateTime.UtcNow;
                    map.ModifiedBy = loggedInUser.EmployeeID;
                }
                else
                {
                    map.IsActive = true;
                    incomingKeys.Remove(key);
                }
            }

            foreach (var item in incomingList)
            {
                var key = GetLeafKey(item.LaboratoryTestID, item.LaboratoryTestSubGroupID, item.LaboratoryTestAnalysisTypeID);
                if (incomingKeys.Contains(key))
                {
                    await _context.TestLayoutTemplateMappings.AddAsync(new TestLayoutTemplateMapping
                    {
                        TestLayoutTemplateID = templateId,
                        LaboratoryTestID = item.LaboratoryTestID,
                        LaboratoryTestSubGroupID = item.LaboratoryTestSubGroupID,
                        LaboratoryTestAnalysisTypeID = item.LaboratoryTestAnalysisTypeID,
                        LeafNode = item.LeafNode,
                        IsDefault = true,
                        IsActive = true,
                        CompanyCode = loggedInUser.CompanyCode,
                        CreatedBy = loggedInUser.EmployeeID,
                        CreatedOn = DateTime.UtcNow
                    });
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}
