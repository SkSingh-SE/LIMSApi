using System.Linq.Dynamic.Core;
using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Repositories
{
    public class LabScopeRepository : ILabScopeRepository
    {
        private readonly LIMSContext _context;
        private LoggedInUserDTO loggedInUser;

        public LabScopeRepository(LIMSContext context)
        {
            _context = context;
            loggedInUser = LoggedInUserProvider.CurrentUser;
        }

        public async Task AddLabScope(LabScopeMaster model)
        {
            model.CompanyCode = loggedInUser.CompanyCode;
            await _context.LabScopeMasters.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteLabScope(long id)
        {
            var existingLabScope = await _context.LabScopeMasters.FirstOrDefaultAsync(x => x.ID == id && x.IsActive);
            if (existingLabScope != null)
            {
                existingLabScope.IsActive = false;
                existingLabScope.ModifiedOn = DateTime.UtcNow;
                _context.LabScopeMasters.Update(existingLabScope);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<LabScopeMaster?> GetLabScopeById(long id)
        {
            return await _context.LabScopeMasters
            .Include(x => x.Specifications)
                .ThenInclude(s => s.TestMethodSpecification)
            .Include(x => x.Specifications)
                .ThenInclude(s => s.Parameters)
                    .ThenInclude(p => p.Equipments)
            .FirstOrDefaultAsync(x => x.ID == id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);
        }

        public async Task UpdateLabScope(LabScopeMaster model)
        {
            _context.LabScopeMasters.Update(model);
            await _context.SaveChangesAsync();
        }

        public async Task<PagedResponse<object>> GetAllLabScopes(PageFilter filter)
        {
            var _query =
                    from labScope in _context.LabScopeMasters
                    join testMethod in _context.LaboratoryTests on labScope.LaboratoryTestID equals testMethod.ID
                    where labScope.IsActive && labScope.CompanyCode == loggedInUser.CompanyCode
               select new
               {
                   labScope.ID,
                   labScope.LaboratoryTestID,
                   LaboratoryTestName = testMethod.Name,
                   TestMethodSpecificationName = string.Join(", ",
                       _context.LabScopeSpecifications
                           .Where(s => s.LabScopeID == labScope.ID)
                           .Join(_context.TestMethodSpecifications, s => s.TestMethodSpecificationID, t => t.ID, (s, t) => t.Name)),
                   ParameterCount = _context.LabScopeSpecifications
                       .Where(s => s.LabScopeID == labScope.ID)
                       .SelectMany(s => s.Parameters)
                       .Count(),
                   IsUnderISO = _context.LabScopeSpecifications
                       .Where(s => s.LabScopeID == labScope.ID)
                       .SelectMany(s => s.Parameters)
                       .Any(p => p.IsUnderISO) ? "Yes" : "No",
                   labScope.ModifiedOn
               };

            _query = _query.AsQueryable().ApplyFilters(filter.Filter);

            if (!string.IsNullOrWhiteSpace(filter.searchTerm))
            {
                var search = filter.searchTerm.Trim().ToLower();
                _query = _query.Where(x =>
                    (x.LaboratoryTestName != null && x.LaboratoryTestName.ToLower().Contains(search))
                    || (x.TestMethodSpecificationName != null && x.TestMethodSpecificationName.ToLower().Contains(search))
                );
            }

            if (filter.SortByColumn != null)
            {
                _query = _query.OrderBy($"{filter.SortByColumn} {(filter.SortOrder == "asc" ? "ascending" : "descending")}");
            }

            return await _query.Cast<object>().ToPagedAsync(filter);
        }

       

     
    }
}
