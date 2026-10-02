using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    [Table("NablDocumentReviews")]
    public class NablDocumentReview : NablFormBase
    {
        [MaxLength(200)]
        public string? DocumentRef { get; set; }

        [MaxLength(500)]
        public string? DocumentTitle { get; set; }

        [MaxLength(200)]
        public string? DocumentType { get; set; }

        [MaxLength(50)]
        public string? CurrentRevision { get; set; }

        public DateTime? ReviewDate { get; set; }

        [MaxLength(200)]
        public string? ReviewedBy { get; set; }

        public string? ReviewFindings { get; set; }

        public bool? ChangeRequired { get; set; }

        public string? ChangeDescription { get; set; }

        public DateTime? NextReviewDate { get; set; }

        [MaxLength(50)]
        public string? ReviewConclusion { get; set; }
        public long? GeneratedDcrId { get; set; }

        [StringLength(100)]
        public string? GeneratedDcrNo { get; set; }

        [StringLength(100)]
        public string? GeneratedDcrChangeType { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public string? DocumentOwner { get; set; }
        public string? ReviewType { get; set; }
        public string? DocumentName { get; set; }
        public string? DepartmentName { get; set; }
        public string? DepartmentDoc { get; set; }
        public string? CurrentIssue { get; set; }
        public string? ReasonForChange { get; set; }
        public string? ImpactOfChange { get; set; }
        public string? NoChangeConclusion { get; set; }
        public string? AdditionalRemarks { get; set; }
        public int? DocumentId { get; set; }
        public long? DepartmentId { get; set; }
        public string? ReviewNo { get; set; }
        [NotMapped]
        public bool CanEditReview { get; set; } = true;
    }
}
