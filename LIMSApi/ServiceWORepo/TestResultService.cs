using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Helpers.Enums;
using LIMSApi.Helpers.StatusFlow;
using LIMSApi.Models;
using LIMSApi.Services;
using LIMSApi.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using System.Linq.Dynamic.Core;

namespace LIMSApi.ServiceWORepo
{
    public class TestResultService : ITestResultService
    {
        private readonly LIMSContext _db;
        private readonly FormulaEvaluator _formulaEvaluator;
        private LoggedInUserDTO loggedInUser;
        private readonly IFileUploadService _fileUploadService;
        private readonly IWorkflowService _workflowService;
        private readonly ISampleStatusService _sampleStatusService;
        private readonly Services.Interface.IPriceCalculationService _priceCalculationService;
        private readonly ILogger<TestResultService> _logger;

        public TestResultService(LIMSContext db, FormulaEvaluator formulaEvaluator, IFileUploadService fileUploadService, IWorkflowService workflowService, ISampleStatusService sampleStatusService, Services.Interface.IPriceCalculationService priceCalculationService, ILogger<TestResultService> logger)
        {
            _db = db;
            _formulaEvaluator = formulaEvaluator;
            loggedInUser = LoggedInUserProvider.CurrentUser;
            _fileUploadService = fileUploadService;
            _workflowService = workflowService;
            _sampleStatusService = sampleStatusService;
            _priceCalculationService = priceCalculationService;
            _logger = logger;
        }


        public async Task<PagedResponse<object>> GetTestingDashboardList(PageFilter filter)
        {
            var allowedStatuses = StatusFilterHelper.GetAllowedStatuses(WorkflowListType.Testing);
            // ----------------------------------------------------------
            // BASE QUERY (Sample → TestResultHeaders → LaboratoryTests)
            // ----------------------------------------------------------
            var query =
                from sample in _db.SampleDetails
                join inward in _db.SampleInwards
                    on sample.InwardID equals inward.ID
                where sample.IsActive
                      && inward.CompanyCode == loggedInUser.CompanyCode && allowedStatuses.Contains(sample.SampleStatus)
                select new
                {
                    sample.ID,
                    sample.SampleNo,
                    inward.CaseNo,
                    sample.SampleStatus,
                    CustomerName = inward.Customer != null ? inward.Customer.Name : "",
                    Material = sample.MetalClassificationID != null ? _db.MetalClassificationMasters.Where(x => x.ID == sample.MetalClassificationID.Value).Select(m => m.Name).FirstOrDefault() : string.Empty,
                    Condition = sample.ProductConditionID != null ? _db.ProductConditionMasters.Where(x => x.ID == sample.ProductConditionID.Value).Select(m => m.Name).FirstOrDefault() : string.Empty,


                    // Sample-level status list
                    SampleLevelStatus = _db.TestResultHeaders
                        .Where(h => h.SampleID == sample.ID && h.IsActive)
                        .Select(h => h.Status)
                        .ToList(),

                    // All tests for this sample
                    Tests = _db.TestResultHeaders
                        .Where(h => h.SampleID == sample.ID && h.IsActive)
                        .Select(h => new
                        {
                            TestName = _db.LaboratoryTests
                                        .Where(t => t.ID == h.LaboratoryTestID)
                                        .Select(t => t.SubGroup)
                                        .FirstOrDefault(),
                        }).ToList(),

                };

            // ----------------------------------------------------------
            // APPLY DYNAMIC FILTERS
            // ----------------------------------------------------------
            query = query.AsQueryable().ApplyFilters(filter.Filter);

            // ----------------------------------------------------------
            // SEARCH
            // ----------------------------------------------------------
            if (!string.IsNullOrWhiteSpace(filter.searchTerm))
            {
                var search = filter.searchTerm.Trim().ToLower();

                query = query.Where(x =>
                    EF.Functions.Like(EF.Property<string>(x, "SampleNo") ?? "", $"%{search}%") ||
                    EF.Functions.Like(EF.Property<string>(x, "CustomerName") ?? "", $"%{search}%") ||
                    EF.Functions.Like(EF.Property<string>(x, "Material") ?? "", $"%{search}%") ||
                    EF.Functions.Like(EF.Property<string>(x, "TestName") ?? "", $"%{search}%") ||
                    x.Tests.Any(t => EF.Functions.Like(t.TestName.ToLower() ?? "", $"%{search}%")) ||
                    EF.Functions.Like(EF.Property<string>(x, "Condition") ?? "", $"%{search}%")
                );
            }

            // ----------------------------------------------------------
            // SORTING
            // ----------------------------------------------------------
            if (!string.IsNullOrWhiteSpace(filter.SortByColumn))
            {
                string order = filter.SortOrder == "asc" ? "ascending" : "descending";
                query = query.OrderBy($"{filter.SortByColumn} {order}");
            }

            // ----------------------------------------------------------
            // TOTAL COUNT
            // ----------------------------------------------------------
            int totalRecords = await query.CountAsync();

            // ----------------------------------------------------------
            // PAGINATION
            // ----------------------------------------------------------
            var data = await query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            // ----------------------------------------------------------
            // FINAL TRANSFORMATION FOR FRONTEND
            // (Calculate Sample-Level Status)
            // ----------------------------------------------------------
            var result = data.Select(x => new
            {
                x.ID,
                x.SampleNo,
                x.CaseNo,
                x.CustomerName,
                x.Material,
                x.Condition,

                // Combined Sample Status
                SampleStatus = CalculateSampleStatus(x.SampleLevelStatus),
                // 👇 Flow visibility
                CurrentStageStatus = x.SampleStatus,

                // 👇 Action control
                ActionStatus = ActionStatusResolver.Resolve(WorkflowListType.Testing,x.SampleStatus).ToString(),

                // Test list
                Tests = string.Join(", ", x.Tests.Where(x => x.TestName != null).Select(t => t.TestName).ToList())
            }).ToList<object>();

            return new PagedResponse<object>(
                result,
                totalRecords,
                filter.PageNumber,
                filter.PageSize
            );
        }
        public async Task SaveTestResult(TestResultSaveDto dto)
        {
            if (dto == null)
                throw new Exception("Dto can not be blank");

            await using var trx = await _db.Database.BeginTransactionAsync();
            try
            {
                await SaveGroup(dto.SampleId, dto.PlanId, dto.GeneralTests);
                await SaveGroup(dto.SampleId, dto.PlanId, dto.ChemicalTests);

                await _db.SaveChangesAsync();
                await trx.CommitAsync();
            }
            catch (Exception ex)
            {
                await trx.RollbackAsync();
                throw new Exception("Error saving test results: " + ex.Message);
            }
        }
        private async Task SaveGroup(long sampleId, long planId, List<TestResultGroupDto> groups)
        {
            if (groups == null) return;

            foreach (var g in groups)
            {
                TestResultHeader header;

                // Load existing header or create new one
                if (g.HeaderId > 0)
                {
                    header = await _db.TestResultHeaders
                        .Include(h => h.Parameters)
                        .FirstOrDefaultAsync(h => h.ID == g.HeaderId)
                        ?? throw new Exception($"Header not found: {g.HeaderId}");
                }
                else
                {
                    header = new TestResultHeader
                    {
                        SampleID = sampleId,
                        LaboratoryTestID = g.LaboratoryTestId,
                        TestPlanID = planId,
                        Status = "In-Progress"
                    };

                    _db.TestResultHeaders.Add(header);
                }

                // Save parameter values
                foreach (var p in g.Parameters)
                {
                    TestResultParameter param;

                    if (p.Id > 0)
                    {
                        param = header.Parameters.First(x => x.ID == p.Id);
                    }
                    else
                    {
                        param = new TestResultParameter
                        {
                            TestResultHeader = header,
                            ParameterID = p.ParameterID
                        };
                        header.Parameters.Add(param);
                    }

                    param.ParameterName = p.ParameterName;
                    param.Unit = p.Unit;
                    param.Remarks = p.Remarks;
                    param.MinValue = p.MinValue;
                    param.MaxValue = p.MaxValue;
                    if(header.Status == "Completed" && p.Value == 0)
                        throw new Exception("All parameters must have values greater than 0 after completion.");
                    param.Value = p.Value.HasValue ? Convert.ToDecimal(p.Value) : p.Value;

                    // Save formula expression if provided
                    if (!string.IsNullOrWhiteSpace(p.Formula))
                    {
                        param.FormulaExpression = p.Formula;
                    }
                }


                // 🚀 **Important: Run calculations**
                await RecalculateAllParameters(header.ID);
            }
        }
        private async Task RecalculateAllParameters(long headerId)
        {
            var header = await _db.TestResultHeaders
                .Include(h => h.Parameters)
                .FirstAsync(h => h.ID == headerId);

            // Build dictionary for formula vars
            Dictionary<string, double> vars = header.Parameters
                .Where(x => x.Value.HasValue && x.Value > 0)
                .ToDictionary(
                    x => $"P{x.ParameterID}",
                    x => (double)x.Value.Value
                );

            bool changed = true;
            int loopGuard = 0;

            // --------------------------------------------------------------------
            // CASCADE CALCULATION LOOP
            // keeps recalculating until no further formulas update
            // --------------------------------------------------------------------
            while (changed && loopGuard < 10)
            {
                changed = false;
                loopGuard++;

                foreach (var param in header.Parameters)
                {
                    // Use FormulaExpression first, fallback to Formula
                    string formulaToUse = !string.IsNullOrWhiteSpace(param.FormulaExpression)
                        ? param.FormulaExpression
                        : param.Formula;

                    if (!string.IsNullOrWhiteSpace(formulaToUse))
                    {
                        try
                        {
                            // Only evaluate when all required inputs exist
                            double? result = _formulaEvaluator.Evaluate(formulaToUse, vars);

                            if (result.HasValue)
                            {
                                decimal newValue = Math.Round((decimal)result.Value, 4);

                                if (param.Value != newValue)
                                {
                                    param.Value = newValue;
                                    param.IsCalculated = true;
                                    changed = true;

                                    // update dictionary for next dependencies
                                    vars[$"P{param.ParameterID}"] = (double)newValue;
                                }
                            }
                        }
                        catch
                        {
                            param.IsCalculated = false;
                        }
                    }

                    // ----------------------------------------------
                    // RANGE CHECK (use SpecMinValue/SpecMaxValue with fallback to MinValue/MaxValue)
                    // ----------------------------------------------
                    decimal? specMin = param.SpecMinValue ?? param.MinValue;
                    decimal? specMax = param.SpecMaxValue ?? param.MaxValue;

                    string resultStatus = _formulaEvaluator.DetermineResultStatus(param.Value, specMin, specMax);
                    if (resultStatus != null)
                    {
                        param.ResultStatus = resultStatus;
                        param.IsWithinLimit = resultStatus != "Fail";
                    }
                    else if (param.MinValue != null || param.MaxValue != null)
                    {
                        if (param.Value != null)
                        {
                            bool withinMin = param.MinValue == null || param.Value >= param.MinValue;
                            bool withinMax = param.MaxValue == null || param.Value <= param.MaxValue;
                            param.IsWithinLimit = withinMin && withinMax;
                            param.ResultStatus = param.IsWithinLimit == true ? "Pass" : "Fail";
                        }
                        else
                        {
                            param.IsWithinLimit = null;
                            param.ResultStatus = null;
                        }
                    }
                }
            }

            // --------------------------------------------------------------------
            // FINAL OVERALL PASS / FAIL
            // --------------------------------------------------------------------
            header.IsOverallPass = EvaluateOverallPass(header);
        }
        private bool EvaluateOverallPass(TestResultHeader header)
        {
            foreach (var p in header.Parameters)
            {
                // chemical fail
                if (p.IsWithinLimit == false)
                    return false;

                // mechanical fail condition (optional)
                if (!string.IsNullOrWhiteSpace(p.Formula) && p.Value == null)
                    return false;
            }

            return true;
        }

