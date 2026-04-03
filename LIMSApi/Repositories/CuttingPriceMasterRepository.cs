using System.Linq.Dynamic.Core;
using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Repositories
{
    public class CuttingPriceMasterRepository : ICuttingPriceMasterRepository
    {
        private readonly LIMSContext _context;
        private LoggedInUserDTO loggedInUser;

        public CuttingPriceMasterRepository(LIMSContext context)
        {
            _context = context;
            loggedInUser = LoggedInUserProvider.CurrentUser;
        }

        public async Task AddCuttingPrice(CuttingPriceMaster model)
        {
            await _context.CuttingPriceMasters.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCuttingPrice(CuttingPriceMaster model)
        {
           _context.CuttingPriceMasters.Update(model);
            await _context.SaveChangesAsync();
        }

        public async Task<CuttingPriceMaster?> GetCuttingPriceById(long id)
        {
            return await _context.CuttingPriceMasters.FirstOrDefaultAsync(x => x.ID == id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);
        }

        public async Task UpdateCuttingPrice(CuttingPriceMaster model)
        {
            _context.CuttingPriceMasters.Update(model);
            await _context.SaveChangesAsync();
        }

        public async Task<PagedResponse<object>> GetAllCuttingPrices(PageFilter filter)
        {
            var _query = from c in _context.CuttingPriceMasters
                         join st in _context.SpecimenTypeMasters on c.SpecimenTypeId equals st.ID into stJoin
                         from st in stJoin.DefaultIfEmpty()
                         where c.IsActive && c.CompanyCode == loggedInUser.CompanyCode
                         select new
                         {
                             c.ID,
                             c.CuttingType,
                             c.UnitType,
                             c.RatePerUnit,
                             c.Remark,
                             c.SpecimenTypeId,
                             SpecimenTypeName = st != null ? st.Name : "",
                             c.ModifiedOn
                         };

            _query = _query.AsQueryable().ApplyFilters(filter.Filter);

            if (!string.IsNullOrWhiteSpace(filter.searchTerm))
            {
                var search = filter.searchTerm.Trim().ToLower();
                _query = _query.Where(x => (x.CuttingType != null && x.CuttingType.ToLower().Contains(search))
                    || (x.SpecimenTypeName != null && x.SpecimenTypeName.ToLower().Contains(search)));
            }

            if (filter.SortByColumn != null)
            {
                _query = _query.OrderBy($"{filter.SortByColumn} {(filter.SortOrder == "asc" ? "ascending" : "descending")}");
            }

            return await _query.Cast<object>().ToPagedAsync(filter);
        }

        public async Task<List<DropdwonSelector>> GetCuttingPriceDropdown(string? searchTerm, int pageNo = 0, int pageSize = 20)
        {
            if (pageNo < 0) pageNo = 0;

            var _query = from a in _context.CuttingPriceMasters where a.IsActive && a.CompanyCode == loggedInUser.CompanyCode select a;

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var search = searchTerm.Trim().ToLower();
                _query = _query.Where(x => x.ID.ToString().Contains(search) || (x.CuttingType != null && x.CuttingType.ToLower().Contains(search)));
            }

            var skip = pageNo * pageSize;

            var data = await (_query.Skip(skip).Take(pageSize).Select(x => new DropdwonSelector
            {
                Id = x.ID,
                Name = x.CuttingType,
            })).ToListAsync();

            return data;
        }

        public async Task<bool> ExistsByName(string name)
        {
            return await _context.CuttingPriceMasters.AnyAsync(x => x.CuttingType == name && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);
        }

        public async Task<bool> ExistsByNameAndNotId(string name, long Id)
        {
            return await _context.CuttingPriceMasters.AnyAsync(x => x.CuttingType == name && x.ID != Id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);
        }

        public async Task<List<CuttingPriceMaster>> GetAllCuttingPricesList()
        {
            return await _context.CuttingPriceMasters
                .Where(x => x.IsActive && x.CompanyCode == loggedInUser.CompanyCode)
                .ToListAsync();
        }

        public async Task<CuttingPriceMaster?> GetBySpecimenAndCuttingType(long? specimenTypeId, string cuttingType)
        {
            return await _context.CuttingPriceMasters
                .FirstOrDefaultAsync(x => x.SpecimenTypeId == specimenTypeId
                    && x.CuttingType == cuttingType
                    && x.IsActive
                    && x.CompanyCode == loggedInUser.CompanyCode);
        }
    }
}
