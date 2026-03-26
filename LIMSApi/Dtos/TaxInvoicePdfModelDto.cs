namespace LIMSApi.Dtos
{
    public class TaxInvoicePdfModelDto
    {
        public string InvoiceNo { get; set; } = null!;
        public DateTime InvoiceDate { get; set; }

        public string CustomerName { get; set; } = null!;
        public string CustomerAddress { get; set; } = null!;
        public string CustomerGst { get; set; } = null!;
        public string State { get; set; } = null!;
        public string StateCode { get; set; } = null!;

        public DateTime ReceivedDate { get; set; }
        public string RefNo { get; set; } = null!;

        public List<TaxInvoiceRow> Rows { get; set; } = new();

        public decimal SubTotal { get; set; }

        // Customer discount
        public decimal? DiscountPercentage { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal DiscountedSubTotal { get; set; }

        public decimal CGST { get; set; }
        public decimal SGST { get; set; }
        public decimal IGST { get; set; }
        public decimal GrandTotal { get; set; }
        
        // Advance payment adjustment
        public decimal AdvancePayment { get; set; }
        public decimal BalancePayable { get; set; }

        public string AmountInWords { get; set; } = null!;
    }

    public class TaxInvoiceRow
    {
        public string Sample { get; set; } = "";
        public string Description { get; set; } = "";
        public string QtyDisplay { get; set; } = "1";
        public decimal Rate { get; set; }
        public decimal Amount { get; set; }
    }
}
