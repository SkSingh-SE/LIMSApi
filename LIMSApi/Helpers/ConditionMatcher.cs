using System;
using System.Text.RegularExpressions;

namespace LIMSApi.Helpers
{
    public static class ConditionMatcher
    {
        private static readonly Regex ConditionRegex = new Regex(
            @"^(<=|==|>=|<|>)\s*(\d+)$",
            RegexOptions.Compiled
        );

        /// <summary>
        /// Returns true if the configuration Value represents a Base Tier condition (starts with operators).
        /// </summary>
        public static bool IsBaseTier(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return false;
            return value.StartsWith("<=") || value.StartsWith("==") ||
                   value.StartsWith(">=") || value.StartsWith("<") ||
                   value.StartsWith(">");
        }

        /// <summary>
        /// Returns true if the configuration Value is the override sentinel string.
        /// </summary>
        public static bool IsOverride(string value)
        {
            return value == "override";
        }

        /// <summary>
        /// Matches an actual count against a condition string (e.g. "<=1").
        /// </summary>
        public static bool MatchesCount(string condition, int count)
        {
            if (string.IsNullOrWhiteSpace(condition)) return false;

            var match = ConditionRegex.Match(condition.Trim());
            if (!match.Success) return false;

            string op = match.Groups[1].Value;
            if (!int.TryParse(match.Groups[2].Value, out int targetValue)) return false;

            return op switch
            {
                "<=" => count <= targetValue,
                "==" => count == targetValue,
                ">=" => count >= targetValue,
                "<"  => count < targetValue,
                ">"  => count > targetValue,
                _    => false
            };
        }

        /// <summary>
        /// Parses the numeric count limit from a condition string (e.g. returns 3 for "<=3").
        /// </summary>
        public static int ParseCountLimit(string condition)
        {
            if (string.IsNullOrWhiteSpace(condition)) return 0;
            var match = ConditionRegex.Match(condition.Trim());
            if (match.Success && int.TryParse(match.Groups[2].Value, out int value))
            {
                return value;
            }
            return 0;
        }
    }
}
