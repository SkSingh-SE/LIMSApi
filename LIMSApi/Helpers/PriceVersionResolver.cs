using LIMSApi.Data;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Helpers
{
    /// <summary>
    /// Canonical resolver for date-effective price versions.
    ///
    /// Rule: pick the version with max(EffectiveFrom) where EffectiveFrom ≤ referenceDate.
    /// Fallback: if referenceDate precedes all versions, return the earliest version.
    ///
    /// Used by Cutting, Machining, and InvoiceCase pricing subsystems.
    /// Never use currentFY for price resolution — always pass the sample inward date.
    /// </summary>
    public static class PriceVersionResolver
    {
        /// <summary>
        /// Resolves the correct version from an in-memory collection.
        /// Use this when the versions are already loaded (e.g. from a master's Versions nav property).
        /// </summary>
        public static T? Resolve<T>(IEnumerable<T> versions, Func<T, DateTime> effectiveFromSelector, DateTime referenceDate)
            where T : class
        {
            var active = versions.ToList();
            if (active.Count == 0) return null;

            var match = active
                .Where(v => effectiveFromSelector(v).Date <= referenceDate.Date)
                .OrderByDescending(v => effectiveFromSelector(v))
                .FirstOrDefault();

            // Fallback: referenceDate precedes all versions → use the earliest
            return match ?? active.OrderBy(v => effectiveFromSelector(v)).First();
        }

        /// <summary>
        /// Resolves the inward FY id from the sample's FinancialYearId.
        /// Falls back to the current default FY if the sample has no FY stamped.
        /// </summary>
        public static async Task<long?> ResolveInwardFyId(LIMSContext db, long? sampleFinancialYearId)
        {
            if (sampleFinancialYearId.HasValue)
                return sampleFinancialYearId;

            return await db.FinancialYears
                .Where(f => f.IsCurrent)
                .Select(f => (long?)f.Id)
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Resolves the inward date to use for price lookups:
        /// returns the sample's CollectionTime, or DateTime.UtcNow as fallback.
        /// </summary>
        public static async Task<DateTime> ResolveInwardDate(LIMSContext db, long sampleInwardId)
        {
            var date = await db.SampleInwards
                .Where(s => s.ID == sampleInwardId)
                .Select(s => (DateTime?)s.CollectionTime)
                .FirstOrDefaultAsync();

            return date ?? DateTime.UtcNow;
        }
    }
}
