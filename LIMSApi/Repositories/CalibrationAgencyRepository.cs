using System.Linq.Dynamic.Core;
using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Repositories
{
    public class CalibrationAgencyRepository : ICalibrationAgencyRepository
    {
        private readonly LIMSContext _context;
        private LoggedInUserDTO loggedInUser;

        public CalibrationAgencyRepository(LIMSContext context)
        {
            _context = context;
            loggedInUser = LoggedInUserProvider.CurrentUser;
        }

        public async Task AddCalibrationAgency(CalibrationAgencyMaster model)
        {
            model.CompanyCode = loggedInUser.CompanyCode;
            await _context.CalibrationAgencyMasters.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCalibrationAgency(long id)
        {
            var existingCalibrationAgency = await _context.CalibrationAgencyMasters.FirstOrDefaultAsync(x => x.ID == id && x.IsActive);
            if (existingCalibrationAgency != null)
            {
                existingCalibrationAgency.IsActive = false;
                existingCalibrationAgency.ModifiedOn = DateTime.UtcNow;
                _context.CalibrationAgencyMasters.Update(existingCalibrationAgency);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<CalibrationAgencyMaster?> GetCalibrationAgencyById(long id)
        {
            return await _context.CalibrationAgencyMasters.FirstOrDefaultAsync(x => x.ID == id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);
        }

        public async Task UpdateCalibrationAgency(CalibrationAgencyMaster model)
        {
            _context.CalibrationAgencyMasters.Update(model);
            await _context.SaveChangesAsync();
        }

        public async Task<PagedResponse<object>> GetAllCalibrationAgencys(PageFilter filter)
        {
            var _query = (from c in _context.CalibrationAgencyMasters where c.IsActive && c.CompanyCode == loggedInUser.CompanyCode select c).AsQueryable().ApplyFilters(filter.Filter);


            if (!string.IsNullOrWhiteSpace(filter.searchTerm))
            {
                var search = filter.searchTerm.Trim().ToLower();
                _query = _query.Where(x =>
                    (x.Name != null && x.Name.ToLower().Contains(search))
                    || (x.ContactPerson1 != null && x.ContactPerson1.ToLower().Contains(search))
                    || (x.ContactNo1 != null && x.ContactNo1.ToLower().Contains(search))
                    || (x.EmailId1 != null && x.EmailId1.ToLower().Contains(search))
                    || (x.Address != null && x.Address.ToLower().Contains(search))
                );
            }

            if (filter.SortByColumn != null)
            {
                _query = _query.OrderBy($"{filter.SortByColumn} {(filter.SortOrder == "asc" ? "ascending" : "descending")}");
            }

            return await _query.Cast<object>().ToPagedAsync(filter);
        }

        public async Task<List<DropdwonSelector>> GetCalibrationAgencyDropdown(string? searchTerm, int pageNo = 0, int pageSize = 20)
        {
            if (pageNo < 0) pageNo = 0;

            var _query = from a in _context.CalibrationAgencyMasters where a.IsActive && a.CompanyCode == loggedInUser.CompanyCode select a;

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var search = searchTerm.Trim().ToLower();
                _query = _query.Where(x =>  (x.Name != null && x.Name.ToLower().Contains(search))
                || x.ID.ToString().Contains(search));
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
            return await _context.CalibrationAgencyMasters.AnyAsync(x => x.Name == name && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);
        }

        public async Task<bool> ExistsByNameAndNotId(string name, long Id)
        {
            return await _context.CalibrationAgencyMasters.AnyAsync(x => x.Name == name && x.ID != Id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);
        }
    }
}
