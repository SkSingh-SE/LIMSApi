using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    public class ProformaInvoiceDetail
    {
        public long Id { get; set; }
        public long ProformaInvoiceHeaderID { get; set; }

        public long SampleID { get; set; }

        public string ChargeType { get; set; } = string.Empty;  // "Machining" | "Test"
        public string Description { get; set; } = string.Empty;

        public int Quantity { get; set; }
        public decimal Rate { get; set; }
        public decimal Amount { get; set; }
        public string? SelectionType { get; set; }
        public decimal? UsedValue { get; set; }
        [ForeignKey("SampleID")]
        public long? InvoiceCaseConfigID { get; set; }


    }

}