        // =====================================================================
        //  GET HEADER + PARAMETERS
        // =====================================================================
        public async Task<TestResultHeader> GetHeaderAsync(long headerId)
        {
            return await _db.TestResultHeaders
                .Include(h => h.Parameters)
                .FirstOrDefaultAsync(h => h.ID == headerId);
        }

        // =====================================================================
        //  UPDATE PARAMETER (Mechanical + Chemical)
        // =====================================================================
        public async Task<object> UpdateParameterAsync(long headerId, long paramId, TestResultParameterDto update)
        {
            var param = await _db.TestResultParameters
                .FirstOrDefaultAsync(x => x.ParameterID == paramId && x.TestResultHeaderID == headerId);

            if (param == null)
                throw new Exception("Parameter not found.");

            param.Value = update.Value != null ? Convert.ToDecimal(update.Value) : null;
            param.Remarks = update.Remarks;

            await RecalculateAllParameters(headerId);
            await _db.SaveChangesAsync();

            return new { Success = true };
        }

        // =====================================================================
        //  COMPLETE TEST RESULT (Evaluate Overall Pass/Fail)
        // =====================================================================
        public async Task CompleteHeaderAsync(long headerId, long userId)
        {
            var header = await _db.TestResultHeaders
                .Include(h => h.Parameters)
                .FirstOrDefaultAsync(h => h.ID == headerId);

            if (header == null)
                throw new Exception("Header not found.");

            // -----------------------------------------------------------------
            // 1. Check missing values
            // -----------------------------------------------------------------
            var missing = header.Parameters
                .Where(x =>
                    x.Value == null || x.Value == 0
                ).ToList();

            if (missing.Any())
                throw new Exception("All parameters must have values greater than 0 before completion.");

            // -----------------------------------------------------------------
            // 2. Calculate Overall Pass/Fail
            // -----------------------------------------------------------------
            bool allPass = true;

            foreach (var p in header.Parameters)
            {
                if (p.MinValue != null || p.MaxValue != null)
                {
                    if (p.IsWithinLimit == false)
                        allPass = false;
                }
            }

            header.IsOverallPass = allPass;
            header.Status = "Completed";

            await _db.SaveChangesAsync();
        }



       

        

        // inside TestResultService

        public async Task<object?> GetSampleDetailsForResult(long sampleId)
        {
            // load sample + testplans + minimal inward header
            var sample = await _db.SampleDetails
                .Where(s => s.ID == sampleId)
                .Include(s => s.SampleInward) // for parent inward info
                    .ThenInclude(c => c.Customer)
                .Include(s => s.AdditionalDetails)
                .Include(s => s.TestPlans)
                    .ThenInclude(tp => tp.GeneralTests)
                        .ThenInclude(gt => gt.Methods)
                .Include(s => s.TestPlans)
                    .ThenInclude(tp => tp.ChemicalTests)
                        .ThenInclude(ct => ct.Elements)
                .Include(s => s.TestPlans)
                    .ThenInclude(tp => tp.ChemicalTests)
                        .ThenInclude(ct => ct.TestTypes)
                .FirstOrDefaultAsync();

            if (sample == null) return null;

            var inward = sample.SampleInward;

            var resultPlans = new List<object>();

            foreach (var plan in sample.TestPlans)
            {
                // ---- GENERAL TESTS ----
                var generalTests = new List<object>();
                foreach (var gt in plan.GeneralTests)
                {
                    // every method may map to one laboratory test/method id
                    foreach (var method in gt.Methods)
                    {
                        var labTestId = method.LaboratoryTestID; // map to LaboratoryTest.ID in your model

                        // find header for this sample + labTest + plan
                        var header = await _db.TestResultHeaders
                            .Include(h => h.Parameters)
                            .Include(i => i.Images)
                            .FirstOrDefaultAsync(h =>
                                h.SampleID == sampleId &&
                                h.LaboratoryTestID == labTestId &&
                                h.TestPlanID == plan.ID);

                        if (header == null)
                        {
                            header = await AutoCreateHeaderForLabTestAsync(sampleId, plan.ID, labTestId);
                        }

                        header.CertificateNo = inward?.CaseNo;
                        generalTests.Add(new
                        {
                            headerId = header.ID,
                            generalTestId = gt.ID,
                            testMethodId = method.ID,
                            laboratoryTestId = labTestId,
                            laboratoryTest = method.LaboratoryTestID > 0 ? (await _db.LaboratoryTests.FindAsync(labTestId))?.Name : "General Test",
                            standard = method.StandardID,
                            reportNo = method.ReportNo,
                            specification1 = gt.Specification1,
                            specification2 = gt.Specification2,
                            specfication1Name = await GetSpecificationNameWithGrade(gt.Specification1),
                            specfication2Name = gt.Specification2 != null ? await GetSpecificationNameWithGrade(gt.Specification2.Value) : string.Empty,
                            testPlanID = plan.ID,
                            type = "General",
                            status = header.Status,

                            parameters = header.Parameters.Select(p => new
                            {
                                p.ID,
                                p.ParameterID,
                                p.ParameterName,
                                unit = p.Unit,
                                value = p.Value,
                                minValue = p.MinValue,
                                maxValue = p.MaxValue,
                                p.Remarks,
                                p.Formula,
                                p.IsCalculated,
                                p.SpecificationLineID
                            }).ToList(),

                            images = header.Images.Select(img => new
                            {
                                img.ID,
                                img.FilePath,
                                img.Caption,
                            }).ToList()
                        });
                    }
                }

                // ---- CHEMICAL TESTS ----
                var chemicalTests = new List<object>();
                foreach (var ct in plan.ChemicalTests)
                {
                    // ct.TestTypes is a list (or collection) of ChemicalTestType items
                    // We create header(si) for each selected LaboratoryTestID in ct.TestTypes where IsSelected=true
                    var typeLabIds = ct.TestTypes?.Where(tt => tt.IsSelected && tt.LaboratoryTestID.HasValue)
                                                  .Select(tt => tt.LaboratoryTestID!.Value)
                                                  .ToList();

                    // If no explicit selected types found, fallback to ct.TestMethod (single)
                    if (typeLabIds != null && typeLabIds.Any())
                    {
                        // create header for each labTestId
                        foreach (var labTestId in typeLabIds)
                        {
                            var header = await _db.TestResultHeaders
                                .Include(h => h.Parameters)
                                .Include(i => i.Images)
                                .FirstOrDefaultAsync(h =>
                                    h.SampleID == sampleId &&
                                    h.LaboratoryTestID == labTestId &&
                                    h.TestPlanID == plan.ID);

                            if (header == null)
                            {
                                header = await AutoCreateChemicalHeaderAsync(sampleId, plan.ID, ct, labTestId);
                            }
                            header.CertificateNo = inward?.CaseNo;
                            chemicalTests.Add(new
                            {
                                headerId = header.ID,
                                chemicalTestId = ct.ID,
                                labTestId = labTestId,
                                laboratoryTest = (await _db.LaboratoryTests.FindAsync(labTestId))?.Name ?? "Chemical Test",
                                specification1 = ct.Specification1,
                                specification2 = ct.Specification2,
                                specfication1Name = await GetSpecificationNameWithGrade(ct.Specification1),
                                specfication2Name = ct.Specification2 != null ? await GetSpecificationNameWithGrade(ct.Specification2.Value) : string.Empty,
                                reportNo = ct.ReportNo,
                                testPlanID = plan.ID,
                                type = "Chemical",
                                status = header.Status,
                                parameters = header.Parameters.Select(p => new
                                {
                                    p.ID,
                                    p.ParameterID,
                                    p.ParameterName,
                                    unit = p.Unit,
                                    value = p.Value,
                                    minValue = p.MinValue,
                                    maxValue = p.MaxValue,
                                    p.Remarks,
                                    p.Formula,
                                    p.IsCalculated,
                                    p.SpecificationLineID,
                                    p.IsAdditional,
                                    p.IsWithinLimit
                                }).ToList(),

                                images = header.Images.Select(img => new
                                {
                                    img.ID,
                                    img.FilePath,
                                    img.Caption,
                                }).ToList()
                            });
                        }
                    }
                }

                resultPlans.Add(new
                {
                    planId = plan.ID,
                    sampleNo = plan.SampleNo,
                    generalTests,
                    chemicalTests
                });
            }

            // top payload
            var payload = new
            {
                inward = new
                {
                    id = inward?.ID ?? 0,
                    caseNo = inward?.CaseNo,
                    customerID = inward?.CustomerID,
                    customerName = inward?.Customer?.Name
                },
                sample = new
                {
                    id = sample.ID,
                    sampleNo = sample.SampleNo,
                    details = sample.Details,
                    metalClassificationID = sample.MetalClassificationID,
                    metalClassification = (await _db.MetalClassificationMasters.FindAsync(sample.MetalClassificationID))?.Name,
                    productConditionID = sample.ProductConditionID,
                    productCondition = (await _db.ProductConditionMasters.FindAsync(sample.ProductConditionID))?.Name,
                    remarks = sample.Remarks,
                    additionalDetails = sample.AdditionalDetails.Select(ad => new { ad.Label, ad.Value })
                },
                plans = resultPlans
            };

            return payload;
        }

