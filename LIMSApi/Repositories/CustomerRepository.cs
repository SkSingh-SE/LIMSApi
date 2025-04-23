using System.Linq.Dynamic.Core;
using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly LIMSContext _context;
        private LoggedInUserDTO loggedInUser;

        public CustomerRepository(LIMSContext context)
        {
            _context = context;
            loggedInUser = LoggedInUserProvider.CurrentUser;
        }

        public async Task AddCustomer(Customer model)
        {
            await _context.Customers.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCustomer(Customer testGroup)
        {
            if (testGroup.ContactPersons != null && testGroup.ContactPersons.Any())
            {
                _context.ContactPersons.RemoveRange(testGroup.ContactPersons);
            }
            _context.Customers.Update(testGroup); 
            await _context.SaveChangesAsync();
        }

        public async Task<Customer?> GetCustomerById(long id)
        {
            return await _context.Customers
                .Include(c => c.ContactPersons)
                .Include(c => c.CustomerCompanyCategories)
                    .ThenInclude(ccc => ccc.CompanyCategory)
                    .Include(d => d.CustomerDispatchModes)
                    .ThenInclude(d => d.DispatchMode)
                .FirstOrDefaultAsync(x => x.ID == id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);
        }


        public async Task UpdateCustomer(Customer model)
        {
            _context.Customers.Update(model);
            await _context.SaveChangesAsync();

        }

        public async Task<PagedResponse<object>> GetAllCustomers(PageFilter filter)
        {
            var _query = (from e in _context.Customers
                          where e.IsActive && e.CompanyCode == loggedInUser.CompanyCode
                          select new
                          {
                              e.ID,
                              e.Name,
                              e.CustomerType,
                              e.PinCode,
                              e.Address,
                              e.GSTNo,
                              SampleReturn = e.SampleReturn ? "Yes" : "No",
                          }).AsQueryable().ApplyFilters(filter.Filter);

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

        public async Task<List<DropdwonSelector>> GetCustomerDropdown(string? searchTerm, int pageNo = 0, int pageSize = 20)
        {
            if (pageNo < 0) pageNo = 0;

            var _query = from a in _context.Customers where a.IsActive && a.CompanyCode == loggedInUser.CompanyCode select a;

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
            return await _context.Customers.AnyAsync(x => x.Name == name && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);
        }

        public async Task<bool> ExistsByNameAndNotId(string name, long Id)
        {
            return await _context.Customers.AnyAsync(x => x.Name == name && x.ID != Id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);
        }
    }
}
