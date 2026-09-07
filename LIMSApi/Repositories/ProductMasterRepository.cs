using System.Linq.Dynamic.Core;
using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Repositories
{
    public class ProductMasterRepository : IProductMasterRepository
    {
        private readonly LIMSContext _context;
        private readonly LoggedInUserDTO _loggedInUser;

        public ProductMasterRepository(LIMSContext context)
        {
            _context = context;
            _loggedInUser = LoggedInUserProvider.CurrentUser;
        }

        public async Task Add(ProductMaster model)
        {
            await _context.ProductMasters.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task Update(ProductMaster model)
        {
            _context.ProductMasters.Update(model);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(ProductMaster model)
        {
            model.IsActive = false;
            _context.ProductMasters.Update(model);
            await _context.SaveChangesAsync();
        }

        public async Task<ProductMaster?> GetById(long id)
        {
            return await _context.ProductMasters
                .Include(x => x.ProductSizeMaster)
                .Include(x => x.MetalClassifications)
                .Include(x => x.Versions)
                    .ThenInclude(v => v.Grades)
                        .ThenInclude(g => g.Conditions)
                .FirstOrDefaultAsync(x => x.ID == id && x.CompanyCode == _loggedInUser.CompanyCode);
        }

        public async Task<ProductMaster?> GetDetailsById(long id)
        {
            return await _context.ProductMasters
                .AsSplitQuery()
                .Include(x => x.ProductSizeMaster)
                .Include(x => x.MetalClassifications)
                    .ThenInclude(mc => mc.MetalClassification)
                .Include(x => x.Versions)
                    .ThenInclude(v => v.StandardOrganization)
                .Include(x => x.Versions)
                    .ThenInclude(v => v.Grades)
                        .ThenInclude(g => g.SpecificationGrade)
                            .ThenInclude(sg => sg.SpecificationHeader)
                .Include(x => x.Versions)
                    .ThenInclude(v => v.Grades)
                        .ThenInclude(g => g.Conditions)
                            .ThenInclude(c => c.ProductCondition1)
                .Include(x => x.Versions)
                    .ThenInclude(v => v.Grades)
                        .ThenInclude(g => g.Conditions)
                            .ThenInclude(c => c.ProductCondition2)
                .Include(x => x.Versions)
                    .ThenInclude(v => v.Grades)
                        .ThenInclude(g => g.Conditions)
                            .ThenInclude(c => c.HeatTreatment)
                .Include(x => x.Versions)
                    .ThenInclude(v => v.Grades)
                        .ThenInclude(g => g.Conditions)
                            .ThenInclude(c => c.ProductSizeMaster)
                .FirstOrDefaultAsync(x => x.ID == id && x.CompanyCode == _loggedInUser.CompanyCode);
        }

        public async Task<PagedResponse<object>> GetAll(PageFilter filter)
        {
            var query = _context.ProductMasters
                .AsNoTracking()
                .Where(x => x.CompanyCode == _loggedInUser.CompanyCode)
                .AsQueryable();

            bool isActiveFilterApplied = false;
            if (filter.Filter != null && filter.Filter.Any())
            {
                foreach (var f in filter.Filter)
                {
                    if (string.IsNullOrWhiteSpace(f.Value)) continue;
                    var val = f.Value.Trim();
                    switch (f.Column.ToLowerInvariant())
                    {
                        case "isactive":
                        case "status":
                            isActiveFilterApplied = true;
                            if (bool.TryParse(val, out var bVal))
                                query = query.Where(x => x.IsActive == bVal);
                            else if (val.Equals("active", StringComparison.OrdinalIgnoreCase))
                                query = query.Where(x => x.IsActive == true);
                            else if (val.Equals("inactive", StringComparison.OrdinalIgnoreCase))
                                query = query.Where(x => x.IsActive == false);
                            // If "all", do not filter by IsActive
                            break;
                        case "productname":
                        case "name":
                            query = query.Where(x => x.ProductName != null && x.ProductName.Contains(val));
                            break;
                        case "applicability":
                        case "grade":
                            query = query.Where(x => x.Versions.Any(v => v.IsActiveVersion && v.Grades.Any(g => g.IsActive && g.SpecificationGrade != null && g.SpecificationGrade.Grade.Contains(val))));
                            break;
                        case "specification":
                        case "governingspecification":
                            query = query.Where(x => x.Versions.Any(v => v.IsActiveVersion && v.Grades.Any(g => g.IsActive && g.SpecificationGrade != null && g.SpecificationGrade.SpecificationHeader != null && ((g.SpecificationGrade.SpecificationHeader.AliasName != null && g.SpecificationGrade.SpecificationHeader.AliasName.Contains(val)) || (g.SpecificationGrade.SpecificationHeader.SpecificationNo != null && g.SpecificationGrade.SpecificationHeader.SpecificationNo.Contains(val))))));
                            break;
                    }
                }
            }

            if (!isActiveFilterApplied)
            {
                query = query.Where(x => x.IsActive);
            }

            if (!string.IsNullOrWhiteSpace(filter.searchTerm))
            {
                var search = filter.searchTerm.Trim();
                query = query.Where(x =>
                    (x.ProductName != null && x.ProductName.Contains(search)) ||
                    (x.DisplayTitle != null && x.DisplayTitle.Contains(search)) ||
                    (x.GradePrefix != null && x.GradePrefix.Contains(search)) ||
                    (x.GradeValue != null && x.GradeValue.Contains(search)) ||
                    (x.ProductSizeMaster != null && x.ProductSizeMaster.DisplayName.Contains(search)) ||
                    x.Versions.Any(v => v.IsActiveVersion && v.Grades.Any(g => g.IsActive && g.SpecificationGrade != null && g.SpecificationGrade.Grade.Contains(search))) ||
                    x.Versions.Any(v => v.IsActiveVersion && v.Grades.Any(g => g.IsActive && g.SpecificationGrade != null && g.SpecificationGrade.SpecificationHeader != null && ((g.SpecificationGrade.SpecificationHeader.AliasName != null && g.SpecificationGrade.SpecificationHeader.AliasName.Contains(search)) || (g.SpecificationGrade.SpecificationHeader.SpecificationNo != null && g.SpecificationGrade.SpecificationHeader.SpecificationNo.Contains(search)))))
                );
            }

            var totalRecords = await query.CountAsync();

            if (!string.IsNullOrWhiteSpace(filter.SortByColumn))
            {
                query = query.OrderBy($"{filter.SortByColumn} {(filter.SortOrder == "asc" ? "ascending" : "descending")}");
            }
            else
            {
                query = query.OrderByDescending(x => x.ID);
            }

            int pageNo = filter.PageNumber < 1 ? 1 : filter.PageNumber;
            int pageSize = filter.PageSize < 1 ? 10 : filter.PageSize;
            int totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
            if (pageNo > totalPages && totalPages > 0) pageNo = 1;
            int skip = (pageNo - 1) * pageSize;

            var pagedEntities = await query
                .Include(x => x.ProductSizeMaster)
                .Include(x => x.Versions)
                    .ThenInclude(v => v.Grades.Where(g => g.IsActive))
                        .ThenInclude(g => g.SpecificationGrade)
                            .ThenInclude(sg => sg.SpecificationHeader)
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();

            var items = pagedEntities.Select(x =>
            {
                var activeVer = x.Versions.FirstOrDefault(v => v.IsActiveVersion) ?? x.Versions.FirstOrDefault();
                var grades = activeVer?.Grades?.Where(g => g.IsActive && g.SpecificationGrade != null).ToList() ?? new List<ProductMasterVersionGrade>();
                var applicabilities = grades.Select(g => g.SpecificationGrade!.Grade).Where(s => !string.IsNullOrEmpty(s)).Distinct().ToList();
                var specs = grades.Select(g => g.SpecificationGrade?.SpecificationHeader != null ? (g.SpecificationGrade.SpecificationHeader.AliasName ?? g.SpecificationGrade.SpecificationHeader.SpecificationNo ?? "") : "").Where(s => !string.IsNullOrEmpty(s)).Distinct().ToList();

                return new ProductMasterListDto
                {
                    ID = x.ID,
                    ProductName = x.ProductName,
                    GradePrefix = x.GradePrefix,
                    GradeValue = x.GradeValue,
                    DisplayTitle = x.DisplayTitle,
                    IsSizeApplicable = x.IsSizeApplicable,
                    ProductSizeName = x.ProductSizeMaster?.DisplayName,
                    ActiveVersionNo = activeVer != null ? activeVer.VersionNumber.ToString() : "1",
                    VersionCount = x.Versions.Count,
                    ApplicabilityCount = grades.Count,
                    Applicabilities = applicabilities,
                    ApplicabilitySummary = string.Join(", ", applicabilities),
                    LinkedSpecsSummary = string.Join(", ", applicabilities),
                    GoverningSpecifications = specs,
                    GoverningSpecificationsSummary = string.Join(", ", specs),
                    CreatedOn = x.CreatedOn,
                    IsActive = x.IsActive
                };
            }).Cast<object>().ToList();

            return new PagedResponse<object>(items, totalRecords, pageNo, pageSize);
        }

        public async Task<List<DropdwonSelector>> GetDropdown(string? searchTerm, int pageNo = 0, int pageSize = 20, long metalId = 0)
        {
            if (pageNo < 0) pageNo = 0;

            var query = _context.ProductMasters
                .Where(x => x.IsActive && x.CompanyCode == _loggedInUser.CompanyCode);

            if (metalId > 0)
            {
                query = query.Where(x => x.MetalClassifications.Any(m => m.MetalClassificationID == metalId)
                    || !x.MetalClassifications.Any());
            }

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                if (FilterHelper.IsExactIdSearch(searchTerm, out long exactId))
                {
                    query = query.Where(x => x.ID == exactId);
                }
                else
                {
                    var search = searchTerm.Trim();
                    query = query.Where(x => x.ProductName.Contains(search) || (x.DisplayTitle != null && x.DisplayTitle.Contains(search)));
                }
            }

            var skip = pageNo * pageSize;
            return await query.OrderBy(x => x.ProductName)
                .Skip(skip)
                .Take(pageSize)
                .Select(x => new DropdwonSelector
                {
                    Id = x.ID,
                    Name = !string.IsNullOrEmpty(x.DisplayTitle) ? x.DisplayTitle : x.ProductName
                })
                .ToListAsync();
        }

        public async Task<bool> ExistsByName(string productName)
        {
            return await _context.ProductMasters.AnyAsync(x => x.ProductName == productName && x.IsActive && x.CompanyCode == _loggedInUser.CompanyCode);
        }

        public async Task<bool> ExistsByNameAndNotId(string productName, long id)
        {
            return await _context.ProductMasters.AnyAsync(x => x.ProductName == productName && x.ID != id && x.IsActive && x.CompanyCode == _loggedInUser.CompanyCode);
        }
    }
}
