using System.Linq.Dynamic.Core;
using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Repositories
{
    public class SpecimenOrientationRepository : ISpecimenOrientationRepository
    {
        private readonly LIMSContext _context;

        public SpecimenOrientationRepository(LIMSContext context)
        {
            _context = context;
        }

        public async Task AddSpecimenOrientation(SpecimenOrientationMaster model)
        {
            await _context.SpecimenOrientationMasters.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteSpecimenOrientation(long id)
        {
            var existingSpecimenOrientation = await _context.SpecimenOrientationMasters.FirstOrDefaultAsync(x => x.ID == id && x.IsActive);
            if (existingSpecimenOrientation != null)
            {
                existingSpecimenOrientation.IsActive = false;
                existingSpecimenOrientation.ModifiedOn = DateTime.UtcNow;
                _context.SpecimenOrientationMasters.Update(existingSpecimenOrientation);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<SpecimenOrientationMaster?> GetSpecimenOrientationById(long id)
        {
            return await _context.SpecimenOrientationMasters.FirstOrDefaultAsync(x => x.ID == id && x.IsActive);
        }

        public async Task UpdateSpecimenOrientation(SpecimenOrientationMaster model)
        {
            _context.SpecimenOrientationMasters.Update(model);
            await _context.SaveChangesAsync();
        }

        public async Task<PagedResponse<object>> GetAllSpecimenOrientations(PageFilter filter)
        {
            var _query = from c in _context.SpecimenOrientationMasters where c.IsActive select c;

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
                _query = _query.Where(x => (x.Name != null && x.Name.ToLower().Contains(search)));
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

        public async Task<List<DropdwonSelector>> GetSpecimenOrientationDropdown(string? searchTerm, int pageNo = 0, int pageSize = 20)
        {
            if (pageNo < 0) pageNo = 0;

            var _query = from a in _context.SpecimenOrientationMasters where a.IsActive select a;

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var search = searchTerm.Trim().ToLower();
                _query = _query.Where(x => (x.Name != null && x.Name.ToLower().Contains(search)));
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
            return await _context.SpecimenOrientationMasters.AnyAsync(x => x.Name == name && x.IsActive);
        }

        public async Task<bool> ExistsByNameAndNotId(string name, long Id)
        {
            return await _context.SpecimenOrientationMasters.AnyAsync(x => x.Name == name && x.ID != Id && x.IsActive);
        }
    }
}
