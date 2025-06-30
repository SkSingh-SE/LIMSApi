using System.Linq.Dynamic.Core;
using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Repositories
{
    public class TestGroupRepository : ITestGroupRepository
    {
        private readonly LIMSContext _context;
        private LoggedInUserDTO loggedInUser;

        public TestGroupRepository(LIMSContext context)
        {
            _context = context;
            loggedInUser = LoggedInUserProvider.CurrentUser;
        }

        public async Task AddTestGroup(TestGroup model)
        {
            //model.CreatedOn = DateTime.UtcNow;
            //model.CreatedBy = loggedInUser.EmployeeID;
            //model.CompanyCode = loggedInUser.CompanyCode;
            //await _context.TestGroups.AddAsync(model);
            //await _context.SaveChangesAsync();
            //if (model.TestGroupMappings != null && model.TestGroupMappings.Any())
            //{
            //    foreach (var mapping in model.TestGroupMappings)
            //    {
            //        mapping.TestGroupID = model.ID;

            //        if (_context.TestGroupMappings.Any(x => x.ID == mapping.ID))
            //        {
            //            _context.TestGroupMappings.Update(mapping);
            //        }
            //        else
            //        {
            //            _context.TestGroupMappings.Add(mapping);
            //        }
            //    }

            //    await _context.SaveChangesAsync();
            //}
            foreach (var mapping in model.TestGroupMappings)
            {
                mapping.TestGroupID = model.ID; 
            }
            await _context.TestGroups.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteTestGroup(TestGroup testGroup)
        {
            if (testGroup.TestGroupMappings != null && testGroup.TestGroupMappings.Any())
            {
                _context.TestGroupMappings.RemoveRange(testGroup.TestGroupMappings);
            }
            _context.TestGroups.Update(testGroup); 
            await _context.SaveChangesAsync();
        }

        public async Task<TestGroup?> GetTestGroupById(long id)
        {
            return await _context.TestGroups.Include(tg => tg.TestGroupMappings).FirstOrDefaultAsync(x => x.ID == id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);
        }

        public async Task UpdateTestGroup(TestGroup model)
        {
            _context.TestGroups.Update(model);
            //if(model.TestGroupMappings != null && model.TestGroupMappings.Any())
            //{
            //    foreach (var mapping in model.TestGroupMappings)
            //    {
            //        _context.TestGroupMappings.Update(mapping);
            //    }
            //}
            await _context.SaveChangesAsync();

        }

        public async Task<PagedResponse<object>> GetAllTestGroups(PageFilter filter)
        {
            var _query = from c in _context.TestGroups where c.IsActive && c.CompanyCode == loggedInUser.CompanyCode select c;

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

            // Total Records Count
            int totalRecords = await _query.CountAsync();

            // Apply Pagination
            var items = await _query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return new PagedResponse<object>(items.Cast<object>().ToList(), totalRecords, filter.PageNumber, filter.PageSize);
        }

        public async Task<List<DropdwonSelector>> GetTestGroupDropdown(string? searchTerm, int pageNo = 0, int pageSize = 20)
        {
            if (pageNo < 0) pageNo = 0;

            var _query = from a in _context.TestGroups where a.IsActive && a.CompanyCode == loggedInUser.CompanyCode select a;

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var search = searchTerm.Trim().ToLower();
                _query = _query.Where(x =>  (x.Name != null && x.Name.ToLower().Contains(search)));
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
            return await _context.TestGroups.AnyAsync(x => x.Name == name && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);
        }

        public async Task<bool> ExistsByNameAndNotId(string name, long Id)
        {
            return await _context.TestGroups.AnyAsync(x => x.Name == name && x.ID != Id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);
        }
    }
}
