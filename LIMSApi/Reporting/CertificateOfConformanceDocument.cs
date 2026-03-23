using LIMSApi.Dtos;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace LIMSApi.Reporting
{
    /// <summary>
    /// Certificate of Conformance — pass/fail summary only, no raw values.
    /// </summary>
    public class CertificateOfConformanceDocument : BaseLabReportDocument
    {
        public CertificateOfConformanceDocument(ReportDataDto data, string? watermark = null)
            : base(data, watermark) { }

        public override void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.MarginVertical(20);
                page.MarginHorizontal(25);

                page.Header().Element(ComposeStandardHeader);

                page.Content().PaddingVertical(8).Column(col =>
                {
                    col.Item().AlignCenter().Text("CERTIFICATE OF CONFORMANCE")
                        .FontSize(13).Bold().FontColor(PrimaryColor);
                    col.Item().PaddingVertical(8);

                    // Customer & Sample Info
                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(cd =>
                        {
                            cd.RelativeColumn(1);
                            cd.RelativeColumn(2);
                        });

                        void Row(TableDescriptor t, string label, string value)
                        {
                            t.Cell().PaddingVertical(2).Text(label).Style(LabelStyle);
                            t.Cell().PaddingVertical(2).Text(value).Style(ValueStyle);
                        }

                        Row(table, "Customer:", Data.CustomerName);
                        Row(table, "Address:", Data.CustomerAddress);
                        Row(table, "Case No:", Data.CaseNo);
                        Row(table, "Sample No:", Data.SampleNo);
                        Row(table, "Description:", Data.SampleDescription);
                        Row(table, "Material/Grade:", $"{Data.MaterialSpec} / {Data.Grade}");
                        Row(table, "Date Received:", Data.DateReceived);
                        Row(table, "Date Tested:", Data.DateTested);
                    });

                    col.Item().PaddingVertical(10);

                    // Conformance Statement
                    var allPass = Data.TestSections
                        .SelectMany(s => s.Parameters)
                        .All(p => p.Status == "Pass" || p.Status == "N/A");

                    col.Item().Border(1).Padding(15).Column(stmtCol =>
                    {
                        stmtCol.Item().AlignCenter().Text("CONFORMANCE STATEMENT")
                            .Style(SectionHeadingStyle);

                        stmtCol.Item().PaddingTop(8).Text(text =>
                        {
                            if (allPass)
                            {
                                text.Span("This is to certify that the above material/sample has been tested and the results ").Style(ValueStyle);
                                text.Span("CONFORM ").Style(ValueStyle).Bold().FontColor(Colors.Green.Darken2);
                                text.Span($"to the requirements of {Data.MaterialSpec} / {Data.Grade}.").Style(ValueStyle);
                            }
                            else
                            {
                                text.Span("This is to certify that the above material/sample has been tested and the results ").Style(ValueStyle);
                                text.Span("DO NOT CONFORM ").Style(ValueStyle).Bold().FontColor(Colors.Red.Darken2);
                                text.Span($"to the requirements of {Data.MaterialSpec} / {Data.Grade}. ").Style(ValueStyle);
                                text.Span("Please refer to the detailed test report for specifics.").Style(ValueStyle);
                            }
                        });
                    });

                    col.Item().PaddingVertical(6);

                    // Tests performed summary
                    col.Item().Text("Tests Performed:").Style(SectionHeadingStyle);
                    foreach (var section in Data.TestSections)
                    {
                        var sectionPass = section.Parameters.All(p => p.Status == "Pass" || p.Status == "N/A");
                        col.Item().PaddingLeft(10).PaddingVertical(1).Text(text =>
                        {
                            text.Span($"• {section.TestName}: ").Style(ValueStyle);
                            text.Span(sectionPass ? "PASS" : "FAIL")
                                .FontSize(8).Bold()
                                .FontColor(sectionPass ? Colors.Green.Darken2 : Colors.Red.Darken2);
                        });
                    }

                    col.Item().PaddingVertical(6);
                    col.Item().Element(ComposeNablScopeNote);
                    col.Item().PaddingVertical(6);
                    col.Item().Element(ComposeSignatures);
                });

                page.Footer().Element(ComposeStandardFooter);
            });
        }
    }
}
