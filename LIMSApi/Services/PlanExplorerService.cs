using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Services.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LIMSApi.Services
{
    public class PlanExplorerService : IPlanExplorerService
    {
        private readonly LIMSContext _context;
        private readonly ILogger<PlanExplorerService> _logger;

        public PlanExplorerService(LIMSContext context, ILogger<PlanExplorerService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ProductMasterExplorerDto?> GetProductMasterExplorerAsync(long productMasterId)
        {
            var pm = await _context.ProductMasters
                .AsNoTracking()
                .Include(p => p.ProductSizeMaster)
                .Include(p => p.MetalClassifications)
                    .ThenInclude(m => m.MetalClassification)
                .Include(p => p.Versions)
                    .ThenInclude(v => v.Grades)
                        .ThenInclude(g => g.SpecificationGrade)
                .FirstOrDefaultAsync(p => p.ID == productMasterId);

            if (pm == null) return null;

            var primaryMetal = pm.MetalClassifications.FirstOrDefault()?.MetalClassification;
            var activeVersion = pm.Versions.FirstOrDefault(v => v.IsActiveVersion) ?? pm.Versions.FirstOrDefault();
            var gradeList = activeVersion?.Grades.Where(g => g.IsActive && g.SpecificationGrade != null).ToList() ?? new List<ProductMasterVersionGrade>();

            var configuredGrades = new List<ConfiguredGradeDto>();

            foreach (var g in gradeList)
            {
                if (g.SpecificationGrade == null) continue;

                var specHeader = await _context.SpecificationHeaders
                    .AsNoTracking()
                    .FirstOrDefaultAsync(h => h.ID == g.SpecificationGrade.SpecificationHeaderID);

                var gradeDto = await BuildGradeDtoAsync(
                    g.SpecificationGradeID,
                    g.SpecificationGrade.Grade,
                    specHeader?.DisplayTitle ?? specHeader?.AliasName ?? "Standard Spec"
                );
                configuredGrades.Add(gradeDto);
            }

            return new ProductMasterExplorerDto
            {
                ProductMasterID = pm.ID,
                ProductName = pm.ProductName,
                DisplayTitle = pm.DisplayTitle,
                MetalClassificationID = primaryMetal?.ID,
                MetalClassificationName = primaryMetal?.Name ?? "",
                ProductSizeMasterID = pm.ProductSizeMasterID,
                ProductSizeDisplayName = pm.ProductSizeMaster?.DisplayName ?? "",
                Grades = configuredGrades
            };
        }

        public async Task<MetalExplorerDto?> GetMetalClassificationExplorerAsync(long metalClassificationId)
        {
            var metal = await _context.MetalClassificationMasters
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == metalClassificationId);

            if (metal == null) return null;

            var grades = await _context.SpecificationGrades
                .AsNoTracking()
                .Where(g => g.MetalClassificationID == metalClassificationId)
                .Take(20)
                .ToListAsync();

            var configuredGrades = new List<ConfiguredGradeDto>();

            foreach (var g in grades)
            {
                var specHeader = await _context.SpecificationHeaders
                    .AsNoTracking()
                    .FirstOrDefaultAsync(h => h.ID == g.SpecificationHeaderID);

                var gradeDto = await BuildGradeDtoAsync(
                    g.ID,
                    g.Grade,
                    specHeader?.DisplayTitle ?? specHeader?.AliasName ?? "Standard Spec"
                );
                gradeDto.MetalClassificationName = metal.Name;
                configuredGrades.Add(gradeDto);
            }

            return new MetalExplorerDto
            {
                MetalClassificationID = metal.ID,
                MetalClassificationName = metal.Name,
                Grades = configuredGrades
            };
        }

        public async Task<LabTestExplorerDto?> GetLabTestExplorerAsync(long labTestId)
        {
            var test = await _context.LaboratoryTests
                .AsNoTracking()
                .Include(t => t.SubGroups)
                .FirstOrDefaultAsync(t => t.ID == labTestId);

            if (test == null) return null;

            var testMethodSpecs = await _context.TestMethodSpecifications
                .AsNoTracking()
                .Where(s => !s.IsDisabled)
                .Select(s => new ConfiguredTestDto
                {
                    LaboratoryTestID = test.ID,
                    LaboratoryTestName = test.Name,
                    TestType = "General",
                    SubGroup = test.SubGroups.FirstOrDefault() != null ? test.SubGroups.FirstOrDefault()!.Name : "",
                    TestMethodStandardID = s.ID,
                    TestMethodStandardName = s.DisplayTitle ?? s.Name,
                    Quantity = 1
                })
                .Take(10)
                .ToListAsync();

            return new LabTestExplorerDto
            {
                LaboratoryTestID = test.ID,
                LaboratoryTestName = test.Name,
                Category = "General",
                Standards = testMethodSpecs
            };
        }

        private async Task<ConfiguredGradeDto> BuildGradeDtoAsync(long gradeId, string gradeName, string specName)
        {
            var specGrade = await _context.SpecificationGrades
                .AsNoTracking()
                .FirstOrDefaultAsync(g => g.ID == gradeId);

            long headerId = specGrade?.SpecificationHeaderID ?? 0;

            // 1. Fetch SubGroup Specification Mappings with accurate Department IsChemical check
            var subgroupTests = await _context.Set<LaboratoryTestSubGroupSpecification>()
                .AsNoTracking()
                .Include(s => s.SubGroup)
                    .ThenInclude(sg => sg!.LaboratoryTest)
                        .ThenInclude(lt => lt!.LabDepartment)
                .Where(s => (s.SpecificationGradeID == gradeId || (headerId > 0 && s.SpecificationHeaderID == headerId))
                            && s.SubGroup != null && s.SubGroup.LaboratoryTest != null && s.SubGroup.LaboratoryTest.IsActive)
                .Select(s => new ConfiguredTestDto
                {
                    LaboratoryTestID = s.SubGroup!.LaboratoryTestID,
                    LaboratoryTestName = s.SubGroup.LaboratoryTest!.Name,
                    TestType = (s.SubGroup.LaboratoryTest.LabDepartment != null && s.SubGroup.LaboratoryTest.LabDepartment.IsChemical) ||
                               s.SubGroup.LaboratoryTest.Name.Contains("Spectro") || s.SubGroup.LaboratoryTest.Name.Contains("Chemical") ||
                               s.SubGroup.Name.Contains("Spectro") || s.SubGroup.Name.Contains("Chemical")
                               ? "Chemical" : "General",
                    SubGroup = s.SubGroup.Name,
                    TestMethodStandardID = null,
                    TestMethodStandardName = "",
                    Quantity = 1
                })
                .ToListAsync();

            // 2. Fetch Analysis Type Specification Mappings (Chemical / Technique-based Tests)
            var analysisTypeTests = await _context.Set<LaboratoryTestAnalysisTypeSpecification>()
                .AsNoTracking()
                .Include(s => s.AnalysisType)
                    .ThenInclude(at => at!.SubGroup)
                        .ThenInclude(sg => sg!.LaboratoryTest)
                            .ThenInclude(lt => lt!.LabDepartment)
                .Where(s => (s.SpecificationGradeID == gradeId || (headerId > 0 && s.SpecificationHeaderID == headerId))
                            && s.AnalysisType != null && s.AnalysisType.SubGroup != null && s.AnalysisType.SubGroup.LaboratoryTest != null && s.AnalysisType.SubGroup.LaboratoryTest.IsActive)
                .Select(s => new ConfiguredTestDto
                {
                    LaboratoryTestID = s.AnalysisType!.SubGroup!.LaboratoryTestID,
                    LaboratoryTestName = s.AnalysisType.SubGroup.LaboratoryTest!.Name,
                    TestType = "Chemical",
                    SubGroup = s.AnalysisType.Name,
                    TestMethodStandardID = null,
                    TestMethodStandardName = "",
                    Quantity = 1
                })
                .ToListAsync();

            // 3. Fetch Specification Line Laboratory Tests
            var specLineTests = await _context.SpecificationLines
                .AsNoTracking()
                .Include(l => l.LaboratoryTest)
                    .ThenInclude(lt => lt!.LabDepartment)
                .Where(l => l.SpecificationGradeID == gradeId && l.LaboratoryTestID != null && l.LaboratoryTest != null && l.LaboratoryTest.IsActive)
                .Select(l => new ConfiguredTestDto
                {
                    LaboratoryTestID = l.LaboratoryTestID!.Value,
                    LaboratoryTestName = l.LaboratoryTest!.Name,
                    TestType = (l.LaboratoryTest.LabDepartment != null && l.LaboratoryTest.LabDepartment.IsChemical) ||
                               (l.Type != null && l.Type.ToLower() == "chemical") ||
                               l.LaboratoryTest.Name.Contains("Spectro") || l.LaboratoryTest.Name.Contains("Chemical")
                               ? "Chemical" : "General",
                    SubGroup = l.Type ?? "General",
                    TestMethodStandardID = null,
                    TestMethodStandardName = "",
                    Quantity = 1
                })
                .ToListAsync();

            var combinedTests = subgroupTests
                .Concat(analysisTypeTests)
                .Concat(specLineTests)
                .GroupBy(t => t.LaboratoryTestID)
                .Select(g => g.First())
                .ToList();

            var chemicalLines = await _context.SpecificationLines
                .AsNoTracking()
                .Include(l => l.Parameter)
                .Include(l => l.ParameterUnit)
                .Where(l => l.SpecificationGradeID == gradeId)
                .Select(l => new ConfiguredParameterDto
                {
                    ParameterID = l.ParameterID ?? 0,
                    ParameterName = l.Parameter != null ? l.Parameter.Name : "",
                    MinValue = l.MinValue,
                    MaxValue = l.MaxValue,
                    ParameterUnitID = l.ParameterUnitID ?? (l.Parameter != null ? l.Parameter.ParameterUnitID ?? 0 : 0),
                    ParameterUnit = l.ParameterUnit != null ? l.ParameterUnit.Name : (l.Parameter != null && l.Parameter.ParameterUnit != null ? l.Parameter.ParameterUnit.Name : ""),
                    Selected = true
                })
                .ToListAsync();

            return new ConfiguredGradeDto
            {
                SpecificationGradeID = gradeId,
                GradeName = gradeName,
                SpecificationName = specName,
                ConfiguredTests = combinedTests,
                ChemicalElements = chemicalLines
            };
        }
    }
}
