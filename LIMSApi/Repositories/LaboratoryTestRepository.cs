using System.Linq.Dynamic.Core;
using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Repositories
{
    public class LaboratoryTestRepository : ILaboratoryTestRepository
    {
        private readonly LIMSContext _context;
        private LoggedInUserDTO loggedInUser;

        public LaboratoryTestRepository(LIMSContext context)
        {
            _context = context;
            loggedInUser = LoggedInUserProvider.CurrentUser;
        }

        public async Task AddTestMethod(LaboratoryTest model)
        {
            await _context.LaboratoryTests.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteTestMethod(long id)
        {
            var existingTestMethod = await _context.LaboratoryTests.FirstOrDefaultAsync(x => x.ID == id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);
            if (existingTestMethod != null)
            {
                existingTestMethod.IsActive = false;
                existingTestMethod.ModifiedOn = DateTime.UtcNow;
                _context.LaboratoryTests.Update(existingTestMethod);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<LaboratoryTest?> GetTestMethodById(long id)
        {
            var test = await _context.LaboratoryTests
                .Include(y => y.SubGroups)
                    .ThenInclude(g => g.MetalClassification)
                .Include(y => y.SubGroups)
                    .ThenInclude(g => g.AnalysisTypes)
                        .ThenInclude(s => s.MetalClassification)
                .Include(y => y.SubGroups)
                    .ThenInclude(g => g.AnalysisTypes)
                        .ThenInclude(s => s.AllowedTechniques)
                            .ThenInclude(t => t.AnalysisTechnique)
                .FirstOrDefaultAsync(x => x.ID == id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);

            if (test != null)
            {
                test.SubGroups = test.SubGroups.Where(sg => sg.IsActive).ToList();
                foreach (var sg in test.SubGroups)
                {
                    sg.AnalysisTypes = sg.AnalysisTypes.Where(at => at.IsActive).ToList();
                }
            }

            return test;
        }

        public async Task UpdateTestMethod(LaboratoryTest model)
        {
            var tracked = _context.ChangeTracker.Entries<LaboratoryTest>()
                .Any(e => e.Entity.ID == model.ID);
            if (!tracked)
                _context.LaboratoryTests.Update(model);
            await _context.SaveChangesAsync();
        }

        public async Task<PagedResponse<object>> GetAllTestMethods(PageFilter filter)
        {
            var _query = from c in _context.LaboratoryTests
                         where c.IsActive && c.CompanyCode == loggedInUser.CompanyCode
                         join d in _context.DepartmentMasters on c.LabDepartmentID equals d.ID into dsGroup
                         from ds in dsGroup.DefaultIfEmpty()
                         select new
                         {
                             c.ID,
                             c.Name,
                             c.LabDepartmentID,
                             DepartmentName = ds.Name,
                             c.IsChemicalTest,
                             c.ModifiedOn,
                             c.CreatedOn
                         };

            _query = _query.AsQueryable().ApplyFilters(filter.Filter);

            if (!string.IsNullOrWhiteSpace(filter.searchTerm))
            {
                var search = filter.searchTerm.Trim();
                _query = _query.Where(x =>
                                    (!string.IsNullOrEmpty(x.Name) && x.Name.Contains(search)) ||
                                    (!string.IsNullOrEmpty(x.DepartmentName) && x.DepartmentName.Contains(search))
                                    );
            }

            if (filter.SortByColumn != null)
            {
                _query = _query.OrderBy($"{filter.SortByColumn} {(filter.SortOrder == "asc" ? "ascending" : "descending")}");
            }

            return await _query.Cast<object>().ToPagedAsync(filter);
        }

        public async Task<List<DropdwonSelector>> GetTestMethodDropdown(string? searchTerm, int pageNo = 0, int pageSize = 20)
        {
            if (pageNo < 0) pageNo = 0;

            // 1. Exact ID lookup
            if (!string.IsNullOrWhiteSpace(searchTerm) && FilterHelper.IsExactIdSearch(searchTerm, out long exactId))
            {
                var exactMatch = await (from a in _context.LaboratoryTests
                                        join d in _context.DepartmentMasters on a.LabDepartmentID equals d.ID into dsGroup
                                        from ds in dsGroup.DefaultIfEmpty()
                                        where a.ID == exactId && a.IsActive && a.CompanyCode == loggedInUser.CompanyCode
                                        select new DropdwonSelector
                                        {
                                            Id = a.ID,
                                            Name = a.Name,
                                            Level = 1,
                                            Selectable = true,
                                            NodeType = "TestGroup",
                                            ParentId = ds != null ? ds.ID : 0,
                                            IsHeader = false,
                                            IsChild = true,
                                            AdditionalValues = new Dictionary<string, object>
                                            {
                                                { "masterTestId", a.ID },
                                                { "masterTestName", a.Name },
                                                { "departmentId", ds != null ? ds.ID : 0 },
                                                { "departmentName", ds != null ? ds.Name : "" },
                                                { "isChemical", a.IsChemicalTest }
                                            }
                                        }).FirstOrDefaultAsync();

                if (exactMatch != null)
                {
                    return new List<DropdwonSelector> { exactMatch };
                }
            }

            // 2. 2-Tier Hierarchy Query: Department (Level 0) -> LaboratoryTest (Level 1 Leaf)
            var query = from a in _context.LaboratoryTests
                        join d in _context.DepartmentMasters on a.LabDepartmentID equals d.ID into dsGroup
                        from ds in dsGroup.DefaultIfEmpty()
                        where a.IsActive && a.CompanyCode == loggedInUser.CompanyCode
                        select new
                        {
                            MasterTestID = a.ID,
                            MasterTestName = a.Name,
                            IsChemical = a.IsChemicalTest,
                            DepartmentID = ds != null ? ds.ID : 0,
                            DepartmentName = ds != null ? ds.Name : "General Laboratory Tests"
                        };

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var search = searchTerm.Trim();
                query = query.Where(x => (x.MasterTestName != null && x.MasterTestName.Contains(search))
                                      || (x.DepartmentName != null && x.DepartmentName.Contains(search)));
            }

            var rawData = await query
                .OrderBy(x => x.DepartmentName)
                .ThenBy(x => x.MasterTestName)
                .ToListAsync();

            var result = new List<DropdwonSelector>();

            if (rawData.Count > 0)
            {
                var deptGroups = rawData.GroupBy(x => new { x.DepartmentID, x.DepartmentName });

                foreach (var deptGroup in deptGroups)
                {
                    // Level 0: Department Header (Non-selectable)
                    result.Add(new DropdwonSelector
                    {
                        Id = 0,
                        Name = deptGroup.Key.DepartmentName,
                        Level = 0,
                        Selectable = false,
                        NodeType = "Department",
                        IsHeader = true,
                        IsChild = false,
                        AdditionalValues = new Dictionary<string, object>
                        {
                            { "departmentId", deptGroup.Key.DepartmentID },
                            { "departmentName", deptGroup.Key.DepartmentName }
                        }
                    });

                    // Level 1: Laboratory Test (Selectable Leaf)
                    foreach (var test in deptGroup)
                    {
                        result.Add(new DropdwonSelector
                        {
                            Id = test.MasterTestID,
                            Name = test.MasterTestName,
                            Level = 1,
                            Selectable = true,
                            NodeType = "TestGroup",
                            ParentId = deptGroup.Key.DepartmentID,
                            IsHeader = false,
                            IsChild = true,
                            AdditionalValues = new Dictionary<string, object>
                            {
                                { "masterTestId", test.MasterTestID },
                                { "masterTestName", test.MasterTestName },
                                { "departmentId", deptGroup.Key.DepartmentID },
                                { "departmentName", deptGroup.Key.DepartmentName },
                                { "isChemical", test.IsChemical }
                            }
                        });
                    }
                }
            }

            return result;
        }

        public async Task<List<DropdwonSelector>> GetGeneralTestMethodDropdown(string? searchTerm, int pageNo = 0, int pageSize = 20)
        {
            if (pageNo < 0) pageNo = 0;

            // Handle Exact ID lookup (e.g. initial form binding by SubGroupID OR fallback by Master LaboratoryTestID)
            if (!string.IsNullOrWhiteSpace(searchTerm) && FilterHelper.IsExactIdSearch(searchTerm, out long exactId))
            {
                var exactMatch = await (from sg in _context.LaboratoryTestSubGroups
                                        join a in _context.LaboratoryTests on sg.LaboratoryTestID equals a.ID
                                        join d in _context.DepartmentMasters on a.LabDepartmentID equals d.ID
                                        where (sg.ID == exactId || a.ID == exactId) && sg.IsActive && a.IsActive && a.CompanyCode == loggedInUser.CompanyCode
                                        orderby (sg.ID == exactId ? 0 : 1), sg.DisplayOrder, sg.ID
                                        select new DropdwonSelector
                                        {
                                            Id = sg.ID,
                                            Name = sg.Name,
                                            Level = 1,
                                            Selectable = true,
                                            NodeType = "SubGroup",
                                            ParentId = a.ID,
                                            IsHeader = false,
                                            IsChild = true,
                                            AdditionalValues = new Dictionary<string, object>
                                            {
                                                { "masterTestId", a.ID },
                                                { "masterTestName", a.Name },
                                                { "reportTestName", sg.ReportTestName },
                                                { "testDuration", sg.TestDuration ?? 1 },
                                                { "metalClassificationId", sg.MetalClassificationID ?? 0 },
                                                { "isChemical", false }
                                            }
                                        }).FirstOrDefaultAsync();

                if (exactMatch != null)
                {
                    return new List<DropdwonSelector> { exactMatch };
                }
            }

            var query = from sg in _context.LaboratoryTestSubGroups
                        join a in _context.LaboratoryTests on sg.LaboratoryTestID equals a.ID
                        join d in _context.DepartmentMasters on a.LabDepartmentID equals d.ID
                        where sg.IsActive && a.IsActive && a.CompanyCode == loggedInUser.CompanyCode
                              && !d.IsChemical
                        select new
                        {
                            SubGroupID = sg.ID,
                            SubGroupName = sg.Name,
                            ReportTestName = sg.ReportTestName,
                            TestDuration = sg.TestDuration,
                            MetalClassificationID = sg.MetalClassificationID,
                            DisplayOrder = sg.DisplayOrder,
                            MasterTestID = a.ID,
                            MasterTestName = a.Name
                        };

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var search = searchTerm.Trim();
                query = query.Where(x => (x.SubGroupName != null && x.SubGroupName.Contains(search))
                                      || (x.MasterTestName != null && x.MasterTestName.Contains(search))
                                      || (x.ReportTestName != null && x.ReportTestName.Contains(search)));
            }

            var rawData = await query
                .OrderBy(x => x.MasterTestName)
                .ThenBy(x => x.DisplayOrder)
                .ThenBy(x => x.SubGroupName)
                .ToListAsync();

            var result = new List<DropdwonSelector>();

            if (rawData.Count > 0)
            {
                // Group by Master Laboratory Test
                var grouped = rawData.GroupBy(x => new { x.MasterTestID, x.MasterTestName });

                foreach (var masterGroup in grouped)
                {
                    // 1. Level 0: Master Test Group Header (Non-selectable)
                    result.Add(new DropdwonSelector
                    {
                        Id = 0,
                        Name = masterGroup.Key.MasterTestName,
                        Level = 0,
                        Selectable = false,
                        NodeType = "TestGroup",
                        IsHeader = true,
                        IsChild = false,
                        AdditionalValues = new Dictionary<string, object>
                        {
                            { "masterTestId", masterGroup.Key.MasterTestID },
                            { "isChemical", false }
                        }
                    });

                    // 2. Level 1: Child SubGroups (Selectable Leaf)
                    foreach (var sg in masterGroup)
                    {
                        result.Add(new DropdwonSelector
                        {
                            Id = sg.SubGroupID,
                            Name = sg.SubGroupName,
                            Level = 1,
                            Selectable = true,
                            NodeType = "SubGroup",
                            ParentId = masterGroup.Key.MasterTestID,
                            IsHeader = false,
                            IsChild = true,
                            AdditionalValues = new Dictionary<string, object>
                            {
                                { "masterTestId", masterGroup.Key.MasterTestID },
                                { "masterTestName", masterGroup.Key.MasterTestName },
                                { "reportTestName", sg.ReportTestName },
                                { "testDuration", sg.TestDuration ?? 1 },
                                { "metalClassificationId", sg.MetalClassificationID ?? 0 },
                                { "isChemical", false }
                            }
                        });
                    }
                }
            }
            else
            {
                // Fallback: If no SubGroups exist yet, return active LaboratoryTests directly
                var fallbackQuery = from a in _context.LaboratoryTests
                                    join d in _context.DepartmentMasters on a.LabDepartmentID equals d.ID
                                    where a.IsActive && a.CompanyCode == loggedInUser.CompanyCode && !d.IsChemical
                                    select new { a.ID, a.Name, Department = d.Name };

                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    fallbackQuery = fallbackQuery.Where(x => x.Name.Contains(searchTerm.Trim()));
                }

                var fallbackData = await fallbackQuery.OrderBy(x => x.Name).ToListAsync();
                foreach (var fb in fallbackData)
                {
                    result.Add(new DropdwonSelector
                    {
                        Id = fb.ID,
                        Name = fb.Name,
                        Level = 1,
                        Selectable = true,
                        NodeType = "TestGroup",
                        IsHeader = false,
                        IsChild = false,
                        AdditionalValues = new Dictionary<string, object>
                        {
                            { "masterTestId", fb.ID },
                            { "masterTestName", fb.Name },
                            { "isChemical", false }
                        }
                    });
                }
            }

            return result;
        }

        public async Task<List<DropdwonSelector>> GetChemicalTestMethodDropdown(string? searchTerm, int pageNo = 0, int pageSize = 20)
        {
            if (pageNo < 0) pageNo = 0;

            // Handle Exact ID lookup (e.g. initial form binding by AnalysisTypeID OR fallback by SubGroupID OR Master LaboratoryTestID)
            if (!string.IsNullOrWhiteSpace(searchTerm) && FilterHelper.IsExactIdSearch(searchTerm, out long exactId))
            {
                var exactMatch = await (from at in _context.LaboratoryTestAnalysisTypes
                                        join sg in _context.LaboratoryTestSubGroups on at.LaboratoryTestSubGroupID equals sg.ID
                                        join a in _context.LaboratoryTests on sg.LaboratoryTestID equals a.ID
                                        where (at.ID == exactId || sg.ID == exactId || a.ID == exactId) && at.IsActive && sg.IsActive && a.IsActive && a.CompanyCode == loggedInUser.CompanyCode
                                        orderby (at.ID == exactId ? 0 : (sg.ID == exactId ? 1 : 2)), at.ID
                                        select new DropdwonSelector
                                        {
                                            Id = at.ID,
                                            Name = at.Name,
                                            Level = 2,
                                            Selectable = true,
                                            NodeType = "AnalysisType",
                                            ParentId = sg.ID,
                                            IsHeader = false,
                                            IsChild = true,
                                            AdditionalValues = new Dictionary<string, object>
                                            {
                                                { "masterTestId", a.ID },
                                                { "masterTestName", a.Name },
                                                { "subGroupId", sg.ID },
                                                { "subGroupName", sg.Name },
                                                { "analysisTypeId", at.ID },
                                                { "reportTestName", sg.ReportTestName },
                                                { "metalClassificationId", at.MetalClassificationID ?? 0 },
                                                { "isChemical", true },
                                                { "techniqueIds", at.AllowedTechniques.Select(t => t.AnalysisTechniqueID).ToList() },
                                                { "techniqueCodes", at.AllowedTechniques.Select(t => t.AnalysisTechnique != null ? t.AnalysisTechnique.Code ?? "" : "").Where(c => c != "").ToList() },
                                                { "techniqueNames", at.AllowedTechniques.Select(t => t.AnalysisTechnique != null ? t.AnalysisTechnique.Name ?? "" : "").Where(n => n != "").ToList() }
                                            }
                                        }).FirstOrDefaultAsync();

                if (exactMatch != null)
                {
                    return new List<DropdwonSelector> { exactMatch };
                }
            }

            // 3-Tier Hierarchy Query: LaboratoryTest (Master) -> LaboratoryTestSubGroup -> LaboratoryTestAnalysisType (Leaf)
            var query = from at in _context.LaboratoryTestAnalysisTypes
                        join sg in _context.LaboratoryTestSubGroups on at.LaboratoryTestSubGroupID equals sg.ID
                        join a in _context.LaboratoryTests on sg.LaboratoryTestID equals a.ID
                        join d in _context.DepartmentMasters on a.LabDepartmentID equals d.ID
                        where at.IsActive && sg.IsActive && a.IsActive && a.CompanyCode == loggedInUser.CompanyCode
                              && d.IsChemical
                        select new
                        {
                            AnalysisTypeID = at.ID,
                            AnalysisTypeName = at.Name,
                            SubGroupID = sg.ID,
                            SubGroupName = sg.Name,
                            ReportTestName = sg.ReportTestName,
                            SubGroupDisplayOrder = sg.DisplayOrder,
                            MetalClassificationID = at.MetalClassificationID,
                            MasterTestID = a.ID,
                            MasterTestName = a.Name,
                            TechniqueIDs = at.AllowedTechniques.Select(t => t.AnalysisTechniqueID).ToList(),
                            TechniqueCodes = at.AllowedTechniques.Select(t => t.AnalysisTechnique != null ? t.AnalysisTechnique.Code ?? "" : "").Where(c => c != "").ToList(),
                            TechniqueNames = at.AllowedTechniques.Select(t => t.AnalysisTechnique != null ? t.AnalysisTechnique.Name ?? "" : "").Where(n => n != "").ToList()
                        };

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var search = searchTerm.Trim();
                query = query.Where(x => (x.AnalysisTypeName != null && x.AnalysisTypeName.Contains(search))
                                      || (x.SubGroupName != null && x.SubGroupName.Contains(search))
                                      || (x.MasterTestName != null && x.MasterTestName.Contains(search))
                                      || (x.ReportTestName != null && x.ReportTestName.Contains(search)));
            }

            var rawData = await query
                .OrderBy(x => x.MasterTestName)
                .ThenBy(x => x.SubGroupDisplayOrder)
                .ThenBy(x => x.SubGroupName)
                .ThenBy(x => x.AnalysisTypeName)
                .ToListAsync();

            var result = new List<DropdwonSelector>();

            if (rawData.Count > 0)
            {
                // Group by Master Laboratory Test (Level 0)
                var masterGroups = rawData.GroupBy(x => new { x.MasterTestID, x.MasterTestName });

                foreach (var masterGroup in masterGroups)
                {
                    // 1. Level 0: Master Test Group Header (Non-selectable)
                    result.Add(new DropdwonSelector
                    {
                        Id = 0,
                        Name = masterGroup.Key.MasterTestName,
                        Level = 0,
                        Selectable = false,
                        NodeType = "TestGroup",
                        IsHeader = true,
                        IsChild = false,
                        AdditionalValues = new Dictionary<string, object>
                        {
                            { "masterTestId", masterGroup.Key.MasterTestID },
                            { "isChemical", true }
                        }
                    });

                    // 2. Level 1: SubGroup Intermediate Header (Non-selectable)
                    var subGroupGroups = masterGroup.GroupBy(x => new { x.SubGroupID, x.SubGroupName, x.ReportTestName });

                    foreach (var sgGroup in subGroupGroups)
                    {
                        result.Add(new DropdwonSelector
                        {
                            Id = 0,
                            Name = sgGroup.Key.SubGroupName,
                            Level = 1,
                            Selectable = false,
                            NodeType = "SubGroup",
                            ParentId = masterGroup.Key.MasterTestID,
                            IsHeader = true,
                            IsChild = true,
                            AdditionalValues = new Dictionary<string, object>
                            {
                                { "masterTestId", masterGroup.Key.MasterTestID },
                                { "subGroupId", sgGroup.Key.SubGroupID },
                                { "reportTestName", sgGroup.Key.ReportTestName },
                                { "isChemical", true }
                            }
                        });

                        // 3. Level 2: Analysis Type (Selectable Leaf)
                        foreach (var at in sgGroup)
                        {
                            result.Add(new DropdwonSelector
                            {
                                Id = at.AnalysisTypeID,
                                Name = at.AnalysisTypeName,
                                Level = 2,
                                Selectable = true,
                                NodeType = "AnalysisType",
                                ParentId = sgGroup.Key.SubGroupID,
                                IsHeader = false,
                                IsChild = true,
                                AdditionalValues = new Dictionary<string, object>
                                {
                                    { "masterTestId", masterGroup.Key.MasterTestID },
                                    { "masterTestName", masterGroup.Key.MasterTestName },
                                    { "subGroupId", sgGroup.Key.SubGroupID },
                                    { "subGroupName", sgGroup.Key.SubGroupName },
                                    { "analysisTypeId", at.AnalysisTypeID },
                                    { "reportTestName", sgGroup.Key.ReportTestName },
                                    { "metalClassificationId", at.MetalClassificationID ?? 0 },
                                    { "isChemical", true },
                                    { "techniqueIds", at.TechniqueIDs },
                                    { "techniqueCodes", at.TechniqueCodes },
                                    { "techniqueNames", at.TechniqueNames }
                                }
                            });
                        }
                    }
                }
            }
            else
            {
                // Fallback: If no AnalysisTypes exist yet, return active Chemical LaboratoryTests directly
                var fallbackQuery = from a in _context.LaboratoryTests
                                    join d in _context.DepartmentMasters on a.LabDepartmentID equals d.ID
                                    where a.IsActive && a.CompanyCode == loggedInUser.CompanyCode && d.IsChemical
                                    select new { a.ID, a.Name, Department = d.Name };

                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    fallbackQuery = fallbackQuery.Where(x => x.Name.Contains(searchTerm.Trim()));
                }

                var fallbackData = await fallbackQuery.OrderBy(x => x.Name).ToListAsync();
                foreach (var fb in fallbackData)
                {
                    result.Add(new DropdwonSelector
                    {
                        Id = fb.ID,
                        Name = fb.Name,
                        Level = 1,
                        Selectable = true,
                        NodeType = "TestGroup",
                        IsHeader = false,
                        IsChild = false,
                        AdditionalValues = new Dictionary<string, object>
                        {
                            { "masterTestId", fb.ID },
                            { "masterTestName", fb.Name },
                            { "isChemical", true }
                        }
                    });
                }
            }

            return result;
        }

        public async Task<bool> ExistsByName(string name)
        {
            return await _context.LaboratoryTests.AnyAsync(x => x.Name == name && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);
        }

        public async Task<bool> ExistsByNameAndNotId(string name, long Id)
        {
            return await _context.LaboratoryTests.AnyAsync(x => x.Name == name && x.ID != Id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);
        }

        public async Task<List<string>> GetDistinctTestNames(string? searchTerm, int pageSize = 20)
        {
            var query = _context.LaboratoryTests
                .Where(x => x.IsActive && x.CompanyCode == loggedInUser.CompanyCode)
                .Select(x => x.Name)
                .Distinct();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var search = searchTerm.Trim();
                query = query.Where(n => n.Contains(search));
            }

            return await query.OrderBy(n => n).Take(pageSize).ToListAsync();
        }

        public async Task<List<object>> GetTestCases(long testMethodId)
        {
            var subgroupCases = from l in _context.LaboratoryTests
                                 join sg in _context.LaboratoryTestSubGroups on l.ID equals sg.LaboratoryTestID
                                 join ic in _context.LaboratoryTestSubGroupInvoiceCases on sg.ID equals ic.LaboratoryTestSubGroupID
                                 join c in _context.InvoiceCaseConfigurations on ic.InvoiceCaseConfigID equals c.ID
                                 where l.ID == testMethodId && c.IsActive && sg.IsActive
                                 select new
                                 {
                                     c.ID,
                                     c.SelectionType,
                                     c.Name,
                                     c.Value,
                                     c.Unit
                                 };

            var analysistypeCases = from l in _context.LaboratoryTests
                                     join sg in _context.LaboratoryTestSubGroups on l.ID equals sg.LaboratoryTestID
                                     join at in _context.LaboratoryTestAnalysisTypes on sg.ID equals at.LaboratoryTestSubGroupID
                                     join ic in _context.LaboratoryTestAnalysisTypeInvoiceCases on at.ID equals ic.LaboratoryTestAnalysisTypeID
                                     join c in _context.InvoiceCaseConfigurations on ic.InvoiceCaseConfigID equals c.ID
                                     where l.ID == testMethodId && c.IsActive && sg.IsActive && at.IsActive
                                     select new
                                     {
                                         c.ID,
                                         c.SelectionType,
                                         c.Name,
                                         c.Value,
                                         c.Unit
                                     };

            var query = subgroupCases.Union(analysistypeCases).Distinct();

            var data = await query.ToListAsync();
            var result = data.Cast<object>().ToList();
            return result;
        }

        public async Task<List<PricingTemplateRowDto>> GetPricingTemplate(long labTestId, long? analysisTypeId)
        {
            List<PricingTemplateRowDto> rows = new();

            if (analysisTypeId.HasValue)
            {
                // Chemical test -> Fetch configurations for this specific AnalysisType
                rows = await (
                    from sg in _context.LaboratoryTestSubGroups
                    join at in _context.LaboratoryTestAnalysisTypes on sg.ID equals at.LaboratoryTestSubGroupID
                    join ic in _context.LaboratoryTestAnalysisTypeInvoiceCases on at.ID equals ic.LaboratoryTestAnalysisTypeID
                    join c in _context.InvoiceCaseConfigurations on ic.InvoiceCaseConfigID equals c.ID
                    where sg.LaboratoryTestID == labTestId && sg.IsActive && at.IsActive && c.IsActive && at.ID == analysisTypeId.Value
                    select new PricingTemplateRowDto
                    {
                        InvoiceCaseConfigID  = c.ID,
                        ConfigName           = c.Name,
                        SelectionType        = c.SelectionType,
                        ConfigValue          = c.Value,
                        GroupName            = at.Name,
                        GroupType            = "AnalysisType",
                        IsOverride           = c.Value == "OVERRIDE",
                        OverrideParameterIDs = c.OverrideParameterIDs
                    }
                ).ToListAsync();
            }
            else
            {
                // Non-chemical test -> Fetch configurations for SubGroups
                rows = await (
                    from sg in _context.LaboratoryTestSubGroups
                    join ic in _context.LaboratoryTestSubGroupInvoiceCases on sg.ID equals ic.LaboratoryTestSubGroupID
                    join c in _context.InvoiceCaseConfigurations on ic.InvoiceCaseConfigID equals c.ID
                    where sg.LaboratoryTestID == labTestId && sg.IsActive && c.IsActive
                    select new PricingTemplateRowDto
                    {
                        InvoiceCaseConfigID  = c.ID,
                        ConfigName           = c.Name,
                        SelectionType        = c.SelectionType,
                        ConfigValue          = c.Value,
                        GroupName            = sg.Name,
                        GroupType            = "SubGroup",
                        IsOverride           = c.Value == "OVERRIDE",
                        OverrideParameterIDs = c.OverrideParameterIDs
                    }
                ).ToListAsync();
            }

            var mergedRows = rows
                .GroupBy(r => r.InvoiceCaseConfigID)
                .Select(g => g.First())
                .ToList();

            var uniqueParamIds = mergedRows
                .Where(r => !string.IsNullOrEmpty(r.OverrideParameterIDs))
                .SelectMany(r => r.OverrideParameterIDs!.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(idStr => long.TryParse(idStr.Trim(), out var id) ? id : 0))
                .Where(id => id > 0)
                .Distinct()
                .ToList();

            if (uniqueParamIds.Any())
            {
                var paramNamesDict = await _context.ParameterMasters
                    .Where(p => uniqueParamIds.Contains(p.ID))
                    .ToDictionaryAsync(p => p.ID, p => p.Name);

                foreach (var row in mergedRows)
                {
                    if (!string.IsNullOrEmpty(row.OverrideParameterIDs))
                    {
                        var ids = row.OverrideParameterIDs.Split(',', StringSplitOptions.RemoveEmptyEntries)
                            .Select(idStr => long.TryParse(idStr.Trim(), out var id) ? id : 0)
                            .Where(id => id > 0);
                        var names = ids.Select(id => paramNamesDict.TryGetValue(id, out var name) ? name : id.ToString());
                        row.OverrideParameterNames = string.Join(", ", names);
                    }
                }
            }

            return mergedRows
                .OrderBy(r => r.IsOverride)
                .ThenBy(r => r.GroupName)
                .ThenBy(r => r.ConfigName)
                .ToList();
        }

        public async Task<List<DropdwonSelector>> GetUnifiedTestMethodDropdown(string? searchTerm, int pageNo = 0, int pageSize = 20)
        {
            if (pageNo < 0) pageNo = 0;

            // 1. Exact ID lookup (matches AnalysisType ID, SubGroup ID, or LaboratoryTest ID)
            if (!string.IsNullOrWhiteSpace(searchTerm) && FilterHelper.IsExactIdSearch(searchTerm, out long exactId))
            {
                // Check AnalysisType first
                var exactAt = await (from at in _context.LaboratoryTestAnalysisTypes
                                     join sg in _context.LaboratoryTestSubGroups on at.LaboratoryTestSubGroupID equals sg.ID
                                     join a in _context.LaboratoryTests on sg.LaboratoryTestID equals a.ID
                                     where (at.ID == exactId || sg.ID == exactId || a.ID == exactId) && at.IsActive && sg.IsActive && a.IsActive && a.CompanyCode == loggedInUser.CompanyCode
                                     orderby (at.ID == exactId ? 0 : (sg.ID == exactId ? 1 : 2)), at.ID
                                     select new DropdwonSelector
                                     {
                                         Id = at.ID,
                                         Name = at.Name,
                                         Level = 2,
                                         Selectable = true,
                                         NodeType = "AnalysisType",
                                         ParentId = sg.ID,
                                         IsHeader = false,
                                         IsChild = true,
                                         AdditionalValues = new Dictionary<string, object>
                                         {
                                             { "masterTestId", a.ID },
                                             { "masterTestName", a.Name },
                                             { "subGroupId", sg.ID },
                                             { "subGroupName", sg.Name },
                                             { "analysisTypeId", at.ID },
                                             { "fullDisplayName", $"{a.Name} - {sg.Name} ({at.Name})" },
                                             { "reportTestName", sg.ReportTestName ?? "" },
                                             { "isChemical", true }
                                         }
                                     }).FirstOrDefaultAsync();

                if (exactAt != null)
                {
                    return new List<DropdwonSelector> { exactAt };
                }

                // Check SubGroup
                var exactSg = await (from sg in _context.LaboratoryTestSubGroups
                                     join a in _context.LaboratoryTests on sg.LaboratoryTestID equals a.ID
                                     where (sg.ID == exactId || a.ID == exactId) && sg.IsActive && a.IsActive && a.CompanyCode == loggedInUser.CompanyCode
                                     orderby (sg.ID == exactId ? 0 : 1), sg.DisplayOrder, sg.ID
                                     select new DropdwonSelector
                                     {
                                         Id = sg.ID,
                                         Name = sg.Name,
                                         Level = 1,
                                         Selectable = true,
                                         NodeType = "SubGroup",
                                         ParentId = a.ID,
                                         IsHeader = false,
                                         IsChild = true,
                                         AdditionalValues = new Dictionary<string, object>
                                         {
                                             { "masterTestId", a.ID },
                                             { "masterTestName", a.Name },
                                             { "subGroupId", sg.ID },
                                             { "subGroupName", sg.Name },
                                             { "fullDisplayName", $"{a.Name} - {sg.Name}" },
                                             { "reportTestName", sg.ReportTestName ?? "" },
                                             { "isChemical", a.IsChemicalTest }
                                         }
                                     }).FirstOrDefaultAsync();

                if (exactSg != null)
                {
                    return new List<DropdwonSelector> { exactSg };
                }

                // Check LaboratoryTest directly
                var exactLt = await (from a in _context.LaboratoryTests
                                     where a.ID == exactId && a.IsActive && a.CompanyCode == loggedInUser.CompanyCode
                                     select new DropdwonSelector
                                     {
                                         Id = a.ID,
                                         Name = a.Name,
                                         Level = 1,
                                         Selectable = true,
                                         NodeType = "TestGroup",
                                         ParentId = 0,
                                         IsHeader = false,
                                         IsChild = false,
                                         AdditionalValues = new Dictionary<string, object>
                                         {
                                             { "masterTestId", a.ID },
                                             { "masterTestName", a.Name },
                                             { "fullDisplayName", a.Name },
                                             { "isChemical", a.IsChemicalTest }
                                         }
                                     }).FirstOrDefaultAsync();

                if (exactLt != null)
                {
                    return new List<DropdwonSelector> { exactLt };
                }
            }

            var result = new List<DropdwonSelector>();

            // 2. Fetch General / Mechanical Tests (2 Levels: Master Test Level 0 Header -> SubGroup Level 1 Selectable Leaf)
            var genQuery = from sg in _context.LaboratoryTestSubGroups
                           join a in _context.LaboratoryTests on sg.LaboratoryTestID equals a.ID
                           join d in _context.DepartmentMasters on a.LabDepartmentID equals d.ID
                           where sg.IsActive && a.IsActive && a.CompanyCode == loggedInUser.CompanyCode && !d.IsChemical
                           select new
                           {
                               SubGroupID = sg.ID,
                               SubGroupName = sg.Name,
                               ReportTestName = sg.ReportTestName,
                               MasterTestID = a.ID,
                               MasterTestName = a.Name,
                               DisplayOrder = sg.DisplayOrder
                           };

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var s = searchTerm.Trim();
                genQuery = genQuery.Where(x => (x.SubGroupName != null && x.SubGroupName.Contains(s))
                                            || (x.MasterTestName != null && x.MasterTestName.Contains(s))
                                            || (x.ReportTestName != null && x.ReportTestName.Contains(s)));
            }

            var genData = await genQuery
                .OrderBy(x => x.MasterTestName)
                .ThenBy(x => x.DisplayOrder)
                .ThenBy(x => x.SubGroupName)
                .ToListAsync();

            if (genData.Count > 0)
            {
                var genGroups = genData.GroupBy(x => new { x.MasterTestID, x.MasterTestName });
                foreach (var group in genGroups)
                {
                    // Level 0 Header (Master Test)
                    result.Add(new DropdwonSelector
                    {
                        Id = 0,
                        Name = group.Key.MasterTestName,
                        Level = 0,
                        Selectable = false,
                        NodeType = "TestGroup",
                        IsHeader = true,
                        IsChild = false,
                        AdditionalValues = new Dictionary<string, object>
                        {
                            { "masterTestId", group.Key.MasterTestID },
                            { "isChemical", false }
                        }
                    });

                    // Level 1 Leaf (SubGroup)
                    foreach (var item in group)
                    {
                        result.Add(new DropdwonSelector
                        {
                            Id = item.SubGroupID,
                            Name = item.SubGroupName,
                            Level = 1,
                            Selectable = true,
                            NodeType = "SubGroup",
                            ParentId = group.Key.MasterTestID,
                            IsHeader = false,
                            IsChild = true,
                            AdditionalValues = new Dictionary<string, object>
                            {
                                { "masterTestId", group.Key.MasterTestID },
                                { "masterTestName", group.Key.MasterTestName },
                                { "subGroupId", item.SubGroupID },
                                { "testId", item.SubGroupID },
                                { "fullDisplayName", $"{group.Key.MasterTestName} - {item.SubGroupName}" },
                                { "reportTestName", item.ReportTestName ?? "" },
                                { "isChemical", false }
                            }
                        });
                    }
                }
            }

            // 3. Fetch Chemical Tests (3 Levels: Master Test Level 0 Header -> SubGroup Level 1 Header -> Analysis Type Level 2 Leaf)
            var chemQuery = from at in _context.LaboratoryTestAnalysisTypes
                            join sg in _context.LaboratoryTestSubGroups on at.LaboratoryTestSubGroupID equals sg.ID
                            join a in _context.LaboratoryTests on sg.LaboratoryTestID equals a.ID
                            join d in _context.DepartmentMasters on a.LabDepartmentID equals d.ID
                            where at.IsActive && sg.IsActive && a.IsActive && a.CompanyCode == loggedInUser.CompanyCode && d.IsChemical
                            select new
                            {
                                AnalysisTypeID = at.ID,
                                AnalysisTypeName = at.Name,
                                SubGroupID = sg.ID,
                                SubGroupName = sg.Name,
                                ReportTestName = sg.ReportTestName,
                                MasterTestID = a.ID,
                                MasterTestName = a.Name,
                                DisplayOrder = sg.DisplayOrder
                            };

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var s = searchTerm.Trim();
                chemQuery = chemQuery.Where(x => (x.AnalysisTypeName != null && x.AnalysisTypeName.Contains(s))
                                              || (x.SubGroupName != null && x.SubGroupName.Contains(s))
                                              || (x.MasterTestName != null && x.MasterTestName.Contains(s))
                                              || (x.ReportTestName != null && x.ReportTestName.Contains(s)));
            }

            var chemData = await chemQuery
                .OrderBy(x => x.MasterTestName)
                .ThenBy(x => x.DisplayOrder)
                .ThenBy(x => x.SubGroupName)
                .ThenBy(x => x.AnalysisTypeName)
                .ToListAsync();

            if (chemData.Count > 0)
            {
                var chemMasterGroups = chemData.GroupBy(x => new { x.MasterTestID, x.MasterTestName });
                foreach (var masterGroup in chemMasterGroups)
                {
                    // Level 0 Header (Master Test)
                    result.Add(new DropdwonSelector
                    {
                        Id = 0,
                        Name = masterGroup.Key.MasterTestName,
                        Level = 0,
                        Selectable = false,
                        NodeType = "TestGroup",
                        IsHeader = true,
                        IsChild = false,
                        AdditionalValues = new Dictionary<string, object>
                        {
                            { "masterTestId", masterGroup.Key.MasterTestID },
                            { "isChemical", true }
                        }
                    });

                    var subGroups = masterGroup.GroupBy(x => new { x.SubGroupID, x.SubGroupName, x.ReportTestName });
                    foreach (var sg in subGroups)
                    {
                        // Level 1 SubHeader (SubGroup)
                        result.Add(new DropdwonSelector
                        {
                            Id = 0,
                            Name = sg.Key.SubGroupName,
                            Level = 1,
                            Selectable = false,
                            NodeType = "SubGroup",
                            ParentId = masterGroup.Key.MasterTestID,
                            IsHeader = true,
                            IsChild = true,
                            AdditionalValues = new Dictionary<string, object>
                            {
                                { "masterTestId", masterGroup.Key.MasterTestID },
                                { "subGroupId", sg.Key.SubGroupID },
                                { "reportTestName", sg.Key.ReportTestName ?? "" },
                                { "isChemical", true }
                            }
                        });

                        // Level 2 Leaf (Analysis Type)
                        foreach (var at in sg)
                        {
                            result.Add(new DropdwonSelector
                            {
                                Id = at.AnalysisTypeID,
                                Name = at.AnalysisTypeName,
                                Level = 2,
                                Selectable = true,
                                NodeType = "AnalysisType",
                                ParentId = sg.Key.SubGroupID,
                                IsHeader = false,
                                IsChild = true,
                                AdditionalValues = new Dictionary<string, object>
                                {
                                    { "masterTestId", masterGroup.Key.MasterTestID },
                                    { "masterTestName", masterGroup.Key.MasterTestName },
                                    { "subGroupId", sg.Key.SubGroupID },
                                    { "subGroupName", sg.Key.SubGroupName },
                                    { "analysisTypeId", at.AnalysisTypeID },
                                    { "testId", at.AnalysisTypeID },
                                    { "fullDisplayName", $"{masterGroup.Key.MasterTestName} - {sg.Key.SubGroupName} ({at.AnalysisTypeName})" },
                                    { "reportTestName", sg.Key.ReportTestName ?? "" },
                                    { "isChemical", true }
                                }
                            });
                        }
                    }
                }
            }

            // 4. Chemical SubGroups without Analysis Types (e.g. Wet Chemical Analysis) (2 Levels)
            var standaloneChemQuery = from sg in _context.LaboratoryTestSubGroups
                                      join a in _context.LaboratoryTests on sg.LaboratoryTestID equals a.ID
                                      join d in _context.DepartmentMasters on a.LabDepartmentID equals d.ID
                                      where sg.IsActive && a.IsActive && a.CompanyCode == loggedInUser.CompanyCode && d.IsChemical
                                            && !_context.LaboratoryTestAnalysisTypes.Any(at => at.LaboratoryTestSubGroupID == sg.ID && at.IsActive)
                                      select new
                                      {
                                          SubGroupID = sg.ID,
                                          SubGroupName = sg.Name,
                                          ReportTestName = sg.ReportTestName,
                                          MasterTestID = a.ID,
                                          MasterTestName = a.Name,
                                          DisplayOrder = sg.DisplayOrder
                                      };

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var s = searchTerm.Trim();
                standaloneChemQuery = standaloneChemQuery.Where(x => (x.SubGroupName != null && x.SubGroupName.Contains(s))
                                                                  || (x.MasterTestName != null && x.MasterTestName.Contains(s))
                                                                  || (x.ReportTestName != null && x.ReportTestName.Contains(s)));
            }

            var standaloneChemData = await standaloneChemQuery
                .OrderBy(x => x.MasterTestName)
                .ThenBy(x => x.DisplayOrder)
                .ThenBy(x => x.SubGroupName)
                .ToListAsync();

            if (standaloneChemData.Count > 0)
            {
                var standaloneGroups = standaloneChemData.GroupBy(x => new { x.MasterTestID, x.MasterTestName });
                foreach (var group in standaloneGroups)
                {
                    // Only add master header if not already added
                    if (!result.Any(r => r.IsHeader && r.Level == 0 && r.AdditionalValues != null && r.AdditionalValues.ContainsKey("masterTestId") && (long)r.AdditionalValues["masterTestId"] == group.Key.MasterTestID))
                    {
                        result.Add(new DropdwonSelector
                        {
                            Id = 0,
                            Name = group.Key.MasterTestName,
                            Level = 0,
                            Selectable = false,
                            NodeType = "TestGroup",
                            IsHeader = true,
                            IsChild = false,
                            AdditionalValues = new Dictionary<string, object>
                            {
                                { "masterTestId", group.Key.MasterTestID },
                                { "isChemical", true }
                            }
                        });
                    }

                    foreach (var item in group)
                    {
                        result.Add(new DropdwonSelector
                        {
                            Id = item.SubGroupID,
                            Name = item.SubGroupName,
                            Level = 1,
                            Selectable = true,
                            NodeType = "SubGroup",
                            ParentId = group.Key.MasterTestID,
                            IsHeader = false,
                            IsChild = true,
                            AdditionalValues = new Dictionary<string, object>
                            {
                                { "masterTestId", group.Key.MasterTestID },
                                { "masterTestName", group.Key.MasterTestName },
                                { "subGroupId", item.SubGroupID },
                                { "testId", item.SubGroupID },
                                { "fullDisplayName", $"{group.Key.MasterTestName} - {item.SubGroupName}" },
                                { "reportTestName", item.ReportTestName ?? "" },
                                { "isChemical", true }
                            }
                        });
                    }
                }
            }

            return result;
        }
    }
}


