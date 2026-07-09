using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Reporting;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;

namespace LIMSApi.ServiceWORepo
{
    public class ReportAutoGenerationService : IReportAutoGenerationService
    {
        private readonly LIMSContext _db;
        private readonly IReportFormatService _formatService;
        private readonly IConfiguration _config;
        private readonly ILogger<ReportAutoGenerationService> _logger;

        public ReportAutoGenerationService(
            LIMSContext db,
            IReportFormatService formatService,
            IConfiguration config,
            ILogger<ReportAutoGenerationService> logger)
        {
            _db = db;
            _formatService = formatService;
            _config = config;
            _logger = logger;
        }

        public async Task GenerateAsync(long sampleId)
        {
            // 1. Resolve format
            var format = await _formatService.ResolveFormatForSampleAsync(sampleId);
            if (format == null)
            {
                _logger.LogWarning("No report format resolved for sample {SampleId}, skipping auto-generation", sampleId);
                return;
            }

            // 2. Build report data
            var reportData = await BuildReportDataForSampleAsync(sampleId);

            // 3. Generate report number
            var reportNo = await GenerateReportNoAsync();
            reportData.ReportNo = reportNo;

            // 4. Generate PDF
            var document = new ConfigDrivenReportDocument(reportData, format);
            var pdfBytes = document.GeneratePdf();

            // 5. Save PDF
            var pdfPath = await SavePdfAsync(pdfBytes, reportNo);

            // 6. Create GeneratedReport record
            var loggedInUser = LoggedInUserProvider.CurrentUser;
            var generated = new GeneratedReport
            {
                ReportFormatID = format.ID,
                SampleID = sampleId,
                ReportNo = reportNo,
                CertificateNo = reportData.CertificateNo,
                Status = "Generated",
                PdfPath = pdfPath,
                GeneratedBy = loggedInUser?.Name ?? "System",
                GeneratedAt = DateTime.UtcNow
            };

            _db.GeneratedReports.Add(generated);
            await _db.SaveChangesAsync();

            _logger.LogInformation("Config-driven report {ReportNo} generated for sample {SampleId} using format {FormatCode}",
                reportNo, sampleId, format.FormatCode);
        }

        public async Task<byte[]> GeneratePreviewAsync(long sampleId, long? formatId = null)
        {
            ReportFormat? format;
            if (formatId.HasValue)
            {
                format = await _db.ReportFormats
                    .Include(f => f.Sections.Where(s => s.IsActive && s.IsVisible))
                    .FirstOrDefaultAsync(f => f.ID == formatId && f.IsActive);
            }
            else
            {
                format = await _formatService.ResolveFormatForSampleAsync(sampleId);
            }

            if (format == null)
                throw new InvalidOperationException("No report format found for preview.");

            var reportData = await BuildReportDataForSampleAsync(sampleId);
            reportData.ReportNo = "PREVIEW";

            var document = new ConfigDrivenReportDocument(reportData, format);
            return document.GeneratePdf();
        }

        // ════════════════════════════════════════════════
        // BUILD REPORT DATA (from sampleId — independent of ReportHeader)
        // Mirrors ReportService.BuildReportDataAsync logic
        // ════════════════════════════════════════════════

