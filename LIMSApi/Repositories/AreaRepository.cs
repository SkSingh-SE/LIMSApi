using System.Linq.Dynamic.Core;
using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Repositories
{
    public class AreaRepository : IAreaRepository
    {
        private readonly LIMSContext _context;
        private LoggedInUserDTO loggedInUser;

        public AreaRepository(LIMSContext context)
        {
            _context = context;
            loggedInUser = LoggedInUserProvider.CurrentUser;
        }

        public async Task<AreaMaster> AddArea(AreaMaster model)
        {
            model.CompanyCode = model.CompanyCode ?? loggedInUser.CompanyCode;
            model.CreatedBy = loggedInUser.EmployeeID;
            await _context.AreaMasters.AddAsync(model);
            await _context.SaveChangesAsync();
            return model;
        }

        public async Task DeleteArea(long id)
        {
            var existingArea = await _context.AreaMasters.FirstOrDefaultAsync(x => x.ID == id && x.IsActive);
            if (existingArea != null)
            {
                existingArea.IsActive = false;
                existingArea.ModifiedOn = DateTime.UtcNow;
                _context.AreaMasters.Update(existingArea);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<AreaMaster?> GetAreaById(long id)
        {
            return await _context.AreaMasters.Include(x => x.City).ThenInclude(s => s.State).ThenInclude(c => c.Country).FirstOrDefaultAsync(x => x.ID == id && x.IsActive);
        }

        public async Task UpdateArea(AreaMaster model)
        {
            _context.AreaMasters.Update(model);
            await _context.SaveChangesAsync();
        }

        public async Task<PagedResponse<AreaMaster>> GetAllAreas(PageFilter filter)
        {
            var _query = from c in _context.AreaMasters where c.IsActive select c;

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

            return new PagedResponse<AreaMaster>(items, totalRecords, filter.PageNumber, filter.PageSize);
        }

        public async Task<List<AreaDropdownDTO>> GetAreaWithPincode(string pincode)
        {
            var data = await (_context.AreaMasters.Include(c => c.City).ThenInclude(s => s.State).ThenInclude(ci => ci.Country)
                .Where(x => x.IsActive && x.Pincode == pincode)
                .Select(x => new AreaDropdownDTO
                {
                    AreaId = x.ID,
                    AreaName = x.Name,
                    CityId = x.CityID,
                    CityName = x.City.Name,
                    StateId = x.City.StateID,
                    StateName = x.City.State.Name,
                    CountryId = x.City.State.CountryID,
                    CountryName = x.City.State.Country.Name
                })
                .ToListAsync());

            return data;
        }

        public async Task<bool> ExistsByName(string name)
        {
            return await _context.AreaMasters.AnyAsync(x => x.Name == name && x.IsActive);
        }
        public async Task<AreaMaster> GetAreaByName(string name)
        {
            return await _context.AreaMasters.FirstOrDefaultAsync(x => x.Name == name && x.IsActive);
        }

        public async Task<bool> ExistsByNameAndNotId(string name, long Id)
        {
            return await _context.AreaMasters.AnyAsync(x => x.Name == name && x.ID != Id && x.IsActive);
        }
    }
}
