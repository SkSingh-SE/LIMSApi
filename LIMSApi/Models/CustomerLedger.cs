using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    public class CustomerLedger : AuditProperty
    {
        [Key]
        public long Id { get; set; }

        public long CustomerId { get; set; }

        public DateTime Date { get; set; }

        [Required, MaxLength(50)]
        public string TransactionType { get; set; } = null!;
        // "Invoice", "Payment", "CreditNote", "DebitNote", "Adjustment"

        [MaxLength(100)]
        public string? ReferenceNo { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal DebitAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal CreditAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Balance { get; set; }

        [MaxLength(50)]
        public string? PaymentMode { get; set; }
        // "Razorpay", "Cash", "Cheque", "NEFT", "UPI"

        [MaxLength(100)]
        public string? ChequeNo { get; set; }

        [MaxLength(200)]
        public string? BankName { get; set; }

        [MaxLength(200)]
        public string? TransactionRef { get; set; }

        public long? InwardId { get; set; }

        public long? InvoiceId { get; set; }

        [MaxLength(500)]
        public string? Remarks { get; set; }

        // Navigation properties
        [ForeignKey(nameof(CustomerId))]
        public virtual Customer? Customer { get; set; }

        [ForeignKey(nameof(InwardId))]
        public virtual SampleInward? Inward { get; set; }

        [ForeignKey(nameof(InvoiceId))]
        public virtual TaxInvoice? Invoice { get; set; }
    }
}
