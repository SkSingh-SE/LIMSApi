using NCalc;

namespace LIMSApi.Helpers
{
    public class FormulaEvaluator
    {
        public double? Evaluate(string expression, IDictionary<string, double> variables)
        {
            if (string.IsNullOrWhiteSpace(expression))
                return null;

            try
            {
                var exp = new Expression(expression);

                // Assign variables
                foreach (var kv in variables)
                    exp.Parameters[kv.Key] = kv.Value;

                var result = exp.Evaluate();

                if (result is double d) return d;
                if (result is int i) return i;

                return Convert.ToDouble(result);
            }
            catch (Exception ex)
            {
                return null; // return null on failure → prevents crash
            }
        }
    }
}
