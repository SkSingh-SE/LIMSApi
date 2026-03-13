using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    [Table("NablPtIlcPlans")]
    public class NablPtIlcPlan : NablFormBase
    {
        public int? PlanYear { get; set; }

        [MaxLength(50)]
        public string? PTType { get; set; } // Proficiency_Testing/ILC

        [MaxLength(500)]
        public string? OrganizingBody { get; set; }

        public string? ScheduleJson { get; set; } // JSON array of {month, testParameter, matrix, sampleCode, participationStatus, zScore, resultStatus}

        public int? TotalParticipations { get; set; }

        public int? SatisfactoryResults { get; set; }

        public int? UnsatisfactoryResults { get; set; }

        public string? CorrectiveActions { get; set; }

        [MaxLength(200)]
        public string? ResponsiblePerson { get; set; }

        public string? OverallAssessment { get; set; }
    }
}
