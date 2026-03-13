using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    [Table("NablTrainingAttendances")]
    public class NablTrainingAttendance : NablFormBase
    {
        public long? TrainingPlanId { get; set; }

        [ForeignKey("TrainingPlanId")]
        public virtual NablTrainingPlan? TrainingPlan { get; set; }

        [MaxLength(500)]
        public string? TrainingTopic { get; set; }

        public DateTime? TrainingDate { get; set; }

        [MaxLength(200)]
        public string? TrainerName { get; set; }

        [MaxLength(200)]
        public string? VenueMode { get; set; }

        public string? AttendeesJson { get; set; } // JSON array of {EmployeeId, EmployeeName, Designation, Signature}

        public int? TotalAttendees { get; set; }
    }
}
