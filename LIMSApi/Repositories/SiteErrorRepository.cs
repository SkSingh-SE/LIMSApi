using System.Linq.Dynamic.Core;
using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Repositories
{
    public class SiteErrorRepository : ISiteErrorRepository
    {
        private readonly LIMSContext _context;

        public SiteErrorRepository(LIMSContext context)
        {
            _context = context;
        }

        public async Task AddSiteError(SiteError model)
        {
            await _context.SiteErrors.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteSiteError(long id)
        {
            var existingSiteError = await _context.SiteErrors.FirstOrDefaultAsync(x => x.ID == id);
            if (existingSiteError != null)
            {
                existingSiteError.ModifiedOn = DateTime.UtcNow;
                _context.SiteErrors.Update(existingSiteError);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<SiteError?> GetSiteErrorById(long id)
        {
            return await _context.SiteErrors.FirstOrDefaultAsync(x => x.ID == id);
        }

        public async Task UpdateSiteError(SiteError model)
        {
            _context.SiteErrors.Update(model);
            await _context.SaveChangesAsync();
        }

        public async Task<PagedResponse<object>> GetAllSiteErrors(PageFilter filter)
        {
            var _query = from c in _context.SiteErrors select c;

            if (filter.Filters != null)
            {
                foreach (var filterItem in filter.Filters)
                {
                    if (string.IsNullOrWhiteSpace(filterItem.Value))
                    {
                        continue;
                    }
                    var propertyName = filterItem.Key;
                    var value = filterItem.Value;

                    _query = _query.Where($"{propertyName}.Contains(@0)", value);
                }
            }

            if (!string.IsNullOrWhiteSpace(filter.searchTerm))
            {
                var search = filter.searchTerm.Trim().ToLower();
                _query = _query.Where(x => (x.Description != null && x.Description.ToLower().Contains(search))
                                    );
            }

            if (filter.SortBy != null && filter.SortBy.Any())
            {
                var sortingExpressions = filter.SortBy
                   .Select(s => $"{s.Key} {(s.Value ? "descending" : "ascending")}");
                string orderByString = string.Join(", ", sortingExpressions);

                _query = _query.OrderBy(orderByString);
            }

            // Total Records Count
            int totalRecords = await _query.CountAsync();

            // Apply Pagination
            var items = await _query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return new PagedResponse<object>(items.Cast<object>().ToList(), totalRecords, filter.PageNumber, filter.PageSize);
        }

        
    }
}
