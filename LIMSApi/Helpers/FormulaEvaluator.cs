using NCalc;
using System.Text.RegularExpressions;

namespace LIMSApi.Helpers
{
    public class FormulaEvaluator
    {
        // Regex to match aggregate functions: MEAN(...), MAX(...), MIN(...), STDEV(...), SUM(...), COUNT(...)
        private static readonly Regex AggregateRegex = new Regex(
            @"(MEAN|MAX|MIN|STDEV|SUM|COUNT)\(([^)]+)\)",
            RegexOptions.IgnoreCase | RegexOptions.Compiled
        );

        public double? Evaluate(string expression, IDictionary<string, double> variables)
        {
            if (string.IsNullOrWhiteSpace(expression))
                return null;

            try
            {
                // Pre-process aggregate functions before NCalc evaluation
                string processed = PreProcessAggregates(expression, variables);

                var exp = new Expression(processed);

                // Assign variables
                foreach (var kv in variables)
                    exp.Parameters[kv.Key] = kv.Value;

                var result = exp.Evaluate();

                if (result is double d) return d;
                if (result is int i) return i;

                return Convert.ToDouble(result);
            }
            catch (Exception)
            {
                return null; // return null on failure to prevent crash
            }
        }

        /// <summary>
        /// Pre-processes aggregate functions (MEAN, MAX, MIN, STDEV, SUM, COUNT)
        /// by resolving them to numeric values before NCalc evaluation.
        /// Example: "MEAN(P1,P2,P3)" with P1=10, P2=20, P3=30 => "20"
        /// </summary>
        private string PreProcessAggregates(string expression, IDictionary<string, double> variables)
        {
            return AggregateRegex.Replace(expression, match =>
            {
                string funcName = match.Groups[1].Value.ToUpper();
                string argsStr = match.Groups[2].Value;

                // Parse argument list (param names separated by commas)
                var argNames = argsStr.Split(',')
                    .Select(a => a.Trim())
                    .ToList();

                // Resolve values from variables dictionary
                var values = new List<double>();
                foreach (var argName in argNames)
                {
                    if (variables.TryGetValue(argName, out double val))
                    {
                        values.Add(val);
                    }
                    else if (double.TryParse(argName, out double literal))
                    {
                        values.Add(literal);
                    }
                    // Skip unresolved variables — aggregate will use available values
                }

                if (!values.Any())
                    return "0"; // No values available — return 0

                double result = funcName switch
                {
                    "MEAN" => values.Average(),
                    "MAX" => values.Max(),
                    "MIN" => values.Min(),
                    "SUM" => values.Sum(),
                    "COUNT" => values.Count,
                    "STDEV" => CalculateStdDev(values),
                    _ => 0
                };

                return result.ToString(System.Globalization.CultureInfo.InvariantCulture);
            });
        }

        /// <summary>
        /// Calculates population standard deviation for a list of values.
        /// </summary>
        private static double CalculateStdDev(List<double> values)
        {
            if (values.Count <= 1) return 0;

            double mean = values.Average();
            double sumOfSquares = values.Sum(v => (v - mean) * (v - mean));
            return Math.Sqrt(sumOfSquares / values.Count);
        }

        /// <summary>
        /// Determines Pass/Fail/Marginal status based on spec limits.
        /// Marginal = value is within 5% of the boundary.
        /// </summary>
        public string DetermineResultStatus(decimal? value, decimal? specMin, decimal? specMax)
        {
            if (!value.HasValue)
                return null;

            bool hasMin = specMin.HasValue;
            bool hasMax = specMax.HasValue;

            if (!hasMin && !hasMax)
                return null; // No spec limits defined

            bool withinMin = !hasMin || value >= specMin;
            bool withinMax = !hasMax || value <= specMax;

            if (!withinMin || !withinMax)
                return "Fail";

            // Check for marginal (within 5% of boundary)
            if (hasMin && hasMax)
            {
                decimal range = specMax.Value - specMin.Value;
                if (range > 0)
                {
                    decimal marginThreshold = range * 0.05m;
                    bool nearMin = (value.Value - specMin.Value) <= marginThreshold;
                    bool nearMax = (specMax.Value - value.Value) <= marginThreshold;

                    if (nearMin || nearMax)
                        return "Marginal";
                }
            }

            return "Pass";
        }
    }
}
