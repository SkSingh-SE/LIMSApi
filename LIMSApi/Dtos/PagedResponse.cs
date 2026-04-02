namespace LIMSApi.Dtos
{
    public class PagedResponse<T>
    {
        public List<T> Items { get; set; }
        public int TotalRecords { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }

        public PagedResponse(List<T> items, int totalRecords, int pageNumber, int pageSize)
        {
            Items = items;
            TotalRecords = totalRecords;
            PageSize = pageSize < 1 ? 10 : pageSize;
            TotalPages = (int)Math.Ceiling((double)totalRecords / PageSize);

            if (pageNumber < 1) pageNumber = 1;
            PageNumber = (pageNumber > TotalPages && TotalPages > 0) ? 1 : pageNumber;
        }
    }
}
