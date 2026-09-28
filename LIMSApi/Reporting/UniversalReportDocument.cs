using System;
using System.IO;
using System.Linq;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace LIMSApi.Reporting
{
    /// <summary>
    /// Render-time (non-frozen) report lifecycle identity. Pure presentation input:
    /// never stored, never hashed, never recalculated. Lets the PDF title block,
    /// authentication section and footer show the correct lifecycle state
    /// (PREVIEW / GENERATED / RELEASED / SUPERSEDED / VOID) with actor + dates.
    /// </summary>
    public sealed class UniversalReportRenderInfo
    {
        public string ReportStatus { get; set; } = "PREVIEW";
        public DateTime? GeneratedOn { get; set; }
        public string? GeneratedByName { get; set; }
        public DateTime? ReleasedOn { get; set; }
        public string? ReleasedByName { get; set; }
        public string? ReportDataHash { get; set; }
        public string? ResultRevisionHash { get; set; }
        public string? SnapshotHash { get; set; }
    }

    /// <summary>
    /// Phase 9 Universal Report document — pure PRESENTATION over frozen history.
    /// Follows the standard ISO 17025 / NABL 8-section report layout matching UI preview.
    /// 1. Laboratory Header (Left: Lab Details, Center: NABL/ILAC Mark, Right: QR Code)
    /// 2. Report Identity Banner ("TEST REPORT" + Barcode strip)
    /// 3. Customer & Sample Details (Side-by-side cards)
    /// 4. Test Details Card
    /// 5. Test Results Table (7 standard columns)
    /// 6. Remarks / Observations (ISO 17025 conformity & uncertainty clauses)
    /// 7. Signatures (3 columns: Analyst, Reviewer, Approver)
    /// 8. Footer (Accreditation disclaimer & End of Report divider)
    /// </summary>
    public class UniversalReportDocument : BaseLabReportDocument
    {
        private readonly UniversalReportDataDto _u;
        private readonly UniversalReportRenderInfo _info;

        private static readonly string NavyDark = "#1e293b";
        private static readonly string BlueBanner = "#e8f1fa";
        private static readonly string BorderColor = "#cbd5e1";
        private static readonly string CardHeaderBg = "#f1f5f9";
        private static readonly string LightRowBg = "#f8fafc";

        public UniversalReportDocument(UniversalReportDataDto data, string? watermarkText = null, UniversalReportRenderInfo? info = null)
            : base(MapHeader(data, info), watermarkText)
        {
            _u = data;
            _info = info ?? new UniversalReportRenderInfo();
        }

        private static ReportDataDto MapHeader(UniversalReportDataDto u, UniversalReportRenderInfo? info) => new()
        {
            ReportNo = u.ReportNo ?? "PREVIEW",
            ReportDate = info?.GeneratedOn ?? u.ApprovedOn ?? DateTime.UtcNow,
            LabName = u.LabName ?? "Laboratory",
            LabAddress = u.BranchAddress ?? u.LabAddress ?? string.Empty,
            LabPhone = u.LabPhone ?? string.Empty,
            LabEmail = u.LabEmail ?? string.Empty,
            LabLogoPath = u.LabLogoPath,
            UlrNo = u.UlrNo,
            IsNabl = u.ShowNablMark,
            NablCertNo = u.NablCertNo,
            NablLogoPath = u.NablLogoPath,
            TestedByName = u.AnalystName ?? "-",
            ReviewedByName = u.VerifiedByName ?? u.ReviewerName ?? "-",
            AuthorizedByName = u.ApprovedByName ?? "-",
            DecisionRule = u.DecisionRule,
            StatementOfConformity = u.OverallDecision
        };

        private static string Safe(string? v, string fallback = "—") => string.IsNullOrWhiteSpace(v) ? fallback : v;
        private static string FmtDate(DateTime? d) => d.HasValue ? d.Value.ToLocalTime().ToString("dd MMM yyyy") : "—";

        private string? ResolveDiskPath(string? path)
        {
            if (string.IsNullOrWhiteSpace(path)) return null;
            if (Path.IsPathRooted(path) && File.Exists(path)) return path;

            var candidates = new[]
            {
                Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", path),
                Path.Combine(AppContext.BaseDirectory, "wwwroot", path),
                Path.Combine(Directory.GetCurrentDirectory(), path),
                Path.Combine(AppContext.BaseDirectory, path),
                Path.Combine(AssetsPath, path)
            };

            foreach (var c in candidates)
            {
                if (File.Exists(c)) return c;
            }
            return null;
        }

        private string? ResolveLabLogo()
        {
            var resolved = ResolveDiskPath(_u.LabLogoPath ?? Data.LabLogoPath);
            if (resolved != null) return resolved;

            var orgDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Uploads", "Organization");
            if (Directory.Exists(orgDir))
            {
                return Directory.GetFiles(orgDir, "*.*")
                    .FirstOrDefault(f => f.EndsWith(".png", StringComparison.OrdinalIgnoreCase) || f.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) || f.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase));
            }
            return null;
        }

        private string? ResolveNablLogo()
        {
            var resolved = ResolveDiskPath(_u.NablLogoPath ?? Data.NablLogoPath);
            if (resolved != null) return resolved;

            var nablDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Uploads", "Nabl");
            if (Directory.Exists(nablDir))
            {
                return Directory.GetFiles(nablDir, "*.*")
                    .FirstOrDefault(f => f.EndsWith(".png", StringComparison.OrdinalIgnoreCase) || f.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) || f.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase));
            }
            return null;
        }

        public override void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(24);
                page.Header().Element(ComposeHeaderSection);
                page.Content().PaddingVertical(4).Column(col =>
                {
                    col.Item().Element(ComposeIdentityAndBarcodeSection);
                    col.Item().PaddingTop(6).Element(ComposeCustomerAndSampleSection);
                    col.Item().PaddingTop(6).Element(ComposeTestDetailsSection);
                    col.Item().PaddingTop(6).Element(ComposeTestResultsSection);
                    col.Item().PaddingTop(6).Element(ComposeRemarksSection);
                    col.Item().PaddingTop(8).Element(ComposeSignaturesSection);
                });
                page.Footer().Element(ComposeFooterSection);
                if (!string.IsNullOrWhiteSpace(WatermarkText))
                    page.Background().Element(ComposeWatermark);
            });
        }

        // ── 1. LABORATORY HEADER ──
        private void ComposeHeaderSection(IContainer container)
        {
            container.BorderBottom(1).BorderColor(BorderColor).PaddingBottom(6).Row(row =>
            {
                // Left: Lab Details (and Logo if configured)
                row.RelativeItem().Row(leftRow =>
                {
                    var labLogoPath = ResolveLabLogo();
                    if (labLogoPath != null)
                    {
                        leftRow.ConstantItem(55).AlignMiddle().PaddingRight(6).Image(labLogoPath).FitArea();
                    }

                    leftRow.RelativeItem().AlignMiddle().Column(col =>
                    {
                        col.Item().Text(Safe(_u.LabName, "DEVINE LABORATORY").ToUpper())
                           .FontSize(11).Bold().FontColor(NavyDark);

                        if (!string.IsNullOrWhiteSpace(_u.LabSubtitle))
                            col.Item().Text(_u.LabSubtitle).FontSize(6.5f).FontColor(Colors.Grey.Darken2);

                        if (!string.IsNullOrWhiteSpace(_u.LabTagline))
                            col.Item().Text(_u.LabTagline).FontSize(6.5f).Italic().FontColor(Colors.Grey.Darken1);

                        var address = _u.BranchAddress ?? _u.LabAddress;
                        if (!string.IsNullOrWhiteSpace(address))
                            col.Item().Text(address).FontSize(7).FontColor(Colors.Grey.Darken3);

                        var contactParts = new[]
                        {
                            string.IsNullOrWhiteSpace(_u.LabPhone) ? null : $"Phone: {_u.LabPhone}",
                            string.IsNullOrWhiteSpace(_u.LabEmail) ? null : $"Email: {_u.LabEmail}",
                            string.IsNullOrWhiteSpace(_u.LabWebsite) ? null : $"Website: {_u.LabWebsite}"
                        }.Where(s => s != null).ToList();

                        if (contactParts.Count > 0)
                        {
                            col.Item().Text(string.Join("  |  ", contactParts))
                               .FontSize(6.5f).FontColor(Colors.Grey.Darken2);
                        }
                    });
                });

                // Center: NABL / ILAC Emblem or Standard Lab Badge
                row.ConstantItem(120).AlignCenter().AlignMiddle().Column(col =>
                {
                    if (_u.ShowNablMark)
                    {
                        var nablLogoPath = ResolveDiskPath(_u.NablLogoPath);
                        if (nablLogoPath != null)
                        {
                            col.Item().Height(30).AlignCenter().Image(nablLogoPath).FitArea();
                            col.Item().PaddingTop(1).AlignCenter().Text(Safe(_u.NablCertNo, "TC5098")).FontSize(7.5f).Bold().FontColor(Colors.Black);
                        }
                        else
                        {
                            col.Item().Border(1).BorderColor(Colors.Blue.Darken2).Padding(3).AlignCenter().Column(emblem =>
                            {
                                emblem.Item().Text("ilac-MRA / NABL").FontSize(6.5f).Bold().FontColor(Colors.Blue.Darken2);
                                emblem.Item().Text(Safe(_u.NablCertNo, "TC5098")).FontSize(7.5f).Bold().FontColor(Colors.Black);
                            });
                        }
                    }
                    else
                    {
                        col.Item().Background(Colors.Grey.Lighten4).Border(0.5f).BorderColor(BorderColor).Padding(4).AlignCenter()
                           .Text("STANDARD LAB REPORT").FontSize(6.5f).FontColor(Colors.Grey.Darken2);
                    }
                });

                // Right: QR Code for Verification
                row.ConstantItem(55).AlignRight().AlignMiddle().Column(col =>
                {
                    var qrUrl = !string.IsNullOrWhiteSpace(_u.ReportNo)
                        ? $"https://lims.verification/report/{_u.ReportNo}"
                        : "https://lims.verification/report/preview";
                    var qrBytes = QrCodeHelper.GenerateQrPng(qrUrl, pixelsPerModule: 3);
                    if (qrBytes != null)
                    {
                        col.Item().Height(45).Width(45).Image(qrBytes).FitArea();
                    }
                });
            });
        }

        // ── 2. REPORT IDENTITY & BARCODE STRIP ──
        private void ComposeIdentityAndBarcodeSection(IContainer container)
        {
            container.Column(col =>
            {
                // Title Banner
                col.Item().Background(NavyDark).PaddingVertical(3).PaddingHorizontal(6).AlignCenter()
                   .Text("TEST REPORT").FontSize(9.5f).Bold().FontColor(Colors.White).LetterSpacing(0.08f);

                // Identity Details Grid with Barcode
                col.Item().Border(1).BorderColor(BorderColor).BorderTop(0).Background(LightRowBg).Padding(4).Row(row =>
                {
                    // Left: Report No & ULR No
                    row.RelativeItem(3).Column(left =>
                    {
                        DataRow(left.Item(), "Report No.", _u.ReportNo ?? "— (Draft Preview)", boldVal: true);
                        DataRow(left.Item(), "ULR No.", _u.UlrNo ?? (_u.ShowNablMark ? "— (Assigned on Release)" : "Not Applicable (Non-NABL)"));
                    });

                    // Center: Barcode Visual
                    row.RelativeItem(3).AlignCenter().AlignMiddle().Column(center =>
                    {
                        var barcodePayload = _u.UlrNo ?? _u.ReportNo ?? $"EXEC-{_u.TestExecutionID}";
                        var barcodeBytes = BarcodeHelper.GenerateBarcodeBmp(barcodePayload, height: 26, moduleWidth: 1);
                        if (barcodeBytes != null && barcodeBytes.Length > 0)
                        {
                            center.Item().Height(20).AlignCenter().Image(barcodeBytes).FitArea();
                        }
                        center.Item().AlignCenter().Text(barcodePayload).FontSize(6.5f).Bold().FontColor(Colors.Grey.Darken3);
                    });

                    // Right: Report Date, Issue Date, Page No
                    row.RelativeItem(3).Column(right =>
                    {
                        DataRow(right.Item(), "Report Date", FmtDate(_u.DateTested ?? DateTime.UtcNow));
                        var issueDateStr = _info.ReleasedOn.HasValue ? FmtDate(_info.ReleasedOn) : "— (Pending Release)";
                        DataRow(right.Item(), "Issue Date", issueDateStr);
                        right.Item().Row(r =>
                        {
                            r.ConstantItem(60).Text("Page No.").FontSize(7).FontColor(Colors.Grey.Darken2);
                            r.ConstantItem(6).Text(":").FontSize(7).FontColor(Colors.Grey.Darken2);
                            r.RelativeItem().Text(text =>
                            {
                                text.CurrentPageNumber().FontSize(7);
                                text.Span(" of ").FontSize(7);
                                text.TotalPages().FontSize(7);
                            });
                        });
                    });
                });
            });
        }

        // ── 3. CUSTOMER & SAMPLE DETAILS (2 SIDE-BY-SIDE CARDS) ──
        private void ComposeCustomerAndSampleSection(IContainer container)
        {
            container.Row(row =>
            {
                // Customer Details Card
                row.RelativeItem().Border(1).BorderColor(BorderColor).Column(c =>
                {
                    c.Item().Background(CardHeaderBg).BorderBottom(1).BorderColor(BorderColor).PaddingVertical(2).PaddingHorizontal(4)
                     .Text("CUSTOMER DETAILS").FontSize(7.5f).Bold().FontColor(NavyDark);

                    c.Item().Padding(4).Column(col =>
                    {
                        DataRow(col.Item(), "Customer Name", Safe(_u.CustomerName), boldVal: true);
                        DataRow(col.Item(), "Address", Safe(_u.CustomerAddress));
                        DataRow(col.Item(), "Contact Person", Safe(_u.ContactPerson));
                        DataRow(col.Item(), "Reference", Safe(_u.CustomerReference ?? _u.CaseNo));
                    });
                });

                row.ConstantItem(6);

                // Sample Details Card
                row.RelativeItem().Border(1).BorderColor(BorderColor).Column(c =>
                {
                    c.Item().Background(CardHeaderBg).BorderBottom(1).BorderColor(BorderColor).PaddingVertical(2).PaddingHorizontal(4)
                     .Text("SAMPLE DETAILS").FontSize(7.5f).Bold().FontColor(NavyDark);

                    c.Item().Padding(4).Column(col =>
                    {
                        DataRow(col.Item(), "Sample Description", Safe(_u.SampleDescription));
                        DataRow(col.Item(), "Sample Identification", Safe(_u.SampleNo), boldVal: true);
                        DataRow(col.Item(), "Sample Received Date", FmtDate(_u.SampleReceivedOn));
                        DataRow(col.Item(), "Test Commencement Date", FmtDate(_u.DateTested));
                        DataRow(col.Item(), "Test Completion Date", FmtDate(_u.DateTested));
                        DataRow(col.Item(), "Sample Condition", "As Received");
                        DataRow(col.Item(), "Sampling Source", "Customer Supplied");
                        DataRow(col.Item(), "Sampling Plan", "Not Applicable");
                    });
                });
            });
        }

        // ── 4. TEST DETAILS ──
        private void ComposeTestDetailsSection(IContainer container)
        {
            container.Border(1).BorderColor(BorderColor).Column(c =>
            {
                c.Item().Background(CardHeaderBg).BorderBottom(1).BorderColor(BorderColor).PaddingVertical(2).PaddingHorizontal(4)
                 .Text("TEST DETAILS").FontSize(7.5f).Bold().FontColor(NavyDark);

                c.Item().Padding(4).Row(row =>
                {
                    row.RelativeItem(3).Column(left =>
                    {
                        DataRow(left.Item(), "Test Name", Safe(_u.LaboratoryTestName), boldVal: true);
                        DataRow(left.Item(), "Test Method", Safe(_u.TestMethodStandard ?? _u.TestMethodName));
                        DataRow(left.Item(), "Discipline", Safe(_u.DisciplineName));
                        DataRow(left.Item(), "Test Equipment", Safe(_u.TestEquipment));
                        DataRow(left.Item(), "Environmental Conditions", Safe(_u.EnvironmentalConditions));
                    });

                    row.RelativeItem(2).Column(right =>
                    {
                        DataRow(right.Item(), "Method Version", Safe(_u.TestMethodVersion));
                    });
                });
            });
        }

        // ── 5. TEST RESULTS TABLE ──
        private void ComposeTestResultsSection(IContainer container)
        {
            container.Border(1).BorderColor(BorderColor).Column(c =>
            {
                c.Item().Background(CardHeaderBg).BorderBottom(1).BorderColor(BorderColor).PaddingVertical(2).PaddingHorizontal(4)
                 .Text("TEST RESULTS").FontSize(7.5f).Bold().FontColor(NavyDark);

                c.Item().Table(table =>
                {
                    table.ColumnsDefinition(cd =>
                    {
                        cd.ConstantColumn(24); // S. No.
                        cd.RelativeColumn(3);  // Parameter
                        cd.ConstantColumn(55); // Result
                        cd.ConstantColumn(38); // Unit
                        cd.RelativeColumn(3);  // Requirement / Specification
                        cd.ConstantColumn(65); // MDL
                        cd.ConstantColumn(45); // NABL Scope
                    });

                    // Table Header
                    table.Header(h =>
                    {
                        h.Cell().Background(LightRowBg).BorderBottom(1).BorderColor(BorderColor).Padding(3).AlignCenter()
                         .Text("S. No.").FontSize(7).Bold().FontColor(NavyDark);
                        h.Cell().Background(LightRowBg).BorderBottom(1).BorderColor(BorderColor).Padding(3)
                         .Text("Parameter").FontSize(7).Bold().FontColor(NavyDark);
                        h.Cell().Background(LightRowBg).BorderBottom(1).BorderColor(BorderColor).Padding(3).AlignRight()
                         .Text("Result").FontSize(7).Bold().FontColor(NavyDark);
                        h.Cell().Background(LightRowBg).BorderBottom(1).BorderColor(BorderColor).Padding(3).AlignCenter()
                         .Text("Unit").FontSize(7).Bold().FontColor(NavyDark);
                        h.Cell().Background(LightRowBg).BorderBottom(1).BorderColor(BorderColor).Padding(3)
                         .Text($"Requirement / Specification ({Safe(_u.SpecificationTitle)})").FontSize(7).Bold().FontColor(NavyDark);
                        h.Cell().Background(LightRowBg).BorderBottom(1).BorderColor(BorderColor).Padding(3).AlignCenter()
                         .Text("Method Detection Limit (if applicable)").FontSize(6.5f).Bold().FontColor(NavyDark);
                        h.Cell().Background(LightRowBg).BorderBottom(1).BorderColor(BorderColor).Padding(3).AlignCenter()
                         .Text("NABL Scope (Yes / No)").FontSize(6.5f).Bold().FontColor(NavyDark);
                    });

                    var rows = _u.ResultParameters.Where(p => p.IsReportable).OrderBy(p => p.DisplayOrder).ToList();
                    if (rows.Count == 0)
                    {
                        table.Cell().ColumnSpan(7).Padding(6).AlignCenter().Text("No reportable parameters.").FontSize(7.5f).FontColor(Colors.Grey.Medium);
                    }

                    int idx = 0;
                    foreach (var p in rows)
                    {
                        idx++;
                        var reportedVal = p.ReportedValue ?? p.DisplayValue ?? (p.ComplianceValue.HasValue ? p.ComplianceValue.Value.ToString("F2") : p.RawValue ?? "—");
                        var specStr = FormatSpec(p);
                        var nablScope = _u.ShowNablMark ? "Yes" : "No";

                        table.Cell().BorderBottom(0.5f).BorderColor(BorderColor).Padding(3).AlignCenter()
                             .Text(idx.ToString()).FontSize(7).FontColor(Colors.Grey.Darken2);
                        table.Cell().BorderBottom(0.5f).BorderColor(BorderColor).Padding(3)
                             .Text(Safe(p.ParameterName)).FontSize(7).Bold().FontColor(Colors.Black);
                        table.Cell().BorderBottom(0.5f).BorderColor(BorderColor).Padding(3).AlignRight()
                             .Text(reportedVal).FontSize(7.5f).Bold().FontColor(Colors.Black);
                        table.Cell().BorderBottom(0.5f).BorderColor(BorderColor).Padding(3).AlignCenter()
                             .Text(Safe(p.Unit)).FontSize(7).FontColor(Colors.Grey.Darken3);
                        table.Cell().BorderBottom(0.5f).BorderColor(BorderColor).Padding(3)
                             .Text(specStr).FontSize(7).FontColor(Colors.Grey.Darken3);
                        table.Cell().BorderBottom(0.5f).BorderColor(BorderColor).Padding(3).AlignCenter()
                             .Text("—").FontSize(7).FontColor(Colors.Grey.Darken2);
                        table.Cell().BorderBottom(0.5f).BorderColor(BorderColor).Padding(3).AlignCenter()
                             .Text(nablScope).FontSize(7).FontColor(Colors.Grey.Darken3);
                    }
                });
            });
        }

        // ── 6. REMARKS / OBSERVATIONS ──
        private void ComposeRemarksSection(IContainer container)
        {
            container.Border(1).BorderColor(BorderColor).Column(c =>
            {
                c.Item().Background(CardHeaderBg).BorderBottom(1).BorderColor(BorderColor).PaddingVertical(2).PaddingHorizontal(4)
                 .Text("REMARKS / OBSERVATIONS").FontSize(7.5f).Bold().FontColor(NavyDark);

                c.Item().Padding(4).Column(col =>
                {
                    bool isPass = string.Equals(_u.OverallDecision, "PASS", StringComparison.OrdinalIgnoreCase);
                    var verb = isPass ? "conforms" : "does not conform";
                    var specName = Safe(_u.SpecificationTitle, "the governing standard");

                    col.Item().PaddingBottom(1.5f).Text($"1. The sample {verb} to the requirements of {specName} for the tested parameters.")
                       .FontSize(6.8f).FontColor(Colors.Grey.Darken3);
                    col.Item().PaddingBottom(1.5f).Text("2. The results reported are only for the sample tested.")
                       .FontSize(6.8f).FontColor(Colors.Grey.Darken3);
                    col.Item().PaddingBottom(1.5f).Text("3. NABL accredited tests are marked as \"Yes\" under NABL Scope column.")
                       .FontSize(6.8f).FontColor(Colors.Grey.Darken3);
                    col.Item().PaddingBottom(1.5f).Text($"4. This report shall not be reproduced, except in full, without the written approval of {Safe(_u.LabName, "Devine Laboratory")}.")
                       .FontSize(6.8f).FontColor(Colors.Grey.Darken3);

                    // Measurement Uncertainty
                    var muStr = FormatUncertainty(_u);
                    col.Item().PaddingBottom(1.5f).Text(text =>
                    {
                        text.Span("5. Expanded Uncertainty (U): ").FontSize(6.8f).Bold();
                        text.Span(muStr).FontSize(6.8f);
                    });

                    // Verification Notes
                    if (!string.IsNullOrWhiteSpace(_u.ReviewRemarks) || !string.IsNullOrWhiteSpace(_u.ApprovalRemarks))
                    {
                        var note = _u.ReviewRemarks ?? _u.ApprovalRemarks;
                        col.Item().Text($"6. {note}").FontSize(6.8f).FontColor(Colors.Grey.Darken3);
                    }
                });
            });
        }

        // ── 7. SIGNATURES (3 COLUMNS) ──
        private void ComposeSignaturesSection(IContainer container)
        {
            container.Border(1).BorderColor(BorderColor).Padding(6).Row(row =>
            {
                SignatoryColumn(row.RelativeItem(), "Tested By", "(Analyst)", _u.AnalystName, _u.AnalystDesignation, _u.DateTested, _u.AnalystSignaturePath);
                SignatoryColumn(row.RelativeItem().BorderLeft(0.5f).BorderRight(0.5f).BorderColor(BorderColor), "Verified By", "(Technical Reviewer)", _u.VerifiedByName, _u.ReviewerDesignation, _u.VerifiedOn, _u.ReviewerSignaturePath);
                SignatoryColumn(row.RelativeItem(), "Approved By", "(Authorized Signatory)", _u.ApprovedByName, _u.ApproverDesignation, _u.ApprovedOn, _u.ApproverSignaturePath);
            });
        }

        private void SignatoryColumn(IContainer container, string title, string role, string? name, string? designation, DateTime? date, string? signaturePath)
        {
            container.AlignCenter().Column(col =>
            {
                col.Item().AlignCenter().Text(t =>
                {
                    t.Span(title).FontSize(7.5f).Bold().FontColor(NavyDark);
                    t.Span($"\n{role}").FontSize(6.5f).FontColor(Colors.Grey.Darken2);
                });

                var resolvedSig = ResolveDiskPath(signaturePath);
                if (resolvedSig != null)
                {
                    col.Item().PaddingVertical(2).Height(24).AlignCenter().Image(resolvedSig).FitArea();
                }
                else
                {
                    // Clean dashed signature line
                    col.Item().PaddingVertical(8).Width(110).BorderBottom(0.75f).BorderColor(BorderColor);
                }

                col.Item().AlignCenter().Text(Safe(name, "—")).FontSize(8).Bold().FontColor(Colors.Black);
                col.Item().AlignCenter().Text(Safe(designation, "—")).FontSize(6.5f).FontColor(Colors.Grey.Darken2);
                col.Item().AlignCenter().Text($"Date: {FmtDate(date)}").FontSize(6.8f).FontColor(Colors.Grey.Darken3);
            });
        }

        // ── 8. FOOTER ──
        private void ComposeFooterSection(IContainer container)
        {
            container.Column(col =>
            {
                // End of Report divider
                col.Item().AlignCenter().PaddingVertical(2)
                   .Text("---------- End of Report ----------").FontSize(7).FontColor(Colors.Grey.Darken2);

                // Accreditation Disclaimer Box
                col.Item().Border(0.5f).BorderColor(BorderColor).Background(LightRowBg).Padding(3).AlignCenter().Column(inner =>
                {
                    inner.Item().AlignCenter()
                         .Text("This laboratory is accredited by NABL, India in accordance with ISO/IEC 17025:2017 for the tests as specified in the scope of accreditation.")
                         .FontSize(6.2f).FontColor(Colors.Grey.Darken3);
                    inner.Item().AlignCenter()
                         .Text("The validity of this report can be verified by scanning the QR code or on NABL website (www.nabl-india.org).")
                         .FontSize(6.2f).FontColor(Colors.Grey.Darken3);
                });

                // Document Hash & Page Numbering Bottom Line
                col.Item().PaddingTop(2).Row(r =>
                {
                    var dataHash = string.IsNullOrWhiteSpace(_info.ReportDataHash) ? (_u.SnapshotHash ?? "—") : _info.ReportDataHash;
                    if (dataHash.Length > 24) dataHash = dataHash.Substring(0, 24) + "...";

                    r.RelativeItem().Text($"Document Hash: {dataHash} (Immutable ISO 17025 Electronic Report)")
                     .FontSize(6f).FontColor(Colors.Grey.Darken2);

                    r.ConstantItem(60).AlignRight().Text(text =>
                    {
                        text.Span("Page ").FontSize(6.5f).FontColor(Colors.Grey.Darken2);
                        text.CurrentPageNumber().FontSize(6.5f).FontColor(Colors.Grey.Darken2);
                        text.Span(" of ").FontSize(6.5f).FontColor(Colors.Grey.Darken2);
                        text.TotalPages().FontSize(6.5f).FontColor(Colors.Grey.Darken2);
                    });
                });
            });
        }

        private static void DataRow(IContainer container, string label, string value, bool boldVal = false)
        {
            container.PaddingBottom(1.5f).Row(r =>
            {
                r.ConstantItem(85).Text(label).FontSize(7).FontColor(Colors.Grey.Darken2);
                r.ConstantItem(6).Text(":").FontSize(7).FontColor(Colors.Grey.Darken2);
                var valText = r.RelativeItem().Text(value).FontSize(7).FontColor(Colors.Black);
                if (boldVal) valText.Bold();
            });
        }

        private static string FormatSpec(UniversalResultParameterDto p)
        {
            var min = p.EffectiveMin ?? p.SpecMin;
            var max = p.EffectiveMax ?? p.SpecMax;
            if (min.HasValue && max.HasValue) return $"{min.Value:G29} - {max.Value:G29}";
            if (min.HasValue) return $"Min. {min.Value:G29}";
            if (max.HasValue) return $"Max. {max.Value:G29}";
            return "—";
        }

        private static string FormatUncertainty(UniversalReportDataDto u)
        {
            decimal val = 3.0m;
            decimal k = 2.0m;
            if (u.MeasurementUncertainty != null)
            {
                if (u.MeasurementUncertainty.ExpandedUncertainty.HasValue)
                    val = u.MeasurementUncertainty.ExpandedUncertainty.Value;
                else if (u.MeasurementUncertainty.Value.HasValue)
                    val = u.MeasurementUncertainty.Value.Value;

                k = u.MeasurementUncertainty.CoverageFactor;
            }
            return $"±{val:G29} % at approx. 95.45% confidence level (coverage factor k = {k:G29}).";
        }
    }
}
