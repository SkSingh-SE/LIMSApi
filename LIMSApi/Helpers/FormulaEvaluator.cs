using NCalc;
using System;
using System.Collections.Generic;
using System.Linq;
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

        // Matches {P12}, {Code}, {SOIL_LL}, {Test.Code} — stored formula token format
        private static readonly Regex CodeTokenRegex = new Regex(
            @"\{([A-Za-z0-9_.]+)\}",
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
                var namedValues = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);
                foreach (var kv in paramValues)
                {
                    namedValues[$"P{kv.Key}"] = kv.Value;
                    namedValues[kv.Key.ToString()] = kv.Value;
                }
                string ncalcExpr = ConvertToNCalcExpression(expression);
                ncalcExpr = PreProcessAggregates(ncalcExpr, namedValues);

                var exp = new Expression(ncalcExpr);
                foreach (var kv in namedValues)
                    exp.Parameters[kv.Key] = kv.Value;

                var result = exp.Evaluate();
                return ToDouble(result);
            }
            catch { return null; }
        }

        /// <summary>
        /// Evaluates formula using semantic variable codes (e.g. "{SOIL_LL} - {SOIL_PL}") or legacy "P12" tokens.
        /// </summary>
        public double? Evaluate(string expression, IDictionary<string, double> variables)
        {
            if (string.IsNullOrWhiteSpace(expression)) return null;
            try
            {
                var dict = new Dictionary<string, double>(variables, StringComparer.OrdinalIgnoreCase);
                string ncalcExpr = ConvertToNCalcExpression(expression);
                ncalcExpr = PreProcessAggregates(ncalcExpr, dict);

                var exp = new Expression(ncalcExpr);
                foreach (var kv in dict)
                    exp.Parameters[kv.Key] = kv.Value;

                var result = exp.Evaluate();
                return ToDouble(result);
            }
            catch { return null; }
        }

        // ──────────────────────────────────────────────────
        // Public: Topological Sort & Circular Dependency Check
        // ──────────────────────────────────────────────────

        /// <summary>
        /// Orders items so that dependencies are evaluated before dependent items.
        /// Throws InvalidOperationException if circular dependency is detected.
        /// </summary>
        public List<T> OrderByTopologicalSort<T>(
            IEnumerable<T> items,
            Func<T, string> getCode,
            Func<T, string?> getFormula)
        {
            var itemList = items.ToList();
            var itemMap = itemList.ToDictionary(getCode, item => item, StringComparer.OrdinalIgnoreCase);
            var dependencies = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);

            foreach (var item in itemList)
            {
                string code = getCode(item);
                string? formula = getFormula(item);
                var deps = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                if (!string.IsNullOrWhiteSpace(formula))
                {
                    var referencedTokens = ExtractTokens(formula);
                    foreach (var token in referencedTokens)
                    {
                        if (itemMap.ContainsKey(token) && !string.Equals(token, code, StringComparison.OrdinalIgnoreCase))
                        {
                            deps.Add(token);
                        }
                    }
                }
                dependencies[code] = deps;
            }

            var result = new List<T>();
            var visited = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase); // true = visiting (in stack), false = visited

            void Visit(string node)
            {
                if (visited.TryGetValue(node, out bool inStack))
                {
                    if (inStack)
                    {
                        throw new InvalidOperationException($"Circular formula dependency detected involving parameter '{node}'.");
                    }
                    return; // Already visited
                }

                visited[node] = true; // Mark currently visiting

                if (dependencies.TryGetValue(node, out var nodeDeps))
                {
                    foreach (var dep in nodeDeps)
                    {
                        Visit(dep);
                    }
                }

                visited[node] = false; // Finished visiting
                if (itemMap.TryGetValue(node, out var matchedItem))
                {
                    result.Add(matchedItem);
                }
            }

            foreach (var item in itemList)
            {
                string code = getCode(item);
                if (!visited.ContainsKey(code))
                {
                    Visit(code);
                }
            }

            return result;
        }

        // ──────────────────────────────────────────────────
        // Public: Aggregate Calculation
        // ──────────────────────────────────────────────────

        /// <summary>
        /// Evaluates configured aggregate (Average, Min, Max, Sum, Count, StDev, Last) over numeric series.
        /// </summary>
        public double? CalculateAggregate(IEnumerable<double> values, string? aggregateType)
        {
            var valList = values.ToList();
            if (!valList.Any()) return null;

            string type = (aggregateType ?? "Average").Trim().ToUpperInvariant();
            return type switch
            {
                "AVERAGE" or "AVG" or "MEAN" => valList.Average(),
                "MIN" => valList.Min(),
                "MAX" => valList.Max(),
                "SUM" => valList.Sum(),
                "COUNT" => valList.Count,
                "STDEV" => CalculateStdDev(valList),
                "LAST" => valList.LastOrDefault(),
                "FIRST" => valList.FirstOrDefault(),
                _ => valList.Average()
            };
        }

        // ──────────────────────────────────────────────────
        // Public: ValidateFormula
        // ──────────────────────────────────────────────────

        /// <summary>
        /// Validates formula expression using valid parameter IDs or codes.
        /// </summary>
        public string? ValidateFormula(string expression, IEnumerable<long> validParamIds)
        {
            if (string.IsNullOrWhiteSpace(expression))
                return "Formula expression cannot be empty.";

            var validSet = new HashSet<long>(validParamIds);
            var tokenMatches = ParamTokenRegex.Matches(expression);

            if (!tokenMatches.Any())
                return "Formula must contain at least one parameter reference (e.g. {P12}).";

            foreach (Match m in tokenMatches)
            {
                long paramId = long.Parse(m.Groups[1].Value);
                if (!validSet.Contains(paramId))
                    return $"Invalid parameter reference: P{paramId} does not exist.";
            }

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
                return null;
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

        /// <summary>
        /// Extracts all unique string tokens referenced in a formula.
        /// e.g. "{SOIL_LL} - {SOIL_PL}" → ["SOIL_LL", "SOIL_PL"]
        /// </summary>
        public IEnumerable<string> ExtractTokens(string expression)
        {
            if (string.IsNullOrWhiteSpace(expression))
                return Enumerable.Empty<string>();

            return CodeTokenRegex.Matches(expression)
                .Select(m => m.Groups[1].Value)
                .Distinct()
                .ToList();
        }

        // ──────────────────────────────────────────────────
        // Public: DetermineResultStatus
        // ──────────────────────────────────────────────────

        /// <summary>
        /// Pass / Fail / Marginal / NotApplicable — Marginal = within 5% of spec boundary.
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

        /// <summary>
        /// Evaluates a formula and returns the exact substitution string, dependency list, and evaluated result.
        /// </summary>
        public FormulaEvaluationTrace EvaluateWithTrace(string formula, IDictionary<string, double> variables, int precision = 2)
        {
            var trace = new FormulaEvaluationTrace();
            if (string.IsNullOrWhiteSpace(formula))
            {
                trace.ErrorMessage = "Formula expression is empty.";
                return trace;
            }

            try
            {
                var deps = ExtractTokens(formula).ToList();
                trace.Dependencies = deps;

                string substituted = formula;
                foreach (var token in deps)
                {
                    if (variables.TryGetValue(token, out double val))
                    {
                        substituted = Regex.Replace(substituted, @"\{" + Regex.Escape(token) + @"\}", val.ToString($"F{precision}", System.Globalization.CultureInfo.InvariantCulture));
                    }
                }

                var evalResult = Evaluate(formula, variables);
                if (evalResult.HasValue)
                {
                    trace.IsValid = true;
                    trace.Result = Math.Round(evalResult.Value, precision);
                    trace.FormattedResult = trace.Result.Value.ToString($"F{precision}", System.Globalization.CultureInfo.InvariantCulture);
                    trace.SubstitutionTrace = $"{substituted} = {trace.FormattedResult}";
                }
                else
                {
                    trace.ErrorMessage = "Evaluation resulted in null or non-numeric value.";
                    trace.SubstitutionTrace = substituted;
                }
            }
            catch (Exception ex)
            {
                trace.ErrorMessage = ex.Message;
            }
            return trace;
        }

        // ──────────────────────────────────────────────────
        // Private helpers
        // ──────────────────────────────────────────────────

        private static string ConvertToNCalcExpression(string formula)
            => CodeTokenRegex.Replace(formula, m => $"[{m.Groups[1].Value}]");

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

    public class FormulaEvaluationTrace
    {
        public bool IsValid { get; set; }
        public string? ErrorMessage { get; set; }
        public double? Result { get; set; }
        public string? FormattedResult { get; set; }
        public string SubstitutionTrace { get; set; } = string.Empty;
        public List<string> Dependencies { get; set; } = new();
    }
}
