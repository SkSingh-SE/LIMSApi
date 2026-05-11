using System.Linq.Dynamic.Core;
using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Repositories
{
    public class ProductConditionRepository : IProductConditionRepository
    {
        private readonly LIMSContext _context;

        public ProductConditionRepository(LIMSContext context)
        {
            _context = context;
        }

        public async Task AddProductCondition(ProductConditionMaster model)
        {
            await _context.ProductConditionMasters.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteProductCondition(long id)
        {
            var existingProductCondition = await _context.ProductConditionMasters.FirstOrDefaultAsync(x => x.ID == id && x.IsActive);
            if (existingProductCondition != null)
            {
                existingProductCondition.IsActive = false;
                existingProductCondition.ModifiedOn = DateTime.UtcNow;
                _context.ProductConditionMasters.Update(existingProductCondition);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<ProductConditionMaster?> GetProductConditionById(long id)
        {
            return await _context.ProductConditionMasters
                .Include(x => x.ProductConditionCategory)
                .Include(x => x.LinkedHeatTreatment)
                .Include(x => x.PropertiesCaptured).ThenInclude(pc => pc.PropertyType)
                .FirstOrDefaultAsync(x => x.ID == id && x.IsActive);
        }

        public async Task UpdateProductCondition(ProductConditionMaster model)
        {
            _context.ProductConditionMasters.Update(model);
            await _context.SaveChangesAsync();
        }

        public async Task<PagedResponse<object>> GetAllProductConditions(PageFilter filter)
        {
            var _query = (from c in _context.ProductConditionMasters where c.IsActive select c).AsQueryable().ApplyFilters(filter.Filter);


            if (!string.IsNullOrWhiteSpace(filter.searchTerm))
            {
                var search = filter.searchTerm.Trim();
                _query = _query.Where(x => (x.Name != null && x.Name.Contains(search)) || (x.Code != null && x.Code.Contains(search)));
            }

            if (filter.SortByColumn != null)
            {
                _query = _query.OrderBy($"{filter.SortByColumn} {(filter.SortOrder == "asc" ? "ascending" : "descending")}");
            }

            return await _query.Cast<object>().ToPagedAsync(filter);
        }

        public async Task<List<DropdwonSelector>> GetProductConditionDropdown(string? searchTerm, int pageNo = 0, int pageSize = 20)
        {
            if (pageNo < 0) pageNo = 0;

            var _query = from a in _context.ProductConditionMasters where a.IsActive select a;

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
            return await _context.ProductConditionMasters.AnyAsync(x => x.Name == name && x.IsActive);
        }

        public async Task<bool> ExistsByNameAndNotId(string name, long Id)
        {
            return await _context.ProductConditionMasters.AnyAsync(x => x.Name == name && x.ID != Id && x.IsActive);
        }

        public async Task<bool> ExistsByCode(string code)
        {
            return await _context.ProductConditionMasters.AnyAsync(x => x.Code == code && x.IsActive);
        }

        public async Task<bool> ExistsByCodeAndNotId(string code, long id)
        {
            return await _context.ProductConditionMasters.AnyAsync(x => x.Code == code && x.ID != id && x.IsActive);
        }
    }
}
