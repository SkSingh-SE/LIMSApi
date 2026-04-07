using LIMSApi.Data;
using LIMSApi.Models;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Jobs
{
    public class FinancialYearRolloverJob
    {
        private readonly LIMSContext _db;
        private readonly ILogger<FinancialYearRolloverJob> _logger;

        public FinancialYearRolloverJob(LIMSContext db, ILogger<FinancialYearRolloverJob> logger)
        {
            _db = db;
            _logger = logger;
        }

        /// <summary>
        /// Runs on April 1st. Creates new FY if it doesn't exist, sets it as default.
        /// </summary>
        public async Task Execute()
        {
            var now = DateTime.UtcNow;
            var newStartYear = now.Month >= 4 ? now.Year : now.Year - 1;
            var fyYear = $"{newStartYear}-{newStartYear + 1}";

            // Check if this FY already exists
            var existing = await _db.FinancialYears.FirstOrDefaultAsync(f => f.Year == fyYear);
            if (existing != null)
            {
                _logger.LogInformation("Financial Year {FY} already exists. Skipping creation.", fyYear);

                // Ensure it's the current one
                if (!existing.IsCurrent)
                {
                    var allCurrent = await _db.FinancialYears.Where(f => f.IsCurrent).ToListAsync();
                    var oldYear = allCurrent.FirstOrDefault()?.Year ?? "(none)";
                    foreach (var f in allCurrent) f.IsCurrent = false;
                    existing.IsCurrent = true;

                    _db.FinancialYearChangeLogs.Add(new FinancialYearChangeLog
                    {
                        FinancialYearId = existing.Id,
                        OldYear = oldYear,
                        NewYear = fyYear,
                        ChangedById = 1, // System
                        ChangedOn = DateTime.UtcNow,
                        Reason = "Auto-rollover: set as current"
                    });
                    await _db.SaveChangesAsync();
                    _logger.LogInformation("Financial Year {FY} set as current via auto-rollover.", fyYear);
                }
                return;
            }

            // Get organization ID from first org
            var orgId = await _db.Organizations.Select(o => o.Id).FirstOrDefaultAsync();

            // Unset all current flags
            var currentFYs = await _db.FinancialYears.Where(f => f.IsCurrent).ToListAsync();
            var previousYear = currentFYs.FirstOrDefault()?.Year ?? "(none)";
            foreach (var f in currentFYs) f.IsCurrent = false;

            // Create new FY
            var newFY = new FinancialYear
            {
                Year = fyYear,
                StartDate = new DateTime(newStartYear, 4, 1),
                EndDate = new DateTime(newStartYear + 1, 3, 31),
                IsCurrent = true,
                OrganizationId = orgId
            };
            _db.FinancialYears.Add(newFY);
            await _db.SaveChangesAsync();

            // Audit log
            _db.FinancialYearChangeLogs.Add(new FinancialYearChangeLog
            {
                FinancialYearId = newFY.Id,
                OldYear = previousYear,
                NewYear = fyYear,
                ChangedById = 1, // System
                ChangedOn = DateTime.UtcNow,
                Reason = "Auto-created by FinancialYearRolloverJob"
            });
            await _db.SaveChangesAsync();

            _logger.LogInformation("Financial Year {FY} created and set as current.", fyYear);
        }
    }
}
