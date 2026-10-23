using LIMSApi.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Dtos
{
    public class TrainingAttendancePrintDto
    {
        public long? TrainingPlanId { get; set; }
        public string TrainingTopic { get; set; }
        public int? PlanningYear { get; set; }
        public string Month { get; set; }
        public string Audience { get; set; } // Map to 'To be attended by'
        public string ProviderAgencyName { get; set; }
        public long? ProviderAgencyId { get; set; }
        public bool? EvaluationRequired { get; set; }
        public string Agency { get; set; }
        public string QuestionSet { get; set; }
        public long? QuestionSetId { get; set; }
        public DateTime? PlanDate { get; set; }
        public string? TrainerName { get; set; }
         public string? VenueMode { get; set; }
        public DateTime? TrainingDatetime { get; set; }
        public string? PreparedBy { get; set; }

        [MaxLength(200)]
        public string? ReviewedBy { get; set; }

        [MaxLength(200)]
        public string? ApprovedBy { get; set; }

        [NotMapped]
        public List<Participates>? Participants { get; set; }
    }
    
}
