using System.Linq.Dynamic.Core;
using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Repositories
{
    public class SpecimenOrientationRepository : ISpecimenOrientationRepository
    {
        private readonly LIMSContext _context;

        public SpecimenOrientationRepository(LIMSContext context)
        {
            _context = context;
        }

        public async Task AddSpecimenOrientation(SpecimenOrientationMaster model)
        {
            await _context.SpecimenOrientationMasters.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteSpecimenOrientation(long id)
        {
            var existingSpecimenOrientation = await _context.SpecimenOrientationMasters.FirstOrDefaultAsync(x => x.ID == id && x.IsActive);
            if (existingSpecimenOrientation != null)
            {
                existingSpecimenOrientation.IsActive = false;
                existingSpecimenOrientation.ModifiedOn = DateTime.UtcNow;
                _context.SpecimenOrientationMasters.Update(existingSpecimenOrientation);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<SpecimenOrientationMaster?> GetSpecimenOrientationById(long id)
        {
            return await _context.SpecimenOrientationMasters
                .Include(x => x.SpecimenOrientationCategory)
                .Include(x => x.ApplicableForms).ThenInclude(af => af.ProductForm)
                .Include(x => x.ApplicableClassifications).ThenInclude(ac => ac.MetalClassification)
                .FirstOrDefaultAsync(x => x.ID == id && x.IsActive);
        }

        public async Task UpdateSpecimenOrientation(SpecimenOrientationMaster model)
        {
            _context.SpecimenOrientationMasters.Update(model);
            await _context.SaveChangesAsync();
        }

        public async Task<PagedResponse<object>> GetAllSpecimenOrientations(PageFilter filter)
        {
            var _query = (from c in _context.SpecimenOrientationMasters where c.IsActive select c).AsQueryable().ApplyFilters(filter.Filter);


            if (!string.IsNullOrWhiteSpace(filter.searchTerm))
            {
                if (FilterHelper.IsExactIdSearch(filter.searchTerm, out long exactId))
                {
                    _query = _query.Where(x => x.ID == exactId);
                }
                else
                {
                    var search = filter.searchTerm.Trim();
                    _query = _query.Where(x => (x.Name != null && x.Name.Contains(search)) || (x.Code != null && x.Code.Contains(search)));
                }
            }

            if (filter.SortByColumn != null)
            {
                _query = _query.OrderBy($"{filter.SortByColumn} {(filter.SortOrder == "asc" ? "ascending" : "descending")}");
            }

            return await _query.Cast<object>().ToPagedAsync(filter);
        }

        public async Task<List<DropdwonSelector>> GetSpecimenOrientationDropdown(string? searchTerm, int pageNo = 0, int pageSize = 20)
        {
            if (pageNo < 0) pageNo = 0;

            var _query = from a in _context.SpecimenOrientationMasters where a.IsActive select a;

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var search = searchTerm.Trim();
                if (long.TryParse(search, out var searchId))
                    _query = _query.Where(x => x.ID == searchId || (x.Name != null && x.Name.Contains(search)));
                else
                    _query = _query.Where(x => x.Name != null && x.Name.Contains(search));
            }

            var skip = pageNo * pageSize;

            var data = await (_query.Skip(skip).Take(pageSize).Select(x => new DropdwonSelector
            {
                Id = x.ID,
                Name = x.Name,
            })).ToListAsync();

            return data;
        }

        public async Task<bool> ExistsByName(string name)
        {
            return await _context.SpecimenOrientationMasters.AnyAsync(x => x.Name == name && x.IsActive);
        }

        public async Task<bool> ExistsByNameAndNotId(string name, long Id)
        {
            return await _context.SpecimenOrientationMasters.AnyAsync(x => x.Name == name && x.ID != Id && x.IsActive);
        }

        public async Task<bool> ExistsByCode(string code)
        {
            return await _context.SpecimenOrientationMasters.AnyAsync(x => x.Code == code && x.IsActive);
        }

        public async Task<bool> ExistsByCodeAndNotId(string code, long id)
        {
            return await _context.SpecimenOrientationMasters.AnyAsync(x => x.Code == code && x.ID != id && x.IsActive);
        }
    }
}
