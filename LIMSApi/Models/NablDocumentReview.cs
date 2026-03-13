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
        public string? ReviewConclusion { get; set; } // ApprovedWithoutChange/ApprovedWithChange/Obsolete
    }
}
