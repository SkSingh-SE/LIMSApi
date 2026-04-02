using System.Linq.Dynamic.Core;
using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Repositories
{
    public class CustomerPurchaseOrderRepository : ICustomerPurchaseOrderRepository
    {
        private readonly LIMSContext _context;
        private LoggedInUserDTO loggedInUser;

        public CustomerPurchaseOrderRepository(LIMSContext context)
        {
            _context = context;
            loggedInUser = LoggedInUserProvider.CurrentUser;
        }

        public async Task<CustomerPurchaseOrder> Add(CustomerPurchaseOrder model)
        {
            model.CompanyCode = loggedInUser.CompanyCode;
            model.RemainingAmount = model.POAmount;
            await _context.CustomerPurchaseOrders.AddAsync(model);
            await _context.SaveChangesAsync();
            return model;
        }

        public async Task<CustomerPurchaseOrder> Update(CustomerPurchaseOrder model)
        {
            model.ModifiedBy = loggedInUser.EmployeeID;
            model.ModifiedOn = DateTime.UtcNow;
            _context.CustomerPurchaseOrders.Update(model);
            await _context.SaveChangesAsync();
            return model;
        }

        public async Task<CustomerPurchaseOrder> Delete(long id)
        {
            var existing = await _context.CustomerPurchaseOrders
                .FirstOrDefaultAsync(x => x.ID == id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);
            if (existing == null)
                throw new InvalidOperationException("Purchase Order not found!");

            existing.IsActive = false;
            _context.CustomerPurchaseOrders.Update(existing);
            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<CustomerPurchaseOrder?> GetById(long id)
        {
            return await _context.CustomerPurchaseOrders
                .Include(x => x.Customer)
                .FirstOrDefaultAsync(x => x.ID == id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);
        }

        public async Task<PagedResponse<object>> GetAll(PageFilter filter)
        {
            var _query = from po in _context.CustomerPurchaseOrders
                         join c in _context.Customers on po.CustomerId equals c.ID
                         where po.IsActive && po.CompanyCode == loggedInUser.CompanyCode
                         select new
                         {
                             po.ID,
                             po.CustomerId,
                             CustomerName = c.Name,
                             po.PONumber,
                             po.PODate,
                             po.ValidUntil,
                             po.POAmount,
                             po.UtilizedAmount,
                             po.RemainingAmount,
                             po.Status,
                             po.Terms,
                             po.CreatedOn,
                             po.ModifiedOn
                         };

            _query = _query.AsQueryable().ApplyFilters(filter.Filter);

            if (!string.IsNullOrWhiteSpace(filter.searchTerm))
            {
                var search = filter.searchTerm.Trim().ToLower();
                _query = _query.Where(x =>
                    x.PONumber.ToLower().Contains(search) ||
                    x.CustomerName.ToLower().Contains(search));
            }

            if (filter.SortByColumn != null)
            {
                _query = _query.OrderBy($"{filter.SortByColumn} {(filter.SortOrder == "asc" ? "ascending" : "descending")}");
            }
            else
            {
                _query = _query.OrderByDescending(x => x.ID);
            }

            return await _query.Cast<object>().ToPagedAsync(filter);
        }

        public async Task<List<DropdwonSelector>> GetDropdown(long customerId, string? searchTerm, int pageNo = 0, int pageSize = 20)
        {
            if (pageNo < 0) pageNo = 0;

            var _query = _context.CustomerPurchaseOrders
                .Where(x => x.IsActive && x.CustomerId == customerId && x.Status == "Active");

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var search = searchTerm.Trim().ToLower();
                _query = _query.Where(x => x.PONumber.ToLower().Contains(search));
            }

            var skip = pageNo * pageSize;

            return await _query
                .Skip(skip).Take(pageSize)
                .Select(x => new DropdwonSelector
                {
                    Id = x.ID,
                    Name = x.PONumber,
                    AdditionalValues = new Dictionary<string, object>
                    {
                        { "RemainingAmount", x.RemainingAmount },
                        { "POAmount", x.POAmount },
                        { "ValidUntil", x.ValidUntil! }
                    }
                })
                .ToListAsync();
        }

        public async Task<bool> ExistsByPONumber(string poNumber, long customerId)
        {
            return await _context.CustomerPurchaseOrders
                .AnyAsync(x => x.PONumber == poNumber && x.CustomerId == customerId && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);
        }

        public async Task<bool> ExistsByPONumberAndNotId(string poNumber, long customerId, long id)
        {
            return await _context.CustomerPurchaseOrders
                .AnyAsync(x => x.PONumber == poNumber && x.CustomerId == customerId && x.ID != id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);
        }
    }
}
