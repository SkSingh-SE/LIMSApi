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
        public string? GenearalRemarks { get; set; }
        public DateTime? TrainingDatetime { get; set; }
        [NotMapped]
        public List<Participates>? Participants { get; set; }
    }
    [NotMapped]
    public class Participates
    {
        public int? SlNo { get; set; }
        public string? Feedback { get; set; }
        public string? FilePath { get; set; }
        public string? FileName{ get; set; }
        public string? ParticipantName { get; set; }
        public int? UploadReferenceID { get; set; }
    }
}
