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
                var search = filter.searchTerm.Trim();
                _query = _query.Where(x =>
                    x.FinancialYear.Contains(search)
                    || x.LaboratoryTest.Contains(search));
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

        public async Task<List<InvoiceCase>> GetByLabTest(long laboratoryTestId)
        {
            return await _context.InvoiceCases
                .Include(x => x.InvoiceCasePrices)
                .Include(x => x.LaboratoryTest)
                .Include(x => x.FinancialYearEntity)
                .Where(x => x.LaboratoryTestID == laboratoryTestId && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode)
                .OrderByDescending(x => x.EffectiveFrom)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task SaveAllVersions(long laboratoryTestId, List<InvoiceCase> incoming)
        {
            var existing = await _context.InvoiceCases
                .Include(x => x.InvoiceCasePrices)
                .Where(x => x.LaboratoryTestID == laboratoryTestId && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode)
                .ToListAsync();

            var incomingIds = incoming.Where(v => v.ID != 0).Select(v => v.ID).ToHashSet();

            // Soft-delete versions removed from the form
            foreach (var ex in existing.Where(e => !incomingIds.Contains(e.ID)))
            {
                ex.IsActive = false;
                ex.ModifiedOn = DateTime.UtcNow;
            }

            foreach (var v in incoming)
            {
                var match = v.ID != 0 ? existing.FirstOrDefault(e => e.ID == v.ID) : null;

                if (match == null)
                {
                    _context.InvoiceCases.Add(v);
                }
                else
                {
                    match.EffectiveFrom = v.EffectiveFrom;
                    match.FinancialYearId = v.FinancialYearId;
                    match.DefaultPricingType = v.DefaultPricingType;
                    match.ModifiedOn = DateTime.UtcNow;

                    // Remove price rows dropped from the form
                    var toRemove = match.InvoiceCasePrices
                        .Where(p => !v.InvoiceCasePrices.Any(np => np.ID == p.ID))
                        .ToList();
                    foreach (var p in toRemove)
                        match.InvoiceCasePrices.Remove(p);

                    // Add or update price rows
                    foreach (var np in v.InvoiceCasePrices)
                    {
                        var pMatch = np.ID != 0
                            ? match.InvoiceCasePrices.FirstOrDefault(p => p.ID == np.ID)
                            : null;

                        if (pMatch == null)
                        {
                            match.InvoiceCasePrices.Add(np);
                        }
                        else
                        {
                            pMatch.InvoiceCaseConfigID = np.InvoiceCaseConfigID;
                            pMatch.Name = np.Name;
                            pMatch.AliasName = np.AliasName;
                            pMatch.Price = np.Price;
                        }
                    }
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task<long?> GetFinancialYearIdForDate(DateTime date)
        {
            return await _context.FinancialYears
                .Where(f => f.StartDate <= date && f.EndDate >= date)
                .Select(f => (long?)f.Id)
                .FirstOrDefaultAsync();
        }

        public async Task<long?> GetCurrentFinancialYearId()
        {
            return await _context.FinancialYears
                .Where(f => f.IsCurrent)
                .Select(f => (long?)f.Id)
                .FirstOrDefaultAsync();
        }
    }
}
