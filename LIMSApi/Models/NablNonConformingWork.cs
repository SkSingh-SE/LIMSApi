using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    [Table("NablNonConformingWorks")]
    public class NablNonConformingWork : NablFormBase
    {
        public DateTime? NCDate { get; set; }

        [MaxLength(200)]
        public string? SampleCode { get; set; }

        [MaxLength(500)]
        public string? TestParameter { get; set; }

        public string? NCDescription { get; set; }

        [MaxLength(50)]
        public string? NCSource { get; set; } // InternalAudit/CustomerComplaint/EquipmentFailure/Personnel/Method/Other

        [MaxLength(200)]
        public string? DetectedBy { get; set; }

        [MaxLength(200)]
        public string? IdentifiedBy { get; set; }

        public bool? SuspendedWork { get; set; }

        public string? AffectedResults { get; set; }
        [MaxLength(50)]
        public string? NCCategory { get; set; } // Minor/Major

        public string? RootCauseAnalysis { get; set; }
        public string? SignatureTDQM { get; set; }
        public DateTime? CloserDate { get; set; }
        public long? DepartmentId { get; set; }

        [MaxLength(200)]
        public string? DepartmentName { get; set; }
        public long? ReportedByEmployeeId { get; set; }

        [MaxLength(200)]
        public string? ReportedByEmployeeName { get; set; }

        // ===========================
        // Classification
        // ===========================

        [MaxLength(100)]
        public string? Source { get; set; }

        [MaxLength(100)]
        public string? Category { get; set; }

        [MaxLength(100)]
        public string? Priority { get; set; }

        [MaxLength(100)]
        public string? ReferenceModule { get; set; }

        public int? ReferenceId { get; set; }

        [MaxLength(100)]
        public string? ReferenceNo { get; set; }

        public bool CustomerAffected { get; set; }

        // ===========================
        // Description
        // ===========================

        public string? Description { get; set; }

        public string? ImmediateAction { get; set; }

        public string? ProblemDescription { get; set; }

        // ===========================
        // Workflow
        // ===========================

        [MaxLength(30)]
        public string? Status { get; set; }

        public int CurrentStep { get; set; }
        public string? NcNo { get; set; }
        public long? ChecklistId { get; set; }

        // Navigation Properties

        public virtual NablNonConformingWorkInvestigation? Investigation { get; set; }

        public virtual NablNonConformingWorkCorrectiveAction? CorrectiveAction { get; set; }

        public virtual NablNonConformingWorkVerification? Verification { get; set; }

        public virtual NablNonConformingWorkClosure? Closure { get; set; }

        [NotMapped]
        public int RequestStep { get; set; }
    }
}
