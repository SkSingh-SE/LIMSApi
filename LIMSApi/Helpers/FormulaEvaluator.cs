using NCalc;
using System.Text.RegularExpressions;

namespace LIMSApi.Helpers
{
    public class FormulaEvaluator
    {
        // Matches {P12}, {P999} — stored formula token format
        private static readonly Regex ParamTokenRegex = new Regex(
            @"\{P(\d+)\}",
            RegexOptions.Compiled
        );

        // Matches MEAN/AVG/MAX/MIN/STDEV/SUM/COUNT aggregate functions
        private static readonly Regex AggregateRegex = new Regex(
            @"(MEAN|AVG|MAX|MIN|STDEV|SUM|COUNT)\(([^)]+)\)",
            RegexOptions.IgnoreCase | RegexOptions.Compiled
        );

        // ──────────────────────────────────────────────────
        // Public: Evaluate
        // ──────────────────────────────────────────────────

        /// <summary>
        /// Primary overload: evaluates formula like "{P12}+({P15}/6)" using paramId→value map.
        /// </summary>
        public double? Evaluate(string expression, IDictionary<long, double> paramValues)
        {
            if (string.IsNullOrWhiteSpace(expression)) return null;
            try
            {
                var namedValues = paramValues.ToDictionary(kv => $"P{kv.Key}", kv => kv.Value);
                string ncalcExpr = ConvertToNCalcExpression(expression);
                ncalcExpr = PreProcessAggregates(ncalcExpr, namedValues);

                var exp = new Expression(ncalcExpr, EvaluateOptions.IgnoreCase);
                foreach (var kv in namedValues)
                    exp.Parameters[kv.Key] = kv.Value;

                var result = exp.Evaluate();
                return ToDouble(result);
            }
            catch { return null; }
        }

        /// <summary>
        /// Backward-compatible overload: accepts IDictionary&lt;string, double&gt; where keys are "P12", "P15" etc.
        /// </summary>
        public double? Evaluate(string expression, IDictionary<string, double> variables)
        {
            if (string.IsNullOrWhiteSpace(expression)) return null;
            try
            {
                string ncalcExpr = ConvertToNCalcExpression(expression);
                ncalcExpr = PreProcessAggregates(ncalcExpr, variables);

                var exp = new Expression(ncalcExpr, EvaluateOptions.IgnoreCase);
                foreach (var kv in variables)
                    exp.Parameters[kv.Key] = kv.Value;

                var result = exp.Evaluate();
                return ToDouble(result);
            }
            catch { return null; }
        }

        // ──────────────────────────────────────────────────
        // Public: ValidateFormula
        // ──────────────────────────────────────────────────

        /// <summary>
        /// Validates formula expression using the set of valid parameter IDs from the database.
        /// Returns null if valid; returns an error message string if invalid.
        /// </summary>
        public string? ValidateFormula(string expression, IEnumerable<long> validParamIds)
        {
            if (string.IsNullOrWhiteSpace(expression))
                return "Formula expression cannot be empty.";

            var validSet = new HashSet<long>(validParamIds);
            var tokenMatches = ParamTokenRegex.Matches(expression);

            if (!tokenMatches.Any())
                return "Formula must contain at least one parameter reference (e.g. {P12}).";

            // Validate each referenced param ID exists
            foreach (Match m in tokenMatches)
            {
                long paramId = long.Parse(m.Groups[1].Value);
                if (!validSet.Contains(paramId))
                    return $"Invalid parameter reference: P{paramId} does not exist.";
            }

            // Dry-run with dummy values to catch syntax errors
            var dummyValues = tokenMatches
                .Select(m => long.Parse(m.Groups[1].Value))
                .Distinct()
                .ToDictionary(id => $"P{id}", _ => 1.0);

            try
            {
                string ncalcExpr = ConvertToNCalcExpression(expression);
                ncalcExpr = PreProcessAggregates(ncalcExpr, dummyValues);

                var exp = new Expression(ncalcExpr);
                foreach (var kv in dummyValues)
                    exp.Parameters[kv.Key] = kv.Value;

                exp.Evaluate();
                return null; // valid
            }
            catch (Exception ex)
            {
                return $"Formula syntax error: {ex.Message}";
            }
        }

        /// <summary>
        /// Extracts all unique parameter IDs referenced in a formula.
        /// e.g. "{P12}+({P15}/6)" → [12, 15]
        /// </summary>
        public IEnumerable<long> ExtractParamIds(string expression)
        {
            if (string.IsNullOrWhiteSpace(expression))
                return Enumerable.Empty<long>();

            return ParamTokenRegex.Matches(expression)
                .Select(m => long.Parse(m.Groups[1].Value))
                .Distinct()
                .ToList();
        }

