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
                    join labSpec in _context.LabScopeSpecifications on labScope.ID equals labSpec.LabScopeID
                    join testMS in _context.TestMethodSpecifications on labSpec.TestMethodSpecificationID equals testMS.ID
                    join labPara in _context.LabScopeSpecificationParameters on labSpec.ID equals labPara.LabScopeSpecificationID
                    join p in _context.ParameterMasters on labPara.ParameterID equals p.ID
                    join pu in _context.ParameterUnitMasters on labPara.ParameterUnitID equals pu.ID
                    where labScope.IsActive && labScope.CompanyCode == loggedInUser.CompanyCode
               select new
               {
                   labScope.ID,
                   labScope.LaboratoryTestID,
                   LaboratoryTestName = testMethod.Name,
                   labSpec.TestMethodSpecificationID,
                   TestMethodSpecificationName = testMS.Name,
                   ParameterName = p.Name,
                   ParameterUnit = pu.Name,
                   labPara.QualitativeQuantitative,
                   labPara.IsUnderISO,
                   labPara.LowerLimit,
                   labPara.UpperLimit,
                   labScope.ModifiedOn
               };

            _query = _query.AsQueryable().ApplyFilters(filter.Filter);

            if (!string.IsNullOrWhiteSpace(filter.searchTerm))
            {
                var search = filter.searchTerm.Trim().ToLower();
                _query = _query.Where(x => (x.LaboratoryTestName != null && x.LaboratoryTestName.ToLower().Contains(search)));
            }

            if (filter.SortByColumn != null)
            {
                _query = _query.OrderBy($"{filter.SortByColumn} {(filter.SortOrder == "asc" ? "ascending" : "descending")}");
            }

            // Total Records Count
            int totalRecords = await _query.CountAsync();

            // Apply Pagination
            var items = await _query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return new PagedResponse<object>(items.Cast<object>().ToList(), totalRecords, filter.PageNumber, filter.PageSize);
        }

       

     
    }
}
