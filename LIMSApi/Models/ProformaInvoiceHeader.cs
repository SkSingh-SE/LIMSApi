using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    public class ProformaInvoiceHeader : AuditProperty
    {
        [Key]
        public long ID { get; set; }
        public long InwardID { get; set; }

        public string PINo { get; set; }
        public DateTime PIDate { get; set; }

        public decimal SubTotal { get; set; }

        // Customer discount (applied before GST)
        [Column(TypeName = "decimal(5,2)")]
        public decimal? DiscountPercentage { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal DiscountAmount { get; set; } = 0;

        public decimal CGST { get; set; }
        public decimal SGST { get; set; }
        public decimal IGST { get; set; }

        public decimal TaxAmount { get; set; }
        public decimal GrandTotal { get; set; }

        public bool IsGenerated { get; set; } = true;

        // Financial Year linkage
        public long? FinancialYearId { get; set; }

        [ForeignKey(nameof(FinancialYearId))]
        public virtual FinancialYear? FinancialYearEntity { get; set; }

        [ForeignKey("InwardID")]
        public virtual SampleInward? SampleInward { get; set; }
        public ICollection<ProformaInvoiceDetail> Details { get; set; } = new List<ProformaInvoiceDetail>();
        public virtual ICollection<InvoiceLineItem> LineItems { get; set; } = new List<InvoiceLineItem>();
    }

}
