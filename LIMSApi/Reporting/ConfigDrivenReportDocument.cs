using System.Text.Json;
using System.Text.RegularExpressions;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Helpers.Enums;
using LIMSApi.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace LIMSApi.Reporting
{
    /// <summary>
    /// Config-driven PDF renderer — reads ReportFormat sections and renders dynamically.
    /// Completely independent from EnhancedReportDocument / BaseLabReportDocument.
    /// Uses same styling constants for consistent output.
    /// </summary>
    public class ConfigDrivenReportDocument : IDocument
    {
        private readonly ReportDataDto _data;
        private readonly ReportFormat _format;
        private readonly string _assetsPath;
        private JsonElement? _headerConfig;

        // ── STYLE CONSTANTS (matching existing report styles) ──
        private const string PrimaryColor = "#B71C1C";
        private const string BorderColor = "#333333";
        private const string HeaderBg = "#F5F5F5";
        private const float CellBorderWidth = 0.5f;
        private const float CellPadding = 3f;

        private const float FontCompanyName = 11f;
        private const float FontSectionHeader = 8f;
        private const float FontTableHeader = 7f;
        private const float FontTableCell = 7f;
        private const float FontFooter = 5.5f;
        private const float FontSmall = 6f;
        private const float FontLabel = 7f;

        public ConfigDrivenReportDocument(ReportDataDto data, ReportFormat format)
        {
            _data = data;
            _format = format;
            _assetsPath = Path.Combine(Directory.GetCurrentDirectory(), "Assets");
        }

        public DocumentMetadata GetMetadata() => new()
        {
            Title = _data.ReportNo,
            Author = _data.LabName,
            Creator = "LIMS Config-Driven Report Engine"
        };

        // ────────────────────────────────────────────────
        // COMPOSE — MAIN ENTRY
        // ────────────────────────────────────────────────

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                // Page size from format config
                var size = _format.PageSize == "Letter" ? PageSizes.Letter : PageSizes.A4;
                page.Size(_format.PageLayout == "Landscape" ? size.Landscape() : size);

                page.MarginVertical(15);
                page.MarginHorizontal(20);
                page.DefaultTextStyle(x => x.FontFamily("Arial"));

                // Header & Footer are rendered from sections if present
                var sections = _format.Sections
                    .Where(s => s.IsActive && s.IsVisible)
                    .OrderBy(s => s.SortOrder)
                    .ToList();

                var headerSection = sections.FirstOrDefault(s => s.SectionType == ReportSectionType.Header);
                if (headerSection != null)
                {
                    _headerConfig = GetConfig(headerSection);
                    page.Header().Element(c => RenderHeaderSection(c, _headerConfig));
                }

                page.Content().Element(c => ComposeContent(c, sections));

                page.Footer().Element(c => ComposePageFooter(c, _headerConfig));
            });
        }

        // ────────────────────────────────────────────────
        // CONTENT — iterate sections
        // ────────────────────────────────────────────────

        private void ComposeContent(IContainer container, List<ReportFormatSection> sections)
        {
            container.PaddingVertical(4).Column(col =>
            {
                var contentSections = sections
                    .Where(s => s.SectionType != ReportSectionType.Header)
                    .ToList();

                // Check if Signature + QR are adjacent — render them side-by-side
                var sigIndex = contentSections.FindIndex(s => s.SectionType == ReportSectionType.SignatureBlock);
                var qrIndex = contentSections.FindIndex(s => s.SectionType == ReportSectionType.QRCode);
                var combineSignatureQr = sigIndex >= 0 && qrIndex >= 0 && Math.Abs(sigIndex - qrIndex) == 1;

                for (int idx = 0; idx < contentSections.Count; idx++)
                {
                    var section = contentSections[idx];
                    var config = GetConfig(section);
                    var isLast = idx == contentSections.Count - 1;

                    switch (section.SectionType)
                    {
                        case ReportSectionType.CustomerInfo:
                            col.Item().Element(c => RenderCustomerInfo(c, config));
                            break;

                        case ReportSectionType.SampleInfo:
                            col.Item().Element(c => RenderSampleInfo(c, config));
                            break;

                        case ReportSectionType.ResultTable:
                            col.Item().Element(c => RenderResultTable(c, config));
                            break;

                        case ReportSectionType.ChemicalPivot:
                            col.Item().Element(c => RenderChemicalPivot(c, config));
                            break;

                        case ReportSectionType.Observation:
                            col.Item().Element(c => RenderObservation(c, config));
                            break;

                        case ReportSectionType.Conformity:
                            col.Item().Element(c => RenderConformity(c, config));
                            break;

                        case ReportSectionType.ImageGallery:
                            col.Item().Element(c => RenderImageGallery(c, config));
                            break;

                        case ReportSectionType.CustomText:
                            col.Item().Element(c => RenderCustomText(c, config));
                            break;

                        case ReportSectionType.SignatureBlock:
                            if (combineSignatureQr)
                            {
                                // Render Signature + QR side by side (like EnhancedReportDocument)
                                var qrSection = contentSections[qrIndex];
                                var qrConfig = GetConfig(qrSection);
                                col.Item().Row(row =>
                                {
                                    row.RelativeItem(3).Element(c => RenderSignatureBlock(c, config));
                                    row.RelativeItem(1).Element(c => RenderQrCode(c, qrConfig));
                                });
                            }
                            else
                            {
                                col.Item().Element(c => RenderSignatureBlock(c, config));
                            }
                            break;

                        case ReportSectionType.ScopeNote:
                            col.Item().Element(c => RenderScopeNote(c, config));
                            break;

                        case ReportSectionType.QRCode:
                            // Skip if already rendered with Signature
                            if (!combineSignatureQr)
                                col.Item().Element(c => RenderQrCode(c, config));
                            break;

                        case ReportSectionType.Divider:
                            col.Item().Element(c => RenderDivider(c, config));
                            break;

                        case ReportSectionType.PageBreak:
                            // Don't add page break at the very end
                            if (!isLast)
                                col.Item().PageBreak();
                            break;
                    }

                    // Add spacing between sections (but not after the last one)
                    if (!isLast && section.SectionType != ReportSectionType.Divider
                              && section.SectionType != ReportSectionType.PageBreak)
                    {
                        col.Item().PaddingVertical(2);
                    }
                }
            });
        }

        // ────────────────────────────────────────────────
        // SECTION: Header
        // ────────────────────────────────────────────────

        private void RenderHeaderSection(IContainer container, JsonElement? config)
        {
            var showCompanyLogo = GetBool(config, "showCompanyLogo", true);
            var showNablLogo = GetBool(config, "showNablLogo", true);
            var showUlr = GetBool(config, "showUlr", true);
            var title = GetString(config, "title", "Test Certificate");
            var showCertIdentity = GetBool(config, "showCertIdentity", true);

            container.Column(col =>
            {
                // ── ROW 1: Logo + Company Info + NABL Logos ──
                col.Item()
                    .BorderBottom(1).BorderColor(PrimaryColor)
                    .PaddingBottom(4)
                    .Row(row =>
                    {
                        // LEFT: Company Logo
                        if (showCompanyLogo)
                        {
                            row.ConstantItem(60).AlignMiddle().Column(logoCol =>
                            {
                                var logoPath = ResolveImagePath(_data.LabLogoPath, "TPTL-icon.png");
                                if (logoPath != null)
                                {
                                    try { logoCol.Item().Height(50).Image(logoPath).FitArea(); }
                                    catch { logoCol.Item().Height(50).AlignCenter().AlignMiddle()
                                        .Text("LOGO").FontSize(FontSmall).FontColor(Colors.Grey.Medium); }
                                }
                            });
                            row.ConstantItem(6);
                        }

                        // CENTER: Company Name + Address + Contact + CIN
                        row.RelativeItem().AlignMiddle().Column(center =>
                        {
                            center.Item().AlignCenter()
                                .Text(Safe(_data.LabName))
                                .FontSize(FontCompanyName).Bold().FontColor(PrimaryColor);

                            center.Item().AlignCenter()
                                .Text(Safe(_data.LabAddress))
                                .FontSize(6.5f).FontColor(Colors.Grey.Darken2);

                            center.Item().AlignCenter()
                                .Text($"Ph: {Safe(_data.LabPhone)}, {Safe(_data.LabEmail)}")
                                .FontSize(FontSmall).FontColor(Colors.Grey.Darken1);

                            if (!string.IsNullOrWhiteSpace(_data.CIN))
                            {
                                center.Item().AlignCenter()
                                    .Text($"CIN  {_data.CIN}")
                                    .FontSize(FontFooter).FontColor(Colors.Grey.Darken1);
                            }
                        });

                        // RIGHT: NABL/ilac-MRA Logos + TC Number
                        if (showNablLogo)
                        {
                            row.ConstantItem(90).AlignMiddle().Column(right =>
                            {
                                var nablLogoPath = ResolveImagePath(_data.NablLogoPath, "nabl_logo.png");
                                if (nablLogoPath != null)
                                {
                                    try { right.Item().Height(35).AlignCenter().Image(nablLogoPath).FitArea(); }
                                    catch { right.Item().Height(35).AlignCenter().AlignMiddle()
                                        .Text("NABL").FontSize(FontSmall).Bold(); }
                                }

                                // TC Number (e.g., "TC 5098")
                                if (!string.IsNullOrWhiteSpace(_data.NablCertNo))
                                {
                                    right.Item().AlignCenter()
                                        .Text($"TC {_data.NablCertNo}")
                                        .FontSize(FontSmall).Bold();
                                }
                            });
                        }
                    });

                // ── ROW 2: Title + Page Number ──
                col.Item().PaddingTop(3).Row(titleRow =>
                {
                    titleRow.RelativeItem().AlignCenter().PaddingVertical(3)
                        .Text(title).FontSize(12).Bold().FontColor(PrimaryColor);

                    titleRow.ConstantItem(80).AlignRight().AlignBottom()
                        .Text(text =>
                        {
                            text.Span("Page ").FontSize(FontSmall);
                            text.CurrentPageNumber().FontSize(FontSmall);
                            text.Span(" of ").FontSize(FontSmall);
                            text.TotalPages().FontSize(FontSmall);
                        });
                });

                // ── ROW 3: Certificate Identity Grid (repeats every page) ──
                if (showCertIdentity)
                {
                    col.Item().Border(CellBorderWidth).BorderColor(BorderColor).Table(table =>
                    {
                        table.ColumnsDefinition(c =>
                        {
                            c.RelativeColumn(1.3f); // Label left
                            c.RelativeColumn(2.2f); // Value left
                            c.RelativeColumn(1.3f); // Label right
                            c.RelativeColumn(1.5f); // Value right
                        });

                        // ULR row
                        if (showUlr && !string.IsNullOrWhiteSpace(_data.UlrNo))
                        {
                            AddIdentityCell(table, "ULR - " + Safe(_data.UlrNo), 4, true);
                        }

                        AddIdentityRow(table, "Test Certificate No.", Safe(_data.CertificateNo), "Date of Issue:", Safe(_data.DateOfIssue));
                        AddIdentityRow(table, "Customer Name:", Safe(_data.CustomerName), "Sample Received On:", Safe(_data.SampleReceivedDate));
                        AddIdentityRow(table, "Customer Address:", Safe(_data.CustomerAddress), "Test Performed at:", Safe(_data.TestPerformedAt));
                    });
                }
            });
        }

        // Helper: full-width identity cell
        private static void AddIdentityCell(TableDescriptor table, string text, int colSpan, bool bold)
        {
            table.Cell().ColumnSpan((uint)colSpan)
                .Border(CellBorderWidth).BorderColor(BorderColor)
                .Background(HeaderBg)
                .Padding(CellPadding)
                .Text(text).FontSize(FontLabel).Bold();
        }

        // Helper: 4-column identity row
        private static void AddIdentityRow(TableDescriptor table, string label1, string value1, string label2, string value2)
        {
            table.Cell().Border(CellBorderWidth).BorderColor(BorderColor)
                .Background(HeaderBg).Padding(CellPadding)
                .Text(label1).FontSize(FontLabel).Bold();

            table.Cell().Border(CellBorderWidth).BorderColor(BorderColor)
                .Padding(CellPadding)
                .Text(value1).FontSize(FontTableCell);

            table.Cell().Border(CellBorderWidth).BorderColor(BorderColor)
                .Background(HeaderBg).Padding(CellPadding)
                .Text(label2).FontSize(FontLabel).Bold();

            table.Cell().Border(CellBorderWidth).BorderColor(BorderColor)
                .Padding(CellPadding)
                .Text(value2).FontSize(FontTableCell);
        }

        // ────────────────────────────────────────────────
        // SECTION: Customer Info
        // ────────────────────────────────────────────────

        private void RenderCustomerInfo(IContainer container, JsonElement? config)
        {
            var columns = GetInt(config, "columns", 2);
            var sectionTitle = GetString(config, "sectionTitle", "INFORMATION PROVIDED BY THE CUSTOMER");
            var fields = GetStringArray(config, "fields", new[]
                { "POReference", "SampleDescription", "StampedAs", "SampleDrawnBy", "NatureOfSample", "MaterialSpec" });

            var fieldMap = BuildCustomerFieldMap();
            RenderKeyValueGrid(container, sectionTitle, fields, fieldMap, columns);
        }

        // ────────────────────────────────────────────────
        // SECTION: Sample Info
        // ────────────────────────────────────────────────

        private void RenderSampleInfo(IContainer container, JsonElement? config)
        {
            var columns = GetInt(config, "columns", 2);
            var fields = GetStringArray(config, "fields", new[]
                { "SampleDescription", "Grade", "HeatNo", "Dimensions", "DateReceived", "DateTested" });

            var fieldMap = BuildSampleFieldMap();
            RenderKeyValueGrid(container, "Sample Details", fields, fieldMap, columns);
        }

        // ────────────────────────────────────────────────
        // SECTION: Result Table
        // ────────────────────────────────────────────────

        private void RenderResultTable(IContainer container, JsonElement? config)
        {
            var variant = GetString(config, "variant", "grouped");
            var showSpecRow = GetBool(config, "showSpecRow", true);

            if (_data.TestSections == null || !_data.TestSections.Any()) return;

            // Filter: only General (non-chemical) test sections
            var generalSections = _data.TestSections
                .Where(s => s.TestType != "Chemical")
                .ToList();

            if (!generalSections.Any()) return;

            container.Column(col =>
            {
                foreach (var section in generalSections)
                {
                    if (variant == "grouped")
                    {
                        // Section header
                        col.Item()
                            .Background(PrimaryColor)
                            .Padding(CellPadding)
                            .Text(text =>
                            {
                                text.Span(Safe(section.TestCategory)).FontSize(FontSectionHeader)
                                    .Bold().FontColor(Colors.White);
                            });

                        // Sub-header: Test name + method + date
                        col.Item()
                            .Background("#EEEEEE")
                            .Border(CellBorderWidth).BorderColor(BorderColor)
                            .Padding(CellPadding)
                            .Text(text =>
                            {
                                text.Span($"Test: {Safe(section.TestName)}").FontSize(FontTableCell).Bold();
                                if (!string.IsNullOrWhiteSpace(section.TestMethod))
                                    text.Span($"  |  Method: {section.TestMethod}").FontSize(FontTableCell);
                                if (!string.IsNullOrWhiteSpace(section.DateOfTesting))
                                    text.Span($"  |  Date: {section.DateOfTesting}").FontSize(FontTableCell);
                            });
                    }

                    // Result table
                    RenderParameterTable(col, section.Parameters, showSpecRow);

                    col.Item().PaddingVertical(3);
                }
            });
        }

        private void RenderParameterTable(ColumnDescriptor col, List<ReportDataParameter> parameters, bool showSpecRow)
        {
            if (parameters == null || !parameters.Any()) return;

            col.Item().Border(CellBorderWidth).BorderColor(BorderColor).Table(table =>
            {
                table.ColumnsDefinition(c =>
                {
                    c.ConstantColumn(25);    // Sr
                    c.RelativeColumn(2.5f);  // Parameter
                    c.RelativeColumn(1);     // Unit
                    c.RelativeColumn(1);     // Spec Min
                    c.RelativeColumn(1);     // Spec Max
                    c.RelativeColumn(1.2f);  // Result
                });

                // Header row
                AddHeaderCell(table, "Sr");
                AddHeaderCell(table, "Parameter");
                AddHeaderCell(table, "Unit");
                AddHeaderCell(table, "Spec Min");
                AddHeaderCell(table, "Spec Max");
                AddHeaderCell(table, "Result");

                int sr = 1;
                foreach (var param in parameters)
                {
                    var bg = sr % 2 == 0 ? "#FAFAFA" : "#FFFFFF";
                    AddDataCell(table, sr.ToString(), bg, HorizontalAlignment.Center);
                    AddDataCell(table, Safe(param.Name), bg);
                    AddDataCell(table, Safe(param.Unit), bg, HorizontalAlignment.Center);
                    AddDataCell(table, Safe(param.SpecMin), bg, HorizontalAlignment.Center);
                    AddDataCell(table, Safe(param.SpecMax), bg, HorizontalAlignment.Center);
                    AddResultCell(table, Safe(param.Result), param.Status, bg);
                    sr++;
                }
            });
        }

        // ────────────────────────────────────────────────
        // SECTION: Chemical Pivot
        // ────────────────────────────────────────────────

        private void RenderChemicalPivot(IContainer container, JsonElement? config)
        {
            var showSpecRow = GetBool(config, "showSpecRow", true);

            var chemicalSections = _data.TestSections?
                .Where(s => s.TestType == "Chemical")
                .ToList();

            if (chemicalSections == null || !chemicalSections.Any()) return;

            container.Column(col =>
            {
                foreach (var section in chemicalSections)
                {
                    // Section header
                    col.Item()
                        .Background(PrimaryColor)
                        .Padding(CellPadding)
                        .Text(Safe(section.TestCategory))
                        .FontSize(FontSectionHeader).Bold().FontColor(Colors.White);

                    var parameters = section.Parameters;
                    if (parameters == null || !parameters.Any()) continue;

                    // Get distinct elements
                    var elements = parameters.Select(p => p.Name).Distinct().ToList();

                    // Check if multiple subgroups exist (e.g., GTAW, SMAW)
                    var subGroups = parameters.Select(p => p.SubGroup).Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().ToList();

                    if (subGroups.Count <= 1)
                    {
                        // Simple chemical table: Element | Unit | Spec Min | Spec Max | Result
                        col.Item().Border(CellBorderWidth).BorderColor(BorderColor).Table(table =>
                        {
                            table.ColumnsDefinition(c =>
                            {
                                c.RelativeColumn(2);    // Element
                                c.RelativeColumn(1);    // Unit
                                c.RelativeColumn(1.2f); // Spec Min
                                c.RelativeColumn(1.2f); // Spec Max
                                c.RelativeColumn(1.2f); // Result
                            });

                            AddHeaderCell(table, "Element");
                            AddHeaderCell(table, "Unit");
                            AddHeaderCell(table, "Spec Min");
                            AddHeaderCell(table, "Spec Max");
                            AddHeaderCell(table, "Result");

                            int sr = 0;
                            foreach (var param in parameters)
                            {
                                var bg = sr % 2 == 0 ? "#FFFFFF" : "#FAFAFA";
                                AddDataCell(table, Safe(param.Name), bg);
                                AddDataCell(table, Safe(param.Unit), bg, HorizontalAlignment.Center);
                                AddDataCell(table, Safe(param.SpecMin), bg, HorizontalAlignment.Center);
                                AddDataCell(table, Safe(param.SpecMax), bg, HorizontalAlignment.Center);
                                AddResultCell(table, Safe(param.Result), param.Status, bg);
                                sr++;
                            }
                        });
                    }
                    else
                    {
                        // Pivot table: Element | Unit | SubGroup1 | SubGroup2 | ...
                        col.Item().Border(CellBorderWidth).BorderColor(BorderColor).Table(table =>
                        {
                            table.ColumnsDefinition(c =>
                            {
                                c.RelativeColumn(2);    // Element
                                c.RelativeColumn(1);    // Unit
                                foreach (var _ in subGroups)
                                    c.RelativeColumn(1.2f);
                            });

                            AddHeaderCell(table, "Element");
                            AddHeaderCell(table, "Unit");
                            foreach (var sg in subGroups)
                                AddHeaderCell(table, sg);

                            // Spec row
                            if (showSpecRow)
                            {
                                AddDataCell(table, "Specification", HeaderBg);
                                AddDataCell(table, "", HeaderBg);
                                foreach (var sg in subGroups)
                                {
                                    var specParam = parameters.FirstOrDefault(p => p.SubGroup == sg);
                                    AddDataCell(table, specParam != null ? $"{Safe(specParam.SpecMin)}-{Safe(specParam.SpecMax)}" : "-", HeaderBg, HorizontalAlignment.Center);
                                }
                            }

                            int sr = 0;
                            foreach (var element in elements)
                            {
                                var bg = sr % 2 == 0 ? "#FFFFFF" : "#FAFAFA";
                                var first = parameters.First(p => p.Name == element);
                                AddDataCell(table, element, bg);
                                AddDataCell(table, Safe(first.Unit), bg, HorizontalAlignment.Center);

                                foreach (var sg in subGroups)
                                {
                                    var param = parameters.FirstOrDefault(p => p.Name == element && p.SubGroup == sg);
                                    if (param != null)
                                        AddResultCell(table, Safe(param.Result), param.Status, bg);
                                    else
                                        AddDataCell(table, "-", bg, HorizontalAlignment.Center);
                                }
                                sr++;
                            }
                        });
                    }

                    col.Item().PaddingVertical(3);
                }
            });
        }

        // ────────────────────────────────────────────────
        // SECTION: Observation / Remarks
        // ────────────────────────────────────────────────

        private void RenderObservation(IContainer container, JsonElement? config)
        {
            var defaultText = GetString(config, "defaultText", "");
            var remarks = !string.IsNullOrWhiteSpace(_data.Remarks) ? _data.Remarks : defaultText;

            container.Border(CellBorderWidth).BorderColor(BorderColor).Column(col =>
            {
                col.Item().Background(HeaderBg).Padding(CellPadding)
                    .Text("Remarks / Observations").FontSize(FontLabel).Bold();

                col.Item().Padding(CellPadding)
                    .Text(Safe(remarks, "N/A")).FontSize(FontTableCell).LineHeight(1.3f);
            });
        }

        // ────────────────────────────────────────────────
        // SECTION: Conformity Statement
        // ────────────────────────────────────────────────

        private void RenderConformity(IContainer container, JsonElement? config)
        {
            var passText = GetString(config, "passText", "The sample conforms to the specified requirements.");
            var failText = GetString(config, "failText", "The sample does NOT conform to the specified requirements.");
            var showDecisionRule = GetBool(config, "showDecisionRule", true);

            // Determine overall conformity
            var allParams = _data.TestSections?.SelectMany(s => s.Parameters) ?? Enumerable.Empty<ReportDataParameter>();
            var hasResults = allParams.Any();
            var allPass = !hasResults || allParams.All(p => p.Status != "Fail");

            container.Border(CellBorderWidth).BorderColor(BorderColor).Column(col =>
            {
                col.Item().Background(HeaderBg).Padding(CellPadding)
                    .Text("Statement of Conformity").FontSize(FontLabel).Bold();

                if (showDecisionRule && !string.IsNullOrWhiteSpace(_data.DecisionRule))
                {
                    col.Item().Padding(CellPadding)
                        .Text($"Decision Rule: {_data.DecisionRule}")
                        .FontSize(FontSmall).FontColor(Colors.Grey.Darken1);
                }

                col.Item().Padding(CellPadding)
                    .Text(allPass ? passText : failText)
                    .FontSize(FontTableCell)
                    .FontColor(allPass ? "#2E7D32" : "#C62828")
                    .Bold();
            });
        }

        // ────────────────────────────────────────────────
        // SECTION: Image Gallery
        // ────────────────────────────────────────────────

        private void RenderImageGallery(IContainer container, JsonElement? config)
        {
            var gridColumns = GetInt(config, "gridColumns", 2);
            var maxHeight = GetInt(config, "maxHeight", 140);
            var showCaptions = GetBool(config, "showCaptions", true);

            var allImages = _data.TestSections?
                .SelectMany(s => s.Images ?? Enumerable.Empty<ReportDataImage>())
                .Where(i => !string.IsNullOrWhiteSpace(i.Url))
                .ToList();

            if (allImages == null || !allImages.Any()) return;

            container.Column(col =>
            {
                col.Item().Background(HeaderBg).Padding(CellPadding)
                    .Text("Test Images").FontSize(FontLabel).Bold();

                // Render images in grid
                for (int i = 0; i < allImages.Count; i += gridColumns)
                {
                    col.Item().Row(row =>
                    {
                        for (int j = 0; j < gridColumns && (i + j) < allImages.Count; j++)
                        {
                            var img = allImages[i + j];
                            row.RelativeItem().Column(imgCol =>
                            {
                                var imgPath = GetImageFullPath(img.Url);
                                if (imgPath != null)
                                {
                                    try
                                    {
                                        imgCol.Item().Height(maxHeight).Image(imgPath).FitArea();
                                    }
                                    catch
                                    {
                                        imgCol.Item().Height(maxHeight).AlignCenter().AlignMiddle()
                                            .Text("Image not found").FontSize(FontSmall).FontColor(Colors.Grey.Medium);
                                    }
                                }

                                if (showCaptions && !string.IsNullOrWhiteSpace(img.Caption))
                                {
                                    imgCol.Item().AlignCenter()
                                        .Text(img.Caption).FontSize(6.5f).Italic();
                                }
                            });
                        }
                    });
                    col.Item().PaddingVertical(3);
                }
            });
        }

        // ────────────────────────────────────────────────
        // SECTION: Custom Text
        // ────────────────────────────────────────────────

        private void RenderCustomText(IContainer container, JsonElement? config)
        {
            var content = GetString(config, "content", "");
            var fontSize = GetInt(config, "fontSize", 8);
            var bold = GetBool(config, "bold", false);
            var italic = GetBool(config, "italic", false);

            if (string.IsNullOrWhiteSpace(content)) return;

            container.Padding(CellPadding).Text(text =>
            {
                var span = text.Span(content).FontSize(fontSize);
                if (bold) span.Bold();
                if (italic) span.Italic();
            });
        }

        // ────────────────────────────────────────────────
        // SECTION: Signature Block
        // ────────────────────────────────────────────────

        private void RenderSignatureBlock(IContainer container, JsonElement? config)
        {
            var count = GetInt(config, "count", 3);
            var showDesignation = GetBool(config, "showDesignation", true);
            var showStamp = GetBool(config, "showStamp", true);
            var roles = GetStringArray(config, "roles", new[] { "Tested By", "Reviewed By", "Approved By" });

            var signatories = new[]
            {
                new { Name = _data.TestedByName, Designation = _data.TestedByDesignation, SignaturePath = _data.TestedBySignaturePath },
                new { Name = _data.ReviewedByName, Designation = _data.ReviewedByDesignation, SignaturePath = _data.ReviewedBySignaturePath },
                new { Name = _data.AuthorizedByName, Designation = _data.AuthorizedByDesignation, SignaturePath = _data.AuthorizedBySignaturePath }
            };

            container.PaddingTop(10).Row(row =>
            {
                for (int i = 0; i < Math.Min(count, signatories.Length); i++)
                {
                    var sig = signatories[i];
                    var role = i < roles.Length ? roles[i] : $"Signatory {i + 1}";
                    var isLast = i == Math.Min(count, signatories.Length) - 1;

                    row.RelativeItem().Column(col =>
                    {
                        // Signature image
                        if (!string.IsNullOrWhiteSpace(sig.SignaturePath))
                        {
                            var sigPath = GetImageFullPath(sig.SignaturePath);
                            if (sigPath != null)
                            {
                                try { col.Item().Height(38).Image(sigPath).FitArea(); }
                                catch { col.Item().Height(38); }
                            }
                            else col.Item().Height(38);
                        }
                        else
                        {
                            col.Item().Height(38);
                        }

                        // Horizontal line
                        col.Item().LineHorizontal(0.5f).LineColor(BorderColor);

                        // Role
                        col.Item().PaddingTop(2).Text(role)
                            .FontSize(6.5f).Bold().FontColor(PrimaryColor);

                        // Name
                        col.Item().Text(Safe(sig.Name)).FontSize(FontTableCell);

                        // Designation
                        if (showDesignation && !string.IsNullOrWhiteSpace(sig.Designation))
                        {
                            col.Item().Text(sig.Designation).FontSize(FontSmall).FontColor(Colors.Grey.Darken1);
                        }

                        // Company stamp (last signatory only)
                        if (showStamp && isLast && !string.IsNullOrWhiteSpace(_data.CompanyStampPath))
                        {
                            var stampPath = GetImageFullPath(_data.CompanyStampPath);
                            if (stampPath != null)
                            {
                                try { col.Item().Height(35).Image(stampPath).FitArea(); }
                                catch { /* skip */ }
                            }
                        }
                    });

                    if (i < Math.Min(count, signatories.Length) - 1)
                        row.ConstantItem(20); // spacer between signatories
                }
            });
        }

        // ────────────────────────────────────────────────
        // SECTION: NABL Scope Note
        // ────────────────────────────────────────────────

        private void RenderScopeNote(IContainer container, JsonElement? config)
        {
            var autoGenerate = GetBool(config, "autoGenerate", true);
            var customText = GetString(config, "customText", "");

            container
                .Border(1).BorderColor("#E65100")
                .Background("#FFF3E0")
                .Padding(6)
                .Column(col =>
                {
                    col.Item().Text("NABL Accreditation Note").FontSize(8).Bold().FontColor("#E65100");

                    col.Item().PaddingTop(3);

                    if (autoGenerate && _data.NablInfo?.OutOfScopeParameterNames?.Any() == true)
                    {
                        var paramNames = string.Join(", ", _data.NablInfo.OutOfScopeParameterNames);
                        col.Item().Text($"The following parameters are NOT covered under NABL Accreditation: {paramNames}. " +
                            "Results for these parameters are reported in our capacity as a non-accredited laboratory.")
                            .FontSize(FontSmall).LineHeight(1.3f);
                    }
                    else if (autoGenerate)
                    {
                        col.Item().Text("All parameters are within NABL Accreditation scope.")
                            .FontSize(FontSmall).LineHeight(1.3f);
                    }

                    if (!string.IsNullOrWhiteSpace(customText))
                    {
                        col.Item().PaddingTop(2).Text(customText).FontSize(FontSmall).LineHeight(1.3f);
                    }
                });
        }

        // ────────────────────────────────────────────────
        // SECTION: QR Code
        // ────────────────────────────────────────────────

        private void RenderQrCode(IContainer container, JsonElement? config)
        {
            var size = GetInt(config, "size", 60);

            if (string.IsNullOrWhiteSpace(_data.QrCodeData)) return;

            container.AlignRight().Width(size).Column(col =>
            {
                try
                {
                    var qrBytes = QrCodeHelper.GenerateQrPng(_data.QrCodeData, 8);
                    if (qrBytes != null && qrBytes.Length > 0)
                    {
                        col.Item().Image(qrBytes).FitArea();
                        col.Item().AlignCenter().Text("Scan to verify").FontSize(5);
                    }
                }
                catch
                {
                    // Fallback: try static QR image
                    var qrPath = Path.Combine(_assetsPath, "qr_code.png");
                    if (File.Exists(qrPath))
                    {
                        try { col.Item().Image(qrPath).FitArea(); }
                        catch { /* skip */ }
                    }
                }
            });
        }

        // ────────────────────────────────────────────────
        // SECTION: Divider
        // ────────────────────────────────────────────────

        private void RenderDivider(IContainer container, JsonElement? config)
        {
            var style = GetString(config, "style", "line");
            var thickness = GetInt(config, "thickness", 1);

            if (style == "space")
                container.PaddingVertical(thickness * 5);
            else
                container.PaddingVertical(3).LineHorizontal(thickness).LineColor(Colors.Grey.Lighten2);
        }

        // ────────────────────────────────────────────────
        // PAGE FOOTER (every page)
        // ────────────────────────────────────────────────

        private void ComposePageFooter(IContainer container, JsonElement? headerConfig)
        {
            var showFooterQr = GetBool(headerConfig, "showFooterQr", false);
            var footerConditionsText = GetString(headerConfig, "footerConditionsText", "");

            // Determine conditions text: use config text if provided, else fall back to DB conditions
            // Quill empty state is "<p><br></p>" — treat as empty
            var strippedConfigText = StripHtmlToPlainText(footerConditionsText);
            var hasConfigConditions = !string.IsNullOrWhiteSpace(strippedConfigText);
            var hasDbConditions = _data.ReportConditions?.Any() == true;

            if (!hasConfigConditions && !hasDbConditions && !showFooterQr) return;

            container.Column(col =>
            {
                col.Item().LineHorizontal(0.8f).LineColor(PrimaryColor);

                col.Item().PaddingTop(2).Row(row =>
                {
                    // LEFT: Conditions text
                    row.RelativeItem().Column(left =>
                    {
                        left.Item().Text("Condition of Reporting:").FontSize(FontFooter).Bold();

                        if (hasConfigConditions)
                        {
                            // Use already-stripped text from Quill HTML
                            var lines = strippedConfigText.Split('\n', StringSplitOptions.RemoveEmptyEntries);
                            int num = 1;
                            foreach (var line in lines)
                            {
                                var trimmed = line.Trim();
                                if (!string.IsNullOrWhiteSpace(trimmed))
                                {
                                    left.Item().Text($"{num}) {trimmed}").FontSize(4.5f).LineHeight(1.2f);
                                    num++;
                                }
                            }
                        }
                        else if (hasDbConditions)
                        {
                            // Fall back to DB-stored conditions
                            int num = 1;
                            foreach (var condition in _data.ReportConditions!)
                            {
                                left.Item().Text($"{num}) {condition}").FontSize(4.5f).LineHeight(1.2f);
                                num++;
                            }
                        }
                    });

                    // RIGHT: QR Code in footer (if enabled)
                    if (showFooterQr && !string.IsNullOrWhiteSpace(_data.QrCodeData))
                    {
                        row.ConstantItem(55).AlignRight().AlignMiddle().Column(qrCol =>
                        {
                            try
                            {
                                var qrBytes = QrCodeHelper.GenerateQrPng(_data.QrCodeData, 6);
                                if (qrBytes != null && qrBytes.Length > 0)
                                {
                                    qrCol.Item().Height(45).Image(qrBytes).FitArea();
                                }
                            }
                            catch
                            {
                                var qrPath = Path.Combine(_assetsPath, "qr_code.png");
                                if (File.Exists(qrPath))
                                {
                                    try { qrCol.Item().Height(45).Image(qrPath).FitArea(); }
                                    catch { /* skip */ }
                                }
                            }
                        });
                    }
                });
            });
        }

        // ────────────────────────────────────────────────
        // HELPER: Render Key-Value Grid
        // ────────────────────────────────────────────────

        private void RenderKeyValueGrid(IContainer container, string title, string[] fields,
            Dictionary<string, (string Label, string Value)> fieldMap, int columns)
        {
            container.Border(CellBorderWidth).BorderColor(BorderColor).Column(col =>
            {
                // Title
                col.Item()
                    .Background(PrimaryColor)
                    .Padding(CellPadding)
                    .Text(title)
                    .FontSize(FontSectionHeader).Bold().FontColor(Colors.White);

                // Grid
                col.Item().Table(table =>
                {
                    table.ColumnsDefinition(c =>
                    {
                        for (int i = 0; i < columns; i++)
                        {
                            c.RelativeColumn(1.2f); // Label
                            c.RelativeColumn(2f);   // Value
                        }
                    });

                    var activeFields = fields.Where(f => fieldMap.ContainsKey(f)).ToList();

                    for (int i = 0; i < activeFields.Count; i += columns)
                    {
                        for (int j = 0; j < columns; j++)
                        {
                            if (i + j < activeFields.Count)
                            {
                                var field = fieldMap[activeFields[i + j]];

                                table.Cell()
                                    .Border(CellBorderWidth).BorderColor(BorderColor)
                                    .Background(HeaderBg)
                                    .Padding(CellPadding)
                                    .Text(field.Label).FontSize(FontLabel).Bold();

                                table.Cell()
                                    .Border(CellBorderWidth).BorderColor(BorderColor)
                                    .Padding(CellPadding)
                                    .Text(field.Value).FontSize(FontTableCell);
                            }
                            else
                            {
                                // Empty cells to fill row
                                table.Cell().Border(CellBorderWidth).BorderColor(BorderColor).Padding(CellPadding);
                                table.Cell().Border(CellBorderWidth).BorderColor(BorderColor).Padding(CellPadding);
                            }
                        }
                    }
                });
            });
        }

        // ────────────────────────────────────────────────
        // FIELD MAPS
        // ────────────────────────────────────────────────

        private Dictionary<string, (string Label, string Value)> BuildCustomerFieldMap()
        {
            return new Dictionary<string, (string, string)>
            {
                ["CustomerName"] = ("Customer Name", Safe(_data.CustomerName)),
                ["Address"] = ("Address", Safe(_data.CustomerAddress)),
                ["GSTNo"] = ("GST No", Safe(_data.CustomerGST)),
                ["POReference"] = ("Reference", Safe(_data.CustomerReference)),
                ["SampleDescription"] = ("Description", Safe(_data.SampleDescription)),
                ["ContactPerson"] = ("Contact Person", ""),
                ["CertificateNo"] = ("Certificate No", Safe(_data.CertificateNo)),
                ["UlrNo"] = ("ULR No", Safe(_data.UlrNo)),
                ["DateOfIssue"] = ("Date of Issue", Safe(_data.DateOfIssue)),
                ["SampleReceivedDate"] = ("Sample Received", Safe(_data.SampleReceivedDate)),
                ["TestPerformedAt"] = ("Test Performed At", Safe(_data.TestPerformedAt)),
                ["StampedAs"] = ("Stamped As", Safe(_data.StampedAs)),
                ["NatureOfSample"] = ("Nature of Sample", Safe(_data.NatureOfSample)),
                ["SampleDrawnBy"] = ("Sample Drawn By", Safe(_data.SampleDrawnBy)),
                ["MaterialSpec"] = ("Specification", Safe(_data.MaterialSpec)),
            };
        }

        private Dictionary<string, (string Label, string Value)> BuildSampleFieldMap()
        {
            return new Dictionary<string, (string, string)>
            {
                ["CaseNo"] = ("Case No", Safe(_data.CaseNo)),
                ["SampleNo"] = ("Sample No", Safe(_data.SampleNo)),
                ["SampleDescription"] = ("Description", Safe(_data.SampleDescription)),
                ["MaterialSpec"] = ("Material Specification", Safe(_data.MaterialSpec)),
                ["Grade"] = ("Grade", Safe(_data.Grade)),
                ["ProductForm"] = ("Product Form", Safe(_data.ProductForm)),
                ["SpecimenOrientation"] = ("Specimen Orientation", Safe(_data.SpecimenOrientation)),
                ["HeatTreatment"] = ("Heat Treatment", Safe(_data.HeatTreatment)),
                ["HeatNo"] = ("Heat No", Safe(_data.HeatNo)),
                ["BatchNo"] = ("Batch No", Safe(_data.BatchNo)),
                ["Quantity"] = ("Quantity", _data.Quantity?.ToString() ?? "-"),
                ["Dimensions"] = ("Dimensions", BuildDimensionString()),
                ["DateReceived"] = ("Date Received", Safe(_data.DateReceived)),
                ["DateTested"] = ("Date Tested", Safe(_data.DateTested)),
                ["DateReported"] = ("Date Reported", Safe(_data.DateReported)),
                ["Thickness"] = ("Thickness", _data.Thickness?.ToString("F2") ?? "-"),
                ["Diameter"] = ("Diameter", _data.Diameter?.ToString("F2") ?? "-"),
                ["Width"] = ("Width", _data.Width?.ToString("F2") ?? "-"),
                ["Length"] = ("Length", _data.Length?.ToString("F2") ?? "-"),
                ["CrossSectionArea"] = ("Cross Section Area", _data.CrossSectionArea?.ToString("F2") ?? "-"),
                ["GaugeLength"] = ("Gauge Length", _data.GaugeLength?.ToString("F2") ?? "-"),
                ["RoomTemperature"] = ("Room Temperature (°C)", _data.RoomTemperature?.ToString("F1") ?? "-"),
                ["RoomHumidity"] = ("Room Humidity (%)", _data.RoomHumidity?.ToString("F1") ?? "-"),
                ["EquipmentUsed"] = ("Equipment Used", Safe(_data.EquipmentUsed)),
                ["LabRoom"] = ("Lab Room", Safe(_data.LabRoom)),
            };
        }

        // ────────────────────────────────────────────────
        // TABLE HELPERS
        // ────────────────────────────────────────────────

        private enum HorizontalAlignment { Left, Center, Right }

        private static void AddHeaderCell(TableDescriptor table, string text)
        {
            table.Cell()
                .Border(CellBorderWidth).BorderColor(BorderColor)
                .Background(PrimaryColor)
                .Padding(CellPadding)
                .AlignCenter()
                .Text(text).FontSize(FontTableHeader).Bold().FontColor(Colors.White);
        }

        private static void AddDataCell(TableDescriptor table, string text, string bgColor,
            HorizontalAlignment align = HorizontalAlignment.Left)
        {
            var cell = table.Cell()
                .Border(CellBorderWidth).BorderColor(BorderColor)
                .Background(bgColor)
                .Padding(CellPadding);

            if (align == HorizontalAlignment.Center) cell = cell.AlignCenter();
            else if (align == HorizontalAlignment.Right) cell = cell.AlignRight();

            cell.Text(text).FontSize(FontTableCell);
        }

        private static void AddResultCell(TableDescriptor table, string result, string status, string bgColor)
        {
            var fontColor = status switch
            {
                "Pass" => "#2E7D32",
                "Fail" => "#C62828",
                _ => "#333333"
            };

            table.Cell()
                .Border(CellBorderWidth).BorderColor(BorderColor)
                .Background(bgColor)
                .Padding(CellPadding)
                .AlignCenter()
                .Text(result).FontSize(FontTableCell).Bold().FontColor(fontColor);
        }

        // ────────────────────────────────────────────────
        // UTILITY HELPERS
        // ────────────────────────────────────────────────

        private static string Safe(string? value, string fallback = "-") =>
            string.IsNullOrWhiteSpace(value) ? fallback : value;

        private string BuildDimensionString()
        {
            var parts = new List<string>();
            if (_data.Thickness.HasValue) parts.Add($"Thickness: {_data.Thickness:F2} mm");
            if (_data.Diameter.HasValue) parts.Add($"Diameter: {_data.Diameter:F2} mm");
            if (_data.Width.HasValue) parts.Add($"Width: {_data.Width:F2} mm");
            if (_data.Length.HasValue) parts.Add($"Length: {_data.Length:F2} mm");
            if (_data.CrossSectionArea.HasValue && _data.CrossSectionArea.Value > 0)
                parts.Add($"C/S Area: {_data.CrossSectionArea.Value} mm²");
            if (_data.GaugeLength.HasValue && _data.GaugeLength.Value > 0)
                parts.Add($"G.L.: {_data.GaugeLength.Value} mm");
            return parts.Any() ? string.Join(" | ", parts) : "-";
        }

        private string? ResolveImagePath(string? dtoPath, string? fallbackAssetName)
        {
            if (!string.IsNullOrWhiteSpace(dtoPath))
            {
                var full = GetImageFullPath(dtoPath);
                if (full != null) return full;
            }
            if (!string.IsNullOrWhiteSpace(fallbackAssetName))
            {
                var assetPath = Path.Combine(_assetsPath, fallbackAssetName);
                if (File.Exists(assetPath)) return assetPath;
            }
            return null;
        }

        private string? GetImageFullPath(string? path)
        {
            if (string.IsNullOrWhiteSpace(path)) return null;

            // Absolute path
            if (Path.IsPathRooted(path) && File.Exists(path)) return path;

            // Relative to Assets
            var assetsPath = Path.Combine(_assetsPath, path);
            if (File.Exists(assetsPath)) return assetsPath;

            // Relative to wwwroot
            var wwwrootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", path);
            if (File.Exists(wwwrootPath)) return wwwrootPath;

            // Relative to wwwroot/Uploads
            var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Uploads", path);
            if (File.Exists(uploadsPath)) return uploadsPath;

            return null;
        }

        // ── JSON CONFIG HELPERS ──

        private static JsonElement? GetConfig(ReportFormatSection section)
        {
            if (string.IsNullOrWhiteSpace(section.ConfigJson)) return null;
            try { return JsonDocument.Parse(section.ConfigJson).RootElement; }
            catch { return null; }
        }

        private static string GetString(JsonElement? config, string key, string defaultValue)
        {
            if (config == null) return defaultValue;
            if (config.Value.TryGetProperty(key, out var prop) && prop.ValueKind == JsonValueKind.String)
                return prop.GetString() ?? defaultValue;
            return defaultValue;
        }

        private static bool GetBool(JsonElement? config, string key, bool defaultValue)
        {
            if (config == null) return defaultValue;
            if (config.Value.TryGetProperty(key, out var prop))
            {
                if (prop.ValueKind == JsonValueKind.True) return true;
                if (prop.ValueKind == JsonValueKind.False) return false;
            }
            return defaultValue;
        }

        private static int GetInt(JsonElement? config, string key, int defaultValue)
        {
            if (config == null) return defaultValue;
            if (config.Value.TryGetProperty(key, out var prop))
            {
                if (prop.ValueKind == JsonValueKind.Number)
                    return prop.GetInt32();
                // Handle string-encoded numbers from frontend select elements
                if (prop.ValueKind == JsonValueKind.String && int.TryParse(prop.GetString(), out var parsed))
                    return parsed;
            }
            return defaultValue;
        }

        /// <summary>
        /// Converts Quill HTML to plain text: strips tags, converts &lt;li&gt; / &lt;p&gt; / &lt;br&gt; to newlines, decodes entities.
        /// </summary>
        private static string StripHtmlToPlainText(string html)
        {
            if (string.IsNullOrWhiteSpace(html)) return "";

            // Replace block tags with newlines
            var text = Regex.Replace(html, @"</(p|li|div|br|h[1-6])>", "\n", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, @"<br\s*/?>", "\n", RegexOptions.IgnoreCase);

            // Remove all remaining HTML tags
            text = Regex.Replace(text, @"<[^>]+>", "");

            // Decode common HTML entities
            text = text.Replace("&amp;", "&")
                       .Replace("&lt;", "<")
                       .Replace("&gt;", ">")
                       .Replace("&nbsp;", " ")
                       .Replace("&quot;", "\"");

            // Collapse multiple newlines
            text = Regex.Replace(text, @"\n{3,}", "\n\n");

            return text.Trim();
        }

        private static string[] GetStringArray(JsonElement? config, string key, string[] defaultValue)
        {
            if (config == null) return defaultValue;
            if (config.Value.TryGetProperty(key, out var prop) && prop.ValueKind == JsonValueKind.Array)
            {
                return prop.EnumerateArray()
                    .Where(e => e.ValueKind == JsonValueKind.String)
                    .Select(e => e.GetString()!)
                    .ToArray();
            }
            return defaultValue;
        }
    }
}
