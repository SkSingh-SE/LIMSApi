using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    [Table("NablInductionTrainings")]
    public class NablInductionTraining : NablFormBase
    {
        // FK to existing Employee master
        public long? EmployeeId { get; set; }

        [ForeignKey("EmployeeId")]
        public virtual EmployeeMaster? Employee { get; set; }

        [MaxLength(200)]
        public string? EmployeeName { get; set; }

        [MaxLength(200)]
        public string? Qualification { get; set; }

        public DateTime? DateOfJoining { get; set; }

        [MaxLength(200)]
        public string? Position { get; set; }

        // On Job Training
        public DateTime? TrainingDate { get; set; }

        [MaxLength(200)]
        public string? TrainerName { get; set; }

        [MaxLength(200)]
        public string? TrainerDesignation { get; set; }

        [MaxLength(200)]
        public string? SampleName { get; set; }

        [MaxLength(100)]
        public string? SampleRefNo { get; set; }

        [MaxLength(200)]
        public string? Parameter { get; set; }

        [MaxLength(200)]
        public string? TestMethodSop { get; set; }

        // Training Evaluation
        public DateTime? EvaluationDate { get; set; }

        [MaxLength(100)]
        public string? EvaluationMode { get; set; } // Retesting or Replicate testing

        [MaxLength(200)]
        public string? EvalParameter { get; set; }

        [MaxLength(200)]
        public string? EvalTestMethodSop { get; set; }

        // Results
        [MaxLength(100)]
        public string? ObservedValue1 { get; set; }

        [MaxLength(100)]
        public string? ObservedValue2 { get; set; }

        [MaxLength(100)]
        public string? ObservedValueAverage { get; set; }

        [MaxLength(100)]
        public string? OriginalValue { get; set; }

        [MaxLength(500)]
        public string? Remarks { get; set; }

        [MaxLength(500)]
        public string? TrainerComments { get; set; }
    }
}
