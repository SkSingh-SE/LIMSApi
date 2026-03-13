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

        public DateTime? ReviewDate { get; set; }
    }
}
