using System.Linq.Dynamic.Core;
using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Repositories
{
    public class AnalysisTechniqueRepository : IAnalysisTechniqueRepository
    {
        private readonly LIMSContext _context;
        private LoggedInUserDTO loggedInUser;

        public AnalysisTechniqueRepository(LIMSContext context)
        {
            _context = context;
            loggedInUser = LoggedInUserProvider.CurrentUser;
        }

        public async Task AddAnalysisTechnique(AnalysisTechniqueMaster model)
        {
            await _context.AnalysisTechniqueMasters.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAnalysisTechnique(AnalysisTechniqueMaster model)
        {
            _context.AnalysisTechniqueMasters.Update(model);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAnalysisTechnique(AnalysisTechniqueMaster model)
        {
            _context.AnalysisTechniqueMasters.Update(model);
            await _context.SaveChangesAsync();
        }

        public async Task<AnalysisTechniqueMaster?> GetAnalysisTechniqueById(long id)
        {
            return await _context.AnalysisTechniqueMasters
                .FirstOrDefaultAsync(x => x.ID == id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);
        }

        public async Task<PagedResponse<object>> GetAllAnalysisTechniques(PageFilter filter)
        {
            var _query = (from c in _context.AnalysisTechniqueMasters
                          where c.IsActive && c.CompanyCode == loggedInUser.CompanyCode
                          select c).AsQueryable().ApplyFilters(filter.Filter);

            if (!string.IsNullOrWhiteSpace(filter.searchTerm))
            {
                var search = filter.searchTerm.Trim();
                _query = _query.Where(x =>
                    (x.Name != null && x.Name.Contains(search))
                    || (x.Code != null && x.Code.Contains(search))
                    || (x.AliasNames != null && x.AliasNames.Contains(search))
                );
            }

            if (filter.SortByColumn != null)
            {
                _query = _query.OrderBy($"{filter.SortByColumn} {(filter.SortOrder == "asc" ? "ascending" : "descending")}");
            }
            else
            {
                _query = _query.OrderBy(x => x.Name);
            }

            var projected = _query.Select(x => new
            {
                x.ID,
                x.Name,
                x.Code,
                x.AliasNames,
                x.Description,
                x.CreatedBy,
                x.CreatedOn,
                x.ModifiedBy,
                x.ModifiedOn,
                x.IsActive
            });

            return await projected.Cast<object>().ToPagedAsync(filter);
        }

        public async Task<List<DropdwonSelector>> GetAnalysisTechniqueDropdown(string? searchTerm, int pageNo = 0, int pageSize = 20)
        {
            if (pageNo < 0) pageNo = 0;

            var _query = from a in _context.AnalysisTechniqueMasters
                         where a.IsActive && a.CompanyCode == loggedInUser.CompanyCode
                         select a;

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                if (FilterHelper.IsExactIdSearch(searchTerm, out long exactId))
                {
                    _query = _query.Where(x => x.ID == exactId);
                }
                else
                {
                    var search = searchTerm.Trim();
                    _query = _query.Where(x =>
                        (x.Name != null && x.Name.Contains(search))
                        || (x.Code != null && x.Code.Contains(search))
                        || (x.AliasNames != null && x.AliasNames.Contains(search)));
                }
            }

            var skip = pageNo * pageSize;

            var data = await (_query.OrderBy(x => x.Name).Skip(skip).Take(pageSize).Select(x => new DropdwonSelector
            {
                Id = x.ID,
                Name = x.Name,
            })).ToListAsync();

            return data;
        }

        public async Task<bool> ExistsByName(string name)
        {
            return await _context.AnalysisTechniqueMasters.AnyAsync(x => x.Name == name && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);
        }

        public async Task<bool> ExistsByNameAndNotId(string name, long id)
        {
            return await _context.AnalysisTechniqueMasters.AnyAsync(x => x.Name == name && x.ID != id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);
        }

        public async Task<bool> ExistsByCode(string code)
        {
            return await _context.AnalysisTechniqueMasters.AnyAsync(x => x.Code == code && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);
        }

        public async Task<bool> ExistsByCodeAndNotId(string code, long id)
        {
            return await _context.AnalysisTechniqueMasters.AnyAsync(x => x.Code == code && x.ID != id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);
        }
    }
}
