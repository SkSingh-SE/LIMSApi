using System.Linq.Dynamic.Core;
using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Repositories
{
    public class DepartmentRepository : IDepartmentRepository
    {
        private readonly LIMSContext _context;

        public DepartmentRepository(LIMSContext context)
        {
            _context = context;
        }

        public async Task AddDepartment(DepartmentMaster model)
        {
            await _context.DepartmentMasters.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteDepartment(long id)
        {
            var existingDepartment = await _context.DepartmentMasters.FirstOrDefaultAsync(x => x.ID == id && x.IsActive);
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
            return await _context.DepartmentMasters.FirstOrDefaultAsync(x => x.ID == id && x.IsActive);
        }

        public async Task UpdateDepartment(DepartmentMaster model)
        {
            _context.DepartmentMasters.Update(model);
            await _context.SaveChangesAsync();
        }

        public async Task<PagedResponse<object>> GetAllDepartments(PageFilter filter)
        {
            var _query = from c in _context.DepartmentMasters where c.IsActive select c;

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
                _query = _query.Where(x => ( x.ID.ToString().Contains(search))
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

        public async Task<List<DropdwonSelector>> GetDepartmentDropdown(string? searchTerm, int pageNo = 0, int pageSize = 20)
        {
            if (pageNo < 0) pageNo = 0;

            var _query = from a in _context.DepartmentMasters where a.IsActive select a;

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var search = searchTerm.Trim().ToLower();
                _query = _query.Where(x =>  x.ID.ToString().Contains(search)
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
            return await _context.DepartmentMasters.AnyAsync(x => x.Name == name && x.IsActive);
        }

        public async Task<bool> ExistsByNameAndNotId(string name, long Id)
        {
            return await _context.DepartmentMasters.AnyAsync(x => x.Name == name && x.ID != Id && x.IsActive);
        }
    }
}
