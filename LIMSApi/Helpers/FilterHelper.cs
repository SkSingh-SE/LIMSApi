using System.Diagnostics;
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
                if (property == null)
                {
                    Debug.WriteLine($"[FilterHelper] Column '{filter.Column}' not found on type '{typeof(T).Name}' — filter skipped.");
                    continue;
                }

                var propType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;

                if (propType == typeof(string))
                {
                    query = ApplyStringFilter(query, filter.Column, filter.Type, filter.Value);
                }
                else if (propType == typeof(int) || propType == typeof(decimal) || propType == typeof(double) || propType == typeof(long))
                {
                    query = ApplyNumberFilter(query, filter.Column, filter.Type, filter.Value);
                }
                else if (propType == typeof(DateTime))
                {
                    query = ApplyDateFilter(query, filter.Column, filter.Type, filter.Value, filter.Value2);
                }
                else if (propType == typeof(bool))
                {
                    query = ApplyBoolFilter(query, filter.Column, filter.Type, filter.Value);
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
            if (!decimal.TryParse(value, out decimal numericValue)) return query;

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
            dateValue = dateValue.Date; // Strip time component

            switch (type)
            {
                case "Equal":
                    // Match entire day: >= start of day AND < next day
                    return query.Where($"{column} >= @0 && {column} < @1", dateValue, dateValue.AddDays(1));
                case "NotEqual":
                    return query.Where($"{column} < @0 || {column} >= @1", dateValue, dateValue.AddDays(1));
                case "GreaterThan":
                    return query.Where($"{column} >= @0", dateValue.AddDays(1));
                case "LessThan":
                    return query.Where($"{column} < @0", dateValue);
                case "Between":
                    if (!string.IsNullOrWhiteSpace(value2) && DateTime.TryParse(value2, out DateTime dateValue2))
                    {
                        dateValue2 = dateValue2.Date;
                        // Include entire end date day
                        return query.Where($"{column} >= @0 && {column} < @1", dateValue, dateValue2.AddDays(1));
                    }
                    break;
            }
            return query;
        }

        private static IQueryable<T> ApplyBoolFilter<T>(IQueryable<T> query, string column, string type, string value)
        {
            if (!TryParseBool(value, out bool boolValue)) return query;

            switch (type)
            {
                case "Equal":
                    return query.Where($"{column} == @0", boolValue);
                case "NotEqual":
                    return query.Where($"{column} != @0", boolValue);
                default:
                    return query;
            }
        }

        private static bool TryParseBool(string value, out bool result)
        {
            result = false;
            if (string.IsNullOrWhiteSpace(value)) return false;
            var v = value.Trim().ToLowerInvariant();
            if (v == "true" || v == "yes" || v == "1") { result = true; return true; }
            if (v == "false" || v == "no" || v == "0") { result = false; return true; }
            return false;
        }

        /// <summary>
        /// Dropdown helper: if searchTerm is a numeric ID, check for exact ID match.
        /// Used by SearchableDropdown rebinding — it passes the selected ID as searchTerm.
        /// Call this before applying name-based search in dropdown methods.
        /// </summary>
        public static bool IsExactIdSearch(string? searchTerm, out long exactId)
        {
            exactId = 0;
            if (string.IsNullOrWhiteSpace(searchTerm)) return false;
            var term = searchTerm.Trim();
            if (term.Equals("undefined", StringComparison.OrdinalIgnoreCase) ||
                term.Equals("null", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
            return long.TryParse(term, out exactId) && exactId > 0;
        }

        public static bool IsUserApprover(string assignedToValue, long userId)
        {
            if (string.IsNullOrWhiteSpace(assignedToValue))
                return false;

            return assignedToValue
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Any(v => long.TryParse(v, out var id) && id == userId);
        }

    }
}
