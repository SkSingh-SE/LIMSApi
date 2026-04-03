using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    /// <summary>
    /// Quotation: ISO 17025 Clause 8.2.1 — Formal quotation with validity period.
    /// Issued before Proforma Invoice. Contains estimated test charges and terms.
    /// </summary>
    public class Quotation : AuditProperty
    {
        [Key]
        public long ID { get; set; }

        [Required, MaxLength(20)]
        public string QuotationNo { get; set; } = string.Empty;

        public DateTime QuotationDate { get; set; } = DateTime.UtcNow;

        public DateTime? ValidUntil { get; set; }

        public long CustomerID { get; set; }

        [MaxLength(500)]
        public string? Subject { get; set; }

        [MaxLength(2000)]
        public string? TermsAndConditions { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal SubTotal { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal DiscountAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TaxAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal GrandTotal { get; set; }

        [MaxLength(50)]
        public string Status { get; set; } = "Draft"; // Draft, Sent, Accepted, Rejected, Expired

        public long? AcceptedBy { get; set; }
        public DateTime? AcceptedOn { get; set; }

        public long? ConvertedToInwardID { get; set; }

        [ForeignKey(nameof(CustomerID))]
        public virtual Customer? Customer { get; set; }

        [ForeignKey(nameof(ConvertedToInwardID))]
        public virtual SampleInward? ConvertedToInward { get; set; }

        public virtual ICollection<QuotationItem> Items { get; set; } = new List<QuotationItem>();
    }

    /// <summary>
    /// QuotationItem: Line items within a quotation (test charges, preparation charges, etc.)
    /// </summary>
    public class QuotationItem : AuditProperty
    {
        [Key]
        public long ID { get; set; }

        public long QuotationID { get; set; }

        [Required, MaxLength(500)]
        public string Description { get; set; } = string.Empty;

        public long? LaboratoryTestID { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Quantity { get; set; } = 1;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Rate { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [ForeignKey(nameof(QuotationID))]
        public virtual Quotation? Quotation { get; set; }
    }
}
