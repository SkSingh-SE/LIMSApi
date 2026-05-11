using System.Linq.Dynamic.Core;
using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Repositories
{
    [Authorize]
    public class DepartmentRepository : IDepartmentRepository
    {
        private readonly LIMSContext _context;
        private LoggedInUserDTO loggedInUser;

        public DepartmentRepository(LIMSContext context)
        {
            _context = context;
            loggedInUser = LoggedInUserProvider.CurrentUser;
        }

        public async Task AddDepartment(DepartmentMaster model)
        {
            await _context.DepartmentMasters.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteDepartment(long id)
        {
            var existingDepartment = await _context.DepartmentMasters.FirstOrDefaultAsync(x => x.ID == id && x.IsActive && loggedInUser.CompanyCode == x.CompanyCode);
            if (existingDepartment != null)
            {
                existingDepartment.IsActive = false;
                existingDepartment.ModifiedOn = DateTime.UtcNow;
                _context.DepartmentMasters.Update(existingDepartment);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<DepartmentMaster?> GetDepartmentById(long id)
        {
            return await _context.DepartmentMasters.FirstOrDefaultAsync(x => x.ID == id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);
        }

        public async Task UpdateDepartment(DepartmentMaster model)
        {
            _context.DepartmentMasters.Update(model);
            await _context.SaveChangesAsync();
        }

        public async Task<PagedResponse<object>> GetAllDepartments(PageFilter filter)
        {
            var _query = (from c in _context.DepartmentMasters
                          where c.IsActive && c.CompanyCode == loggedInUser.CompanyCode
                          join e in _context.EmployeeMasters on c.CreatedBy equals e.ID into emp
                          from eg in emp.DefaultIfEmpty()
                          select new
                          {
                              c.ID,
                              c.Name,
                              c.Description,
                              c.IsChemical,
                              c.CreatedOn,
                              c.ModifiedOn,
                              CreatedBy = eg.Name

                          }).AsQueryable()
                            .ApplyFilters(filter.Filter);

            if (!string.IsNullOrWhiteSpace(filter.searchTerm))
            {
                var search = filter.searchTerm.Trim();
                _query = _query.Where(x =>
                    (x.Name != null && x.Name.Contains(search))
                    || (x.Description != null && x.Description.Contains(search))
                    || (x.CreatedBy != null && x.CreatedBy.Contains(search))
                );
            }

            if (filter.SortByColumn != null)
            {
                _query = _query.OrderBy($"{filter.SortByColumn} {(filter.SortOrder == "asc" ? "ascending" : "descending")}");
            }

            return await _query.Cast<object>().ToPagedAsync(filter);
        }

        public async Task<List<DropdwonSelector>> GetDepartmentDropdown(string? searchTerm, int pageNo = 0, int pageSize = 20)
        {
            if (pageNo < 0) pageNo = 0;

            var _query = from a in _context.DepartmentMasters where a.IsActive && a.CompanyCode == loggedInUser.CompanyCode select a;

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
                AdditionalValues = new Dictionary<string, object> { { "isChemical", x.IsChemical } }
            })).ToListAsync();

            return data;
        }

        public async Task<bool> ExistsByName(string name)
        {
            return await _context.DepartmentMasters.AnyAsync(x => x.Name == name && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);
        }

        public async Task<bool> ExistsByNameAndNotId(string name, long Id)
        {
            return await _context.DepartmentMasters.AnyAsync(x => x.Name == name && x.ID != Id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);
        }
    }
}
