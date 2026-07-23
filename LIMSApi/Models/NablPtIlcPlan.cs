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
        public string? LaboratoryId { get; set; }
        public string? LaboratoryName { get; set; }
        public string? FieldOfAccreditation { get; set; }
        public string? ActivitiesJson { get; set; }
        public string? Note{ get; set; }
        public DateTime? PeriodStartDate { get; set; }
        public DateTime? PeriodEndDate { get; set; }
        [NotMapped]
        public List<PtilcActivity>? Activities { get; set; }
    }
    [NotMapped]
    public class PtilcActivity
    {
        public string? AccreditedDiscipline { get; set; }
        public string? GroupSubgroup { get; set; }

        public List<PtilcYear> Years { get; set; } = new();
    }
    [NotMapped]
    public class PtilcYear
    {
        public string? PtActivity { get; set; }
        public string? Status { get; set; }
        public string? Remarks { get; set; }
    }
}
