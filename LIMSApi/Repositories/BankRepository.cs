using System.Linq.Dynamic.Core;
using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Repositories
{
    public class BankRepository : IBankRepository
    {
        private readonly LIMSContext _context;
        private LoggedInUserDTO loggedInUser;

        public BankRepository(LIMSContext context)
        {
            _context = context;
            loggedInUser = LoggedInUserProvider.CurrentUser;
        }

        public async Task AddBank(BankMaster model)
        {
            await _context.BankMasters.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteBank(BankMaster model)
        {
           _context.BankMasters.Update(model);
            await _context.SaveChangesAsync();
        }

        public async Task<BankMaster?> GetBankById(long id)
        {
            return await _context.BankMasters.FirstOrDefaultAsync(x => x.ID == id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);
        }

        public async Task UpdateBank(BankMaster model)
        {
            _context.BankMasters.Update(model);
            await _context.SaveChangesAsync();
        }

        public async Task<PagedResponse<object>> GetAllBanks(PageFilter filter)
        {
            var _query = (from c in _context.BankMasters where c.IsActive && c.CompanyCode == loggedInUser.CompanyCode select c).AsQueryable().ApplyFilters(filter.Filter);

            
            if (!string.IsNullOrWhiteSpace(filter.searchTerm))
            {
                var search = filter.searchTerm.Trim().ToLower();
                _query = _query.Where(x =>
                    (x.BankName != null && x.BankName.ToLower().Contains(search)) ||
                    (x.AccountHolderName != null && x.AccountHolderName.ToLower().Contains(search)) ||
                    (x.AccountNumber != null && x.AccountNumber.ToLower().Contains(search)) ||
                    (x.BranchName != null && x.BranchName.ToLower().Contains(search)));
            }

            if (filter.SortByColumn != null)
            {
                _query = _query.OrderBy($"{filter.SortByColumn} {(filter.SortOrder == "asc" ? "ascending" : "descending")}");
            }

            return await _query.Cast<object>().ToPagedAsync(filter);
        }

        public async Task<List<DropdwonSelector>> GetBankDropdown(string? searchTerm, int pageNo = 0, int pageSize = 20)
        {
            if (pageNo < 0) pageNo = 0;

            var _query = from a in _context.BankMasters where a.IsActive && a.CompanyCode == loggedInUser.CompanyCode select a;

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                if (FilterHelper.IsExactIdSearch(searchTerm, out long exactId))
                {
                    _query = _query.Where(x => x.ID == exactId);
                }
                else
                {
                    var search = searchTerm.Trim().ToLower();
                    _query = _query.Where(x => (x.BankName != null && x.BankName.ToLower().Contains(search)));
                }
            }

            var skip = pageNo * pageSize;

            var data = await (_query.Skip(skip).Take(pageSize).Select(x => new DropdwonSelector
            {
                Id = x.ID,
                Name = x.BankName,
            })).ToListAsync();

            return data;
        }

        public async Task<bool> ExistsByName(string name)
        {
            return await _context.BankMasters.AnyAsync(x => x.BankName == name && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);
        }

        public async Task<bool> ExistsByNameAndNotId(string name, long Id)
        {
            return await _context.BankMasters.AnyAsync(x => x.BankName == name && x.ID != Id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);
        }

        public async Task<bool> ExistsByAccountNumber(string accountNumber)
        {
            return await _context.BankMasters.AnyAsync(x => x.AccountNumber == accountNumber && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);
        }

        public async Task<bool> ExistsByAccountNumberAndNotId(string accountNumber, long id)
        {
            return await _context.BankMasters.AnyAsync(x => x.AccountNumber == accountNumber && x.ID != id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);
        }
    }
}
