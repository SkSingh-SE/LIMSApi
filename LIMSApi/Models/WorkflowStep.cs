using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LIMSApi.Models
{
    public class WorkflowStep
    {
        [Key]
        public long ID { get; set; }

        public long WorkflowID { get; set; }

        [Required, MaxLength(200)]
        public string Name { get; set; }

        public int OrderNo { get; set; }


        [Required, MaxLength(50)]
        public string AssignedToType { get; set; } // User, Role, Permission

        [Required, MaxLength(200)]
        public string AssignedToValue { get; set; }
        public bool IsActive { get; set; } = true;

        public ICollection<WorkflowTransition> Transitions { get; set; } = new List<WorkflowTransition>();
        [ForeignKey("WorkflowID"),JsonIgnore]
        public virtual Workflow? Workflow { get; set; }
    }
}
