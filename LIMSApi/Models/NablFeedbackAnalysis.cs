using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    [Table("NablFeedbackAnalyses")]
    public class NablFeedbackAnalysis : NablFormBase
    {
        public DateTime? AnalysisPeriodFrom { get; set; }

        public DateTime? AnalysisPeriodTo { get; set; }

        public int? TotalFeedbacks { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal? AverageSatisfaction { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal? AverageTurnaround { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal? AverageAccuracy { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal? AverageCommunication { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal? AverageService { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal? OverallScore { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal? AcceptanceCriteria { get; set; }

        public bool? MeetsAcceptanceCriteria { get; set; }

        public string? KeyStrengths { get; set; }

        public string? AreasForImprovement { get; set; }

        public string? ActionPlan { get; set; }

        [MaxLength(200)]
        public string? AnalysedBy { get; set; }

        // Inter-form linking: FK to CustomerFeedback
        public long? CustomerFeedbackId { get; set; }

        [ForeignKey("CustomerFeedbackId")]
        public virtual NablCustomerFeedback? CustomerFeedback { get; set; }

        public long? CustomerID { get; set; }
        public string? ActionDetails { get; set; }
        public string? ActionTaken { get; set; }
        public string? Address { get; set; }
        public string? AnalysisNo { get; set; }
        public string? ContactPerson { get; set; }
        public string? CorrectiveActionRequired { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerRemarks { get; set; }
        public string? EffectivenessStatus { get; set; }
        public string? Email { get; set; }
        public string? FinalStatus { get; set; }
        public string? Suggestions { get; set; }
        public string? ImprovementOpportunity { get; set; }
        public string? IssuesIdentified { get; set; }
        public string? MobileNo { get; set; }
        public string? RootCause { get; set; }
        public string? ResponsiblePerson { get; set; }
        public string? NewRequirement { get; set; }
        public string? OverallConclusion { get; set; }
        public string? OverallCustomerSatisfaction { get; set; }
        public string? OverallGrade { get; set; }
        public decimal? AverageRating { get; set; }
        public string? PositiveObservations { get; set; }
        public string? VerificationRemarks { get; set; }
        public DateTime? AnalysisDate { get; set; }
        public DateTime? TargetCompletionDate { get; set; }
        public DateTime? VerificationDate { get; set; }
        public DateTime? FeedbackDate { get; set; }
        public string? RatingsJson { get; set; }

        [NotMapped]
        public List<FeedbackRatings> FeedbackRatings{ get; set; }
    }

    [NotMapped]
    public class FeedbackRatings
    {
        public string? ParameterName { get; set; }
        public int? Rating { get; set; }
    }
}
