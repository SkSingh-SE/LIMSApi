namespace LIMSApi.Dtos
{
    public class PageFilter
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? searchTerm { get; set; }
        public string? SortByColumn { get; set; } = "ID";
        public string? SortOrder { get; set; } = "asc";
        public Dictionary<string, bool>? SortBy { get; set; } 
        public List<Filter>? Filter { get; set; }
    }
    public class Filter
    {
        public required string Column { get; set; }
        public required string Type { get; set; }
        public required string Value { get; set; }
        public string? Value2 { get; set; }
    }
}
