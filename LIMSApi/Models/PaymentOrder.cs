using LIMSApi.Helpers.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    public class PaymentOrder
    {
        [Key]
        public long ID { get; set; }

        public string OrderNo { get; set; } = null!; // LIMS internal order no
        public PaymentType PaymentType { get; set; }

        // -----------------------------
        // BUSINESS LINKAGE (CONTROLLED)
        // -----------------------------

        // PI / Final Invoice (Case level)
        public long? InwardID { get; set; }
        public string? CaseNo { get; set; }

        // Final Invoice (Single Sample scenario)
        public long? SampleID { get; set; }

        // Amendment (ONLY Report)
        public long? ReportID { get; set; }
        public long CustomerId { get; set; }

        // Amount
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "INR";

        // Razorpay
        public string RazorpayOrderId { get; set; } = null!;
        public string? RazorpayPaymentId { get; set; }
        public string? RazorpaySignature { get; set; }

        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

        // Security
        public string PaymentToken { get; set; } = null!;
        public DateTime TokenExpiry { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public DateTime? PaidOn { get; set; }

        public bool EmailSent { get; set; } = false;
        public bool WhatsAppSent { get; set; } = false;
        public bool SMSSent { get; set; } = false;
        public long? TaxInvoiceID { get; set; }
        public DateTime? LinkSentOn { get; set; }
        [ForeignKey(nameof(CustomerId))]
        public virtual Customer? Customer { get; set; }

        [ForeignKey(nameof(TaxInvoiceID))]
        public TaxInvoice? TaxInvoice { get; set; }
    }

}
