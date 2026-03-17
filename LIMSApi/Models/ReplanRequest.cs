using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LIMSApi.Models
{
    [Table("ReplanRequests")]
    public class ReplanRequest : AuditProperty
    {
        [Key]
        public long Id { get; set; }

        public long PlanId { get; set; }  // FK to SampleTestPlan

        public long RequestedById { get; set; }

        [MaxLength(200)]
        public string? RequestedByName { get; set; }

        public DateTime RequestedAt { get; set; } = DateTime.UtcNow;

        [Required, MaxLength(500)]
        public string Reason { get; set; } = string.Empty;

        [MaxLength(50)]
        public string Status { get; set; } = "Pending";  // Pending, Approved, Rejected

        public long? ApprovedById { get; set; }

        [MaxLength(200)]
        public string? ApprovedByName { get; set; }

        public DateTime? ApprovedAt { get; set; }

        [MaxLength(500)]
        public string? ApprovalRemarks { get; set; }

        [ForeignKey("PlanId"), JsonIgnore]
        public virtual SampleTestPlan? SampleTestPlan { get; set; }
    }
}
