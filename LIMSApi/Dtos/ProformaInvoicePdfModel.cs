namespace LIMSApi.Dtos
{
    public class ProformaInvoicePdfModel
    {
        public string InvoiceNo { get; set; } = "";
        public DateTime InvoiceDate { get; set; }

        public string CustomerName { get; set; } = "";
        public string CustomerAddress { get; set; } = "";
        public string CustomerGst { get; set; } = "";
        public string State { get; set; } = "";
        public string StateCode { get; set; } = "24";
        public string RefNo { get; set; } = "";
        public DateTime ReceivedDate { get; set; }

        public decimal SubTotal { get; set; }
        public decimal CGST { get; set; }
        public decimal SGST { get; set; }
        public decimal IGST { get; set; }
        public decimal GrandTotal { get; set; }
        public string AmountInWords { get; set; } = "";

        public List<ProformaInvoicePdfRow> Rows { get; set; } = new();
    }

    public class ProformaInvoicePdfRow
    {
        public string Sample { get; set; } = "";
        public string Description { get; set; } = "";
        public string QtyDisplay { get; set; } = "1";
        public decimal Rate { get; set; }
        public decimal Amount { get; set; }
    }

}