        // ──────────────────────────────────────────────────
        // Public: DetermineResultStatus
        // ──────────────────────────────────────────────────

        /// <summary>
        /// Pass / Fail / Marginal — Marginal = within 5% of spec boundary.
        /// </summary>
        public string? DetermineResultStatus(decimal? value, decimal? specMin, decimal? specMax)
        {
            if (!value.HasValue) return null;

            bool hasMin = specMin.HasValue;
            bool hasMax = specMax.HasValue;
            if (!hasMin && !hasMax) return null;

            bool withinMin = !hasMin || value >= specMin;
            bool withinMax = !hasMax || value <= specMax;
            if (!withinMin || !withinMax) return "Fail";

            if (hasMin && hasMax)
            {
                decimal range = specMax.Value - specMin.Value;
                if (range > 0)
                {
                    decimal marginThreshold = range * 0.05m;
                    bool nearMin = (value.Value - specMin.Value) <= marginThreshold;
                    bool nearMax = (specMax.Value - value.Value) <= marginThreshold;
                    if (nearMin || nearMax) return "Marginal";
                }
            }

            return "Pass";
        }

        // ──────────────────────────────────────────────────
        // Private helpers
        // ──────────────────────────────────────────────────

        /// <summary>
        /// Converts "{P12}+({P15}/6)" → "P12+(P15/6)" for NCalc, and cleans % prefixes and target assignments.
        /// </summary>
        private static string ConvertToNCalcExpression(string formula)
        {
            if (string.IsNullOrWhiteSpace(formula)) return string.Empty;
            string expr = ParamTokenRegex.Replace(formula, m => $"P{m.Groups[1].Value}");

            // Strip assignment if present (e.g. "CE = C + Mn / 6")
            int eqIdx = expr.IndexOf('=');
            if (eqIdx > 0 && !expr.StartsWith(">=") && !expr.StartsWith("<=") && !expr.StartsWith("==") && !expr.StartsWith("!="))
            {
                var left = expr.Substring(0, eqIdx).Trim();
                if (Regex.IsMatch(left, @"^[a-zA-Z_%][a-zA-Z0-9_ %]*$"))
                {
                    expr = expr.Substring(eqIdx + 1).Trim();
                }
            }

            // Strip % prefix from parameter tokens (e.g. "%C" -> "C", "%Mn" -> "Mn")
            expr = Regex.Replace(expr, @"%([a-zA-Z_][a-zA-Z0-9_]*)", "$1");
            return expr;
        }

        /// <summary>
        /// Pre-processes MEAN/AVG/MAX/MIN/SUM/COUNT/STDEV aggregate functions
        /// by resolving them to numeric literals before NCalc evaluates.
        /// </summary>
        private static string PreProcessAggregates(string expression, IDictionary<string, double> variables)
        {
            return AggregateRegex.Replace(expression, match =>
            {
                string funcName = match.Groups[1].Value.ToUpper();
                string argsStr = match.Groups[2].Value;

                var argNames = argsStr.Split(',').Select(a => a.Trim()).ToList();
                var values = new List<double>();

                foreach (var argName in argNames)
                {
                    if (variables.TryGetValue(argName, out double val))
                        values.Add(val);
                    else if (double.TryParse(argName,
                             System.Globalization.NumberStyles.Any,
                             System.Globalization.CultureInfo.InvariantCulture,
                             out double literal))
                        values.Add(literal);
                }

                if (!values.Any()) return "0";

                double result = funcName switch
                {
                    "MEAN" or "AVG" => values.Average(),
                    "MAX"           => values.Max(),
                    "MIN"           => values.Min(),
                    "SUM"           => values.Sum(),
                    "COUNT"         => values.Count,
                    "STDEV"         => CalculateStdDev(values),
                    _               => 0
                };

                return result.ToString(System.Globalization.CultureInfo.InvariantCulture);
            });
        }

        private static double CalculateStdDev(List<double> values)
        {
            if (values.Count <= 1) return 0;
            double mean = values.Average();
            double sumOfSquares = values.Sum(v => (v - mean) * (v - mean));
            return Math.Sqrt(sumOfSquares / values.Count);
        }

        private static double? ToDouble(object? result)
        {
            if (result is double d) return d;
            if (result is int i)    return (double)i;
            if (result is decimal dec) return (double)dec;
            if (result is float f)  return (double)f;
            if (result is long l)   return (double)l;
            try { return Convert.ToDouble(result); }
            catch { return null; }
        }
    }
}
