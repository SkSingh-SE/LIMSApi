using System.Linq.Dynamic.Core;
using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Repositories
{
    public class HeatTreatmentRepository : IHeatTreatmentRepository
    {
        private readonly LIMSContext _context;

        public HeatTreatmentRepository(LIMSContext context)
        {
            _context = context;
        }

        public async Task AddHeatTreatment(HeatTreatmentMaster model)
        {
            await _context.HeatTreatmentMasters.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteHeatTreatment(long id)
        {
            var existingHeatTreatment = await _context.HeatTreatmentMasters.FirstOrDefaultAsync(x => x.ID == id && x.IsActive);
            if (existingHeatTreatment != null)
            {
                existingHeatTreatment.IsActive = false;
                existingHeatTreatment.ModifiedOn = DateTime.UtcNow;
                _context.HeatTreatmentMasters.Update(existingHeatTreatment);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<HeatTreatmentMaster?> GetHeatTreatmentById(long id)
        {
            return await _context.HeatTreatmentMasters
                .Include(x => x.HeatTreatmentCategory)
                .Include(x => x.CoolingMedium)
                .Include(x => x.ApplicableClassifications).ThenInclude(ac => ac.MetalClassification)
                .FirstOrDefaultAsync(x => x.ID == id && x.IsActive);
        }

        public async Task UpdateHeatTreatment(HeatTreatmentMaster model)
        {
            _context.HeatTreatmentMasters.Update(model);
            await _context.SaveChangesAsync();
        }

        public async Task<PagedResponse<object>> GetAllHeatTreatments(PageFilter filter)
        {
            var _query = (from c in _context.HeatTreatmentMasters where c.IsActive select c).AsQueryable().ApplyFilters(filter.Filter);


            if (!string.IsNullOrWhiteSpace(filter.searchTerm))
            {
                var search = filter.searchTerm.Trim();
                _query = _query.Where(x => (x.Name != null && x.Name.Contains(search)) || (x.Code != null && x.Code.Contains(search)));
            }

            if (filter.SortByColumn != null)
            {
                _query = _query.OrderBy($"{filter.SortByColumn} {(filter.SortOrder == "asc" ? "ascending" : "descending")}");
            }

            return await _query.Cast<object>().ToPagedAsync(filter);
        }

        public async Task<List<DropdwonSelector>> GetHeatTreatmentDropdown(string? searchTerm, int pageNo = 0, int pageSize = 20)
        {
            if (pageNo < 0) pageNo = 0;

            var _query = from a in _context.HeatTreatmentMasters where a.IsActive select a;

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                if (FilterHelper.IsExactIdSearch(searchTerm, out long exactId))
                {
                    _query = _query.Where(x => x.ID == exactId);
                }
                else
                {
                    var search = searchTerm.Trim();
                    _query = _query.Where(x => (x.Name != null && x.Name.Contains(search)));
                }
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
            return await _context.HeatTreatmentMasters.AnyAsync(x => x.Name == name && x.IsActive);
        }

        public async Task<bool> ExistsByNameAndNotId(string name, long Id)
        {
            return await _context.HeatTreatmentMasters.AnyAsync(x => x.Name == name && x.ID != Id && x.IsActive);
        }

        public async Task<bool> ExistsByCode(string code)
        {
            return await _context.HeatTreatmentMasters.AnyAsync(x => x.Code == code && x.IsActive);
        }

        public async Task<bool> ExistsByCodeAndNotId(string code, long Id)
        {
            return await _context.HeatTreatmentMasters.AnyAsync(x => x.Code == code && x.ID != Id && x.IsActive);
        }
    }
}
