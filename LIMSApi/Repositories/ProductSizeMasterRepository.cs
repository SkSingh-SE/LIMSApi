using System.Linq.Dynamic.Core;
using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Repositories
{
    public class ProductSizeMasterRepository : IProductSizeMasterRepository
    {
        private readonly LIMSContext _context;
        private LoggedInUserDTO loggedInUser;

        public ProductSizeMasterRepository(LIMSContext context)
        {
            _context = context;
            loggedInUser = LoggedInUserProvider.CurrentUser;
        }

        public async Task AddProductSize(ProductSizeMaster model)
        {
            await _context.ProductSizeMasters.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteProductSize(ProductSizeMaster model)
        {
            _context.ProductSizeMasters.Update(model);
            await _context.SaveChangesAsync();
        }

        public async Task<ProductSizeMaster?> GetProductSizeById(long id)
        {
            return await _context.ProductSizeMasters
                .Include(x => x.ParameterUnit)
                .FirstOrDefaultAsync(x => x.ID == id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);
        }

        public async Task UpdateProductSize(ProductSizeMaster model)
        {
            _context.ProductSizeMasters.Update(model);
            await _context.SaveChangesAsync();
        }

        public async Task<PagedResponse<object>> GetAllProductSizes(PageFilter filter)
        {
            var _query = (from c in _context.ProductSizeMasters.Include(x => x.ParameterUnit)
                          where c.IsActive && c.CompanyCode == loggedInUser.CompanyCode
                          select c).AsQueryable().ApplyFilters(filter.Filter);

            if (!string.IsNullOrWhiteSpace(filter.searchTerm))
            {
                var search = filter.searchTerm.Trim();
                _query = _query.Where(x =>
                    (x.DisplayName != null && x.DisplayName.Contains(search))
                    || (x.SizeType != null && x.SizeType.Contains(search))
                    || (x.ParameterUnit != null && x.ParameterUnit.Name.Contains(search))
                );
            }
            if (filter.SortByColumn != null)
            {
                _query = _query.OrderBy($"{filter.SortByColumn} {(filter.SortOrder == "asc" ? "ascending" : "descending")}");
            }

            var projected = _query.Select(x => new
            {
                x.ID,
                x.SizeType,
                x.MinValue,
                x.MaxValue,
                x.ParameterUnitID,
                UnitName = x.ParameterUnit != null ? x.ParameterUnit.Name : null,
                x.DisplayName,
                x.CreatedBy,
                x.CreatedOn,
                x.ModifiedBy,
                x.ModifiedOn,
                x.IsActive
            });

            return await projected.Cast<object>().ToPagedAsync(filter);
        }

        public async Task<List<DropdwonSelector>> GetProductSizeDropdown(string? searchTerm, int pageNo = 0, int pageSize = 20)
        {
            if (pageNo < 0) pageNo = 0;

            var _query = from a in _context.ProductSizeMasters
                         where a.IsActive && a.CompanyCode == loggedInUser.CompanyCode
                         select a;

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                if (FilterHelper.IsExactIdSearch(searchTerm, out long exactId))
                {
                    _query = _query.Where(x => x.ID == exactId);
                }
                else
                {
                    var search = searchTerm.Trim();
                    _query = _query.Where(x => (x.DisplayName != null && x.DisplayName.Contains(search)));
                }
            }

            var skip = pageNo * pageSize;

            var data = await (_query.OrderBy(x => x.DisplayName).Skip(skip).Take(pageSize).Select(x => new DropdwonSelector
            {
                Id = x.ID,
                Name = x.DisplayName,
            })).ToListAsync();

            return data;
        }

        public async Task<bool> ExistsByName(string displayName)
        {
            return await _context.ProductSizeMasters.AnyAsync(x => x.DisplayName == displayName && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);
        }

        public async Task<bool> ExistsByNameAndNotId(string displayName, long id)
        {
            return await _context.ProductSizeMasters.AnyAsync(x => x.DisplayName == displayName && x.ID != id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);
        }
    }
}
