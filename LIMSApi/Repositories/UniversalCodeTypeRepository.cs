using System.Linq.Dynamic.Core;
using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Repositories
{
    public class UniversalCodeTypeRepository : IUniversalCodeTypeRepository
    {
        private readonly LIMSContext _context;

        public UniversalCodeTypeRepository(LIMSContext context)
        {
            _context = context;
        }

        public async Task AddUniversalCodeType(UniversalCodeTypeMaster model)
        {
            await _context.UniversalCodeTypeMasters.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteUniversalCodeType(long id)
        {
            var existingUniversalCodeType = await _context.UniversalCodeTypeMasters.FirstOrDefaultAsync(x => x.ID == id && x.IsActive);
            if (existingUniversalCodeType != null)
            {
                existingUniversalCodeType.IsActive = false;
                existingUniversalCodeType.ModifiedOn = DateTime.UtcNow;
                _context.UniversalCodeTypeMasters.Update(existingUniversalCodeType);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<UniversalCodeTypeMaster?> GetUniversalCodeTypeById(long id)
        {
            return await _context.UniversalCodeTypeMasters.FirstOrDefaultAsync(x => x.ID == id && x.IsActive);
        }

        public async Task UpdateUniversalCodeType(UniversalCodeTypeMaster model)
        {
            _context.UniversalCodeTypeMasters.Update(model);
            await _context.SaveChangesAsync();
        }

        public async Task<PagedResponse<object>> GetAllUniversalCodeTypes(PageFilter filter)
        {
            var _query = (from c in _context.UniversalCodeTypeMasters where c.IsActive select c).AsQueryable().ApplyFilters(filter.Filter);

            _query = _query.AsQueryable().ApplyFilters(filter.Filter);

            if (!string.IsNullOrWhiteSpace(filter.searchTerm))
            {
                var search = filter.searchTerm.Trim();
                _query = _query.Where(x => (x.Name != null && x.Name.Contains(search)));
            }

            if (filter.SortByColumn != null)
            {
                _query = _query.OrderBy($"{filter.SortByColumn} {(filter.SortOrder == "asc" ? "ascending" : "descending")}");
            }

            return await _query.Cast<object>().ToPagedAsync(filter);
        }

        public async Task<List<DropdwonSelector>> GetUniversalCodeTypeDropdown(string? searchTerm, int pageNo = 0, int pageSize = 20)
        {
            if (pageNo < 0) pageNo = 0;

            var _query = from a in _context.UniversalCodeTypeMasters where a.IsActive select a;

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
                Name = x.Name,
            })).ToListAsync();

            return data;
        }

        public async Task<bool> ExistsByName(string name)
        {
            return await _context.UniversalCodeTypeMasters.AnyAsync(x => x.Name == name && x.IsActive);
        }

        public async Task<bool> ExistsByNameAndNotId(string name, long Id)
        {
            return await _context.UniversalCodeTypeMasters.AnyAsync(x => x.Name == name && x.ID != Id && x.IsActive);
        }
    }
}
