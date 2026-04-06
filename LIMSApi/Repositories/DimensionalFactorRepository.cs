using System.Linq.Dynamic.Core;
using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using Microsoft.EntityFrameworkCore;


namespace LIMSApi.Repositories
{
    public class DimensionalFactorRepository : IDimensionalFactorRepository
    {
        private readonly LIMSContext _context;

        public DimensionalFactorRepository(LIMSContext context)
        {
            _context = context;
        }

        public async Task AddDimensionalFactor(DimensionalFactorMaster model)
        {
            await _context.DimensionalFactorMasters.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteDimensionalFactor(long id)
        {
            var existingDimensionalFactor = await _context.DimensionalFactorMasters.FirstOrDefaultAsync(x => x.ID == id && x.IsActive);
            if (existingDimensionalFactor != null)
            {
                existingDimensionalFactor.IsActive = false;
                existingDimensionalFactor.ModifiedOn = DateTime.UtcNow;
                _context.DimensionalFactorMasters.Update(existingDimensionalFactor);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<DimensionalFactorMaster?> GetDimensionalFactorById(long id)
        {
            return await _context.DimensionalFactorMasters
                .Include(x => x.ParameterUnit)
                .Include(x => x.DefaultTestMethod)
                .Include(x => x.ApplicableForms).ThenInclude(af => af.ProductForm)
                .FirstOrDefaultAsync(x => x.ID == id && x.IsActive);
        }

        public async Task UpdateDimensionalFactor(DimensionalFactorMaster model)
        {
            _context.DimensionalFactorMasters.Update(model);
            await _context.SaveChangesAsync();
        }

        public async Task<PagedResponse<object>> GetAllDimensionalFactors(PageFilter filter)
        {
            var _query = _context.DimensionalFactorMasters
                .Where(c => c.IsActive)
                .Include(x => x.ParameterUnit)
                .Include(x => x.DefaultTestMethod)
                .Include(x => x.ApplicableForms).ThenInclude(af => af.ProductForm)
                .AsQueryable()
                .ApplyFilters(filter.Filter);

            if (!string.IsNullOrWhiteSpace(filter.searchTerm))
            {
                var search = filter.searchTerm.Trim().ToLower();
                _query = _query.Where(x => (x.Name != null && x.Name.ToLower().Contains(search)) || (x.Code != null && x.Code.ToLower().Contains(search)));
            }

            if (filter.SortByColumn != null)
            {
                _query = _query.OrderBy($"{filter.SortByColumn} {(filter.SortOrder == "asc" ? "ascending" : "descending")}");
            }

            return await _query.Cast<object>().ToPagedAsync(filter);
        }

        public async Task<List<DropdwonSelector>> GetDimensionalFactorDropdown(string? searchTerm, int pageNo = 0, int pageSize = 20)
        {
            if (pageNo < 0) pageNo = 0;

            var _query = from a in _context.DimensionalFactorMasters where a.IsActive select a;

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                if (FilterHelper.IsExactIdSearch(searchTerm, out long exactId))
                {
                    _query = _query.Where(x => x.ID == exactId);
                }
                else
                {
                    var search = searchTerm.Trim().ToLower();
                    _query = _query.Where(x => (x.Name != null && x.Name.ToLower().Contains(search)));
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
            return await _context.DimensionalFactorMasters.AnyAsync(x => x.Name == name && x.IsActive);
        }

        public async Task<bool> ExistsByNameAndNotId(string name, long Id)
        {
            return await _context.DimensionalFactorMasters.AnyAsync(x => x.Name == name && x.ID != Id && x.IsActive);
        }

        public async Task<bool> ExistsByCode(string code)
        {
            return await _context.DimensionalFactorMasters.AnyAsync(x => x.Code == code && x.IsActive);
        }

        public async Task<bool> ExistsByCodeAndNotId(string code, long id)
        {
            return await _context.DimensionalFactorMasters.AnyAsync(x => x.Code == code && x.ID != id && x.IsActive);
        }
    }
}
