namespace LIMSApi.IntegrationTests.Helpers;

/// <summary>
/// Fluent builder for constructing PageFilter payloads.
/// </summary>
public class PageFilterBuilder
{
    private int _pageNumber = 1;
    private int _pageSize = 10;
    private string? _searchTerm;
    private string _sortByColumn = "ID";
    private string _sortOrder = "desc";
    private readonly List<object> _filters = new();

    public PageFilterBuilder Page(int number, int size = 10)
    {
        _pageNumber = number;
        _pageSize = size;
        return this;
    }

    public PageFilterBuilder Search(string term)
    {
        _searchTerm = term;
        return this;
    }

    public PageFilterBuilder SortBy(string column, string order = "asc")
    {
        _sortByColumn = column;
        _sortOrder = order;
        return this;
    }

    public PageFilterBuilder AddFilter(string column, string type, string value, string? value2 = null)
    {
        _filters.Add(new { column, type, value, value2 });
        return this;
    }

    public PageFilterBuilder Contains(string column, string value) =>
        AddFilter(column, "Contains", value);

    public PageFilterBuilder Equal(string column, string value) =>
        AddFilter(column, "Equal", value);

    public PageFilterBuilder Between(string column, string from, string to) =>
        AddFilter(column, "Between", from, to);

    public object Build() => new
    {
        pageNumber = _pageNumber,
        pageSize = _pageSize,
        searchTerm = _searchTerm,
        sortByColumn = _sortByColumn,
        sortOrder = _sortOrder,
        filter = _filters
    };
}
