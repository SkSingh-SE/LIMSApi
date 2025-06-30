using System.Linq.Dynamic.Core;
using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Repositories
{
    public class CityRepository : ICityRepository
    {
        private readonly LIMSContext _context;
        private LoggedInUserDTO loggedInUser;

        public CityRepository(LIMSContext context)
        {
            _context = context;
            loggedInUser = LoggedInUserProvider.CurrentUser;
        }

        public async Task AddCity(CityMaster model)
        {
            model.CompanyCode = model.CompanyCode ?? loggedInUser.CompanyCode;
            model.CreatedBy = loggedInUser.EmployeeID;
            await _context.CityMasters.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCity(long id)
        {
            var existingCity = await _context.CityMasters.FirstOrDefaultAsync(x => x.ID == id && x.IsActive);
            if (existingCity != null)
            {
                existingCity.IsActive = false;
                existingCity.ModifiedOn = DateTime.UtcNow;
                _context.CityMasters.Update(existingCity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<CityMaster> GetCityById(long id)
        {
            return await _context.CityMasters.Include(x => x.State).ThenInclude(y => y.Country).FirstOrDefaultAsync(x => x.ID == id && x.IsActive);
        }

        public async Task UpdateCity(CityMaster state)
        {
            _context.CityMasters.Update(state);
            await _context.SaveChangesAsync();
        }

        public async Task<PagedResponse<object>> GetAllCities(PageFilter filter)
        {
            var _query = from s in _context.CityMasters where s.IsActive select s;

            _query = _query.AsQueryable().ApplyFilters(filter.Filter);

            if (!string.IsNullOrWhiteSpace(filter.searchTerm))
            {
                var search = filter.searchTerm.Trim().ToLower();
                _query = _query.Where(x => (x.Code != null && x.Code.ToLower().Contains(search))
                                     || (x.Name != null && x.Name.ToLower().Contains(search)));
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
            return await _context.CityMasters.AnyAsync(x => x.Name == name && x.IsActive);
        }
        public async Task<CityMaster?> GetByName(string name)
        {
            return await _context.CityMasters.FirstOrDefaultAsync(x => x.Name == name && x.IsActive);
        }

        public async Task<bool> ExistsByNameAndNotId(string name, long id)
        {
            return await _context.CityMasters.AnyAsync(x => x.Name == name && x.ID != id && x.IsActive);
        }
    }
}
