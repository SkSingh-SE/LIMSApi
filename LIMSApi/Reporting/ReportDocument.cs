using LIMSApi.Dtos;
using LIMSApi.Models;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Org.BouncyCastle.Utilities.Collections;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Diagnostics.Metrics;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace LIMSApi.Reporting
{
    public class ReportDocument : IDocument
    {
        private readonly Report _report;

        public string LogoPath;
        public string SignPath;
        public string QrCodePath;
        public string BasePath;

        public ReportDocument(Report report)
        {
            _report = report;

            LogoPath = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "logo.png");
            SignPath = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "signature.png");
            QrCodePath = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "qr_code.png");
            BasePath = Directory.GetCurrentDirectory();
        }

        public DocumentMetadata GetMetadata() => new()
        {
            Title = _report.ReportNo,
            Author = "Divine Metallurgical Services Pvt. Ltd."
        };

        // ====================== GLOBAL STYLES ======================

        private static TextStyle LabelStyle =>
            TextStyle.Default.FontSize(7).FontColor(Colors.Grey.Darken2);

        private static TextStyle ValueStyle =>
            TextStyle.Default.FontSize(8);

        private static TextStyle SectionTitleStyle =>
            TextStyle.Default.FontSize(8).Bold().Underline();

        // ====================== COMPOSE ======================

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(20);

                page.Background().Border(1);

                page.Header().Element(RenderGlobalHeader);

                page.Content().PaddingVertical(6).Column(col =>
                {
                    bool first = true;

                    foreach (var block in _report.Blocks.OrderBy(b => b.SortOrder))
                    {
                        if (block.BlockType == "Header" && !first)
                            col.Item().PageBreak();

                        first = false;

                        col.Item().Element(e => RenderBlock(e, block));
                    }
                });

                page.Footer().Element(RenderGlobalFooter);
            });
        }

        // ====================== BLOCK ROUTER ======================

        private void RenderBlock(IContainer c, ReportBlock block)
        {
            switch (block.BlockType)
            {
                case "Header": RenderHeader(c, block); break;
                case "KeyValueTable": RenderKeyValueTable(c, block); break;
                case "Observation": RenderObservation(c, block); break;
                case "ResultTable": RenderTable(c, block); break;
                case "Statement": RenderStatement(c, block); break;
                case "LongTermMatrix": RenderLongTermMatrix(c, block); break;
                case "Image": RenderImage(c, block); break;
                case "ImageGallery": RenderGallery(c, block); break;
                case "SectionTitle": RenderSectionTitle(c, block); break;
                case "EndFooter":
                    RenderEndOfReportFooter(c,block);
                    break;

                default: c.Text($"Unknown block: {block.BlockType}"); break;
            }
        }

        // ====================== GLOBAL HEADER ======================

        private void RenderGlobalHeader(IContainer c)
        {
            c.Padding(4).Row(row =>
            {
                row.ConstantItem(40).Image(LogoPath).FitArea();

                row.RelativeItem(2).PaddingLeft(6).Column(col =>
                {
                    col.Item().Text("DIVINE METALLURGICAL SERVICES PVT. LTD.")
                        .Bold().FontSize(9);

                    col.Item().Text("CIN: U74900GJ2012PTC069436")
                        .FontSize(6);
                });

                row.RelativeItem(3).PaddingLeft(6).Column(col =>
                {
                    col.Item().Text("14, Gopal Industrial Estate, Vallabhnagar\nBRTS, Odhav, Ahmedabad – 382415")
                        .FontSize(6);

                    col.Item().Text("Ph: +91 79 2289 2804 / 1013 | +91 92272 20993")
                        .FontSize(6);

                    col.Item().Text("Email: divinelab_nhp@rediffmail.com\naccounts@divinelaboratory.com")
                        .FontSize(6);
                });

                //row.ConstantItem(80).AlignMiddle().Row(r =>
                //{
                //    r.ConstantItem(38).Image(LogoPath).FitArea();
                //    r.ConstantItem(4);
                //    r.ConstantItem(38).Image(LogoPath).FitArea();
                //});
            });
        }

        // ====================== HEADER BLOCK ======================

        private void RenderHeader(IContainer container, ReportBlock block)
        {
            var d = JsonSerializer.Deserialize<HeaderPayload>(block.PayloadJson);

            container
                .Border(1)
                .Padding(4)
                .Column(col =>
                {
                    // 🔹 TITLE
                    col.Item()
                       .AlignCenter()
                       .Text("TEST CERTIFICATE")
                       .Style(SectionTitleStyle);

                    // 🔹 TWO COLUMN KEY–VALUE LAYOUT
                    col.Item().Table(t =>
                    {
                        t.ColumnsDefinition(c =>
                        {
                            c.ConstantColumn(120); // Label
                            c.RelativeColumn();    // Value
                            c.ConstantColumn(120); // Label
                            c.RelativeColumn();    // Value
                        });

                        // Row 1
                        t.Cell().Text("Test Certificate No.").Style(LabelStyle);
                        t.Cell().Text(d.CertificateNo).Style(ValueStyle);
                        t.Cell().Text("Date of Issue").Style(LabelStyle);
                        t.Cell().Text(d.DateOfIssue).Style(ValueStyle);

                        // Row 2
                        t.Cell().Text("Report No.").Style(LabelStyle);
                        t.Cell().Text(d.ReportNo).Style(ValueStyle);
                        t.Cell().Text("Test Performed At").Style(LabelStyle);
                        t.Cell().Text(d.TestPerformedAt).Style(ValueStyle);

                        //AddKV(t, "Sample No.", d.SampleNo);


                        //AddKV(t, "Test Name", d.TestName);

                        //AddKV(t, "Customer Name", d.CustomerName);
                        //AddKV(t, "Sample Received On", d.SampleReceivedOn);

                        //AddKV(t, "Customer Address", d.CustomerAddress);
                        //AddKV(t, "Test Performed At", d.TestPerformedAt);
                    });
                });
        }


        // ====================== KEY VALUE ======================

        private void RenderKeyValueTable(IContainer c, ReportBlock block)
        {
            var p = JsonSerializer.Deserialize<KeyValueTablePayload>(block.PayloadJson);

            c.Border(1).Padding(4).Column(col =>
            {
                col.Item().Text(p.Title).Style(SectionTitleStyle);

                col.Item().Table(t =>
                {
                    t.ColumnsDefinition(c =>
                    {
                        c.RelativeColumn(1);
                        c.RelativeColumn(3);
                    });

                    foreach (var r in p.Rows)
                        AddKV(t, r.Label, r.Value);
                });
            });
        }

        private void RenderObservation(IContainer c, ReportBlock block)
        {
            var p = JsonSerializer.Deserialize<ObservationPayload>(block.PayloadJson);

            c.Border(1).Padding(4).Column(col =>
            {
                col.Item().Text(p.Title).Style(SectionTitleStyle);

                foreach (var f in p.Fields)
                {
                    col.Item().Row(r =>
                    {
                        r.RelativeItem(1).Text(f.Label).Style(LabelStyle);
                        r.RelativeItem(3).Text(f.Value).Style(ValueStyle);
                    });
                }
            });
        }

        // ====================== TABLE ======================

        private void RenderTable(IContainer c, ReportBlock block)
        {
            var p = JsonSerializer.Deserialize<TablePayload>(block.PayloadJson);

            c.Border(1).Padding(2).Column(col =>
            {
                col.Item().Text(p.Title).Style(SectionTitleStyle);

                col.Item().Table(t =>
                {
                    t.ColumnsDefinition(c =>
                    {
                        foreach (var _ in p.Columns)
                            c.RelativeColumn();
                    });

                    t.Header(h =>
                    {
                        foreach (var col in p.Columns)
                            h.Cell().BorderBottom(0.5f).PaddingVertical(2)
                                .Text(col).FontSize(7).Bold();
                    });

                    foreach (var r in p.Rows)
                    {
                        t.Cell().PaddingVertical(2).Text(r.Parameter).FontSize(7);
                        t.Cell().PaddingVertical(2).Text(r.Result).FontSize(7);
                        t.Cell().PaddingVertical(2).Text(r.Unit).FontSize(7);
                        t.Cell().PaddingVertical(2).Text(r.Min).FontSize(7);
                        t.Cell().PaddingVertical(2).Text(r.Max).FontSize(7);
                    }
                });
            });
        }

        // ====================== STATEMENT ======================

        private void RenderStatement(IContainer c, ReportBlock block)
        {
            var p = JsonSerializer.Deserialize<StatementPayload>(block.PayloadJson);

            c.Border(1).Padding(4).Column(col =>
            {
                col.Item().Text("Statement of Conformity").Style(SectionTitleStyle);
                col.Item().Text(p.Text).FontSize(7);
            });
        }

        // ====================== LONG TERM MATRIX ======================


        private void RenderLongTermMatrix(IContainer c, ReportBlock block)
        {
            var p = JsonSerializer.Deserialize<LongTermMatrixPayload>(block.PayloadJson);
            if (p == null || !p.Tests.Any())
                return;

            c.Border(1).Padding(4).Column(col =>
            {
                foreach (var test in p.Tests)
                {
                    col.Item().Text(test.TestName).Style(SectionTitleStyle);

                    int maxReadings = test.Rows.Any()
                        ? test.Rows.Max(r => r.Values?.Count ?? 0)
                        : 0;

                    col.Item().Table(t =>
                    {
                        t.ColumnsDefinition(c =>
                        {
                            c.RelativeColumn(); // Parameter
                            for (int i = 0; i < maxReadings; i++)
                                c.RelativeColumn();
                            if (p.ShowAverage)
                                c.RelativeColumn();
                        });

                        t.Header(h =>
                        {
                            h.Cell().Text("Parameter").FontSize(7).Bold();
                            for (int i = 0; i < maxReadings; i++)
                                h.Cell().Text($"T{i + 1}").FontSize(7).Bold();
                            if (p.ShowAverage)
                                h.Cell().Text("Avg").FontSize(7).Bold();
                        });

                        foreach (var r in test.Rows)
                        {
                            t.Cell().Text(r.Parameter).FontSize(7);

                            for (int i = 0; i < maxReadings; i++)
                            {
                                var value = i < r.Values.Count ? r.Values[i] : null;
                                t.Cell().Text(value?.ToString() ?? "-").FontSize(7);
                            }

                            if (p.ShowAverage)
                                t.Cell().Text(r.Average?.ToString("0.###") ?? "-").FontSize(7);
                        }
                    });
                }
            });
        }


        // ====================== IMAGES ======================

        private void RenderImage(IContainer c, ReportBlock block)
        {
            var p = JsonSerializer.Deserialize<ImagePayload>(block.PayloadJson);

            c.Border(1).Padding(4).Column(col =>
            {
                col.Item().Text(p.Caption).FontSize(7).Italic();
                col.Item().Image(p.Url).FitArea();
            });
        }

        //private void RenderGallery(IContainer c, ReportBlock block)
        //{
        //    var imageGallary = JsonSerializer.Deserialize<ImageGalleryPayload>(block.PayloadJson);

        //    c.Border(1).Padding(4).Column(col =>
        //    {
        //        foreach (var img in imageGallary.Images)
        //        {
        //            col.Item().Image(Path.Combine(BasePath, "wwwroot", img.Url)).FitArea();
        //            if (!string.IsNullOrEmpty(img.Caption))
        //                col.Item().Text(img.Caption).FontSize(7).Italic();
        //        }
        //    });
        //}
        private void RenderGallery(IContainer c, ReportBlock block)
        {
            var gallery = JsonSerializer.Deserialize<ImageGalleryPayload>(block.PayloadJson);
            if (gallery == null || gallery.Images == null || !gallery.Images.Any())
                return;

            const float ImageSize = 120;
            const int ImagesPerRow = 2;

            c.Border(1).Padding(6).Column(col =>
            {
                for (int i = 0; i < gallery.Images.Count; i += ImagesPerRow)
                {
                    var rowImages = gallery.Images
                        .Skip(i)
                        .Take(ImagesPerRow)
                        .ToList();

                    col.Item().Row(row =>
                    {
                        foreach (var img in rowImages)
                        {
                            row.RelativeItem().Column(imgCol =>
                            {
                                imgCol.Item()
                                    .Height(ImageSize)
                                    .Width(ImageSize)
                                    .AlignCenter()
                                    .Image(Path.Combine(BasePath, "wwwroot", img.Url))
                                    .FitArea();

                                if (!string.IsNullOrWhiteSpace(img.Caption))
                                {
                                    imgCol.Item()
                                        .Text(img.Caption)
                                        .FontSize(7)
                                        .Italic()
                                        .AlignCenter();
                                }
                            });
                        }

                        // fill empty columns if last row has fewer images
                        for (int j = rowImages.Count; j < ImagesPerRow; j++)
                        {
                            row.RelativeItem();
                        }
                    });
                }
            });
        }


        //== End of report blocks ==//
        private void RenderEndOfReportFooter(IContainer container, ReportBlock block)
        {
            EndFooterPayload p = string.IsNullOrWhiteSpace(block.PayloadJson)? new EndFooterPayload(): JsonSerializer.Deserialize<EndFooterPayload>(block.PayloadJson)!;
            container.PaddingTop(10).Column(col =>
            {
                col.Item().LineHorizontal(1);

                col.Item().Padding(6).Row(row =>
                {
                    RenderSignatureBox(row.RelativeItem(), "Tested By",
                        p.TestedBy.Name, p.TestedBy.Designation, p.TestedBy.SignaturePath);

                    row.ConstantItem(8);

                    RenderSignatureBox(row.RelativeItem(), "Reviewed & Authorized By",
                        p.ReviewedBy.Name, p.ReviewedBy.Designation, p.ReviewedBy.SignaturePath);

                    row.ConstantItem(8);

                    RenderSignatureBox(row.RelativeItem(), "Witnessed By",
                        p.WitnessedBy.Name, p.WitnessedBy.Organization, p.WitnessedBy.SignaturePath);
                });

                col.Item()
                   .PaddingTop(8)
                   .AlignCenter()
                   .Text("*** END OF REPORT ***")
                   .FontSize(8)
                   .Bold();
            });
        }

        private void RenderSignatureBox(
            IContainer container,
            string title,
            string name,
            string designation,
            string signaturePath)
        {
            container.Border(1).Padding(6).Column(c =>
            {
                c.Item().Height(35).Image(signaturePath).FitArea();

                c.Item().AlignCenter().Text(title).FontSize(7).Bold();
                c.Item().AlignCenter().Text(name).FontSize(7);
                c.Item().AlignCenter().Text(designation).FontSize(7);
            });
        }



        // ====================== FOOTER ======================

        private void RenderGlobalFooter(IContainer container)
        {
            container.Column(col =>
            {
                // 🔹 Top separator line (PDF style)
                col.Item()
                   .LineHorizontal(0.8f);

                // 🔹 Main footer content
                col.Item()
                   .Padding(4)
                   .Row(row =>
                   {
                       // 1️⃣ LEFT : QR CODE
                       row.ConstantItem(45)
                          .AlignTop()
                          .Image(QrCodePath)
                          .FitArea();

                       // 2️⃣ CENTER : CONDITIONS TEXT
                       row.RelativeItem()
                          .PaddingLeft(6)
                          .Column(c =>
                          {
                              c.Item()
                               .Text("Condition of Reporting:")
                               .FontSize(6)
                               .Bold();

                              c.Item()
                               .Text(
                                   $"1) DMSPL clarifies that sampling is not conducted by us. The provided report pertains solely to the sample submitted by the customer.2) Reproduction of the report, either in whole or in part, requires explicit written consent from DMSPL.3) DMSPL assumes no responsibility for the authenticity of reproduced reports including Xerox, email, scanned or WhatsApp copies.4) Customer - provided samples are stored for 15 days from the reporting date unless otherwise specified.5) All testing procedures are conducted under ambient environmental conditions unless explicitly specified otherwise.6) DMSPL is not liable for any financial loss resulting from the use of this report.7) Measurement uncertainty is not included unless explicitly specified by the customer.8) The laboratory does not assume responsibility for commercial statements of conformity.9) Signature in the statement of conformity for ferrous alloys is given without considering associated measurement uncertainty."
                               )
                               .FontSize(5)
                               .LineHeight(1.1f);
                          });
                   });

                // 🔹 PAGE NUMBER (BOTTOM RIGHT)
                col.Item()
                   .AlignRight()
                   .PaddingRight(6)
                   .Text(t =>
                   {
                       t.Span("Page ");
                       t.CurrentPageNumber();
                       t.Span(" of ");
                       t.TotalPages();
                   });
            });
        }


        // ====================== HELPERS ======================

        private static void AddKV(TableDescriptor t, string label, string value)
        {
            t.Cell().PaddingVertical(2).Text(label).Style(LabelStyle);
            t.Cell().PaddingVertical(2).Text(value).Style(ValueStyle);
        }
        private void RenderSectionTitle(IContainer container, ReportBlock block)
        {
            var payload = JsonSerializer.Deserialize<SectionTitlePayload>(block.PayloadJson);

            container
                .Border(1)
                .Padding(4)
                .Text(payload.Title)
                .Style(SectionTitleStyle);
        }


        // ====================== PAYLOAD MODELS ======================
        //public class EndFooterPayload
        //{
        //    public SignatoryPayload TestedBy { get; set; } = new();
        //    public SignatoryPayload ReviewedBy { get; set; } = new();
        //    public SignatoryPayload WitnessedBy { get; set; } = new();
        //}
        //public class SignatoryPayload
        //{
        //    public string Name { get; set; } = string.Empty;
        //    public string Designation { get; set; } = string.Empty;

        //    // Optional: client / TUV / third-party
        //    public string? Organization { get; set; }

        //    // Absolute or relative path
        //    public string SignaturePath { get; set; } = string.Empty;
        //}

        //public class SectionTitlePayload
        //{
        //    public string Title { get; set; }
        //}

        //public class HeaderPayload
        //{
        //    public string CertificateNo { get; set; }
        //    public string ReportNo { get; set; }
        //    public string DateOfIssue { get; set; }
        //    public long SampleId { get; set; }
        //    public string SampleNo { get; set; } = string.Empty;

        //    public string TestName { get; set; }
        //    public string TestMethod { get; set; }

        //    public string CustomerName { get; set; }
        //    public string CustomerAddress { get; set; }

        //    public string SampleReceivedOn { get; set; }
        //    public string TestPerformedAt { get; set; }
        //}


        //public class KeyValueTablePayload
        //{
        //    public string Title { get; set; }
        //    public List<KeyValueRow> Rows { get; set; }
        //}

        //public class KeyValueRow
        //{
        //    public string Label { get; set; }
        //    public string Value { get; set; }
        //}

        //public class ObservationPayload
        //{
        //    public string Title { get; set; }
        //    public List<ObservationField> Fields { get; set; }
        //}

        //public class ObservationField
        //{
        //    public string Label { get; set; }
        //    public string Value { get; set; }
        //}

        //public class TablePayload
        //{
        //    public string Title { get; set; }
        //    public string[] Columns { get; set; }
        //    public List<TableRowPayload> Rows { get; set; }
        //}

        //public class TableRowPayload
        //{
        //    public string Parameter { get; set; }
        //    public string Result { get; set; }
        //    public string Unit { get; set; }
        //    public string Min { get; set; }
        //    public string Max { get; set; }
        //}

        //public class StatementPayload
        //{
        //    public string Text { get; set; }
        //}

        //public class LongTermMatrixPayload
        //{
        //    public bool ShowAverage { get; set; }
        //    public List<LongTermTestPayload> Tests { get; set; }
        //}

        //public class LongTermTestPayload
        //{
        //    public string TestName { get; set; }
        //    public List<LongTermRowPayload> Rows { get; set; }
        //}

        //public class LongTermRowPayload
        //{
        //    public string Parameter { get; set; }
        //    public List<object> Values { get; set; }
        //    public decimal? Average { get; set; }
        //}

        //public class ImagePayload
        //{
        //    public string Url { get; set; }
        //    public string Caption { get; set; }
        //}

        //public class GalleryImage
        //{
        //    public string Url { get; set; }
        //    public string Caption { get; set; }
        //}

    }
}
