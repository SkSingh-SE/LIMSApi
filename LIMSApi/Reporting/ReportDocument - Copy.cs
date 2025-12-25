//using LIMSApi.Models;
//using QuestPDF.Fluent;
//using QuestPDF.Helpers;
//using QuestPDF.Infrastructure;
//using System.Text.Json;

//namespace LIMSApi.Reporting
//{
//    public class ReportDocument : IDocument
//    {
//        private readonly Report _report;
//        public string logoPath;
//        public string signPath;
//        public string qrCodePath;
//        public ReportDocument(Report report)
//        {
//            _report = report;
//            signPath = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "signature.png");
//            logoPath = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "logo.png");
//            qrCodePath = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "qr_code.png");
//        }

//        public DocumentMetadata GetMetadata() => new DocumentMetadata
//        {
//            Title = _report.ReportNo,
//            Author = "Your Lab"
//        };

        
//        public void Compose(IDocumentContainer container)
//        {
//            container.Page(page =>
//            {
//                page.Size(PageSizes.A4);
//                page.Margin(25);

//                // Page border
//                page.Background()
//                    .Border(1)
//                    .BorderColor(Colors.Black);

//                // HEADER (tight & safe)
//                page.Header()  // 🔒 SAFE LIMIT
//                    .Element(RenderGlobalHeader);

//                // CONTENT
//                page.Content().PaddingTop(8)
//                                .PaddingBottom(8)
//                                .Column(col =>
//                                {
//                                    bool firstTest = true;

//                                    foreach (var block in _report.Blocks.OrderBy(b => b.SortOrder))
//                                    {
//                                        // 🔹 New test → always new page
//                                        if (block.BlockType == "Header")
//                                        {
//                                            if (!firstTest)
//                                                col.Item().PageBreak();

//                                            firstTest = false;
//                                        }

//                                        col.Item()
//                                           .Element(e => RenderBlock(e, block));
//                                    }
//                                });


//                // FOOTER (small)
//                page.Footer()
//                    .Element(RenderGlobalFooter);
//            });
//        }




//        private void RenderBlock(IContainer container, ReportBlock block)
//        {
//            switch (block.BlockType)
//            {
//                case "Header":
//                    RenderHeader(container, block);
//                    break;
//                case "ResultTable":
//                    RenderTable(container, block);
//                    break;
//                case "KeyValueTable":
//                    RenderKeyValueTable(container, block);
//                    break;
//                case "Statement":
//                    RenderStatement(container, block);
//                    break;
//                case "Image":
//                    RenderImage(container, block);
//                    break;
//                case "ImageGallery":
//                    RenderGallery(container, block);
//                    break;
//                case "Observation":
//                    RenderObservation(container, block);
//                    break;
//                case "LongTermMatrix":
//                    RenderLongTermMatrix(container, block);
//                    break;
//                case "Footer":
//                    RenderFooter(container, block);
//                    break;
//                case "SectionTitle":
//                    RenderSectionTitle(container, block);
//                    break;
//                default:
//                    container.Text($"Unknown Block: {block.BlockType}");
//                    break;
//            }
//        }
//        private void RenderGlobalHeader(IContainer container)
//        {
//            container.Padding(6).Column(col =>
//            {
//                col.Item().Row(row =>
//                {
//                    // 1️⃣ LEFT LOGO
//                    row.ConstantItem(45)
//                       .AlignMiddle()
//                       .Image(logoPath)
//                       .FitArea();

//                    // 2️⃣ COMPANY NAME + CIN
//                    row.RelativeItem(2).PaddingLeft(20).Column(c =>
//                    {
//                        c.Item()
//                         .Text("DIVINE METALLURGICAL SERVICES PVT. LTD.")
//                         .Bold()
//                         .FontSize(14);

//                        c.Item().AlignBottom().PaddingTop(10)
//                         .Text("CIN: U74900GJ2012PTC069436")
//                         .FontSize(6);
//                    });

//                    // 3️⃣ ADDRESS + CONTACT
//                    row.RelativeItem(3).PaddingLeft(10).Column(c =>
//                    {
//                        c.Item()
//                         .Text("14, Gopal Industrial Estate, Vallabhnagar,\nBRTS, Odhav, Ahmedabad – 382415")
//                         .FontSize(6);

