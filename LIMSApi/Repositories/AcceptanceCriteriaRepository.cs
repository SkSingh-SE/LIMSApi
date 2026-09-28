using System.Linq.Dynamic.Core;
using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Repositories
{
    public class AcceptanceCriteriaRepository : IAcceptanceCriteriaRepository
    {
        private readonly LIMSContext _context;
        private LoggedInUserDTO loggedInUser;

        public AcceptanceCriteriaRepository(LIMSContext context)
        {
            _context = context;
            loggedInUser = LoggedInUserProvider.CurrentUser;
        }

        public async Task AddAcceptanceCriteria(AcceptanceCriteriaMaster model)
        {
            await _context.AcceptanceCriteriaMasters.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAcceptanceCriteria(AcceptanceCriteriaMaster model)
        {
            _context.AcceptanceCriteriaMasters.Update(model);
            await _context.SaveChangesAsync();
        }

        public async Task<AcceptanceCriteriaMaster?> GetAcceptanceCriteriaById(long id)
        {
            return await _context.AcceptanceCriteriaMasters
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.ID == id && x.CompanyCode == loggedInUser.CompanyCode);
        }

        public async Task<PagedResponse<object>> GetAllAcceptanceCriteria(PageFilter filter)
        {
            var _query = from c in _context.AcceptanceCriteriaMasters
                         where c.CompanyCode == loggedInUser.CompanyCode
                         select c;

            _query = _query.AsQueryable().ApplyFilters(filter.Filter);

            if (!string.IsNullOrWhiteSpace(filter.searchTerm))
            {
                var search = filter.searchTerm.Trim();
                _query = _query.Where(x => (x.Name != null && x.Name.Contains(search))
                                        || (x.Code != null && x.Code.Contains(search))
                                        || (x.Description != null && x.Description.Contains(search))
                                        || (x.EvaluationType != null && x.EvaluationType.Contains(search))
                                        || (x.DecisionRule != null && x.DecisionRule.Contains(search)));
            }

            if (filter.SortByColumn != null)
            {
                _query = _query.OrderBy($"{filter.SortByColumn} {(filter.SortOrder == "asc" ? "ascending" : "descending")}");
            }
            else
            {
                _query = _query.OrderBy(x => x.DisplayOrder).ThenBy(x => x.Name);
            }

            return await _query.Cast<object>().ToPagedAsync(filter);
        }

        public async Task<List<DropdwonSelector>> GetAcceptanceCriteriaDropdown(string? searchTerm, int pageNo = 0, int pageSize = 20)
        {
            if (pageNo < 0) pageNo = 0;

            var _query = from a in _context.AcceptanceCriteriaMasters
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
                    _query = _query.Where(x => (x.Name != null && x.Name.Contains(search))
                                            || (x.Code != null && x.Code.Contains(search)));
                }
            }

            var skip = pageNo * pageSize;

            var data = await (_query.OrderBy(x => x.DisplayOrder).ThenBy(x => x.Name).Skip(skip).Take(pageSize).Select(x => new DropdwonSelector
            {
                Id = x.ID,
                Name = x.Name,
            })).ToListAsync();

            return data;
        }

        public async Task<bool> ExistsByCode(string code)
        {
            return await _context.AcceptanceCriteriaMasters.AnyAsync(x => x.Code.ToLower() == code.ToLower() && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);
        }

        public async Task<bool> ExistsByCodeAndNotId(string code, long id)
        {
            return await _context.AcceptanceCriteriaMasters.AnyAsync(x => x.Code.ToLower() == code.ToLower() && x.ID != id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);
        }

        public async Task<bool> ExistsByName(string name)
        {
            return await _context.AcceptanceCriteriaMasters.AnyAsync(x => x.Name.ToLower() == name.ToLower() && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);
        }

        public async Task<bool> ExistsByNameAndNotId(string name, long id)
        {
            return await _context.AcceptanceCriteriaMasters.AnyAsync(x => x.Name.ToLower() == name.ToLower() && x.ID != id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);
        }
    }
}
