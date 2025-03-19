using System.Linq.Dynamic.Core;
using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Repositories
{
    public class TestMethodStandardRepository : ITestMethodStandardRepository
    {
        private readonly LIMSContext _context;
        private LoggedInUserDTO loggedInUser;

        public TestMethodStandardRepository(LIMSContext context)
        {
            _context = context;
            loggedInUser = LoggedInUserProvider.CurrentUser;
        }

        public async Task AddTestMethodStandard(TestMethodStandard model)
        {
            await _context.TestMethodStandards.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteTestMethodStandard(long id)
        {
            var existingTestMethodStandard = await _context.TestMethodStandards.FirstOrDefaultAsync(x => x.ID == id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);
            if (existingTestMethodStandard != null)
            {
                existingTestMethodStandard.IsActive = false;
                existingTestMethodStandard.ModifiedOn = DateTime.UtcNow;
                _context.TestMethodStandards.Update(existingTestMethodStandard);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<TestMethodStandard?> GetTestMethodStandardById(long id)
        {
            return await _context.TestMethodStandards.FirstOrDefaultAsync(x => x.ID == id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);
        }

        public async Task UpdateTestMethodStandard(TestMethodStandard model)
        {
            _context.TestMethodStandards.Update(model);
            await _context.SaveChangesAsync();
        }

        public async Task<PagedResponse<object>> GetAllTestMethodStandards(PageFilter filter)
        {
            var _query = from c in _context.TestMethodStandards where c.IsActive && c.CompanyCode == loggedInUser.CompanyCode select c;

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

        public async Task<List<DropdwonSelector>> GetTestMethodStandardDropdown(string? searchTerm, int pageNo = 0, int pageSize = 20)
        {
            if (pageNo < 0) pageNo = 0;

            var _query = from a in _context.TestMethodStandards where a.IsActive && a.CompanyCode == loggedInUser.CompanyCode select a;

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
            return await _context.TestMethodStandards.AnyAsync(x => x.Name == name && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);
        }

        public async Task<bool> ExistsByNameAndNotId(string name, long Id)
        {
            return await _context.TestMethodStandards.AnyAsync(x => x.Name == name && x.ID != Id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);
        }
    }
}
