using System.ComponentModel.DataAnnotations;

namespace LIMSApi.Models
{
    public class Workflow : AuditProperty
    {
        [Key]
        public long ID { get; set; }

        [Required, MaxLength(200)]
        public string Name { get; set; }

        [Required, MaxLength(100)]
        public string EntityType { get; set; } // "Sample", "Plan", "Report"

        public ICollection<WorkflowStep> Steps { get; set; } = new List<WorkflowStep>();
    }
}
