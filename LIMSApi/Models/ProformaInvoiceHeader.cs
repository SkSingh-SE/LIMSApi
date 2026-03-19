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

        public decimal CGST { get; set; }
        public decimal SGST { get; set; }
        public decimal IGST { get; set; }

        public decimal TaxAmount { get; set; }
        public decimal GrandTotal { get; set; }

        public bool IsGenerated { get; set; } = true;

        [ForeignKey("InwardID")]
        public virtual SampleInward? SampleInward { get; set; }
        public ICollection<ProformaInvoiceDetail> Details { get; set; } = new List<ProformaInvoiceDetail>();
        public virtual ICollection<InvoiceLineItem> LineItems { get; set; } = new List<InvoiceLineItem>();
    }

}
