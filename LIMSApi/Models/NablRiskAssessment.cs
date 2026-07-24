using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    [Table("NablRiskAssessments")]
    public class NablRiskAssessment : NablFormBase
    {
        public DateTime? AssessmentDate { get; set; }

        [MaxLength(500)]
        public string? ProcessArea { get; set; }

        public string? RisksJson { get; set; } // JSON array of {riskId, riskDescription, cause, likelihood: 1-5, severity: 1-5, riskScore, currentControls, treatmentAction, residualLikelihood, residualSeverity, residualScore, riskOwner, targetDate, status}

        [MaxLength(50)]
        public string? OverallRiskLevel { get; set; } // Low/Medium/High/Critical

        [MaxLength(200)]
        public string? AssessedBy { get; set; }
        public DateTime? RiskDate { get; set; }
        public string? RiskNo { get; set; }
        public string? DepartmentName { get; set; }
        public string? Type { get; set; }
        public string? Impact { get; set; }
        public string? Likelihood { get; set; }
        public string? Category { get; set; }
        public string? IdentifiedByName { get; set; }
        public string? RiskLevel { get; set; }
        public long? DepartmentId { get; set; }
        public long? IdentifiedById { get; set; }
        public int? RiskScore { get; set; }
        public string? Opportunity { get; set; }
        public string? ExistingSituation { get; set; }
        public string? ExpectedBenefit { get; set; }
        public string? Title { get; set; }
        public string? ExistingControls { get; set; }
        public string? RiskOwner { get; set; }
        public string? EffectivenessRemarks { get; set; }
        public string? Effectiveness { get; set; }
        public string? RiskRemarks { get; set; }
        [NotMapped]
        public List<ActionPlans>? ActionPlans { get; set; }
    }
    [NotMapped]
    public class ActionPlans
    {
        public string? Action { get; set; }
        public string? ResponsiblePerson { get; set; }
        public DateTime? TargetDate { get; set; }
        public DateTime? CompletionDate { get; set; }
        public string? Status { get; set; }
    }
}
