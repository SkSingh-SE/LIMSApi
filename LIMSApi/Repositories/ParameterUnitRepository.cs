using System.Linq.Dynamic.Core;
using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Repositories
{
    public class ParameterUnitRepository : IParameterUnitRepository
    {
        private readonly LIMSContext _context;

        public ParameterUnitRepository(LIMSContext context)
        {
            _context = context;
        }

        public async Task AddParameterUnit(ParameterUnitMaster model)
        {
            await _context.ParameterUnitMasters.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteParameterUnit(long id)
        {
            var existingParameterUnit = await _context.ParameterUnitMasters.FirstOrDefaultAsync(x => x.ID == id && x.IsActive);
            if (existingParameterUnit != null)
            {
                existingParameterUnit.IsActive = false;
                existingParameterUnit.ModifiedOn = DateTime.UtcNow;
                _context.ParameterUnitMasters.Update(existingParameterUnit);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<ParameterUnitMaster?> GetParameterUnitById(long id)
        {
            return await _context.ParameterUnitMasters
                .Include(x => x.Equivalents)
                .FirstOrDefaultAsync(x => x.ID == id && x.IsActive);
        }

        public async Task UpdateParameterUnit(ParameterUnitMaster model)
        {
            _context.ParameterUnitMasters.Update(model);
            await _context.SaveChangesAsync();
        }

        public async Task<PagedResponse<object>> GetAllParameterUnits(PageFilter filter)
        {
            var _query = (from c in _context.ParameterUnitMasters where c.IsActive select c).AsQueryable().ApplyFilters(filter.Filter);


            if (!string.IsNullOrWhiteSpace(filter.searchTerm))
            {
                var search = filter.searchTerm.Trim();
                _query = _query.Where(x =>
                    (x.Name != null && x.Name.Contains(search))
                    || (x.ConversaionFactor != null && x.ConversaionFactor.Contains(search))
                );
            }
            if (filter.SortByColumn != null)
            {
                _query = _query.OrderBy($"{filter.SortByColumn} {(filter.SortOrder == "asc" ? "ascending" : "descending")}");
            }

            return await _query.Cast<object>().ToPagedAsync(filter);
        }

        public async Task<List<DropdwonSelector>> GetParameterUnitDropdown(string? searchTerm, int pageNo = 0, int pageSize = 20)
        {
            if (pageNo < 0) pageNo = 0;

            var _query = from a in _context.ParameterUnitMasters where a.IsActive select a;

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
            return await _context.ParameterUnitMasters.AnyAsync(x => x.Name == name && x.IsActive);
        }

        public async Task<bool> ExistsByNameAndNotId(string name, long Id)
        {
            return await _context.ParameterUnitMasters.AnyAsync(x => x.Name == name && x.ID != Id && x.IsActive);
        }
    }
}
