using LIMSApi.Data;
using LIMSApi.Dtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace LIMSApi.ServiceWORepo
{
    public class TestAutoSuggestService : ITestAutoSuggestService
    {
        private readonly LIMSContext _context;
        private readonly ILogger<TestAutoSuggestService> _logger;
        private readonly IMemoryCache _cache;
        private static readonly TimeSpan SmartSuggestCacheDuration = TimeSpan.FromHours(1);
        private const string SmartSuggestCachePrefix = "SmartSuggest_";

        public TestAutoSuggestService(LIMSContext context, ILogger<TestAutoSuggestService> logger, IMemoryCache cache)
        {
            _context = context;
            _logger = logger;
            _cache = cache;
        }

        public async Task<List<SuggestedTestDto>> GetSuggestedTestsBySpecification(long specificationGradeId)
        {
            // ProductSpecification maps Grade → LaboratoryTest (no join table needed)
            var tests = await _context.ProductSpecifications
                .Where(ps => ps.GradeID == specificationGradeId && ps.IsActive)
                .Join(_context.LaboratoryTests,
                    ps => ps.LaboratoryTestID,
                    lt => lt.ID,
                    (ps, lt) => new SuggestedTestDto
                    {
                        LaboratoryTestID = lt.ID,
                        LaboratoryTestName = lt.Name,
                        SubGroup = lt.SubGroup,
                        Source = "Specification",
                        IsPerBatch = false,
                        TestMethodStandardID = null,
                        TestMethodStandardName = null
                    })
                .ToListAsync();

            return tests
                .GroupBy(t => t.LaboratoryTestID)
                .Select(g => g.First())
                .ToList();
        }

        public async Task<List<SuggestedTestDto>> GetSuggestedTestsByProductSpec(long productSpecificationId)
        {
            var tests = await _context.ProductTestGroups
                .Where(ptg => ptg.ProductSpecificationID == productSpecificationId && ptg.IsActive)
                .Include(ptg => ptg.LaboratoryTest)
                .Include(ptg => ptg.TestMethodSpecification)
                .Select(ptg => new SuggestedTestDto
                {
                    LaboratoryTestID = ptg.LaboratoryTestID,
                    LaboratoryTestName = ptg.LaboratoryTest != null ? ptg.LaboratoryTest.Name : string.Empty,
                    SubGroup = ptg.LaboratoryTest != null ? ptg.LaboratoryTest.SubGroup : string.Empty,
                    Source = "ProductTestGroup",
                    IsPerBatch = ptg.IsPerBatch,
                    TestMethodStandardID = ptg.TestMethodStandardID,
                    TestMethodStandardName = ptg.TestMethodSpecification != null ? ptg.TestMethodSpecification.Name : null
                })
                .ToListAsync();

            return tests;
        }

        public async Task<TestAutoSuggestResult> GetUnifiedSuggestions(long? specificationGradeId, long? productSpecificationId)
        {
            var specTests = new List<SuggestedTestDto>();
            var productTests = new List<SuggestedTestDto>();

            if (specificationGradeId.HasValue && specificationGradeId.Value > 0)
            {
                specTests = await GetSuggestedTestsBySpecification(specificationGradeId.Value);
            }

            if (productSpecificationId.HasValue && productSpecificationId.Value > 0)
            {
                productTests = await GetSuggestedTestsByProductSpec(productSpecificationId.Value);
            }

            // Merge: prefer ProductTestGroup entries when the same LaboratoryTestID appears in both sources
            // because ProductTestGroup carries TestMethodStandard info
            var productTestIds = new HashSet<long>(productTests.Select(t => t.LaboratoryTestID));

            var mergedTests = new List<SuggestedTestDto>(productTests);

            foreach (var specTest in specTests)
            {
                if (!productTestIds.Contains(specTest.LaboratoryTestID))
                {
                    mergedTests.Add(specTest);
                }
            }

            return new TestAutoSuggestResult
            {
                SuggestedTests = mergedTests.OrderBy(t => t.SubGroup).ThenBy(t => t.LaboratoryTestName).ToList(),
                SpecificationTestCount = specTests.Count,
                ProductTestGroupCount = productTests.Count
            };
        }

        public async Task<SmartSuggestResult> GetSmartSuggestions(SmartSuggestRequest request)
        {
            var cacheKey = $"{SmartSuggestCachePrefix}{request.SpecificationGradeId}_{request.MetalClassificationId}_{request.ProductConditionId}_{request.CustomerId}";

            if (_cache.TryGetValue(cacheKey, out SmartSuggestResult? cached) && cached != null)
            {
                return cached;
            }

            var merged = new Dictionary<long, SuggestedTestDto>();
            int specMatchCount = 0;
            int customerHistoryCount = 0;
            int globalFrequencyCount = 0;
            int trendingCount = 0;

            // A. Spec Lines (30 points)
            if (request.SpecificationGradeId.HasValue && request.SpecificationGradeId.Value > 0)
            {
                var specTests = await GetSuggestedTestsBySpecification(request.SpecificationGradeId.Value);
                specMatchCount = specTests.Count;

                foreach (var test in specTests)
                {
                    if (!merged.ContainsKey(test.LaboratoryTestID))
                    {
                        merged[test.LaboratoryTestID] = new SuggestedTestDto
                        {
                            LaboratoryTestID = test.LaboratoryTestID,
                            LaboratoryTestName = test.LaboratoryTestName,
                            SubGroup = test.SubGroup,
                            Source = test.Source,
                            IsPerBatch = test.IsPerBatch,
                            TestMethodStandardID = test.TestMethodStandardID,
                            TestMethodStandardName = test.TestMethodStandardName,
                            Score = 0,
                            Tags = new List<string>()
                        };
                    }
                    merged[test.LaboratoryTestID].Score += 30;
                    if (!merged[test.LaboratoryTestID].Tags.Contains("Spec Required"))
                        merged[test.LaboratoryTestID].Tags.Add("Spec Required");
                }
            }

            // B. Lab Scope (15 points)
            if (request.MetalClassificationId.HasValue && request.ProductConditionId.HasValue)
            {
                var labScopeStats = await _context.TestUsageStats
                    .Where(t => t.MetalClassificationID == request.MetalClassificationId.Value
                        && t.ProductConditionID == request.ProductConditionId.Value
                        && t.CustomerID == null
                        && t.UsageCount > 0)
                    .Include(t => t.LaboratoryTest)
                    .ToListAsync();

                foreach (var stat in labScopeStats)
                {
                    if (stat.LaboratoryTest == null) continue;

                    if (!merged.ContainsKey(stat.LaboratoryTestID))
                    {
                        merged[stat.LaboratoryTestID] = new SuggestedTestDto
                        {
                            LaboratoryTestID = stat.LaboratoryTestID,
                            LaboratoryTestName = stat.LaboratoryTest.Name,
                            SubGroup = stat.LaboratoryTest.SubGroup,
                            Source = "LabScope",
                            Score = 0,
                            Tags = new List<string>()
                        };
                    }
                    merged[stat.LaboratoryTestID].Score += 15;
                    if (!merged[stat.LaboratoryTestID].Tags.Contains("Lab Scope"))
                        merged[stat.LaboratoryTestID].Tags.Add("Lab Scope");
                }
            }

            // C. Customer History (25 points)
            if (request.CustomerId.HasValue && request.MetalClassificationId.HasValue)
            {
                var customerStats = await _context.TestUsageStats
                    .Where(t => t.CustomerID == request.CustomerId.Value
                        && t.MetalClassificationID == request.MetalClassificationId.Value)
                    .Include(t => t.LaboratoryTest)
                    .ToListAsync();

                customerHistoryCount = customerStats.Count;

                foreach (var stat in customerStats)
                {
                    if (stat.LaboratoryTest == null) continue;

                    if (!merged.ContainsKey(stat.LaboratoryTestID))
                    {
                        merged[stat.LaboratoryTestID] = new SuggestedTestDto
                        {
                            LaboratoryTestID = stat.LaboratoryTestID,
                            LaboratoryTestName = stat.LaboratoryTest.Name,
                            SubGroup = stat.LaboratoryTest.SubGroup,
                            Source = "CustomerHistory",
                            Score = 0,
                            Tags = new List<string>()
                        };
                    }
                    merged[stat.LaboratoryTestID].Score += 25;
                    if (!merged[stat.LaboratoryTestID].Tags.Contains("Customer Favorite"))
                        merged[stat.LaboratoryTestID].Tags.Add("Customer Favorite");
                }
            }

            // D. Global Frequency (20 points) — top 20 by usage
            if (request.MetalClassificationId.HasValue)
            {
                var globalStats = await _context.TestUsageStats
                    .Where(t => t.CustomerID == null
                        && t.MetalClassificationID == request.MetalClassificationId.Value)
                    .OrderByDescending(t => t.UsageCount)
                    .Take(20)
                    .Include(t => t.LaboratoryTest)
                    .ToListAsync();

                globalFrequencyCount = globalStats.Count;

                foreach (var stat in globalStats)
                {
                    if (stat.LaboratoryTest == null) continue;

                    if (!merged.ContainsKey(stat.LaboratoryTestID))
                    {
                        merged[stat.LaboratoryTestID] = new SuggestedTestDto
                        {
                            LaboratoryTestID = stat.LaboratoryTestID,
                            LaboratoryTestName = stat.LaboratoryTest.Name,
                            SubGroup = stat.LaboratoryTest.SubGroup,
                            Source = "GlobalFrequency",
                            Score = 0,
                            Tags = new List<string>()
                        };
                    }
                    merged[stat.LaboratoryTestID].Score += 20;
                    if (!merged[stat.LaboratoryTestID].Tags.Contains("Most Popular"))
                        merged[stat.LaboratoryTestID].Tags.Add("Most Popular");
                }
            }

            // E. Trending bonus (+10 points) — RecentUsageCount > UsageCount * 0.3
            // Check all stats that match any of the above criteria for trending
            var allTestIds = merged.Keys.ToList();
            if (allTestIds.Count > 0)
            {
                var trendingStats = await _context.TestUsageStats
                    .Where(t => allTestIds.Contains(t.LaboratoryTestID)
                        && t.UsageCount > 0
                        && t.RecentUsageCount > t.UsageCount * 0.3)
                    .Select(t => t.LaboratoryTestID)
                    .Distinct()
                    .ToListAsync();

                trendingCount = trendingStats.Count;

                foreach (var testId in trendingStats)
                {
                    if (merged.ContainsKey(testId))
                    {
                        merged[testId].Score += 10;
                        if (!merged[testId].Tags.Contains("Trending"))
                            merged[testId].Tags.Add("Trending");
                    }
                }
            }

            var result = new SmartSuggestResult
            {
                SuggestedTests = merged.Values
                    .OrderByDescending(t => t.Score)
                    .ThenBy(t => t.LaboratoryTestName)
                    .ToList(),
                SpecMatchCount = specMatchCount,
                CustomerHistoryCount = customerHistoryCount,
                GlobalFrequencyCount = globalFrequencyCount,
                TrendingCount = trendingCount
            };

            _cache.Set(cacheKey, result, SmartSuggestCacheDuration);

            return result;
        }
    }
}
