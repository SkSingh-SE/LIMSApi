using System.Linq.Dynamic.Core;
using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Repositories
{
    public class SubContractorRepository : ISubContractorRepository
    {
        private readonly LIMSContext _context;
        private LoggedInUserDTO loggedInUser;

        public SubContractorRepository(LIMSContext context)
        {
            _context = context;
            loggedInUser = LoggedInUserProvider.CurrentUser;
        }

        public async Task AddSubContractor(SubContractorMaster model)
        {
            await _context.SubContractorMasters.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteSubContractor(SubContractorMaster model)
        {
           _context.SubContractorMasters.Update(model);
            await _context.SaveChangesAsync();
        }

        public async Task<SubContractorMaster?> GetSubContractorById(long id)
        {
            return await _context.SubContractorMasters.FirstOrDefaultAsync(x => x.ID == id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);
        }

        public async Task UpdateSubContractor(SubContractorMaster model)
        {
            _context.SubContractorMasters.Update(model);
            await _context.SaveChangesAsync();
        }

        public async Task<PagedResponse<object>> GetAllSubContractors(PageFilter filter)
        {
            var _query = from c in _context.SubContractorMasters where c.IsActive && c.CompanyCode == loggedInUser.CompanyCode select c;

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

            return await _query.Cast<object>().ToPagedAsync(filter);
        }

        public async Task<List<DropdwonSelector>> GetSubContractorDropdown(string? searchTerm, int pageNo = 0, int pageSize = 20)
        {
            if (pageNo < 0) pageNo = 0;

            var _query = from a in _context.SubContractorMasters where a.IsActive && a.CompanyCode == loggedInUser.CompanyCode select a;

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
            return await _context.SubContractorMasters.AnyAsync(x => x.Name == name && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);
        }

        public async Task<bool> ExistsByNameAndNotId(string name, long Id)
        {
            return await _context.SubContractorMasters.AnyAsync(x => x.Name == name && x.ID != Id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);
        }
    }
}
