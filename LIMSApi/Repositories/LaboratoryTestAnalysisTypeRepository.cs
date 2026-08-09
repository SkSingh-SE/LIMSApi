using System.Linq.Dynamic.Core;
using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Repositories
{
    public class LaboratoryTestAnalysisTypeRepository : ILaboratoryTestAnalysisTypeRepository
    {
        private readonly LIMSContext _context;
        private LoggedInUserDTO loggedInUser;

        public LaboratoryTestAnalysisTypeRepository(LIMSContext context)
        {
            _context = context;
            loggedInUser = LoggedInUserProvider.CurrentUser;
        }

        public async Task Add(LaboratoryTestAnalysisType model)
        {
            await _context.LaboratoryTestAnalysisTypes.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task Update(LaboratoryTestAnalysisType model)
        {
            _context.LaboratoryTestAnalysisTypes.Update(model);
            await _context.SaveChangesAsync();
        }

        public async Task<LaboratoryTestAnalysisType?> GetById(long id)
        {
            return await _context.LaboratoryTestAnalysisTypes
                .Include(x => x.SubGroup)
                .Include(x => x.MetalClassification)
                .Include(x => x.AllowedTechniques)
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
                    .ThenInclude(s => s.ProductMaster)
                .Include(x => x.InvoiceCases)
                    .ThenInclude(i => i.InvoiceCaseConfiguration)
                .FirstOrDefaultAsync(x => x.ID == id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);
        }

        public async Task<List<LaboratoryTestAnalysisType>> GetBySubGroupId(long subGroupId)
        {
            return await _context.LaboratoryTestAnalysisTypes
                .Include(x => x.MetalClassification)
                .Include(x => x.AllowedTechniques)
                    .ThenInclude(t => t.AnalysisTechnique)
                .Where(x => x.LaboratoryTestSubGroupID == subGroupId && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode)
                .OrderBy(x => x.Name)
                .ToListAsync();
        }

        public async Task<List<DropdwonSelector>> GetDropdown(long subGroupId)
        {
            return await _context.LaboratoryTestAnalysisTypes
                .Where(x => x.LaboratoryTestSubGroupID == subGroupId && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode)
                .OrderBy(x => x.Name)
                .Select(x => new DropdwonSelector
                {
                    Id = x.ID,
                    Name = x.Name
                }).ToListAsync();
        }

        public async Task<bool> ExistsByNamePerSubGroup(string name, long subGroupId, long? excludeId)
        {
            return await _context.LaboratoryTestAnalysisTypes.AnyAsync(x =>
                x.Name == name
                && x.LaboratoryTestSubGroupID == subGroupId
                && x.IsActive
                && x.CompanyCode == loggedInUser.CompanyCode
                && (!excludeId.HasValue || x.ID != excludeId.Value));
        }

        public async Task<PagedResponse<object>> GetAll(PageFilter filter)
        {
            var query = _context.LaboratoryTestAnalysisTypes
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
                query = query.OrderBy(x => x.Name);
            }

            var projected = query.Select(x => new
            {
                x.ID,
                x.LaboratoryTestSubGroupID,
                x.MetalClassificationID,
                MetalClassification = x.MetalClassification != null ? x.MetalClassification.Name : null,
                x.Name,
                x.TestDuration,
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
