using System.Linq.Dynamic.Core;
using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Repositories
{
    public class PriceDimensionTypeRepository : IPriceDimensionTypeRepository
    {
        private readonly LIMSContext _context;
        private LoggedInUserDTO loggedInUser;

        public PriceDimensionTypeRepository(LIMSContext context)
        {
            _context = context;
            loggedInUser = LoggedInUserProvider.CurrentUser;
        }

        public async Task<PriceDimensionType> AddPriceDimensionType(PriceDimensionType model)
        {
            model.CompanyCode = loggedInUser.CompanyCode;
            await _context.PriceDimensionTypes.AddAsync(model);
            await _context.SaveChangesAsync();
            return model;
        }

        public async Task<PriceDimensionType> DeletePriceDimensionType(long id)
        {
            var existing = await _context.PriceDimensionTypes.FirstOrDefaultAsync(x => x.ID == id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);
            if (existing != null)
            {
                existing.IsActive = false;
                _context.PriceDimensionTypes.Update(existing);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new InvalidOperationException("Price Dimension Type not found!");
            }
            return existing;
        }

        public async Task<PriceDimensionType?> GetPriceDimensionTypeById(long id)
        {
            return await _context.PriceDimensionTypes.FirstOrDefaultAsync(x => x.ID == id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);
        }

        public async Task<PriceDimensionType> UpdatePriceDimensionType(PriceDimensionType model)
        {
            _context.PriceDimensionTypes.Update(model);
            await _context.SaveChangesAsync();
            return model;
        }

        public async Task<PagedResponse<object>> GetAllPriceDimensionTypes(PageFilter filter)
        {
            var _query = from c in _context.PriceDimensionTypes where c.IsActive && c.CompanyCode == loggedInUser.CompanyCode select c;

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

            return await _query.Cast<object>().ToPagedAsync(filter);
        }

        public async Task<List<DropdwonSelector>> GetPriceDimensionTypeDropdown(string? searchTerm, int pageNo = 0, int pageSize = 20)
        {
            if (pageNo < 0) pageNo = 0;

            var _query = from a in _context.PriceDimensionTypes where a.IsActive select a;

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

            var data = await (_query.OrderBy(x => x.SortOrder).Skip(skip).Take(pageSize).Select(x => new DropdwonSelector
            {
                Id = x.ID,
                Name = x.Name,
            })).ToListAsync();

            return data;
        }

        public async Task<bool> ExistsByName(string name)
        {
            return await _context.PriceDimensionTypes.AnyAsync(x => x.Name == name && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);
        }

        public async Task<bool> ExistsByNameAndNotId(string name, long id)
        {
            return await _context.PriceDimensionTypes.AnyAsync(x => x.Name == name && x.ID != id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);
        }
    }
}
