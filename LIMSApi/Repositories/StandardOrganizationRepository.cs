using System.Linq.Dynamic.Core;
using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Repositories
{
    public class StandardOrganizationRepository : IStandardOrganizationRepository
    {
        private readonly LIMSContext _context;

        public StandardOrganizationRepository(LIMSContext context)
        {
            _context = context;
        }

        public async Task AddStandardOrganization(StandardOrganizationMaster model)
        {
            await _context.StandardOrganizationMasters.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteStandardOrganization(long id)
        {
            var existingStandardOrganization = await _context.StandardOrganizationMasters.FirstOrDefaultAsync(x => x.ID == id && x.IsActive);
            if (existingStandardOrganization != null)
            {
                existingStandardOrganization.IsActive = false;
                existingStandardOrganization.ModifiedOn = DateTime.UtcNow;
                _context.StandardOrganizationMasters.Update(existingStandardOrganization);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<StandardOrganizationMaster?> GetStandardOrganizationById(long id)
        {
            return await _context.StandardOrganizationMasters.FirstOrDefaultAsync(x => x.ID == id && x.IsActive);
        }

        public async Task UpdateStandardOrganization(StandardOrganizationMaster model)
        {
            _context.StandardOrganizationMasters.Update(model);
            await _context.SaveChangesAsync();
        }

        public async Task<PagedResponse<object>> GetAllStandardOrganizations(PageFilter filter)
        {
            var _query = (from c in _context.StandardOrganizationMasters where c.IsActive select c).AsQueryable().ApplyFilters(filter.Filter);


            if (!string.IsNullOrWhiteSpace(filter.searchTerm))
            {
                var search = filter.searchTerm.Trim();
                _query = _query.Where(x =>
                    (x.Name != null && x.Name.Contains(search))
                    || (x.NumberType != null && x.NumberType.Contains(search))
                );
            }

            if (filter.SortByColumn != null)
            {
                _query = _query.OrderBy($"{filter.SortByColumn} {(filter.SortOrder == "asc" ? "ascending" : "descending")}");
            }

            return await _query.Cast<object>().ToPagedAsync(filter);
        }

        public async Task<List<DropdwonSelector>> GetStandardOrganizationDropdown(string? searchTerm, int pageNo = 0, int pageSize = 20)
        {
            if (pageNo < 0) pageNo = 0;

            var _query = from a in _context.StandardOrganizationMasters where a.IsActive select a;

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

            var items = await _query.Skip(skip).Take(pageSize).Select(x => new { x.ID, x.Name, x.NumberType }).ToListAsync();

            var data = items.Select(x => new DropdwonSelector
            {
                Id = x.ID,
                Name = x.Name,
                AdditionalValues = new Dictionary<string, object> { { "numberType", x.NumberType ?? "None" } }
            }).ToList();

            return data;
        }

        public async Task<bool> ExistsByName(string name)
        {
            return await _context.StandardOrganizationMasters.AnyAsync(x => x.Name == name && x.IsActive);
        }

        public async Task<bool> ExistsByNameAndNotId(string name, long Id)
        {
            return await _context.StandardOrganizationMasters.AnyAsync(x => x.Name == name && x.ID != Id && x.IsActive);
        }
    }
}
