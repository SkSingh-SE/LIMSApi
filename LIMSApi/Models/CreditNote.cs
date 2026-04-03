using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    /// <summary>
    /// CreditNote: Issued when invoice amount needs to be reduced (GST law requirement).
    /// Used for invoice corrections, partial refunds, or billing adjustments.
    /// </summary>
    public class CreditNote : AuditProperty
    {
        [Key]
        public long ID { get; set; }

        [Required, MaxLength(20)]
        public string CreditNoteNo { get; set; } = string.Empty;

        public DateTime CreditNoteDate { get; set; } = DateTime.UtcNow;

        public long TaxInvoiceID { get; set; }

        public long CustomerID { get; set; }

        [Required, MaxLength(500)]
        public string Reason { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TaxAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        [MaxLength(50)]
        public string Status { get; set; } = "Draft"; // Draft, Approved, Cancelled

        public long? ApprovedBy { get; set; }
        public DateTime? ApprovedOn { get; set; }

        [ForeignKey(nameof(TaxInvoiceID))]
        public virtual TaxInvoice? TaxInvoice { get; set; }

        [ForeignKey(nameof(CustomerID))]
        public virtual Customer? Customer { get; set; }
    }
}
