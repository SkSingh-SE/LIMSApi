using LIMSApi.Dtos;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace LIMSApi.Reporting
{
    /// <summary>
    /// Professional PDF report renderer using QuestPDF Fluent API.
    /// Layout: Company Header -> Customer/Sample Info -> Test Results -> Images -> Remarks -> Signatures -> QR -> Footer.
    /// </summary>
    public class EnhancedReportDocument : IDocument
    {
        private readonly ReportDataDto _data;
        private readonly string _assetsPath;

        public EnhancedReportDocument(ReportDataDto data)
        {
            _data = data;
            _assetsPath = Path.Combine(Directory.GetCurrentDirectory(), "Assets");
        }

        public DocumentMetadata GetMetadata() => new()
        {
            Title = _data.ReportNo,
            Author = "Divine Metallurgical Services Pvt. Ltd.",
            Creator = "LIMS Report Engine"
        };

        // ────────────────────────────────────────────────
        // GLOBAL STYLES
        // ────────────────────────────────────────────────

        private static readonly string PrimaryColor = "#B71C1C"; // deep red
        private static readonly string HeaderBg = "#F5F5F5";
        private static readonly string BorderColor = "#BDBDBD";

        private static TextStyle TitleStyle =>
            TextStyle.Default.FontSize(12).Bold().FontColor(PrimaryColor);

        private static TextStyle SubTitleStyle =>
            TextStyle.Default.FontSize(9).Bold();

        private static TextStyle LabelStyle =>
            TextStyle.Default.FontSize(7.5f).FontColor(Colors.Grey.Darken2).Bold();

        private static TextStyle ValueStyle =>
            TextStyle.Default.FontSize(8);

        private static TextStyle SmallStyle =>
            TextStyle.Default.FontSize(6).FontColor(Colors.Grey.Darken1);

        private static TextStyle TableHeaderStyle =>
            TextStyle.Default.FontSize(7).Bold().FontColor(Colors.White);

        private static TextStyle TableCellStyle =>
            TextStyle.Default.FontSize(7);

        private static TextStyle SectionHeadingStyle =>
            TextStyle.Default.FontSize(9).Bold().FontColor(PrimaryColor);

        // ────────────────────────────────────────────────
        // COMPOSE
        // ────────────────────────────────────────────────

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.MarginVertical(15);
                page.MarginHorizontal(20);

                page.Header().Element(ComposeHeader);

                page.Content().PaddingVertical(4).Column(col =>
                {
                    // Section 2: Customer & Sample Info
                    col.Item().Element(ComposeCustomerSampleInfo);

                    col.Item().PaddingVertical(2);

                    // Section 3: Test Results (one per test section)
                    foreach (var section in _data.TestSections)
                    {
                        col.Item().Element(c => ComposeTestResults(c, section));
                        col.Item().PaddingVertical(2);
                    }

                    // Section 4: Test Images
                    foreach (var section in _data.TestSections.Where(s => s.Images.Any()))
                    {
                        col.Item().Element(c => ComposeTestImages(c, section));
                        col.Item().PaddingVertical(2);
                    }

                    // Section 5: Remarks
                    if (!string.IsNullOrWhiteSpace(_data.Remarks))
                    {
                        col.Item().Element(ComposeRemarks);
                        col.Item().PaddingVertical(2);
                    }

                    // Section 5b: NABL Scope Note (if partial or out of scope)
                    if (_data.NablInfo?.OutOfScopeParameterNames?.Any() == true)
                    {
                        col.Item().PaddingVertical(3).Border(0.5f).BorderColor(Colors.Orange.Darken2).Padding(6).Column(noteCol =>
                        {
                            noteCol.Item().Text("NABL Accreditation Note").Style(SectionHeadingStyle);
                            noteCol.Item().PaddingTop(2).Text(text =>
                            {
                                text.Span("The following tests are NOT covered under NABL Accreditation: ").Style(ValueStyle);
                                text.Span(string.Join(", ", _data.NablInfo.OutOfScopeParameterNames)).Style(ValueStyle).Bold();
                                text.Span(". Results for these tests are reported in our capacity as a non-accredited laboratory.").Style(ValueStyle);
                            });
                            if (_data.NablInfo.IsPartialScope)
                            {
                                noteCol.Item().PaddingTop(2).Text("* Partial NABL accreditation — see NABL column in results table for scope details.")
                                    .Style(SmallStyle);
                            }
                        });
                        col.Item().PaddingVertical(2);
                    }

                    // Section 6: Statement of Conformity (if applicable)
                    col.Item().Element(ComposeConformityStatement);

                    // Section 7: Signatures
                    col.Item().Element(ComposeSignatures);

                    col.Item().PaddingVertical(4);

                    // Section 7: QR Code
                    col.Item().Element(ComposeQrCode);
                });

                page.Footer().Element(ComposeFooter);
            });
        }

        // ────────────────────────────────────────────────
        // SECTION 1: COMPANY HEADER
        // ────────────────────────────────────────────────

        private void ComposeHeader(IContainer container)
        {
            container.Column(col =>
            {
                // Top bar with logo, company name, NABL
                col.Item()
                    .BorderBottom(2).BorderColor(PrimaryColor)
                    .PaddingBottom(4)
                    .Row(row =>
                    {
                        // LEFT: Company Logo
                        var logoPath = Path.Combine(_assetsPath, "logo.png");
                        if (File.Exists(logoPath))
                        {
                            row.ConstantItem(50)
                                .AlignMiddle()
                                .Image(logoPath)
                                .FitArea();
                        }
                        else
                        {
                            row.ConstantItem(50);
                        }

                        row.ConstantItem(6); // spacer

                        // CENTER: Company Info
                        row.RelativeItem().AlignMiddle().Column(center =>
                        {
                            center.Item().Text("DIVINE METALLURGICAL SERVICES PVT. LTD.")
                                .Style(TitleStyle);

                            center.Item().Text("14, Gopal Industrial Estate, Vallabhnagar, BRTS, Odhav, Ahmedabad - 382415")
                                .FontSize(6.5f).FontColor(Colors.Grey.Darken2);

                            center.Item().Text("Ph: +91 79 2289 2804 / 1013 | Email: divinelab_nhp@rediffmail.com")
                                .FontSize(6).FontColor(Colors.Grey.Darken1);

                            center.Item().Text("CIN: U74900GJ2012PTC069436")
                                .FontSize(5.5f).FontColor(Colors.Grey.Darken1);
                        });

                        // RIGHT: NABL Logo + Cert No (conditional on scope)
                        row.ConstantItem(80).AlignMiddle().Column(right =>
                        {
                            if (_data.IsNabl || _data.NablInfo?.IsPartialScope == true)
                            {
                                var nablLogoPath = Path.Combine(_assetsPath, "nabl_logo.png");
                                if (File.Exists(nablLogoPath))
                                {
                                    right.Item()
                                        .Height(30)
                                        .AlignCenter()
                                        .Image(nablLogoPath)
                                        .FitArea();
                                }

                                if (!string.IsNullOrWhiteSpace(_data.NablCertNo))
                                {
                                    var certText = _data.NablInfo?.IsPartialScope == true
                                        ? $"{_data.NablCertNo} *"
                                        : _data.NablCertNo;
                                    right.Item()
                                        .AlignCenter()
                                        .Text(certText)
                                        .FontSize(5.5f).Bold();
                                }
                            }
                        });
                    });

                // Report No / Date / Page line
                col.Item()
                    .PaddingTop(3)
                    .Row(row =>
                    {
                        row.RelativeItem().Text(t =>
                        {
                            t.Span("Report No: ").Style(LabelStyle);
                            t.Span(_data.ReportNo).Style(ValueStyle);
                        });

                        if (!string.IsNullOrWhiteSpace(_data.CertificateNo))
                        {
                            row.RelativeItem().AlignCenter().Text(t =>
                            {
                                t.Span("Certificate No: ").Style(LabelStyle);
                                t.Span(_data.CertificateNo).Style(ValueStyle);
                            });
                        }

                        row.RelativeItem().AlignRight().Text(t =>
                        {
                            t.Span("Date: ").Style(LabelStyle);
                            t.Span(_data.ReportDate.ToString("dd-MM-yyyy")).Style(ValueStyle);
                            t.Span("   Page ").Style(SmallStyle);
                            t.CurrentPageNumber().Style(SmallStyle);
                            t.Span(" of ").Style(SmallStyle);
                            t.TotalPages().Style(SmallStyle);
                        });
                    });
            });
        }

        // ────────────────────────────────────────────────
        // SECTION 2: CUSTOMER & SAMPLE INFO TABLE
        // ────────────────────────────────────────────────

        private void ComposeCustomerSampleInfo(IContainer container)
        {
            container
                .Border(0.5f).BorderColor(BorderColor)
                .Column(col =>
                {
                    // Section title
                    col.Item()
                        .Background(PrimaryColor)
                        .Padding(3)
                        .Text("TEST CERTIFICATE")
                        .FontSize(9).Bold().FontColor(Colors.White)
                        .AlignCenter();

                    // Two-column key-value table
                    col.Item().Padding(4).Table(table =>
                    {
                        table.ColumnsDefinition(c =>
                        {
                            c.ConstantColumn(100); // Label left
                            c.RelativeColumn();    // Value left
                            c.ConstantColumn(100); // Label right
                            c.RelativeColumn();    // Value right
                        });

                        // Row 1: Customer Name + Case No
                        AddInfoRow(table, "Customer Name", _data.CustomerName, "Case No.", _data.CaseNo);
                        // Row 2: Customer Address + Sample No
                        AddInfoRow(table, "Customer Address", _data.CustomerAddress, "Sample No.", _data.SampleNo);
                        // Row 3: GST + Description
                        AddInfoRow(table, "GST No.", _data.CustomerGST, "Description", _data.SampleDescription);
                        // Row 4: Material Spec + Grade
                        AddInfoRow(table, "Material Spec.", _data.MaterialSpec, "Grade", _data.Grade);
                        // Row 5: Date Received + Date Tested
                        AddInfoRow(table, "Date Received", _data.DateReceived, "Date Tested", _data.DateTested);
                        // Row 6: Date Reported
                        AddInfoRow(table, "Date Reported", _data.DateReported, "", "");
                    });
                });
        }

        private static void AddInfoRow(TableDescriptor table,
            string label1, string value1,
            string label2, string value2)
        {
            // Left side
            table.Cell()
                .BorderBottom(0.25f).BorderColor(Colors.Grey.Lighten2)
                .PaddingVertical(2).PaddingHorizontal(3)
                .Text(label1).Style(LabelStyle);

            table.Cell()
                .BorderBottom(0.25f).BorderColor(Colors.Grey.Lighten2)
                .PaddingVertical(2).PaddingHorizontal(3)
                .Text(value1).Style(ValueStyle);

            // Right side
            table.Cell()
                .BorderBottom(0.25f).BorderColor(Colors.Grey.Lighten2)
                .PaddingVertical(2).PaddingHorizontal(3)
                .Text(label2).Style(LabelStyle);

            table.Cell()
                .BorderBottom(0.25f).BorderColor(Colors.Grey.Lighten2)
                .PaddingVertical(2).PaddingHorizontal(3)
                .Text(value2).Style(ValueStyle);
        }

        // ────────────────────────────────────────────────
        // SECTION 3: TEST RESULTS TABLE (Dynamic)
        // ────────────────────────────────────────────────

        private void ComposeTestResults(IContainer container, ReportDataTestSection section)
        {
            container
                .Border(0.5f).BorderColor(BorderColor)
                .Column(col =>
                {
                    // Test name header
                    col.Item()
                        .Background(HeaderBg)
                        .BorderBottom(0.5f).BorderColor(BorderColor)
                        .Padding(4)
                        .Text(section.TestName)
                        .Style(SectionHeadingStyle);

                    if (!string.IsNullOrWhiteSpace(section.SpecificationName))
                    {
                        col.Item()
                            .PaddingHorizontal(4).PaddingVertical(1)
                            .Text(t =>
                            {
                                t.Span("Specification: ").Style(LabelStyle);
                                t.Span(section.SpecificationName).Style(ValueStyle);
                            });
                    }

                    if (!section.Parameters.Any())
                    {
                        col.Item().Padding(6).Text("No parameters recorded.").Style(SmallStyle);
                        return;
                    }

                    // Render different table depending on test type
                    if (section.TestType == "Chemical")
                    {
                        col.Item().Element(c => ComposeChemicalTable(c, section.Parameters));
                    }
                    else
                    {
                        col.Item().Element(c => ComposeGeneralTable(c, section.Parameters));
                    }
                });
        }

        /// <summary>
        /// General / Mechanical test table: Sr.No | Parameter | Unit | Spec Limit | Result | Status
        /// </summary>
        private void ComposeGeneralTable(IContainer container, List<ReportDataParameter> parameters)
        {
            container.Padding(2).Table(table =>
            {
                table.ColumnsDefinition(c =>
                {
                    c.ConstantColumn(25);  // Sr.No
                    c.RelativeColumn(3);   // Parameter
                    c.RelativeColumn(1);   // Unit
                    c.RelativeColumn(2);   // Specification Limit
                    c.RelativeColumn(1.5f);// Result
                    c.RelativeColumn(1);   // Status
                });

                // Header
                table.Header(h =>
                {
                    AddHeaderCell(h, "Sr.");
                    AddHeaderCell(h, "Parameter");
                    AddHeaderCell(h, "Unit");
                    AddHeaderCell(h, "Specification Limit");
                    AddHeaderCell(h, "Result");
                    AddHeaderCell(h, "Status");
                });

                // Rows
                for (int i = 0; i < parameters.Count; i++)
                {
                    var p = parameters[i];
                    string bgColor = i % 2 == 0 ? "#FFFFFF" : "#FAFAFA";

                    AddTableCell(table, (i + 1).ToString(), bgColor);
                    AddTableCell(table, p.Name, bgColor);
                    AddTableCell(table, p.Unit, bgColor);
                    AddTableCell(table, FormatSpecLimit(p.SpecMin, p.SpecMax), bgColor);
                    AddTableCell(table, p.Result ?? "-", bgColor);
                    AddStatusCell(table, p.Status, bgColor);
                }
            });
        }

        /// <summary>
        /// Chemical test table: Element | Unit | Spec Min | Spec Max | Result | Status
        /// </summary>
        private void ComposeChemicalTable(IContainer container, List<ReportDataParameter> parameters)
        {
            container.Padding(2).Table(table =>
            {
                table.ColumnsDefinition(c =>
                {
                    c.RelativeColumn(2);   // Element
                    c.RelativeColumn(1);   // Unit
                    c.RelativeColumn(1);   // Spec Min
                    c.RelativeColumn(1);   // Spec Max
                    c.RelativeColumn(1.5f);// Result
                    c.RelativeColumn(1);   // Status
                });

                // Header
                table.Header(h =>
                {
                    AddHeaderCell(h, "Element");
                    AddHeaderCell(h, "Unit");
                    AddHeaderCell(h, "Spec Min");
                    AddHeaderCell(h, "Spec Max");
                    AddHeaderCell(h, "Result");
                    AddHeaderCell(h, "Status");
                });

                // Rows
                for (int i = 0; i < parameters.Count; i++)
                {
                    var p = parameters[i];
                    string bgColor = i % 2 == 0 ? "#FFFFFF" : "#FAFAFA";

                    AddTableCell(table, p.Name, bgColor);
                    AddTableCell(table, p.Unit, bgColor);
                    AddTableCell(table, p.SpecMin ?? "-", bgColor);
                    AddTableCell(table, p.SpecMax ?? "-", bgColor);
                    AddTableCell(table, p.Result ?? "-", bgColor);
                    AddStatusCell(table, p.Status, bgColor);
                }
            });
        }

        // ────────────────────────────────────────────────
        // SECTION 4: TEST IMAGES
        // ────────────────────────────────────────────────

        private void ComposeTestImages(IContainer container, ReportDataTestSection section)
        {
            container
                .Border(0.5f).BorderColor(BorderColor)
                .Column(col =>
                {
                    col.Item()
                        .Background(HeaderBg)
                        .BorderBottom(0.5f).BorderColor(BorderColor)
                        .Padding(4)
                        .Text($"Test Images - {section.TestName}")
                        .Style(SectionHeadingStyle);

                    const float ImageSize = 140;
                    const int ImagesPerRow = 2;

                    col.Item().Padding(6).Column(imgCol =>
                    {
                        for (int i = 0; i < section.Images.Count; i += ImagesPerRow)
                        {
                            var rowImages = section.Images
                                .Skip(i)
                                .Take(ImagesPerRow)
                                .ToList();

                            imgCol.Item().Row(row =>
                            {
                                foreach (var img in rowImages)
                                {
                                    row.RelativeItem().Column(c =>
                                    {
                                        var imgPath = GetImageFullPath(img.Url);
                                        if (File.Exists(imgPath))
                                        {
                                            c.Item()
                                                .Height(ImageSize)
                                                .AlignCenter()
                                                .Image(imgPath)
                                                .FitArea();
                                        }
                                        else
                                        {
                                            c.Item()
                                                .Height(ImageSize)
                                                .AlignCenter()
                                                .Background(Colors.Grey.Lighten3)
                                                .Text("Image not found")
                                                .FontSize(7).FontColor(Colors.Grey.Darken1);
                                        }

                                        if (!string.IsNullOrWhiteSpace(img.Caption))
                                        {
                                            c.Item()
                                                .PaddingTop(2)
                                                .AlignCenter()
                                                .Text(img.Caption)
                                                .FontSize(6.5f).Italic();
                                        }
                                    });
                                }

                                // Fill remaining columns
                                for (int j = rowImages.Count; j < ImagesPerRow; j++)
                                    row.RelativeItem();
                            });
                        }
                    });
                });
        }

        // ────────────────────────────────────────────────
        // SECTION 5: REMARKS & OBSERVATIONS
        // ────────────────────────────────────────────────

        private void ComposeRemarks(IContainer container)
        {
            container
                .Border(0.5f).BorderColor(BorderColor)
                .Column(col =>
                {
                    col.Item()
                        .Background(HeaderBg)
                        .BorderBottom(0.5f).BorderColor(BorderColor)
                        .Padding(4)
                        .Text("Remarks & Observations")
                        .Style(SectionHeadingStyle);

                    col.Item()
                        .Padding(6)
                        .Text(_data.Remarks)
                        .Style(ValueStyle)
                        .LineHeight(1.3f);
                });
        }

        // ────────────────────────────────────────────────
        // STATEMENT OF CONFORMITY
        // ────────────────────────────────────────────────

        private void ComposeConformityStatement(IContainer container)
        {
            if (_data.StatementOfConformity != "Applicable")
            {
                container.Text(""); // empty — no section rendered
                return;
            }

            container.PaddingTop(6).Column(col =>
            {
                col.Item().BorderBottom(1).BorderColor(Colors.Grey.Medium)
                    .PaddingBottom(2)
                    .Text("Statement of Conformity")
                    .FontSize(9).Bold();

                col.Item().PaddingTop(4).Text(text =>
                {
                    text.Span("Decision Rule: ").FontSize(8).Bold();
                    text.Span(_data.DecisionRule ?? "Not specified").FontSize(8);
                });

                // Overall conformity
                var allParams = _data.TestSections.SelectMany(s => s.Parameters).ToList();
                var hasNonConforming = allParams.Any(p => p.ConformityResult == "Does not conform");
                var overallConformity = hasNonConforming
                    ? "The test results do not conform to the specification requirements."
                    : "The test results conform to the specification requirements.";

                col.Item().PaddingTop(2).Text(text =>
                {
                    text.Span("Overall Result: ").FontSize(8).Bold();
                    text.Span(overallConformity).FontSize(8);
                });

                // MOU note
                if (_data.DecisionRule != null && _data.DecisionRule.Contains("With MOU"))
                {
                    col.Item().PaddingTop(2).Text(
                        "Note: Expanded uncertainty (U) at 95% confidence level has been considered in the conformity assessment."
                    ).FontSize(7).Italic().FontColor(Colors.Grey.Darken1);
                }
            });
        }

        // ────────────────────────────────────────────────
        // SIGNATURES
        // ────────────────────────────────────────────────

        private void ComposeSignatures(IContainer container)
        {
            container.PaddingTop(6).Column(col =>
            {
                col.Item().LineHorizontal(0.5f).LineColor(BorderColor);

                col.Item().PaddingTop(6).Row(row =>
                {
                    RenderSignatureBlock(row.RelativeItem(),
                        "Tested By",
                        _data.TestedByName,
                        _data.TestedByDesignation,
                        _data.TestedBySignaturePath);

                    row.ConstantItem(10);

                    RenderSignatureBlock(row.RelativeItem(),
                        "Reviewed By",
                        _data.ReviewedByName,
                        _data.ReviewedByDesignation,
                        _data.ReviewedBySignaturePath);

                    row.ConstantItem(10);

                    RenderSignatureBlock(row.RelativeItem(),
                        "Authorized Signatory",
                        _data.AuthorizedByName,
                        _data.AuthorizedByDesignation,
                        _data.AuthorizedBySignaturePath);
                });
            });
        }

        private void RenderSignatureBlock(IContainer container,
            string title, string name, string? designation, string? signaturePath)
        {
            container.Border(0.5f).BorderColor(BorderColor).Padding(4).Column(c =>
            {
                // Signature image
                c.Item().Height(30).AlignCenter().Column(sigCol =>
                {
                    if (!string.IsNullOrWhiteSpace(signaturePath))
                    {
                        var fullPath = GetImageFullPath(signaturePath);
                        if (File.Exists(fullPath))
                        {
                            sigCol.Item()
                                .Height(28)
                                .AlignCenter()
                                .Image(fullPath)
                                .FitArea();
                        }
                    }
                });

                c.Item().LineHorizontal(0.25f).LineColor(Colors.Grey.Lighten1);

                c.Item().PaddingTop(2).AlignCenter()
                    .Text(title).FontSize(6.5f).Bold().FontColor(PrimaryColor);

                if (!string.IsNullOrWhiteSpace(name))
                {
                    c.Item().AlignCenter().Text(name).FontSize(7);
                }

                if (!string.IsNullOrWhiteSpace(designation))
                {
                    c.Item().AlignCenter().Text(designation).FontSize(6).FontColor(Colors.Grey.Darken1);
                }
            });
        }

        // ────────────────────────────────────────────────
        // SECTION 7: QR CODE
        // ────────────────────────────────────────────────

        private void ComposeQrCode(IContainer container)
        {
            var qrPath = Path.Combine(_assetsPath, "qr_code.png");
            if (!File.Exists(qrPath))
                return;

            container.AlignRight().Row(row =>
            {
                row.RelativeItem(); // push QR to right

                row.ConstantItem(60).Column(col =>
                {
                    col.Item()
                        .Height(55)
                        .Image(qrPath)
                        .FitArea();

                    col.Item()
                        .AlignCenter()
                        .Text("Scan to verify\nreport authenticity")
                        .FontSize(5).FontColor(Colors.Grey.Darken1)
                        .AlignCenter();
                });
            });
        }

        // ────────────────────────────────────────────────
        // SECTION 8: FOOTER
        // ────────────────────────────────────────────────

        private void ComposeFooter(IContainer container)
        {
            container.Column(col =>
            {
                col.Item().LineHorizontal(0.8f).LineColor(PrimaryColor);

                col.Item().PaddingTop(2).PaddingHorizontal(4).Row(row =>
                {
                    row.RelativeItem().Column(c =>
                    {
                        c.Item().Text(
                            "This report shall not be reproduced except in full without written approval of " +
                            "Divine Metallurgical Services Pvt. Ltd.")
                            .FontSize(5).FontColor(Colors.Grey.Darken1).Italic();

                        if (_data.IsNabl)
                        {
                            c.Item().Text(
                                "The results reported relate only to the items tested. " +
                                "NABL accredited as per ISO/IEC 17025:2017.")
                                .FontSize(5).FontColor(Colors.Grey.Darken1);
                        }
                    });

                    row.ConstantItem(80).AlignRight().AlignBottom()
                        .Text(t =>
                        {
                            t.Span("Page ").Style(SmallStyle);
                            t.CurrentPageNumber().Style(SmallStyle);
                            t.Span(" of ").Style(SmallStyle);
                            t.TotalPages().Style(SmallStyle);
                        });
                });
            });
        }

        // ────────────────────────────────────────────────
        // TABLE HELPERS
        // ────────────────────────────────────────────────

        private static void AddHeaderCell(dynamic header, string text)
        {
            header.Cell()
                .Background(PrimaryColor)
                .PaddingVertical(3)
                .PaddingHorizontal(4)
                .Text(text)
                .Style(TableHeaderStyle);
        }

        private static void AddTableCell(TableDescriptor table, string text, string bgColor)
        {
            table.Cell()
                .Background(bgColor)
                .BorderBottom(0.25f).BorderColor(Colors.Grey.Lighten2)
                .PaddingVertical(2)
                .PaddingHorizontal(4)
                .Text(text)
                .Style(TableCellStyle);
        }

        private static void AddStatusCell(TableDescriptor table, string status, string bgColor)
        {
            string statusColor = status switch
            {
                "Pass" => "#2E7D32",  // green
                "Fail" => "#C62828",  // red
                _ => "#757575"        // grey
            };

            table.Cell()
                .Background(bgColor)
                .BorderBottom(0.25f).BorderColor(Colors.Grey.Lighten2)
                .PaddingVertical(2)
                .PaddingHorizontal(4)
                .Text(status)
                .FontSize(7).Bold().FontColor(statusColor);
        }

        private static string FormatSpecLimit(string? min, string? max)
        {
            if (string.IsNullOrWhiteSpace(min) && string.IsNullOrWhiteSpace(max))
                return "-";

            if (!string.IsNullOrWhiteSpace(min) && !string.IsNullOrWhiteSpace(max))
                return $"{min} - {max}";

            if (!string.IsNullOrWhiteSpace(min))
                return $"Min: {min}";

            return $"Max: {max}";
        }

        /// <summary>
        /// Resolves image paths. If the path is already absolute, returns it;
        /// otherwise treats it as relative to wwwroot.
        /// </summary>
        private string GetImageFullPath(string path)
        {
            if (Path.IsPathRooted(path))
                return path;

            // Try relative to Assets first
            var assetsRelative = Path.Combine(_assetsPath, path);
            if (File.Exists(assetsRelative))
                return assetsRelative;

            // Try relative to wwwroot
            var wwwrootRelative = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", path);
            if (File.Exists(wwwrootRelative))
                return wwwrootRelative;

            // Fallback: return as-is
            return path;
        }
    }
}
