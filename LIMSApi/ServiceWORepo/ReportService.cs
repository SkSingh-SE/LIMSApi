// LIMSApi/ServiceWORepo/ReportingService.cs
using System.Linq.Dynamic.Core;
using System.Text.Json;
using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Helpers.Enums;
using LIMSApi.Helpers.StatusFlow;
using LIMSApi.Models;
using LIMSApi.Reporting;
using LIMSApi.Services.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic.FileIO;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using QuestPDF.Fluent;
using Razorpay.Api;
using static LIMSApi.Reporting.ReportDocument;

namespace LIMSApi.ServiceWORepo
{
    public class ReportService : IReportService
    {
        private readonly LIMSContext _db;
        private readonly IFileUploadService fileUploadService;
        private readonly LoggedInUserDTO loggedInUser;
        private readonly IWorkflowService _workflowService;
        private readonly ITestResultService _testResultService;
        private readonly IReportBlockGenerator _reportBlockGenerator;
        private readonly ISampleStatusService _sampleStatusService;
        private readonly IConfiguration _config;
        private readonly EmailService _emailService;
        private readonly TemplateService _templateService;

        public ReportService(LIMSContext db, IFileUploadService uploadService, IWorkflowService workflowService, ITestResultService testResultService, IReportBlockGenerator reportBlockGenerator, ISampleStatusService sampleStatusService, IConfiguration config, EmailService emailService, TemplateService templateService)
        {
            _db = db;
            fileUploadService = uploadService;
            loggedInUser = LoggedInUserProvider.CurrentUser;
            _workflowService = workflowService;
            _testResultService = testResultService;
            _reportBlockGenerator = reportBlockGenerator;
            _sampleStatusService = sampleStatusService;
            _config = config;
            _emailService = emailService;
            _templateService = templateService;
        }

