using LIMSApi.Dtos;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace LIMSApi.Reporting
{
    /// <summary>
    /// Chemical-only report: OES/wet chemistry element table.
    /// Shows only chemical test sections with element composition.
    /// </summary>
    public class ChemicalAnalysisDocument : BaseLabReportDocument
    {
        public ChemicalAnalysisDocument(ReportDataDto data, string? watermark = null)
            : base(data, watermark) { }

        public override void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.MarginVertical(15);
                page.MarginHorizontal(20);

                page.Header().Element(ComposeStandardHeader);

                page.Content().PaddingVertical(4).Column(col =>
                {
                    // Title
                    col.Item().AlignCenter().Text("CHEMICAL ANALYSIS REPORT")
                        .FontSize(11).Bold().FontColor(PrimaryColor);
                    col.Item().PaddingVertical(4);

                    // Customer & Sample Info
                    col.Item().Element(ComposeCustomerInfo);
                    col.Item().PaddingVertical(4);

                    // Chemical test sections only
                    var chemSections = Data.TestSections.Where(s => s.TestType == "Chemical").ToList();
                    foreach (var section in chemSections)
                    {
                        col.Item().Element(c => ComposeChemicalTable(c, section));
                        col.Item().PaddingVertical(4);
                    }

                    // NABL Note
                    col.Item().Element(ComposeNablScopeNote);
                    col.Item().PaddingVertical(4);

                    // Signatures
                    col.Item().Element(ComposeSignatures);
                });

                page.Footer().Element(ComposeStandardFooter);
            });
        }

        private void ComposeCustomerInfo(IContainer container)
        {
            container.Table(table =>
            {
                table.ColumnsDefinition(cd =>
                {
                    cd.RelativeColumn(1);
                    cd.RelativeColumn(2);
                    cd.RelativeColumn(1);
                    cd.RelativeColumn(2);
                });

                void Row(TableDescriptor t, string l1, string v1, string l2, string v2)
                {
                    t.Cell().PaddingVertical(1).Text(l1).Style(LabelStyle);
                    t.Cell().PaddingVertical(1).Text(v1).Style(ValueStyle);
                    t.Cell().PaddingVertical(1).Text(l2).Style(LabelStyle);
                    t.Cell().PaddingVertical(1).Text(v2).Style(ValueStyle);
                }

                Row(table, "Customer:", Data.CustomerName, "Case No:", Data.CaseNo);
                Row(table, "Sample No:", Data.SampleNo, "Material:", Data.MaterialSpec);
                Row(table, "Grade:", Data.Grade, "Date Tested:", Data.DateTested);
            });
        }

        private void ComposeChemicalTable(IContainer container, ReportDataTestSection section)
        {
            container.Column(col =>
            {
                col.Item().Text(section.TestName).Style(SectionHeadingStyle);
                col.Item().PaddingTop(3).Table(table =>
                {
                    table.ColumnsDefinition(cd =>
                    {
                        cd.RelativeColumn(2); // Element
                        cd.RelativeColumn(1); // Unit
                        cd.RelativeColumn(1); // Spec Min
                        cd.RelativeColumn(1); // Spec Max
                        cd.RelativeColumn(1); // Result
                        cd.RelativeColumn(1); // Status
                    });

                    // Header
                    table.Header(header =>
                    {
                        void H(IContainer c, string text) =>
                            c.Background(PrimaryColor).Padding(3).Text(text).Style(TableHeaderStyle);

                        header.Cell().Element(c => H(c, "Element"));
                        header.Cell().Element(c => H(c, "Unit"));
                        header.Cell().Element(c => H(c, "Spec Min"));
                        header.Cell().Element(c => H(c, "Spec Max"));
                        header.Cell().Element(c => H(c, "Result"));
                        header.Cell().Element(c => H(c, "Status"));
                    });

                    // Rows
                    foreach (var p in section.Parameters)
                    {
                        var bgColor = p.Status == "Fail" ? "#FFF3F3" : "#FFFFFF";
                        void Cell(IContainer c, string text) =>
                            c.Background(bgColor).BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2)
                             .Padding(2).Text(text).Style(TableCellStyle);

                        table.Cell().Element(c => Cell(c, p.Name));
                        table.Cell().Element(c => Cell(c, p.Unit));
                        table.Cell().Element(c => Cell(c, p.SpecMin ?? "-"));
                        table.Cell().Element(c => Cell(c, p.SpecMax ?? "-"));
                        table.Cell().Element(c => Cell(c, p.Result ?? "-"));
                        table.Cell().Element(c =>
                            c.Background(bgColor).BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2)
                             .Padding(2).Text(p.Status)
                             .FontSize(7).Bold()
                             .FontColor(p.Status == "Pass" ? Colors.Green.Darken2 :
                                        p.Status == "Fail" ? Colors.Red.Darken2 : Colors.Grey.Darken1));
                    }
                });
            });
        }
    }
}
