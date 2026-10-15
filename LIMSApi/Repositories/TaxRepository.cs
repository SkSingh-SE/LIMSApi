using System.Linq.Dynamic.Core;
using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Repositories
{
    public class TaxRepository : ITaxRepository
    {
        private readonly LIMSContext _context;
        private LoggedInUserDTO loggedInUser;

        public TaxRepository(LIMSContext context)
        {
            _context = context;
            loggedInUser = LoggedInUserProvider.CurrentUser;
        }

        public async Task AddTax(TaxMaster model)
        {
            await _context.TaxMasters.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteTax(TaxMaster model)
        {
           _context.TaxMasters.Update(model);
            await _context.SaveChangesAsync();
        }

        public async Task<TaxMaster?> GetTaxById(long id)
        {
            return await _context.TaxMasters.FirstOrDefaultAsync(x => x.ID == id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);
        }

        public async Task UpdateTax(TaxMaster model)
        {
            _context.TaxMasters.Update(model);
            await _context.SaveChangesAsync();
        }

        public async Task<PagedResponse<object>> GetAllTaxs(PageFilter filter)
        {
            var baseQuery = from c in _context.TaxMasters
                            where c.IsActive && c.CompanyCode == loggedInUser.CompanyCode
                            join empMod in _context.EmployeeMasters on c.ModifiedBy equals empMod.ID into empModJoin
                            from empMod in empModJoin.DefaultIfEmpty()
                            join empCre in _context.EmployeeMasters on c.CreatedBy equals empCre.ID into empCreJoin
                            from empCre in empCreJoin.DefaultIfEmpty()
                            select new
                            {
                                c.ID,
                                c.Name,
                                c.Rate,
                                c.IsActive,
                                c.CompanyCode,
                                c.CreatedBy,
                                c.CreatedOn,
                                c.ModifiedBy,
                                ModifiedOn = c.ModifiedOn ?? c.CreatedOn,
                                CreatedByName = empCre != null ? empCre.Name : null,
                                ModifiedByName = empMod != null ? empMod.Name : (empCre != null ? empCre.Name : null)
                            };

            var _query = baseQuery.AsQueryable().ApplyFilters(filter.Filter);

            if (!string.IsNullOrWhiteSpace(filter.searchTerm))
            {
                var search = filter.searchTerm.Trim();
                // Numeric Rate column — parse as decimal for exact match instead of CAST LIKE.
                decimal? searchRate = decimal.TryParse(search, out var r) ? r : (decimal?)null;
                _query = _query.Where(x =>
                    (x.Name != null && x.Name.Contains(search))
                    || (searchRate != null && x.Rate == searchRate)
                    || (x.ModifiedByName != null && x.ModifiedByName.Contains(search))
                    || (x.CreatedByName != null && x.CreatedByName.Contains(search)));
            }

            if (filter.SortByColumn != null)
            {
                _query = _query.OrderBy($"{filter.SortByColumn} {(filter.SortOrder == "asc" ? "ascending" : "descending")}");
            }

            return await _query.Cast<object>().ToPagedAsync(filter);
        }

        public async Task<List<DropdwonSelector>> GetTaxDropdown(string? searchTerm, int pageNo = 0, int pageSize = 20)
        {
            if (pageNo < 0) pageNo = 0;

            var _query = from a in _context.TaxMasters where a.IsActive && a.CompanyCode == loggedInUser.CompanyCode select a;

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
            return await _context.TaxMasters.AnyAsync(x => x.Name == name && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);
        }

        public async Task<bool> ExistsByNameAndNotId(string name, long Id)
        {
            return await _context.TaxMasters.AnyAsync(x => x.Name == name && x.ID != Id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);
        }
    }
}