        /// <summary>
        /// Auto-create header for general (mechanical) test: use LaboratoryTest->Parameters mapping (ParameterMaster)
        /// </summary>
        private async Task<TestResultHeader> AutoCreateHeaderForLabTestAsync(long sampleId, long planId, long labTestId)
        {
            var header = new TestResultHeader
            {
                SampleID = sampleId,
                LaboratoryTestID = labTestId,
                TestPlanID = planId,
                CreatedBy = loggedInUser.EmployeeID,
                CreatedOn = DateTime.UtcNow,
                Status = "Pending"
            };

            var labTest = await _db.LaboratoryTests
                .Include(t => t.Parameters)
                    .ThenInclude(tp => tp.Parameter)
                        .ThenInclude(u => u.ParameterUnit)
                .FirstOrDefaultAsync(t => t.ID == labTestId);

            if (labTest != null)
            {
                foreach (var tp in labTest.Parameters)
                {
                    var pm = tp.Parameter;
                    if (pm == null) continue;

                    header.Parameters.Add(new TestResultParameter
                    {
                        ParameterID = pm.ID,
                        ParameterName = pm.Name,
                        Unit = pm.ParameterUnit?.Name,
                        Value = null,
                        IsAdditional = false,
                        Formula = pm.Formula,
                        IsCalculated = pm.IsCalculated,
                    });
                }
            }

            _db.TestResultHeaders.Add(header);
            await _db.SaveChangesAsync();

            return header;
        }

        /// <summary>
        /// Auto-create header for chemical test: create parameters from ChemicalTest.Elements
        /// and set MinValue/MaxValue from element data. header is linked to a specific laboratory test (labTestId)
        /// </summary>
        private async Task<TestResultHeader> AutoCreateChemicalHeaderAsync(long sampleId, long planId, ChemicalTest ct, long labTestId)
        {
            var header = new TestResultHeader
            {
                SampleID = sampleId,
                LaboratoryTestID = labTestId,
                TestPlanID = planId,
                CreatedBy = 0,
                CreatedOn = DateTime.UtcNow,
                Status = "Pending"
            };

            // Elements are specification lines with ParameterID and Min/Max values
            foreach (var el in ct.Elements)
            {
                // Try to load parameter master to get name/unit (if exists)
                ParameterMaster pm = null;
                if (el.ParameterID > 0)
                    pm = await _db.ParameterMasters.FindAsync(el.ParameterID);

                header.Parameters.Add(new TestResultParameter
                {
                    ParameterID = el.ParameterID,
                    ParameterName = pm?.Name ?? el.ParameterUnit /* fallback to whatever available */,
                    Unit = pm?.ParameterUnit?.Name ?? el.ParameterUnit,
                    Value = null,
                    Formula = null,
                    IsCalculated = false,
                    IsAdditional = false,
                    MinValue = el.MinValue,
                    MaxValue = el.MaxValue,
                    SpecificationLineID = el.SpecificationLineID
                });
            }

            _db.TestResultHeaders.Add(header);
            await _db.SaveChangesAsync();

            return header;
        }

        /// <summary>
        /// Auto-create TestResultHeaders from an approved plan (ISO 17025 compliant).
        /// Mechanical: parameters from LaboratoryTestParameter + min/max from SpecificationLine.
        /// Chemical: parameters from ChemicalTestElement (already specification-driven).
        /// Each unit of Quantity = 1 independent TestResultHeader.
        /// </summary>
        public async Task<AutoCreateHeadersResponse> AutoCreateHeadersFromPlanAsync(long planId)
        {
            var response = new AutoCreateHeadersResponse();

            var plan = await _db.TestPlans
                .Include(p => p.GeneralTests)
                    .ThenInclude(gt => gt.Methods)
                .Include(p => p.ChemicalTests)
                    .ThenInclude(ct => ct.Elements)
                .FirstOrDefaultAsync(p => p.ID == planId);

            if (plan == null)
            {
                response.Success = false;
                response.Warnings.Add($"Plan {planId} not found.");
                return response;
            }

            var sampleId = plan.SampleID;

            // ── General (Mechanical) Tests ──
            foreach (var generalTest in plan.GeneralTests)
            {
                // Collect specification IDs for this general test
                var specIds = new List<long>();
                if (generalTest.Specification1 > 0) specIds.Add(generalTest.Specification1);
                if (generalTest.Specification2.HasValue && generalTest.Specification2.Value > 0)
                    specIds.Add(generalTest.Specification2.Value);

                foreach (var method in generalTest.Methods)
                {
                    if (method.LaboratoryTestID <= 0) continue;

                    var quantity = method.Quantity > 0 ? method.Quantity : 1;

                    // Count existing headers to avoid duplicates
                    var existingCount = await _db.TestResultHeaders.CountAsync(h =>
                        h.SampleID == sampleId &&
                        h.LaboratoryTestID == method.LaboratoryTestID &&
                        h.TestPlanID == planId);

                    var headersToCreate = quantity - existingCount;
                    if (headersToCreate <= 0) continue;

                    // Build parameters using ISO 17025 hybrid approach
                    var parameters = await BuildMechanicalParametersAsync(method.LaboratoryTestID, specIds, response.Warnings);

                    for (int i = 0; i < headersToCreate; i++)
                    {
                        var header = new TestResultHeader
                        {
                            SampleID = sampleId,
                            LaboratoryTestID = method.LaboratoryTestID,
                            TestPlanID = planId,
                            CreatedBy = loggedInUser.EmployeeID,
                            CreatedOn = DateTime.UtcNow,
                            Status = "Pending"
                        };

                        // Clone parameters for each header
                        foreach (var p in parameters)
                        {
                            header.Parameters.Add(new TestResultParameter
                            {
                                ParameterID = p.ParameterID,
                                ParameterName = p.ParameterName,
                                Unit = p.Unit,
                                Value = null,
                                Formula = p.Formula,
                                IsCalculated = p.IsCalculated,
                                IsAdditional = false,
                                MinValue = p.MinValue,
                                MaxValue = p.MaxValue,
                                SpecificationLineID = p.SpecificationLineID
                            });
                        }

                        _db.TestResultHeaders.Add(header);
                        response.HeadersCreated++;
                    }
                }
            }

            // ── Chemical Tests ──
            foreach (var chemTest in plan.ChemicalTests)
            {
                // Chemical tests use ChemicalTestType for lab test IDs
                var labTestIds = await _db.Set<ChemicalTestType>()
                    .Where(tt => tt.ChemicalTestID == chemTest.ID && tt.IsSelected)
                    .Select(tt => tt.LaboratoryTestID ?? 0)
                    .Where(id => id > 0)
                    .ToListAsync();

                foreach (var labTestId in labTestIds)
                {
                    var exists = await _db.TestResultHeaders.AnyAsync(h =>
                        h.SampleID == sampleId &&
                        h.LaboratoryTestID == labTestId &&
                        h.TestPlanID == planId);

                    if (exists) continue;

                    await AutoCreateChemicalHeaderAsync(sampleId, planId, chemTest, labTestId);
                    response.HeadersCreated++;
                }
            }

            // Check specimen quantity (Gap 6 warning)
            await AddQuantityWarnings(plan, sampleId, response.Warnings);

            await _db.SaveChangesAsync();
            return response;
        }

