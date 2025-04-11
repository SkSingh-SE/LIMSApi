using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LIMSApi.Models
{
    public class OEMMaster : AuditProperty
    {
        [Key]
        public long ID { get; set; }

        [Required]
        [StringLength(100)]
        public required string Name { get; set; }

        [StringLength(100)]
        public string? ContactPerson1 { get; set; }
        [StringLength(100)]
        public string? ContactPerson2 { get; set; }
        [StringLength(100)]
        public string? ContactPerson3 { get; set; }

        [Phone, StringLength(100)]
        public string? ContactNo1 { get; set; }
        [Phone, StringLength(100)]
        public string? ContactNo2 { get; set; }
        [Phone, StringLength(100)]
        public string? ContactNo3 { get; set; }

        [StringLength(100)]
        [EmailAddress]
        public string? EmailId1 { get; set; }
        [StringLength(100)]
        [EmailAddress]
        public string? EmailId2 { get; set; }
        [StringLength(100)]
        [EmailAddress]
        public string? EmailId3 { get; set; }
        [StringLength (100)]
        public string? Note {  get; set; }

        [StringLength(255)]
        public string? AgreementFilePath { get; set; } // Store document path

        [Required]
        public bool SupplierApproved { get; set; }

        [Required]
        public bool IsBlacklisted { get; set; }

        [StringLength(255)]
        public string? ReasonForBlacklisting { get; set; }

        public long BlacklistedBy { get; set; }

        public long EvaluatedBy { get; set; } // Evaluation by GM

        public long ApprovedBy { get; set; } // Approval by MD / TD

        public DateTime? EvaluationDate { get; set; }
    }
}
