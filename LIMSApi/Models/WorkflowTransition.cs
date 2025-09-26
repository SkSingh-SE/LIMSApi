using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace LIMSApi.Models
{
    public class WorkflowTransition
    {
        [Key]
        public long ID { get; set; }
        public long StepID { get; set; }
        public long? ToStepID { get; set; }
        public string? ToStepName { get; set; }

        [Required, MaxLength(100)]
        public string Action { get; set; } = string.Empty; // Next, Back, Cancel
        public string Alias { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;

        [ForeignKey("StepID"), JsonIgnore]
        public virtual WorkflowStep? WorkflowStep { get; set; }
    }
}
