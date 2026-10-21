using System.Linq.Dynamic.Core;
using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Repositories
{
    public class CoolingMediumRepository : ICoolingMediumRepository
    {
        private readonly LIMSContext _context;
        public CoolingMediumRepository(LIMSContext context) { _context = context; }

        public async Task AddCoolingMedium(CoolingMediumMaster model)
        {
            await _context.CoolingMediumMasters.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCoolingMedium(long id)
        {
            var existing = await _context.CoolingMediumMasters.FirstOrDefaultAsync(x => x.ID == id && x.IsActive);
            if (existing != null)
            {
                existing.IsActive = false;
                existing.ModifiedOn = DateTime.UtcNow;
                _context.CoolingMediumMasters.Update(existing);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<CoolingMediumMaster?> GetCoolingMediumById(long id)
        {
            return await _context.CoolingMediumMasters.FirstOrDefaultAsync(x => x.ID == id && x.IsActive);
        }

        public async Task UpdateCoolingMedium(CoolingMediumMaster model)
        {
            _context.CoolingMediumMasters.Update(model);
            await _context.SaveChangesAsync();
        }

        public async Task<PagedResponse<object>> GetAllCoolingMediums(PageFilter filter)
        {
            var baseQuery = from c in _context.CoolingMediumMasters
                            where c.IsActive
                            join empMod in _context.EmployeeMasters on c.ModifiedBy equals empMod.ID into empModJoin
                            from empMod in empModJoin.DefaultIfEmpty()
                            join empCre in _context.EmployeeMasters on c.CreatedBy equals empCre.ID into empCreJoin
                            from empCre in empCreJoin.DefaultIfEmpty()
                            select new
                            {
                                c.ID,
                                c.Name,
                                c.IsActive,
                                c.CompanyCode,
                                c.CreatedBy,
                                c.CreatedOn,
                                c.ModifiedBy,
                                ModifiedOn = c.ModifiedOn ?? c.CreatedOn,
                                CreatedByName = empCre != null ? empCre.Name : null,
                                ModifiedByName = empMod != null ? empMod.Name : (empCre != null ? empCre.Name : null)
                            };

            var _query = baseQuery.AsQueryable().ApplyFilters(filter.Filter);
            if (!string.IsNullOrWhiteSpace(filter.searchTerm))
            {
                var search = filter.searchTerm.Trim();
                _query = _query.Where(x => (x.Name != null && x.Name.Contains(search))
                    || (x.ModifiedByName != null && x.ModifiedByName.Contains(search))
                    || (x.CreatedByName != null && x.CreatedByName.Contains(search)));
            }
            if (filter.SortByColumn != null)
            {
                _query = _query.OrderBy($"{filter.SortByColumn} {(filter.SortOrder == "asc" ? "ascending" : "descending")}");
            }
            return await _query.Cast<object>().ToPagedAsync(filter);
        }

        public async Task<List<DropdwonSelector>> GetCoolingMediumDropdown(string? searchTerm, int pageNo = 0, int pageSize = 20)
        {
            if (pageNo < 0) pageNo = 0;
            var _query = from a in _context.CoolingMediumMasters where a.IsActive select a;
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
            var data = await (_query.Skip(skip).Take(pageSize).Select(x => new DropdwonSelector { Id = x.ID, Name = x.Name })).ToListAsync();
            return data;
        }

        public async Task<bool> ExistsByName(string name) => await _context.CoolingMediumMasters.AnyAsync(x => x.Name == name && x.IsActive);
        public async Task<bool> ExistsByNameAndNotId(string name, long Id) => await _context.CoolingMediumMasters.AnyAsync(x => x.Name == name && x.ID != Id && x.IsActive);
    }
}
