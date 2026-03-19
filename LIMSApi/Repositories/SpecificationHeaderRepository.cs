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
            return await _context.SpecificationHeaders
                 .Include(x => x.Grades)
                     .ThenInclude(sl => sl.SpecificationLines).ThenInclude(t => t.LaboratoryTests)
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
                          join g in _context.SpecificationGrades on c.ID equals g.SpecificationHeaderID

                          join so in _context.StandardOrganizationMasters
                          on c.StandardOrganizationID equals so.ID into soGroup
                          from so in soGroup.DefaultIfEmpty()

                          join mc in _context.MetalClassificationMasters
                          on g.MetalClassificationID equals mc.ID into mcGroup
                          from mc in mcGroup.DefaultIfEmpty()

                          where c.IsActive && c.IsCustom == false

                          select new
                          {
                              c.ID,
                              c.Standard,
                              c.Part,
                              c.StandardOrganizationID,
                              StandardOrganizationName = so.Name,
                              MetalClassificationName = mc != null ? mc.Name : null,
                              g.UNSSteelNumber,
                              c.AliasName,
                              g.Grade,
                              c.StandardYear
                          }).AsQueryable().ApplyFilters(filter.Filter);


            if (!string.IsNullOrWhiteSpace(filter.searchTerm))
            {
                var search = filter.searchTerm.Trim().ToLower();
                _query = _query.Where(x => (x.Standard != null && x.Standard.ToLower().Contains(search))
                || (x.Part != null && x.Part.ToLower().Contains(search))
                || (x.StandardYear != null && x.StandardYear.ToLower().Contains(search))
                || (x.UNSSteelNumber != null && x.UNSSteelNumber.ToLower().Contains(search))
                || (x.Grade != null && x.Grade.ToLower().Contains(search))
                || (x.AliasName != null && x.AliasName.ToLower().Contains(search))
                || (x.MetalClassificationName != null && x.MetalClassificationName.ToLower().Contains(search))
                );
            }

            if (filter.SortByColumn != null)
            {
                _query = _query.OrderBy($"{filter.SortByColumn} {(filter.SortOrder == "asc" ? "ascending" : "descending")}");
            }

            // Total Records Count
            int totalRecords = await _query.CountAsync();

            // Apply Pagination
            var items = await _query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return new PagedResponse<object>(items.Cast<object>().ToList(), totalRecords, filter.PageNumber, filter.PageSize);
        }

        public async Task<PagedResponse<object>> GetAllCustomSpecificationHeaders(PageFilter filter)
        {
            var _query = (from c in _context.SpecificationHeaders
                          join g in _context.SpecificationGrades on c.ID equals g.SpecificationHeaderID
                          join so in _context.StandardOrganizationMasters
                          on c.StandardOrganizationID equals so.ID into soGroup
                          from so in soGroup.DefaultIfEmpty()

                          join mc in _context.MetalClassificationMasters
                          on g.MetalClassificationID equals mc.ID into mcGroup
                          from mc in mcGroup.DefaultIfEmpty()

                          where c.IsActive && c.IsCustom == true
                          select new
                          {
                              c.ID,
                              c.Standard,
                              c.Part,
                              c.StandardOrganizationID,
                              StandardOrganizationName = so.Name,
                              MetalClassificationName = mc != null ? mc.Name : null,
                              g.UNSSteelNumber,
                              c.AliasName,
                              g.Grade,
                              c.StandardYear
                          }).AsQueryable().ApplyFilters(filter.Filter);


            if (!string.IsNullOrWhiteSpace(filter.searchTerm))
            {
                var search = filter.searchTerm.Trim().ToLower();
                _query = _query.Where(x => (x.Standard != null && x.Standard.ToLower().Contains(search))
                || (x.Part != null && x.Part.ToLower().Contains(search))
                || (x.StandardYear != null && x.StandardYear.ToLower().Contains(search))
                || (x.UNSSteelNumber != null && x.UNSSteelNumber.ToLower().Contains(search))
                || (x.Grade != null && x.Grade.ToLower().Contains(search))
                || (x.AliasName != null && x.AliasName.ToLower().Contains(search))
                || (x.MetalClassificationName != null && x.MetalClassificationName.ToLower().Contains(search))
                );
            }

            if (filter.SortByColumn != null)
            {
                _query = _query.OrderBy($"{filter.SortByColumn} {(filter.SortOrder == "asc" ? "ascending" : "descending")}");
            }

            // Total Records Count
            int totalRecords = await _query.CountAsync();

            // Apply Pagination
            var items = await _query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return new PagedResponse<object>(items.Cast<object>().ToList(), totalRecords, filter.PageNumber, filter.PageSize);
        }


        public async Task<List<DropdwonSelector>> GetSpecificationHeaderDropdown(string? searchTerm, int pageNo = 0, int pageSize = 20)
        {
            if (pageNo < 0) pageNo = 0;

            var _query = from a in _context.SpecificationHeaders where a.IsActive select new
                         {
                             a.ID,
                             a.AliasName
                         };

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var search = searchTerm.Trim().ToLower();
                _query = _query.Where(x => (x.AliasName != null && x.AliasName.ToLower().Contains(search)) || x.ID.ToString().Contains(search));
            }

            var skip = pageNo * pageSize;

            var data = await (_query.Skip(skip).Take(pageSize).Select(x => new DropdwonSelector
            {
                Id = x.ID,
                Name = x.AliasName,
            })).ToListAsync();

            return data;
        }

        public async Task<List<DropdwonSelector>> GetGradeDropdown(string? searchTerm, int pageNo = 0, int pageSize = 20)
        {
            if (pageNo < 0) pageNo = 0;

            var query =
                from a in _context.SpecificationHeaders
                join g in _context.SpecificationGrades on a.ID equals g.SpecificationHeaderID
                join mc in _context.MetalClassificationMasters on g.MetalClassificationID equals mc.ID into mcGroup
                from mc in mcGroup.DefaultIfEmpty()
                where a.IsActive
                select new
                {
                    g.ID,
                    AliasName = a.AliasName + "-" + g.Grade + (mc != null ? ("-" + mc.Name) : "")
                };

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var search = searchTerm.Trim().ToLower();

                if (int.TryParse(search, out int id))
                {
                    query = query.Where(x => x.ID == id);
                }
                else
                {
                    query = query.Where(x => x.AliasName != null && x.AliasName.ToLower().Contains(search));
                }
            }


            var skip = pageNo * pageSize;

            var data = await query
                .Skip(skip)
                .Take(pageSize)
                .Select(x => new DropdwonSelector
                {
                    Id = x.ID,
                    Name = x.AliasName
                })
                .ToListAsync();

            return data;
        }
        public async Task<List<DropdwonSelector>> GetGradeDropdownMetalWise(string? searchTerm, int pageNo = 0, int pageSize = 20, long metalId = 0)
        {
            if (pageNo < 0) pageNo = 0;

            var query = from m in _context.MaterialTestMappings
                        join g in _context.SpecificationGrades on m.GradeID equals g.ID into gm
                        from g in gm.DefaultIfEmpty()
                        join h in _context.SpecificationHeaders on g.SpecificationHeaderID equals h.ID
                        join mc in _context.MetalClassificationMasters on g.MetalClassificationID equals mc.ID into mcGroup
                        from mc in mcGroup.DefaultIfEmpty()
                        where m.IsActive
                        select new
                        {
                            m.GradeID,
                            Grade = g.Grade,
                            AliasName = h.AliasName + "-" + g.Grade + (mc != null ? ("-" + mc.Name) : ""),
                            m.MetalClassificationID,
                            m.ProductConditionID
                        };

            // Filter by MetalClassificationID (if provided)
            if (metalId > 0)
                query = query.Where(x => x.MetalClassificationID == metalId);



            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var search = searchTerm.Trim().ToLower();
                query = query.Where(x =>
                    (x.AliasName != null && x.AliasName.ToLower().Contains(search))
                    || x.GradeID.ToString().Contains(search));
            }

            var skip = pageNo * pageSize;

            var data = await query
                .Skip(skip)
                .Take(pageSize)
                .Select(x => new DropdwonSelector
                {
                    Id = (long)x.GradeID,
                    Name = x.AliasName
                })
                .ToListAsync();

            return data;
        }

        public async Task<bool> ExistsByName(string name)
        {
            return await _context.SpecificationHeaders.AnyAsync(x => x.AliasName == name && x.IsActive);
        }

        public async Task<bool> ExistsByNameAndNotId(string name, long Id)
        {
            return await _context.SpecificationHeaders.AnyAsync(x => x.AliasName == name && x.ID != Id && x.IsActive);
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

            var query = from spec in _context.SpecificationHeaders
                        join grade in _context.SpecificationGrades
                            on spec.ID equals grade.SpecificationHeaderID
                        join line in _context.SpecificationLines
                            on grade.ID equals line.SpecificationGradeID
                        join specLineLabTest in _context.SpecificationLineLaboratoryTests
                            on line.ID equals specLineLabTest.SpecificationLineID
                        join test in _context.LaboratoryTests
                            on specLineLabTest.LaboratoryTestID equals test.ID
                        where gradeIds.Contains(grade.ID)   
                              && spec.IsActive
                              && spec.CompanyCode == loggedInUser.CompanyCode
                        select new DropdwonSelector
                        {
                            Id = test.ID,
                            Name = test.Name
                        };

            var data = await query.ToListAsync();
            return data.DistinctBy(x => x.Id).ToList();

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