        /// <summary>
        /// Build mechanical test parameters using ISO 17025 hybrid approach:
        /// 1. Start with LaboratoryTestParameter (test method definition)
        /// 2. Enrich with min/max from SpecificationLine (Type="mechanical")
        /// 3. Warn on mismatches in both directions
        /// </summary>
        private async Task<List<TestResultParameter>> BuildMechanicalParametersAsync(
            long labTestId, List<long> specIds, List<string> warnings)
        {
            var result = new List<TestResultParameter>();

            // 1. Get parameters from test method definition
            var labTest = await _db.LaboratoryTests
                .Include(t => t.Parameters)
                    .ThenInclude(tp => tp.Parameter)
                        .ThenInclude(p => p.ParameterUnit)
                .FirstOrDefaultAsync(t => t.ID == labTestId);

            var testMethodParams = labTest?.Parameters?.ToList() ?? new List<LaboratoryTestParameter>();
            var labTestName = labTest?.Name ?? $"LabTest#{labTestId}";

            // 2. Get matching specification lines (Type="mechanical") linked to this lab test
            var specLines = new List<SpecificationLine>();
            if (specIds.Any())
            {
                // Get spec line IDs linked to this lab test
                var linkedSpecLineIds = await _db.SpecificationLineLaboratoryTests
                    .Where(slt => slt.LaboratoryTestID == labTestId)
                    .Select(slt => slt.SpecificationLineID)
                    .ToListAsync();

                specLines = await _db.SpecificationLines
                    .Include(sl => sl.Parameter)
                        .ThenInclude(p => p.ParameterUnit)
                    .Where(sl => linkedSpecLineIds.Contains(sl.ID)
                        && sl.Type == "mechanical"
                        && sl.ParameterID.HasValue
                        && sl.SpecificationGrade != null
                        && specIds.Contains(sl.SpecificationGrade.ID))
                    .ToListAsync();

                // Alternative: if the above doesn't work due to SpecificationGrade navigation,
                // try matching via SpecificationGradeID
                if (!specLines.Any())
                {
                    specLines = await _db.SpecificationLines
                        .Include(sl => sl.Parameter)
                            .ThenInclude(p => p.ParameterUnit)
                        .Where(sl => linkedSpecLineIds.Contains(sl.ID)
                            && sl.Type == "mechanical"
                            && sl.ParameterID.HasValue)
                        .ToListAsync();

                    // Filter by specification grade IDs matching our specIds
                    specLines = specLines.Where(sl => sl.SpecificationGradeID.HasValue && specIds.Contains(sl.SpecificationGradeID.Value)).ToList();
                }
            }

            // Build a lookup: ParameterID -> SpecificationLine
            var specLookup = specLines
                .Where(sl => sl.ParameterID.HasValue)
                .GroupBy(sl => sl.ParameterID!.Value)
                .ToDictionary(g => g.Key, g => g.First());

            // Track which spec line parameters have been used
            var usedSpecParamIds = new HashSet<long>();

            // 3. For each test method parameter, create TestResultParameter with spec enrichment
            foreach (var tp in testMethodParams)
            {
                var pm = tp.Parameter;
                if (pm == null) continue;

                var trp = new TestResultParameter
                {
                    ParameterID = pm.ID,
                    ParameterName = pm.Name,
                    Unit = pm.ParameterUnit?.Name,
                    Formula = pm.Formula,
                    IsCalculated = pm.IsCalculated,
                    Value = null,
                    IsAdditional = false
                };

                // Enrich with specification min/max if available
                if (specLookup.TryGetValue(pm.ID, out var matchingSpec))
                {
                    trp.MinValue = matchingSpec.MinValue;
                    trp.MaxValue = matchingSpec.MaxValue;
                    trp.SpecificationLineID = matchingSpec.ID;
                    usedSpecParamIds.Add(pm.ID);
                }
                // else: parameter stays with null min/max (For Information Only)

                result.Add(trp);
            }

            // 4. Reverse check: spec parameters NOT in test method → include + warn
            foreach (var specLine in specLines.Where(sl => sl.ParameterID.HasValue))
            {
                var paramId = specLine.ParameterID!.Value;
                if (usedSpecParamIds.Contains(paramId)) continue;

                var pm = specLine.Parameter;
                var paramName = pm?.Name ?? $"Parameter#{paramId}";

                warnings.Add($"Specification requires '{paramName}' but test method '{labTestName}' doesn't include it. Adding from specification.");

                result.Add(new TestResultParameter
                {
                    ParameterID = paramId,
                    ParameterName = paramName,
                    Unit = pm?.ParameterUnit?.Name,
                    Formula = null,
                    IsCalculated = false,
                    Value = null,
                    IsAdditional = false,
                    MinValue = specLine.MinValue,
                    MaxValue = specLine.MaxValue,
                    SpecificationLineID = specLine.ID
                });
            }

            // 5. Warn if no parameters at all
            if (!result.Any())
            {
                warnings.Add($"No parameters defined for '{labTestName}'. Verify test method and specification setup.");
            }
            else if (!specLines.Any() && specIds.Any())
            {
                warnings.Add($"Specification does not define mechanical parameters for '{labTestName}'. Parameters created without acceptance criteria (min/max).");
            }

            return result;
        }

        /// <summary>
        /// Gap 6: Check planned quantity vs available specimens and add warnings
        /// </summary>
        private async Task AddQuantityWarnings(SampleTestPlan plan, long sampleId, List<string> warnings)
        {
            var cuttingSample = await _db.CuttingChargeSamples
                .Include(cs => cs.CuttingChargeDetails)
                .FirstOrDefaultAsync(cs => cs.SampleID == sampleId);

            if (cuttingSample == null) return; // No preparation yet — skip

            var availableSpecimens = cuttingSample.CuttingChargeDetails?.Sum(d => d.Quantity) ?? 0;

            var totalPlannedQuantity = plan.GeneralTests
                .SelectMany(gt => gt.Methods)
                .Sum(m => m.Quantity);

            if (totalPlannedQuantity > availableSpecimens && availableSpecimens > 0)
            {
                warnings.Add($"Planned quantity ({totalPlannedQuantity}) exceeds available specimens ({availableSpecimens}). Verify specimen preparation.");
            }
        }

        private async Task<string> GetSpecificationNameWithGrade(long gradeId)
        {
            var grade = await (from g in _db.SpecificationGrades
                               join h in _db.SpecificationHeaders
                                   on g.SpecificationHeaderID equals h.ID
                               join tc in _db.TestMethodSpecifications on g.TestMethodSpecificationID equals tc.ID into tcGroup
                               from tc in tcGroup.DefaultIfEmpty()
                               where g.ID == gradeId
                               select new
                               {
                                   Grade = h.AliasName + "-" + g.Grade + (tc != null ? ("-" + tc.Name) : ""),
                               })
                    .FirstOrDefaultAsync();

            return grade?.Grade ?? string.Empty;
        }

        private string CalculateSampleStatus(List<string> statuses)
        {
            if (statuses == null || statuses.Count == 0)
                return "Pending";

            // If any test is running, sample is in progress
            if (statuses.Any(s =>
                s == "Started" ||
                s == "In Progress"))
                return "In Progress";

            // If ALL tests are completed
            if (statuses.All(s => s == "Completed"))
                return "Completed";

            // Default fallback
            return "Pending";
        }


        public async Task<StartTestResponse> StartTest(long Id)
        {
            var header = await _db.TestResultHeaders.FindAsync(Id);
            if (header == null)
                throw new Exception("Test Result Header not found.");

            var response = new StartTestResponse { Success = true };

            // Check preparation status — warn but don't block
            var sample = await _db.SampleDetails.FindAsync(header.SampleID);
            if (sample != null && sample.PreparationRequired)
            {
                var cuttingSample = await _db.CuttingChargeSamples
                    .FirstOrDefaultAsync(cs => cs.SampleID == header.SampleID);

                if (cuttingSample == null ||
                    (cuttingSample.PreparationStatus != "Completed" && cuttingSample.PreparationStatus != "QCVerified"))
                {
                    response.PreparationWarning = true;
                    response.PreparationStatus = cuttingSample?.PreparationStatus ?? "No preparation record";
                    response.WarningMessage = "Sample preparation data is not yet entered in the system. You can add preparation details later.";
                }
            }

            // Allow test to start regardless of preparation status
            header.Status = "Started";
            header.StartedAt = DateTime.UtcNow;
            header.StartedBy = loggedInUser.EmployeeID;

            await _db.SaveChangesAsync();

            // Return timing info
            response.TestStartTime = header.StartedAt;
            var employee = await _db.EmployeeMasters.FindAsync(loggedInUser.EmployeeID);
            response.PerformedByName = employee?.Name ?? "";

            return response;
        }

