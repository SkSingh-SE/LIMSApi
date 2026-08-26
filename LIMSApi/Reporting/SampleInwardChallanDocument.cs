using LIMSApi.Dtos;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace LIMSApi.Reporting
{
    /// <summary>
    /// Sample Inward Receipt & Acknowledgement Challan Document.
    /// Printed directly from Inward / Review workspace to provide customers a formal receipt.
    /// </summary>
    public class SampleInwardChallanDocument : IDocument
    {
        private readonly SampleInwardDto _inward;

        private static readonly string PrimaryColorHex = "#da261c"; // Brand Red
        private static readonly TextStyle LabelStyle = TextStyle.Default.FontSize(8).Bold().FontColor(Colors.Grey.Darken3);
        private static readonly TextStyle ValueStyle = TextStyle.Default.FontSize(8).FontColor(Colors.Grey.Darken4);
        private static readonly TextStyle TitleStyle = TextStyle.Default.FontSize(11).Bold().FontColor(PrimaryColorHex);
        private static readonly TextStyle TableHeaderStyle = TextStyle.Default.FontSize(7.5f).Bold().FontColor(Colors.White);

        public SampleInwardChallanDocument(SampleInwardDto inward)
        {
            _inward = inward;
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.MarginVertical(20);
                page.MarginHorizontal(25);

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
                    row.RelativeItem(3).Column(c =>
                    {
                        c.Item().Text("LABORATORY SAMPLE INWARD RECEIPT CHALLAN")
                            .Style(TitleStyle);
                        c.Item().Text("Acknowledgement of Sample Receipt, Test Request & Preparation Plan")
                            .FontSize(7.5f).Italic().FontColor(Colors.Grey.Darken2);
                    });

                    row.RelativeItem(2).AlignRight().Column(c =>
                    {
                        c.Item().Text($"Challan No: {_inward.CaseNo}").FontSize(9).Bold().FontColor(PrimaryColorHex);
                        c.Item().Text($"Date: {_inward.CollectionTime:dd/MM/yyyy HH:mm}").FontSize(8);
                    });
                });

                col.Item().PaddingTop(4).LineHorizontal(1.5f).LineColor(PrimaryColorHex);
            });
        }

        private void ComposeContent(IContainer container)
        {
            container.PaddingVertical(6).Column(col =>
            {
                // Customer & Info Grid
                col.Item().Border(0.5f).BorderColor(Colors.Grey.Lighten1).Padding(6).Row(row =>
                {
                    // Customer Details
                    row.RelativeItem(3).Column(c =>
                    {
                        c.Item().Text("CUSTOMER DETAILS").FontSize(8).Bold().FontColor(PrimaryColorHex);
                        c.Item().Text(t =>
                        {
                            t.Span("Name: ").Style(LabelStyle);
                            t.Span(_inward.CustomerName ?? "-").Style(ValueStyle);
                        });
                        c.Item().Text(t =>
                        {
                            t.Span("Address: ").Style(LabelStyle);
                            t.Span($"{_inward.Address}, {_inward.Area}, {_inward.City}, {_inward.State} - {_inward.PinCode}").Style(ValueStyle);
                        });
                        c.Item().Text(t =>
                        {
                            t.Span("GST No: ").Style(LabelStyle);
                            t.Span(string.IsNullOrEmpty(_inward.GstNo) ? "-" : _inward.GstNo).Style(ValueStyle);
                        });
                    });

                    // Inward Metadata
                    row.RelativeItem(2).Column(c =>
                    {
                        c.Item().Text("INWARD METADATA").FontSize(8).Bold().FontColor(PrimaryColorHex);
                        c.Item().Text(t =>
                        {
                            t.Span("Urgent Processing: ").Style(LabelStyle);
                            t.Span(_inward.Urgent ? "YES" : "NO").Style(ValueStyle);
                        });
                        c.Item().Text(t =>
                        {
                            t.Span("Return Sample: ").Style(LabelStyle);
                            t.Span(_inward.ReturnSample ? "YES" : "NO").Style(ValueStyle);
                        });
                        c.Item().Text(t =>
                        {
                            t.Span("Receipt Note: ").Style(LabelStyle);
                            t.Span(_inward.SampleReceiptNote ?? "-").Style(ValueStyle);
                        });
                        c.Item().Text(t =>
                        {
                            t.Span("Report Stop: ").Style(LabelStyle);
                            t.Span(_inward.IsReportStopped ? "STOPPED" : "ACTIVE").Style(ValueStyle);
                        });
                    });
                });

                col.Item().PaddingVertical(4);

                // Samples & Tests Table
                col.Item().Text("REGISTERED SAMPLES & TEST ALLOCATION").FontSize(8.5f).Bold().FontColor(PrimaryColorHex);
                col.Item().PaddingTop(2).Table(table =>
                {
                    table.ColumnsDefinition(cd =>
                    {
                        cd.ConstantColumn(22);  // #
                        cd.ConstantColumn(80);  // Sample No
                        cd.RelativeColumn(3);   // Description / Heat No
                        cd.RelativeColumn(2);   // Metal Base / Grade
                        cd.ConstantColumn(30);  // Qty
                        cd.RelativeColumn(4);   // Requested Tests
                        cd.ConstantColumn(50);  // Cutting
                    });

                    // Table Header
                    table.Header(header =>
                    {
                        header.Cell().Background(PrimaryColorHex).Padding(3).Text("#").Style(TableHeaderStyle);
                        header.Cell().Background(PrimaryColorHex).Padding(3).Text("Sample No").Style(TableHeaderStyle);
                        header.Cell().Background(PrimaryColorHex).Padding(3).Text("Description / Heat No").Style(TableHeaderStyle);
                        header.Cell().Background(PrimaryColorHex).Padding(3).Text("Metal / Grade").Style(TableHeaderStyle);
                        header.Cell().Background(PrimaryColorHex).Padding(3).Text("Qty").Style(TableHeaderStyle);
                        header.Cell().Background(PrimaryColorHex).Padding(3).Text("Requested Tests").Style(TableHeaderStyle);
                        header.Cell().Background(PrimaryColorHex).Padding(3).Text("Prep Required").Style(TableHeaderStyle);
                    });

                    int index = 1;
                    foreach (var s in _inward.SampleDetails ?? new List<SampleDetailDto>())
                    {
                        var bg = index % 2 == 0 ? Colors.Grey.Lighten4 : Colors.White;
                        table.Cell().Background(bg).Padding(3).Text(index.ToString()).Style(ValueStyle);
                        table.Cell().Background(bg).Padding(3).Text(s.SampleNo ?? "-").Style(ValueStyle).Bold();
                        table.Cell().Background(bg).Padding(3).Text(s.Details ?? s.Remarks ?? "-").Style(ValueStyle);
                        table.Cell().Background(bg).Padding(3).Text(s.MetalClassificationName ?? s.ProductConditionName ?? "-").Style(ValueStyle);
                        table.Cell().Background(bg).Padding(3).Text(s.Quantity.ToString()).Style(ValueStyle);

                        // Flatten test names
                        var testNames = new List<string>();
                        foreach (var plan in s.TestPlans ?? new List<SampleTestPlanDto>())
                        {
                            (plan.GeneralTests ?? new List<GeneralTestDto>()).ForEach(gt =>
                            {
                                (gt.Methods ?? new List<GeneralTestMethodDto>()).ForEach(m =>
                                {
                                    if (!string.IsNullOrEmpty(m.StandardName)) testNames.Add(m.StandardName);
                                });
                            });
                            (plan.ChemicalTests ?? new List<ChemicalTestDto>()).ForEach(ct =>
                            {
                                if (!string.IsNullOrEmpty(ct.MetalClassificationName)) 
                                    testNames.Add($"Chemical ({ct.MetalClassificationName})");
                                else 
                                    testNames.Add("Chemical Analysis");
                            });
                        }

                        table.Cell().Background(bg).Padding(3).Text(testNames.Count > 0 ? string.Join(", ", testNames.Distinct()) : "Pending Plan").Style(ValueStyle);
                        table.Cell().Background(bg).Padding(3).Text(s.PreparationRequired ? "YES" : "NO").Style(ValueStyle);

                        index++;
                    }
                });

                col.Item().PaddingVertical(8);

                // Signatures Section
                col.Item().Row(row =>
                {
                    row.RelativeItem().Border(0.5f).BorderColor(Colors.Grey.Lighten1).Padding(8).Column(c =>
                    {
                        c.Item().Text("SAMPLE RECEIVED BY").FontSize(7.5f).Bold().FontColor(PrimaryColorHex);
                        c.Item().PaddingTop(20).Text("Signature & Date").FontSize(7).Italic();
                    });

                    row.ConstantItem(15);

                    row.RelativeItem().Border(0.5f).BorderColor(Colors.Grey.Lighten1).Padding(8).Column(c =>
                    {
                        c.Item().Text("CUSTOMER REPRESENTATIVE").FontSize(7.5f).Bold().FontColor(PrimaryColorHex);
                        c.Item().PaddingTop(20).Text("Signature & Date").FontSize(7).Italic();
                    });
                });
            });
        }

        private void ComposeFooter(IContainer container)
        {
            container.Column(col =>
            {
                col.Item().LineHorizontal(0.5f).LineColor(Colors.Grey.Lighten1);
                col.Item().PaddingTop(2).Row(row =>
                {
                    row.RelativeItem().Text("Terms: Samples stored for 30 days post testing unless return requested.").FontSize(6.5f).Italic();
                    row.RelativeItem().AlignRight().Text(text =>
                    {
                        text.Span("Page ").FontSize(6.5f);
                        text.CurrentPageNumber().FontSize(6.5f);
                        text.Span(" of ").FontSize(6.5f);
                        text.TotalPages().FontSize(6.5f);
                    });
                });
            });
        }
    }
}
