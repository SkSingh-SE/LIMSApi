namespace LIMSApi.Services.Interface
{
    public interface IFinancialYearService
    {
        /// <summary>
        /// Get current financial year string (e.g., "2025-2026").
        /// Queries FinancialYears where IsCurrent = true, falls back to date-based computation.
        /// </summary>
        Task<string> GetCurrentFinancialYearAsync();

        /// <summary>
        /// Get financial year string for a specific date.
        /// Indian FY: April start. March 2026 → "2025-2026", April 2026 → "2026-2027".
        /// </summary>
        string GetFinancialYearForDate(DateTime date);

        /// <summary>
        /// Set current financial year with audit trail.
        /// Unsets old IsCurrent, sets new, logs change to FinancialYearChangeLog.
        /// </summary>
        Task SetCurrentFinancialYearAsync(long financialYearId, long changedById, string? reason = null);
    }
}