        public async Task<CompleteTestResponse> CompleteTest(long Id)
        {
            var response = new CompleteTestResponse();
            // begin transaction
            var trx = await _db.Database.BeginTransactionAsync();
            try
            {


                var header = await _db.TestResultHeaders.FindAsync(Id);
                if (header == null)
                    throw new Exception("Test Result Header not found.");

                // --- Preparation Validation (Phase 5) ---
                var sampleForPrep = await _db.SampleDetails.FindAsync(header.SampleID);
                if (sampleForPrep != null && sampleForPrep.PreparationRequired)
                {
                    var prepExists = await _db.CuttingChargeSamples
                        .AnyAsync(cs => cs.SampleID == header.SampleID);
                    header.PreparationDataMissing = !prepExists;
                }

                // --- Determine final status: Verification or Completed ---
                bool verificationWorkflowExists = await _db.Workflows
                    .AnyAsync(w => w.EntityType == "TestResult" && w.IsActive);

                if (verificationWorkflowExists)
                {
                    header.Status = "PendingVerification";
                }
                else
                {
                    header.Status = "Completed";
                }

                header.CompletedAt = DateTime.UtcNow;
                header.ModifiedBy = loggedInUser.EmployeeID;

                // ---------------------------------
                // Long-Term Test Status Resolution
                // ---------------------------------
                var longTermTests = await _db.LongTermTests
                    .Where(x => x.TestResultHeaderID == header.ID)
                    .ToListAsync();

                if (longTermTests.Any())
                {
                    foreach (var ltt in longTermTests)
                    {
                        if (!header.StartedAt.HasValue)
                            continue;

                        var elapsedHours =
                            (DateTime.UtcNow - header.StartedAt.Value).TotalHours;

                        if (elapsedHours >= ltt.DurationHours)
                        {
                            // Normal completion
                            ltt.Status = "Completed";
                            ltt.EndedAt = DateTime.UtcNow;
                        }
                        else
                        {
                            // Early termination (LIMS standard)
                            ltt.Status = "Force Completed";
                            ltt.EndedAt = DateTime.UtcNow;
                        }
                    }
                }

                await _db.SaveChangesAsync();

                var completedStatuses = new[] { "Completed", "PendingVerification", "Verified" };
                bool hasPendingTests = await _db.TestResultHeaders.AnyAsync(h =>
                h.SampleID == header.SampleID &&
                h.IsActive &&
                !completedStatuses.Contains(h.Status));

                // -----------------------------
                // 🔹 LAST TEST COMPLETED | No Pending Test
                // -----------------------------

                if (!hasPendingTests)
                {
                    // 3️⃣ Mark Sample as testing-completed
                    var sample = await _db.SampleDetails.FindAsync(header.SampleID);
                    if (sample == null)
                        throw new Exception("Sample not found.");

                    sample.IsTestingCompleted = true;
                    sample.TestingCompletedOn = DateTime.UtcNow;

                    await _db.SaveChangesAsync();

                    // Use verification status if workflow exists, otherwise completed
                    var sampleStatus = verificationWorkflowExists
                        ? SampleStatus.TESTING_UNDER_VERIFICATION
                        : SampleStatus.TESTING_COMPLETED;

                    await _sampleStatusService.ForceAutoStatusAsync(
                                sample.ID,
                                sampleStatus,
                                loggedInUser.EmployeeID
                            );

                    // 4️⃣ Auto-create Report Header (if not exists)
                    var existingReport = await _db.ReportHeaders
                        .FirstOrDefaultAsync(r => r.SampleID == sample.ID && r.IsActive);

                    if (existingReport == null)
                    {
                        var report = new ReportHeader
                        {
                            SampleID = sample.ID,
                            CertificateNo = header.CertificateNo,
                            Status = "Pending",
                            CreatedOn = DateTime.UtcNow,
                            CreatedBy = loggedInUser.EmployeeID,
                            IsActive = true
                        };

                        _db.ReportHeaders.Add(report);
                        await _db.SaveChangesAsync();

                        await _workflowService.StartWorkflow(report.ID,WorkFlowEntityTypeExtensions.GetEntityType(WorkFlowEntityType.Report_Review));
                    }

                    // 5️⃣ Check inward-level testing status (informational only - price calculation happens after approval)
                    await CheckAndUpdateInwardTestingStatusAsync(sample.InwardID);

                }
                await trx.CommitAsync();

                // Return timing info
                response.TestEndTime = header.CompletedAt;
                var employee = await _db.EmployeeMasters.FindAsync(loggedInUser.EmployeeID);
                response.PerformedByName = employee?.Name ?? "";
            }
            catch (Exception)
            {
                await trx.RollbackAsync();
                throw;
            }
            return response;
        }

        /// <summary>
        /// Checks inward-level testing status (informational only)
        /// Note: Price calculation is triggered separately after report approval, not during testing completion
        /// </summary>
        private async Task CheckAndUpdateInwardTestingStatusAsync(long inwardId)
        {
            // Get all samples for this inward
            var samples = await _db.SampleDetails
                .Where(s => s.InwardID == inwardId && s.IsActive)
                .ToListAsync();

            if (!samples.Any())
                return;

            // Count testing status (informational tracking only)
            var totalSamples = samples.Count;
            var completedSamples = samples.Count(s => s.IsTestingCompleted);
            
            // Determine inward testing status (informational only)
            bool isTestingPartial = completedSamples > 0 && completedSamples < totalSamples;
            bool isTestingCompleted = completedSamples == totalSamples;

            // Log status for informational purposes
            if (isTestingPartial)
            {
                _logger.LogInformation("Inward {InwardID} has partial testing status: {Completed}/{Total} samples completed", 
                    inwardId, completedSamples, totalSamples);
            }
            else if (isTestingCompleted)
            {
                _logger.LogInformation("Inward {InwardID} has completed testing: {Completed}/{Total} samples completed", 
                    inwardId, completedSamples, totalSamples);
            }

            // Note: Price calculation will be triggered after report approval (FINAL_REPORT_APPROVED)
            // This method only tracks status, does NOT trigger price calculation
        }

        public async Task MoveToLongTerm(MoveToLongTermDto dto)
        {
            if (dto == null || dto.HeaderId <= 0)
                throw new Exception("Invalid input data");

            var header = await _db.TestResultHeaders.FindAsync(dto.HeaderId);
            if (header == null) throw new KeyNotFoundException($"HeaderId {dto.HeaderId} not found");

            var lt = new LongTermTest
            {
                TestResultHeaderID = header.ID,
                SampleID = header.SampleID,
                DurationHours = dto.DurationHours,
                StartedAt = DateTime.UtcNow,
                Status = "Running",
                CreatedBy = loggedInUser.EmployeeID,
                CreatedOn = DateTime.UtcNow
            };

            _db.LongTermTests.Add(lt);

            // mark header status appropriately
            header.Status = "Long-Term";
            _db.TestResultHeaders.Update(header);

            await _db.SaveChangesAsync();

        }

        public async Task<PagedResponse<object>> GetLongTermList(PageFilter filter)
        {
            var companyCode = loggedInUser.CompanyCode;

            // --------------------------------------------------------
            // Base Query: LongTermTests + Sample + Laboratory Test
            // --------------------------------------------------------
            var query =
                from lt in _db.LongTermTests
                join header in _db.TestResultHeaders
                    on lt.TestResultHeaderID equals header.ID
                join sample in _db.SampleDetails
                    on header.SampleID equals sample.ID
                join inward in _db.SampleInwards
                    on sample.InwardID equals inward.ID
                join labTest in _db.LaboratoryTests
                    on header.LaboratoryTestID equals labTest.ID
                where lt.IsActive && inward.CompanyCode == companyCode
                select new
                {
                    lt.ID,
                    headerId = lt.TestResultHeaderID,
                    header.SampleID,
                    sample.SampleNo,
                    TestName = labTest.SubGroup,
                    lt.DurationHours,
                    lt.StartedAt,
                    lt.EndedAt,
                    lt.Status,
                    CaseNo = inward.CaseNo,
                    Readings = _db.LongTermRecords.Where(x => x.LongTermTestID == lt.ID).ToList(),
                    Customer = inward.Customer != null ? inward.Customer.Name : string.Empty,
                    Material = sample.MetalClassificationID != null
                        ? _db.MetalClassificationMasters.Where(m => m.ID == sample.MetalClassificationID).Select(x => x.Name).FirstOrDefault()
                        : "",
                };

            // --------------------------------------------------------
            // Apply dynamic filters
            // --------------------------------------------------------
            query = query.AsQueryable().ApplyFilters(filter.Filter);

            // --------------------------------------------------------
            // Search
            // --------------------------------------------------------
            if (!string.IsNullOrWhiteSpace(filter.searchTerm))
            {
                var search = filter.searchTerm.Trim().ToLower();

                query = query.Where(x =>
                    EF.Functions.Like(EF.Property<string>(x, "SampleNo") ?? "", $"%{search}%") ||
                    EF.Functions.Like(EF.Property<string>(x, "CaseNo") ?? "", $"%{search}%") ||
                    EF.Functions.Like(EF.Property<string>(x, "Customer") ?? "", $"%{search}%") ||
                    EF.Functions.Like(EF.Property<string>(x, "SubGroup") ?? "", $"%{search}%")
                );
            }

            // --------------------------------------------------------
            // Sorting
            // --------------------------------------------------------
            if (!string.IsNullOrWhiteSpace(filter.SortByColumn))
            {
                string order = filter.SortOrder == "asc" ? "ascending" : "descending";
                query = query.OrderBy($"{filter.SortByColumn} {order}");
            }

            // --------------------------------------------------------
            // Pagination
            // --------------------------------------------------------
            int totalRecords = await query.CountAsync();

            var items = await query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync<object>();

            return new PagedResponse<object>(
                items,
                totalRecords,
                filter.PageNumber,
                filter.PageSize
            );
        }