        private async Task<ReportDataDto> BuildReportDataForSampleAsync(long sampleId)
        {
            // 1. Load sample with chain
            var sample = await _db.SampleDetails
                .Include(s => s.SampleInward)
                    .ThenInclude(i => i.Customer)
                .Include(s => s.MetalClassification)
                .Include(s => s.ProductCondition)
                    .ThenInclude(pc => pc.LinkedHeatTreatment)
                .Include(s => s.SpecimenOrientation)
                .Include(s => s.ProductForm)
                .FirstOrDefaultAsync(s => s.ID == sampleId)
                ?? throw new InvalidOperationException($"Sample {sampleId} not found.");

            var inward = sample.SampleInward
                ?? throw new InvalidOperationException("Sample inward not found.");
            var customer = inward.Customer
                ?? throw new InvalidOperationException("Customer not found.");

            // 2. Load test result headers
            var testHeaders = await _db.TestResultHeaders
                .Include(h => h.Parameters)
                .Include(h => h.Images)
                .Include(h => h.LaboratoryTest)
                    .ThenInclude(lt => lt.LabDepartment)
                .Where(h => h.SampleID == sampleId && h.IsActive)
                .OrderBy(h => h.LaboratoryTest.Name)
                .ToListAsync();

            // 3. Dates
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
            var labTestGroups = testHeaders.GroupBy(h => h.LaboratoryTestID)
                .ToDictionary(g => g.Key, g => g.Count());

            foreach (var header in testHeaders)
            {
                var testType = DetermineTestType(header);
                var baseName = header.LaboratoryTest?.Name ?? "Unknown Test";
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

            // 5. Remarks
            var allRemarks = testHeaders
                .SelectMany(h => h.Parameters)
                .Where(p => !string.IsNullOrWhiteSpace(p.Remarks))
                .Select(p => $"{p.ParameterName}: {p.Remarks}")
                .ToList();

            // 6. Signatories
            var loggedInUser = LoggedInUserProvider.CurrentUser;
            var companyCode = loggedInUser?.CompanyCode ?? "LIMS";
            var signatories = await _db.AuthorizedSignatories
                .Where(s => s.IsActive && s.ApplicableFor && s.CompanyCode == companyCode)
                .OrderBy(s => s.Id)
                .Take(3)
                .ToListAsync();

            var testedBy = signatories.ElementAtOrDefault(0);
            var reviewedBy = signatories.ElementAtOrDefault(1);
            var authorizedBy = signatories.ElementAtOrDefault(2);

            // 7. Organization
            var org = await _db.Organizations
                .FirstOrDefaultAsync(o => o.IsActive && o.CompanyCode == companyCode);

            // 8. NABL Accreditation
            var nablAccred = org != null
                ? await _db.NablAccreditations
                    .Where(n => n.IsActive && n.OrganizationId == org.Id)
                    .OrderByDescending(n => n.ExpiryDate)
                    .FirstOrDefaultAsync()
                : null;

            // 9. Configuration values
            var configKeys = new[] { "REPORT_CONDITIONS", "COMPANY_STAMP_PATH" };
            var configs = await _db.Configurations
                .Where(c => configKeys.Contains(c.KeyName) && c.IsActive && c.CompanyCode == companyCode)
                .ToDictionaryAsync(c => c.KeyName, c => c.Value);

            // 10. Additional details
            var additionalDetails = await _db.SampleAdditionalDetails
                .Where(d => d.SampleID == sample.ID)
                .ToDictionaryAsync(d => d.Label, d => d.Value);

            // 11. ULR
            var ulr = await GetUlrForSample(sampleId);

            // 12. CrossSectionArea + GaugeLength
            decimal? crossSectionArea = null;
            if (sample.Diameter.HasValue && sample.Diameter > 0)
                crossSectionArea = Math.Round((decimal)(Math.PI / 4) * sample.Diameter.Value * sample.Diameter.Value, 4);
            else if (sample.Thickness.HasValue && sample.Width.HasValue && sample.Thickness > 0 && sample.Width > 0)
                crossSectionArea = Math.Round(sample.Thickness.Value * sample.Width.Value, 4);

            decimal? gaugeLength = null;
            if (crossSectionArea.HasValue && crossSectionArea > 0)
                gaugeLength = Math.Round(5.65m * (decimal)Math.Sqrt((double)crossSectionArea.Value), 2);

            // 13. Equipment + Lab Room
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

            // 14. Build DTO
            var dto = new ReportDataDto
            {
                ReportId = 0,
                ReportHeaderId = 0,
                ReportNo = "",
                CertificateNo = inward.CaseNo,
                ReportDate = DateTime.UtcNow,

                LabName = org?.LabName ?? "Laboratory",
                LabAddress = org?.LabAddress ?? "",
                LabPhone = org?.ContactPhone ?? "",
                LabEmail = org?.ContactEmail ?? "",
                LabLogoPath = org?.OrganizationLogo,
                CIN = org?.CIN,
                NablLogoPath = nablAccred?.LogoPath,
                CompanyStampPath = configs.GetValueOrDefault("COMPANY_STAMP_PATH"),

                UlrNo = ulr,
                DateOfIssue = DateTime.UtcNow.ToString("dd-MM-yyyy"),
                SampleReceivedDate = inward.CollectionTime.ToString("dd-MM-yyyy"),
                TestPerformedAt = org?.LabName ?? "Laboratory",

                CustomerName = customer.Name,
                CustomerAddress = customer.Address,
                CustomerGST = customer.GSTNo ?? "",

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

                RoomTemperature = firstCompletedHeader?.RoomTemperature,
                RoomHumidity = firstCompletedHeader?.RoomHumidity,
                EquipmentUsed = equipmentNames.Any() ? string.Join(", ", equipmentNames) : null,
                LabRoom = labRoomName,

                DateReceived = inward.CollectionTime.ToString("dd-MM-yyyy"),
                DateTested = latestCompleted.ToString("dd-MM-yyyy"),
                DateReported = DateTime.UtcNow.ToString("dd-MM-yyyy"),

                ReportConditions = configs.GetValueOrDefault("REPORT_CONDITIONS")
                    ?.Split('\n', StringSplitOptions.RemoveEmptyEntries)
                    .ToList() ?? new List<string>(),

                TestSections = testSections,
                Remarks = allRemarks.Any() ? string.Join("\n", allRemarks) : null,

                TestedByName = testedBy?.Name ?? "Lab Analyst",
                TestedByDesignation = testedBy?.Designation ?? "Lab Analyst",
                TestedBySignaturePath = testedBy?.SignaturePath,
                ReviewedByName = reviewedBy?.Name ?? "Technical Manager",
                ReviewedByDesignation = reviewedBy?.Designation ?? "Technical Manager",
                ReviewedBySignaturePath = reviewedBy?.SignaturePath,
                AuthorizedByName = authorizedBy?.Name ?? "Authorized Signatory",
                AuthorizedByDesignation = authorizedBy?.Designation ?? "Director",
                AuthorizedBySignaturePath = authorizedBy?.SignaturePath,

                QrCodeData = $"{_config["PublicBaseUrl"]}/report/verify/",

                IsNabl = testHeaders.All(h => h.IsNabl),
                NablCertNo = nablAccred?.CertificateNumber
            };

            // NABL scope info
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

            // Conformity
            dto.StatementOfConformity = inward.StatementOfConformity;
            dto.DecisionRule = inward.DecisionRule;

            if (inward.StatementOfConformity == "Applicable")
            {
                foreach (var section in dto.TestSections)
                {
                    foreach (var param in section.Parameters)
                    {
                        if (param.Status == "Pass") param.ConformityResult = "Conforms";
                        else if (param.Status == "Fail") param.ConformityResult = "Does not conform";
                    }
                }
            }

            return dto;
        }

        // ── HELPER METHODS (duplicated from ReportService for isolation) ──

        private static string DetermineTestType(TestResultHeader header)
        {
            var testName = header.LaboratoryTest?.Name?.ToLower() ?? "";
            var deptName = header.LaboratoryTest?.LabDepartment?.Name?.ToLower() ?? "";

            if (testName.Contains("chemical") || testName.Contains("composition") ||
                deptName.Contains("chemical") || deptName.Contains("spectrometer"))
                return "Chemical";

            return "General";
        }

        private static string DetermineTestCategory(TestResultHeader header, string testType)
        {
            if (testType == "Chemical") return "CHEMICAL";
            var deptName = header.LaboratoryTest?.LabDepartment?.Name?.ToUpper() ?? "";
            return !string.IsNullOrEmpty(deptName)
                ? $"{deptName} + METALS & ALLOYS"
                : "MECHANICAL + METALS & ALLOYS";
        }

        private static string? FormatDecimal(decimal? value, int decimalPrecision = 2)
        {
            if (!value.HasValue) return null;
            return value.Value.ToString($"F{decimalPrecision}");
        }

        private async Task<string?> GetUlrForSample(long sampleId)
        {
            var ulr = await _db.Set<GeneralTestMethod>()
                .Where(m => m.GeneralTest.SampleTestPlan.SampleID == sampleId
                    && !string.IsNullOrEmpty(m.UlrNo))
                .Select(m => m.UlrNo)
                .FirstOrDefaultAsync();

            if (!string.IsNullOrEmpty(ulr)) return ulr;

            ulr = await _db.Set<ChemicalTest>()
                .Where(c => c.SampleTestPlan.SampleID == sampleId
                    && !string.IsNullOrEmpty(c.UlrNo))
                .Select(c => c.UlrNo)
                .FirstOrDefaultAsync();

            return ulr;
        }

        private async Task<string> GenerateReportNoAsync()
        {
            // Simple sequential: GR-YYYYMMDD-XXXX
            var today = DateTime.UtcNow.ToString("yyyyMMdd");
            var count = await _db.GeneratedReports
                .CountAsync(r => r.ReportNo.StartsWith($"GR-{today}"));

            return $"GR-{today}-{(count + 1):D4}";
        }

        private async Task<string> SavePdfAsync(byte[] pdfBytes, string reportNo)
        {
            var uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Uploads", "GeneratedReports");
            Directory.CreateDirectory(uploadsDir);

            var fileName = $"{reportNo}.pdf";
            var filePath = Path.Combine(uploadsDir, fileName);
            await File.WriteAllBytesAsync(filePath, pdfBytes);

            return $"Uploads/GeneratedReports/{fileName}";
        }
    }
}
