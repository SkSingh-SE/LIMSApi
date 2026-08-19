using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Models;
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
            var subgroupTests = await _context.Set<LaboratoryTestSubGroupSpecification>()
                .Where(s => s.SpecificationGradeID == specificationGradeId && s.SubGroup != null && s.SubGroup.LaboratoryTest != null && s.SubGroup.LaboratoryTest.IsActive)
                .Select(s => new SuggestedTestDto
                {
                    LaboratoryTestID = s.SubGroup!.LaboratoryTestID,
                    LaboratoryTestName = s.SubGroup!.LaboratoryTest!.Name,
                    SubGroup = s.SubGroup!.Name,
                    Source = "Specification",
                    IsPerBatch = false,
                    TestMethodStandardID = null,
                    TestMethodStandardName = null
                })
                .ToListAsync();

            var analysisTypeTests = await _context.Set<LaboratoryTestAnalysisTypeSpecification>()
                .Where(s => s.SpecificationGradeID == specificationGradeId && s.AnalysisType != null && s.AnalysisType.SubGroup != null && s.AnalysisType.SubGroup.LaboratoryTest != null && s.AnalysisType.SubGroup.LaboratoryTest.IsActive)
                .Select(s => new SuggestedTestDto
                {
                    LaboratoryTestID = s.AnalysisType!.SubGroup!.LaboratoryTestID,
                    LaboratoryTestName = s.AnalysisType!.SubGroup!.LaboratoryTest!.Name,
                    SubGroup = s.AnalysisType!.Name,
                    Source = "Specification",
                    IsPerBatch = false,
                    TestMethodStandardID = null,
                    TestMethodStandardName = null
                })
                .ToListAsync();

            return subgroupTests.Concat(analysisTypeTests)
                .GroupBy(t => t.LaboratoryTestID)
                .Select(g => g.First())
                .ToList();
        }

        public async Task<List<SuggestedTestDto>> GetSuggestedTestsByProductSpec(long productSpecificationId)
        {
            // 1. Direct Product Master mappings in LaboratoryTestSubGroupSpecification
            var directSubgroupTests = await _context.Set<LaboratoryTestSubGroupSpecification>()
                .Where(s => s.ProductMasterID == productSpecificationId && s.SubGroup != null && s.SubGroup.LaboratoryTest != null && s.SubGroup.LaboratoryTest.IsActive)
                .Select(s => new SuggestedTestDto
                {
                    LaboratoryTestID = s.SubGroup!.LaboratoryTestID,
                    LaboratoryTestName = s.SubGroup!.LaboratoryTest!.Name,
                    SubGroup = s.SubGroup!.Name,
                    Source = "ProductMasterMapping",
                    IsPerBatch = false,
                    TestMethodStandardID = null,
                    TestMethodStandardName = null
                })
                .ToListAsync();

            // 2. Direct Product Master mappings in LaboratoryTestAnalysisTypeSpecification
            var directAnalysisTests = await _context.Set<LaboratoryTestAnalysisTypeSpecification>()
                .Where(s => s.ProductMasterID == productSpecificationId && s.AnalysisType != null && s.AnalysisType.SubGroup != null && s.AnalysisType.SubGroup.LaboratoryTest != null && s.AnalysisType.SubGroup.LaboratoryTest.IsActive)
                .Select(s => new SuggestedTestDto
                {
                    LaboratoryTestID = s.AnalysisType!.SubGroup!.LaboratoryTestID,
                    LaboratoryTestName = s.AnalysisType!.SubGroup!.LaboratoryTest!.Name,
                    SubGroup = s.AnalysisType!.Name,
                    Source = "ProductMasterMapping",
                    IsPerBatch = false,
                    TestMethodStandardID = null,
                    TestMethodStandardName = null
                })
                .ToListAsync();

            // 3. Traversal via Product Master Active Version Grades
            var gradeIds = await _context.Set<ProductMasterVersionGrade>()
                .Where(g => (g.ProductMasterVersionID == productSpecificationId || g.ID == productSpecificationId || g.ProductMasterVersion.ProductMasterID == productSpecificationId) && g.IsActive)
                .Select(g => g.SpecificationGradeID)
                .Distinct()
                .ToListAsync();

            var allTests = new List<SuggestedTestDto>();
            allTests.AddRange(directSubgroupTests);
            allTests.AddRange(directAnalysisTests);

            foreach (var gradeId in gradeIds)
            {
                var gradeTests = await GetSuggestedTestsBySpecification(gradeId);
                allTests.AddRange(gradeTests);
            }

            return allTests
                .GroupBy(t => t.LaboratoryTestID)
                .Select(g => {
                    var item = g.First();
                    item.Source = "ProductMaster";
                    return item;
                })
                .ToList();
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

            var merged = productTests.Concat(specTests)
                .GroupBy(t => t.LaboratoryTestID)
                .Select(g => g.First())
                .ToList();

            return new TestAutoSuggestResult
            {
                SuggestedTests = merged,
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
                            SubGroup = stat.LaboratoryTest.SubGroups.Select(x => x.Name).FirstOrDefault() ?? "",
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
                            SubGroup = stat.LaboratoryTest.SubGroups.Select(x => x.Name).FirstOrDefault() ?? "",
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
                            SubGroup = stat.LaboratoryTest.SubGroups.Select(x => x.Name).FirstOrDefault() ?? "",
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