        public async Task<object?> GetLongTermDetail(long longTermTestId)
        {
            var companyCode = loggedInUser.CompanyCode;

            // Load Long-Term Test + Header + Sample + Inward + LaboratoryTest
            var lt = await _db.LongTermTests
                .Include(x => x.TestResultHeader)
                    .ThenInclude(h => h.Parameters)
                .Include(x => x.Sample)
                .Include(x => x.Records)
                .FirstOrDefaultAsync(x =>
                    x.ID == longTermTestId &&
                    x.IsActive
                );

            if (lt == null) return null;

            // Fetch sample, test name, customer etc.
            var sample = await _db.SampleDetails
                .Include(s => s.SampleInward)
                    .ThenInclude(i => i.Customer)
                .FirstOrDefaultAsync(s => s.ID == lt.SampleID);

            var labTest = await _db.LaboratoryTests
                .FirstOrDefaultAsync(l => l.ID == lt.TestResultHeader.LaboratoryTestID);

            // Build parameter dropdown list (for recording new readings)
            var parameterList = lt.TestResultHeader.Parameters
                .Select(p => new
                {
                    p.ParameterID,
                    p.ParameterName,
                    p.Unit
                }).ToList();

            // Readings list
            var readings = lt.Records
                .OrderByDescending(r => r.RecordedAt)
                .Select(r => new
                {
                    r.ID,
                    r.RecordedAt,
                    r.DataJson,
                    Parsed = SafeParseLongTermJson(r.DataJson)
                }).ToList();

            // Final payload for UI
            var result = new
            {
                longTermTestId = lt.ID,
                sampleId = lt.SampleID,
                sampleNo = sample?.SampleNo,
                caseNo = sample?.SampleInward?.CaseNo,
                customer = sample?.SampleInward?.Customer?.Name,
                material = sample?.MetalClassificationID != null
                    ? _db.MetalClassificationMasters
                        .Where(m => m.ID == sample.MetalClassificationID)
                        .Select(m => m.Name)
                        .FirstOrDefault()
                    : null,

                testName = labTest?.SubGroup,

                durationHours = lt.DurationHours,
                startedAt = lt.StartedAt,
                endedAt = lt.EndedAt,
                status = lt.Status,

                parameters = parameterList,
                readings
            };

            return result;
        }

        private object SafeParseLongTermJson(string json)
        {
            try
            {
                return System.Text.Json.JsonSerializer.Deserialize<object>(json);
            }
            catch
            {
                return new { error = "Invalid JSON", raw = json };
            }
        }

        public async Task<object?> GetParametersForHeader(long headerId)
        {
            var header = await _db.TestResultHeaders
                .Include(h => h.Parameters)
                .FirstOrDefaultAsync(h => h.ID == headerId);

            if (header == null)
                return null;

            var parameters = header.Parameters.Select(p => new
            {
                p.ParameterID,
                p.ParameterName,
                p.Unit
            }).ToList();

            return parameters;
        }

        public async Task RecordLongTermReading(LongTermRecordDto dto)
        {
            if (dto.LongTermTestId <= 0)
                throw new Exception("Invalid long-term test ID.");

            var lt = await _db.LongTermTests
                .FirstOrDefaultAsync(x => x.ID == dto.LongTermTestId && x.IsActive);

            if (lt == null)
                throw new Exception("Long-term test not found.");

            // Convert to DataJson automatically
            var jsonObj = new
            {
                ParameterId = dto.ParameterId,
                ParameterName = (await _db.ParameterMasters.FindAsync(dto.ParameterId))?.Name,
                Value = dto.Value,
                Remarks = dto.Remarks,
                RecordedAt = DateTime.UtcNow
            };

            var record = new LongTermRecord
            {
                LongTermTestID = dto.LongTermTestId,
                DataJson = System.Text.Json.JsonSerializer.Serialize(jsonObj),
                RecordedAt = DateTime.UtcNow,
                CreatedOn = DateTime.UtcNow,
                CreatedBy = loggedInUser.EmployeeID,
                IsActive = true
            };

            _db.LongTermRecords.Add(record);
            await _db.SaveChangesAsync();
        }
        public async Task<string> UploadTestImageAsync(long headerId, IFormFile file, string? caption)
        {
            var header = await _db.TestResultHeaders
                .Include(h => h.Images)
                .FirstOrDefaultAsync(h => h.ID == headerId);

            if (header == null)
                throw new Exception("TestResultHeader not found");

            // Create image entry
            var resultImage = new TestResultImage
            {
                TestResultHeaderID = headerId,
                FilePath = "",
                Caption = caption,
                SortOrder = header.Images.Any() ? header.Images.Max(x => x.SortOrder) + 1 : 1
            };
            var imagePath = string.Empty;
            if (file != null)
            {
                var fileUploadResponse = await _fileUploadService.UploadFileAsync(file, FileType.Test, null, "Test-Results");
                if (fileUploadResponse == null)
                    throw new InvalidOperationException("File upload failed!");
                resultImage.FilePath = fileUploadResponse.FilePath;
                resultImage.FileName = fileUploadResponse.OriginalFileName;
                resultImage.UploadReferenceID = fileUploadResponse.ID;
                imagePath = fileUploadResponse.FilePath;
            }




            _db.TestResultImages.Add(resultImage);
            await _db.SaveChangesAsync();

            return imagePath;
        }

        public async Task<List<TestResultImageDto>> UploadTestImagesAsync(long headerId, List<IFormFile> files, List<string>? captions)
        {
            var header = await _db.TestResultHeaders
                .Include(h => h.Images)
                .FirstOrDefaultAsync(h => h.ID == headerId);

            if (header == null)
                throw new Exception("TestResultHeader not found");

            if (files == null || !files.Any())
                throw new Exception("No files provided");

            int sortOrder = header.Images.Any()
                ? header.Images.Max(x => x.SortOrder) + 1
                : 1;

            var uploadedImages = new List<TestResultImageDto>();

            for (int i = 0; i < files.Count; i++)
            {
                var file = files[i];
                var caption = captions != null && captions.Count > i
                    ? captions[i]
                    : null;

                var uploadResponse = await _fileUploadService.UploadFileAsync(
                    file,
                    FileType.Test,
                    null,
                    "Test-Results"
                );

                if (uploadResponse == null)
                    throw new InvalidOperationException("File upload failed!");

                var imageEntity = new TestResultImage
                {
                    TestResultHeaderID = headerId,
                    FilePath = uploadResponse.FilePath,
                    FileName = uploadResponse.OriginalFileName,
                    UploadReferenceID = uploadResponse.ID,
                    Caption = caption,
                    SortOrder = sortOrder++,
                    CreatedBy = loggedInUser.EmployeeID,
                    CreatedOn = DateTime.UtcNow
                };

                _db.TestResultImages.Add(imageEntity);

                uploadedImages.Add(new TestResultImageDto
                {
                    Id = imageEntity.ID,
                    FilePath = imageEntity.FilePath,
                    FileName = imageEntity.FileName,
                    Caption = caption
                });
            }

            await _db.SaveChangesAsync();
            return uploadedImages;
        }

        public async Task<List<TestResultImageDto>> UploadedTestImages(long headerId)
        {
            var header = await _db.TestResultHeaders
                 .Include(h => h.Images)
                 .FirstOrDefaultAsync(h => h.ID == headerId);
            if (header == null)
                throw new Exception("TestResultHeader not found");

            var images = header.Images.Select(x =>
            new TestResultImageDto
            {
                Id = x.ID,
                FileName = x.FileName,
                FilePath = x.FilePath,
                Caption = x.Caption
            }).ToList();

            return images;
        }

