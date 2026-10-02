using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    [Table("NablQualityControlPlans")]
    public class NablQualityControlPlan : NablFormBase
    {
        public long? DepartmentId { get; set; }

        [ForeignKey("DepartmentId")]
        public virtual DepartmentMaster? Department { get; set; }

        [MaxLength(200)]
        public string? DepartmentName { get; set; }

        [MaxLength(500)]
        public string? TestParameter { get; set; }

        [MaxLength(500)]
        public string? TestMethod { get; set; }

        [MaxLength(100)]
        public string? ControlType { get; set; } // Replicate/Retesting/BlankTest/ControlChart

        [MaxLength(200)]
        public string? Frequency { get; set; }

        [MaxLength(50)]
        public string? FrequencyUnit { get; set; } // Daily/Weekly/Monthly

        [MaxLength(200)]
        public string? ResponsiblePerson { get; set; }

        [MaxLength(500)]
        public string? AcceptanceCriteria { get; set; }

        public DateTime? NextDueDate { get; set; }

        public DateTime? LastPerformedDate { get; set; }

        [MaxLength(500)]
        public string? ActionOnFailure { get; set; }
        public string? PlanNo { get; set; }
        public string? RetentionPeriod { get; set; }
        public string? LabIncharge { get; set; }
        public string? MaterialProductGroup { get; set; }
        public string? Discipline { get; set; }
        public int? PlanYear { get; set; }
        public DateTime? EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public ICollection<NablQualityControlPlanActivity> Activities { get; set; }= new List<NablQualityControlPlanActivity>();
    }
}
