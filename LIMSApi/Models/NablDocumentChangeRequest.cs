using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    [Table("NablDocumentChangeRequests")]
    public class NablDocumentChangeRequest : NablFormBase
    {
        [MaxLength(200)]
        public string? DocumentRef { get; set; }

        [MaxLength(500)]
        public string? DocumentTitle { get; set; }

        [MaxLength(200)]
        public string? DocumentType { get; set; }

        [MaxLength(50)]
        public string? CurrentVersion { get; set; }

        public string? ChangeDescription { get; set; }

        public string? ReasonForChange { get; set; }

        [MaxLength(200)]
        public string? RequestedBy { get; set; }

        public DateTime? RequestDate { get; set; }

        [MaxLength(50)]
        public string? UrgencyLevel { get; set; }  

        public string? AssessedImpact { get; set; }

        [MaxLength(200)]
        public string? AssessmentBy { get; set; }

        public DateTime? AssessmentDate { get; set; }

        [MaxLength(50)]
        public string? Disposition { get; set; } 

        public DateTime? ImplementationDate { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public DateTime? NextReviewDate { get; set; }
        public long? ReviewedById { get; set; }
        public long? DepartmentId { get; set; }
        public string? CurrentIssue { get; set; } 
        public string? ReviewedByName { get; set; } 
        public string? CurrentRevision { get; set; } 
        public string? RequestNo { get; set; } 
        public string? ChangeType { get; set; } 
        public string? DescriptionOfChange { get; set; } 
        public string? ImpactOfChange { get; set; } 
        public string? Reference { get; set; } 
        public string? DepartmentName { get; set; } 
        public string? DepartmentDoc { get; set; } 
        public string? Designation { get; set; } 
        public string? DocumentOwner { get; set; } 
        public string? Priority { get; set; } 
        public string? DocumentName { get; set; } 
        public int? DocumentId { get; set; }
        public long? SourceReviewId { get; set; }
        public long? DesignationId { get; set; }
    }
}
