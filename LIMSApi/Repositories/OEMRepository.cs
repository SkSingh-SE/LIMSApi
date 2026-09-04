using System.Linq.Dynamic.Core;
using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Repositories
{
    public class OEMRepository : IOEMRepository
    {
        private readonly LIMSContext _context;
        private LoggedInUserDTO loggedInUser;

        public OEMRepository(LIMSContext context)
        {
            _context = context;
            loggedInUser = LoggedInUserProvider.CurrentUser;
        }

        public async Task AddOEM(OEMMaster model)
        {
            model.CompanyCode = loggedInUser.CompanyCode;
            await _context.OEMMasters.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteOEM(long id)
        {
            var existingOEM = await _context.OEMMasters.FirstOrDefaultAsync(x => x.ID == id && x.IsActive);
            if (existingOEM != null)
            {
                existingOEM.IsActive = false;
                existingOEM.ModifiedOn = DateTime.UtcNow;
                _context.OEMMasters.Update(existingOEM);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<OEMMaster?> GetOEMById(long id)
        {
            return await _context.OEMMasters.FirstOrDefaultAsync(x => x.ID == id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);
        }

        public async Task UpdateOEM(OEMMaster model)
        {
            _context.OEMMasters.Update(model);
            await _context.SaveChangesAsync();
        }

        public async Task<PagedResponse<object>> GetAllOEMs(PageFilter filter)
        {
            var _query = from c in _context.OEMMasters
                         join empMod in _context.EmployeeMasters on c.ModifiedBy equals empMod.ID into empModGroup
                         from empMod in empModGroup.DefaultIfEmpty()
                         join empCre in _context.EmployeeMasters on c.CreatedBy equals empCre.ID into empCreGroup
                         from empCre in empCreGroup.DefaultIfEmpty()
                         where c.IsActive && c.CompanyCode == loggedInUser.CompanyCode
                         select new
                         {
                             c.ID,
                             c.Name,
                             c.ContactPerson1,
                             c.ContactPerson2,
                             c.ContactPerson3,
                             c.ContactNo1,
                             c.ContactNo2,
                             c.ContactNo3,
                             c.EmailId1,
                             c.EmailId2,
                             c.EmailId3,
                             c.Address,
                             c.AgreementFilePath,
                             c.FileName,
                             c.SupplierApproved,
                             c.IsBlacklisted,
                             c.ReasonForBlacklisting,
                             c.CreatedBy,
                             CreatedByName = empCre != null ? empCre.Name : "-",
                             c.CreatedOn,
                             ModifiedByName = empMod != null ? empMod.Name : (empCre != null ? empCre.Name : "-"),
                             ModifiedOn = c.ModifiedOn ?? c.CreatedOn
                         };

            _query = _query.AsQueryable().ApplyFilters(filter.Filter);

            if (!string.IsNullOrWhiteSpace(filter.searchTerm))
            {
                var search = filter.searchTerm.Trim();
                _query = _query.Where(x =>
                    (x.Name != null && x.Name.Contains(search))
                    || (x.ContactPerson1 != null && x.ContactPerson1.Contains(search))
                    || (x.EmailId1 != null && x.EmailId1.Contains(search))
                    || (x.ContactNo1 != null && x.ContactNo1.Contains(search))
                    || (x.Address != null && x.Address.Contains(search))
                );
            }

            if (filter.SortByColumn != null)
            {
                _query = _query.OrderBy($"{filter.SortByColumn} {(filter.SortOrder == "asc" ? "ascending" : "descending")}");
            }

            return await _query.Cast<object>().ToPagedAsync(filter);
        }

        public async Task<List<DropdwonSelector>> GetOEMDropdown(string? searchTerm, int pageNo = 0, int pageSize = 20)
        {
            if (pageNo < 0) pageNo = 0;

            var _query = from a in _context.OEMMasters where a.IsActive && a.CompanyCode == loggedInUser.CompanyCode select a;

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
            return await _context.OEMMasters.AnyAsync(x => x.Name == name && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);
        }

        public async Task<bool> ExistsByNameAndNotId(string name, long Id)
        {
            return await _context.OEMMasters.AnyAsync(x => x.Name == name && x.ID != Id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);
        }
    }
}
