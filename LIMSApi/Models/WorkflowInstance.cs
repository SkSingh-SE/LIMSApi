using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LIMSApi.Models
{
    public class WorkflowInstance:AuditProperty
    {
        [Key]
        public long ID { get; set; }
        public long WorkflowID { get; set; }

        public long EntityID { get; set; } // e.g., SampleInward.ID
        public string EntityType { get; set; } = string.Empty;
        public long CurrentStepID { get; set; }

        [Required, MaxLength(50)]
        public string Status { get; set; } // Pending, Approved, Rejected, Completed
        [ForeignKey("WorkflowID"),JsonIgnore]
        public virtual Workflow? Workflow { get; set; }
        [ForeignKey("CurrentStepID"), JsonIgnore]
        public virtual WorkflowStep? CurrentStep { get; set; }

    }
}
