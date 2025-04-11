using System.Linq.Dynamic.Core;
using LIMSApi.Dtos;

namespace LIMSApi.Helpers
{
    public static class FilterHelper
    {
        public static IQueryable<T> ApplyFilters<T>(this IQueryable<T> query, List<Filter> filters)
        {
            if (filters == null || !filters.Any())
            {
                return query;
            }

            foreach (var filter in filters)
            {
                if (string.IsNullOrWhiteSpace(filter.Value)) continue;

                var property = typeof(T)
            .GetProperties()
            .FirstOrDefault(p => p.Name.Equals(filter.Column, StringComparison.OrdinalIgnoreCase));
                if (property == null) continue;

                if (property.PropertyType == typeof(string))
                {
                    query = ApplyStringFilter(query, filter.Column, filter.Type, filter.Value);
                }
                else if (property.PropertyType == typeof(int) || property.PropertyType == typeof(decimal) || property.PropertyType == typeof(double))
                {
                    query = ApplyNumberFilter(query, filter.Column, filter.Type, filter.Value);
                }
                else if (property.PropertyType == typeof(DateTime))
                {
                    query = ApplyDateFilter(query, filter.Column, filter.Type, filter.Value, filter.Value2);
                }
            }

            return query;
        }

        private static IQueryable<T> ApplyStringFilter<T>(IQueryable<T> query, string column, string type, string value)
        {
            switch (type)
            {
                case "Contains":
                    return query.Where($"{column}.Contains(@0)", value);
                case "Equal":
                    return query.Where($"{column} == @0", value);
                case "NotEqual":
                    return query.Where($"{column} != @0", value);
                default:
                    return query;
            }
        }

        private static IQueryable<T> ApplyNumberFilter<T>(IQueryable<T> query, string column, string type, string value)
        {
            if (!int.TryParse(value, out int numericValue)) return query;

            switch (type)
            {
                case "Equal":
                    return query.Where($"{column} == @0", numericValue);
                case "NotEqual":
                    return query.Where($"{column} != @0", numericValue);
                case "GreaterThan":
                    return query.Where($"{column} > @0", numericValue);
                case "LessThan":
                    return query.Where($"{column} < @0", numericValue);
                default:
                    return query;
            }
        }

        private static IQueryable<T> ApplyDateFilter<T>(IQueryable<T> query, string column, string type, string value, string? value2)
        {
            if (!DateTime.TryParse(value, out DateTime dateValue)) return query;

            switch (type)
            {
                case "Equal":
                    return query.Where($"{column} == @0", dateValue);
                case "NotEqual":
                    return query.Where($"{column} != @0", dateValue);
                case "GreaterThan":
                    return query.Where($"{column} > @0", dateValue);
                case "LessThan":
                    return query.Where($"{column} < @0", dateValue);
                case "Between":
                    if (!string.IsNullOrWhiteSpace(value2) && DateTime.TryParse(value2, out DateTime dateValue2))
                    {
                        return query.Where($"{column} >= @0 && {column} <= @1", dateValue, dateValue2);
                    }
                    break;
            }
            return query;
        }
    }
}
