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
                .FirstOrDefaultAsync(x => x.ID == id && x.IsActive && x.CompanyCode == _loggedInUser.CompanyCode);
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
                .FirstOrDefaultAsync(x => x.ID == id && x.IsActive && x.CompanyCode == _loggedInUser.CompanyCode);
        }

        public async Task<PagedResponse<object>> GetAll(PageFilter filter)
        {
            var query = _context.ProductMasters
                .Include(x => x.ProductSizeMaster)
                .Include(x => x.Versions)
                    .ThenInclude(v => v.Grades)
                        .ThenInclude(g => g.SpecificationGrade)
                .Where(x => x.IsActive && x.CompanyCode == _loggedInUser.CompanyCode)
                .AsQueryable()
                .ApplyFilters(filter.Filter);

            if (!string.IsNullOrWhiteSpace(filter.searchTerm))
            {
                var search = filter.searchTerm.Trim();
                query = query.Where(x =>
                    (x.ProductName != null && x.ProductName.Contains(search)) ||
                    (x.DisplayTitle != null && x.DisplayTitle.Contains(search)) ||
                    (x.GradePrefix != null && x.GradePrefix.Contains(search)) ||
                    (x.ProductSizeMaster != null && x.ProductSizeMaster.DisplayName.Contains(search))
                );
            }

            if (!string.IsNullOrWhiteSpace(filter.SortByColumn))
            {
                query = query.OrderBy($"{filter.SortByColumn} {(filter.SortOrder == "asc" ? "ascending" : "descending")}");
            }

            var projected = query.Select(x => new
            {
                x.ID,
                x.ProductName,
                x.GradePrefix,
                x.DisplayTitle,
                x.IsSizeApplicable,
                ProductSizeName = x.ProductSizeMaster != null ? x.ProductSizeMaster.DisplayName : null,
                ActiveVersionNo = x.Versions.Where(v => v.IsActiveVersion).Select(v => v.VersionNumber.ToString()).FirstOrDefault() ?? "1",
                LinkedSpecsSummary = string.Join(", ", x.Versions.Where(v => v.IsActiveVersion).SelectMany(v => v.Grades).Select(g => g.SpecificationGrade != null ? g.SpecificationGrade.Grade : "").Where(s => !string.IsNullOrEmpty(s))),
                x.CreatedBy,
                x.CreatedOn,
                x.IsActive
            });

            return await projected.Cast<object>().ToPagedAsync(filter);
        }

        public async Task<List<DropdwonSelector>> GetDropdown(string? searchTerm, int pageNo = 0, int pageSize = 20)
        {
            if (pageNo < 0) pageNo = 0;

            var query = _context.ProductMasters
                .Where(x => x.IsActive && x.CompanyCode == _loggedInUser.CompanyCode);

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