        public async Task<PagedResponse<object>> GetReportDashboardList(PageFilter filter)
        {
            var userId = loggedInUser.EmployeeID;

            // Entry point: TESTING_VERIFIED (priority 5). Everything at priority 6/7/99 is included via GetAllowedStatuses.
            var allowedStatuses = StatusFilterHelper.GetAllowedStatuses(WorkflowListType.Reporting)
                .Append(SampleStatus.TESTING_VERIFIED.ToString())
                .ToList();

            // ----------------------------------------------------------
            // BASE QUERY
            // Start from SampleDetails that have at least one TestResultHeader,
            // LEFT JOIN to ReportHeaders so we also see samples without reports.
            // ----------------------------------------------------------
            var query = from sample in _db.SampleDetails

                        join inward in _db.SampleInwards
                            on sample.InwardID equals inward.ID

                        // Only samples that have reached verification or reporting stage
                        where _db.TestResultHeaders.Any(trh => trh.SampleID == sample.ID)
                              && sample.IsActive
                              && inward.CompanyCode == loggedInUser.CompanyCode
                              && allowedStatuses.Contains(sample.SampleStatus)

                        // LEFT JOIN to ReportHeader
                        join rh in _db.ReportHeaders.Where(r => r.IsActive)
                            on sample.ID equals rh.SampleID into reportJoin
                        from report in reportJoin.DefaultIfEmpty()

                        // LEFT JOIN to AmendmentRequest (only when report exists)
                        join amendment in _db.AmendmentRequests on report.ID equals amendment.ReportHeaderID into amendmentJoin
                        from amendment in amendmentJoin
                            .Where(a => a.Status == "Pending")
                            .OrderByDescending(a => a.CreatedOn)
                            .Take(1)
                            .DefaultIfEmpty()

                        // LEFT JOIN WorkflowInstance
                        join instance in _db.WorkflowInstances
                            on new
                            {
                                EntityID = amendment != null ? amendment.ID : (report != null ? report.ID : 0L),
                                EntityType = amendment != null
                                    ? WorkFlowEntityTypeExtensions.GetEntityType(WorkFlowEntityType.Report_Amendment)
                                    : WorkFlowEntityTypeExtensions.GetEntityType(WorkFlowEntityType.Report_Review)
                            }
                            equals new { instance.EntityID, instance.EntityType }
                            into workflowJoin
                        from instance in workflowJoin.DefaultIfEmpty()

                        join step in _db.WorkflowSteps
                            on instance.CurrentStepID equals step.ID
                            into stepJoin
                        from step in stepJoin.DefaultIfEmpty()

                        select new
                        {
                            ReportHeaderId = report != null ? report.ID : 0L,
                            AmendmentRequestId = amendment != null ? amendment.ID : 0L,
                            sampleId = sample.ID,
                            sample.SampleNo,
                            InwardId = inward.ID,
                            inward.CaseNo,
                            inward.CustomerID,
                            Customer = inward.Customer != null
                                ? inward.Customer.Name
                                : string.Empty,

                            Material = sample.MetalClassificationID != null
                                ? _db.MetalClassificationMasters
                                    .Where(x => x.ID == sample.MetalClassificationID.Value)
                                    .Select(m => m.Name)
                                    .FirstOrDefault()
                                : string.Empty,

                            Condition = sample.ProductConditionID != null
                                ? _db.ProductConditionMasters
                                    .Where(x => x.ID == sample.ProductConditionID.Value)
                                    .Select(m => m.Name)
                                    .FirstOrDefault()
                                : string.Empty,

                            ReportNo = report != null ? report.ReportNo : "",
                            PdfPath = report != null ? report.PdfPath : null,

                            // Authoritative sample-level status (same as Test List)
                            SampleStatus = sample.SampleStatus,

                            // Report-level status — used for workflow/action logic only
                            ReportStatus = report != null ? report.Status : null,

                            WorkflowStatus = instance == null ? "Pending" : instance.Status,
                            CurrentStep = step != null ? step.Name : null,

                            CanTakeAction = instance != null
                                            && instance.IsActive
                                            && step != null
                                            && FilterHelper.IsUserApprover(step.AssignedToValue, userId),

                            Actions = instance != null
                                        && instance.IsActive
                                        && step != null
                                            ? step.Transitions
                                                .Where(t => t.IsActive)
                                                .Select(t => new
                                                {
                                                    ID = instance.ID,
                                                    Name = t.Alias ?? t.Action,
                                                    Action = t.Action
                                                })
                                                .ToList()
                                            : null
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
                var search = filter.searchTerm.Trim();

                query = query.Where(x =>
                    x.SampleNo.Contains(search) ||
                    x.CaseNo.Contains(search) ||
                    x.Customer.Contains(search) ||
                    x.Material.Contains(search) ||
                    x.Condition.Contains(search) ||
                    x.ReportNo.Contains(search)
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
            // FINAL SHAPE FOR FRONTEND
            // ----------------------------------------------------------
            var result = data.Select(x => new
            {
                x.sampleId,
                x.ReportHeaderId,
                x.InwardId,
                x.SampleNo,
                x.CaseNo,
                x.Customer,
                x.CustomerID,
                x.Material,
                x.Condition,
                // Use sample-level status for the badge (consistent with Test List)
                Status = x.SampleStatus,

                x.ReportNo,
                x.CurrentStep,
                x.CanTakeAction,
                x.Actions
            }).ToList<object>();

            return new PagedResponse<object>(
                result,
                totalRecords,
                filter.PageNumber,
                filter.PageSize
            );
        }


        public async Task<ReportReadDto> CreateReportFromSampleAsync(ReportCreateFromSampleDto dto)
        {
            // -------------------------------------------------
            // 1. Load Test Headers
            // -------------------------------------------------
            var headers = await _db.TestResultHeaders
                .Include(h => h.Parameters)
                .Include(h => h.Images)
                .Include(h => h.LaboratoryTest)
                .Where(h => dto.TestResultHeaderIds.Contains(h.ID))
                .ToListAsync();

            if (!headers.Any())
                throw new Exception("No TestResultHeaders found");

            var sampleId = headers.First().SampleID;

            // -------------------------------------------------
            // 2. Report No + Certificate
            // -------------------------------------------------
            string reportNo = string.IsNullOrWhiteSpace(dto.ReportNo)
                ? $"R-{DateTime.UtcNow:yyyyMMddHHmmss}"
                : dto.ReportNo;

            string certificateNo = $"DMSPL-{DateTime.UtcNow:yy}-{sampleId:D6}-1";

            // -------------------------------------------------
            // 3. Create Report
            // -------------------------------------------------
            var report = new Report
            {
                ReportNo = reportNo,
                Status = "Draft",
                CreatedBy = loggedInUser.EmployeeID,
                CreatedOn = DateTime.UtcNow
            };

            int sortOrder = 0;

            //// -------------------------------------------------
            //// 4. HEADER BLOCK (ONCE)
            //// -------------------------------------------------
            //report.Blocks.Add(new ReportBlock
            //{
            //    BlockType = "Header",
            //    SortOrder = sortOrder++,
            //    PayloadJson = JsonSerializer.Serialize(new
            //    {
            //        CertificateNo = certificateNo,
            //        ReportNo = reportNo,
            //        DateOfIssue = DateTime.UtcNow.ToString("dd-MM-yyyy"),
            //        SampleId = sampleId,
            //        TestName = "Multiple Tests"
            //    })
            //});

            // -------------------------------------------------
            // 5. LOOP THROUGH TESTS (🔥 CORE PART)
            // -------------------------------------------------
            foreach (var header in headers)
            {
                // 5.1 Load template per test
                var template = await _db.ReportTemplates
                    .Include(t => t.Blocks)
                    .FirstOrDefaultAsync(t =>
                        t.TestTypeID == header.LaboratoryTestID &&
                        t.IsActive);

                if (template == null)
                {
                    template = await _db.ReportTemplates
                    .Include(t => t.Blocks)
                    .FirstOrDefaultAsync(t => t.IsDefault && t.IsActive);
                }

                //// 5.2 Section title
                //report.Blocks.Add(new ReportBlock
                //{
                //    BlockType = "SectionTitle",
                //    SortOrder = sortOrder++,
                //    PayloadJson = JsonSerializer.Serialize(new
                //    {
                //        Title = header.LaboratoryTest.Name
                //    })
                //});

                // 5.3 Generate blocks via generator
                var previewPayload =
                    await _testResultService.GetSampleDetailsForResult(sampleId);

                //var blocks = await _reportBlockGenerator.GenerateBlocksAsync(
                //    template,
                //    header,
                //    previewPayload!,
                //    reportNo,
                //    certificateNo
                //);

                //foreach (var block in blocks.Where(b => b != null))
                //{
                //    block.SortOrder = sortOrder++;
                //    report.Blocks.Add(block);
                //}
            }

            // -------------------------------------------------
            // 6. SAVE
            // -------------------------------------------------
            _db.Reports.Add(report);

            var reportHeader = new ReportHeader
            {
                SampleID = sampleId,
                ReportNo = reportNo,
                Status = "Generated",
                GeneratedAt = DateTime.UtcNow,
                GeneratedBy = loggedInUser.EmployeeID.ToString(),
                CertificateNo = certificateNo
            };

            _db.ReportHeaders.Add(reportHeader);

            await _db.SaveChangesAsync();

            return await GetReportAsync(report.ID);
        }

        // ---------------------------------------------------------
        // GET REPORT BY ID (with Blocks)
        // ---------------------------------------------------------
        public async Task<ReportReadDto> GetReportAsync(long id)
        {
            var report = await _db.Reports
                .Include(r => r.Blocks)
                .FirstOrDefaultAsync(r => r.ID == id);

            if (report == null) return null;

            string? certificateNo = ExtractCertificateFromHeaderBlock(report);

            return new ReportReadDto
            {
                Id = report.ID,
                ReportNo = report.ReportNo,
                CertificateNo = certificateNo,
                Status = report.Status,
                CreatedOn = report.CreatedOn,
                Blocks = report.Blocks
                    .OrderBy(b => b.SortOrder)
                    .Select(b => new ReportBlockReadDto
                    {
                        Id = b.ID,
                        BlockType = b.BlockType,
                        SortOrder = b.SortOrder,
                        PayloadJson = b.PayloadJson
                    })
                    .ToList()
            };
        }

        private string? ExtractCertificateFromHeaderBlock(Report report)
        {
            var block = report.Blocks.FirstOrDefault(x => x.BlockType == "Header");
            if (block == null) return null;

            try
            {
                using var doc = JsonDocument.Parse(block.PayloadJson);
                return doc.RootElement.GetProperty("CertificateNo").GetString();
            }
            catch
            {
                return null;
            }
        }


        // ---------------------------------------------------------
        // GENERATE PDF
        // ---------------------------------------------------------
        public async Task<string> GeneratePdfAsync(long reportId)
        {
            var report = await _db.Reports
                .Include(r => r.Blocks)
                .FirstOrDefaultAsync(r => r.ID == reportId);

            if (report == null)
                throw new Exception("Report not found");


            var fileName = $"report_{reportId}_{DateTime.UtcNow:yyyyMMddHHmmss}.pdf";
            var filePath = fileUploadService.GetFilePath(fileName, FileType.Report, null);


            var document = new ReportDocument(report);
            document.GeneratePdf(filePath);

            report.PdfPath = filePath;
            // Optionally store PDF path in Report
            var reportHeader = await _db.ReportHeaders.FirstOrDefaultAsync(r => r.ReportNo == report.ReportNo);
            if (reportHeader != null)
            {
                reportHeader.PdfPath = filePath;
                await _db.SaveChangesAsync();
            }

            return filePath;
        }

        public async Task<bool> PerformAction(WorkflowActionRequestDto dto)
        {
            try
            {
                await _workflowService.PerformWorkflowActionAsync(dto);
            }
            catch (Exception ex)
            {
                throw new Exception("Error performing workflow action: " + ex.Message);
            }
            return true;
        }

        public async Task<ReportPreviewDto> GetReportPreviewAsync(long reportHeaderId)
        {
            // -------------------------------------------------
            // 1. Load Report Header + Sample + Inward
            // -------------------------------------------------
            // Try finding by ReportHeader ID first, then by SampleID (frontend may send either)
            var reportHeader = await _db.ReportHeaders
                .Include(r => r.Sample)
                    .ThenInclude(s => s.SampleInward)
                        .ThenInclude(i => i.Customer)
                .Include(r => r.Sample)
                    .ThenInclude(s => s.SpecimenOrientation)
                .Include(r => r.Sample)
                    .ThenInclude(s => s.ProductForm)
                .Include(r => r.Sample)
                    .ThenInclude(s => s.ProductCondition)
                        .ThenInclude(pc => pc.LinkedHeatTreatment)
                .FirstOrDefaultAsync(r => r.ID == reportHeaderId);

            // Fallback: treat the ID as a SampleID
            if (reportHeader == null)
            {
                reportHeader = await _db.ReportHeaders
                    .Include(r => r.Sample)
                        .ThenInclude(s => s.SampleInward)
                            .ThenInclude(i => i.Customer)
                    .Include(r => r.Sample)
                        .ThenInclude(s => s.SpecimenOrientation)
                    .Include(r => r.Sample)
                        .ThenInclude(s => s.ProductForm)
                    .Include(r => r.Sample)
                        .ThenInclude(s => s.ProductCondition)
                            .ThenInclude(pc => pc.LinkedHeatTreatment)
                    .FirstOrDefaultAsync(r => r.SampleID == reportHeaderId && r.IsActive);
            }

            if (reportHeader == null)
                throw new Exception("Report header not found");

            var pendingAmendment = await _db.AmendmentRequests
                .Where(a =>
                    a.ReportHeaderID == reportHeader.ID &&
                    a.Status == "Pending")
                .OrderByDescending(a => a.CreatedOn)
                .FirstOrDefaultAsync();


            var sample = reportHeader.Sample!;
            var inward = sample.SampleInward!;

            // -------------------------------------------------
            // 2. Reuse existing Test Result API (CRITICAL)
            // -------------------------------------------------
            var testResultPayload =
                await _testResultService.GetSampleDetailsForResult(sample.ID);

            if (testResultPayload == null)
                throw new Exception("Test result data not found");

            // dynamic because existing method returns anonymous object
            dynamic testData = testResultPayload;

            // -------------------------------------------------
            // 3. Load Workflow + Actions (SAME PATTERN AS YOUR CODE)
            // -------------------------------------------------

            long entityId;
            string entityType;

            if (reportHeader.Status == "Under Amendment Review" && pendingAmendment != null)
            {
                entityId = pendingAmendment.ID;
                entityType = WorkFlowEntityTypeExtensions.GetEntityType(
                    WorkFlowEntityType.Report_Amendment);
            }
            else
            {
                entityId = reportHeader.ID;
                entityType = WorkFlowEntityTypeExtensions.GetEntityType(
                    WorkFlowEntityType.Report_Review);
            }

            var workflowInstance = await _workflowService.GetActiveInstanceForEntityAsync(entityId, entityType);

            var actions = new List<ReportActionDto>();

            bool canTakeAction = false;

            if (workflowInstance != null)
            {
                var step = await _workflowService.GetCurrentWorkflowStepAsync(entityId, entityType);

                if (step != null)
                {

                    canTakeAction = FilterHelper.IsUserApprover(step.AssignedToValue, loggedInUser.EmployeeID);

                    if (canTakeAction)
                    {
                        actions = step.Transitions
                            .Where(t => t.IsActive)
                            .Select(t => new ReportActionDto
                            {
                                Id = workflowInstance.ID,
                                Name = t.Alias ?? t.Action,
                                Action = t.Action
                            })
                            .ToList();
                    }
                }
            }

            // -------------------------------------------------
            // 4. Map Test Plans → Mechanical / Chemical
            // -------------------------------------------------
            var mechanicalTests = new List<ReportTestDto>();
            var chemicalTests = new List<ReportTestDto>();

            foreach (var plan in testData.plans)
            {
                foreach (var gt in plan.generalTests)
                {
                    mechanicalTests.Add(MapTestDto(gt));
                }

                foreach (var ct in plan.chemicalTests)
                {
                    chemicalTests.Add(MapTestDto(ct));
                }
            }

            // -------------------------------------------------
            // 5. Long-Term Tests
            // -------------------------------------------------
            var longTermTests = await _db.LongTermTests
                        .Where(x => x.SampleID == sample.ID)
                        .Include(x => x.TestResultHeader)
                            .ThenInclude(trh => trh.Parameters)
                        .Include(x => x.TestResultHeader)
                            .ThenInclude(trh => trh.LaboratoryTest)
                        .Include(x => x.Records)
                        .ToListAsync();

            var longTermDtos = longTermTests.Select(ltt => new ReportLongTermTestDto
            {
                LongTermTestId = ltt.ID,

                TestName =
                    ltt.TestResultHeader?.LaboratoryTest?.Name
                    ?? "Long Term Test",

                DurationHours = ltt.DurationHours,

                StartedAt =
                    ltt.TestResultHeader?.StartedAt
                    ?? DateTime.UtcNow,

                EndedAt =
                    ltt.TestResultHeader?.CompletedAt,

                Status = ltt.Status,

                Parameters = ltt.TestResultHeader?.Parameters
                    .Select(p => new ReportLongTermParameterDto
                    {
                        ParameterName = p.ParameterName,
                        Value = p.Value != null ? p.Value.ToString()! : string.Empty
                    }).ToList()
                    ?? new List<ReportLongTermParameterDto>(),

                Readings = ltt.Records
                    .OrderBy(r => r.RecordedAt)
                    .Select(r => new ReportLongTermReadingDto
                    {
                        RecordedAt = r.RecordedAt,
                        Parsed = !string.IsNullOrWhiteSpace(r.DataJson) ? SafeParseLongTermJson(r.DataJson) : null
                    }).ToList()
            }).ToList();

            // -------------------------------------------------
            // 6. Assemble FINAL DTO
            // -------------------------------------------------
            ReportAmendmentPreviewDto? amendmentDto = null;

            if (reportHeader.Status == "Under Amendment Review" && pendingAmendment != null)
            {
                amendmentDto = new ReportAmendmentPreviewDto
                {
                    AmendmentRequestId = pendingAmendment.ID,
                    Reason = pendingAmendment.Reason,
                    FileName = pendingAmendment.FileName,
                    FilePath = pendingAmendment.FilePath,
                    RequestedOn = pendingAmendment.CreatedOn
                };
            }

            bool canSubmitForReportApproval = workflowInstance == null
                && sample.SampleStatus == SampleStatus.TESTING_VERIFIED.ToString()
                && (reportHeader.Status == "Pending" || reportHeader.Status == "Report Generated");

            return new ReportPreviewDto
            {
                ReportHeaderId = reportHeader.ID,
                WorkflowInstanceId = workflowInstance?.ID ?? 0,

                SampleNo = sample.SampleNo,
                CaseNo = inward.CaseNo,
                Customer = inward.Customer?.Name ?? "",
                Material = testData.sample.metalClassification,
                Condition = testData.sample.productCondition,

                ReportNo = reportHeader.ReportNo,

                // Sample Details for approval review
                Grade = sample.ProductCondition?.Name,
                SampleDescription = (string?)testData.sample.details,
                SampleReceivedDate = inward.CreatedOn,
                TestStartDate = reportHeader.CreatedOn,
                Thickness = sample.Thickness,
                Diameter = sample.Diameter,
                Width = sample.Width,
                Length = sample.Length,
                StatementOfConformity = inward.StatementOfConformity,
                DecisionRule = inward.DecisionRule,

                // NABL compliance fields
                ProductForm = sample.ProductForm?.Name,
                SpecimenOrientation = sample.SpecimenOrientation?.Name,
                HeatTreatment = sample.ProductCondition?.LinkedHeatTreatment?.Name,
                CrossSectionArea = sample.Diameter.HasValue && sample.Diameter > 0
                    ? Math.Round((decimal)(Math.PI / 4) * sample.Diameter.Value * sample.Diameter.Value, 4)
                    : sample.Thickness.HasValue && sample.Width.HasValue
                        ? Math.Round(sample.Thickness.Value * sample.Width.Value, 4)
                        : null,
                GaugeLength = (sample.Diameter.HasValue && sample.Diameter > 0
                    ? Math.Round(5.65m * (decimal)Math.Sqrt((double)(Math.PI / 4) * (double)(sample.Diameter.Value * sample.Diameter.Value)), 2)
                    : sample.Thickness.HasValue && sample.Width.HasValue
                        ? Math.Round(5.65m * (decimal)Math.Sqrt((double)(sample.Thickness.Value * sample.Width.Value)), 2)
                        : null),

                MechanicalTests = mechanicalTests,
                ChemicalTests = chemicalTests,
                LongTermTests = longTermDtos,

                Actions = canTakeAction ? actions : new List<ReportActionDto>(),
                Status = reportHeader.Status == "Under Amendment Review"
                    ? "Under Amendment Review"
                    : reportHeader.Status == "Pending" && workflowInstance != null
                        ? "Pending for Approval"
                        : reportHeader.Status == "Pending"
                            ? "Report Ready"
                            : reportHeader.Status,
                Amendment = amendmentDto,
                CanSubmitForReportApproval = canSubmitForReportApproval
            };
        }
        private LongTermParsedValue? SafeParseLongTermJson(string json)
        {
            try
            {
                return JsonSerializer.Deserialize<LongTermParsedValue>(
                    json,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
            }
            catch
            {
                return null;
            }
        }

        private ReportTestDto MapTestDto(dynamic test)
        {
            // Add specimen label when multiple specimens exist
            string baseName = test.laboratoryTest ?? "Test";
            int totalSpec = 1;
            int seqNo = 1;
            try { totalSpec = (int)(test.totalSpecimens ?? 1); } catch { }
            try { seqNo = (int)(test.sequenceNo ?? 1); } catch { }
            string testName = totalSpec > 1 ? $"{baseName} - Specimen {seqNo}" : baseName;

            return new ReportTestDto
            {
                TestResultHeaderId = test.headerId,
                TestName = testName,
                TestMethod = test.standardName != null ? (string?)test.standardName : (test.standard != null ? test.standard.ToString() : null),
                ReportNo = test.reportNo,
                Specification1Name = test.specfication1Name,
                Specification2Name = test.specfication2Name,
                Status = test.status,
                Parameters = ((IEnumerable<dynamic>)test.parameters)
                    .Select(p => new ReportTestParameterDto
                    {
                        ParameterID = p.ParameterID,
                        ParameterName = p.ParameterName,
                        Value = p.value,
                        Unit = p.unit,
                        MinValue = p.specMinValue ?? p.minValue,
                        MaxValue = p.specMaxValue ?? p.maxValue,
                        IsWithinLimit = p.isWithinLimit ?? (p.minValue != null && p.maxValue != null
                            ? (p.value >= p.minValue && p.value <= p.maxValue)
                            : true),
                        ResultStatus = p.resultStatus,
                        Remarks = p.Remarks,
                        DecimalPrecision = (int)(p.decimalPrecision ?? 2),
                        ConversionFactor = (decimal)(p.conversionFactor ?? 1m),
                        ConvertedValue = p.convertedValue,
                        SelectedUnit = p.selectedUnit
                    }).ToList(),
                Images = ((IEnumerable<dynamic>)test.images)
                    .Select(img => new ReportTestImageDto
                    {
                        ID = img.ID,
                        FilePath = img.FilePath,
                        Caption = img.Caption
                    }).ToList()
            };
        }

        public async Task<string> GeneratePdfForSampleAsync(long sampleId)
        {
            // -------------------------------------------------
            // 1️⃣ Get Report Header for Sample
            // -------------------------------------------------
            var reportHeader = await _db.ReportHeaders
                .Include(x => x.Sample).ThenInclude(s => s.SampleInward).ThenInclude(c => c.Customer)
                .FirstOrDefaultAsync(r => r.SampleID == sampleId);

            // If no report header exists, create one via GenerateReportAsync
            if (reportHeader == null)
            {
                var reportId = await GenerateReportAsync(sampleId);
                var report = await _db.Reports.Include(r => r.Blocks).FirstAsync(r => r.ID == reportId);
                reportHeader = await _db.ReportHeaders.FindAsync(report.ReportHeaderID);
            }

            if (reportHeader == null)
                throw new InvalidOperationException("Failed to create report header.");

            // -------------------------------------------------
            // 2️⃣ Generate PDF using EnhancedReportDocument
            // -------------------------------------------------
            var reportData = await BuildReportDataAsync(reportHeader.ID);
            var document = Reporting.ReportDocumentFactory.Create(
                Helpers.Enums.ReportFormatType.TestCertificate, reportData);
            var pdfBytes = document.GeneratePdf();

            // Save PDF to disk
            var uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Uploads", "Report");
            Directory.CreateDirectory(uploadsDir);
            var fileName = $"Report-{reportHeader.ReportNo}-{DateTime.UtcNow:yyyyMMddHHmmss}.pdf";
            var filePath = Path.Combine(uploadsDir, fileName);
            await File.WriteAllBytesAsync(filePath, pdfBytes);

            // Update report header
            reportHeader.PdfPath = $"Uploads/Report/{fileName}";
            if (reportHeader.Status == "Pending" || string.IsNullOrEmpty(reportHeader.Status))
                reportHeader.Status = "Report Generated";
            reportHeader.GeneratedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            return filePath;

            // Email notification and amendment token creation will be handled
            // when report is sent for approval (separate workflow)
        }
        public string GenerateReportLink(string token)
        {
            return $"{_config["PublicBaseUrl"]}/report/amend/{token}";
        }


        // =====================================================
        // 2️⃣ GENERATE FINAL REPORT (PERSIST VERSION)
        // =====================================================
        public async Task<long> GenerateReportAsync(long sampleId)
        {
            // -------------------------------------------------
            // 1️⃣ Load Sample
            // -------------------------------------------------
            var sample = await _db.SampleDetails
                .Include(s => s.SampleInward)
                    .ThenInclude(i => i.Customer)
                .FirstOrDefaultAsync(s => s.ID == sampleId)
                ?? throw new InvalidOperationException("Sample not found");

            // -------------------------------------------------
            // 2️⃣ Load Test Headers (WITH LongTerm inside)
            // -------------------------------------------------
            var testHeaders = await _db.TestResultHeaders
                .Include(h => h.Parameters)
                .Include(h => h.Images)
                .Include(h => h.LongTermTests)
                    .ThenInclude(lt => lt.Records)
                .Include(h => h.LaboratoryTest)
                .Where(h => h.SampleID == sampleId)
                .OrderBy(h => h.LaboratoryTest.Name)
                .ToListAsync();

            if (!testHeaders.Any())
                throw new InvalidOperationException("No test results found for sample.");

            // -------------------------------------------------
            // 3️⃣ Report Header
            // -------------------------------------------------
            var reportHeader = await GetOrCreateReportHeaderAsync(sample);

            if (reportHeader.Status == "Final")
                throw new InvalidOperationException("Report already finalized.");

            // -------------------------------------------------
            // 4️⃣ BUILD REPORT PAYLOAD
            // -------------------------------------------------
            var reportPayload = new ReportRenderPayload
            {
                SampleId = sample.ID,
                SampleNo = sample.SampleNo,
                ReportNo = reportHeader.ReportNo,
                CertificateNo = reportHeader.CertificateNo
            };

            foreach (var testHeader in testHeaders)
            {
                var testPayload = new TestReportPayload
                {
                    TestName = testHeader.LaboratoryTest.Name,

                    Header = new HeaderPayload
                    {
                        CertificateNo = reportHeader.CertificateNo,
                        ReportNo = reportHeader.ReportNo,
                        DateOfIssue = DateTime.UtcNow.ToString("dd-MM-yyyy"),

                        SampleId = sample.ID,
                        SampleNo = sample.SampleNo,

                        TestName = testHeader.LaboratoryTest.Name,
                        TestMethod = "A Grade",

                        CustomerName = sample.SampleInward.Customer.Name,
                        CustomerAddress = sample.SampleInward.Customer.Address,

                        SampleReceivedOn = sample.SampleInward.CollectionTime.ToString("dd-MM-yyyy"),
                        TestPerformedAt = "DMSPL, Ahmedabad"
                    }
                };

                // ---------------- Customer ----------------
                testPayload.CustomerDetails = new KeyValueTablePayload
                {
                    Title = "Customer Details",
                    Rows = new List<KeyValueRow>
                    {
                        new() { Label = "Customer Name", Value = sample.SampleInward.Customer.Name },
                        new() { Label = "Customer Address", Value = sample.SampleInward.Customer.Address }
                    }
                };

                // ---------------- Sample ----------------
                testPayload.SampleDetails = new KeyValueTablePayload
                {
                    Title = "Sample Details",
                    Rows = new List<KeyValueRow>
                    {
                        new() { Label = "Sample No", Value = sample.SampleNo },
                        new() { Label = "Case No", Value = sample.SampleInward.CaseNo },
                        new() { Label = "Material", Value = sample.MetalClassification?.Name },
                        new() { Label = "Condition", Value = sample.ProductCondition?.Name }
                    }
                };

                // ---------------- Result Table ----------------
                if (testHeader.Parameters.Any())
                {
                    testPayload.ResultTable = new TablePayload
                    {
                        Title = "Test Results",
                        Columns = new[] { "Parameter", "Result", "Unit", "Min", "Max" },
                        Rows = testHeader.Parameters.Select(p => new TableRowPayload
                        {
                            Parameter = p.ParameterName,
                            Result = p.Value?.ToString(),
                            Unit = p.Unit,
                            Min = p.MinValue?.ToString(),
                            Max = p.MaxValue?.ToString()
                        }).ToList()
                    };
                }

                // ---------------- Observation ----------------
                var remarks = testHeader.Parameters
                    .Where(p => !string.IsNullOrWhiteSpace(p.Remarks))
                    .ToList();

                if (remarks.Any())
                {
                    testPayload.Observation = new ObservationPayload
                    {
                        Title = "Observation",
                        Fields = remarks.Select(p => new ObservationField
                        {
                            Label = p.ParameterName,
                            Value = p.Remarks
                        }).ToList()
                    };
                }

                // ---------------- Statement ----------------
                if (testHeader.Parameters.Any(p => p.MinValue != null || p.MaxValue != null))
                {
                    testPayload.Statement = new StatementPayload
                    {
                        Text = testHeader.Parameters.All(p => p.IsWithinLimit == true)
                            ? "The result meets the specified requirements."
                            : "The result does not meet the specified requirements."
                    };
                }

                // ---------------- Long Term ----------------
                var longTerm = testHeader.LongTermTests
                    ?.FirstOrDefault(x => x.Status == "Completed");

                if (longTerm != null && longTerm.Records.Any())
                {
                    testPayload.LongTerm = BuildLongTermPayload(longTerm);
                }

                if (testHeader.Images != null && testHeader.Images.Any())
                {
                    testPayload.Images = new ImageGalleryPayload
                    {
                        Title = "Test Images",
                        Images = testHeader.Images.Select(img => new ImagePayload
                        {
                            Url = img.FilePath,   // or img.ImageUrl
                            Caption = img.Caption
                        }).ToList()
                    };
                }

                reportPayload.Tests.Add(testPayload);
            }

            // -------------------------------------------------
            // 5️⃣ PAYLOAD → BLOCKS
            // -------------------------------------------------
            var blocks = new List<ReportBlock>();
            bool firstTest = true;

            foreach (var test in reportPayload.Tests)
            {
                var testBlocks = _reportBlockGenerator
                    .GenerateBlocksFromPayload(test, pageBreak: !firstTest);

                blocks.AddRange(testBlocks);
                firstTest = false;
            }

            // -------------------------------------------------
            // 6️⃣ SAVE REPORT
            // -------------------------------------------------
            int nextVersion = await GetNextReportVersionAsync(reportHeader.ID);

            var report = new Report
            {
                ReportHeaderID = reportHeader.ID,
                ReportNo = reportHeader.ReportNo,
                CertificateNo = reportHeader.CertificateNo,
                Version = nextVersion,
                Status = "Final",
                GeneratedBy = loggedInUser.EmployeeID.ToString(),
                GeneratedAt = DateTime.UtcNow,
                SnapshotJson = JsonSerializer.Serialize(reportPayload)
            };

            int sortOrder = 0;
            foreach (var block in blocks)
            {
                block.SortOrder = sortOrder++;
                report.Blocks.Add(block);
            }

            _db.Reports.Add(report);

            reportHeader.Status = "Report Generated";
            reportHeader.GeneratedBy = await _db.EmployeeMasters
                .Where(x => x.ID == loggedInUser.EmployeeID)
                .Select(x => x.Name)
                .FirstOrDefaultAsync();

            reportHeader.GeneratedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            return report.ID;
        }

        private LongTermMatrixPayload BuildLongTermPayload(LongTermTest longTerm)
        {
            return new LongTermMatrixPayload
            {
                ShowAverage = false,
                Tests = new List<LongTermTestPayload>
                    {
                        new LongTermTestPayload
                        {
                            TestName = longTerm.TestResultHeader.LaboratoryTest.Name,

                            Rows = longTerm.TestResultHeader.Parameters
                                .Select(p => new LongTermRowPayload
                                {
                                    Parameter = p.ParameterName,

                                    Values = longTerm.Records
                                        .OrderBy(r => r.RecordedAt)
                                        .Select(r => GetValueFromRecord(r, p.ParameterName))
                                        .ToList<object>(),

                                    Average = null // future use
                                })
                                .ToList()
                        }
                    }
            };
        }
        private string? GetValueFromRecord(LongTermRecord record, string parameterName)
        {
            if (string.IsNullOrWhiteSpace(record.DataJson) || string.IsNullOrWhiteSpace(parameterName))
                return null;

            var parsed = SafeParseLongTermJson(record.DataJson);
            if (parsed == null || string.IsNullOrWhiteSpace(parsed.ParameterName))
                return null;

            string parsedKey = parsed.ParameterName.Trim();
            string paramKey = parameterName.Trim();

            // Take starting 2 characters safely
            parsedKey = parsedKey.Length >= 2 ? parsedKey[..2] : parsedKey;
            paramKey = paramKey.Length >= 2 ? paramKey[..2] : paramKey;

            if (string.Equals(parsedKey, paramKey, StringComparison.OrdinalIgnoreCase))
            {
                return parsed.Value?.ToString();
            }

            return null;
        }


        // =====================================================
        // 🔥 BLOCK BUILDER (CORE IDEA)
        // =====================================================
        private async Task<List<ReportBlockReadDto>> BuildBlocksAsync(
            List<TestResultHeader> testHeaders,
            SampleDetail sample,
            string reportNo,
            string? certificateNo)
        {
            var result = new List<ReportBlockReadDto>();

            //foreach (var header in testHeaders)
            //{
            //    var template = await ResolveTemplateAsync(header.LaboratoryTestID);

            //    var blocks = await _reportBlockGenerator.GenerateBlocksAsync(
            //        template,
            //        header,
            //        sample,
            //        reportNo,
            //        certificateNo);

            //    foreach (var block in blocks)
            //    {
            //        result.Add(new ReportBlockReadDto
            //        {
            //            BlockType = block.BlockType,
            //            PayloadJson = block.PayloadJson
            //        });
            //    }
            //}

            return result;
        }

        // =====================================================
        // HELPERS
        // =====================================================


        private async Task<List<TestResultHeader>> LoadTestResultHeaders(long sampleId)
        {
            return await _db.TestResultHeaders
                .Include(h => h.Parameters)
                .Include(h => h.Images)
                .Include(h => h.LaboratoryTest)
                .Where(h => h.SampleID == sampleId)
                .OrderBy(h => h.LaboratoryTest.Name)
                .ToListAsync();
        }

        private async Task<ReportHeader> GetOrCreateReportHeaderAsync(
            SampleDetail sample)
        {
            var header = await _db.ReportHeaders
                .FirstOrDefaultAsync(h => h.SampleID == sample.ID);

            if (header != null)
            {
                if (string.IsNullOrWhiteSpace(header.ReportNo))
                    header.ReportNo = await GenerateReportNoAsync();
                if (string.IsNullOrWhiteSpace(header.CertificateNo))
                    header.CertificateNo = await GenerateCertificateNoAsync();
                return header;
            }

            header = new ReportHeader
            {
                SampleID = sample.ID,
                ReportNo = await GenerateReportNoAsync(),
                CertificateNo = await GenerateCertificateNoAsync(),
                GeneratedAt = DateTime.UtcNow,
                Status = "Report Generated"
            };

            _db.ReportHeaders.Add(header);
            await _db.SaveChangesAsync();

            return header;
        }

        private async Task<int> GetNextReportVersionAsync(long reportHeaderId)
        {
            var max = await _db.Reports
                .Where(r => r.ReportHeaderID == reportHeaderId)
                .Select(r => (int?)r.Version)
                .MaxAsync();

            return (max ?? 0) + 1;
        }

        private async Task<ReportTemplate> ResolveTemplateAsync(long labTestId)
        {
            return await _db.ReportTemplates
                .Include(t => t.Blocks)
                .FirstOrDefaultAsync(t => t.TestTypeID == labTestId && t.IsActive)
            ?? await _db.ReportTemplates
                .Include(t => t.Blocks)
                .FirstOrDefaultAsync(t => t.IsDefault && t.IsActive);
        }

        private async Task<string> GenerateReportNoAsync()
        {
            var config = await _db.NumberingConfigs
                .FirstOrDefaultAsync(n => n.ModuleName == "REPORT" && n.CompanyCode == loggedInUser.CompanyCode);

            if (config != null)
            {
                config.CurrentNumber++;
                return $"{config.Prefix}-{DateTime.UtcNow:yyyyMMdd}-{config.CurrentNumber:D4}";
            }
            return $"RPT-{DateTime.UtcNow:yyyyMMddHHmmss}";
        }

        private async Task<string> GenerateCertificateNoAsync()
        {
            var config = await _db.NumberingConfigs
                .FirstOrDefaultAsync(n => n.ModuleName == "TC" && n.CompanyCode == loggedInUser.CompanyCode);

            var year = DateTime.UtcNow.Year.ToString().Substring(2);
            if (config != null)
            {
                config.CurrentNumber++;
                return $"{config.Prefix}-{year}-{config.CurrentNumber:D6}-0";
            }
            return $"TC-{year}-{DateTime.UtcNow:HHmmss}-0";
        }

        private async Task<string?> GetUlrForSample(long sampleId)
        {
            // Fetch ULR from test plan's GeneralTestMethod or ChemicalTest
            var ulr = await _db.Set<Models.GeneralTestMethod>()
                .Where(m => m.GeneralTest.SampleTestPlan.SampleID == sampleId
                    && !string.IsNullOrEmpty(m.UlrNo))
                .Select(m => m.UlrNo)
                .FirstOrDefaultAsync();

            if (!string.IsNullOrEmpty(ulr)) return ulr;

            ulr = await _db.Set<Models.ChemicalTest>()
                .Where(c => c.SampleTestPlan.SampleID == sampleId
                    && !string.IsNullOrEmpty(c.UlrNo))
                .Select(c => c.UlrNo)
                .FirstOrDefaultAsync();

            return ulr;
        }

        /// <summary>
        /// Returns only formats relevant to this report's actual test data.
        /// </summary>
        public async Task<List<object>> GetAvailableFormatsForReportAsync(long reportHeaderId)
        {
            // Find sample — try by ReportHeader ID first, then by SampleID
            var reportHeader = await _db.ReportHeaders.FirstOrDefaultAsync(r => r.ID == reportHeaderId);
            if (reportHeader == null)
                reportHeader = await _db.ReportHeaders.FirstOrDefaultAsync(r => r.SampleID == reportHeaderId && r.IsActive);

            if (reportHeader == null) return Reporting.ReportDocumentFactory.GetAvailableFormats()
                .Select(f => (object)new { f.FormatType, f.DisplayName, f.Description }).ToList();

            var testHeaders = await _db.TestResultHeaders
                .Include(h => h.LaboratoryTest)
                    .ThenInclude(lt => lt.LabDepartment)
                .Where(h => h.SampleID == reportHeader.SampleID && h.IsActive)
                .ToListAsync();

            bool hasChemical = testHeaders.Any(h => DetermineTestType(h) == "Chemical");
            bool hasMechanical = testHeaders.Any(h => DetermineTestType(h) == "General");
            bool hasImages = await _db.TestResultImages.AnyAsync(img =>
                testHeaders.Select(h => h.ID).Contains(img.TestResultHeaderID));

            var formats = new List<object>();

            // Always available
            if (hasChemical || hasMechanical)
                formats.Add(new { FormatType = (int)Helpers.Enums.ReportFormatType.TestCertificate, DisplayName = "Test Certificate (MTC)", Description = "Complete test certificate with all sections" });

            if (hasChemical)
                formats.Add(new { FormatType = (int)Helpers.Enums.ReportFormatType.ChemicalAnalysis, DisplayName = "Chemical Analysis Report", Description = "Chemical composition analysis only" });

            if (hasMechanical)
                formats.Add(new { FormatType = (int)Helpers.Enums.ReportFormatType.MechanicalTest, DisplayName = "Mechanical Test Report", Description = "Mechanical properties only" });

            if (hasChemical && hasMechanical)
                formats.Add(new { FormatType = (int)Helpers.Enums.ReportFormatType.FullTestReport, DisplayName = "Full Test Report", Description = "All test sections combined" });

            formats.Add(new { FormatType = (int)Helpers.Enums.ReportFormatType.CertificateOfConformance, DisplayName = "Certificate of Conformance (COC)", Description = "Pass/Fail conformance statement only" });

            if (hasImages)
                formats.Add(new { FormatType = (int)Helpers.Enums.ReportFormatType.Metallography, DisplayName = "Metallography Report", Description = "Test images + microstructure analysis" });

            return formats;
        }

        /// <summary>
        /// Public report verification — returns basic info for QR code scan (no auth required).
        /// </summary>
        public async Task<object?> GetReportVerificationAsync(string reportNo)
        {
            var reportHeader = await _db.ReportHeaders
                .Include(r => r.Sample)
                    .ThenInclude(s => s.SampleInward)
                        .ThenInclude(i => i.Customer)
                .Include(r => r.Sample)
                    .ThenInclude(s => s.MetalClassification)
                .FirstOrDefaultAsync(r => r.ReportNo == reportNo && r.IsActive);

            if (reportHeader == null) return null;

            var sample = reportHeader.Sample;
            var inward = sample?.SampleInward;
            var org = await _db.Organizations.FirstOrDefaultAsync(o => o.IsActive);

            // Test summary — pass/fail per test
            var testHeaders = await _db.TestResultHeaders
                .Include(h => h.Parameters)
                .Include(h => h.LaboratoryTest)
                .Where(h => h.SampleID == sample!.ID && h.IsActive)
                .ToListAsync();

            var testDetails = testHeaders.Select(h => new
            {
                testName = h.LaboratoryTest?.Name ?? "Test",
                parameterCount = h.Parameters.Count,
                allPassed = h.Parameters.All(p => p.IsWithinLimit != false),
                status = h.Status,
                parameters = h.Parameters.OrderBy(p => p.ID).Select(p => new
                {
                    name = p.ParameterName,
                    unit = p.Unit,
                    specMin = FormatDecimal(p.SpecMinValue ?? p.MinValue, p.DecimalPrecision),
                    specMax = FormatDecimal(p.SpecMaxValue ?? p.MaxValue, p.DecimalPrecision),
                    result = FormatDecimal(p.Value, p.DecimalPrecision),
                    status = p.IsWithinLimit == true ? "Pass" : p.IsWithinLimit == false ? "Fail" : "N/A",
                    remarks = p.Remarks
                }).ToList()
            }).ToList();

            return new
            {
                reportNo = reportHeader.ReportNo,
                certificateNo = reportHeader.CertificateNo,
                sampleNo = sample?.SampleNo,
                caseNo = inward?.CaseNo,
                customerName = inward?.Customer?.Name,
                customerAddress = inward?.Customer?.Address,
                material = sample?.MetalClassification?.Name,
                condition = sample?.ProductCondition?.Name,
                sampleDescription = sample?.Details,
                dateOfIssue = reportHeader.GeneratedAt.ToString("dd-MM-yyyy"),
                dateReceived = inward?.CollectionTime.ToString("dd-MM-yyyy"),
                status = reportHeader.Status,
                labName = org?.LabName,
                labAddress = org?.LabAddress,
                labPhone = org?.ContactPhone,
                labEmail = org?.ContactEmail,
                labCIN = org?.CIN,
                pdfPath = reportHeader.PdfPath,
                nablCertNo = (await _db.NablAccreditations
                    .Where(n => n.IsActive && n.OrganizationId == (org != null ? org.Id : 0))
                    .Select(n => n.CertificateNumber)
                    .FirstOrDefaultAsync()),
                testDetails
            };
        }

        public async Task RequestAmendmentAsync(long reportHeaderId, string reason, IFormFile file)
        {
            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                var header = await _db.ReportHeaders
                    .FirstOrDefaultAsync(h => h.ID == reportHeaderId);

                if (header == null)
                    throw new InvalidOperationException("Report not found.");

                if (header.Status != "Report Generated")
                    throw new InvalidOperationException(
                        "Amendment allowed only on generated reports.");

                // Fix 3A — Guard: amendment workflow must be configured before creating amendment record
                var amendmentEntityType = WorkFlowEntityTypeExtensions.GetEntityType(WorkFlowEntityType.Report_Amendment);
                if (!await _workflowService.WorkflowExistsForEntityType(amendmentEntityType))
                    throw new InvalidOperationException(
                        "Cannot request amendment: No 'Report Amendment' workflow is configured. Contact administrator.");

                // -------------------------------------------------
                // 1️⃣ Upload Supporting Document
                // -------------------------------------------------
                if (file == null)
                    throw new InvalidOperationException("Supporting document is required.");

                var uploadResult = await fileUploadService.UploadFileAsync(file, FileType.Report, null, "ReportAmend");

                // -------------------------------------------------
                // 2️⃣ Create Amendment Request
                // -------------------------------------------------
                var amendment = new AmendmentRequest
                {
                    ReportHeaderID = reportHeaderId,
                    Reason = reason,
                    Status = "Pending",

                    FilePath = uploadResult.FilePath,
                    FileName = uploadResult.OriginalFileName,
                    UploadReferenceID = uploadResult.ID,

                    CreatedBy = loggedInUser.EmployeeID,
                    CreatedOn = DateTime.UtcNow
                };

                _db.AmendmentRequests.Add(amendment);

                // -------------------------------------------------
                // 3️⃣ Update Report Header Status
                // -------------------------------------------------
                header.Status = "Under Amendment Review";

                await _db.SaveChangesAsync();
                // -------------------------------------------------
                // 4️⃣ Start Workflow (NEW ENTITY TYPE)
                // -------------------------------------------------
                await _workflowService.StartWorkflow(amendment.ID, WorkFlowEntityTypeExtensions.GetEntityType(WorkFlowEntityType.Report_Amendment));

                await _sampleStatusService.ForceAutoStatusAsync(header.SampleID, SampleStatus.REPORT_AMENDED_BY_INTERNAL, loggedInUser.EmployeeID);
                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        // =====================================================
        // BUILD REPORT DATA DTO (joins all entities)
        // =====================================================
        public async Task<ReportDataDto> BuildReportDataAsync(long reportHeaderId)
        {
            // 1. Load report header with sample chain
            var reportHeader = await _db.ReportHeaders
                .Include(r => r.Reports)
                .Include(r => r.Sample)
                    .ThenInclude(s => s.SampleInward)
                        .ThenInclude(i => i.Customer)
                .Include(r => r.Sample)
                    .ThenInclude(s => s.MetalClassification)
                .Include(r => r.Sample)
                    .ThenInclude(s => s.ProductCondition)
                        .ThenInclude(pc => pc.LinkedHeatTreatment)
                .Include(r => r.Sample)
                    .ThenInclude(s => s.SpecimenOrientation)
                .Include(r => r.Sample)
                    .ThenInclude(s => s.ProductForm)
                .FirstOrDefaultAsync(r => r.ID == reportHeaderId)
                ?? throw new InvalidOperationException("Report header not found.");

            var sample = reportHeader.Sample
                ?? throw new InvalidOperationException("Sample not found for report header.");
            var inward = sample.SampleInward
                ?? throw new InvalidOperationException("Sample inward not found.");
            var customer = inward.Customer
                ?? throw new InvalidOperationException("Customer not found.");

            // 2. Load test result headers with parameters + images
            var testHeaders = await _db.TestResultHeaders
                .Include(h => h.Parameters)
                .Include(h => h.Images)
                .Include(h => h.LaboratoryTest)
                    .ThenInclude(lt => lt.LabDepartment)
                .Where(h => h.SampleID == sample.ID)
                .OrderBy(h => h.LaboratoryTest.Name)
                .ToListAsync();

            // 3. Determine tested/completed dates
            var earliestStarted = testHeaders
                .Where(h => h.StartedAt.HasValue)
                .Select(h => h.StartedAt!.Value)
                .DefaultIfEmpty(DateTime.UtcNow)
                .Min();

            var latestCompleted = testHeaders
                .Where(h => h.CompletedAt.HasValue)
                .Select(h => h.CompletedAt!.Value)
                .DefaultIfEmpty(DateTime.UtcNow)
                .Max();

            // 4. Build test sections
            var testSections = new List<ReportDataTestSection>();

            // Group by LabTestID to detect multi-specimen tests
            var labTestGroups = testHeaders.GroupBy(h => h.LaboratoryTestID)
                .ToDictionary(g => g.Key, g => g.Count());

            foreach (var header in testHeaders)
            {
                var testType = DetermineTestType(header);
                var baseName = header.LaboratoryTest?.Name ?? "Unknown Test";

                // Add specimen label when multiple headers share same lab test
                var hasMultipleSpecimens = labTestGroups.GetValueOrDefault(header.LaboratoryTestID, 1) > 1;
                var testName = hasMultipleSpecimens
                    ? $"{baseName} - Specimen {header.SequenceNo}"
                    : baseName;

                var section = new ReportDataTestSection
                {
                    TestResultHeaderId = header.ID,
                    TestName = testName,
                    TestType = testType,
                    TestCategory = DetermineTestCategory(header, testType),
                    SpecificationName = header.LaboratoryTest?.Name,
                    TestMethod = header.Parameters
                        .Select(p => p.TestMethodUsed)
                        .FirstOrDefault(m => !string.IsNullOrEmpty(m))
                        ?? header.LaboratoryTest?.Name,
                    DateOfTesting = header.CompletedAt?.ToString("dd-MM-yyyy") ?? "",
                    Parameters = header.Parameters
                        .OrderBy(p => p.ID)
                        .Select(p => new ReportDataParameter
                        {
                            Name = p.ParameterName,
                            Unit = p.Unit,
                            SpecMin = FormatDecimal(p.SpecMinValue ?? p.MinValue, p.DecimalPrecision),
                            SpecMax = FormatDecimal(p.SpecMaxValue ?? p.MaxValue, p.DecimalPrecision),
                            Result = FormatDecimal(p.Value, p.DecimalPrecision),
                            Status = p.IsWithinLimit == true ? "Pass"
                                   : p.IsWithinLimit == false ? "Fail"
                                   : "N/A",
                            IsWithinNablScope = p.IsWithinNablScope,
                            NablScopeStatus = p.NablScopeStatus,
                            ExpandedUncertainty = p.ExpandedUncertainty,
                            CoverageFactor = p.CoverageFactor,
                            SubGroup = testType == "Chemical"
                                ? (_db.ChemicalTests
                                    .Where(ct => ct.SampleTestPlanID == header.TestPlanID)
                                    .Include(ct => ct.AnalysisType)
                                        .ThenInclude(at => at.SubGroup)
                                    .Select(ct => ct.AnalysisType != null && ct.AnalysisType.SubGroup != null ? ct.AnalysisType.SubGroup.ReportTestName : null)
                                    .FirstOrDefault() ?? header.LaboratoryTest.Name)
                                : null
                        })
                        .ToList(),
                    Images = header.Images
                        .OrderBy(img => img.SortOrder)
                        .Select(img => new ReportDataImage
                        {
                            Url = img.FilePath,
                            Caption = img.Caption
                        })
                        .ToList()
                };

                testSections.Add(section);
            }

            // 5. Collect all remarks
            var allRemarks = testHeaders
                .SelectMany(h => h.Parameters)
                .Where(p => !string.IsNullOrWhiteSpace(p.Remarks))
                .Select(p => $"{p.ParameterName}: {p.Remarks}")
                .ToList();

            // 6. Resolve signatory names
            var generatedByName = await ResolveEmployeeName(reportHeader.GeneratedBy);

            // 6b. Resolve signatories from DB (fallback to defaults if not configured)
            var signatories = await _db.AuthorizedSignatories
                .Where(s => s.IsActive && s.ApplicableFor && s.CompanyCode == loggedInUser.CompanyCode)
                .OrderBy(s => s.Id)
                .Take(3)
                .ToListAsync();

            var testedBy = signatories.ElementAtOrDefault(0);
            var reviewedBy = signatories.ElementAtOrDefault(1);
            var authorizedBy = signatories.ElementAtOrDefault(2);

            // 6c. Fetch Organization info for report header
            var org = await _db.Organizations
                .FirstOrDefaultAsync(o => o.IsActive && o.CompanyCode == loggedInUser.CompanyCode);

            // 6d. Fetch NABL accreditation (for cert number + logo path)
            var nablAccred = org != null
                ? await _db.NablAccreditations
                    .Where(n => n.IsActive && n.OrganizationId == org.Id)
                    .OrderByDescending(n => n.ExpiryDate)
                    .FirstOrDefaultAsync()
                : null;

            // 6e. Fetch configuration values
            var configKeys = new[] { "REPORT_CONDITIONS", "COMPANY_STAMP_PATH" };
            var configs = await _db.Configurations
                .Where(c => configKeys.Contains(c.KeyName) && c.IsActive && c.CompanyCode == loggedInUser.CompanyCode)
                .ToDictionaryAsync(c => c.KeyName, c => c.Value);

            // 6f. Fetch SampleAdditionalDetail for customer-provided info
            var additionalDetails = await _db.SampleAdditionalDetails
                .Where(d => d.SampleID == sample.ID)
                .ToDictionaryAsync(d => d.Label, d => d.Value);

            // 6g. Calculate CrossSectionArea and GaugeLength from dimensions
            decimal? crossSectionArea = null;
            if (sample.Diameter.HasValue && sample.Diameter > 0)
                crossSectionArea = Math.Round((decimal)(Math.PI / 4) * sample.Diameter.Value * sample.Diameter.Value, 4);
            else if (sample.Thickness.HasValue && sample.Width.HasValue && sample.Thickness > 0 && sample.Width > 0)
                crossSectionArea = Math.Round(sample.Thickness.Value * sample.Width.Value, 4);

            decimal? gaugeLength = null;
            if (crossSectionArea.HasValue && crossSectionArea > 0)
                gaugeLength = Math.Round(5.65m * (decimal)Math.Sqrt((double)crossSectionArea.Value), 2);

            // 6h. Resolve equipment names, lab room, and environment from test headers
            var firstCompletedHeader = testHeaders.FirstOrDefault(h => h.CompletedAt.HasValue) ?? testHeaders.FirstOrDefault();
            var equipmentNames = new List<string>();
            foreach (var header in testHeaders.Where(h => h.EquipmentID.HasValue || !string.IsNullOrEmpty(h.EquipmentIdsJson)))
            {
                var eqIds = new List<long>();
                if (header.EquipmentID.HasValue) eqIds.Add(header.EquipmentID.Value);
                if (!string.IsNullOrEmpty(header.EquipmentIdsJson))
                {
                    try
                    {
                        var parsed = System.Text.Json.JsonSerializer.Deserialize<List<long>>(header.EquipmentIdsJson);
                        if (parsed != null) eqIds.AddRange(parsed);
                    }
                    catch { }
                }
                foreach (var eqId in eqIds.Distinct())
                {
                    var eq = await _db.EquipmentMasters.Where(e => e.ID == eqId).Select(e => e.Name).FirstOrDefaultAsync();
                    if (eq != null && !equipmentNames.Contains(eq)) equipmentNames.Add(eq);
                }
            }

            var labRoomName = firstCompletedHeader?.LabRoomId > 0
                ? await _db.LabRooms.Where(r => r.ID == firstCompletedHeader.LabRoomId).Select(r => r.Name).FirstOrDefaultAsync()
                : null;

            // 7. Build DTO
            var dto = new ReportDataDto
            {
                ReportId = reportHeader.Reports.FirstOrDefault()?.ID ?? 0,
                ReportHeaderId = reportHeader.ID,
                ReportNo = reportHeader.ReportNo,
                CertificateNo = reportHeader.CertificateNo,
                ReportDate = reportHeader.GeneratedAt,

                // Lab/Company Identity
                LabName = org?.LabName ?? "Laboratory",
                LabAddress = org?.LabAddress ?? "",
                LabPhone = org?.ContactPhone ?? "",
                LabEmail = org?.ContactEmail ?? "",
                LabLogoPath = org?.OrganizationLogo,
                CIN = org?.CIN,
                NablLogoPath = nablAccred?.LogoPath,
                CompanyStampPath = configs.GetValueOrDefault("COMPANY_STAMP_PATH"),

                // Certificate Identity — ULR from test plan
                UlrNo = await GetUlrForSample(sample.ID),
                DateOfIssue = reportHeader.GeneratedAt.ToString("dd-MM-yyyy"),
                SampleReceivedDate = inward.CollectionTime.ToString("dd-MM-yyyy"),
                TestPerformedAt = org?.LabName ?? "Laboratory",

                CustomerName = customer.Name,
                CustomerAddress = customer.Address,
                CustomerGST = customer.GSTNo ?? "",

                // Customer Provided Info
                CustomerReference = additionalDetails.GetValueOrDefault("Reference"),
                StampedAs = additionalDetails.GetValueOrDefault("StampedAs"),
                NatureOfSample = additionalDetails.GetValueOrDefault("NatureOfSample") ?? sample.Details,
                SampleDrawnBy = additionalDetails.GetValueOrDefault("SampleDrawnBy"),

                CaseNo = inward.CaseNo,
                SampleNo = sample.SampleNo,
                SampleDescription = sample.Details,
                MaterialSpec = sample.MetalClassification?.Name ?? "",
                Grade = sample.ProductCondition?.Name ?? "",
                ProductForm = sample.ProductForm?.Name,
                SpecimenOrientation = sample.SpecimenOrientation?.Name,
                HeatTreatment = sample.ProductCondition?.LinkedHeatTreatment?.Name,
                HeatNo = additionalDetails.GetValueOrDefault("Heat No"),
                BatchNo = additionalDetails.GetValueOrDefault("Batch No"),
                Quantity = sample.Quantity,

                Thickness = sample.Thickness,
                Diameter = sample.Diameter,
                Width = sample.Width,
                Length = sample.Length,
                CrossSectionArea = crossSectionArea,
                GaugeLength = gaugeLength,

                // Test Conditions
                RoomTemperature = firstCompletedHeader?.RoomTemperature,
                RoomHumidity = firstCompletedHeader?.RoomHumidity,
                EquipmentUsed = equipmentNames.Any() ? string.Join(", ", equipmentNames) : null,
                LabRoom = labRoomName,

                DateReceived = inward.CollectionTime.ToString("dd-MM-yyyy"),
                DateTested = latestCompleted.ToString("dd-MM-yyyy"),
                DateReported = reportHeader.GeneratedAt.ToString("dd-MM-yyyy"),

                // Footer Conditions
                ReportConditions = configs.GetValueOrDefault("REPORT_CONDITIONS")
                    ?.Split('\n', StringSplitOptions.RemoveEmptyEntries)
                    .ToList() ?? new List<string>(),

                TestSections = testSections,

                Remarks = allRemarks.Any() ? string.Join("\n", allRemarks) : null,

                TestedByName = testedBy?.Name ?? generatedByName ?? "Lab Analyst",
                TestedByDesignation = testedBy?.Designation ?? "Lab Analyst",
                TestedBySignaturePath = testedBy?.SignaturePath ?? "Assets/signature.png",

                ReviewedByName = reviewedBy?.Name ?? "Technical Manager",
                ReviewedByDesignation = reviewedBy?.Designation ?? "Technical Manager",
                ReviewedBySignaturePath = reviewedBy?.SignaturePath ?? "Assets/signature.png",

                AuthorizedByName = authorizedBy?.Name ?? "Authorized Signatory",
                AuthorizedByDesignation = authorizedBy?.Designation ?? "Director",
                AuthorizedBySignaturePath = authorizedBy?.SignaturePath ?? "Assets/signature.png",

                QrCodeData = $"{_config["PublicBaseUrl"]}/report/verify/{reportHeader.ReportNo}",

                IsNabl = testHeaders.All(h => h.IsNabl),
                NablCertNo = nablAccred?.CertificateNumber
            };

            // Build NABL scope info for report
            var outOfScopeParams = testSections
                .SelectMany(s => s.Parameters)
                .Where(p => p.NablScopeStatus == "OutsideScope")
                .Select(p => p.Name)
                .Distinct()
                .ToList();

            dto.NablInfo = new NablReportInfo
            {
                IsPartialScope = outOfScopeParams.Any() && testSections.SelectMany(s => s.Parameters).Any(p => p.NablScopeStatus == "WithinScope"),
                OutOfScopeParameterNames = outOfScopeParams
            };

            // ── Conformity Assessment ──
            dto.StatementOfConformity = inward.StatementOfConformity;
            dto.DecisionRule = inward.DecisionRule;

            if (inward.StatementOfConformity == "Applicable")
            {
                foreach (var section in dto.TestSections)
                {
                    foreach (var param in section.Parameters)
                    {
                        if (param.Status == "Pass")
                            param.ConformityResult = "Conforms";
                        else if (param.Status == "Fail")
                            param.ConformityResult = "Does not conform";
                    }
                }
            }

            return dto;
        }

        // =====================================================
        // GENERATE ENHANCED PDF
        // =====================================================
        public async Task<string> GenerateEnhancedPdfAsync(long reportHeaderId)
        {
            // 1. Build report data
            var reportData = await BuildReportDataAsync(reportHeaderId);

            // 2. Generate PDF
            var fileName = $"report_{reportData.ReportNo}_{DateTime.UtcNow:yyyyMMddHHmmss}.pdf";
            var filePath = fileUploadService.GetFilePath(fileName, FileType.Report, null);

            var document = new EnhancedReportDocument(reportData);
            document.GeneratePdf(filePath);

            // 3. Update report header with PDF path
            var reportHeader = await _db.ReportHeaders
                .FirstOrDefaultAsync(r => r.ID == reportHeaderId);

            if (reportHeader != null)
            {
                reportHeader.PdfPath = filePath;
                await _db.SaveChangesAsync();
            }

            return filePath;
        }

        // =====================================================
        // PRIVATE HELPERS for BuildReportData
        // =====================================================

        /// <summary>
        /// Determines if a test is "Chemical" or "General" based on the lab department or test name.
        /// </summary>
        private static string DetermineTestType(TestResultHeader header)
        {
            var testName = header.LaboratoryTest?.Name?.ToLower() ?? "";
            var deptName = header.LaboratoryTest?.LabDepartment?.Name?.ToLower() ?? "";

            if (testName.Contains("chemical") || testName.Contains("composition") ||
                deptName.Contains("chemical") || deptName.Contains("spectrometer"))
                return "Chemical";

            return "General";
        }

        /// <summary>Format decimal: removes trailing zeros (545.0000 → 545, 600.50 → 600.5)</summary>
        private static string? FormatDecimal(decimal? value, int decimalPrecision = 2)
        {
            if (!value.HasValue) return null;
            return value.Value.ToString($"F{decimalPrecision}");
        }

        private static string DetermineTestCategory(TestResultHeader header, string testType)
        {
            if (testType == "Chemical") return "CHEMICAL";

            var deptName = header.LaboratoryTest?.LabDepartment?.Name?.ToUpper() ?? "";
            if (!string.IsNullOrEmpty(deptName)) return $"{deptName} + METALS & ALLOYS";

            return "MECHANICAL + METALS & ALLOYS";
        }

        private async Task<string?> ResolveEmployeeName(string? employeeIdStr)
        {
            if (string.IsNullOrWhiteSpace(employeeIdStr))
                return null;

            if (!long.TryParse(employeeIdStr, out var empId))
                return employeeIdStr; // already a name

            return await _db.EmployeeMasters
                .Where(e => e.ID == empId)
                .Select(e => e.Name)
                .FirstOrDefaultAsync();
        }

        // =====================================================
        // GENERATE REPORT BY FORMAT TYPE
        // =====================================================
        public async Task<byte[]> GenerateReportByFormatAsync(long reportHeaderId, Helpers.Enums.ReportFormatType formatType, string? watermark = null)
        {
            var reportData = await BuildReportDataAsync(reportHeaderId);

            // Filter test sections based on format type
            if (formatType == Helpers.Enums.ReportFormatType.MechanicalTest)
                reportData.TestSections = reportData.TestSections.Where(s => s.TestType == "General").ToList();
            else if (formatType == Helpers.Enums.ReportFormatType.ChemicalAnalysis)
                reportData.TestSections = reportData.TestSections.Where(s => s.TestType == "Chemical").ToList();

            var document = Reporting.ReportDocumentFactory.Create(formatType, reportData, watermark);
            return document.GeneratePdf();
        }
    }

}
