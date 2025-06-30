using System.Linq.Dynamic.Core;
using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Repositories
{
    public class MakerRepository : IMakerRepository
    {
        private readonly LIMSContext _context;

        public MakerRepository(LIMSContext context)
        {
            _context = context;
        }

        public async Task AddMaker(MakerMaster model)
        {
            await _context.MakerMasters.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteMaker(long id)
        {
            var existingMaker = await _context.MakerMasters.FirstOrDefaultAsync(x => x.ID == id && x.IsActive);
            if (existingMaker != null)
            {
                existingMaker.IsActive = false;
                existingMaker.ModifiedOn = DateTime.UtcNow;
                _context.MakerMasters.Update(existingMaker);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<MakerMaster?> GetMakerById(long id)
        {
            return await _context.MakerMasters.FirstOrDefaultAsync(x => x.ID == id && x.IsActive);
        }

        public async Task UpdateMaker(MakerMaster model)
        {
            _context.MakerMasters.Update(model);
            await _context.SaveChangesAsync();
        }

        public async Task<PagedResponse<object>> GetAllMakers(PageFilter filter)
        {
            var _query = from c in _context.MakerMasters where c.IsActive select c;

            _query = _query.AsQueryable().ApplyFilters(filter.Filter);

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

        public async Task<List<DropdwonSelector>> GetMakerDropdown(string? searchTerm, int pageNo = 0, int pageSize = 20)
        {
            if (pageNo < 0) pageNo = 0;

            var _query = from a in _context.MakerMasters where a.IsActive select a;

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var search = searchTerm.Trim().ToLower();
                _query = _query.Where(x =>  (x.Name != null && x.Name.ToLower().Contains(search)));
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
            return await _context.MakerMasters.AnyAsync(x => x.Name == name && x.IsActive);
        }

        public async Task<bool> ExistsByNameAndNotId(string name, long Id)
        {
            return await _context.MakerMasters.AnyAsync(x => x.Name == name && x.ID != Id && x.IsActive);
        }
    }
}
