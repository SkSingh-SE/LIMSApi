using System.Linq.Dynamic.Core;
using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Repositories
{
    public class ProductSpecificationRepository : IProductSpecificationRepository
    {
        private readonly LIMSContext _context;

        public ProductSpecificationRepository(LIMSContext context)
        {
            _context = context;
        }

        public async Task AddProductSpecification(ProductSpecification model)
        {
            await _context.ProductSpecifications.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteProductSpecification(long id)
        {
            var existingProductSpecification = await _context.ProductSpecifications.FirstOrDefaultAsync(x => x.ID == id && x.IsActive);
            if (existingProductSpecification != null)
            {
                existingProductSpecification.IsActive = false;
                existingProductSpecification.ModifiedOn = DateTime.UtcNow;
                _context.ProductSpecifications.Update(existingProductSpecification);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<ProductSpecification?> GetProductSpecificationById(long id)
        {
            return await _context.ProductSpecifications.FirstOrDefaultAsync(x => x.ID == id && x.IsActive);
        }

        public async Task UpdateProductSpecification(ProductSpecification model)
        {
            _context.ProductSpecifications.Update(model);
            await _context.SaveChangesAsync();
        }

        public async Task<PagedResponse<object>> GetAllProductSpecifications(PageFilter filter)
        {
            var _query = (from c in _context.ProductSpecifications where c.IsActive select c).AsQueryable().ApplyFilters(filter.Filter);


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

        public async Task<List<DropdwonSelector>> GetProductSpecificationDropdown(string? searchTerm, int pageNo = 0, int pageSize = 20)
        {
            if (pageNo < 0) pageNo = 0;

            var _query = from a in _context.ProductSpecifications where a.IsActive select a;

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
            return await _context.ProductSpecifications.AnyAsync(x => x.Name == name && x.IsActive);
        }

        public async Task<bool> ExistsByNameAndNotId(string name, long Id)
        {
            return await _context.ProductSpecifications.AnyAsync(x => x.Name == name && x.ID != Id && x.IsActive);
        }
    }
}
