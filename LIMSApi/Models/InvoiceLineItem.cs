using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    public class InvoiceLineItem : AuditProperty
    {
        [Key]
        public long ID { get; set; }

        public long? ProformaInvoiceHeaderID { get; set; }  // FK to ProformaInvoiceHeader
        public long? TaxInvoiceID { get; set; }  // FK to TaxInvoice (for final invoices)
        public long? SampleInwardID { get; set; }  // Optional: link to specific inward

        [Required, MaxLength(300)]
        public string Description { get; set; } = string.Empty;  // "Courier Charges", "Urgency Surcharge", etc.

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal TaxPercent { get; set; }  // e.g., 18.00 for GST

        [Column(TypeName = "decimal(18,2)")]
        public decimal TaxAmount { get; set; }  // Calculated: Amount * TaxPercent / 100

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }  // Amount + TaxAmount

        [MaxLength(500)]
        public string? Remark { get; set; }

        [ForeignKey("ProformaInvoiceHeaderID")]
        public virtual ProformaInvoiceHeader? ProformaInvoiceHeader { get; set; }

        [ForeignKey("TaxInvoiceID")]
        public virtual TaxInvoice? TaxInvoice { get; set; }

        [ForeignKey("SampleInwardID")]
        public virtual SampleInward? SampleInward { get; set; }
    }
}
