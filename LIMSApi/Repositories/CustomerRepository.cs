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
            var customer = await _context.Customers.AsNoTracking()
   .Include(x => x.ContactPersons)
   .Include(x => x.CustomerDispatchModes)
   .Include(x => x.CustomerCompanyCategories)
       .ThenInclude(ccc => ccc.CompanyCategory)
   .FirstOrDefaultAsync(x => x.ID == id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);
            return customer;

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
                              e.ModifiedOn,
                              e.CreatedOn
                          }).AsQueryable().ApplyFilters(filter.Filter);

            if (!string.IsNullOrWhiteSpace(filter.searchTerm))
            {
                var search = filter.searchTerm.Trim();
                _query = _query.Where(x =>
                    (x.Name != null && x.Name.Contains(search))
                    || x.CustomerType.Contains(search)
                    || x.PinCode.Contains(search)
                    || (x.GSTNo != null && x.GSTNo.Contains(search))
                    || x.SampleReturn.Contains(search)
                );
            }

            if (filter.SortByColumn != null)
            {
                _query = _query.OrderBy($"{filter.SortByColumn} {(filter.SortOrder == "asc" ? "ascending" : "descending")}");
            }

            return await _query.Cast<object>().ToPagedAsync(filter);
        }

        public async Task<List<DropdwonSelector>> GetCustomerDropdown(string? searchTerm, int pageNo = 0, int pageSize = 20)
        {
            if (pageNo < 0) pageNo = 0;

            var _query = from a in _context.Customers where a.IsActive && a.CompanyCode == loggedInUser.CompanyCode select a;

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
        public async Task<bool> ValidateDuplicateCustomer(string gst, long Id)
        {
            return await _context.Customers.AnyAsync(x => x.GSTNo == gst && x.ID != Id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);
        }
    }
}
