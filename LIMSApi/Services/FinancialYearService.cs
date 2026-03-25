using LIMSApi.Data;
using LIMSApi.Models;
using LIMSApi.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Services
{
    public class FinancialYearService : IFinancialYearService
    {
        private readonly LIMSContext _db;
        private readonly ILogger<FinancialYearService> _logger;

        public FinancialYearService(LIMSContext db, ILogger<FinancialYearService> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<string> GetCurrentFinancialYearAsync()
        {
            var fy = await _db.FinancialYears.FirstOrDefaultAsync(f => f.IsCurrent);
            if (fy != null)
                return fy.Year;

            // Fallback: compute from current date
            var computed = GetFinancialYearForDate(DateTime.UtcNow);
            _logger.LogWarning("No FinancialYear with IsCurrent=true found. Using computed FY: {FY}", computed);
            return computed;
        }

        public string GetFinancialYearForDate(DateTime date)
        {
            // Indian FY: April to March
            var year = date.Month >= 4 ? date.Year : date.Year - 1;
            return $"{year}-{year + 1}";
        }

        public async Task SetCurrentFinancialYearAsync(long financialYearId, long changedById, string? reason = null)
        {
            var newFy = await _db.FinancialYears.FirstOrDefaultAsync(f => f.Id == financialYearId)
                ?? throw new Exception($"FinancialYear {financialYearId} not found");

            // Find current FY to log the change
            var oldFy = await _db.FinancialYears.FirstOrDefaultAsync(f => f.IsCurrent);
            var oldYear = oldFy?.Year ?? "(none)";

            // Unset all IsCurrent flags
            var allCurrent = await _db.FinancialYears.Where(f => f.IsCurrent).ToListAsync();
            foreach (var fy in allCurrent)
                fy.IsCurrent = false;

            // Set new
            newFy.IsCurrent = true;

            // Log the change
            _db.FinancialYearChangeLogs.Add(new FinancialYearChangeLog
            {
                FinancialYearId = financialYearId,
                OldYear = oldYear,
                NewYear = newFy.Year,
                ChangedById = changedById,
                ChangedOn = DateTime.UtcNow,
                Reason = reason
            });

            await _db.SaveChangesAsync();

            _logger.LogInformation("Financial year changed from {OldFY} to {NewFY} by user {UserId}",
                oldYear, newFy.Year, changedById);
        }
    }
}
