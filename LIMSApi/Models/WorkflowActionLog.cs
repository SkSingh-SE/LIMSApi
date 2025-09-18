using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    public class WorkflowActionLog
    {
        [Key]
        public long ID { get; set; }
        public long WorkflowID { get; set; }
        public long InstanceID { get; set; }
        public long StepID { get; set; }

        [Required, MaxLength(100)]
        public string Action { get; set; }

        public long EmployeeID { get; set; }

        [MaxLength(1000)]
        public string Comments { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        [ForeignKey("StepID")]
        public virtual WorkflowStep? WorkflowStep { get; set; }
        [ForeignKey("EmployeeID")]
        public virtual EmployeeMaster? Employee { get; set; }
        [ForeignKey("WorkflowID")]
        public virtual Workflow? Workflow { get; set; }

    }
}
