using LIMSApi.Dtos;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace LIMSApi.Reporting
{
    /// <summary>
    /// Abstract base class for all lab report documents.
    /// Provides shared header, footer, watermark, and style infrastructure.
    /// </summary>
    public abstract class BaseLabReportDocument : IDocument
    {
        protected readonly ReportDataDto Data;
        protected readonly string AssetsPath;
        protected readonly string? WatermarkText;

        // ── Shared Styles ──
        protected static readonly string PrimaryColor = "#B71C1C";
        protected static readonly string HeaderBg = "#F5F5F5";

        protected static TextStyle TitleStyle =>
            TextStyle.Default.FontSize(12).Bold().FontColor(PrimaryColor);
        protected static TextStyle SubTitleStyle =>
            TextStyle.Default.FontSize(9).Bold();
        protected static TextStyle LabelStyle =>
            TextStyle.Default.FontSize(7.5f).FontColor(Colors.Grey.Darken2).Bold();
        protected static TextStyle ValueStyle =>
            TextStyle.Default.FontSize(8);
        protected static TextStyle SmallStyle =>
            TextStyle.Default.FontSize(6).FontColor(Colors.Grey.Darken1);
        protected static TextStyle TableHeaderStyle =>
            TextStyle.Default.FontSize(7).Bold().FontColor(Colors.White);
        protected static TextStyle TableCellStyle =>
            TextStyle.Default.FontSize(7);
        protected static TextStyle SectionHeadingStyle =>
            TextStyle.Default.FontSize(9).Bold().FontColor(PrimaryColor);

        protected BaseLabReportDocument(ReportDataDto data, string? watermarkText = null)
        {
            Data = data;
            AssetsPath = Path.Combine(Directory.GetCurrentDirectory(), "Assets");
            WatermarkText = watermarkText;
        }

        public DocumentMetadata GetMetadata() => new()
        {
            Title = Data.ReportNo,
            Author = "Divine Metallurgical Services Pvt. Ltd.",
            Creator = "LIMS Report Engine"
        };

        public abstract void Compose(IDocumentContainer container);

        /// <summary>
        /// Standard company header with conditional NABL logo.
        /// </summary>
        protected void ComposeStandardHeader(IContainer container)
        {
            container.Column(col =>
            {
                col.Item()
                    .BorderBottom(2).BorderColor(PrimaryColor)
                    .PaddingBottom(4)
                    .Row(row =>
                    {
                        // Logo
                        var logoPath = Path.Combine(AssetsPath, "TPTL-icon.png");
                        if (File.Exists(logoPath))
                        {
                            row.ConstantItem(50).AlignMiddle().Image(logoPath).FitArea();
                        }
                        else
                        {
                            row.ConstantItem(50);
                        }

                        row.ConstantItem(6);

                        // Company info
                        row.RelativeItem().AlignMiddle().Column(center =>
                        {
                            center.Item().Text("DIVINE METALLURGICAL SERVICES PVT. LTD.").Style(TitleStyle);
                            center.Item().Text("14, Gopal Industrial Estate, Vallabhnagar, BRTS, Odhav, Ahmedabad - 382415")
                                .FontSize(6.5f).FontColor(Colors.Grey.Darken2);
                        });

                        // NABL logo (conditional)
                        row.ConstantItem(80).AlignMiddle().Column(right =>
                        {
                            if (Data.IsNabl || Data.NablInfo?.IsPartialScope == true)
                            {
                                var nablPath = Path.Combine(AssetsPath, "nabl_logo.png");
                                if (File.Exists(nablPath))
                                {
                                    right.Item().Height(30).AlignCenter().Image(nablPath).FitArea();
                                }

                                if (!string.IsNullOrWhiteSpace(Data.NablCertNo))
                                {
                                    var certText = Data.NablInfo?.IsPartialScope == true
                                        ? $"{Data.NablCertNo} *" : Data.NablCertNo;
                                    right.Item().AlignCenter().Text(certText).FontSize(5.5f).Bold();
                                }
                            }
                        });
                    });

                // Report No / Date line
                col.Item().PaddingTop(3).Row(row =>
                {
                    row.RelativeItem().Text($"Report No: {Data.ReportNo}").Style(SubTitleStyle);
                    row.RelativeItem().AlignCenter().Text($"Certificate No: {Data.CertificateNo ?? "N/A"}").Style(SubTitleStyle);
                    row.RelativeItem().AlignRight().Text($"Date: {Data.ReportDate:dd-MM-yyyy}").Style(SubTitleStyle);
                });
            });
        }

        /// <summary>
        /// Standard page footer with page numbers.
        /// </summary>
        protected void ComposeStandardFooter(IContainer container)
        {
            container.Column(col =>
            {
                col.Item().LineHorizontal(0.5f).LineColor(Colors.Grey.Lighten1);
                col.Item().PaddingTop(3).Row(row =>
                {
                    row.RelativeItem().Text($"Generated on {DateTime.Now:dd MMM yyyy HH:mm}").Style(SmallStyle);

                    if (!string.IsNullOrWhiteSpace(Data.NablCertNo) && (Data.IsNabl || Data.NablInfo?.IsPartialScope == true))
                    {
                        row.RelativeItem().AlignCenter().Text($"NABL Cert: {Data.NablCertNo}").Style(SmallStyle);
                    }

                    row.RelativeItem().AlignRight().Text(text =>
                    {
                        text.Span("Page ").FontSize(6).FontColor(Colors.Grey.Darken1);
                        text.CurrentPageNumber().FontSize(6);
                        text.Span(" of ").FontSize(6).FontColor(Colors.Grey.Darken1);
                        text.TotalPages().FontSize(6);
                    });
                });
            });
        }

        /// <summary>
        /// Adds a watermark overlay if configured.
        /// </summary>
        protected void ComposeWatermark(IContainer container)
        {
            if (string.IsNullOrWhiteSpace(WatermarkText)) return;

            container.AlignCenter().AlignMiddle()
                .Text(WatermarkText)
                .FontSize(60)
                .Bold()
                .FontColor(Colors.Grey.Lighten3);
        }

        /// <summary>
        /// NABL scope note section for reports with out-of-scope parameters.
        /// </summary>
        protected void ComposeNablScopeNote(IContainer container)
        {
            if (Data.NablInfo?.OutOfScopeParameterNames?.Any() != true) return;

            container.PaddingVertical(3).Border(0.5f).BorderColor(Colors.Orange.Darken2).Padding(6).Column(noteCol =>
            {
                noteCol.Item().Text("NABL Accreditation Note").Style(SectionHeadingStyle);
                noteCol.Item().PaddingTop(2).Text(text =>
                {
                    text.Span("The following tests are NOT covered under NABL Accreditation: ").Style(ValueStyle);
                    text.Span(string.Join(", ", Data.NablInfo.OutOfScopeParameterNames)).Style(ValueStyle).Bold();
                    text.Span(". Results for these tests are reported in our capacity as a non-accredited laboratory.").Style(ValueStyle);
                });
            });
        }

        /// <summary>
        /// Signature block with 3 signatories.
        /// </summary>
        protected void ComposeSignatures(IContainer container)
        {
            container.PaddingTop(20).Row(row =>
            {
                void SignatureBlock(IContainer c, string name, string? designation)
                {
                    c.Column(col =>
                    {
                        col.Item().LineHorizontal(0.5f);
                        col.Item().PaddingTop(3).Text(name).Style(LabelStyle);
                        if (!string.IsNullOrWhiteSpace(designation))
                            col.Item().Text(designation).Style(SmallStyle);
                    });
                }

                row.RelativeItem().Element(c => SignatureBlock(c, Data.TestedByName, Data.TestedByDesignation));
                row.ConstantItem(40);
                row.RelativeItem().Element(c => SignatureBlock(c, Data.ReviewedByName, Data.ReviewedByDesignation));
                row.ConstantItem(40);
                row.RelativeItem().Element(c => SignatureBlock(c, Data.AuthorizedByName, Data.AuthorizedByDesignation));
            });
        }
    }
}
