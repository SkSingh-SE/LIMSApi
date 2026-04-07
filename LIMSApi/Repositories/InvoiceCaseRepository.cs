using System.Linq.Dynamic.Core;
using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Repositories
{
    public class InvoiceCaseRepository : IInvoiceCaseRepository
    {
        private readonly LIMSContext _context;
        private LoggedInUserDTO loggedInUser;

        public InvoiceCaseRepository(LIMSContext context)
        {
            _context = context;
            loggedInUser = LoggedInUserProvider.CurrentUser;
        }

        public async Task AddInvoiceCase(InvoiceCase model)
        {
            await _context.InvoiceCases.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteInvoiceCase(InvoiceCase model)
        {
            _context.InvoiceCases.Update(model);
            await _context.SaveChangesAsync();
        }

        public async Task<InvoiceCase?> GetInvoiceCaseById(long id)
        {
            return await _context.InvoiceCases.Include(y => y.InvoiceCasePrices).FirstOrDefaultAsync(x => x.ID == id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);
        }

        public async Task UpdateInvoiceCase(InvoiceCase model)
        {
            _context.InvoiceCases.Update(model);
            await _context.SaveChangesAsync();
        }

        public async Task<PagedResponse<object>> GetAllInvoiceCases(PageFilter filter)
        {
            var _query = from c in _context.InvoiceCases
                         join l in _context.LaboratoryTests on c.LaboratoryTestID equals l.ID into ltGroup
                         from lt in ltGroup.DefaultIfEmpty()
                         join fy in _context.FinancialYears on c.FinancialYearId equals fy.Id into fyGroup
                         from fy in fyGroup.DefaultIfEmpty()
                         where c.IsActive && c.CompanyCode == loggedInUser.CompanyCode
                         select new
                         {
                             c.ID,
                             c.FinancialYearId,
                             FinancialYear = fy != null ? fy.Year : "",
                             LaboratoryTest = lt != null ? lt.Name : "",
                             TierCount = _context.InvoiceCasePrices.Count(p => p.InvoiceCaseID == c.ID),
                             c.ModifiedOn,
                             Tiers = _context.InvoiceCasePrices
                                 .Where(p => p.InvoiceCaseID == c.ID)
                                 .Select(p => new { p.Name, p.Price, p.AliasName })
                                 .ToList()
                         };

            _query = _query.AsQueryable().ApplyFilters(filter.Filter);

            if (!string.IsNullOrWhiteSpace(filter.searchTerm))
            {
                var search = filter.searchTerm.Trim().ToLower();
                _query = _query.Where(x =>
                    x.FinancialYear.ToLower().Contains(search)
                    || x.LaboratoryTest.ToLower().Contains(search));
            }

            if (filter.SortByColumn != null)
            {
                _query = _query.OrderBy($"{filter.SortByColumn} {(filter.SortOrder == "asc" ? "ascending" : "descending")}");
            }

            return await _query.Cast<object>().ToPagedAsync(filter);
        }

        public async Task<List<DropdwonSelector>> GetInvoiceCaseDropdown(string? searchTerm, int pageNo = 0, int pageSize = 20)
        {
            if (pageNo < 0) pageNo = 0;

            var _query = from i in _context.InvoiceCases
                         join a in _context.InvoiceCasePrices on i.ID equals a.InvoiceCaseID
                         where i.IsActive && i.CompanyCode == loggedInUser.CompanyCode
                         select new
                         {
                             i.ID,
                             Name = a.Name
                         };

           
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                if (FilterHelper.IsExactIdSearch(searchTerm, out long exactId))
                {
                    _query = _query.Where(x => x.ID == exactId);
                }
                else
                {
                    var search = searchTerm.Trim().ToLower();
                    _query = _query.Where(x => (x.Name != null && x.Name.ToLower().Contains(search)));
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

        public async Task<bool> ExistsByFinancialAndName(long? financialYearId, string name)
        {
            return await _context.InvoiceCases.Include(y => y.InvoiceCasePrices).AnyAsync(x => x.FinancialYearId == financialYearId && x.InvoiceCasePrices.Any(p => p.Name == name) && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);
        }

        public async Task<bool> ExistsByFinancialYearAndTest(long? financialYearId, long laboratoryTestId)
        {
            return await _context.InvoiceCases.AnyAsync(x => x.FinancialYearId == financialYearId && x.LaboratoryTestID == laboratoryTestId && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);
        }

        public async Task<bool> ExistsByFinancialYearAndTestNotId(long? financialYearId, long laboratoryTestId, long excludeId)
        {
            return await _context.InvoiceCases.AnyAsync(x => x.FinancialYearId == financialYearId && x.LaboratoryTestID == laboratoryTestId && x.ID != excludeId && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);
        }
    }
}
