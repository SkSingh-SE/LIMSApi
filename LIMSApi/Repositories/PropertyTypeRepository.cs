using System.Linq.Dynamic.Core;
using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Repositories
{
    public class PropertyTypeRepository : IPropertyTypeRepository
    {
        private readonly LIMSContext _context;
        public PropertyTypeRepository(LIMSContext context) { _context = context; }

        public async Task AddPropertyType(PropertyTypeMaster model) { await _context.PropertyTypeMasters.AddAsync(model); await _context.SaveChangesAsync(); }

        public async Task DeletePropertyType(long id) {
            var existing = await _context.PropertyTypeMasters.FirstOrDefaultAsync(x => x.ID == id && x.IsActive);
            if (existing != null) { existing.IsActive = false; existing.ModifiedOn = DateTime.UtcNow; _context.PropertyTypeMasters.Update(existing); await _context.SaveChangesAsync(); }
        }

        public async Task<PropertyTypeMaster?> GetPropertyTypeById(long id) => await _context.PropertyTypeMasters.FirstOrDefaultAsync(x => x.ID == id && x.IsActive);

        public async Task UpdatePropertyType(PropertyTypeMaster model) { _context.PropertyTypeMasters.Update(model); await _context.SaveChangesAsync(); }

        public async Task<PagedResponse<object>> GetAllPropertyTypes(PageFilter filter) {
            var _query = (from c in _context.PropertyTypeMasters where c.IsActive select c).AsQueryable().ApplyFilters(filter.Filter);
            if (!string.IsNullOrWhiteSpace(filter.searchTerm)) { var search = filter.searchTerm.Trim(); _query = _query.Where(x => (x.Name != null && x.Name.Contains(search))); }
            if (filter.SortByColumn != null) { _query = _query.OrderBy($"{filter.SortByColumn} {(filter.SortOrder == "asc" ? "ascending" : "descending")}"); }
            return await _query.Cast<object>().ToPagedAsync(filter);
        }

        public async Task<List<DropdwonSelector>> GetPropertyTypeDropdown(string? searchTerm, int pageNo = 0, int pageSize = 20) {
            if (pageNo < 0) pageNo = 0;
            var _query = from a in _context.PropertyTypeMasters where a.IsActive select a;
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
            return await (_query.Skip(skip).Take(pageSize).Select(x => new DropdwonSelector { Id = x.ID, Name = x.Name })).ToListAsync();
        }

        public async Task<bool> ExistsByName(string name) => await _context.PropertyTypeMasters.AnyAsync(x => x.Name == name && x.IsActive);
        public async Task<bool> ExistsByNameAndNotId(string name, long Id) => await _context.PropertyTypeMasters.AnyAsync(x => x.Name == name && x.ID != Id && x.IsActive);
    }
}
