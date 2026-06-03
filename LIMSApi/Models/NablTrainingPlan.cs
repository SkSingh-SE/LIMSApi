using Microsoft.DotNet.Scaffolding.Shared;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    [Table("NablTrainingPlans")]
    public class NablTrainingPlan : NablFormBase
    {
        // Planning document fields (F-8) - matches frontend trainingPlanModel.ts
        public int? PlanningYear { get; set; }

        public decimal? TotalBudget { get; set; }

        [MaxLength(50)]
        public string? ApprovalStatus { get; set; } // Draft, Pending, Approved, Rejected

        // Courses stored as JSON array of { courseCode, courseName, provider, duration,
        // targetAudience, objectives, tentativeMonth, agency, remarks }
        public string? CoursesJson { get; set; }

        // Individual employee training record fields (kept for backward compatibility)
        public long? EmployeeId { get; set; }

        [ForeignKey("EmployeeId")]
        public virtual EmployeeMaster? Employee { get; set; }

        [MaxLength(200)]
        public string? EmployeeName { get; set; }

        [MaxLength(200)]
        public string? Department { get; set; }

        [MaxLength(500)]
        public string? TrainingTopic { get; set; }

        [MaxLength(500)]
        public string? TrainingObjective { get; set; }

        [MaxLength(50)]
        public string? TrainingType { get; set; } // Internal/External

        public DateTime? PlannedDate { get; set; }

        public DateTime? ActualDate { get; set; }

        [MaxLength(200)]
        public string? TrainerName { get; set; }

        [MaxLength(200)]
        public string? TrainerDesignation { get; set; }

        [MaxLength(100)]
        public string? Duration { get; set; }

        [MaxLength(200)]
        public string? VenueMode { get; set; }

        [MaxLength(200)]
        public string? NeedIdentifiedBy { get; set; }

        [MaxLength(50)]
        public string? TrainingStatus { get; set; } // Planned/Completed/Cancelled

        [MaxLength(500)]
        public string? CompletionRemarks { get; set; }
        public string? PlanMonth { get; set; }
        public string? Provider { get; set; }
        public string? TargetAudience { get; set; }
        public string? Objectives { get; set; }
        public string? Agency { get; set; }
       
    }
}