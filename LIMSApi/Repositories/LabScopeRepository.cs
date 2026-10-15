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
            .Include(x => x.LaboratoryTest)
            .Include(x => x.SubGroup)
            .Include(x => x.AnalysisType)
            .Include(x => x.Specifications)
                .ThenInclude(s => s.TestMethodSpecification)
            .Include(x => x.Specifications)
                .ThenInclude(s => s.TestMethodSpecificationVersion)
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
            // Base query — only SQL-translatable expressions
            var baseQuery =
                    from labScope in _context.LabScopeMasters
                    join testMethod in _context.LaboratoryTests on labScope.LaboratoryTestID equals testMethod.ID
                    where labScope.IsActive && labScope.CompanyCode == loggedInUser.CompanyCode
                    join empMod in _context.EmployeeMasters on labScope.ModifiedBy equals empMod.ID into empModJoin
                    from empMod in empModJoin.DefaultIfEmpty()
                    join empCre in _context.EmployeeMasters on labScope.CreatedBy equals empCre.ID into empCreJoin
                    from empCre in empCreJoin.DefaultIfEmpty()
               select new
               {
                   labScope.ID,
                   labScope.LaboratoryTestID,
                   LaboratoryTestName = testMethod.Name,
                   testMethod.IsChemicalTest,
                   labScope.LaboratoryTestSubGroupID,
                   SubGroupName = labScope.SubGroup != null ? labScope.SubGroup.Name : null,
                   labScope.AnalysisTypeID,
                   AnalysisTypeName = labScope.AnalysisType != null ? labScope.AnalysisType.Name : null,
                   ParameterCount = _context.LabScopeSpecificationParameters
                       .Count(p => _context.LabScopeSpecifications
                           .Any(s => s.LabScopeID == labScope.ID && s.ID == p.LabScopeSpecificationID)),
                   HasISO = _context.LabScopeSpecificationParameters
                       .Any(p => p.IsUnderISO && _context.LabScopeSpecifications
                           .Any(s => s.LabScopeID == labScope.ID && s.ID == p.LabScopeSpecificationID)),
                   labScope.ValidFrom,
                   labScope.ValidUntil,
                   labScope.NextReviewDate,
                   labScope.CreatedBy,
                   labScope.CreatedOn,
                   labScope.ModifiedBy,
                   ModifiedOn = labScope.ModifiedOn ?? labScope.CreatedOn,
                   CreatedByName = empCre != null ? empCre.Name : null,
                   ModifiedByName = empMod != null ? empMod.Name : (empCre != null ? empCre.Name : null)
               };

            if (!string.IsNullOrWhiteSpace(filter.searchTerm))
            {
                var search = filter.searchTerm.Trim();
                baseQuery = baseQuery.Where(x =>
                    (x.LaboratoryTestName != null && x.LaboratoryTestName.Contains(search))
                    || (x.SubGroupName != null && x.SubGroupName.Contains(search))
                    || (x.AnalysisTypeName != null && x.AnalysisTypeName.Contains(search))
                    || (x.ModifiedByName != null && x.ModifiedByName.Contains(search))
                    || (x.CreatedByName != null && x.CreatedByName.Contains(search)));
            }

            if (filter.SortByColumn != null)
            {
                baseQuery = baseQuery.OrderBy($"{filter.SortByColumn} {(filter.SortOrder == "asc" ? "ascending" : "descending")}");
            }

            // Paginate in DB, then materialize and add computed fields
            var totalRecords = await baseQuery.CountAsync();
            var pageSize = filter.PageSize > 0 ? filter.PageSize : 10;
            var pageNumber = filter.PageNumber > 0 ? filter.PageNumber : 1;

            var dbItems = await baseQuery
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Build spec names in memory (string.Join can't translate to SQL)
            var scopeIds = dbItems.Select(x => x.ID).ToList();
            var specNames = await _context.LabScopeSpecifications
                .Where(s => scopeIds.Contains(s.LabScopeID))
                .Select(s => new
                {
                    s.LabScopeID,
                    SpecTitle = s.TestMethodSpecification != null ? (s.TestMethodSpecification.DisplayTitle ?? s.TestMethodSpecification.Name) : "",
                    VersionStr = s.TestMethodSpecificationVersion != null ? s.TestMethodSpecificationVersion.Version : null
                })
                .ToListAsync();

            var specNameMap = specNames
                .GroupBy(x => x.LabScopeID)
                .ToDictionary(
                    g => g.Key,
                    g => string.Join(", ", g.Select(x => !string.IsNullOrEmpty(x.VersionStr) ? $"{x.SpecTitle} : {x.VersionStr}" : x.SpecTitle))
                );

            var result = dbItems.Select(x =>
            {
                var hierarchyParts = new List<string>();
                if (!string.IsNullOrEmpty(x.LaboratoryTestName)) hierarchyParts.Add(x.LaboratoryTestName);
                if (!string.IsNullOrEmpty(x.SubGroupName)) hierarchyParts.Add(x.SubGroupName);
                if (x.IsChemicalTest && !string.IsNullOrEmpty(x.AnalysisTypeName)) hierarchyParts.Add(x.AnalysisTypeName);

                var hierarchyName = hierarchyParts.Count > 0 ? string.Join(" > ", hierarchyParts) : (x.LaboratoryTestName ?? "");
                var childName = (x.IsChemicalTest && !string.IsNullOrEmpty(x.AnalysisTypeName))
                    ? x.AnalysisTypeName
                    : (!string.IsNullOrEmpty(x.SubGroupName) ? x.SubGroupName : (x.LaboratoryTestName ?? ""));

                return (object)new
                {
                    x.ID,
                    x.LaboratoryTestID,
                    x.LaboratoryTestName,
                    x.IsChemicalTest,
                    x.LaboratoryTestSubGroupID,
                    x.SubGroupName,
                    x.AnalysisTypeID,
                    x.AnalysisTypeName,
                    HierarchyName = hierarchyName,
                    ChildName = childName,
                    TestMethodSpecificationName = specNameMap.GetValueOrDefault(x.ID, ""),
                    x.ParameterCount,
                    IsUnderISO = x.HasISO ? "Yes" : "No",
                    x.ValidFrom,
                    x.ValidUntil,
                    x.NextReviewDate,
                    x.CreatedBy,
                    x.CreatedOn,
                    x.ModifiedBy,
                    x.ModifiedOn,
                    x.CreatedByName,
                    x.ModifiedByName
                };
            }).ToList();

            return new PagedResponse<object>(result, totalRecords, pageNumber, pageSize);
        }

       

     
    }
}