        // =====================================================================
        //  CALCULATE PARAMETERS (Trigger recalculation for all derived params)
        // =====================================================================
        public async Task<CalculateParametersResultDto> CalculateParameters(long headerId)
        {
            var header = await _db.TestResultHeaders
                .Include(h => h.Parameters)
                .FirstOrDefaultAsync(h => h.ID == headerId);

            if (header == null)
                throw new Exception("TestResultHeader not found.");

            // Build dictionary for formula vars
            Dictionary<string, double> vars = header.Parameters
                .Where(x => x.Value.HasValue && x.Value > 0)
                .ToDictionary(
                    x => $"P{x.ParameterID}",
                    x => (double)x.Value.Value
                );

            bool changed = true;
            int loopGuard = 0;
            int recalcCount = 0;

            while (changed && loopGuard < 10)
            {
                changed = false;
                loopGuard++;

                foreach (var param in header.Parameters)
                {
                    // Use FormulaExpression first, fallback to Formula
                    string formulaToUse = !string.IsNullOrWhiteSpace(param.FormulaExpression)
                        ? param.FormulaExpression
                        : param.Formula;

                    if (!string.IsNullOrWhiteSpace(formulaToUse))
                    {
                        try
                        {
                            double? result = _formulaEvaluator.Evaluate(formulaToUse, vars);

                            if (result.HasValue)
                            {
                                decimal newValue = Math.Round((decimal)result.Value, 4);

                                if (param.Value != newValue)
                                {
                                    param.Value = newValue;
                                    param.IsCalculated = true;
                                    changed = true;
                                    recalcCount++;

                                    vars[$"P{param.ParameterID}"] = (double)newValue;
                                }
                            }
                        }
                        catch
                        {
                            param.IsCalculated = false;
                        }
                    }

                    // Determine ResultStatus using enhanced spec limits
                    decimal? specMin = param.SpecMinValue ?? param.MinValue;
                    decimal? specMax = param.SpecMaxValue ?? param.MaxValue;

                    string resultStatus = _formulaEvaluator.DetermineResultStatus(param.Value, specMin, specMax);
                    if (resultStatus != null)
                    {
                        param.ResultStatus = resultStatus;
                        param.IsWithinLimit = resultStatus != "Fail";
                    }
                    else if (param.MinValue != null || param.MaxValue != null)
                    {
                        // Fallback to existing min/max check
                        if (param.Value != null)
                        {
                            bool withinMin = param.MinValue == null || param.Value >= param.MinValue;
                            bool withinMax = param.MaxValue == null || param.Value <= param.MaxValue;
                            param.IsWithinLimit = withinMin && withinMax;
                            param.ResultStatus = param.IsWithinLimit == true ? "Pass" : "Fail";
                        }
                        else
                        {
                            param.IsWithinLimit = null;
                            param.ResultStatus = null;
                        }
                    }
                }
            }

            // Overall Pass/Fail
            header.IsOverallPass = EvaluateOverallPass(header);

            await _db.SaveChangesAsync();

            var paramResults = header.Parameters.Select(p => new ParameterCalculationResultDto
            {
                Id = p.ID,
                ParameterID = p.ParameterID,
                ParameterName = p.ParameterName,
                Value = p.Value,
                IsCalculated = p.IsCalculated,
                ResultStatus = p.ResultStatus,
                IsWithinLimit = p.IsWithinLimit
            }).ToList();

            return new CalculateParametersResultDto
            {
                Success = true,
                Message = $"Recalculated {recalcCount} parameter(s).",
                ParametersRecalculated = recalcCount,
                IsOverallPass = header.IsOverallPass,
                Parameters = paramResults
            };
        }

        // =====================================================================
        //  ADD STANDALONE PARAMETER
        // =====================================================================
        public async Task<object> AddStandaloneParameter(long headerId, AddStandaloneParameterDto dto)
        {
            var header = await _db.TestResultHeaders
                .Include(h => h.Parameters)
                .FirstOrDefaultAsync(h => h.ID == headerId);

            if (header == null)
                throw new Exception("TestResultHeader not found.");

            var param = new TestResultParameter
            {
                TestResultHeaderID = headerId,
                ParameterID = 0, // standalone — no master parameter reference
                ParameterName = dto.ParameterName,
                Unit = dto.Unit,
                IsAdditional = true,
                IsStandalone = true,
                FormulaExpression = dto.FormulaExpression,
                SpecMinValue = dto.SpecMinValue,
                SpecMaxValue = dto.SpecMaxValue,
                AcceptanceCriteria = dto.AcceptanceCriteria
            };

            _db.TestResultParameters.Add(param);
            await _db.SaveChangesAsync();

            return new
            {
                Success = true,
                Message = "Standalone parameter added successfully.",
                ParameterId = param.ID,
                param.ParameterName,
                param.Unit
            };
        }

        // =====================================================================
        //  ADD PARAMETER FROM ANOTHER TEST METHOD
        // =====================================================================
        public async Task<object> AddParameterFromMethod(long headerId, AddParameterFromMethodDto dto)
        {
            var header = await _db.TestResultHeaders
                .Include(h => h.Parameters)
                .FirstOrDefaultAsync(h => h.ID == headerId);

            if (header == null)
                throw new Exception("TestResultHeader not found.");

            // Load the parameter from master
            var masterParam = await _db.ParameterMasters
                .Include(p => p.ParameterUnit)
                .FirstOrDefaultAsync(p => p.ID == dto.ParameterID);

            if (masterParam == null)
                throw new Exception($"Parameter with ID {dto.ParameterID} not found in master.");

            // Check if already added
            var existing = header.Parameters.FirstOrDefault(p =>
                p.ParameterID == dto.ParameterID && p.SourceTestMethodId == dto.SourceTestMethodId);

            if (existing != null)
                throw new Exception("This parameter from the specified method is already added.");

            var param = new TestResultParameter
            {
                TestResultHeaderID = headerId,
                ParameterID = masterParam.ID,
                ParameterName = masterParam.Name,
                Unit = masterParam.ParameterUnit?.Name ?? string.Empty,
                IsAdditional = true,
                IsStandalone = false,
                SourceTestMethodId = dto.SourceTestMethodId,
                Formula = masterParam.Formula,
                IsCalculated = masterParam.IsCalculated,
                FormulaExpression = masterParam.Formula
            };

            _db.TestResultParameters.Add(param);
            await _db.SaveChangesAsync();

            return new
            {
                Success = true,
                Message = "Parameter added from test method successfully.",
                ParameterId = param.ID,
                param.ParameterName,
                param.Unit,
                SourceTestMethodId = dto.SourceTestMethodId
            };
        }

        // =====================================================================
        //  GET ENVIRONMENT AT TEST TIME
        // =====================================================================
        public async Task<EnvironmentAtTimeDto> GetEnvironmentAtTime(long headerId)
        {
            var header = await _db.TestResultHeaders
                .FirstOrDefaultAsync(h => h.ID == headerId);

            if (header == null)
                throw new Exception("TestResultHeader not found.");

            return new EnvironmentAtTimeDto
            {
                HeaderId = header.ID,
                LabRoomId = header.LabRoomId,
                RoomTemperature = header.RoomTemperature,
                RoomHumidity = header.RoomHumidity,
                EquipmentIdsJson = header.EquipmentIdsJson,
                TestStartTime = header.TestStartTime,
                TestEndTime = header.TestEndTime,
                PerformedByName = header.PerformedByName,
                PerformedById = header.PerformedById
            };
        }

        // =====================================================================
        //  Phase 5: Test Verification Workflow
        // =====================================================================

        public async Task SubmitForVerification(long headerId)
        {
            var header = await _db.TestResultHeaders.FindAsync(headerId);
            if (header == null) throw new Exception("Header not found.");

            header.Status = "PendingVerification";
            await _db.SaveChangesAsync();

            // Update sample status
            await _sampleStatusService.ForceAutoStatusAsync(
                header.SampleID,
                SampleStatus.TESTING_UNDER_VERIFICATION,
                loggedInUser.EmployeeID);

            // Start workflow if configured
            try
            {
                await _workflowService.StartWorkflow(headerId, "TestResult");
            }
            catch
            {
                // Workflow may not be configured — that's OK
            }
        }

        public async Task<PagedResponse<object>> GetVerificationList(PageFilter filter)
        {
            var query = from h in _db.TestResultHeaders
                        where h.IsActive && h.Status == "PendingVerification"
                        join sample in _db.SampleDetails on h.SampleID equals sample.ID
                        join inward in _db.SampleInwards on sample.InwardID equals inward.ID
                        where inward.CompanyCode == loggedInUser.CompanyCode
                        select new
                        {
                            h.ID,
                            sample.SampleNo,
                            inward.CaseNo,
                            TestName = _db.LaboratoryTests
                                .Where(t => t.ID == h.LaboratoryTestID)
                                .Select(t => t.Name)
                                .FirstOrDefault(),
                            ParametersCount = h.Parameters.Count,
                            h.IsOverallPass,
                            h.IsNabl,
                            PerformedBy = h.PerformedByName,
                            h.CompletedAt,
                            h.Status
                        };

            int totalRecords = await query.CountAsync();

            var data = await query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return new PagedResponse<object>(
                data.Cast<object>().ToList(),
                totalRecords,
                filter.PageNumber,
                filter.PageSize);
        }

