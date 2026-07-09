using System.Linq.Dynamic.Core;
using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Repositories
{
    public class ChemicalSampleCategoryRepository : IChemicalSampleCategoryRepository
    {
        private readonly LIMSContext _context;
        private LoggedInUserDTO loggedInUser;

        public ChemicalSampleCategoryRepository(LIMSContext context)
        {
            _context = context;
            loggedInUser = LoggedInUserProvider.CurrentUser;
        }

        public async Task Add(ChemicalSampleCategory model)
        {
            await _context.ChemicalSampleCategories.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task Update(ChemicalSampleCategory model)
        {
            _context.ChemicalSampleCategories.Update(model);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(ChemicalSampleCategory model)
        {
            _context.ChemicalSampleCategories.Update(model);
            await _context.SaveChangesAsync();
        }

        public async Task<ChemicalSampleCategory?> GetById(long id)
        {
            return await _context.ChemicalSampleCategories
                .FirstOrDefaultAsync(x => x.ID == id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);
        }

        public async Task<PagedResponse<object>> GetAll(PageFilter filter)
        {
            var query = (from c in _context.ChemicalSampleCategories
                         where c.IsActive && c.CompanyCode == loggedInUser.CompanyCode
                         select c).AsQueryable().ApplyFilters(filter.Filter);

            if (!string.IsNullOrWhiteSpace(filter.searchTerm))
            {
                var search = filter.searchTerm.Trim();
                query = query.Where(x => x.Name.Contains(search));
            }
            if (filter.SortByColumn != null)
            {
                query = query.OrderBy($"{filter.SortByColumn} {(filter.SortOrder == "asc" ? "ascending" : "descending")}");
            }

            var projected = query.Select(x => new
            {
                x.ID,
                x.Name,
                x.SortOrder,
                x.CreatedBy,
                x.CreatedOn,
                x.ModifiedBy,
                x.ModifiedOn,
                x.IsActive
            });

            return await projected.Cast<object>().ToPagedAsync(filter);
        }

        public async Task<List<DropdwonSelector>> GetDropdown(string? searchTerm, int pageNo = 0, int pageSize = 20)
        {
            if (pageNo < 0) pageNo = 0;

            var query = from a in _context.ChemicalSampleCategories
                        where a.IsActive && a.CompanyCode == loggedInUser.CompanyCode
                        select a;

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                if (FilterHelper.IsExactIdSearch(searchTerm, out long exactId))
                {
                    query = query.Where(x => x.ID == exactId);
                }
                else
                {
                    var search = searchTerm.Trim();
                    query = query.Where(x => x.Name.Contains(search));
                }
            }

            var skip = pageNo * pageSize;

            var data = await query.OrderBy(x => x.Name).Skip(skip).Take(pageSize)
                .Select(x => new DropdwonSelector { Id = x.ID, Name = x.Name })
                .ToListAsync();

            return data;
        }

        public async Task<bool> ExistsByName(string name)
        {
            return await _context.ChemicalSampleCategories.AnyAsync(
                x => x.Name == name && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);
        }

        public async Task<bool> ExistsByNameAndNotId(string name, long id)
        {
            return await _context.ChemicalSampleCategories.AnyAsync(
                x => x.Name == name && x.ID != id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);
        }
    }
}
