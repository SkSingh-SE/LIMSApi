using System.Text.Json;
using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers.Enums;
using LIMSApi.Models;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Helpers
{
    public class EffectiveConfigurationResolver : IEffectiveConfigurationResolver
    {
        private readonly LIMSContext _context;
        private readonly FormulaEvaluator _formulaEvaluator;

        public EffectiveConfigurationResolver(LIMSContext context, FormulaEvaluator formulaEvaluator)
        {
            _context = context;
            _formulaEvaluator = formulaEvaluator;
        }

        public async Task<UniversalPlanPreviewResponseDto> ResolveEffectiveConfigurationAsync(UniversalPlanPreviewRequestDto request)
        {
            var response = new UniversalPlanPreviewResponseDto();
            var val = response.ValidationSummary;
            var refDate = request.ReferenceDate ?? DateTime.UtcNow;

            // 1. Sample & Product/Grade Gate
            SampleDetail? sample = null;
            if (request.SampleID > 0)
            {
                sample = await _context.SampleDetails
                    .Include(s => s.SampleInward)
                    .Include(s => s.ProductMaster)
                    .Include(s => s.SpecificationGrade)
                        .ThenInclude(g => g!.SpecificationHeader)
                    .FirstOrDefaultAsync(s => s.ID == request.SampleID);
            }

            if (sample == null)
            {
                val.SamplePass = false;
                val.SampleMessage = $"Sample with ID {request.SampleID} not found.";
                val.BlockingErrors.Add(val.SampleMessage);
            }
            else
            {
                val.SamplePass = true;
                refDate = sample.SampleInward?.CollectionTime ?? sample.SampleInward?.CreatedOn ?? refDate;
            }

            // Product & Grade
            long? gradeId = request.SpecificationGradeID ?? sample?.SpecificationGradeID;
            long? specHeaderId = request.SpecificationHeaderID;

            if (gradeId.HasValue && gradeId.Value > 0)
            {
                var grade = await _context.SpecificationGrades
                    .Include(g => g.SpecificationHeader)
                    .FirstOrDefaultAsync(g => g.ID == gradeId.Value);

                if (grade != null)
                {
                    response.SpecificationGradeID = grade.ID;
                    response.GradeName = grade.Grade;
                    specHeaderId ??= grade.SpecificationHeaderID;
                    val.ProductGradePass = true;
                }
                else
                {
                    val.ProductGradePass = false;
                    val.ProductGradeMessage = $"Specification Grade {gradeId.Value} not found.";
                    val.BlockingErrors.Add(val.ProductGradeMessage);
                }
            }
            else if (!specHeaderId.HasValue || specHeaderId.Value == 0)
            {
                // Configuration-driven: N/A for standardless/ad-hoc testing without specification grade
                val.ProductGradePass = true;
                val.ProductGradeMessage = "N/A — No Product Grade linked (Standardless / Ad-hoc Testing).";
            }
            else
            {
                val.ProductGradePass = false;
                val.ProductGradeMessage = "Applicable Product Grade is required for specification-driven test planning.";
                val.BlockingErrors.Add(val.ProductGradeMessage);
            }

            // Specification Header
            if (specHeaderId.HasValue && specHeaderId.Value > 0)
            {
                var header = await _context.SpecificationHeaders.FirstOrDefaultAsync(h => h.ID == specHeaderId.Value);
                if (header != null)
                {
                    response.SpecificationHeaderID = header.ID;
                    response.SpecificationTitle = header.DisplayTitle ?? header.AliasName ?? header.Code;
                    val.SpecificationPass = true;
                }
                else
                {
                    val.SpecificationPass = false;
                    val.SpecificationMessage = $"Specification Header {specHeaderId.Value} not found.";
                    val.BlockingErrors.Add(val.SpecificationMessage);
                }
            }
            else
            {
                // Configuration-driven: N/A when specification is not applicable (Ad-hoc / standardless testing)
                val.SpecificationPass = true;
                val.SpecificationMessage = "N/A — No Specification Header linked (Ad-hoc / Standardless Test).";
            }

            // 2. Specification Version Gate (Strict Fail Loud on zero / ambiguity; explicit superseded override permitted)
            long resolvedSpecVersionId = 0;
            if (specHeaderId.HasValue && specHeaderId.Value > 0)
            {
                var allSpecVersions = await _context.SpecificationVersions
                    .Where(v => v.SpecificationHeaderID == specHeaderId.Value)
                    .OrderByDescending(v => v.IsDefault)
                    .ThenByDescending(v => v.ID)
                    .ToListAsync();

                if (request.SpecificationVersionID.HasValue && request.SpecificationVersionID.Value > 0)
                {
                    var requestedVer = allSpecVersions.FirstOrDefault(v => v.ID == request.SpecificationVersionID.Value);
                    if (requestedVer == null)
                    {
                        val.SpecificationVersionPass = false;
                        val.SpecificationVersionMessage = $"Requested Specification Version {request.SpecificationVersionID.Value} does not belong to Specification Header {specHeaderId.Value}.";
                        val.BlockingErrors.Add(val.SpecificationVersionMessage);
                    }
                    else
                    {
                        resolvedSpecVersionId = requestedVer.ID;
                        response.SpecificationVersionID = requestedVer.ID;
                        response.SpecificationVersionNumber = requestedVer.Version;
                        response.IsSupersededSpecVersion = requestedVer.Status == VersionStatus.Superseded;
                        val.SpecificationVersionPass = true;
                    }
                }
                else
                {
                    // Auto-resolve active version on reference date
                    var activeCandidateVersions = allSpecVersions
                        .Where(v => v.Status == VersionStatus.Active &&
                                    (v.EffectiveDate == null || v.EffectiveDate <= refDate) &&
                                    (v.SupersededDate == null || v.SupersededDate > refDate))
                        .ToList();

                    if (activeCandidateVersions.Count == 0)
                    {
                        val.SpecificationVersionPass = false;
                        val.SpecificationVersionMessage = $"No active specification version is effective for reference date {refDate:yyyy-MM-dd} under Specification '{response.SpecificationTitle}'.";
                        val.BlockingErrors.Add(val.SpecificationVersionMessage);
                    }
                    else if (activeCandidateVersions.Count == 1)
                    {
                        var v = activeCandidateVersions[0];
                        resolvedSpecVersionId = v.ID;
                        response.SpecificationVersionID = v.ID;
                        response.SpecificationVersionNumber = v.Version;
                        response.IsSupersededSpecVersion = false;
                        val.SpecificationVersionPass = true;
                    }
                    else
                    {
                        var defaultVer = activeCandidateVersions.FirstOrDefault(v => v.IsDefault);
                        if (defaultVer != null && activeCandidateVersions.Count(v => v.IsDefault) == 1)
                        {
                            resolvedSpecVersionId = defaultVer.ID;
                            response.SpecificationVersionID = defaultVer.ID;
                            response.SpecificationVersionNumber = defaultVer.Version;
                            response.IsSupersededSpecVersion = false;
                            val.SpecificationVersionPass = true;
                        }
                        else
                        {
                            val.SpecificationVersionPass = false;
                            val.SpecificationVersionMessage = $"Ambiguous specification versions: {activeCandidateVersions.Count} active versions are effective for reference date {refDate:yyyy-MM-dd}. Exactly one must be designated as default, or an explicit version must be selected.";
                            val.BlockingErrors.Add(val.SpecificationVersionMessage);
                        }
                    }
                }
            }
            else
            {
                // Configuration-driven: N/A when specification version is not required
                val.SpecificationVersionPass = true;
                val.SpecificationVersionMessage = "N/A — Specification Version not required for standardless test.";
            }

            // 3. Laboratory Test Definition Gate (Screen 13 Direct Mappings Only)
            var test = await _context.LaboratoryTests
                .Include(t => t.Discipline)
                .Include(t => t.LabDepartment)
                .Include(t => t.Parameters.Where(p => p.IsActive))
                    .ThenInclude(p => p.Parameter)
                        .ThenInclude(pm => pm!.ParameterUnit)
                .Include(t => t.Methods.Where(m => m.IsActive))
                    .ThenInclude(m => m.TestMethodSpecification)
                .Include(t => t.Conditions.Where(c => c.IsActive))
                    .ThenInclude(c => c.ConditionMaster)
                        .ThenInclude(cm => cm.ParameterUnit)
                .FirstOrDefaultAsync(t => t.ID == request.LaboratoryTestID);

            if (test == null)
            {
                val.TestDefinitionPass = false;
                val.TestDefinitionMessage = $"Laboratory Test Definition {request.LaboratoryTestID} not found.";
                val.BlockingErrors.Add(val.TestDefinitionMessage);
                return response;
            }

            if (!test.IsActive)
            {
                val.TestDefinitionPass = false;
                val.TestDefinitionMessage = $"Laboratory Test '{test.Name}' is inactive.";
                val.BlockingErrors.Add(val.TestDefinitionMessage);
            }
            else
            {
                val.TestDefinitionPass = true;
            }

            response.LaboratoryTestID = test.ID;
            response.LaboratoryTestCode = test.Code;
            response.LaboratoryTestName = test.Name;
            response.DisciplineID = test.DisciplineID;
            response.DisciplineName = test.Discipline?.Name;

            // 4. Test Method & Method Version Gate (Screen 13 Direct Methods)
            var supportedMethods = test.Methods.Where(m => m.IsActive).ToList();
            LaboratoryTestMethod? selectedMethodMapping = null;

            if (request.TestMethodSpecificationID.HasValue && request.TestMethodSpecificationID.Value > 0)
            {
                selectedMethodMapping = supportedMethods.FirstOrDefault(m => m.TestMethodSpecificationID == request.TestMethodSpecificationID.Value);
            }
            else
            {
                selectedMethodMapping = supportedMethods.FirstOrDefault(m => m.IsDefault) ?? supportedMethods.FirstOrDefault();
            }

            if (selectedMethodMapping == null)
            {
                val.TestMethodPass = false;
                val.TestMethodMessage = $"No supported test method is configured for Laboratory Test '{test.Name}'.";
                val.BlockingErrors.Add(val.TestMethodMessage);
            }
            else
            {
                val.TestMethodPass = true;
                response.TestMethodSpecificationID = selectedMethodMapping.TestMethodSpecificationID;
                response.TestMethodName = selectedMethodMapping.TestMethodSpecification?.Name;
                response.TestMethodStandard = selectedMethodMapping.TestMethodSpecification?.TestMethodStandard;

                // Load all versions for this method (for preview dropdown)
                var allMethodVersions = await _context.TestMethodSpecificationVersions
                    .Where(v => v.TestMethodSpecificationID == selectedMethodMapping.TestMethodSpecificationID)
                    .OrderByDescending(v => v.IsDefault)
                    .ThenByDescending(v => v.ID)
                    .ToListAsync();

                response.AvailableMethodVersions = allMethodVersions.Select(v => new TestMethodVersionOptionDto
                {
                    ID = v.ID,
                    Version = v.Version,
                    Year = v.Year,
                    Status = v.Status.ToString(),
                    IsDefault = v.IsDefault,
                    IsActive = v.Status == VersionStatus.Active,
                    IsSuperseded = v.Status == VersionStatus.Superseded,
                    EffectiveDate = v.EffectiveDate,
                    SupersededDate = v.SupersededDate
                }).ToList();

                // Method Version Resolution
                if (request.TestMethodSpecificationVersionID.HasValue && request.TestMethodSpecificationVersionID.Value > 0)
                {
                    var requestedMv = allMethodVersions.FirstOrDefault(v => v.ID == request.TestMethodSpecificationVersionID.Value);
                    if (requestedMv == null)
                    {
                        val.MethodVersionPass = false;
                        val.MethodVersionMessage = $"Requested Method Version {request.TestMethodSpecificationVersionID.Value} not found for method '{response.TestMethodName}'.";
                        val.BlockingErrors.Add(val.MethodVersionMessage);
                    }
                    else
                    {
                        response.TestMethodSpecificationVersionID = requestedMv.ID;
                        response.TestMethodVersion = requestedMv.Version;
                        response.IsSupersededMethodVersion = requestedMv.Status == VersionStatus.Superseded;
                        val.MethodVersionPass = true;
                    }
                }
                else
                {
                    // Auto-resolve active method version on reference date
                    var activeMethodCandidates = allMethodVersions
                        .Where(v => v.Status == VersionStatus.Active &&
                                    (v.EffectiveDate == null || v.EffectiveDate <= refDate) &&
                                    (v.SupersededDate == null || v.SupersededDate > refDate))
                        .ToList();

                    if (activeMethodCandidates.Count == 0)
                    {
                        val.MethodVersionPass = false;
                        val.MethodVersionMessage = $"No valid active method version is effective for Test Method '{response.TestMethodName}' on reference date {refDate:yyyy-MM-dd}.";
                        val.BlockingErrors.Add(val.MethodVersionMessage);
                    }
                    else if (activeMethodCandidates.Count == 1)
                    {
                        var mv = activeMethodCandidates[0];
                        response.TestMethodSpecificationVersionID = mv.ID;
                        response.TestMethodVersion = mv.Version;
                        response.IsSupersededMethodVersion = false;
                        val.MethodVersionPass = true;
                    }
                    else
                    {
                        var defaultMv = activeMethodCandidates.FirstOrDefault(v => v.IsDefault);
                        if (defaultMv != null && activeMethodCandidates.Count(v => v.IsDefault) == 1)
                        {
                            response.TestMethodSpecificationVersionID = defaultMv.ID;
                            response.TestMethodVersion = defaultMv.Version;
                            response.IsSupersededMethodVersion = false;
                            val.MethodVersionPass = true;
                        }
                        else
                        {
                            val.MethodVersionPass = false;
                            val.MethodVersionMessage = $"Ambiguous method versions: {activeMethodCandidates.Count} active versions are effective for Test Method '{response.TestMethodName}' on reference date {refDate:yyyy-MM-dd}. Exactly one must be designated as default, or an explicit version must be selected.";
                            val.BlockingErrors.Add(val.MethodVersionMessage);
                        }
                    }
                }
            }

            // 5. Branch & Department Routing Gate (BranchID + DisciplineID -> Department)
            long branchId = request.BranchID > 0 ? request.BranchID : (sample?.SampleInward?.BranchID ?? 1);
            var branch = await _context.Branches.FirstOrDefaultAsync(b => b.ID == branchId);

            if (branch == null || !branch.IsActive)
            {
                val.BranchPass = false;
                val.BranchMessage = $"Execution Branch {branchId} is invalid or inactive.";
                val.BlockingErrors.Add(val.BranchMessage);
            }
            else
            {
                val.BranchPass = true;
                response.BranchID = branch.ID;
                response.BranchName = branch.Name;

                // Department Resolution
                DepartmentMaster? dept = null;
                if (test.DisciplineID.HasValue)
                {
                    dept = await _context.DepartmentMasters
                        .FirstOrDefaultAsync(d => d.BranchID == branch.ID && d.DisciplineID == test.DisciplineID.Value && d.IsActive);
                }

                dept ??= await _context.DepartmentMasters
                    .FirstOrDefaultAsync(d => d.BranchID == branch.ID && d.IsActive);

                if (dept != null)
                {
                    response.DepartmentID = dept.ID;
                    response.DepartmentName = dept.Name;
                    response.DepartmentRoutingSource = $"Resolved from Branch ({branch.Name}) + Discipline ({response.DisciplineName ?? "General"})";
                    val.DepartmentRoutingPass = true;
                }
                else
                {
                    val.DepartmentRoutingPass = false;
                    val.DepartmentRoutingMessage = $"No active laboratory department found for Branch '{branch.Name}' and Discipline '{response.DisciplineName ?? "General"}'.";
                    val.BlockingErrors.Add(val.DepartmentRoutingMessage);
                }
            }

            // 6. Parameters & Specification Requirements Gate
            List<SpecificationLine> specLines = new();
            if (resolvedSpecVersionId > 0 && gradeId.HasValue)
            {
                specLines = await _context.SpecificationLines
                    .Include(sl => sl.Parameter)
                        .ThenInclude(p => p!.ParameterUnit)
                    .Include(sl => sl.ParameterUnit)
                    .Where(sl => sl.SpecificationVersionID == resolvedSpecVersionId &&
                                 sl.SpecificationGradeID == gradeId.Value &&
                                 (sl.LaboratoryTestID == test.ID || sl.LaboratoryTestID == null))
                    .ToListAsync();
            }

            var specLinesByParamId = specLines.ToDictionary(l => l.ParameterID, l => l);
            bool allMandatoryParamsSatisfied = true;

            foreach (var tp in test.Parameters.Where(p => p.IsActive).OrderBy(p => p.DisplayOrder))
            {
                var pm = tp.Parameter;
                if (pm == null) continue;

                specLinesByParamId.TryGetValue(pm.ID, out var specLine);

                string reqText = "-";
                decimal? minVal = specLine?.LowerLimitDecimalValue;
                decimal? maxVal = specLine?.UpperLimitDecimalValue;
                string? formula = specLine?.Equation ?? pm.Formula;

                if (minVal.HasValue && maxVal.HasValue)
                {
                    reqText = $"{minVal.Value} – {maxVal.Value}";
                }
                else if (minVal.HasValue)
                {
                    reqText = $"≥ {minVal.Value}";
                }
                else if (maxVal.HasValue)
                {
                    reqText = $"≤ {maxVal.Value}";
                }
                else if (!string.IsNullOrWhiteSpace(specLine?.TestCondition))
                {
                    reqText = specLine.TestCondition;
                }

                string unit = specLine?.ParameterUnit?.Symbol ??
                              specLine?.ParameterUnit?.Name ??
                              pm.ParameterUnit?.Symbol ??
                              pm.ParameterUnit?.Name ?? "-";

                bool hasReq = specLine != null;
                if (tp.IsMandatory && !hasReq)
                {
                    allMandatoryParamsSatisfied = false;
                    if (specHeaderId.HasValue && specHeaderId.Value > 0)
                    {
                        val.Warnings.Add($"Mandatory parameter '{pm.Name}' has no configured requirement in Specification Version {response.SpecificationVersionNumber}.");
                    }
                }

                response.Parameters.Add(new PreviewParameterDto
                {
                    ParameterID = pm.ID,
                    ParameterCode = !string.IsNullOrWhiteSpace(pm.Code) ? pm.Code : $"PARAM_{pm.ID}",
                    ParameterName = pm.Name,
                    ParameterUnit = unit,
                    InputType = pm.InputType ?? "Decimal",
                    IsMandatory = tp.IsMandatory,
                    IsReportable = tp.IsReportable,
                    RequirementText = reqText,
                    MinValue = minVal,
                    MaxValue = maxVal,
                    AcceptanceCriteria = specLine?.TestCondition,
                    Equation = formula,
                    HasRequirement = hasReq,
                    Status = tp.IsMandatory ? "Required" : "Optional"
                });
            }

            if (!specHeaderId.HasValue || specHeaderId.Value <= 0)
            {
                val.MandatoryParametersPass = true;
                val.MandatoryParametersMessage = "N/A — Standardless test without specification requirements.";
            }
            else
            {
                val.MandatoryParametersPass = allMandatoryParamsSatisfied;
                if (!allMandatoryParamsSatisfied)
                {
                    val.MandatoryParametersMessage = "One or more mandatory test parameters do not have configured specification requirements.";
                    val.BlockingErrors.Add(val.MandatoryParametersMessage);
                }
            }

            // 7. Conditions Gate
            var specLineIds = specLines.Select(s => s.ID).ToList();
            var lineConditions = await _context.SpecificationLineConditions
                .Include(c => c.ConditionMaster)
                    .ThenInclude(cm => cm.ParameterUnit)
                .Where(c => specLineIds.Contains(c.SpecificationLineID))
                .ToListAsync();

            var lineCondsByMasterId = lineConditions.ToDictionary(c => c.ConditionMasterID, c => c);
            bool allMandatoryConditionsSatisfied = true;

            foreach (var tc in test.Conditions.Where(c => c.IsActive).OrderBy(c => c.DisplayOrder))
            {
                var cm = tc.ConditionMaster;
                if (cm == null) continue;

                lineCondsByMasterId.TryGetValue(cm.ID, out var lineCond);

                string confVal = lineCond?.Value1 ?? "-";
                if (!string.IsNullOrWhiteSpace(lineCond?.Value2))
                {
                    confVal = $"{lineCond.Value1} – {lineCond.Value2}";
                }
                string unit = cm.ParameterUnit?.Symbol ?? cm.ParameterUnit?.Name ?? "";
                if (!string.IsNullOrWhiteSpace(unit) && confVal != "-")
                {
                    confVal = $"{confVal} {unit}";
                }

                bool hasConf = lineCond != null && !string.IsNullOrWhiteSpace(confVal) && confVal != "-";
                if (tc.IsMandatory && !hasConf)
                {
                    allMandatoryConditionsSatisfied = false;
                    if (specHeaderId.HasValue && specHeaderId.Value > 0)
                    {
                        val.Warnings.Add($"Mandatory condition '{cm.Name}' has no configured value in specification requirements.");
                    }
                }

                response.Conditions.Add(new PreviewConditionDto
                {
                    ConditionMasterID = cm.ID,
                    ConditionCode = !string.IsNullOrWhiteSpace(cm.Code) ? cm.Code : $"COND_{cm.ID}",
                    ConditionName = cm.Name,
                    Category = cm.Category ?? "",
                    ParameterUnit = unit,
                    IsMandatory = tc.IsMandatory,
                    ConfiguredValue = confVal,
                    HasConfiguration = hasConf,
                    Status = tc.IsMandatory ? "Required" : "Optional"
                });
            }

            if (!specHeaderId.HasValue || specHeaderId.Value <= 0)
            {
                val.RequiredConditionsPass = true;
                val.RequiredConditionsMessage = "N/A — Standardless test without specification conditions.";
            }
            else
            {
                val.RequiredConditionsPass = allMandatoryConditionsSatisfied;
                if (!allMandatoryConditionsSatisfied)
                {
                    val.RequiredConditionsMessage = "One or more mandatory test conditions do not have configured values.";
                    val.BlockingErrors.Add(val.RequiredConditionsMessage);
                }
            }

            response.IsConfigurationReady = val.AllPassed;
            return response;
        }

        public async Task<TestExecutionConfigSnapshotDto> ResolveSnapshotDtoAsync(UniversalTestGroup utg)
        {
            var previewReq = new UniversalPlanPreviewRequestDto
            {
                SampleID = utg.SampleTestPlan?.SampleID ?? 0,
                LaboratoryTestID = utg.LaboratoryTestID,
                SpecificationHeaderID = utg.SpecificationHeaderID,
                SpecificationVersionID = utg.SpecificationVersionID,
                SpecificationGradeID = utg.SpecificationGradeID,
                TestMethodSpecificationID = utg.TestMethodSpecificationID,
                TestMethodSpecificationVersionID = utg.TestMethodSpecificationVersionID,
                BranchID = utg.BranchID,
                ReferenceDate = utg.CreatedOn != default ? utg.CreatedOn : DateTime.UtcNow
            };

            var preview = await ResolveEffectiveConfigurationAsync(previewReq);

            var snapshot = new TestExecutionConfigSnapshotDto
            {
                LaboratoryTestID = preview.LaboratoryTestID,
                LaboratoryTestName = preview.LaboratoryTestName,
                LaboratoryTestCode = preview.LaboratoryTestCode,
                TestMethodSpecificationID = preview.TestMethodSpecificationID,
                TestMethodStandard = preview.TestMethodStandard,
                TestMethodName = preview.TestMethodName,
                TestMethodVersion = preview.TestMethodVersion,
                SpecificationHeaderID = preview.SpecificationHeaderID,
                SpecificationTitle = preview.SpecificationTitle,
                SpecificationGradeID = preview.SpecificationGradeID,
                GradeName = preview.GradeName,
                SnapshotDateUtc = DateTime.UtcNow,
                DefaultAggregateType = "Average",
                AggregateScope = "Observation"
            };

            int order = 1;
            foreach (var p in preview.Parameters)
            {
                var paramDto = new SnapshotParameterDto
                {
                    ParameterMasterID = p.ParameterID,
                    Code = p.ParameterCode,
                    Name = p.ParameterName,
                    Unit = p.ParameterUnit ?? "Unitless",
                    InputType = p.InputType ?? "Decimal",
                    DecimalPrecision = 2,
                    IsCalculated = !string.IsNullOrWhiteSpace(p.Equation),
                    Formula = p.Equation,
                    SpecMin = p.MinValue,
                    SpecMax = p.MaxValue,
                    AcceptanceCriteria = p.AcceptanceCriteria ?? "Within specification range",
                    DisplayOrder = order++,
                    IsRequired = p.IsMandatory,
                    AggregateType = "Average"
                };

                if (!string.IsNullOrWhiteSpace(paramDto.Formula))
                {
                    try
                    {
                        paramDto.FormulaDependencies = _formulaEvaluator.ExtractTokens(paramDto.Formula).ToList();
                    }
                    catch { }
                }

                snapshot.Parameters.Add(paramDto);
            }

            foreach (var c in preview.Conditions)
            {
                snapshot.Conditions.Add(new SnapshotConditionDto
                {
                    ConditionMasterID = c.ConditionMasterID,
                    DimensionName = c.ConditionName,
                    Unit = c.ParameterUnit ?? "",
                    ConfiguredValue1 = c.ConfiguredValue,
                    IsMandatory = c.IsMandatory
                });
            }

            // Equipment & Calibration
            var branchEquipment = await _context.EquipmentMasters
                .Include(e => e.Calibrations)
                .Where(e => e.IsActive && e.BranchID == utg.BranchID)
                .ToListAsync();

            var matchedEquipment = branchEquipment
                .Where(e => (!string.IsNullOrWhiteSpace(snapshot.LaboratoryTestName) && e.Name.Contains("Casagrande", StringComparison.OrdinalIgnoreCase))
                         || (preview.DepartmentID.HasValue && e.DepartmentID == preview.DepartmentID.Value)
                         || e.Name.Contains(snapshot.LaboratoryTestName, StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (!matchedEquipment.Any() && branchEquipment.Any())
            {
                matchedEquipment = branchEquipment.Take(2).ToList();
            }

            foreach (var eq in matchedEquipment)
            {
                var latestCal = eq.Calibrations.OrderByDescending(cal => cal.CalibrationDate).FirstOrDefault();
                snapshot.Equipment.Add(new SnapshotEquipmentDto
                {
                    EquipmentID = eq.ID,
                    Name = eq.Name,
                    Model = eq.ModelNo,
                    CalibrationNo = latestCal?.Certificate ?? eq.EquipmentNo,
                    CalibratedOn = latestCal?.CalibrationDate,
                    ValidUpto = latestCal?.CalibrationDueDate
                });
            }

            // Factors & Conversions
            snapshot.Factors.Add(new SnapshotFactorDto
            {
                FactorType = "Multiplication Factor",
                FactorName = "Standard Testing Factor",
                Value = 1.00m,
                AppliedOn = "All Results",
                Description = "Standard testing / dilution factor"
            });

            // Measurement Uncertainty (MU)
            var mu = await _context.NablMeasurementUncertainties
                .Where(u => u.IsActive && (u.TestParameter == snapshot.LaboratoryTestName || (u.TestMethod != null && u.TestMethod == snapshot.TestMethodStandard) || (u.TestParameter != null && u.TestParameter.Contains("Soil"))))
                .FirstOrDefaultAsync();

            snapshot.MeasurementUncertainty = new SnapshotMeasurementUncertaintyDto
            {
                UncertaintyType = mu?.UncertaintyType ?? "Expanded Uncertainty (k=2)",
                Value = mu?.ExpandedUncertainty ?? 2.50m,
                CoverageFactor = mu?.CoverageFactor ?? 2.0m,
                Unit = mu?.Unit ?? "%",
                Basis = "Type B",
                Remarks = "As per ISO 17025"
            };

            // Acceptance Criteria
            var decisionRule = utg.SampleTestPlan?.SampleDetail?.SampleInward?.DecisionRule;
            snapshot.AcceptanceCriteria = new SnapshotAcceptanceCriteriaDto
            {
                DecisionRule = !string.IsNullOrWhiteSpace(decisionRule) ? decisionRule : "All Parameters Must Pass",
                OverallDecision = "Pass if all parameters within spec range",
                RoundingRule = "Round to nearest",
                RoundingPrecision = 0.01m
            };

            // Execution Layout
            snapshot.RendererType = "ObservationMatrix";
            snapshot.SpecimenMode = "Single";
            snapshot.ObservationMode = "Multiple";
            snapshot.ConfiguredReadingCount = 1;

            return snapshot;
        }
    }
}
