using LIMSApi.Dtos;

namespace LIMSApi.Helpers
{
    public static class PageFilterNormalizer
    {
        public static void Normalize(PageFilter filter)
        {
            if (filter == null) return;
            if (filter.PageNumber < 1) filter.PageNumber = 1;
            if (filter.PageSize < 1) filter.PageSize = 10;
            if (filter.PageSize > 500) filter.PageSize = 500;
        }
    }
}