        public async Task VerifyTest(long headerId, string? comments)
        {
            var header = await _db.TestResultHeaders.FindAsync(headerId);
            if (header == null) throw new Exception("Header not found.");

            header.Status = "Verified";
            await _db.SaveChangesAsync();

            // Check if all tests for sample are verified
            bool allVerified = !await _db.TestResultHeaders.AnyAsync(h =>
                h.SampleID == header.SampleID &&
                h.IsActive &&
                h.Status != "Verified" && h.Status != "Completed");

            if (allVerified)
            {
                await _sampleStatusService.ForceAutoStatusAsync(
                    header.SampleID,
                    SampleStatus.TESTING_VERIFIED,
                    loggedInUser.EmployeeID);
            }
        }

        public async Task RejectVerification(long headerId, string? comments)
        {
            var header = await _db.TestResultHeaders.FindAsync(headerId);
            if (header == null) throw new Exception("Header not found.");

            header.Status = "VerificationRejected";
            await _db.SaveChangesAsync();

            await _sampleStatusService.ForceAutoStatusAsync(
                header.SampleID,
                SampleStatus.TESTING_VERIFICATION_REJECTED,
                loggedInUser.EmployeeID);
        }

        // =====================================================================
        //  Phase 5: Preparation Status
        // =====================================================================

        public async Task<PreparationStatusDto> GetPreparationStatus(long sampleId)
        {
            var sample = await _db.SampleDetails.FindAsync(sampleId);
            if (sample == null) throw new Exception("Sample not found.");

            var result = new PreparationStatusDto
            {
                PreparationRequired = sample.PreparationRequired,
            };

            // Machining charges from MachiningChargeItems (fallback to SampleDetail.MachiningAmount)
            var machiningItems = await _db.MachiningChargeItems
                .Where(m => m.SampleID == sampleId && m.IsActive)
                .ToListAsync();
            result.MachiningCharges = machiningItems.Any()
                ? machiningItems.Sum(m => m.Amount)
                : sample.MachiningAmount;
            result.MachiningBreakdown = machiningItems.Select(m => new MachiningChargeLineDto
            {
                Id = m.ID, Description = m.Description, Amount = m.Amount, Remark = m.Remark
            }).ToList();

            // Check if cutting charge data exists
            var cuttingHeader = await _db.CuttingChargeHeaders
                .Include(h => h.Samples)
                    .ThenInclude(s => s.CuttingChargeDetails)
                .FirstOrDefaultAsync(h => h.InwardID == sample.InwardID && h.IsActive);

            if (cuttingHeader != null)
            {
                var cuttingSample = cuttingHeader.Samples.FirstOrDefault(s => s.SampleID == sampleId);
                if (cuttingSample != null)
                {
                    result.PreparationRecorded = true;
                    result.PreparationStatus = cuttingSample.PreparationStatus;
                    result.CuttingCharges = cuttingSample.SampleTotal;
                    result.CuttingEditUrl = $"/sample/cutting/edit/{cuttingHeader.ID}";

                    result.Specimens = cuttingSample.CuttingChargeDetails.Select(d => (object)new
                    {
                        d.ID,
                        cuttingSample.SpecimenTypeId,
                        cuttingSample.Length,
                        cuttingSample.Width,
                        cuttingSample.Thickness,
                        cuttingSample.Diameter,
                        cuttingSample.Orientation
                    }).ToList();
                }
            }

            return result;
        }

        // =====================================================================
        //  Phase 6: Unified Price Summary
        // =====================================================================

        public async Task<UnifiedPriceSummaryDto> GetUnifiedPriceSummary(long sampleId)
        {
            var sample = await _db.SampleDetails
                .Include(s => s.SampleInward)
                .FirstOrDefaultAsync(s => s.ID == sampleId);

            if (sample == null) throw new Exception("Sample not found.");

            // Machining line items (fallback to legacy single amount)
            var machiningItems = await _db.MachiningChargeItems
                .Where(m => m.SampleID == sampleId && m.IsActive)
                .ToListAsync();

            var dto = new UnifiedPriceSummaryDto
            {
                SampleId = sampleId,
                InwardId = sample.InwardID,
                MachiningCharges = machiningItems.Any()
                    ? machiningItems.Sum(m => m.Amount)
                    : sample.MachiningAmount,
                OtherPreparationCharges = sample.OtherPreparationCharge,
                BillingStatus = sample.SampleInward?.BillingStatus,
                MachiningBreakdown = machiningItems.Select(m => new MachiningChargeLineDto
                {
                    Id = m.ID,
                    Description = m.Description,
                    Amount = m.Amount,
                    Remark = m.Remark
                }).ToList()
            };

            // Cutting charges
            var cuttingHeader = await _db.CuttingChargeHeaders
                .Include(h => h.Samples)
                    .ThenInclude(s => s.CuttingChargeDetails)
                .FirstOrDefaultAsync(h => h.InwardID == sample.InwardID && h.IsActive);

            if (cuttingHeader != null)
            {
                var cuttingSample = cuttingHeader.Samples.FirstOrDefault(s => s.SampleID == sampleId);
                if (cuttingSample != null)
                {
                    dto.CuttingCharges = cuttingSample.SampleTotal;
                    dto.CuttingBreakdown = cuttingSample.CuttingChargeDetails.Select(d => new CuttingChargeLineDto
                    {
                        CuttingType = d.CuttingType ?? "",
                        UnitType = d.UnitType ?? "",
                        Rate = d.Rate,
                        Quantity = d.Quantity,
                        Total = d.Total
                    }).ToList();
                }
            }

            dto.TotalPreparationCharges = dto.CuttingCharges + dto.MachiningCharges + dto.OtherPreparationCharges;

            // Test charges
            var headers = await _db.TestResultHeaders
                .Where(h => h.SampleID == sampleId && h.IsActive)
                .ToListAsync();

            foreach (var h in headers)
            {
                var testName = await _db.LaboratoryTests
                    .Where(t => t.ID == h.LaboratoryTestID)
                    .Select(t => t.Name)
                    .FirstOrDefaultAsync() ?? "Unknown";

                var finalPrice = h.PriceOverridden ? (h.OverridePrice ?? 0) : (h.CalculatedPrice ?? 0);

                dto.TestCharges.Add(new TestChargeLineDto
                {
                    HeaderId = h.ID,
                    TestName = testName,
                    CalculatedPrice = h.CalculatedPrice ?? 0,
                    OverridePrice = h.OverridePrice,
                    FinalPrice = finalPrice,
                    IsOverridden = h.PriceOverridden
                });
            }

            dto.TotalTestCharges = dto.TestCharges.Sum(t => t.FinalPrice);
            dto.GrandTotal = dto.TotalPreparationCharges + dto.TotalTestCharges;

            return dto;
        }

        // =============================================================
        // Machining Charge Items — simple line items per sample
        // =============================================================

        public async Task<List<MachiningChargeLineDto>> GetMachiningItems(long sampleId)
        {
            return await _db.MachiningChargeItems
                .Where(m => m.SampleID == sampleId && m.IsActive)
                .OrderBy(m => m.ID)
                .Select(m => new MachiningChargeLineDto
                {
                    Id = m.ID,
                    Description = m.Description,
                    Amount = m.Amount,
                    Remark = m.Remark
                })
                .ToListAsync();
        }

        public async Task<MachiningChargeLineDto> AddMachiningItem(long sampleId, MachiningChargeItemDto dto)
        {
            var sample = await _db.SampleDetails.FindAsync(sampleId);
            if (sample == null) throw new Exception("Sample not found.");

            var item = new MachiningChargeItem
            {
                SampleID = sampleId,
                Description = dto.Description,
                Amount = dto.Amount,
                Remark = dto.Remark,
                IsActive = true,
                CreatedOn = DateTime.UtcNow,
                CreatedBy = loggedInUser.EmployeeID,
                CompanyCode = loggedInUser.CompanyCode
            };

            _db.MachiningChargeItems.Add(item);
            await _db.SaveChangesAsync();

            return new MachiningChargeLineDto
            {
                Id = item.ID,
                Description = item.Description,
                Amount = item.Amount,
                Remark = item.Remark
            };
        }

        public async Task<MachiningChargeLineDto> UpdateMachiningItem(long itemId, MachiningChargeItemDto dto)
        {
            var item = await _db.MachiningChargeItems.FindAsync(itemId);
            if (item == null) throw new Exception("Machining charge item not found.");

            item.Description = dto.Description;
            item.Amount = dto.Amount;
            item.Remark = dto.Remark;
            item.ModifiedOn = DateTime.UtcNow;
            item.ModifiedBy = loggedInUser.EmployeeID;

            await _db.SaveChangesAsync();

            return new MachiningChargeLineDto
            {
                Id = item.ID,
                Description = item.Description,
                Amount = item.Amount,
                Remark = item.Remark
            };
        }

        public async Task DeleteMachiningItem(long itemId)
        {
            var item = await _db.MachiningChargeItems.FindAsync(itemId);
            if (item == null) throw new Exception("Machining charge item not found.");

            item.IsActive = false;
            item.ModifiedOn = DateTime.UtcNow;
            item.ModifiedBy = loggedInUser.EmployeeID;

            await _db.SaveChangesAsync();
        }
    }
}