//                        c.Item()
//                         .Text("Ph: +91 79 2289 2804 / 1013 | +91 92272 20993")
//                         .FontSize(6);

//                        c.Item()
//                         .Text("Email: divinelab_nhp@rediffmail.com\naccounts@divinelaboratory.com")
//                         .FontSize(6);
//                    });

//                    // 4️⃣ RIGHT LOGOS
//                    row.ConstantItem(90).AlignMiddle().Row(r =>
//                    {
//                        r.ConstantItem(40)
//                         .Image(logoPath)
//                         .FitArea();

//                        r.ConstantItem(10)
//                        .Padding(5);

//                        r.ConstantItem(40)
//                         .Image(logoPath)
//                         .FitArea();
//                    });
//                });

//            });
//        }



//        private void RenderHeader(IContainer container, ReportBlock block)
//        {
//            var d = JsonSerializer.Deserialize<HeaderPayload>(block.PayloadJson);

//            container.Border(1).PaddingBottom(12).Column(col =>
//            {
//                col.Item().AlignCenter().AlignTop()
//                    .Text("Test Certificate")
//                    .Bold()
//                    .FontSize(8).Underline();

//                col.Item().Table(t =>
//                {
//                    t.ColumnsDefinition(c =>
//                    {
//                        c.RelativeColumn();
//                        c.RelativeColumn();
//                    });

//                    t.Cell().Text($"Report No: {d.ReportNo}");
//                    t.Cell().Text($"Certificate No: {d.CertificateNo}");

//                    t.Cell().Text($"Date of Issue: {d.DateOfIssue}");
//                    t.Cell().Text($"Sample ID: {d.SampleId}");
//                });
//            });

//        }

//        private void RenderKeyValueTable(IContainer container, ReportBlock block)
//        {
//            var payload = JsonSerializer.Deserialize<KeyValueTablePayload>(block.PayloadJson);

//            container.Border(1).PaddingBottom(10).Padding(8).Column(col =>
//            {
//                col.Item().Text(payload.Title)
//                    .Bold().FontSize(12);

//                col.Item().Table(table =>
//                {
//                    table.ColumnsDefinition(c =>
//                    {
//                        c.RelativeColumn(1);
//                        c.RelativeColumn(3);
//                    });

//                    foreach (var row in payload.Rows)
//                    {
//                        table.Cell().Padding(3).Text(row.Label).Bold();
//                        table.Cell().Padding(3).Text(row.Value);
//                    }
//                });
//            });

//        }

//        private void RenderObservation(IContainer container, ReportBlock block)
//        {
//            var payload = JsonSerializer.Deserialize<ObservationPayload>(block.PayloadJson);

//            container.Border(1).PaddingBottom(10).Padding(8).Column(col =>
//            {
//                col.Item().Text(payload.Title)
//                    .Bold().FontSize(12);

//                foreach (var f in payload.Fields)
//                {
//                    col.Item().Row(r =>
//                    {
//                        r.RelativeItem(1).Text(f.Label + ":").Bold();
//                        r.RelativeItem(3).Text(f.Value);
//                    });
//                }
//            });

//        }


//        private void RenderTable(IContainer container, ReportBlock block)
//        {
//            var payload = JsonSerializer.Deserialize<TablePayload>(block.PayloadJson);

//            container.Border(1).Padding(5).Column(col =>
//            {
//                col.Item().PaddingBottom(6)
//                    .Text(payload.Title)
//                    .Bold()
//                    .FontSize(12);

//                col.Item().Table(table =>
//                {
//                    table.ColumnsDefinition(cols =>
//                    {
//                        foreach (var _ in payload.Columns)
//                            cols.RelativeColumn();
//                    });

//                    // 🔹 HEADER (REPEATS AUTOMATICALLY ON PAGE BREAK)
//                    table.Header(header =>
//                    {
//                        foreach (var colName in payload.Columns)
//                        {
//                            header.Cell()
//                                .Background(Colors.Grey.Lighten3)
//                                .Padding(4)
//                                .Text(colName)
//                                .Bold();
//                        }
//                    });

