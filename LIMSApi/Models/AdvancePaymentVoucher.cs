using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    /// <summary>
    /// AdvancePaymentVoucher: GST Act Section 31 requires a receipt voucher for advance payments.
    /// Created when advance payment is received before invoice generation.
    /// </summary>
    public class AdvancePaymentVoucher : AuditProperty
    {
        [Key]
        public long ID { get; set; }

        [Required, MaxLength(20)]
        public string VoucherNo { get; set; } = string.Empty;

        public DateTime VoucherDate { get; set; } = DateTime.UtcNow;

        public long CustomerID { get; set; }

        public long InwardID { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [MaxLength(50)]
        public string PaymentMode { get; set; } = string.Empty; // Cash, Cheque, NEFT, UPI

        [MaxLength(100)]
        public string? TransactionReference { get; set; }

        [MaxLength(50)]
        public string Status { get; set; } = "Active"; // Active, AdjustedToInvoice, Refunded

        public long? AdjustedToInvoiceID { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal AdjustedAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal BalanceAmount { get; set; }

        [ForeignKey(nameof(CustomerID))]
        public virtual Customer? Customer { get; set; }

        [ForeignKey(nameof(InwardID))]
        public virtual SampleInward? Inward { get; set; }

        [ForeignKey(nameof(AdjustedToInvoiceID))]
        public virtual TaxInvoice? AdjustedToInvoice { get; set; }
    }
}
