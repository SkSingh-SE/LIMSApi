using System.Linq;
using System.Linq.Dynamic.Core;
using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Repositories
{
    public class SpecificationHeaderRepository : ISpecificationHeaderRepository
    {
        private readonly LIMSContext _context;
        private LoggedInUserDTO loggedInUser;
        public SpecificationHeaderRepository(LIMSContext context)
        {
            _context = context;
            loggedInUser = LoggedInUserProvider.CurrentUser;
        }

        public async Task AddSpecificationHeader(SpecificationHeader model)
        {
            await _context.SpecificationHeaders.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteSpecificationHeader(long id)
        {
            var existingSpecificationHeader = await _context.SpecificationHeaders.FirstOrDefaultAsync(x => x.ID == id && x.IsActive);
            if (existingSpecificationHeader != null)
            {
                existingSpecificationHeader.IsActive = false;
                existingSpecificationHeader.ModifiedOn = DateTime.UtcNow;
                _context.SpecificationHeaders.Update(existingSpecificationHeader);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<SpecificationHeader?> GetSpecificationHeaderById(long id)
        {
            // AsSplitQuery: prevents Cartesian explosion from multiple ThenInclude chains on
            // the same Grades/SpecificationLines collections. EF Core issues separate SQL queries
            // per Include chain instead of a single mega-JOIN that multiplies rows.
            return await _context.SpecificationHeaders
                 .AsSplitQuery()
                 .Include(x => x.Grades)
                     .ThenInclude(g => g.MetalClassification)
                 .Include(x => x.Grades)
                     .ThenInclude(sl => sl.SpecificationLines)
                         .ThenInclude(l => l.Parameter)
                 .Include(x => x.Grades)
                     .ThenInclude(sl => sl.SpecificationLines)
                         .ThenInclude(l => l.TestMethodMappings)
                 .Include(x => x.HeaderParameters)
                     .ThenInclude(hp => hp.Parameter)
                 .Include(x => x.HeaderParameters)
                     .ThenInclude(hp => hp.ParameterUnit)
                 .FirstOrDefaultAsync(x => x.ID == id && x.IsActive);
        }

        public async Task UpdateSpecificationHeader(SpecificationHeader model)
        {
            _context.SpecificationHeaders.Update(model);
            await _context.SaveChangesAsync();
        }

        public async Task<PagedResponse<object>> GetAllSpecificationHeaders(PageFilter filter)
        {
            var _query = (from c in _context.SpecificationHeaders

                          join so in _context.StandardOrganizationMasters
                          on c.StandardOrganizationID equals so.ID into soGroup
                          from so in soGroup.DefaultIfEmpty()

                          where c.IsActive && c.IsCustom == false

                          select new
                          {
                              c.ID,
                              c.SpecificationNo,
                              c.Standard,
                              c.Part,
                              c.StandardOrganizationID,
                              StandardOrganizationName = so != null ? so.Name : null,
                              c.AliasName,
                              c.DisplayTitle,
                              c.StandardYear,
                              c.Version,
                              c.Title,
                              c.ModifiedOn
                          }).AsQueryable().ApplyFilters(filter.Filter);


            if (!string.IsNullOrWhiteSpace(filter.searchTerm))
            {
                var search = filter.searchTerm.Trim();
                _query = _query.Where(x => (x.Standard != null && x.Standard.Contains(search))
                || (x.Part != null && x.Part.Contains(search))
                || (x.StandardYear != null && x.StandardYear.Contains(search))
                || (x.AliasName != null && x.AliasName.Contains(search))
                || (x.DisplayTitle != null && x.DisplayTitle.Contains(search))
                || (x.SpecificationNo != null && x.SpecificationNo.Contains(search))
                || (x.Title != null && x.Title.Contains(search))
                );
            }

            if (filter.SortByColumn != null)
            {
                _query = _query.OrderBy($"{filter.SortByColumn} {(filter.SortOrder == "asc" ? "ascending" : "descending")}");
            }

            return await _query.Cast<object>().ToPagedAsync(filter);
        }

        public async Task<PagedResponse<object>> GetAllCustomSpecificationHeaders(PageFilter filter)
        {
            var _query = (from c in _context.SpecificationHeaders
                          join so in _context.StandardOrganizationMasters
                          on c.StandardOrganizationID equals so.ID into soGroup
                          from so in soGroup.DefaultIfEmpty()

                          where c.IsActive && c.IsCustom == true
                          select new
                          {
                              c.ID,
                              c.SpecificationNo,
                              c.Standard,
                              c.Part,
                              c.StandardOrganizationID,
                              StandardOrganizationName = so != null ? so.Name : null,
                              c.AliasName,
                              c.DisplayTitle,
                              c.StandardYear,
                              c.Version,
                              c.Title,
                              c.ModifiedOn
                          }).AsQueryable().ApplyFilters(filter.Filter);


            if (!string.IsNullOrWhiteSpace(filter.searchTerm))
            {
                var search = filter.searchTerm.Trim();
                _query = _query.Where(x => (x.Standard != null && x.Standard.Contains(search))
                || (x.Part != null && x.Part.Contains(search))
                || (x.StandardYear != null && x.StandardYear.Contains(search))
                || (x.AliasName != null && x.AliasName.Contains(search))
                || (x.DisplayTitle != null && x.DisplayTitle.Contains(search))
                || (x.SpecificationNo != null && x.SpecificationNo.Contains(search))
                || (x.Title != null && x.Title.Contains(search))
                );
            }

            if (filter.SortByColumn != null)
            {
                _query = _query.OrderBy($"{filter.SortByColumn} {(filter.SortOrder == "asc" ? "ascending" : "descending")}");
            }

            return await _query.Cast<object>().ToPagedAsync(filter);
        }


        public async Task<List<DropdwonSelector>> GetSpecificationHeaderDropdown(string? searchTerm, int pageNo = 0, int pageSize = 20)
        {
            if (pageNo < 0) pageNo = 0;

            // 1. Exact ID search lookup
            if (!string.IsNullOrWhiteSpace(searchTerm) && FilterHelper.IsExactIdSearch(searchTerm, out long exactId))
            {
                var exactMatch = await (from a in _context.SpecificationHeaders
                                        join so in _context.StandardOrganizationMasters on a.StandardOrganizationID equals so.ID into soGroup
                                        from so in soGroup.DefaultIfEmpty()
                                        where a.ID == exactId && a.IsActive
                                        select new DropdwonSelector
                                        {
                                            Id = a.ID,
                                            Name = a.DisplayTitle ?? (!string.IsNullOrEmpty(a.SpecificationNo) ? (a.AliasName + " " + a.SpecificationNo) : a.AliasName),
                                            Level = 1,
                                            Selectable = true,
                                            NodeType = "SpecificationStandard",
                                            ParentId = so != null ? so.ID : 0,
                                            IsHeader = false,
                                            IsChild = true,
                                            AdditionalValues = new Dictionary<string, object>
                                            {
                                                { "materialSpecificationId", a.ID },
                                                { "specificationNo", a.SpecificationNo ?? a.AliasName },
                                                { "aliasName", a.AliasName },
                                                { "standardOrgId", so != null ? so.ID : 0 },
                                                { "standardOrgName", so != null ? so.Name : "" }
                                            }
                                        }).FirstOrDefaultAsync();

                if (exactMatch != null)
                {
                    return new List<DropdwonSelector> { exactMatch };
                }
            }

            // 2. 2-Tier Hierarchy Query: StandardOrganization (Level 0) -> SpecificationHeader (Level 1 Leaf)
            var query = from a in _context.SpecificationHeaders
                        join so in _context.StandardOrganizationMasters on a.StandardOrganizationID equals so.ID into soGroup
                        from so in soGroup.DefaultIfEmpty()
                        where a.IsActive
                        select new
                        {
                            SpecHeaderID = a.ID,
                            SpecAliasName = a.AliasName,
                            SpecNo = a.SpecificationNo,
                            SpecDisplayTitle = a.DisplayTitle,
                            StandardOrgID = so != null ? so.ID : 0,
                            StandardOrgName = so != null ? so.Name : "Other Standards"
                        };

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var search = searchTerm.Trim();
                query = query.Where(x => (x.SpecAliasName != null && x.SpecAliasName.Contains(search))
                                      || (x.SpecNo != null && x.SpecNo.Contains(search))
                                      || (x.SpecDisplayTitle != null && x.SpecDisplayTitle.Contains(search))
                                      || (x.StandardOrgName != null && x.StandardOrgName.Contains(search)));
            }

            var rawData = await query
                .OrderBy(x => x.StandardOrgName)
                .ThenBy(x => x.SpecAliasName)
                .ToListAsync();

            var result = new List<DropdwonSelector>();

            if (rawData.Count > 0)
            {
                var orgGroups = rawData.GroupBy(x => new { x.StandardOrgID, x.StandardOrgName });

                foreach (var orgGroup in orgGroups)
                {
                    // Level 0: Standard Organization Header (Non-selectable)
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

                    // Level 1: Specification Header (Selectable Leaf)
                    foreach (var spec in orgGroup)
                    {
                        var displayTitle = !string.IsNullOrEmpty(spec.SpecDisplayTitle)
                            ? spec.SpecDisplayTitle
                            : (!string.IsNullOrEmpty(spec.SpecNo) ? $"{spec.SpecAliasName} {spec.SpecNo}" : spec.SpecAliasName);

                        result.Add(new DropdwonSelector
                        {
                            Id = spec.SpecHeaderID,
                            Name = displayTitle,
                            Level = 1,
                            Selectable = true,
                            NodeType = "SpecificationStandard",
                            ParentId = orgGroup.Key.StandardOrgID,
                            IsHeader = false,
                            IsChild = true,
                            AdditionalValues = new Dictionary<string, object>
                            {
                                { "materialSpecificationId", spec.SpecHeaderID },
                                { "specificationNo", spec.SpecNo ?? spec.SpecAliasName },
                                { "aliasName", spec.SpecAliasName },
                                { "displayTitle", displayTitle },
                                { "standardOrgId", orgGroup.Key.StandardOrgID },
                                { "standardOrgName", orgGroup.Key.StandardOrgName }
                            }
                        });
                    }
                }
            }

            return result;
        }

        public async Task<List<DropdwonSelector>> GetGradeDropdown(string? searchTerm, int pageNo = 0, int pageSize = 20)
        {
            return await GetGradeDropdownMetalWise(searchTerm, pageNo, pageSize, 0);
        }

        public async Task<List<DropdwonSelector>> GetGradeDropdownMetalWise(string? searchTerm, int pageNo = 0, int pageSize = 20, long metalId = 0)
        {
            if (pageNo < 0) pageNo = 0;

            // 1. Exact ID lookup for instantaneous single-item rebind
            if (!string.IsNullOrWhiteSpace(searchTerm) && FilterHelper.IsExactIdSearch(searchTerm, out long exactId))
            {
                var exactMatch = await (from g in _context.SpecificationGrades
                                        join h in _context.SpecificationHeaders on g.SpecificationHeaderID equals h.ID
                                        join so in _context.StandardOrganizationMasters on h.StandardOrganizationID equals so.ID into soGroup
                                        from so in soGroup.DefaultIfEmpty()
                                        join mc in _context.MetalClassificationMasters on g.MetalClassificationID equals mc.ID into mcGroup
                                        from mc in mcGroup.DefaultIfEmpty()
                                        where g.ID == exactId && h.IsActive
                                        select new DropdwonSelector
                                        {
                                            Id = g.ID,
                                            Name = g.Grade,
                                            Level = 2,
                                            Selectable = true,
                                            NodeType = "Grade",
                                            ParentId = h.ID,
                                            IsHeader = false,
                                            IsChild = true,
                                            AdditionalValues = new Dictionary<string, object>
                                            {
                                                { "materialSpecificationId", h.ID },
                                                { "materialSpecificationName", h.AliasName },
                                                { "specificationNo", h.SpecificationNo ?? h.AliasName },
                                                { "displayTitle", h.DisplayTitle ?? h.AliasName },
                                                { "standardOrgId", so != null ? so.ID : 0 },
                                                { "standardOrgName", so != null ? so.Name : "" },
                                                { "gradeId", g.ID },
                                                { "gradeName", g.Grade },
                                                { "metalClassificationId", g.MetalClassificationID ?? 0 },
                                                { "metalClassificationName", mc != null ? mc.Name : "" }
                                            }
                                        }).FirstOrDefaultAsync();

                if (exactMatch != null)
                {
                    return new List<DropdwonSelector> { exactMatch };
                }
            }

            // 2. 3-Tier Hierarchy Query: StandardOrganization (Level 0) -> SpecificationHeader (Level 1) -> SpecificationGrade (Level 2 Leaf)
            var query = from g in _context.SpecificationGrades
                        join h in _context.SpecificationHeaders on g.SpecificationHeaderID equals h.ID
                        join so in _context.StandardOrganizationMasters on h.StandardOrganizationID equals so.ID into soGroup
                        from so in soGroup.DefaultIfEmpty()
                        join mc in _context.MetalClassificationMasters on g.MetalClassificationID equals mc.ID into mcGroup
                        from mc in mcGroup.DefaultIfEmpty()
                        where h.IsActive
                        select new
                        {
                            GradeID = g.ID,
                            GradeName = g.Grade,
                            MetalClassificationID = g.MetalClassificationID,
                            MetalClassificationName = mc != null ? mc.Name : null,
                            SpecHeaderID = h.ID,
                            SpecAliasName = h.AliasName,
                            SpecNo = h.SpecificationNo,
                            SpecDisplayTitle = h.DisplayTitle,
                            StandardOrgID = so != null ? so.ID : 0,
                            StandardOrgName = so != null ? so.Name : "Other Standards"
                        };

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var search = searchTerm.Trim();
                query = query.Where(x => (x.GradeName != null && x.GradeName.Contains(search))
                                      || (x.SpecAliasName != null && x.SpecAliasName.Contains(search))
                                      || (x.SpecNo != null && x.SpecNo.Contains(search))
                                      || (x.SpecDisplayTitle != null && x.SpecDisplayTitle.Contains(search))
                                      || (x.StandardOrgName != null && x.StandardOrgName.Contains(search)));
            }

            var rawData = await query
                .OrderBy(x => metalId > 0 ? (x.MetalClassificationID == metalId ? 0 : 1) : 0)
                .ThenBy(x => x.StandardOrgName)
                .ThenBy(x => x.SpecAliasName)
                .ThenBy(x => x.GradeName)
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

                    // 2. Level 1: Specification Standard Intermediate Header (Non-selectable)
                    var specGroups = orgGroup.GroupBy(x => new
                    {
                        x.SpecHeaderID,
                        x.SpecAliasName,
                        x.SpecNo,
                        DisplayTitle = !string.IsNullOrEmpty(x.SpecDisplayTitle)
                            ? x.SpecDisplayTitle
                            : (!string.IsNullOrEmpty(x.SpecNo) ? $"{x.SpecAliasName} {x.SpecNo}" : x.SpecAliasName)
                    });

                    foreach (var specGroup in specGroups)
                    {
                        result.Add(new DropdwonSelector
                        {
                            Id = 0,
                            Name = specGroup.Key.DisplayTitle,
                            Level = 1,
                            Selectable = false,
                            NodeType = "SpecificationStandard",
                            ParentId = orgGroup.Key.StandardOrgID,
                            IsHeader = true,
                            IsChild = true,
                            AdditionalValues = new Dictionary<string, object>
                            {
                                { "standardOrgId", orgGroup.Key.StandardOrgID },
                                { "materialSpecificationId", specGroup.Key.SpecHeaderID },
                                { "specificationNo", specGroup.Key.SpecNo ?? specGroup.Key.SpecAliasName },
                                { "name", specGroup.Key.SpecAliasName },
                                { "displayTitle", specGroup.Key.DisplayTitle }
                            }
                        });

                        // 3. Level 2: Specification Grade Leaf (Selectable)
                        foreach (var g in specGroup)
                        {
                            result.Add(new DropdwonSelector
                            {
                                Id = g.GradeID,
                                Name = g.GradeName,
                                Level = 2,
                                Selectable = true,
                                NodeType = "Grade",
                                ParentId = specGroup.Key.SpecHeaderID,
                                IsHeader = false,
                                IsChild = true,
                                AdditionalValues = new Dictionary<string, object>
                                {
                                    { "materialSpecificationId", specGroup.Key.SpecHeaderID },
                                    { "materialSpecificationName", specGroup.Key.SpecAliasName },
                                    { "specificationNo", specGroup.Key.SpecNo ?? specGroup.Key.SpecAliasName },
                                    { "displayTitle", specGroup.Key.DisplayTitle },
                                    { "standardOrgId", orgGroup.Key.StandardOrgID },
                                    { "standardOrgName", orgGroup.Key.StandardOrgName },
                                    { "gradeId", g.GradeID },
                                    { "gradeName", g.GradeName },
                                    { "metalClassificationId", g.MetalClassificationID ?? 0 },
                                    { "metalClassificationName", g.MetalClassificationName ?? "" }
                                }
                            });
                        }
                    }
                }
            }

            return result;
        }

        // Uniqueness is (AliasName + Version) so the same spec can exist across versions.
        public async Task<bool> ExistsByName(string name)
        {
            return await _context.SpecificationHeaders.AnyAsync(x => x.AliasName == name && x.IsActive);
        }

        public async Task<bool> ExistsByNameAndNotId(string name, long Id)
        {
            return await _context.SpecificationHeaders.AnyAsync(x => x.AliasName == name && x.ID != Id && x.IsActive);
        }

        public async Task<bool> ExistsByNameAndVersion(string name, string? version, long excludeId = 0)
        {
            return await _context.SpecificationHeaders.AnyAsync(x =>
                x.AliasName == name
                && ((x.Version ?? "") == (version ?? ""))
                && x.ID != excludeId
                && x.IsActive);
        }

        public async Task<List<DropdwonSelector>> GetDefaultStandardForSpecification(long gradeId)
        {
            var query = from grade in _context.SpecificationGrades
                        join spec in _context.SpecificationHeaders
                            on grade.SpecificationHeaderID equals spec.ID
                        join std in _context.StandardOrganizationMasters
                            on spec.StandardOrganizationID equals std.ID
                        where grade.ID == gradeId
                              && spec.IsActive
                              && spec.CompanyCode == loggedInUser.CompanyCode
                        select new DropdwonSelector
                        {
                            Id = std.ID,   
                            Name = std.Name  
                        };

            return await query.ToListAsync();

        }
        public async Task<List<DropdwonSelector>> GetTestMethodsForSpecifications(long gradeId1, long gradeId2 = 0)
        {
            var gradeIds = new List<long> { gradeId1 };
            if (gradeId2 != 0)
            {
                gradeIds.Add(gradeId2);
            }

            var query = from sub in _context.Set<LaboratoryTestSubGroupSpecification>()
                        join test in _context.LaboratoryTests on sub.SubGroup!.LaboratoryTestID equals test.ID
                        where sub.SpecificationGradeID.HasValue && gradeIds.Contains(sub.SpecificationGradeID.Value)
                              && test.IsActive
                        select new DropdwonSelector
                        {
                            Id = test.ID,
                            Name = test.Name
                        };

            var data = await query.ToListAsync();
            return data.GroupBy(x => x.Id).Select(g => g.First()).ToList();
        }
        public async Task<List<ChemicalElementDto>> GetChemicalElementsBySpecificationsAsync(long spec1Id = 0, long spec2Id = 0)
        {
            var gradeIds = new List<long>();
            if (spec1Id > 0) gradeIds.Add(spec1Id);
            if (spec2Id > 0 && spec2Id != spec1Id) gradeIds.Add(spec2Id);

            if (!gradeIds.Any())
                return new List<ChemicalElementDto>();

            var chemicalLines = await _context.SpecificationLines
                .Where(l => gradeIds.Contains(l.SpecificationGradeID.Value) && l.Type.ToLower() == "chemical")
                .Select(l => new ChemicalElementDto
                {
                    SpecificationLineID = l.ID,
                    ParameterID = l.ParameterID,
                    ParameterName = l.Parameter != null ? l.Parameter.Name : null,
                    MinValue = l.MinValue,
                    MaxValue = l.MaxValue,
                    ParameterUnitID = l.ParameterUnitID,
                    ParameterUnit = l.ParameterUnit != null ? l.ParameterUnit.Name : null
                })
                .Distinct()
                .ToListAsync();

            // Deduplicate by parameterID
            var unique = chemicalLines
                .GroupBy(x => x.ParameterID)
                .Select(g => g.First())
                .ToList();

            return unique;
        }

    }
}
