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
                    specHeader?.DisplayTitle ?? specHeader?.AliasName ?? "Standard Spec",
                    productMasterId
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
                .Include(t => t.LabDepartment)
                .Include(t => t.SubGroups)
                .FirstOrDefaultAsync(t => t.ID == labTestId);

            if (test == null) return null;

            var isChem = test.IsChemicalTest || (test.LabDepartment != null && test.LabDepartment.IsChemical);

            var testMethodSpecs = await _context.TestMethodSpecifications
                .AsNoTracking()
                .Where(s => !s.IsDisabled)
                .Select(s => new ConfiguredTestDto
                {
                    LaboratoryTestID = test.ID,
                    LaboratoryTestName = test.Name,
                    TestType = isChem ? "Chemical" : "General",
                    SubGroup = test.SubGroups.FirstOrDefault() != null ? test.SubGroups.FirstOrDefault()!.Name : "",
                    SourceTag = "Lab Scope",
                    SourceTags = new List<string> { "Lab Scope" },
                    TestMethodSpecificationID = s.ID,
                    TestMethodSpecificationName = s.DisplayTitle ?? s.Name,
                    Quantity = 1
                })
                .Take(10)
                .ToListAsync();

            return new LabTestExplorerDto
            {
                LaboratoryTestID = test.ID,
                LaboratoryTestName = test.Name,
                Category = isChem ? "Chemical" : "General",
                TestMethodSpecifications = testMethodSpecs
            };
        }

        public async Task<List<ConfiguredTestDto>> GetUniversalTestSearchAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query) || query.Trim().Length < 2)
                return new List<ConfiguredTestDto>();

            var term = query.Trim().ToLower();

            var labTests = await _context.LaboratoryTests
                .AsNoTracking()
                .Include(t => t.LabDepartment)
                .Include(t => t.SubGroups)
                .Where(t => t.IsActive && (t.Name.ToLower().Contains(term) || t.SubGroups.Any(sg => sg.Name.ToLower().Contains(term) || (sg.ReportTestName != null && sg.ReportTestName.ToLower().Contains(term)))))
                .Take(20)
                .ToListAsync();

            return labTests.Select(t =>
            {
                var isChem = t.IsChemicalTest || (t.LabDepartment != null && t.LabDepartment.IsChemical);
                return new ConfiguredTestDto
                {
                    LaboratoryTestID = t.ID,
                    LaboratoryTestName = t.Name,
                    TestType = isChem ? "Chemical" : "General",
                    SubGroup = t.SubGroups.FirstOrDefault() != null ? t.SubGroups.FirstOrDefault()!.Name : "",
                    SourceTag = "Universal Master",
                    SourceTags = new List<string> { "Universal Master" },
                    TestMethodSpecificationID = null,
                    TestMethodSpecificationName = "",
                    Quantity = 1
                };
            }).ToList();
        }

        private async Task<ConfiguredGradeDto> BuildGradeDtoAsync(long gradeId, string gradeName, string specName, long productMasterId = 0)
        {
            var specGrade = await _context.SpecificationGrades
                .AsNoTracking()
                .FirstOrDefaultAsync(g => g.ID == gradeId);

            long headerId = specGrade?.SpecificationHeaderID ?? 0;

            // 1. Fetch SubGroup Specification Mappings (searching ProductMasterID or GradeID or HeaderID)
            var subgroupTests = await _context.Set<LaboratoryTestSubGroupSpecification>()
                .AsNoTracking()
                .Include(s => s.SubGroup)
                    .ThenInclude(sg => sg!.LaboratoryTest)
                        .ThenInclude(lt => lt!.LabDepartment)
                .Include(s => s.SubGroup)
                    .ThenInclude(sg => sg!.TestMethods)
                        .ThenInclude(tm => tm.TestMethodSpecification)
                .Where(s => ((productMasterId > 0 && s.ProductMasterID == productMasterId) || s.SpecificationGradeID == gradeId || (headerId > 0 && s.SpecificationHeaderID == headerId))
                            && s.SubGroup != null && s.SubGroup.LaboratoryTest != null && s.SubGroup.LaboratoryTest.IsActive)
                .Select(s => new ConfiguredTestDto
                {
                    LaboratoryTestID = s.SubGroup!.LaboratoryTestID,
                    LaboratoryTestSubGroupID = s.LaboratoryTestSubGroupID,
                    LaboratoryTestName = s.SubGroup.LaboratoryTest!.Name,
                    TestType = (s.SubGroup.LaboratoryTest.IsChemicalTest || (s.SubGroup.LaboratoryTest.LabDepartment != null && s.SubGroup.LaboratoryTest.LabDepartment.IsChemical))
                               ? "Chemical" : "General",
                    SubGroup = s.SubGroup.Name,
                    SourceTag = (productMasterId > 0 && s.ProductMasterID == productMasterId) ? "Product Master" : "Lab Scope",
                    TestMethodSpecificationID = (s.SubGroup.TestMethods.FirstOrDefault(m => m.IsDefault) ?? s.SubGroup.TestMethods.FirstOrDefault()) != null 
                        ? (s.SubGroup.TestMethods.FirstOrDefault(m => m.IsDefault) ?? s.SubGroup.TestMethods.FirstOrDefault())!.TestMethodSpecificationID : null,
                    TestMethodSpecificationName = (s.SubGroup.TestMethods.FirstOrDefault(m => m.IsDefault) ?? s.SubGroup.TestMethods.FirstOrDefault()) != null && (s.SubGroup.TestMethods.FirstOrDefault(m => m.IsDefault) ?? s.SubGroup.TestMethods.FirstOrDefault())!.TestMethodSpecification != null
                        ? ((s.SubGroup.TestMethods.FirstOrDefault(m => m.IsDefault) ?? s.SubGroup.TestMethods.FirstOrDefault())!.TestMethodSpecification!.DisplayTitle ?? (s.SubGroup.TestMethods.FirstOrDefault(m => m.IsDefault) ?? s.SubGroup.TestMethods.FirstOrDefault())!.TestMethodSpecification!.Name) : "",
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
                .Include(s => s.AnalysisType)
                    .ThenInclude(at => at!.TestMethods)
                        .ThenInclude(tm => tm.TestMethodSpecification)
                .Include(s => s.AnalysisType)
                    .ThenInclude(at => at!.AllowedTechniques)
                        .ThenInclude(t => t.AnalysisTechnique)
                .Where(s => ((productMasterId > 0 && s.ProductMasterID == productMasterId) || s.SpecificationGradeID == gradeId || (headerId > 0 && s.SpecificationHeaderID == headerId))
                            && s.AnalysisType != null && s.AnalysisType.SubGroup != null && s.AnalysisType.SubGroup.LaboratoryTest != null && s.AnalysisType.SubGroup.LaboratoryTest.IsActive)
                .Select(s => new ConfiguredTestDto
                {
                    LaboratoryTestID = s.AnalysisType!.SubGroup!.LaboratoryTestID,
                    LaboratoryTestSubGroupID = s.AnalysisType!.LaboratoryTestSubGroupID,
                    LaboratoryTestAnalysisTypeID = s.LaboratoryTestAnalysisTypeID,
                    LaboratoryTestName = s.AnalysisType.SubGroup.LaboratoryTest!.Name,
                    TestType = "Chemical",
                    SubGroup = s.AnalysisType.Name,
                    SourceTag = (productMasterId > 0 && s.ProductMasterID == productMasterId) ? "Product Master" : "Lab Scope",
                    TestMethodSpecificationID = (s.AnalysisType.TestMethods.FirstOrDefault(m => m.IsDefault) ?? s.AnalysisType.TestMethods.FirstOrDefault()) != null
                        ? (s.AnalysisType.TestMethods.FirstOrDefault(m => m.IsDefault) ?? s.AnalysisType.TestMethods.FirstOrDefault())!.TestMethodSpecificationID : null,
                    TestMethodSpecificationName = (s.AnalysisType.TestMethods.FirstOrDefault(m => m.IsDefault) ?? s.AnalysisType.TestMethods.FirstOrDefault()) != null && (s.AnalysisType.TestMethods.FirstOrDefault(m => m.IsDefault) ?? s.AnalysisType.TestMethods.FirstOrDefault())!.TestMethodSpecification != null
                        ? ((s.AnalysisType.TestMethods.FirstOrDefault(m => m.IsDefault) ?? s.AnalysisType.TestMethods.FirstOrDefault())!.TestMethodSpecification!.DisplayTitle ?? (s.AnalysisType.TestMethods.FirstOrDefault(m => m.IsDefault) ?? s.AnalysisType.TestMethods.FirstOrDefault())!.TestMethodSpecification!.Name) : "",
                    TechniqueIDs = s.AnalysisType.AllowedTechniques.Select(t => t.AnalysisTechniqueID).ToList(),
                    TechniqueCodes = s.AnalysisType.AllowedTechniques.Select(t => t.AnalysisTechnique != null ? t.AnalysisTechnique.Code ?? "" : "").Where(c => c != "").ToList(),
                    TechniqueNames = s.AnalysisType.AllowedTechniques.Select(t => t.AnalysisTechnique != null ? t.AnalysisTechnique.Name ?? "" : "").Where(n => n != "").ToList(),
                    Quantity = 1
                })
                .ToListAsync();

            // 3. Fetch Specification Line Laboratory Tests (Configured via Product Master Grade / Spec)
            var rawSpecLines = await _context.SpecificationLines
                .AsNoTracking()
                .Include(l => l.LaboratoryTest)
                    .ThenInclude(lt => lt!.LabDepartment)
                .Include(l => l.LaboratoryTest)
                    .ThenInclude(lt => lt!.SubGroups)
                        .ThenInclude(sg => sg.AnalysisTypes)
                .Include(l => l.LaboratoryTest)
                    .ThenInclude(lt => lt!.SubGroups)
                        .ThenInclude(sg => sg.TestMethods)
                            .ThenInclude(tm => tm.TestMethodSpecification)
                .Include(l => l.TestMethodMappings)
                    .ThenInclude(tm => tm.TestMethodSpecification)
                .Where(l => l.SpecificationGradeID == gradeId && l.LaboratoryTestID != null && l.LaboratoryTest != null && l.LaboratoryTest.IsActive)
                .ToListAsync();

            var specLineTests = rawSpecLines.Select(l =>
            {
                var isChem = l.LaboratoryTest!.IsChemicalTest || (l.LaboratoryTest.LabDepartment != null && l.LaboratoryTest.LabDepartment.IsChemical);
                var firstSg = l.LaboratoryTest.SubGroups.FirstOrDefault(sg => sg.IsActive) ?? l.LaboratoryTest.SubGroups.FirstOrDefault();
                var firstAt = firstSg?.AnalysisTypes.FirstOrDefault(at => at.IsActive) ?? firstSg?.AnalysisTypes.FirstOrDefault();
                var firstMethod = l.TestMethodMappings.FirstOrDefault()?.TestMethodSpecification 
                                  ?? firstSg?.TestMethods.FirstOrDefault(tm => tm.IsDefault)?.TestMethodSpecification 
                                  ?? firstSg?.TestMethods.FirstOrDefault()?.TestMethodSpecification;

                return new ConfiguredTestDto
                {
                    LaboratoryTestID = l.LaboratoryTestID!.Value,
                    LaboratoryTestSubGroupID = firstSg?.ID,
                    LaboratoryTestAnalysisTypeID = isChem ? firstAt?.ID : null,
                    LaboratoryTestName = l.LaboratoryTest.Name,
                    TestType = isChem ? "Chemical" : "General",
                    SubGroup = isChem ? (firstAt?.Name ?? firstSg?.Name ?? "Chemical") : (firstSg?.Name ?? l.Type ?? "General"),
                    SourceTag = "PM Scope",
                    TestMethodSpecificationID = l.TestMethodMappings.FirstOrDefault()?.TestMethodSpecificationID 
                                                ?? (firstSg?.TestMethods.FirstOrDefault(m => m.IsDefault) ?? firstSg?.TestMethods.FirstOrDefault())?.TestMethodSpecificationID,
                    TestMethodSpecificationName = firstMethod != null ? (firstMethod.DisplayTitle ?? firstMethod.Name) : "",
                    Quantity = 1
                };
            }).ToList();

            var allTests = subgroupTests.Concat(analysisTypeTests).Concat(specLineTests).ToList();

            var combinedTests = allTests
                .GroupBy(t => t.LaboratoryTestID)
                .Select(g =>
                {
                    var tags = g.Select(x => x.SourceTag).Where(t => !string.IsNullOrEmpty(t)).Distinct().ToList();
                    // Prefer Lab Scope version if available, otherwise first
                    var preferred = g.FirstOrDefault(x => x.SourceTag == "Lab Scope") ?? g.First();
                    preferred.SourceTags = tags;
                    if (tags.Count > 1)
                    {
                        preferred.SourceTag = "Both Sources";
                    }
                    else
                    {
                        preferred.SourceTag = tags.FirstOrDefault() ?? "PM Scope";
                    }
                    var withSubGroup = g.FirstOrDefault(x => x.LaboratoryTestSubGroupID.HasValue && x.LaboratoryTestSubGroupID.Value > 0);
                    if (withSubGroup != null && (!preferred.LaboratoryTestSubGroupID.HasValue || preferred.LaboratoryTestSubGroupID.Value == 0))
                    {
                        preferred.LaboratoryTestSubGroupID = withSubGroup.LaboratoryTestSubGroupID;
                    }
                    var withAnalysisType = g.FirstOrDefault(x => x.LaboratoryTestAnalysisTypeID.HasValue && x.LaboratoryTestAnalysisTypeID.Value > 0);
                    if (withAnalysisType != null && (!preferred.LaboratoryTestAnalysisTypeID.HasValue || preferred.LaboratoryTestAnalysisTypeID.Value == 0))
                    {
                        preferred.LaboratoryTestAnalysisTypeID = withAnalysisType.LaboratoryTestAnalysisTypeID;
                    }
                    var withMethod = g.FirstOrDefault(x => x.TestMethodSpecificationID.HasValue && x.TestMethodSpecificationID.Value > 0);
                    if (withMethod != null && (!preferred.TestMethodSpecificationID.HasValue || preferred.TestMethodSpecificationID.Value == 0))
                    {
                        preferred.TestMethodSpecificationID = withMethod.TestMethodSpecificationID;
                        preferred.TestMethodSpecificationName = withMethod.TestMethodSpecificationName;
                    }
                    return preferred;
                })
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
                IsScopeConfigured = (combinedTests.Count > 0 || chemicalLines.Count > 0),
                ConfiguredTests = combinedTests,
                ChemicalElements = chemicalLines
            };
        }
    }
}
