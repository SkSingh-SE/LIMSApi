using System.Diagnostics.Metrics;
using System.Linq;
using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using LIMSApi.Helpers;

namespace LIMSApi.Repositories
{
    public class CountryRepository : ICountryRepository
    {
        private readonly LIMSContext _context;
        private LoggedInUserDTO loggedInUser;
        public CountryRepository(LIMSContext context)
        {
            _context = context;
            loggedInUser = LoggedInUserProvider.CurrentUser;
        }

        public async Task AddCountry(CountryMaster country)
        {
            country.CompanyCode = country.CompanyCode ?? loggedInUser.CompanyCode;
            country.CreatedBy = loggedInUser.EmployeeID;
            await _context.CountryMasters.AddAsync(country);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCountry(long id)
        {
            var existingCountry = await _context.CountryMasters.FirstOrDefaultAsync(x => x.ID == id && x.IsActive);
            if (existingCountry != null)
            {
                existingCountry.IsActive = false;
                existingCountry.ModifiedOn = DateTime.UtcNow;
                _context.CountryMasters.Update(existingCountry);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<CountryMaster> GetCountryById(long id)
        {
            return await _context.CountryMasters.FirstOrDefaultAsync(x => x.ID == id && x.IsActive);
        }

        public async Task UpdateCountry(CountryMaster country)
        {
            _context.CountryMasters.Update(country);
            await _context.SaveChangesAsync();
        }

        public async Task<PagedResponse<object>> GetAllCountries(PageFilter filter)
        {
            var _query = from c in _context.CountryMasters where c.IsActive select c;

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
                                     || (x.Name != null && x.Name.ToLower().Contains(search))
                                     );
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

        public async Task<bool> ExistsByName(string name)
        {
            return await _context.CountryMasters.AnyAsync(x => x.Name == name && x.IsActive);
        }
        public async Task<CountryMaster?> GetByName(string name)
        {
            return await _context.CountryMasters.FirstOrDefaultAsync(x => x.Name == name && x.IsActive);
        }
        public async Task<bool> ExistsByNameAndNotId(string name, long id)
        {
            return await _context.CountryMasters.AnyAsync(x => x.Name == name && x.ID != id && x.IsActive);
        }
    }
}
