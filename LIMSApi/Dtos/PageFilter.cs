namespace LIMSApi.Dtos
{
    public class PageFilter
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? searchTerm { get; set; }
        public Dictionary<string, bool>? SortBy { get; set; } 
        public Dictionary<string, string>? Filters { get; set; }
    }
}
