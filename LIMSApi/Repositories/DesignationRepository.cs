using System.Linq.Dynamic.Core;
using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace LIMSApi.Repositories
{
    public class DesignationRepository : IDesignationRepository
    {
        private readonly LIMSContext _context;
        private LoggedInUserDTO loggedInUser;

        public DesignationRepository(LIMSContext context)
        {
            _context = context;
            loggedInUser = LoggedInUserProvider.CurrentUser;
        }

        public async Task AddDesignation(DesignationMaster model)
        {
            await _context.DesignationMasters.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteDesignation(long id)
        {
            var existingDesignation = await _context.DesignationMasters.FirstOrDefaultAsync(x => x.ID == id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);
            if (existingDesignation != null)
            {
                existingDesignation.IsActive = false;
                existingDesignation.ModifiedOn = DateTime.UtcNow;
                _context.DesignationMasters.Update(existingDesignation);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<DesignationMaster?> GetDesignationById(long id)
        {
            return await _context.DesignationMasters
                .Include(x => x.Role)
                .FirstOrDefaultAsync(x => x.ID == id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);
        }

        public async Task UpdateDesignation(DesignationMaster model)
        {
            _context.DesignationMasters.Update(model);
            await _context.SaveChangesAsync();
        }

        public async Task<PagedResponse<object>> GetAllDesignations(PageFilter filter)
        {
            var _query = (from c in _context.DesignationMasters
                          where c.IsActive && c.CompanyCode == loggedInUser.CompanyCode
                          join e in _context.EmployeeMasters on c.CreatedBy equals e.ID into emp
                          from eg in emp.DefaultIfEmpty()
                          join r in _context.RoleMasters on c.RoleID equals r.ID into role
                          from rg in role.DefaultIfEmpty()
                          select new
                          {
                              c.ID,
                              c.Name,
                              c.Description,
                              c.RoleID,
                              RoleName = rg != null ? rg.Name : null,
                              c.CreatedOn,
                              c.ModifiedOn,
                              CreatedBy = eg.Name

                          }).AsQueryable()
                            .ApplyFilters(filter.Filter);


            if (!string.IsNullOrWhiteSpace(filter.searchTerm))
            {
                var search = filter.searchTerm.Trim().ToLower();
                _query = _query.Where(x => (x.Description != null && x.Description.ToLower().Contains(search))
                                     || (x.Name != null && x.Name.ToLower().Contains(search)));
            }

            if (filter.SortByColumn != null)
            {
                _query = _query.OrderBy($"{filter.SortByColumn} {(filter.SortOrder == "asc" ? "ascending" : "descending")}");
            }

            return await _query.Cast<object>().ToPagedAsync(filter);
        }

        public async Task<List<DropdwonSelector>> GetDesignationDropdown(string? searchTerm, int pageNo = 0, int pageSize = 20)
        {
            if (pageNo < 0) pageNo = 0;

            var _query = from a in _context.DesignationMasters where a.IsActive && a.CompanyCode == loggedInUser.CompanyCode select a;

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var search = searchTerm.Trim().ToLower();
                _query = _query.Where(x => ( x.ID.ToString().Contains(search))
                                      || (x.Name != null && x.Name.ToLower().Contains(search)));
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
            return await _context.DesignationMasters.AnyAsync(x => x.Name == name && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);
        }

        public async Task<bool> ExistsByNameAndNotId(string name, long Id)
        {
            return await _context.DesignationMasters.AnyAsync(x => x.Name == name && x.ID != Id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);
        }
    }
}
