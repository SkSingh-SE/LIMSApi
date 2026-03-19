using Hangfire;
using LIMSApi.Data;
using LIMSApi.Models;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Jobs
{
    public class TestUsageStatsJob
    {
        private readonly LIMSContext _context;
        private readonly ILogger<TestUsageStatsJob> _logger;

        public TestUsageStatsJob(LIMSContext context, ILogger<TestUsageStatsJob> logger)
        {
            _context = context;
            _logger = logger;
        }

        [AutomaticRetry(Attempts = 3)]
        public async Task Execute()
        {
            var log = new JobExecutionLog
            {
                JobName = nameof(TestUsageStatsJob),
                StartTime = DateTime.Now
            };

            try
            {
                _logger.LogInformation("TestUsageStatsJob started at {time}", DateTime.Now);

                var sixMonthsAgo = DateTime.UtcNow.AddMonths(-6);
                var computedAt = DateTime.UtcNow;

                // Query test result headers joined with sample details and inwards
                // to gather usage statistics per (LaboratoryTestID, MetalClassificationID, ProductConditionID, CustomerID)
                var customerStats = await _context.TestResultHeaders
                    .Include(tr => tr.Sample)
                        .ThenInclude(s => s!.SampleInward)
                    .Where(tr => tr.Sample != null && tr.Sample.SampleInward != null)
                    .GroupBy(tr => new
                    {
                        tr.LaboratoryTestID,
                        tr.Sample!.MetalClassificationID,
                        tr.Sample.ProductConditionID,
                        tr.Sample.SampleInward!.CustomerID
                    })
                    .Select(g => new TestUsageStats
                    {
                        LaboratoryTestID = g.Key.LaboratoryTestID,
                        MetalClassificationID = g.Key.MetalClassificationID,
                        ProductConditionID = g.Key.ProductConditionID,
                        CustomerID = g.Key.CustomerID,
                        UsageCount = g.Count(),
                        RecentUsageCount = g.Count(tr => tr.CompletedAt != null && tr.CompletedAt >= sixMonthsAgo),
                        LastUsedDate = g.Max(tr => tr.CompletedAt ?? tr.CreatedOn),
                        ComputedAt = computedAt
                    })
                    .ToListAsync();

                // Compute global stats (CustomerID = null) aggregated across all customers
                var globalStats = customerStats
                    .GroupBy(s => new
                    {
                        s.LaboratoryTestID,
                        s.MetalClassificationID,
                        s.ProductConditionID
                    })
                    .Select(g => new TestUsageStats
                    {
                        LaboratoryTestID = g.Key.LaboratoryTestID,
                        MetalClassificationID = g.Key.MetalClassificationID,
                        ProductConditionID = g.Key.ProductConditionID,
                        CustomerID = null,
                        UsageCount = g.Sum(s => s.UsageCount),
                        RecentUsageCount = g.Sum(s => s.RecentUsageCount),
                        LastUsedDate = g.Max(s => s.LastUsedDate),
                        ComputedAt = computedAt
                    })
                    .ToList();

                // Clear existing stats and insert new rows
                await _context.Database.ExecuteSqlRawAsync("DELETE FROM TestUsageStats");

                var allStats = new List<TestUsageStats>();
                allStats.AddRange(customerStats);
                allStats.AddRange(globalStats);

                await _context.TestUsageStats.AddRangeAsync(allStats);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Inserted {Count} TestUsageStats rows", allStats.Count);

                // Update LaboratoryTest table with aggregated global counts
                var labTestAggregates = globalStats
                    .GroupBy(s => s.LaboratoryTestID)
                    .Select(g => new
                    {
                        LaboratoryTestID = g.Key,
                        GlobalUsageCount = g.Sum(s => s.UsageCount),
                        RecentUsageCount = g.Sum(s => s.RecentUsageCount),
                        LastPerformedDate = g.Max(s => s.LastUsedDate)
                    })
                    .ToList();

                foreach (var agg in labTestAggregates)
                {
                    var labTest = await _context.LaboratoryTests.FindAsync(agg.LaboratoryTestID);
                    if (labTest != null)
                    {
                        labTest.GlobalUsageCount = agg.GlobalUsageCount;
                        labTest.RecentUsageCount = agg.RecentUsageCount;
                        labTest.LastPerformedDate = agg.LastPerformedDate;
                    }
                }

                await _context.SaveChangesAsync();

                log.Status = "Success";
                _logger.LogInformation(
                    "TestUsageStatsJob completed successfully. Updated {LabTestCount} laboratory tests at {time}",
                    labTestAggregates.Count, DateTime.Now);
            }
            catch (Exception ex)
            {
                log.Status = "Failed";
                log.ErrorMessage = ex.Message;
                _logger.LogError(ex, "TestUsageStatsJob failed at {time}", DateTime.Now);
            }
            finally
            {
                log.EndTime = DateTime.Now;
                _context.JobExecutionLogs.Add(log);
                await _context.SaveChangesAsync();
            }
        }
    }
}
