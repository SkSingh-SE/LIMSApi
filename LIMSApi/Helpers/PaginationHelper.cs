using LIMSApi.Dtos;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Helpers
{
    public static class PaginationHelper
    {
        public static async Task<PagedResponse<T>> ToPagedAsync<T>(this IQueryable<T> query, PageFilter filter)
        {
            if (filter.PageNumber < 1) filter.PageNumber = 1;
            if (filter.PageSize < 1) filter.PageSize = 10;
            if (filter.PageSize > 500) filter.PageSize = 500;

            int totalRecords = await query.CountAsync();
            int totalPages = (int)Math.Ceiling((double)totalRecords / filter.PageSize);

            if (filter.PageNumber > totalPages && totalPages > 0)
                filter.PageNumber = 1;

            var items = await query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return new PagedResponse<T>(items, totalRecords, filter.PageNumber, filter.PageSize);
        }
    }
}
