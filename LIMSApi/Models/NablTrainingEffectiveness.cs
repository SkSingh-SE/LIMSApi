using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    [Table("NablTrainingEffectiveness")]
    public class NablTrainingEffectiveness : NablFormBase
    {
        public long? TrainingPlanId { get; set; }

        [ForeignKey("TrainingPlanId")]
        public virtual NablTrainingPlan? TrainingPlan { get; set; }

        public long? EmployeeId { get; set; }

        [ForeignKey("EmployeeId")]
        public virtual EmployeeMaster? Employee { get; set; }

        [MaxLength(200)]
        public string? EmployeeName { get; set; }

        [MaxLength(500)]
        public string? TrainingTopic { get; set; }

        public DateTime? TrainingDate { get; set; }

        [MaxLength(200)]
        public string? EvaluationMethod { get; set; }

        public DateTime? EvaluationDate { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal? KnowledgeScore { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal? SkillScore { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal? OverallScore { get; set; }

        [MaxLength(50)]
        public string? EffectivenessResult { get; set; } // Effective/NotEffective

        [MaxLength(500)]
        public string? ActionRequired { get; set; }

        public DateTime? ReEvaluationDate { get; set; }

        [MaxLength(200)]
        public string? EvaluatedBy { get; set; }
    }
}
