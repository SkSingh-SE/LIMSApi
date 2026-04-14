using System.Linq.Dynamic.Core;
using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Repositories
{
    public class InvoiceCaseConfigurationRepository : IInvoiceCaseConfigurationRepository
    {
        private readonly LIMSContext _context;
        private LoggedInUserDTO loggedInUser;

        public InvoiceCaseConfigurationRepository(LIMSContext context)
        {
            _context = context;
            loggedInUser = LoggedInUserProvider.CurrentUser;
        }

        public async Task AddInvoiceCaseConfiguration(InvoiceCaseConfiguration model)
        {
            await _context.InvoiceCaseConfigurations.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteInvoiceCaseConfiguration(InvoiceCaseConfiguration model)
        {
            _context.InvoiceCaseConfigurations.Update(model);
            await _context.SaveChangesAsync();
        }

        public async Task<InvoiceCaseConfiguration?> GetInvoiceCaseConfigurationById(long id)
        {
            return await _context.InvoiceCaseConfigurations.Include(y => y.AliasNames).FirstOrDefaultAsync(x => x.ID == id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);
        }

        public async Task UpdateInvoiceCaseConfiguration(InvoiceCaseConfiguration model)
        {
            _context.InvoiceCaseConfigurations.Update(model);
            await _context.SaveChangesAsync();
        }

        public async Task<PagedResponse<object>> GetAllInvoiceCaseConfigurations(PageFilter filter)
        {
            var _query = from c in _context.InvoiceCaseConfigurations where c.IsActive && c.CompanyCode == loggedInUser.CompanyCode select c;

            _query = _query.AsQueryable().ApplyFilters(filter.Filter);

            if (!string.IsNullOrWhiteSpace(filter.searchTerm))
            {
                var search = filter.searchTerm.Trim();
                _query = _query.Where(x => (x.Name != null && x.Name.Contains(search)));
            }

            if (filter.SortByColumn != null)
            {
                _query = _query.OrderBy($"{filter.SortByColumn} {(filter.SortOrder == "asc" ? "ascending" : "descending")}");
            }

            return await _query.Cast<object>().ToPagedAsync(filter);
        }

        public async Task<List<DropdwonSelector>> GetInvoiceCaseConfigurationDropdown(string? searchTerm, int pageNo = 0, int pageSize = 20)
        {
            if (pageNo < 0) pageNo = 0;

            // Main config name + alias names as separate searchable entries (all point to same ID)
            var mainQuery = from i in _context.InvoiceCaseConfigurations
                            where i.IsActive && i.CompanyCode == loggedInUser.CompanyCode
                            select new { i.ID, i.Name };

            var aliasQuery = from i in _context.InvoiceCaseConfigurations
                             join a in _context.InvoiceCaseAliasNames on i.ID equals a.InvoiceConfigurationID
                             where i.IsActive && i.CompanyCode == loggedInUser.CompanyCode
                                   && a.Name != null && a.Name != ""
                             select new { i.ID, Name = a.Name };

            var _query = mainQuery.Union(aliasQuery);

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
            return await _context.InvoiceCaseConfigurations.AnyAsync(x => x.Name == name && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);
        }

        public async Task<bool> ExistsByNameAndNotId(string name, long Id)
        {
            return await _context.InvoiceCaseConfigurations.AnyAsync(x => x.Name == name && x.ID != Id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);
        }
    }
}
