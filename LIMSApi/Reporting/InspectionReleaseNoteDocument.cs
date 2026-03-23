using LIMSApi.Dtos;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace LIMSApi.Reporting
{
    public class InspectionReleaseNoteDocument : IDocument
    {
        private readonly TpiInspectionDto _inspection;
        private readonly string _companyName;
        private readonly string _assetsPath;

        private static readonly string PrimaryColor = "#B71C1C";
        private static readonly string HeaderBg = "#F5F5F5";

        private static TextStyle TitleStyle =>
            TextStyle.Default.FontSize(14).Bold().FontColor(PrimaryColor);

        private static TextStyle LabelStyle =>
            TextStyle.Default.FontSize(9).Bold().FontColor(Colors.Grey.Darken2);

        private static TextStyle ValueStyle =>
            TextStyle.Default.FontSize(9);

        private static TextStyle SectionHeadingStyle =>
            TextStyle.Default.FontSize(10).Bold().FontColor(PrimaryColor);

        public InspectionReleaseNoteDocument(TpiInspectionDto inspection, string companyName = "Divine Metallurgical Services Pvt. Ltd.")
        {
            _inspection = inspection;
            _companyName = companyName;
            _assetsPath = Path.Combine(Directory.GetCurrentDirectory(), "Assets");
        }

        public DocumentMetadata GetMetadata() => new()
        {
            Title = $"Inspection Release Note - {_inspection.Id}",
            Author = _companyName,
            Creator = "LIMS Report Engine"
        };

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.MarginHorizontal(40);
                page.MarginVertical(30);

                page.Header().Element(ComposeHeader);
                page.Content().Element(ComposeContent);
                page.Footer().Element(ComposeFooter);
            });
        }

        private void ComposeHeader(IContainer container)
        {
            container.Column(col =>
            {
                col.Item().Row(row =>
                {
                    row.RelativeItem().Column(c =>
                    {
                        var logoPath = Path.Combine(_assetsPath, "logo.png");
                        if (File.Exists(logoPath))
                        {
                            c.Item().Height(50).Image(logoPath, ImageScaling.FitHeight);
                        }
                    });

                    row.RelativeItem(2).AlignCenter().Column(c =>
                    {
                        c.Item().Text(_companyName).Style(TitleStyle);
                        c.Item().Text("INSPECTION RELEASE NOTE")
                            .FontSize(12).Bold().FontColor(Colors.Black);
                    });

                    row.RelativeItem().AlignRight().Column(c =>
                    {
                        c.Item().Text($"IRN No: TPI-{_inspection.Id:D6}").Style(ValueStyle);
                        c.Item().Text($"Date: {DateTime.Now:dd MMM yyyy}").Style(ValueStyle);
                    });
                });

                col.Item().PaddingVertical(5).LineHorizontal(1).LineColor(PrimaryColor);
            });
        }

        private void ComposeContent(IContainer container)
        {
            container.PaddingTop(10).Column(col =>
            {
                // Inspection Details Section
                col.Item().Text("Inspection Details").Style(SectionHeadingStyle);
                col.Item().PaddingVertical(5).Table(table =>
                {
                    table.ColumnsDefinition(cd =>
                    {
                        cd.RelativeColumn(1);
                        cd.RelativeColumn(2);
                        cd.RelativeColumn(1);
                        cd.RelativeColumn(2);
                    });

                    AddInfoRow(table, "Case No:", _inspection.SampleInwardNumber ?? "N/A", "Sample No:", _inspection.SampleNo ?? "All Samples");
                    AddInfoRow(table, "TPI Agency:", _inspection.TpiName ?? "N/A", "Stage:", FormatStage(_inspection.Stage));
                    AddInfoRow(table, "Status:", _inspection.Status ?? "N/A", "Scheduled Date:", _inspection.ScheduledDate?.ToString("dd MMM yyyy") ?? "N/A");
                    AddInfoRow(table, "Inspection Date:", _inspection.InspectionDate?.ToString("dd MMM yyyy") ?? "N/A", "Created On:", _inspection.CreatedOn.ToString("dd MMM yyyy"));
                });

                col.Item().PaddingVertical(10);

                // Remarks Section
                if (!string.IsNullOrWhiteSpace(_inspection.Remarks))
                {
                    col.Item().Text("Remarks").Style(SectionHeadingStyle);
                    col.Item().PaddingVertical(5).Background(HeaderBg).Padding(10)
                        .Text(_inspection.Remarks).Style(ValueStyle);
                    col.Item().PaddingVertical(10);
                }

                // Inspector Comments Section
                if (!string.IsNullOrWhiteSpace(_inspection.InspectorComments))
                {
                    col.Item().Text("Inspector Comments").Style(SectionHeadingStyle);
                    col.Item().PaddingVertical(5).Background(HeaderBg).Padding(10)
                        .Text(_inspection.InspectorComments).Style(ValueStyle);
                    col.Item().PaddingVertical(10);
                }

                // Result Section
                col.Item().Text("Inspection Result").Style(SectionHeadingStyle);
                col.Item().PaddingVertical(5).Border(1).Padding(15).Column(resultCol =>
                {
                    var statusColor = _inspection.Status switch
                    {
                        "Approved" => Colors.Green.Darken2,
                        "Rejected" => Colors.Red.Darken2,
                        "Waived" => Colors.Grey.Darken2,
                        _ => Colors.Orange.Darken2
                    };

                    resultCol.Item().AlignCenter().Text(_inspection.Status?.ToUpper() ?? "PENDING")
                        .FontSize(16).Bold().FontColor(statusColor);

                    if (_inspection.Status == "Approved")
                    {
                        resultCol.Item().PaddingTop(5).AlignCenter()
                            .Text("This sample/batch is cleared for further processing.")
                            .Style(ValueStyle);
                    }
                    else if (_inspection.Status == "Rejected")
                    {
                        resultCol.Item().PaddingTop(5).AlignCenter()
                            .Text("This sample/batch has been rejected by TPI inspection.")
                            .Style(ValueStyle).FontColor(Colors.Red.Darken1);
                    }
                });

                col.Item().PaddingVertical(30);

                // Signature Block
                col.Item().Row(row =>
                {
                    row.RelativeItem().Column(c =>
                    {
                        c.Item().LineHorizontal(0.5f);
                        c.Item().PaddingTop(3).Text("Lab Representative").Style(LabelStyle);
                    });

                    row.ConstantItem(50);

                    row.RelativeItem().Column(c =>
                    {
                        c.Item().LineHorizontal(0.5f);
                        c.Item().PaddingTop(3).Text("TPI Inspector").Style(LabelStyle);
                    });

                    row.ConstantItem(50);

                    row.RelativeItem().Column(c =>
                    {
                        c.Item().LineHorizontal(0.5f);
                        c.Item().PaddingTop(3).Text("Authorized Signatory").Style(LabelStyle);
                    });
                });
            });
        }

        private void ComposeFooter(IContainer container)
        {
            container.Column(col =>
            {
                col.Item().LineHorizontal(0.5f).LineColor(Colors.Grey.Lighten1);
                col.Item().PaddingTop(3).Row(row =>
                {
                    row.RelativeItem().Text($"Generated on {DateTime.Now:dd MMM yyyy HH:mm}")
                        .FontSize(6).FontColor(Colors.Grey.Darken1);
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

        private static void AddInfoRow(TableDescriptor table, string label1, string value1, string label2, string value2)
        {
            table.Cell().PaddingVertical(2).Text(label1).Style(LabelStyle);
            table.Cell().PaddingVertical(2).Text(value1).Style(ValueStyle);
            table.Cell().PaddingVertical(2).Text(label2).Style(LabelStyle);
            table.Cell().PaddingVertical(2).Text(value2).Style(ValueStyle);
        }

        private static string FormatStage(string? stage) => stage switch
        {
            "BeforePreparation" => "Before Preparation",
            "BeforeTesting" => "Before Testing",
            _ => stage ?? "N/A"
        };
    }
}
