// LIMSApi/ServiceWORepo/ReportingService.cs
using System.Linq.Dynamic.Core;
using System.Text.Json;
using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Helpers.Enums;
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

            // ----------------------------------------------------------
            // BASE QUERY
            // Report → Sample → Inward → Workflow
            // ----------------------------------------------------------
            var query = from report in _db.ReportHeaders

                        join sample in _db.SampleDetails
                            on report.SampleID equals sample.ID

                        join inward in _db.SampleInwards
                            on sample.InwardID equals inward.ID

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
                                EntityID = amendment != null ? amendment.ID : report.ID,
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

                        where report.IsActive
                              && inward.CompanyCode == loggedInUser.CompanyCode

                        select new
                        {
                            ReportHeaderId = report.ID,
                            AmendmentRequestId = amendment != null ? amendment.ID : 0,
                            sampleId = sample.ID,
                            sample.SampleNo,
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

                            report.ReportNo,
                            report.PdfPath,
                            report.Status,

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
                var search = filter.searchTerm.Trim().ToLower();

                query = query.Where(x =>
                    x.SampleNo.ToLower().Contains(search) ||
                    x.CaseNo.ToLower().Contains(search) ||
                    x.Customer.ToLower().Contains(search) ||
                    x.Material.ToLower().Contains(search) ||
                    x.Condition.ToLower().Contains(search) ||
                    x.ReportNo.ToLower().Contains(search)
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
                x.SampleNo,
                x.CaseNo,
                x.Customer,
                x.CustomerID,
                x.Material,
                x.Condition,
                Status =
    x.Status == "Under Amendment Review"
        ? "Under Amendment Review"
        : x.CanTakeAction
            ? x.WorkflowStatus
            : x.Status,

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
            var reportHeader = await _db.ReportHeaders
                .Include(r => r.Sample)
                    .ThenInclude(s => s.SampleInward)
                        .ThenInclude(i => i.Customer)
                .FirstOrDefaultAsync(r => r.ID == reportHeaderId);

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
                    ltt.TestResultHeader?.LaboratoryTest?.SubGroup
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

                MechanicalTests = mechanicalTests,
                ChemicalTests = chemicalTests,
                LongTermTests = longTermDtos,

                Actions = canTakeAction ? actions : new List<ReportActionDto>(),
                Status = reportHeader.Status == "Under Amendment Review" ? "Under Amendment Review" : reportHeader.Status == "Pending" ? "Pending for Approval" : reportHeader.Status,
                Amendment = amendmentDto
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
            return new ReportTestDto
            {
                TestResultHeaderId = test.headerId,
                TestName = test.laboratoryTest,
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
                        MinValue = p.minValue,
                        MaxValue = p.maxValue,
                        IsWithinLimit = p.minValue != null && p.maxValue != null
                            ? (p.value >= p.minValue && p.value <= p.maxValue)
                            : true,
                        Remarks = p.Remarks
                    }).ToList()
            };
        }

        public async Task<string> GeneratePdfForSampleAsync(long sampleId)
        {
            // -------------------------------------------------
            // 1️⃣ Get Report for Sample
            // -------------------------------------------------
            var reportHeader = await _db.ReportHeaders
                .Include(x => x.Sample).ThenInclude(s => s.SampleInward).ThenInclude(c => c.Customer)
                .FirstOrDefaultAsync(r => r.SampleID == sampleId);

            Report report;

            // -------------------------------------------------
            // 2️⃣ If report not generated → generate it
            // -------------------------------------------------
            if (reportHeader == null || reportHeader.Status != "Final")
            {
                // this internally creates Report + Report if needed
                var reportId = await GenerateReportAsync(sampleId);

                report = await _db.Reports
                    .Include(r => r.Blocks)
                    .FirstAsync(r => r.ID == reportId);
            }
            else
            {
                // -------------------------------------------------
                // 3️⃣ Fetch latest final report
                // -------------------------------------------------
                report = await _db.Reports
                    .Include(r => r.Blocks)
                    .Where(r => r.ReportHeaderID == reportHeader.ID
                                && r.Status == "Final")
                    .OrderByDescending(r => r.Version)
                    .FirstOrDefaultAsync()
                    ?? throw new Exception("Final report not found");
            }

            // -------------------------------------------------
            // 4️⃣ Generate PDF (idempotent)
            // -------------------------------------------------
            var pdfPath = await GeneratePdfAsync(report.ID);

            var token = Guid.NewGuid();

            var amendmentToken = new ReportAmendmentToken
            {
                Token = token,
                SampleID = sampleId,
                ReportID = reportHeader.ID,
                LinkExpiryOn = DateTime.UtcNow.AddDays(7),  // secure link
                FreeUntil = DateTime.UtcNow.AddDays(1),
                IsUsed = false,
                CreatedOn = DateTime.UtcNow
            };

            _db.ReportAmendmentTokens.Add(amendmentToken);
            await _db.SaveChangesAsync();

            // -------------------------------------------------
            // 5️⃣ Build Public Amendment Link
            // -------------------------------------------------
            var amendmentLink = GenerateReportLink(token.ToString());

            // -------------------------------------------------
            // 6️⃣ Email Template Model (MATCHES TEMPLATE)
            // -------------------------------------------------
            var emailModel = new
            {
                CustomerName = reportHeader.Sample?.SampleInward?.Customer?.Name,
                ReportNo = reportHeader.ReportNo,
                AmendmentLink = amendmentLink
            };
            var email = reportHeader.Sample?.SampleInward?.Contacts?.FirstOrDefault(x => x.Selected)?.EmailId;
            // -------------------------------------------------
            // 7️⃣ Send Email with PDF + Link
            // -------------------------------------------------
            var body = await _templateService.GetTemplateAsync(MessageTemplateKey.AMENDED_REPORT_READY, NotificationType.Email, emailModel);
            if (email != null)
            {
                await _emailService.SendEmailWithAttachment(
                    toEmail: email,
                    subject: $"Your Test Report {reportHeader.ReportNo}",
                    body: body,
                    attachmentPath: pdfPath,
                    attachmentName: $"Report_{reportHeader.ReportNo}.pdf"
                );
            }

            return pdfPath;
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

                header.ReportNo = string.IsNullOrWhiteSpace(header.ReportNo) ? GenerateReportNo() : header.ReportNo;
                return header;
            }

            header = new ReportHeader
            {
                SampleID = sample.ID,
                ReportNo = GenerateReportNo(),
                CertificateNo = GenerateCertificateNo(sample.ID),
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

        private static string GenerateReportNo()
            => $"R-{DateTime.UtcNow:yyyyMMddHHmmss}";

        private static string GenerateCertificateNo(long sampleId)
            => $"DMSPL-{DateTime.UtcNow:yy}-{sampleId:D6}-1";

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
    }

}
