using System.Linq.Dynamic.Core;
using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Repositories
{
    public class SpecificationLineRepository : ISpecificationLineRepository
    {
        private readonly LIMSContext _context;

        public SpecificationLineRepository(LIMSContext context)
        {
            _context = context;
        }

        public async Task AddSpecificationLine(SpecificationLine model)
        {
            await _context.SpecificationLines.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteSpecificationLine(long id)
        {
            var existingSpecificationLine = await _context.SpecificationLines.FirstOrDefaultAsync(x => x.ID == id && x.IsActive);
            if (existingSpecificationLine != null)
            {
                existingSpecificationLine.IsActive = false;
                existingSpecificationLine.ModifiedOn = DateTime.UtcNow;
                _context.SpecificationLines.Update(existingSpecificationLine);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<SpecificationLine?> GetSpecificationLineById(long id)
        {
            return await _context.SpecificationLines.FirstOrDefaultAsync(x => x.ID == id && x.IsActive);
        }

        public async Task UpdateSpecificationLine(SpecificationLine model)
        {
            _context.SpecificationLines.Update(model);
            await _context.SaveChangesAsync();
        }

        public async Task<PagedResponse<object>> GetAllSpecificationLines(PageFilter filter)
        {
            var _query = from c in _context.SpecificationLines
                         where c.IsActive
                         join sh in _context.SpecificationHeaders
                         on c.SpecificationHeaderID equals sh.ID into soGroup
                         from sh in soGroup.DefaultIfEmpty()


                         select new
                         {
                             c.ID,
                             c.PropertyType,
                             c.ManualSelection,
                             c.MinValue,
                             c.MaxValue,
                             c.Notes,
                             c.LowerLimit,
                             c.LowerLimitValue,
                             c.UpperLimit,
                             c.UpperLimitValue,
                             c.SpecificationHeaderID,
                             HeaderName = sh.AliasName,
                         };

            if (filter.Filters != null)
            {
                foreach (var filterItem in filter.Filters)
                {
                    if (string.IsNullOrWhiteSpace(filterItem.Value))
                    {
                        continue;
                    }
                    var propertyName = filterItem.Key;
                    var value = filterItem.Value;

                    _query = _query.Where($"{propertyName}.Contains(@0)", value);
                }
            }

            if (!string.IsNullOrWhiteSpace(filter.searchTerm))
            {
                var search = filter.searchTerm.Trim().ToLower();
                _query = _query.Where(x => (x.PropertyType != null && x.PropertyType.ToLower().Contains(search))
                || (x.Notes != null && x.Notes.ToLower().Contains(search))
                || (x.LowerLimit != null && x.LowerLimit.ToLower().Contains(search))
                || (x.LowerLimitValue != null && x.LowerLimitValue.ToString().ToLower().Contains(search))
                || (x.UpperLimit != null && x.UpperLimit.ToLower().Contains(search))
                || (x.UpperLimitValue != null && x.UpperLimitValue.ToString().ToLower().Contains(search))
                || (x.HeaderName != null && x.HeaderName.ToLower().Contains(search))
                );
            }

            if (filter.SortBy != null && filter.SortBy.Any())
            {
                var sortingExpressions = filter.SortBy
                   .Select(s => $"{s.Key} {(s.Value ? "descending" : "ascending")}");
                string orderByString = string.Join(", ", sortingExpressions);

                _query = _query.OrderBy(orderByString);
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

        public async Task<List<DropdwonSelector>> GetSpecificationLineDropdown(string? searchTerm, int pageNo = 0, int pageSize = 20)
        {
            if (pageNo < 0) pageNo = 0;

            var _query = from a in _context.SpecificationLines where a.IsActive select a;

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var search = searchTerm.Trim().ToLower();
                _query = _query.Where(x => (x.PropertyType != null && x.PropertyType.ToLower().Contains(search)));
            }

            var skip = pageNo * pageSize;

            var data = await (_query.Skip(skip).Take(pageSize).Select(x => new DropdwonSelector
            {
                Id = x.ID,
                Name = x.PropertyType != null ? x.PropertyType : x.Notes,
            })).ToListAsync();

            return data;
        }

       
    }
}
