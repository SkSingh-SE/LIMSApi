using System.Linq.Dynamic.Core;
using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Repositories
{
    public class CurrencyRepository : ICurrencyRepository
    {
        private readonly LIMSContext _context;

        public CurrencyRepository(LIMSContext context)
        {
            _context = context;
        }

        public async Task AddCurrency(CurrencyMaster model)
        {
            await _context.CurrencyMasters.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCurrency(long id)
        {
            var existingCurrency = await _context.CurrencyMasters.FirstOrDefaultAsync(x => x.ID == id && x.IsActive);
            if (existingCurrency != null)
            {
                existingCurrency.IsActive = false;
                existingCurrency.ModifiedOn = DateTime.UtcNow;
                _context.CurrencyMasters.Update(existingCurrency);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<CurrencyMaster?> GetCurrencyById(long id)
        {
            return await _context.CurrencyMasters.FirstOrDefaultAsync(x => x.ID == id && x.IsActive);
        }

        public async Task UpdateCurrency(CurrencyMaster model)
        {
            _context.CurrencyMasters.Update(model);
            await _context.SaveChangesAsync();
        }

        public async Task<PagedResponse<CurrencyMaster>> GetAllCurrencys(PageFilter filter)
        {
            var _query = from c in _context.CurrencyMasters where c.IsActive select c;

            if (filter.Filters != null)
            {
                foreach (var filterItem in filter.Filters)
                {
                    if (string.IsNullOrWhiteSpace(filterItem.Value))
                    {
                        continue;
                    }
                    var propertyName = filterItem.Key;
                    var value = filterItem.Value;

                    _query = _query.Where($"{propertyName}.Contains(@0)", value);
                }
            }

            if (!string.IsNullOrWhiteSpace(filter.searchTerm))
            {
                var search = filter.searchTerm.Trim().ToLower();
                _query = _query.Where(x => (x.Code != null && x.Code.ToLower().Contains(search))
                                     || (x.Name != null && x.Name.ToLower().Contains(search)));
            }

            if (filter.SortBy != null && filter.SortBy.Any())
            {
                var sortingExpressions = filter.SortBy
                   .Select(s => $"{s.Key} {(s.Value ? "descending" : "ascending")}");
                string orderByString = string.Join(", ", sortingExpressions);

                _query = _query.OrderBy(orderByString);
            }

            // Total Records Count
            int totalRecords = await _query.CountAsync();

            // Apply Pagination
            var items = await _query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return new PagedResponse<CurrencyMaster>(items, totalRecords, filter.PageNumber, filter.PageSize);
        }

        public async Task<List<DropdwonSelector>> GetCurrencyDropdown(string? searchTerm, int pageNo = 0, int pageSize = 20)
        {
            if (pageNo < 0) pageNo = 0;

            var _query = from a in _context.CurrencyMasters where a.IsActive select a;

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var search = searchTerm.Trim().ToLower();
                _query = _query.Where(x => (x.Code != null && x.Code.ToLower().Contains(search))
                                      || (x.Name != null && x.Name.ToLower().Contains(search)));
            }

            var skip = pageNo * pageSize;

            var data = await (_query.Skip(skip).Take(pageSize).Select(x => new DropdwonSelector
            {
                Id = x.ID,
                Name = x.Code != null ? $"{x.Name}-({x.Code})" : $"{x.Name}",
            })).ToListAsync();

            return data;
        }

        public async Task<bool> ExistsByName(string name)
        {
            return await _context.CurrencyMasters.AnyAsync(x => x.Name == name && x.IsActive);
        }

        public async Task<bool> ExistsByNameAndNotId(string name, long Id)
        {
            return await _context.CurrencyMasters.AnyAsync(x => x.Name == name && x.ID != Id && x.IsActive);
        }
    }
}

