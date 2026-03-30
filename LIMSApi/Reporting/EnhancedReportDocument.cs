using LIMSApi.Dtos;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace LIMSApi.Reporting
{
    /// <summary>
    /// Professional metallurgical lab Test Certificate PDF renderer using QuestPDF Fluent API.
    /// Layout: Header (every page) -> Certificate Title -> Identity Grid -> Customer Info
    ///       -> Test Sections (Chemical pivot / General) -> Images -> End of Report
    ///       -> NABL Scope Note -> Conformity -> Signatures -> QR -> Footer (every page).
    /// All data sourced from ReportDataDto — no hardcoded company information.
    /// </summary>
    public class EnhancedReportDocument : IDocument
    {
        private readonly ReportDataDto _data;
        private readonly string _assetsPath;

        // ────────────────────────────────────────────────
        // STYLE CONSTANTS
        // ────────────────────────────────────────────────

        private const string PrimaryColor = "#B71C1C";
        private const string BorderColor = "#333333";
        private const string HeaderBg = "#F5F5F5";
        private const float CellBorderWidth = 0.5f;
        private const float CellPadding = 3f;

        // Font sizes
        private const float FontCompanyName = 11f;
        private const float FontSectionHeader = 8f;
        private const float FontTableHeader = 7f;
        private const float FontTableCell = 7f;
        private const float FontFooter = 5.5f;
        private const float FontSmall = 6f;
        private const float FontLabel = 7f;

        public EnhancedReportDocument(ReportDataDto data)
        {
            _data = data;
            _assetsPath = Path.Combine(Directory.GetCurrentDirectory(), "Assets");
        }

        public DocumentMetadata GetMetadata() => new()
        {
            Title = _data.ReportNo,
            Author = _data.LabName,
            Creator = "LIMS Report Engine"
        };

        // ────────────────────────────────────────────────
        // COMPOSE — MAIN ENTRY
        // ────────────────────────────────────────────────

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.MarginVertical(15);
                page.MarginHorizontal(20);
                page.DefaultTextStyle(x => x.FontFamily("Arial"));

                page.Header().Element(ComposeHeader);
                page.Content().Element(ComposeContent);
                page.Footer().Element(ComposeFooter);
            });
        }

        // ────────────────────────────────────────────────
        // HEADER (every page)
        // ────────────────────────────────────────────────

        private void ComposeHeader(IContainer container)
        {
            container.Column(col =>
            {
                col.Item()
                    .BorderBottom(1).BorderColor(PrimaryColor)
                    .PaddingBottom(4)
                    .Row(row =>
                    {
                        // LEFT: Company Logo
                        row.ConstantItem(55).AlignMiddle().Column(logoCol =>
                        {
                            var logoPath = ResolveImagePath(_data.LabLogoPath, "logo.png");
                            if (logoPath != null)
                            {
                                try
                                {
                                    logoCol.Item().Height(45).Image(logoPath).FitArea();
                                }
                                catch
                                {
                                    logoCol.Item().Height(45).AlignCenter().AlignMiddle()
                                        .Text("LOGO").FontSize(FontSmall).FontColor(Colors.Grey.Medium);
                                }
                            }
                        });

                        row.ConstantItem(6); // spacer

                        // CENTER: Company name + address + phone/email + CIN
                        row.RelativeItem().AlignMiddle().Column(center =>
                        {
                            center.Item().AlignCenter()
                                .Text(Safe(_data.LabName))
                                .FontSize(FontCompanyName).Bold().FontColor(PrimaryColor);

                            center.Item().AlignCenter()
                                .Text(Safe(_data.LabAddress))
                                .FontSize(6.5f).FontColor(Colors.Grey.Darken2);

                            center.Item().AlignCenter()
                                .Text($"Ph: {Safe(_data.LabPhone)} | Email: {Safe(_data.LabEmail)}")
                                .FontSize(FontSmall).FontColor(Colors.Grey.Darken1);

                            if (!string.IsNullOrWhiteSpace(_data.CIN))
                            {
                                center.Item().AlignCenter()
                                    .Text($"CIN: {_data.CIN}")
                                    .FontSize(FontFooter).FontColor(Colors.Grey.Darken1);
                            }
                        });

                        // RIGHT: NABL Logo + Cert Number
                        row.ConstantItem(80).AlignMiddle().Column(right =>
                        {
                            if (_data.IsNabl || _data.NablInfo?.IsPartialScope == true)
                            {
                                var nablLogoPath = ResolveImagePath(_data.NablLogoPath, "nabl_logo.png");
                                if (nablLogoPath != null)
                                {
                                    try
                                    {
                                        right.Item().Height(32).AlignCenter().Image(nablLogoPath).FitArea();
                                    }
                                    catch
                                    {
                                        right.Item().Height(32).AlignCenter().AlignMiddle()
                                            .Text("NABL").FontSize(FontSmall).Bold();
                                    }
                                }

                                if (!string.IsNullOrWhiteSpace(_data.NablCertNo))
                                {
                                    var certText = _data.NablInfo?.IsPartialScope == true
                                        ? $"{_data.NablCertNo} *"
                                        : _data.NablCertNo;
                                    right.Item().AlignCenter()
                                        .Text(certText).FontSize(FontFooter).Bold();
                                }
                            }
                        });
                    });
            });
        }

        // ────────────────────────────────────────────────
        // CONTENT
        // ────────────────────────────────────────────────

        private void ComposeContent(IContainer container)
        {
            container.PaddingVertical(4).Column(col =>
            {
                // 1. Certificate Title
                col.Item().Element(ComposeCertificateTitle);

                col.Item().PaddingVertical(2);

                // 2. Certificate Identity Grid
                col.Item().Element(ComposeCertificateIdentityGrid);

                col.Item().PaddingVertical(2);

                // 3. Customer Provided Info
                col.Item().Element(ComposeCustomerProvidedInfo);

                col.Item().PaddingVertical(2);

                // 4. Test Sections
                if (_data.TestSections?.Any() == true)
                {
                    foreach (var section in _data.TestSections)
                    {
                        col.Item().Element(c => ComposeTestSection(c, section));
                        col.Item().PaddingVertical(2);
                    }
                }

                // 5. End of Report
                col.Item().Element(ComposeEndOfReport);

                col.Item().PaddingVertical(2);

                // 6. NABL Scope Note
                if (_data.NablInfo?.OutOfScopeParameterNames?.Any() == true)
                {
                    col.Item().Element(ComposeNablScopeNote);
                    col.Item().PaddingVertical(2);
                }

                // 7. Conformity Statement
                if (_data.StatementOfConformity == "Applicable")
                {
                    col.Item().Element(ComposeConformityStatement);
                    col.Item().PaddingVertical(2);
                }

                // 8. Remarks
                if (!string.IsNullOrWhiteSpace(_data.Remarks))
                {
                    col.Item().Element(ComposeRemarks);
                    col.Item().PaddingVertical(2);
                }

                // 9. Signatures
                col.Item().Element(ComposeSignatures);

                col.Item().PaddingVertical(4);

                // 10. QR Code
                col.Item().Element(ComposeQrCode);
            });
        }

        // ────────────────────────────────────────────────
        // 1. CERTIFICATE TITLE
        // ────────────────────────────────────────────────

        private void ComposeCertificateTitle(IContainer container)
        {
            container
                .Background(HeaderBg)
                .Border(CellBorderWidth).BorderColor(BorderColor)
                .PaddingVertical(5)
                .AlignCenter()
                .Text("Test Certificate")
                .FontSize(12).Bold().FontColor(PrimaryColor);
        }

        // ────────────────────────────────────────────────
        // 2. CERTIFICATE IDENTITY GRID
        // ────────────────────────────────────────────────

        private void ComposeCertificateIdentityGrid(IContainer container)
        {
            container.Border(CellBorderWidth).BorderColor(BorderColor).Table(table =>
            {
                table.ColumnsDefinition(c =>
                {
                    c.RelativeColumn(1.2f); // Label left
                    c.RelativeColumn(2f);   // Value left
                    c.RelativeColumn(1.2f); // Label right
                    c.RelativeColumn(2f);   // Value right
                });

                AddIdentityRow(table, "ULR No", Safe(_data.UlrNo), "Date of Issue", Safe(_data.DateOfIssue));
                AddIdentityRow(table, "Certificate No", Safe(_data.CertificateNo), "Page", null, isPageNumber: true);
                AddIdentityRow(table, "Customer Name", Safe(_data.CustomerName), "Sample Received", Safe(_data.SampleReceivedDate));
                AddIdentityRow(table, "Customer Address", Safe(_data.CustomerAddress), "Test Performed At", Safe(_data.TestPerformedAt));
            });
        }

        private static void AddIdentityRow(TableDescriptor table, string label1, string value1,
            string label2, string? value2, bool isPageNumber = false)
        {
            // Label 1
            table.Cell()
                .Border(CellBorderWidth).BorderColor(BorderColor)
                .Background(HeaderBg)
                .Padding(CellPadding)
                .Text(label1).FontSize(FontLabel).Bold();

            // Value 1
            table.Cell()
                .Border(CellBorderWidth).BorderColor(BorderColor)
                .Padding(CellPadding)
                .Text(value1).FontSize(FontTableCell);

            // Label 2
            table.Cell()
                .Border(CellBorderWidth).BorderColor(BorderColor)
                .Background(HeaderBg)
                .Padding(CellPadding)
                .Text(label2).FontSize(FontLabel).Bold();

            // Value 2 — or page number
            if (isPageNumber)
            {
                table.Cell()
                    .Border(CellBorderWidth).BorderColor(BorderColor)
                    .Padding(CellPadding)
                    .Text(t =>
                    {
                        t.CurrentPageNumber().FontSize(FontTableCell);
                        t.Span(" of ").FontSize(FontTableCell);
                        t.TotalPages().FontSize(FontTableCell);
                    });
            }
            else
            {
                table.Cell()
                    .Border(CellBorderWidth).BorderColor(BorderColor)
                    .Padding(CellPadding)
                    .Text(value2 ?? "-").FontSize(FontTableCell);
            }
        }

        // ────────────────────────────────────────────────
        // 3. CUSTOMER PROVIDED INFORMATION
        // ────────────────────────────────────────────────

        private void ComposeCustomerProvidedInfo(IContainer container)
        {
            container.Border(CellBorderWidth).BorderColor(BorderColor).Column(col =>
            {
                // Section header
                col.Item()
                    .Background(PrimaryColor)
                    .Padding(CellPadding)
                    .AlignCenter()
                    .Text("INFORMATION PROVIDED BY THE CUSTOMER")
                    .FontSize(FontSectionHeader).Bold().FontColor(Colors.White);

                col.Item().Table(table =>
                {
                    table.ColumnsDefinition(c =>
                    {
                        c.RelativeColumn(1.2f); // Label left
                        c.RelativeColumn(2f);   // Value left
                        c.RelativeColumn(1.2f); // Label right
                        c.RelativeColumn(2f);   // Value right
                    });

                    // Reference (full width value)
                    AddCustomerInfoFullRow(table, "Reference", Safe(_data.CustomerReference));

                    // Description (full width value)
                    AddCustomerInfoFullRow(table, "Description", Safe(_data.SampleDescription));

                    // Stamped As + Sample Drawn By
                    AddCustomerInfoRow(table, "Stamped As", Safe(_data.StampedAs),
                        "Sample Drawn By", Safe(_data.SampleDrawnBy));

                    // Nature of Sample (full width value)
                    AddCustomerInfoFullRow(table, "Nature", Safe(_data.NatureOfSample));

                    // Specification
                    var specValue = !string.IsNullOrWhiteSpace(_data.MaterialSpec)
                        ? $"{_data.MaterialSpec}{(!string.IsNullOrWhiteSpace(_data.Grade) ? $" - {_data.Grade}" : "")}"
                        : Safe(_data.Grade);
                    AddCustomerInfoFullRow(table, "Specification", specValue);

                    // Dimensions (if any provided)
                    var dimensions = BuildDimensionString();
                    if (!string.IsNullOrWhiteSpace(dimensions))
                    {
                        AddCustomerInfoFullRow(table, "Dimensions", dimensions);
                    }
                });
            });
        }

        private static void AddCustomerInfoRow(TableDescriptor table,
            string label1, string value1, string label2, string value2)
        {
            table.Cell()
                .Border(CellBorderWidth).BorderColor(BorderColor)
                .Background(HeaderBg)
                .Padding(CellPadding)
                .Text(label1).FontSize(FontLabel).Bold();

            table.Cell()
                .Border(CellBorderWidth).BorderColor(BorderColor)
                .Padding(CellPadding)
                .Text(value1).FontSize(FontTableCell);

            table.Cell()
                .Border(CellBorderWidth).BorderColor(BorderColor)
                .Background(HeaderBg)
                .Padding(CellPadding)
                .Text(label2).FontSize(FontLabel).Bold();

            table.Cell()
                .Border(CellBorderWidth).BorderColor(BorderColor)
                .Padding(CellPadding)
                .Text(value2).FontSize(FontTableCell);
        }

        private static void AddCustomerInfoFullRow(TableDescriptor table, string label, string value)
        {
            table.Cell()
                .Border(CellBorderWidth).BorderColor(BorderColor)
                .Background(HeaderBg)
                .Padding(CellPadding)
                .Text(label).FontSize(FontLabel).Bold();

            table.Cell().ColumnSpan(3)
                .Border(CellBorderWidth).BorderColor(BorderColor)
                .Padding(CellPadding)
                .Text(value).FontSize(FontTableCell);
        }

        // ────────────────────────────────────────────────
        // 4. TEST SECTIONS
        // ────────────────────────────────────────────────

        private void ComposeTestSection(IContainer container, ReportDataTestSection section)
        {
            container.Border(CellBorderWidth).BorderColor(BorderColor).Column(col =>
            {
                // Section category banner
                col.Item()
                    .Background(PrimaryColor)
                    .Padding(CellPadding)
                    .Row(bannerRow =>
                    {
                        bannerRow.RelativeItem().Text(
                            !string.IsNullOrWhiteSpace(section.TestCategory)
                                ? section.TestCategory
                                : "TEST RESULTS")
                            .FontSize(FontSectionHeader).Bold().FontColor(Colors.White);

                        bannerRow.RelativeItem().AlignRight()
                            .Text("METALS & ALLOYS")
                            .FontSize(FontSectionHeader).Bold().FontColor(Colors.White);
                    });

                // Sub-header: Test Name | Test Method | Date of Testing
                col.Item()
                    .Background(HeaderBg)
                    .BorderBottom(CellBorderWidth).BorderColor(BorderColor)
                    .Padding(CellPadding)
                    .Row(subRow =>
                    {
                        subRow.RelativeItem().Text(t =>
                        {
                            t.Span("Test Name: ").FontSize(FontLabel).Bold();
                            t.Span(Safe(section.TestName)).FontSize(FontTableCell);
                        });

                        if (!string.IsNullOrWhiteSpace(section.TestMethod))
                        {
                            subRow.RelativeItem().AlignCenter().Text(t =>
                            {
                                t.Span("Test Method: ").FontSize(FontLabel).Bold();
                                t.Span(section.TestMethod).FontSize(FontTableCell);
                            });
                        }

                        if (!string.IsNullOrWhiteSpace(section.DateOfTesting))
                        {
                            subRow.RelativeItem().AlignRight().Text(t =>
                            {
                                t.Span("Dt. of Testing: ").FontSize(FontLabel).Bold();
                                t.Span(section.DateOfTesting).FontSize(FontTableCell);
                            });
                        }
                    });

                // Specification name (if present)
                if (!string.IsNullOrWhiteSpace(section.SpecificationName))
                {
                    col.Item()
                        .PaddingHorizontal(CellPadding).PaddingVertical(1)
                        .Text(t =>
                        {
                            t.Span("Specification: ").FontSize(FontLabel).Bold();
                            t.Span(section.SpecificationName).FontSize(FontTableCell);
                        });
                }

                // Parameters table
                if (!section.Parameters.Any())
                {
                    col.Item().Padding(6)
                        .Text("No parameters recorded.")
                        .FontSize(FontSmall).FontColor(Colors.Grey.Darken1);
                }
                else if (section.TestType == "Chemical")
                {
                    col.Item().Element(c => ComposeChemicalTable(c, section.Parameters));
                }
                else
                {
                    col.Item().Element(c => ComposeGeneralTable(c, section.Parameters));
                }

                // Images (if any, 2 per row)
                if (section.Images?.Any() == true)
                {
                    col.Item().Element(c => ComposeTestImages(c, section));
                }
            });
        }

        /// <summary>
        /// General / Mechanical test table: Sr. | Parameter | Unit | Spec Min | Spec Max | Result
        /// </summary>
        private void ComposeGeneralTable(IContainer container, List<ReportDataParameter> parameters)
        {
            container.Padding(2).Table(table =>
            {
                table.ColumnsDefinition(c =>
                {
                    c.ConstantColumn(25);   // Sr.
                    c.RelativeColumn(3f);   // Parameter
                    c.RelativeColumn(1f);   // Unit
                    c.RelativeColumn(1.2f); // Spec Min
                    c.RelativeColumn(1.2f); // Spec Max
                    c.RelativeColumn(1.5f); // Result
                });

                // Header
                table.Header(h =>
                {
                    AddHeaderCell(h, "Sr.");
                    AddHeaderCell(h, "Parameter");
                    AddHeaderCell(h, "Unit");
                    AddHeaderCell(h, "Spec Min");
                    AddHeaderCell(h, "Spec Max");
                    AddHeaderCell(h, "Result");
                });

                // Rows
                for (int i = 0; i < parameters.Count; i++)
                {
                    var p = parameters[i];
                    var bgColor = i % 2 == 0 ? "#FFFFFF" : "#FAFAFA";

                    AddDataCell(table, (i + 1).ToString(), bgColor, HorizontalAlignment.Center);
                    AddDataCell(table, Safe(p.Name), bgColor);
                    AddDataCell(table, Safe(p.Unit), bgColor, HorizontalAlignment.Center);
                    AddDataCell(table, Safe(p.SpecMin), bgColor, HorizontalAlignment.Center);
                    AddDataCell(table, Safe(p.SpecMax), bgColor, HorizontalAlignment.Center);
                    AddResultCell(table, Safe(p.Result), p.Status, bgColor);
                }
            });
        }

        /// <summary>
        /// Chemical test table with multi-column pivot by SubGroup.
        /// If all SubGroup values are null/empty, renders single-column: Element | Unit | Spec Min | Spec Max | Result.
        /// If multiple SubGroups exist, pivots: Element | Unit | Group1 | Group2 | ...
        /// </summary>
        private void ComposeChemicalTable(IContainer container, List<ReportDataParameter> parameters)
        {
            var subGroups = parameters
                .Where(p => !string.IsNullOrWhiteSpace(p.SubGroup))
                .Select(p => p.SubGroup!)
                .Distinct()
                .OrderBy(g => g)
                .ToList();

            if (subGroups.Count <= 1)
            {
                // Single group or no SubGroup — flat table
                ComposeChemicalTableFlat(container, parameters);
            }
            else
            {
                // Multi-group pivot
                ComposeChemicalTablePivot(container, parameters, subGroups);
            }
        }

        private void ComposeChemicalTableFlat(IContainer container, List<ReportDataParameter> parameters)
        {
            container.Padding(2).Table(table =>
            {
                table.ColumnsDefinition(c =>
                {
                    c.RelativeColumn(2f);   // Element
                    c.RelativeColumn(1f);   // Unit
                    c.RelativeColumn(1.2f); // Spec Min
                    c.RelativeColumn(1.2f); // Spec Max
                    c.RelativeColumn(1.5f); // Result
                });

                table.Header(h =>
                {
                    AddHeaderCell(h, "Element");
                    AddHeaderCell(h, "Unit");
                    AddHeaderCell(h, "Spec Min");
                    AddHeaderCell(h, "Spec Max");
                    AddHeaderCell(h, "Result");
                });

                for (int i = 0; i < parameters.Count; i++)
                {
                    var p = parameters[i];
                    var bgColor = i % 2 == 0 ? "#FFFFFF" : "#FAFAFA";

                    AddDataCell(table, Safe(p.Name), bgColor);
                    AddDataCell(table, Safe(p.Unit), bgColor, HorizontalAlignment.Center);
                    AddDataCell(table, Safe(p.SpecMin), bgColor, HorizontalAlignment.Center);
                    AddDataCell(table, Safe(p.SpecMax), bgColor, HorizontalAlignment.Center);
                    AddResultCell(table, Safe(p.Result), p.Status, bgColor);
                }
            });
        }

        private void ComposeChemicalTablePivot(IContainer container,
            List<ReportDataParameter> parameters, List<string> subGroups)
        {
            // Get unique element names preserving original order
            var elementNames = parameters
                .Select(p => p.Name)
                .Distinct()
                .ToList();

            // Build lookup: (Element, SubGroup) -> parameter
            var lookup = parameters
                .GroupBy(p => (p.Name, p.SubGroup ?? ""))
                .ToDictionary(g => g.Key, g => g.First());

            container.Padding(2).Table(table =>
            {
                // Columns: Element | Unit | SubGroup1 | SubGroup2 | ...
                table.ColumnsDefinition(c =>
                {
                    c.RelativeColumn(2f);  // Element
                    c.RelativeColumn(1f);  // Unit
                    foreach (var _ in subGroups)
                        c.RelativeColumn(1.5f); // One column per subgroup
                });

                table.Header(h =>
                {
                    AddHeaderCell(h, "Element");
                    AddHeaderCell(h, "Unit");
                    foreach (var group in subGroups)
                        AddHeaderCell(h, group);
                });

                for (int i = 0; i < elementNames.Count; i++)
                {
                    var elementName = elementNames[i];
                    var bgColor = i % 2 == 0 ? "#FFFFFF" : "#FAFAFA";

                    // Find any parameter for this element to get the unit
                    var anyParam = parameters.First(p => p.Name == elementName);

                    AddDataCell(table, elementName, bgColor);
                    AddDataCell(table, Safe(anyParam.Unit), bgColor, HorizontalAlignment.Center);

                    foreach (var group in subGroups)
                    {
                        if (lookup.TryGetValue((elementName, group), out var param))
                        {
                            AddResultCell(table, Safe(param.Result), param.Status, bgColor);
                        }
                        else
                        {
                            AddDataCell(table, "-", bgColor, HorizontalAlignment.Center);
                        }
                    }
                }
            });
        }

        // ────────────────────────────────────────────────
        // TEST IMAGES (within a section)
        // ────────────────────────────────────────────────

        private void ComposeTestImages(IContainer container, ReportDataTestSection section)
        {
            container.PaddingVertical(4).PaddingHorizontal(6).Column(imgCol =>
            {
                imgCol.Item().PaddingBottom(2)
                    .Text($"Test Images — {section.TestName}")
                    .FontSize(FontLabel).Bold().FontColor(PrimaryColor);

                const int ImagesPerRow = 2;
                const float ImageHeight = 140;

                for (int i = 0; i < section.Images.Count; i += ImagesPerRow)
                {
                    var rowImages = section.Images.Skip(i).Take(ImagesPerRow).ToList();

                    imgCol.Item().Row(row =>
                    {
                        foreach (var img in rowImages)
                        {
                            row.RelativeItem().Padding(2).Column(c =>
                            {
                                var imgPath = GetImageFullPath(img.Url);
                                if (imgPath != null && File.Exists(imgPath))
                                {
                                    try
                                    {
                                        c.Item().Height(ImageHeight).AlignCenter()
                                            .Image(imgPath).FitArea();
                                    }
                                    catch
                                    {
                                        c.Item().Height(ImageHeight).AlignCenter().AlignMiddle()
                                            .Background(Colors.Grey.Lighten3)
                                            .Text("Image could not be loaded")
                                            .FontSize(FontSmall).FontColor(Colors.Grey.Darken1);
                                    }
                                }
                                else
                                {
                                    c.Item().Height(ImageHeight).AlignCenter().AlignMiddle()
                                        .Background(Colors.Grey.Lighten3)
                                        .Text("Image not found")
                                        .FontSize(FontSmall).FontColor(Colors.Grey.Darken1);
                                }

                                if (!string.IsNullOrWhiteSpace(img.Caption))
                                {
                                    c.Item().PaddingTop(2).AlignCenter()
                                        .Text(img.Caption).FontSize(6.5f).Italic();
                                }
                            });
                        }

                        // Fill empty columns
                        for (int j = rowImages.Count; j < ImagesPerRow; j++)
                            row.RelativeItem();
                    });
                }
            });
        }

        // ────────────────────────────────────────────────
        // 5. END OF REPORT
        // ────────────────────────────────────────────────

        private void ComposeEndOfReport(IContainer container)
        {
            container.Column(col =>
            {
                col.Item().PaddingVertical(4).AlignCenter()
                    .Text("——— End of Report ———")
                    .FontSize(FontSectionHeader).Bold().FontColor(Colors.Grey.Darken2);

                if (!string.IsNullOrWhiteSpace(_data.TestPerformedAt) &&
                    _data.TestPerformedAt.Contains("Witness", StringComparison.OrdinalIgnoreCase))
                {
                    col.Item().AlignCenter()
                        .Text($"Test Witnessed By: {Safe(_data.TestPerformedAt)}")
                        .FontSize(FontTableCell).Italic();
                }
            });
        }

        // ────────────────────────────────────────────────
        // 6. NABL SCOPE NOTE
        // ────────────────────────────────────────────────

        private void ComposeNablScopeNote(IContainer container)
        {
            container
                .Border(1f).BorderColor("#E65100") // orange border
                .Background("#FFF3E0")
                .Padding(6)
                .Column(noteCol =>
                {
                    noteCol.Item().Text("NABL Accreditation Note")
                        .FontSize(FontSectionHeader).Bold().FontColor("#E65100");

                    noteCol.Item().PaddingTop(2).Text(text =>
                    {
                        text.Span("The following parameters are NOT covered under NABL Accreditation: ")
                            .FontSize(FontTableCell);
                        text.Span(string.Join(", ", _data.NablInfo!.OutOfScopeParameterNames))
                            .FontSize(FontTableCell).Bold();
                        text.Span(". Results for these parameters are reported in our capacity as a non-accredited laboratory.")
                            .FontSize(FontTableCell);
                    });

                    if (_data.NablInfo.IsPartialScope)
                    {
                        noteCol.Item().PaddingTop(2)
                            .Text("* Partial NABL accreditation — see NABL column in results table for scope details.")
                            .FontSize(FontSmall).Italic().FontColor(Colors.Grey.Darken2);
                    }
                });
        }

        // ────────────────────────────────────────────────
        // 7. CONFORMITY STATEMENT
        // ────────────────────────────────────────────────

        private void ComposeConformityStatement(IContainer container)
        {
            container.Border(CellBorderWidth).BorderColor(BorderColor).Column(col =>
            {
                col.Item()
                    .Background(HeaderBg)
                    .BorderBottom(CellBorderWidth).BorderColor(BorderColor)
                    .Padding(CellPadding)
                    .Text("Statement of Conformity")
                    .FontSize(FontSectionHeader).Bold();

                col.Item().Padding(6).Column(inner =>
                {
                    inner.Item().Text(t =>
                    {
                        t.Span("Decision Rule: ").FontSize(FontTableCell).Bold();
                        t.Span(Safe(_data.DecisionRule, "Not specified")).FontSize(FontTableCell);
                    });

                    // Overall conformity assessment
                    var allParams = _data.TestSections?.SelectMany(s => s.Parameters).ToList() ?? new();
                    var hasNonConforming = allParams.Any(p =>
                        p.ConformityResult != null &&
                        p.ConformityResult.Contains("not conform", StringComparison.OrdinalIgnoreCase));

                    var overallConformity = hasNonConforming
                        ? "The test results do not conform to the specification requirements."
                        : "The test results conform to the specification requirements.";

                    var conformityColor = hasNonConforming ? "#C62828" : "#2E7D32";

                    inner.Item().PaddingTop(3).Text(t =>
                    {
                        t.Span("Overall Result: ").FontSize(FontTableCell).Bold();
                        t.Span(overallConformity).FontSize(FontTableCell).FontColor(conformityColor).Bold();
                    });

                    // Measurement uncertainty note
                    if (_data.DecisionRule != null &&
                        _data.DecisionRule.Contains("MOU", StringComparison.OrdinalIgnoreCase))
                    {
                        inner.Item().PaddingTop(2)
                            .Text("Note: Expanded uncertainty (U) at 95% confidence level has been considered in the conformity assessment.")
                            .FontSize(FontSmall).Italic().FontColor(Colors.Grey.Darken1);
                    }
                });
            });
        }

        // ────────────────────────────────────────────────
        // 8. REMARKS
        // ────────────────────────────────────────────────

        private void ComposeRemarks(IContainer container)
        {
            container
                .Border(CellBorderWidth).BorderColor(BorderColor)
                .Column(col =>
                {
                    col.Item()
                        .Background(HeaderBg)
                        .BorderBottom(CellBorderWidth).BorderColor(BorderColor)
                        .Padding(CellPadding)
                        .Text("Remarks & Observations")
                        .FontSize(FontSectionHeader).Bold();

                    col.Item()
                        .Padding(6)
                        .Text(_data.Remarks!)
                        .FontSize(FontTableCell)
                        .LineHeight(1.3f);
                });
        }

        // ────────────────────────────────────────────────
        // 9. SIGNATURES
        // ────────────────────────────────────────────────

        private void ComposeSignatures(IContainer container)
        {
            container.PaddingTop(8).Row(row =>
            {
                // LEFT: Tested By
                row.RelativeItem().Element(c => RenderSignatureBlock(c,
                    "Tested By",
                    _data.TestedByName,
                    _data.TestedByDesignation,
                    _data.TestedBySignaturePath,
                    showStamp: false));

                row.ConstantItem(20); // spacer

                // RIGHT: Reviewed & Authorized By
                row.RelativeItem().Element(c => RenderSignatureBlock(c,
                    "Reviewed & Authorized By",
                    !string.IsNullOrWhiteSpace(_data.AuthorizedByName) ? _data.AuthorizedByName : _data.ReviewedByName,
                    !string.IsNullOrWhiteSpace(_data.AuthorizedByDesignation) ? _data.AuthorizedByDesignation : _data.ReviewedByDesignation,
                    !string.IsNullOrWhiteSpace(_data.AuthorizedBySignaturePath) ? _data.AuthorizedBySignaturePath : _data.ReviewedBySignaturePath,
                    showStamp: true));
            });
        }

        private void RenderSignatureBlock(IContainer container, string title,
            string name, string? designation, string? signaturePath, bool showStamp)
        {
            container.Column(c =>
            {
                // Signature image area
                c.Item().Height(40).AlignCenter().Column(sigCol =>
                {
                    if (!string.IsNullOrWhiteSpace(signaturePath))
                    {
                        var fullPath = GetImageFullPath(signaturePath);
                        if (fullPath != null && File.Exists(fullPath))
                        {
                            try
                            {
                                sigCol.Item().Height(38).AlignCenter()
                                    .Image(fullPath).FitArea();
                            }
                            catch
                            {
                                // Signature image failed to load — leave blank
                            }
                        }
                    }
                });

                // Signature line
                c.Item().LineHorizontal(0.5f).LineColor(Colors.Grey.Darken1);

                // Title
                c.Item().PaddingTop(2).AlignCenter()
                    .Text(title).FontSize(6.5f).Bold().FontColor(PrimaryColor);

                // Name
                if (!string.IsNullOrWhiteSpace(name))
                {
                    c.Item().AlignCenter()
                        .Text(name).FontSize(FontTableCell);
                }

                // Designation
                if (!string.IsNullOrWhiteSpace(designation))
                {
                    c.Item().AlignCenter()
                        .Text(designation).FontSize(FontSmall).FontColor(Colors.Grey.Darken1);
                }

                // Company stamp (right block only)
                if (showStamp && !string.IsNullOrWhiteSpace(_data.CompanyStampPath))
                {
                    var stampPath = GetImageFullPath(_data.CompanyStampPath);
                    if (stampPath != null && File.Exists(stampPath))
                    {
                        try
                        {
                            c.Item().PaddingTop(3).Height(35).AlignCenter()
                                .Image(stampPath).FitArea();
                        }
                        catch
                        {
                            // Stamp image failed to load
                        }
                    }
                }
            });
        }

        // ────────────────────────────────────────────────
        // 10. QR CODE
        // ────────────────────────────────────────────────

        private void ComposeQrCode(IContainer container)
        {
            if (string.IsNullOrWhiteSpace(_data.QrCodeData))
                return;

            // Try QR code image from assets
            var qrPath = Path.Combine(_assetsPath, "qr_code.png");
            if (!File.Exists(qrPath))
                return;

            container.AlignRight().Row(row =>
            {
                row.RelativeItem(); // push to right

                row.ConstantItem(60).Column(col =>
                {
                    try
                    {
                        col.Item().Height(55).Image(qrPath).FitArea();
                    }
                    catch
                    {
                        col.Item().Height(55).AlignCenter().AlignMiddle()
                            .Text("QR").FontSize(FontSmall);
                    }

                    col.Item().AlignCenter()
                        .Text("Scan to verify").FontSize(5f).FontColor(Colors.Grey.Darken1);
                });
            });
        }

        // ────────────────────────────────────────────────
        // FOOTER (every page)
        // ────────────────────────────────────────────────

        private void ComposeFooter(IContainer container)
        {
            container.Column(col =>
            {
                col.Item().LineHorizontal(0.8f).LineColor(PrimaryColor);

                col.Item().PaddingTop(2).PaddingHorizontal(4).Column(footerContent =>
                {
                    // Report Conditions (numbered list)
                    if (_data.ReportConditions?.Any() == true)
                    {
                        footerContent.Item().PaddingBottom(1)
                            .Text("Conditions of Reporting:")
                            .FontSize(FontFooter).Bold().FontColor(Colors.Grey.Darken2);

                        for (int i = 0; i < _data.ReportConditions.Count; i++)
                        {
                            footerContent.Item()
                                .Text($"  {i + 1}. {_data.ReportConditions[i]}")
                                .FontSize(FontFooter).FontColor(Colors.Grey.Darken1);
                        }
                    }

                    // Conformity note in footer
                    if (_data.StatementOfConformity == "Applicable")
                    {
                        footerContent.Item().PaddingTop(1)
                            .Text("Statement of conformity is based on the decision rule applied. See conformity section in the report.")
                            .FontSize(FontFooter).Italic().FontColor(Colors.Grey.Darken1);
                    }

                    // NABL note
                    if (_data.IsNabl)
                    {
                        footerContent.Item().PaddingTop(1)
                            .Text("The results reported relate only to the items tested. NABL accredited as per ISO/IEC 17025:2017.")
                            .FontSize(FontFooter).FontColor(Colors.Grey.Darken1);
                    }

                    // Reproduction note
                    footerContent.Item().PaddingTop(1)
                        .Text(t =>
                        {
                            t.Span($"This report shall not be reproduced except in full without written approval of {Safe(_data.LabName)}.")
                                .FontSize(FontFooter).FontColor(Colors.Grey.Darken1).Italic();
                        });
                });

                // Page X of Y — right-aligned
                col.Item().PaddingTop(2).PaddingHorizontal(4).AlignRight()
                    .Text(t =>
                    {
                        t.Span("Page ").FontSize(FontFooter).FontColor(Colors.Grey.Darken1);
                        t.CurrentPageNumber().FontSize(FontFooter).FontColor(Colors.Grey.Darken1);
                        t.Span(" of ").FontSize(FontFooter).FontColor(Colors.Grey.Darken1);
                        t.TotalPages().FontSize(FontFooter).FontColor(Colors.Grey.Darken1);
                    });
            });
        }

        // ────────────────────────────────────────────────
        // TABLE CELL HELPERS
        // ────────────────────────────────────────────────

        private static void AddHeaderCell(dynamic header, string text)
        {
            header.Cell()
                .Element(new Action<IContainer>(c =>
                    c.Background(PrimaryColor)
                     .Border(CellBorderWidth).BorderColor(BorderColor)
                     .Padding(CellPadding)
                     .AlignCenter()
                     .Text(text)
                     .FontSize(FontTableHeader).Bold().FontColor(Colors.White)));
        }

        private static void AddDataCell(TableDescriptor table, string text, string bgColor,
            HorizontalAlignment alignment = HorizontalAlignment.Left)
        {
            var cell = table.Cell()
                .Background(bgColor)
                .Border(CellBorderWidth).BorderColor(BorderColor)
                .Padding(CellPadding);

            if (alignment == HorizontalAlignment.Center)
                cell = cell.AlignCenter();
            else if (alignment == HorizontalAlignment.Right)
                cell = cell.AlignRight();

            cell.Text(text).FontSize(FontTableCell);
        }

        private static void AddResultCell(TableDescriptor table, string result, string status, string bgColor)
        {
            string fontColor = status switch
            {
                "Pass" => "#2E7D32",
                "Fail" => "#C62828",
                _ => "#333333"
            };

            table.Cell()
                .Background(bgColor)
                .Border(CellBorderWidth).BorderColor(BorderColor)
                .Padding(CellPadding)
                .AlignCenter()
                .Text(result)
                .FontSize(FontTableCell).Bold().FontColor(fontColor);
        }

        // ────────────────────────────────────────────────
        // UTILITY HELPERS
        // ────────────────────────────────────────────────

        private enum HorizontalAlignment { Left, Center, Right }

        /// <summary>
        /// Returns a non-null display string. Empty/null values become "-" (or the specified fallback).
        /// </summary>
        private static string Safe(string? value, string fallback = "-")
        {
            return string.IsNullOrWhiteSpace(value) ? fallback : value;
        }

        /// <summary>
        /// Builds a dimension string from available Thickness, Diameter, Width, Length values.
        /// </summary>
        private string BuildDimensionString()
        {
            var parts = new List<string>();
            if (_data.Thickness.HasValue && _data.Thickness.Value > 0)
                parts.Add($"Thickness: {_data.Thickness.Value} mm");
            if (_data.Diameter.HasValue && _data.Diameter.Value > 0)
                parts.Add($"Diameter: {_data.Diameter.Value} mm");
            if (_data.Width.HasValue && _data.Width.Value > 0)
                parts.Add($"Width: {_data.Width.Value} mm");
            if (_data.Length.HasValue && _data.Length.Value > 0)
                parts.Add($"Length: {_data.Length.Value} mm");
            return string.Join(" | ", parts);
        }

        /// <summary>
        /// Resolves an image path with fallback: DTO path → Assets fallback → null.
        /// Checks File.Exists before returning.
        /// </summary>
        private string? ResolveImagePath(string? dtoPath, string assetsFallback)
        {
            // Try the DTO-provided path first
            if (!string.IsNullOrWhiteSpace(dtoPath))
            {
                var resolved = GetImageFullPath(dtoPath);
                if (resolved != null && File.Exists(resolved))
                    return resolved;
            }

            // Try assets fallback
            var fallbackPath = Path.Combine(_assetsPath, assetsFallback);
            if (File.Exists(fallbackPath))
                return fallbackPath;

            return null;
        }

        /// <summary>
        /// Resolves image paths: absolute path → Assets/ relative → wwwroot/ relative → null.
        /// </summary>
        private string? GetImageFullPath(string? path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return null;

            // Already absolute and exists
            if (Path.IsPathRooted(path) && File.Exists(path))
                return path;

            // Try relative to Assets
            var assetsRelative = Path.Combine(_assetsPath, path);
            if (File.Exists(assetsRelative))
                return assetsRelative;

            // Try relative to wwwroot
            var wwwrootRelative = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", path);
            if (File.Exists(wwwrootRelative))
                return wwwrootRelative;

            // Return the original path as-is (caller should check File.Exists)
            return path;
        }
    }
}
