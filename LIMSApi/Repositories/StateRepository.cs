using System.Linq.Dynamic.Core;
using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Repositories
{
    public class StateRepository : IStateRepository
    {
        private readonly LIMSContext _context;
        private LoggedInUserDTO loggedInUser;
        public StateRepository(LIMSContext context)
        {
            _context = context;
            loggedInUser = LoggedInUserProvider.CurrentUser;
        }

        public async Task AddState(StateMaster state)
        {
            state.CompanyCode = state.CompanyCode ?? loggedInUser.CompanyCode;
            state.CreatedBy = loggedInUser.EmployeeID;
            await _context.StateMasters.AddAsync(state);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteState(long id)
        {
            var existingState = await _context.StateMasters.FirstOrDefaultAsync(x => x.ID == id && x.IsActive);
            if (existingState != null)
            {
                existingState.IsActive = false;
                existingState.ModifiedOn = DateTime.UtcNow;
                _context.StateMasters.Update(existingState);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<StateMaster> GetStateById(long id)
        {
            return await _context.StateMasters.Include(x => x.Country).FirstOrDefaultAsync(x => x.ID == id && x.IsActive);
        }

        public async Task UpdateState(StateMaster state)
        {
            _context.StateMasters.Update(state);
            await _context.SaveChangesAsync();
        }

        public async Task<PagedResponse<object>> GetAllStates(PageFilter filter)
        {
            var _query = from s in _context.StateMasters where s.IsActive select s;

            _query = _query.AsQueryable().ApplyFilters(filter.Filter);

            if (!string.IsNullOrWhiteSpace(filter.searchTerm))
            {
                var search = filter.searchTerm.Trim().ToLower();
                _query = _query.Where(x => (x.Code != null && x.Code.ToLower().Contains(search))
                                     || (x.Name != null && x.Name.ToLower().Contains(search))
                                     );
            }

            if (filter.SortByColumn != null)
            {
                _query = _query.OrderBy($"{filter.SortByColumn} {(filter.SortOrder == "asc" ? "ascending" : "descending")}");
            }

            return await _query.Cast<object>().ToPagedAsync(filter);
        }

        public async Task<bool> ExistsByName(string name)
        {
            return await _context.StateMasters.AnyAsync(x => x.Name == name && x.IsActive);
        }
        public async Task<StateMaster?> GetByName(string name)
        {
            return await _context.StateMasters.FirstOrDefaultAsync(x => x.Name == name && x.IsActive);
        }
        public async Task<bool> ExistsByNameAndNotId(string name, long id)
        {
            return await _context.StateMasters.AnyAsync(x => x.Name == name && x.ID != id && x.IsActive);
        }
    }
}
