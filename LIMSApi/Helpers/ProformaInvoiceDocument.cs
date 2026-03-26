
using LIMSApi.Dtos;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace LIMSApi.Helpers
{

    public class ProformaInvoiceDocument : IDocument
    {
        private readonly TaxInvoicePdfModelDto _model;
        private readonly byte[]? _logoBytes;
        private readonly byte[]? _signatureBytes;

        public ProformaInvoiceDocument(TaxInvoicePdfModelDto model, string? logoPath, string? signaturePath)
        {
            _model = model;

            if (!string.IsNullOrWhiteSpace(logoPath) && File.Exists(logoPath))
                _logoBytes = File.ReadAllBytes(logoPath);

            if (!string.IsNullOrWhiteSpace(signaturePath) && File.Exists(signaturePath))
                _signatureBytes = File.ReadAllBytes(signaturePath);
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(20);
                page.DefaultTextStyle(x => x.FontSize(10));
                page.PageColor(Colors.White);

                // Watermark
                page.Background().AlignCenter().AlignMiddle().Text("PROFORMA")
                    .FontSize(80).Bold().FontColor(Colors.Grey.Lighten4);

                page.Content().Column(col =>
                {
                    ComposeHeader(col);
                    col.Spacing(5);
                    ComposeCustomerBlock(col);
                    col.Spacing(5);
                    ComposeDetailTable(col);
                    col.Spacing(5);
                    ComposeTotalsBlock(col);
                    col.Spacing(5);
                    ComposeFooter(col);
                });
            });
        }

        private void ComposeHeader(ColumnDescriptor col)
        {
            col.Item().Row(row =>
            {
                row.ConstantColumn(90).AlignLeft().AlignMiddle().Element(e =>
                {
                    if (_logoBytes != null)
                        e.Image(_logoBytes);
                    else
                        e.Border(1).Padding(5).AlignCenter().AlignMiddle().Text("LOGO");
                });

                row.RelativeColumn().Column(c =>
                {
                    c.Item().AlignCenter().Text("DIVINE METALLURGICAL SERVICES PVT. LTD.")
                        .Bold().FontSize(14);

                    c.Item().AlignCenter().Text("CIN: U74900GJ2012PTC069436");

                    c.Item().AlignCenter()
                        .Text("14, Gopal Industrial Estate, Opp. Vallabhnagar, Odhav, Ahmedabad-382415");

                    c.Item().AlignCenter()
                        .Text("Ph: 079-2289 1013 | Email: accounts@divinelaboratory.com");
                });
            });

            col.Item().Row(r =>
            {
                r.RelativeColumn().Text("PROFORMA INVOICE").Bold().FontSize(12);
                r.RelativeColumn().AlignRight()
                    .Text(text =>
                    {
                        text.Span("Invoice No: ").Bold();
                        text.Span(_model.InvoiceNo);
                        text.Line("");
                        text.Span("Date: ").Bold();
                        text.Span(_model.InvoiceDate.ToString("dd-MM-yyyy"));
                    });
            });
        }

        private void ComposeCustomerBlock(ColumnDescriptor col)
        {
            col.Item().Row(row =>
            {
                row.RelativeColumn().Border(1).Padding(5).Column(c =>
                {
                    c.Item().Text("Customer Details:").Bold();
                    c.Item().Text(_model.CustomerName);
                    c.Item().Text(_model.CustomerAddress);
                    c.Item().Text(text =>
                    {
                        text.Span("GSTIN: ").Bold();
                        text.Span(_model.CustomerGst);
                    });
                    c.Item().Text(text =>
                    {
                        text.Span("State: ").Bold();
                        text.Span($"{_model.State} ({_model.StateCode})");
                    });
                });

                row.RelativeColumn().Border(1).Padding(5).Column(c =>
                {
                    c.Item().Text(text =>
                    {
                        text.Span("Place of Supply: ").Bold();
                        text.Span(_model.State);
                    });
                    c.Item().Text(text =>
                    {
                        text.Span("Date of Received: ").Bold();
                        text.Span(_model.ReceivedDate.ToString("dd-MM-yyyy"));
                    });
                    c.Item().Text(text =>
                    {
                        text.Span("Ref No: ").Bold();
                        text.Span(_model.RefNo);
                    });
                });
            });
        }

        private void ComposeDetailTable(ColumnDescriptor col)
        {
            col.Item().Table(table =>
            {
                table.ColumnsDefinition(c =>
                {
                    c.ConstantColumn(80);   // Sample
                    c.RelativeColumn();     // Description
                    c.ConstantColumn(70);   // Qty
                    c.ConstantColumn(70);   // Rate
                    c.ConstantColumn(80);   // Amount
                });

                // Header
                table.Header(h =>
                {
                    h.Cell().Border(1).Padding(3).Text("Sample").Bold();
                    h.Cell().Border(1).Padding(3).Text("Description").Bold();
                    h.Cell().Border(1).Padding(3).AlignCenter().Text("Qty / Element").Bold();
                    h.Cell().Border(1).Padding(3).AlignRight().Text("Rate").Bold();
                    h.Cell().Border(1).Padding(3).AlignRight().Text("Total Amount (Rs.)").Bold();
                });

                foreach (var row in _model.Rows)
                {
                    table.Cell().Border(1).Padding(3).Text(row.Sample);
                    table.Cell().Border(1).Padding(3).Text(row.Description);
                    table.Cell().Border(1).Padding(3).AlignCenter().Text(row.QtyDisplay);
                    table.Cell().Border(1).Padding(3).AlignRight().Text(row.Rate.ToString("0.00"));
                    table.Cell().Border(1).Padding(3).AlignRight().Text(row.Amount.ToString("0.00"));
                }
            });
        }

        private void ComposeTotalsBlock(ColumnDescriptor col)
        {
            col.Item().Row(row =>
            {
                row.RelativeColumn().Column(c =>
                {
                    c.Item().Text(text =>
                    {
                        text.Span("GST Category: ").Bold();
                        text.Span("Technical Testing & Analysis Service");
                    });
                    c.Item().Text(text =>
                    {
                        text.Span("HSN / SAC Code: ").Bold();
                        text.Span("998346");
                    });
                });

                row.ConstantColumn(160).Table(t =>
                {
                    t.ColumnsDefinition(c =>
                    {
                        c.RelativeColumn();
                        c.ConstantColumn(70);
                    });

                    void Line(string label, decimal amount, bool bold = false)
                    {
                        t.Cell().Border(1).Padding(3)
                            .Text(label).FontSize(10).Bold();
                        t.Cell().Border(1).Padding(3).AlignRight()
                            .Text(amount.ToString("0.00")).FontSize(10).Bold();
                    }

                    Line("Sub Total", _model.SubTotal);

                    // Conditional discount line
                    if (_model.DiscountAmount > 0)
                    {
                        var discLabel = _model.DiscountPercentage.HasValue
                            ? $"Discount @ {_model.DiscountPercentage.Value:0.##}%"
                            : "Discount";
                        Line(discLabel, -_model.DiscountAmount);
                        Line("After Discount", _model.DiscountedSubTotal);
                    }

                    Line("CGST @ 9%", _model.CGST);
                    Line("SGST @ 9%", _model.SGST);
                    Line("IGST @ 18%", _model.IGST);
                    Line("Grand Total", _model.GrandTotal, bold: true);
                });
            });

            col.Item().Text(text =>
            {
                text.Span("Amount in Words: ").Bold();
                text.Span(_model.AmountInWords);
            });
        }

        private void ComposeFooter(ColumnDescriptor col)
        {
            col.Item().Row(row =>
            {
                row.RelativeColumn().Column(c =>
                {
                    c.Item().Text("Current Bank Account Details").Bold();
                    c.Item().Text("Bank : SBI - GIDC Odhav");
                    c.Item().Text("Account No : 32651714994");
                    c.Item().Text("IFSC : SBIN0001387");
                });

                row.RelativeColumn().AlignRight().Column(c =>
                {
                    c.Item().Text("For, DIVINE METALLURGICAL SERVICES PVT. LTD");
                    c.Item().Height(5);

                    c.Item().Element(e =>
                    {
                        if (_signatureBytes != null)
                            e.Image(_signatureBytes).FitHeight().FitWidth();
                        else
                            e.Height(40);
                    });

                    c.Item().Text("Authorised Signatory");
                });
            });
        }
    }

}