//                    // 🔹 BODY (AUTO PAGINATED)
//                    foreach (var row in payload.Rows)
//                    {
//                        table.Cell().Padding(4).Text(row.Parameter);
//                        table.Cell().Padding(4).Text(row.Result);
//                        table.Cell().Padding(4).Text(row.Unit);
//                        table.Cell().Padding(4).Text(row.Min);
//                        table.Cell().Padding(4).Text(row.Max);
//                    }
//                });
//            });
//        }





//        private void RenderStatement(IContainer container, ReportBlock block)
//        {
//            var payload = JsonSerializer.Deserialize<StatementPayload>(block.PayloadJson);

//            container.Border(1).Padding(8)
//                .Text(payload.Text)
//                .FontSize(10);
//        }

//        private void RenderGlobalFooter(IContainer container)
//        {
//            container.Column(col =>
//            {
//                col.Item().LineHorizontal(1);

//                col.Item().Padding(6).Row(row =>
//                {
//                    // 🔹 LEFT LOGO
//                    row.ConstantItem(60)
//                        .AlignTop()
//                        .PaddingRight(15)   // 👉 small gap after logo
//                        .Image(qrCodePath);

//                    // 🔹 CONDITIONS OF REPORTING TEXT (FULL WIDTH)
//                    row.RelativeColumn().Column(c =>
//                    {
//                        c.Item()
//                            .Text("Condition of Reporting:")
//                            .Bold()
//                            .FontSize(7);

//                        c.Item()
//                            .Text(@"1) DMSPL clarifies that sampling is not conducted by us. The provided report pertains solely to the sample submitted by the customer.2) Reproduction of the report, either in whole or in part, requires explicit written consent from DMSPL.3) DMSPL assumes no responsibility for the authenticity of reproduced reports including Xerox, email, scanned or WhatsApp copies.4) Customer-provided samples are stored for 15 days from the reporting date unless otherwise specified.5) All testing procedures are conducted under ambient environmental conditions unless explicitly specified otherwise.6) DMSPL is not liable for any financial loss resulting from the use of this report.7) Measurement uncertainty is not included unless explicitly specified by the customer.8) The laboratory does not assume responsibility for commercial statements of conformity.9) Signature in the statement of conformity for ferrous alloys is given without considering associated measurement uncertainty."
//                            )
//                            .FontSize(5)        // 👉 smaller text
//                            ; // 👉 tight but readable
//                    });
//                });

//                // 🔹 PAGE NUMBER (BOTTOM RIGHT)
//                col.Item()
//                    .AlignRight()
//                    .PaddingRight(8)
//                    .Text(t =>
//                    {
//                        t.Span("Page ");
//                        t.CurrentPageNumber();
//                        t.Span(" of ");
//                        t.TotalPages();
//                    });
//            });
//        }




//        private void RenderImage(IContainer container, ReportBlock block)
//        {
//            var payload = JsonSerializer.Deserialize<ImagePayload>(block.PayloadJson);

//            container.Column(col =>
//            {
//                col.Item().Text(payload.Caption).Bold();
//                col.Item().Image(payload.Url);
//            });
//        }

//        private void RenderGallery(IContainer container, ReportBlock block)
//        {
//            var images = JsonSerializer.Deserialize<List<GalleryImage>>(block.PayloadJson);

//            container.Column(col =>
//            {
//                col.Item().Text("Images").Bold().FontSize(14);

//                foreach (var img in images)
//                {
//                    col.Item().Image(img.Url);
//                    if (!string.IsNullOrEmpty(img.Caption))
//                        col.Item().Text(img.Caption).Italic().FontSize(10);
//                }
//            });
//        }

//        public class GalleryImage
//        {
//            public string Url { get; set; }
//            public string? Caption { get; set; }
//        }


//        public class ImagePayload
//        {
//            public string Url { get; set; }
//            public string Caption { get; set; }
//        }

//        private void RenderFooter(IContainer container, ReportBlock block)
//        {
//            var payload = JsonSerializer.Deserialize<FooterPayload>(block.PayloadJson);

