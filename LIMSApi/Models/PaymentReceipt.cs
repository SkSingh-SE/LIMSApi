using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    public class PaymentReceipt : AuditProperty
    {
        [Key]
        public long Id { get; set; }

        [Required, MaxLength(50)]
        public string ReceiptNo { get; set; } = null!;

        public long CustomerId { get; set; }

        public DateTime Date { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required, MaxLength(50)]
        public string PaymentMode { get; set; } = null!;

        [MaxLength(100)]
        public string? ChequeNo { get; set; }

        [MaxLength(200)]
        public string? BankName { get; set; }

        [MaxLength(200)]
        public string? TransactionRef { get; set; }

        [MaxLength(2000)]
        public string? InvoiceIds { get; set; }
        // JSON array of invoice IDs adjusted, e.g. "[1,2,3]"

        [MaxLength(500)]
        public string? Remarks { get; set; }

        [Required, MaxLength(20)]
        public string Status { get; set; } = "Generated";
        // "Generated", "Sent", "Cancelled"

        // Navigation properties
        [ForeignKey(nameof(CustomerId))]
        public virtual Customer? Customer { get; set; }
    }
}
