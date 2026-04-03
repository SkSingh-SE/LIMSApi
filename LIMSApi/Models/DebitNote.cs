using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    /// <summary>
    /// DebitNote: Issued when invoice amount needs to be increased (GST law requirement).
    /// Used for additional charges, price corrections, or supplementary billing.
    /// </summary>
    public class DebitNote : AuditProperty
    {
        [Key]
        public long ID { get; set; }

        [Required, MaxLength(20)]
        public string DebitNoteNo { get; set; } = string.Empty;

        public DateTime DebitNoteDate { get; set; } = DateTime.UtcNow;

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
