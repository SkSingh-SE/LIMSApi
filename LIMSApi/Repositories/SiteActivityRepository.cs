using System.Linq.Dynamic.Core;
using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Repositories
{
    public class SiteActivityRepository : ISiteActivityRepository
    {
        private readonly LIMSContext _context;

        public SiteActivityRepository(LIMSContext context)
        {
            _context = context;
        }

        public async Task AddSiteActivity(SiteActivity  model)
        {
            await _context.SiteActivities.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task AddMultipleSiteActivities(List<SiteActivity> activities)
        {
            await _context.SiteActivities.AddRangeAsync(activities);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteSiteActivity(long id)
        {
            var existingSiteActivity = await _context.SiteActivities.FirstOrDefaultAsync(x => x.ID == id);
            if (existingSiteActivity != null)
            {
                existingSiteActivity.ModifiedOn = DateTime.UtcNow;
                _context.SiteActivities.Update(existingSiteActivity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<SiteActivity?> GetSiteActivityById(long id)
        {
            return await _context.SiteActivities.FirstOrDefaultAsync(x => x.ID == id);
        }

        public async Task UpdateSiteActivity(SiteActivity model)
        {
            _context.SiteActivities.Update(model);
            await _context.SaveChangesAsync();
        }

        public async Task<PagedResponse<object>> GetAllSiteActivitys(PageFilter filter)
        {
            var _query = from c in _context.SiteActivities select c;

            _query = _query.AsQueryable().ApplyFilters(filter.Filter);

            if (!string.IsNullOrWhiteSpace(filter.searchTerm))
            {
                var search = filter.searchTerm.Trim();
                _query = _query.Where(x => (x.Description != null && x.Description.Contains(search))
                                    );
            }

            if (filter.SortByColumn != null)
            {
                _query = _query.OrderBy($"{filter.SortByColumn} {(filter.SortOrder == "asc" ? "ascending" : "descending")}");
            }

            return await _query.Cast<object>().ToPagedAsync(filter);
        }

        
    }
}
