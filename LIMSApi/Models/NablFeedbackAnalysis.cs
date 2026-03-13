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
    }
}
