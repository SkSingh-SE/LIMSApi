using System.Linq.Dynamic.Core;
using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Repositories
{
    public class AreaRepository : IAreaRepository
    {
        private readonly LIMSContext _context;

        public AreaRepository(LIMSContext context)
        {
            _context = context;
        }

        public async Task AddArea(AreaMaster model)
        {
            await _context.AreaMasters.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteArea(long id)
        {
            var existingArea = await _context.AreaMasters.FirstOrDefaultAsync(x => x.ID == id && x.IsActive);
            if (existingArea != null)
            {
                existingArea.IsActive = false;
                existingArea.ModifiedOn = DateTime.UtcNow;
                _context.AreaMasters.Update(existingArea);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<AreaMaster?> GetAreaById(long id)
        {
            return await _context.AreaMasters.Include(x => x.City).ThenInclude(s => s.State).ThenInclude(c => c.Country).FirstOrDefaultAsync(x => x.ID == id && x.IsActive);
        }

        public async Task UpdateArea(AreaMaster model)
        {
            _context.AreaMasters.Update(model);
            await _context.SaveChangesAsync();
        }

        public async Task<PagedResponse<AreaMaster>> GetAllAreas(PageFilter filter)
        {
            var _query = from c in _context.AreaMasters where c.IsActive select c;

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
                _query = _query.Where(x => (x.Code != null && x.Code.ToLower().Contains(search))
                                     || (x.Name != null && x.Name.ToLower().Contains(search)));
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

            return new PagedResponse<AreaMaster>(items, totalRecords, filter.PageNumber, filter.PageSize);
        }

        public async Task<List<DropdwonSelector>> GetAreaWithPincode(string? searchTerm, int pageNo = 0, int pageSize = 20)
        {
            var _query = from a in _context.AreaMasters where a.IsActive select a;

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var search = searchTerm.Trim();
                _query = _query.Where(x => (x.Pincode != null && x.Pincode.Contains(search)) || x.Name.Contains(search));
            }
            var skip = pageNo * pageSize;

            var data = await (_query.Skip(skip).Take(pageSize).Select(x => new DropdwonSelector
            {
                Id = x.ID,
                Name = x.Pincode != null ? $"{x.Name}-({x.Pincode})" : $"{x.Name}",
            })).ToListAsync();

            return data;
        }

        public async Task<bool> ExistsByName(string name)
        {
            return await _context.AreaMasters.AnyAsync(x=>x.Name == name && x.IsActive);
        }

        public async Task<bool> ExistsByNameAndNotId(string name, long Id)
        {
            return await _context.AreaMasters.AnyAsync(x => x.Name == name && x.ID != Id && x.IsActive);
        }
    }
}
