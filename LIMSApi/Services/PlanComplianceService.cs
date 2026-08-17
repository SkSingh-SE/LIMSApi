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
    public class PlanComplianceService : IPlanComplianceService
    {
        private readonly LIMSContext _context;
        private readonly ILogger<PlanComplianceService> _logger;

        public PlanComplianceService(LIMSContext context, ILogger<PlanComplianceService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<PlanComplianceResultDto> EvaluateComplianceAsync(PlanComplianceRequestDto request)
        {
            var result = new PlanComplianceResultDto
            {
                IsFullyCompliant = true,
                IsScopeConfigured = true,
                IsUnknownSampleWorkflow = request.IsUnknownSample
            };

            // 1. Handle Unknown Sample Workflow
            if (request.IsUnknownSample)
            {
                result.ComplianceBadge = "Unknown Sample";
                result.Message = "Unknown sample workflow active. Testing driven directly by Laboratory Test Master.";
                return result;
            }

            // 2. Evaluate Scope Configuration for Grade
            if (request.SpecificationGradeID.HasValue && request.SpecificationGradeID.Value > 0)
            {
                long gradeId = request.SpecificationGradeID.Value;
                var specGrade = await _context.SpecificationGrades.AsNoTracking().FirstOrDefaultAsync(g => g.ID == gradeId);
                long headerId = specGrade?.SpecificationHeaderID ?? 0;

                bool hasSubGroupSpecs = await _context.Set<LaboratoryTestSubGroupSpecification>()
                    .AnyAsync(s => s.SpecificationGradeID == gradeId || (headerId > 0 && s.SpecificationHeaderID == headerId));

                bool hasAnalysisSpecs = await _context.Set<LaboratoryTestAnalysisTypeSpecification>()
                    .AnyAsync(s => s.SpecificationGradeID == gradeId || (headerId > 0 && s.SpecificationHeaderID == headerId));

                bool hasLines = await _context.SpecificationLines
                    .AnyAsync(l => l.SpecificationGradeID == gradeId);

                if (!hasSubGroupSpecs && !hasAnalysisSpecs && !hasLines)
                {
                    result.IsScopeConfigured = false;
                    result.ComplianceBadge = "Scope Not Configured";
                    result.Message = "Scope Master Not Configured for selected Grade. Observed values will be printed without standard min/max limits.";
                }
            }

            // 3. Evaluate SubGroup Standard Mapping (Non-Chemical Tests)
            if (request.LaboratoryTestSubGroupID.HasValue && request.LaboratoryTestSubGroupID.Value > 0 && request.TestMethodSpecificationID.HasValue && request.TestMethodSpecificationID.Value > 0)
            {
                long subGroupId = request.LaboratoryTestSubGroupID.Value;
                long specId = request.TestMethodSpecificationID.Value;

                bool isMapped = await _context.LaboratoryTestSubGroupMethods
                    .AnyAsync(m => m.LaboratoryTestSubGroupID == subGroupId && m.TestMethodSpecificationID == specId);

                if (!isMapped)
                {
                    result.IsFullyCompliant = false;
                    result.DeviationNotes.Add("Selected Test Method Standard is not in the configured master list for this SubGroup.");
                }

                // Fetch recommended standards for suggestions
                result.RecommendedStandards = await _context.LaboratoryTestSubGroupMethods
                    .AsNoTracking()
                    .Include(m => m.TestMethodSpecification)
                    .Where(m => m.LaboratoryTestSubGroupID == subGroupId && m.TestMethodSpecification != null && !m.TestMethodSpecification.IsDisabled)
                    .Select(m => new DropdwonSelector
                    {
                        Id = m.TestMethodSpecificationID,
                        Name = m.TestMethodSpecification!.DisplayTitle ?? m.TestMethodSpecification.Name
                    })
                    .Distinct()
                    .ToListAsync();
            }

            // 4. Evaluate Analysis Type Standard Mapping (Chemical Tests)
            if (request.LaboratoryTestAnalysisTypeID.HasValue && request.LaboratoryTestAnalysisTypeID.Value > 0 && request.TestMethodSpecificationID.HasValue && request.TestMethodSpecificationID.Value > 0)
            {
                long analysisTypeId = request.LaboratoryTestAnalysisTypeID.Value;
                long specId = request.TestMethodSpecificationID.Value;

                bool isMapped = await _context.LaboratoryTestAnalysisTypeMethods
                    .AnyAsync(m => m.LaboratoryTestAnalysisTypeID == analysisTypeId && m.TestMethodSpecificationID == specId);

                if (!isMapped)
                {
                    result.IsFullyCompliant = false;
                    result.DeviationNotes.Add("Selected Test Method Standard is not in the configured master list for this Analysis Type.");
                }

                if (result.RecommendedStandards.Count == 0)
                {
                    result.RecommendedStandards = await _context.LaboratoryTestAnalysisTypeMethods
                        .AsNoTracking()
                        .Include(m => m.TestMethodSpecification)
                        .Where(m => m.LaboratoryTestAnalysisTypeID == analysisTypeId && m.TestMethodSpecification != null && !m.TestMethodSpecification.IsDisabled)
                        .Select(m => new DropdwonSelector
                        {
                            Id = m.TestMethodSpecificationID,
                            Name = m.TestMethodSpecification!.DisplayTitle ?? m.TestMethodSpecification.Name
                        })
                        .Distinct()
                        .ToListAsync();
                }
            }

            result.ComplianceBadge = !result.IsScopeConfigured ? "Scope Not Configured" : (result.IsFullyCompliant ? "Configured" : "Custom Selection");
            return result;
        }

        public async Task<List<DropdwonSelector>> GetChemicalParametersForAnalysisTypeAsync(long analysisTypeId)
        {
            var parameters = await _context.LaboratoryTestAnalysisTypeParameters
                .AsNoTracking()
                .Include(p => p.Parameter)
                    .ThenInclude(pm => pm!.ParameterUnit)
                .Where(p => p.LaboratoryTestAnalysisTypeID == analysisTypeId && p.Parameter != null)
                .Select(p => new DropdwonSelector
                {
                    Id = p.ParameterID,
                    Name = string.IsNullOrWhiteSpace(p.Parameter!.Symbol) ? p.Parameter.Name : $"{p.Parameter.Name} ({p.Parameter.Symbol})"
                })
                .Distinct()
                .ToListAsync();

            return parameters;
        }
    }
}
