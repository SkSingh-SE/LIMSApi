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

            var _query = from a in _context.LaboratoryTests
                         join d in _context.DepartmentMasters on a.LabDepartmentID equals d.ID
                         where a.IsActive && a.CompanyCode == loggedInUser.CompanyCode
                         select new
                         {
                             a.ID,
                             a.Name,
                             Department = d.Name
                         };

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
                Name = $"{x.Name} ({x.Department}) ",
            })).ToListAsync();

            return data;
        }

        public async Task<List<DropdwonSelector>> GetGeneralTestMethodDropdown(string? searchTerm, int pageNo = 0, int pageSize = 20)
        {
            if (pageNo < 0) pageNo = 0;

            var _query = from sg in _context.LaboratoryTestSubGroups
                         join a in _context.LaboratoryTests on sg.LaboratoryTestID equals a.ID
                         join d in _context.DepartmentMasters on a.LabDepartmentID equals d.ID
                         where sg.IsActive && a.IsActive && a.CompanyCode == loggedInUser.CompanyCode
                               && !d.IsChemical
                         select new
                         {
                             sg.ID,
                             SubGroupName = sg.Name,
                             TestName = a.Name,
                             Department = d.Name
                         };

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                if (FilterHelper.IsExactIdSearch(searchTerm, out long exactId))
                {
                    _query = _query.Where(x => x.ID == exactId);
                }
                else
                {
                    var search = searchTerm.Trim();
                    _query = _query.Where(x => (x.SubGroupName != null && x.SubGroupName.Contains(search)) || (x.TestName != null && x.TestName.Contains(search)));
                }
            }

            var skip = pageNo * pageSize;

            var data = await (_query.Skip(skip).Take(pageSize).Select(x => new DropdwonSelector
            {
                Id = x.ID,
                Name = $"{x.SubGroupName} ({x.TestName})"
            })).ToListAsync();

            if (data.Count == 0)
            {
                // Fallback to LaboratoryTests if no SubGroup is created yet
                var fallbackQuery = from a in _context.LaboratoryTests
                                    join d in _context.DepartmentMasters on a.LabDepartmentID equals d.ID
                                    where a.IsActive && a.CompanyCode == loggedInUser.CompanyCode && !d.IsChemical
                                    select new { a.ID, a.Name, Department = d.Name };

                if (!string.IsNullOrWhiteSpace(searchTerm) && !FilterHelper.IsExactIdSearch(searchTerm, out _))
                {
                    fallbackQuery = fallbackQuery.Where(x => x.Name.Contains(searchTerm.Trim()));
                }

                data = await (fallbackQuery.Skip(skip).Take(pageSize).Select(x => new DropdwonSelector
                {
                    Id = x.ID,
                    Name = $"{x.Name} ({x.Department})"
                })).ToListAsync();
            }

            return data;
        }

        public async Task<List<DropdwonSelector>> GetChemicalTestMethodDropdown(string? searchTerm, int pageNo = 0, int pageSize = 20)
        {
            if (pageNo < 0) pageNo = 0;

            var _query = from a in _context.LaboratoryTests
                         join d in _context.DepartmentMasters on a.LabDepartmentID equals d.ID
                         where a.IsActive && a.CompanyCode == loggedInUser.CompanyCode
                               && d.IsChemical
                         select new
                         {
                             a.ID,
                             a.Name,
                             Department = d.Name
                         };
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
                Name = $"{x.Name} ({x.Department}) ",
            })).ToListAsync();

            return data;
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
    }
}