//            container.Column(col =>
//            {
//                col.Item().LineHorizontal(1);
//                col.Item().Text(payload.Disclaimer).FontSize(9);
//            });
//        }


//        private void RenderLongTermMatrix(IContainer container, ReportBlock block)
//        {
//            var payload = JsonSerializer.Deserialize<LongTermMatrixPayload>(block.PayloadJson);

//            foreach (var test in payload.Tests)
//            {
//                container.Column(col =>
//                {
//                    col.Item().Text(test.TestName).Bold().FontSize(14);
//                    col.Item().Text($"Duration: {test.DurationHours} hrs | Status: {test.Status}")
//                        .FontSize(10)
//                        .Italic();

//                    col.Item().Table(table =>
//                    {
//                        table.ColumnsDefinition(cols =>
//                        {
//                            cols.RelativeColumn(); // Parameter
//                            foreach (var _ in test.Rows.First().Values)
//                                cols.RelativeColumn();
//                            if (payload.ShowAverage)
//                                cols.RelativeColumn();
//                        });

//                        table.Header(h =>
//                        {
//                            h.Cell().Text("Parameter").Bold();
//                            for (int i = 0; i < test.Rows.First().Values.Count; i++)
//                                h.Cell().Text($"T{i + 1}").Bold();

//                            if (payload.ShowAverage)
//                                h.Cell().Text("Avg").Bold();
//                        });

//                        foreach (var row in test.Rows)
//                        {
//                            table.Cell().Text(row.Parameter);

//                            foreach (var v in row.Values)
//                                table.Cell().Text(v?.ToString() ?? "-");

//                            if (payload.ShowAverage)
//                                table.Cell().Text(row.Average?.ToString("0.###"));
//                        }
//                    });

//                    container.PaddingBottom(15);
//                });
//            }
//        }
//        private void RenderSectionTitle(IContainer container, ReportBlock block)
//        {
//            var payload = JsonSerializer.Deserialize<SectionTitlePayload>(block.PayloadJson);
//            container.PaddingTop(15).Text(payload.Title)
//                .FontSize(16)
//                .Bold()
//                .Underline();
//        }



//        public class KeyValueTablePayload
//        {
//            public string Title { get; set; }
//            public List<KeyValueRow> Rows { get; set; }
//        }

//        public class KeyValueRow
//        {
//            public string Label { get; set; }
//            public string Value { get; set; }
//        }

//        public class SectionTitlePayload
//        {
//            public string Title { get; set; }
//        }

//        public class LongTermMatrixPayload
//        {
//            public bool ShowLimits { get; set; }
//            public bool ShowAverage { get; set; }
//            public List<LongTermTestPayload> Tests { get; set; }
//        }

//        public class LongTermTestPayload
//        {
//            public string TestName { get; set; }
//            public int DurationHours { get; set; }
//            public string Status { get; set; }
//            public List<LongTermRowPayload> Rows { get; set; }
//        }

//        public class LongTermRowPayload
//        {
//            public string Parameter { get; set; }
//            public List<object> Values { get; set; }
//            public decimal? Average { get; set; }
//        }

//        public class ObservationPayload
//        {
//            public string Title { get; set; }
//            public List<ObservationField> Fields { get; set; }
//        }

//        public class ObservationField
//        {
//            public string Label { get; set; }
//            public string Value { get; set; }
//        }

//        public class FooterPayload
//        {
//            public string Disclaimer { get; set; }
//        }

//        public class StatementPayload
//        {
//            public string Text { get; set; }
//        }

//        public class TablePayload
//        {
//            public string Title { get; set; }
//            public string[] Columns { get; set; }
//            public List<TableRowPayload> Rows { get; set; }
//        }

//        public class TableRowPayload
//        {
//            public string Parameter { get; set; }
//            public string Result { get; set; }
//            public string Unit { get; set; }
//            public string Min { get; set; }
//            public string Max { get; set; }
//        }

//        public class HeaderPayload
//        {
//            public string CertificateNo { get; set; }
//            public string ReportNo { get; set; }
//            public string DateOfIssue { get; set; }
//            public long SampleId { get; set; }
//            public string TestName { get; set; }
//        }


//    }
//}
