using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LIMSApi.Helpers.Enums;

namespace LIMSApi.Models
{
    /// <summary>
    /// ChargeEvent: Tracks individual charges (tests, preparation, etc.) with status lifecycle
    /// Status: DRAFT → SNAPSHOT → INVOICED
    /// </summary>
    public class ChargeEvent : AuditProperty
    {
        [Key]
        public long ID { get; set; }

        // Business linkage
        public long InwardID { get; set; }
        public long? SampleID { get; set; } // Optional: sample-level charges
        public long? ReportID { get; set; } // Optional: amendment charges

        // Charge details
        [Required, StringLength(100)]
        public string ChargeType { get; set; } = string.Empty; // "Test", "Preparation", "Cutting", "Amendment", etc.

        [Required, StringLength(500)]
        public string Description { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Quantity { get; set; } = 1;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Rate { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        // Status tracking
        [Required, StringLength(50)]
        public string Status { get; set; } = ChargeEventStatus.DRAFT.ToString(); // DRAFT, SNAPSHOT, INVOICED

        // Invoice linkage (when moved to INVOICED)
        public long? TaxInvoiceID { get; set; }
        public long? ProformaInvoiceID { get; set; }

        // Additional metadata
        [StringLength(100)]
        public string? SelectionType { get; set; } // For test pricing (e.g., "Hours", "Load", "Element")

        [Column(TypeName = "decimal(18,2)")]
        public decimal? UsedValue { get; set; } // Value used for pricing calculation

        public long? InvoiceCaseConfigID { get; set; } // Reference to pricing configuration

        // Timestamps
        public DateTime? SnapshotDate { get; set; } // When moved to SNAPSHOT
        public DateTime? InvoicedDate { get; set; } // When moved to INVOICED

        // Navigation properties
        [ForeignKey(nameof(InwardID))]
        public virtual SampleInward? Inward { get; set; }

        [ForeignKey(nameof(SampleID))]
        public virtual SampleDetail? Sample { get; set; }

        [ForeignKey(nameof(TaxInvoiceID))]
        public virtual TaxInvoice? TaxInvoice { get; set; }

        [ForeignKey(nameof(ProformaInvoiceID))]
        public virtual ProformaInvoiceHeader? ProformaInvoice { get; set; }
    }
}

