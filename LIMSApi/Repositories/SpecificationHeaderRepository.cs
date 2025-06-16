using System.Linq.Dynamic.Core;
using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Repositories
{
    public class SpecificationHeaderRepository : ISpecificationHeaderRepository
    {
        private readonly LIMSContext _context;

        public SpecificationHeaderRepository(LIMSContext context)
        {
            _context = context;
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
                          where c.IsActive && c.IsCustom == false
                          from so in soGroup.DefaultIfEmpty()

                          select new
                          {
                              c.ID,
                              c.Standard,
                              c.Part,
                              c.StandardOrganizationID,
                              StandardOrganizationName = so.Name,
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
                          where c.IsActive && c.IsCustom == true
                          from so in soGroup.DefaultIfEmpty()

                          select new
                          {
                              c.ID,
                              c.Standard,
                              c.Part,
                              c.StandardOrganizationID,
                              StandardOrganizationName = so.Name,
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

            var _query = from a in _context.SpecificationHeaders
                         join g in _context.SpecificationGrades on a.ID equals g.SpecificationHeaderID
                         where a.IsActive
                         select new
                         {
                             g.ID,
                             AliasName = $"{a.AliasName}-{g.Grade}",
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

        public async Task<bool> ExistsByName(string name)
        {
            return await _context.SpecificationHeaders.AnyAsync(x => x.AliasName == name && x.IsActive);
        }

        public async Task<bool> ExistsByNameAndNotId(string name, long Id)
        {
            return await _context.SpecificationHeaders.AnyAsync(x => x.AliasName == name && x.ID != Id && x.IsActive);
        }
    }
}
