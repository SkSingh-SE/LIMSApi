using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    [Table("NablEmployeeCompetences")]
    public class NablEmployeeCompetence : NablFormBase
    {
        // FK to existing Employee master
        public long EmployeeId { get; set; }

        [ForeignKey("EmployeeId")]
        public virtual EmployeeMaster? Employee { get; set; }

        [MaxLength(200)]
        public string? EmployeeName { get; set; }

        [MaxLength(100)]
        public string? DesignationName { get; set; }

        // Evaluation Period
        public DateTime? EvaluationPeriodFrom { get; set; }
        public DateTime? EvaluationPeriodTo { get; set; }

        // Parameters stored as JSON array
        public string? ParametersJson { get; set; }

        // Overall
        public decimal OverallRating { get; set; }

        [MaxLength(500)]
        public string? SpecificTrainingRequired { get; set; }

        [MaxLength(200)]
        public string? EvaluationDoneBy { get; set; }

        public DateTime? EvaluationDate { get; set; }
    }
}
