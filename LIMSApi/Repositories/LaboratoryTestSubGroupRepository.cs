using System.Linq.Dynamic.Core;
using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Repositories
{
    public class LaboratoryTestSubGroupRepository : ILaboratoryTestSubGroupRepository
    {
        private readonly LIMSContext _context;
        private LoggedInUserDTO loggedInUser;

        public LaboratoryTestSubGroupRepository(LIMSContext context)
        {
            _context = context;
            loggedInUser = LoggedInUserProvider.CurrentUser;
        }

        public async Task Add(LaboratoryTestSubGroup model)
        {
            await _context.LaboratoryTestSubGroups.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task Update(LaboratoryTestSubGroup model)
        {
            _context.LaboratoryTestSubGroups.Update(model);
            await _context.SaveChangesAsync();
        }

        public async Task<LaboratoryTestSubGroup?> GetById(long id)
        {
            return await _context.LaboratoryTestSubGroups
                .Include(x => x.MetalClassification)
                .Include(x => x.AnalysisTypes)
                    .ThenInclude(s => s.MetalClassification)
                .Include(x => x.AnalysisTypes)
                    .ThenInclude(s => s.AllowedTechniques)
                        .ThenInclude(t => t.AnalysisTechnique)
                .Include(x => x.Parameters)
                    .ThenInclude(p => p.Parameter)
                .Include(x => x.TestMethods)
                    .ThenInclude(m => m.TestMethodSpecification)
                .Include(x => x.TestMethods)
                    .ThenInclude(m => m.TestMethodSpecificationVersion)
                        .ThenInclude(v => v.TestMethodSpecification)
                .Include(x => x.Equipments)
                    .ThenInclude(e => e.Equipment)
                .Include(x => x.Specifications)
                    .ThenInclude(s => s.MaterialSpecification)
                .Include(x => x.Specifications)
                    .ThenInclude(s => s.SpecificationGrade)
                .Include(x => x.Specifications)
                    .ThenInclude(s => s.ProductSpecification)
                .Include(x => x.InvoiceCases)
                    .ThenInclude(i => i.InvoiceCaseConfiguration)
                .FirstOrDefaultAsync(x => x.ID == id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);
        }

        public async Task<List<LaboratoryTestSubGroup>> GetByLabTestId(long labTestId)
        {
            return await _context.LaboratoryTestSubGroups
                .Include(x => x.MetalClassification)
                .Include(x => x.AnalysisTypes)
                    .ThenInclude(s => s.MetalClassification)
                .Include(x => x.AnalysisTypes)
                    .ThenInclude(s => s.AllowedTechniques)
                        .ThenInclude(t => t.AnalysisTechnique)
                .Include(x => x.Parameters)
                    .ThenInclude(p => p.Parameter)
                .Include(x => x.TestMethods)
                    .ThenInclude(m => m.TestMethodSpecification)
                .Include(x => x.TestMethods)
                    .ThenInclude(m => m.TestMethodSpecificationVersion)
                        .ThenInclude(v => v.TestMethodSpecification)
                .Include(x => x.Equipments)
                    .ThenInclude(e => e.Equipment)
                .Include(x => x.Specifications)
                    .ThenInclude(s => s.MaterialSpecification)
                .Include(x => x.Specifications)
                    .ThenInclude(s => s.SpecificationGrade)
                .Include(x => x.Specifications)
                    .ThenInclude(s => s.ProductSpecification)
                .Include(x => x.InvoiceCases)
                    .ThenInclude(i => i.InvoiceCaseConfiguration)
                .Where(x => x.LaboratoryTestID == labTestId && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode)
                .OrderBy(x => x.DisplayOrder).ThenBy(x => x.Name)
                .ToListAsync();
        }

        public async Task<List<DropdwonSelector>> GetDropdown(long labTestId)
        {
            return await _context.LaboratoryTestSubGroups
                .Where(x => x.LaboratoryTestID == labTestId && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode)
                .OrderBy(x => x.DisplayOrder).ThenBy(x => x.Name)
                .Select(x => new DropdwonSelector { Id = x.ID, Name = x.Name })
                .ToListAsync();
        }

        public async Task<bool> ExistsByNamePerLabTest(string name, long labTestId, long? excludeId)
        {
            return await _context.LaboratoryTestSubGroups.AnyAsync(x =>
                x.Name == name
                && x.LaboratoryTestID == labTestId
                && x.IsActive
                && x.CompanyCode == loggedInUser.CompanyCode
                && (!excludeId.HasValue || x.ID != excludeId.Value));
        }

        public async Task<PagedResponse<object>> GetAll(PageFilter filter)
        {
            var query = _context.LaboratoryTestSubGroups
                .Where(x => x.IsActive && x.CompanyCode == loggedInUser.CompanyCode)
                .AsQueryable().ApplyFilters(filter.Filter);

            if (!string.IsNullOrWhiteSpace(filter.searchTerm))
            {
                var search = filter.searchTerm.Trim();
                query = query.Where(x => x.Name.Contains(search));
            }

            if (filter.SortByColumn != null)
            {
                query = query.OrderBy($"{filter.SortByColumn} {(filter.SortOrder == "asc" ? "ascending" : "descending")}");
            }
            else
            {
                query = query.OrderBy(x => x.DisplayOrder).ThenBy(x => x.Name);
            }

            var projected = query.Select(x => new
            {
                x.ID,
                x.LaboratoryTestID,
                x.MetalClassificationID,
                MetalClassification = x.MetalClassification != null ? x.MetalClassification.Name : null,
                x.Name,
                x.ReportTestName,
                x.TestDuration,
                x.DisplayOrder,
                x.CreatedBy,
                x.CreatedOn,
                x.ModifiedBy,
                x.ModifiedOn,
                x.IsActive
            });

            return await projected.Cast<object>().ToPagedAsync(filter);
        }
    }
}
