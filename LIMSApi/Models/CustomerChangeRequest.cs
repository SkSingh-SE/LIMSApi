using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LIMSApi.Models
{
    public class CustomerChangeRequest : AuditProperty
    {
        [Key]
        public long ID { get; set; }

        public long CustomerID { get; set; }

        // JSON snapshot of all Level 2 field values before the change
        [Column(TypeName = "nvarchar(max)")]
        public required string OldValuesJson { get; set; }

        // JSON snapshot of all Level 2 field values proposed by the user
        [Column(TypeName = "nvarchar(max)")]
        public required string NewValuesJson { get; set; }

        // Pending | Approved | Rejected | Superseded
        [MaxLength(50)]
        public required string Status { get; set; }

        [MaxLength(500)]
        public string? RejectionReason { get; set; }

        public long? ReviewedBy { get; set; }
        public DateTime? ReviewedOn { get; set; }

        // Linked workflow instance (null when no workflow is configured)
        public long? WorkflowInstanceID { get; set; }

        [ForeignKey("CustomerID"), JsonIgnore]
        public virtual Customer? Customer { get; set; }
    }
}
