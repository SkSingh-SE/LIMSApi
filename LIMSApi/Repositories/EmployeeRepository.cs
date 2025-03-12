using System.Linq.Dynamic.Core;
using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly LIMSContext _context;

        public EmployeeRepository(LIMSContext context)
        {
            _context = context;
        }

        public async Task AddEmployee(EmployeeMaster model)
        {
            await _context.EmployeeMasters.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteEmployee(long id)
        {
            var existingEmployee = await _context.EmployeeMasters.FirstOrDefaultAsync(x => x.ID == id && x.IsActive);
            if (existingEmployee != null)
            {
                existingEmployee.IsActive = false;
                existingEmployee.ModifiedOn = DateTime.UtcNow;
                _context.EmployeeMasters.Update(existingEmployee);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<EmployeeMaster?> GetEmployeeById(long id)
        {
            return await _context.EmployeeMasters.FirstOrDefaultAsync(x => x.ID == id && x.IsActive);
        }

        public async Task UpdateEmployee(EmployeeMaster model)
        {
            _context.EmployeeMasters.Update(model);
            await _context.SaveChangesAsync();
        }

        public async Task<PagedResponse<object>> GetAllEmployees(PageFilter filter)
        {
            var _query = from e in _context.EmployeeMasters where e.IsActive 
                         join d in _context.DepartmentMasters on e.DepartmentID equals d.ID into dpGroup
                         from dp in dpGroup.DefaultIfEmpty()

                         join ds in _context.DesignationMasters on e.DesignationID equals ds.ID into dsGroup
                         from ds in dpGroup.DefaultIfEmpty()
                         select new
                         {
                             e.ID,
                             e.Name,
                             e.EmailId,
                             e.JoinDate,
                             e.BirthDate,
                             e.Gender,
                             e.DepartmentID,
                             DepartmentName = dp.Name,
                             e.DesignationID,
                             DesignationName = ds.Name
                         };

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
                _query = _query.Where(x => (x.Name != null && x.Name.ToLower().Contains(search)));
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

            return new PagedResponse<object>(items.Cast<object>().ToList(), totalRecords, filter.PageNumber, filter.PageSize);
        }

        public async Task<List<DropdwonSelector>> GetEmployeeDropdown(string? searchTerm, int pageNo = 0, int pageSize = 20)
        {
            if (pageNo < 0) pageNo = 0;

            var _query = from a in _context.EmployeeMasters where a.IsActive select a;

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

        public async Task<bool> ExistsByEmail(string email)
        {
            return await _context.EmployeeMasters.AnyAsync(x => x.EmailId == email && x.IsActive);
        }

        public async Task<bool> ExistsByEmailAndNotId(string email, long Id)
        {
            return await _context.EmployeeMasters.AnyAsync(x => x.EmailId == email && x.ID != Id && x.IsActive);
        }
    }
}
