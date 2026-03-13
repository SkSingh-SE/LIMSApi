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
        public string? UrgencyLevel { get; set; } // Routine/Urgent/Critical

        public string? AssessedImpact { get; set; }

        [MaxLength(200)]
        public string? AssessmentBy { get; set; }

        public DateTime? AssessmentDate { get; set; }

        [MaxLength(50)]
        public string? Disposition { get; set; } // Approved/Rejected/Deferred

        public DateTime? ImplementationDate { get; set; }
    }
}
