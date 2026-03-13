using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    [Table("NablSupplierEvaluations")]
    public class NablSupplierEvaluation : NablFormBase
    {
        public long? SupplierId { get; set; } // FK to Supplier (not yet in codebase)

        [MaxLength(200)]
        public string? SupplierName { get; set; }

        public DateTime? EvaluationDate { get; set; }

        public string? EvaluationCriteria { get; set; } // JSON array of {criteria, maxScore, obtainedScore}

        [Column(TypeName = "decimal(8,2)")]
        public decimal? TotalScore { get; set; }

        [Column(TypeName = "decimal(8,2)")]
        public decimal? MaxScore { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal? PercentageScore { get; set; }

        [MaxLength(50)]
        public string? EvaluationResult { get; set; } // Approved/ConditionalApproval/Rejected

        [MaxLength(200)]
        public string? EvaluatedBy { get; set; }

        public DateTime? NextEvaluationDate { get; set; }

        [MaxLength(500)]
        public string? Remarks { get; set; }
    }
}
