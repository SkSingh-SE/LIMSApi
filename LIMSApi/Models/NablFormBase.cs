using System.ComponentModel.DataAnnotations;

namespace LIMSApi.Models
{
    public abstract class NablFormBase : AuditProperty
    {
        [Key]
        public long ID { get; set; }

        // Document Identity
        [Required, MaxLength(20)]
        public string FormCode { get; set; } = string.Empty;

        [Required, MaxLength(10)]
        public string IssueNo { get; set; } = "01";

        [Required, MaxLength(10)]
        public string RevNo { get; set; } = "00";

        public DateTime Date { get; set; } = DateTime.UtcNow;

        [MaxLength(50)]
        public string? DocumentNo { get; set; }

        // Workflow Status
        [MaxLength(20)]
        public string? Status { get; set; } = "Draft";

        // Prepared / Reviewed / Approved (names for display)
        [MaxLength(200)]
        public string? PreparedBy { get; set; }

        [MaxLength(200)]
        public string? ReviewedBy { get; set; }

        [MaxLength(200)]
        public string? ApprovedBy { get; set; }

        // Workflow tracking (IDs + timestamps for audit)
        public long? PreparedById { get; set; }
        public DateTime? PreparedDate { get; set; }

        public long? ReviewedById { get; set; }
        public DateTime? ReviewedDate { get; set; }

        public long? ApprovedById { get; set; }
        public DateTime? ApprovedDate { get; set; }

        [MaxLength(500)]
        public string? RejectionRemarks { get; set; }

        // Periodic Review
        public DateTime? NextReviewDate { get; set; }
        public int ReviewFrequencyMonths { get; set; } = 12;

        // Effectivity & Obsolescence
        public DateTime? EffectiveDate { get; set; }
        public bool IsObsolete { get; set; } = false;

        [MaxLength(500)]
        public string? ObsoleteReason { get; set; }
        //public long? ParentDocumentId { get; set; }

        //[MaxLength(500)]
        //public string? RevisionReason { get; set; }

        //[MaxLength(20)]
        //public string? RevisionType { get; set; }
    }
}
