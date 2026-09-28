using System.Linq.Dynamic.Core;
using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace LIMSApi.Repositories
{
    public class SpecificationMasterRepository : ISpecificationMasterRepository
    {
        private readonly LIMSContext _context;

        public SpecificationMasterRepository(LIMSContext context)
        {
            _context = context;
        }

        public async Task<PagedResponse<object>> GetAllSpecificationMasters(PageFilter filter, string companyCode)
        {
            var query = (from c in _context.SpecificationHeaders
                         join so in _context.StandardOrganizationMasters
                         on c.StandardOrganizationID equals so.ID into soGroup
                         from so in soGroup.DefaultIfEmpty()
                         where c.CompanyCode == companyCode
                         select new SpecificationMasterListDto
                         {
                             ID = c.ID,
                             Code = c.Code,
                             Name = c.AliasName,
                             StandardReference = c.StandardReference ?? c.Standard,
                             StandardOrganizationID = c.StandardOrganizationID,
                             StandardOrganizationName = so != null ? so.Name : null,
                             Description = c.Description ?? c.Title,
                             IsActive = c.IsActive,
                             VersionCount = c.Versions.Count,
                             CreatedOn = c.CreatedOn,
                             ModifiedOn = c.ModifiedOn
                         }).AsQueryable();

            // Typed filters from filter.Filter
            if (filter.Filter != null && filter.Filter.Any())
            {
                foreach (var f in filter.Filter)
                {
                    if (string.IsNullOrWhiteSpace(f.Value)) continue;

                    var val = f.Value.Trim();
                    switch (f.Column.ToLowerInvariant())
                    {
                        case "code":
                            query = query.Where(x => x.Code != null && x.Code.Contains(val));
                            break;
                        case "name":
                            query = query.Where(x => x.Name != null && x.Name.Contains(val));
                            break;
                        case "standardreference":
                        case "standard":
                            query = query.Where(x => x.StandardReference != null && x.StandardReference.Contains(val));
                            break;
                        case "standardorganizationid":
                            if (long.TryParse(val, out var orgId) && orgId > 0)
                                query = query.Where(x => x.StandardOrganizationID == orgId);
                            break;
                        case "isactive":
                        case "status":
                            if (bool.TryParse(val, out var activeVal))
                                query = query.Where(x => x.IsActive == activeVal);
                            else if (val.Equals("active", StringComparison.OrdinalIgnoreCase))
                                query = query.Where(x => x.IsActive == true);
                            else if (val.Equals("inactive", StringComparison.OrdinalIgnoreCase))
                                query = query.Where(x => x.IsActive == false);
                            break;
                    }
                }
            }

            // Global search term
            if (!string.IsNullOrWhiteSpace(filter.searchTerm))
            {
                var search = filter.searchTerm.Trim();
                query = query.Where(x => (x.Code != null && x.Code.Contains(search))
                                      || (x.Name != null && x.Name.Contains(search))
                                      || (x.StandardReference != null && x.StandardReference.Contains(search))
                                      || (x.StandardOrganizationName != null && x.StandardOrganizationName.Contains(search))
                                      || (x.Description != null && x.Description.Contains(search)));
            }

            // Sort
            if (!string.IsNullOrWhiteSpace(filter.SortByColumn))
            {
                query = query.OrderBy($"{filter.SortByColumn} {(filter.SortOrder == "asc" ? "ascending" : "descending")}");
            }
            else
            {
                query = query.OrderByDescending(x => x.ID);
            }

            return await query.Cast<object>().ToPagedAsync(filter);
        }

        public async Task<SpecificationHeader?> GetSpecificationMasterById(long id, string companyCode)
        {
            return await _context.SpecificationHeaders
                .Include(x => x.StandardOrganization)
                .Include(x => x.Grades)
                .FirstOrDefaultAsync(x => x.ID == id && x.CompanyCode == companyCode);
        }

        public async Task<bool> ExistsByCode(string code, string companyCode)
        {
            return await _context.SpecificationHeaders
                .AnyAsync(x => x.CompanyCode == companyCode && x.Code == code);
        }

        public async Task<bool> ExistsByCodeAndNotId(string code, long id, string companyCode)
        {
            return await _context.SpecificationHeaders
                .AnyAsync(x => x.CompanyCode == companyCode && x.Code == code && x.ID != id);
        }

        public async Task AddSpecificationMaster(SpecificationHeader entity)
        {
            await _context.SpecificationHeaders.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateSpecificationMaster(SpecificationHeader entity)
        {
            _context.SpecificationHeaders.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<List<DropdwonSelector>> GetSpecificationMasterDropdown(string? searchTerm, int pageNo, int pageSize, string companyCode)
        {
            var query = _context.SpecificationHeaders
                .Where(x => x.IsActive && x.CompanyCode == companyCode);

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.Trim();
                query = query.Where(x => (x.Code != null && x.Code.Contains(term))
                                      || x.AliasName.Contains(term)
                                      || (x.Title != null && x.Title.Contains(term)));
            }

            return await query
                .OrderBy(x => x.Code ?? x.AliasName)
                .Skip(pageNo * pageSize)
                .Take(pageSize)
                .Select(x => new DropdwonSelector
                {
                    Id = x.ID,
                    Name = !string.IsNullOrWhiteSpace(x.Code) ? $"{x.Code} - {x.AliasName}" : x.AliasName
                })
                .ToListAsync();
        }

        public async Task<List<DropdwonSelector>> GetStandardOrganizationsDropdown()
        {
            return await _context.StandardOrganizationMasters
                .Where(x => x.IsActive)
                .OrderBy(x => x.Name)
                .Select(x => new DropdwonSelector
                {
                    Id = x.ID,
                    Name = x.Name
                })
                .ToListAsync();
        }

        public async Task<int> GetVersionCount(long id)
        {
            return await _context.SpecificationVersions
                .CountAsync(x => x.SpecificationHeaderID == id);
        }

        public async Task<bool> HasDownstreamReferences(long id)
        {
            // Check UniversalTestGroups
            var hasUtg = await _context.UniversalTestGroups
                .AnyAsync(x => x.SpecificationHeaderID == id);
            if (hasUtg) return true;

            // Check LaboratoryTestAnalysisTypeSpecifications
            var hasLabSpec = await _context.LaboratoryTestAnalysisTypeSpecifications
                .AnyAsync(x => x.SpecificationHeaderID == id);
            if (hasLabSpec) return true;

            // Check ToleranceMasters
            var hasTolerance = await _context.ToleranceMasters
                .AnyAsync(x => x.SpecificationHeaderID == id);
            if (hasTolerance) return true;

            // Check if any sample details reference grades belonging to this specification
            var hasSamples = await _context.SampleDetails
                .AnyAsync(x => x.SpecificationGrade != null && x.SpecificationGrade.SpecificationHeaderID == id);
            if (hasSamples) return true;

            return false;
        }

        public async Task<List<SpecificationGrade>> GetGradesBySpecificationIdAsync(long specificationHeaderId, bool includeInactive = false)
        {
            var query = _context.SpecificationGrades
                .Where(g => g.SpecificationHeaderID == specificationHeaderId);

            if (!includeInactive)
            {
                query = query.Where(g => g.IsActive);
            }

            return await query.OrderBy(g => g.Grade).ToListAsync();
        }

        public async Task<SpecificationGrade?> GetGradeByIdAsync(long gradeId)
        {
            return await _context.SpecificationGrades
                .FirstOrDefaultAsync(g => g.ID == gradeId);
        }

        public async Task<bool> GradeExistsAsync(long specificationHeaderId, string grade, long excludeGradeId = 0)
        {
            var normalized = grade.Trim().ToLower();
            return await _context.SpecificationGrades
                .AnyAsync(g => g.SpecificationHeaderID == specificationHeaderId 
                            && g.ID != excludeGradeId 
                            && g.Grade.ToLower() == normalized);
        }

        public async Task AddGradeAsync(SpecificationGrade grade)
        {
            await _context.SpecificationGrades.AddAsync(grade);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateGradeAsync(SpecificationGrade grade)
        {
            _context.SpecificationGrades.Update(grade);
            await _context.SaveChangesAsync();
        }

        public async Task<int> GetGradeRequirementCountAsync(long gradeId)
        {
            return await _context.SpecificationLines
                .CountAsync(sl => sl.SpecificationGradeID == gradeId);
        }

        public async Task<bool> GradeHasDownstreamReferencesAsync(long gradeId)
        {
            if (await _context.SpecificationLines.AnyAsync(sl => sl.SpecificationGradeID == gradeId))
                return true;

            if (await _context.ProductMasterVersionGrades.AnyAsync(p => p.SpecificationGradeID == gradeId))
                return true;

            if (await _context.SampleDetails.AnyAsync(s => s.SpecificationGradeID == gradeId || s.AssignedGradeID == gradeId))
                return true;

            if (await _context.UniversalTestGroups.AnyAsync(u => u.SpecificationGradeID == gradeId))
                return true;

            return false;
        }
    }
}
