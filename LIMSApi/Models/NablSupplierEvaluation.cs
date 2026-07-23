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
        public long? SupplierRegisterId { get; set; }
        public string? Email { get; set; }
        public string? MobileNo { get; set; }
        public string? NatureOfBusiness { get; set; }
        public DateTime? EvaluatingPeriodFrom { get; set; }
        public DateTime? EvaluatingPeriodTo { get; set; }
        public string? CriteriaJson { get; set; } // JSON array of {criteria, maxScore, obtainedScore}
        public string? IncomingPlanJson { get; set; } // JSON array of {criteria, maxScore, obtainedScore}
        public string? POJson { get; set; } // JSON array of {criteria, maxScore, obtainedScore}
        public string? PresentStatus { get; set; } // JSON array of {criteria, maxScore, obtainedScore}
        public string? ProductsServicesOffered { get; set; } // JSON array of {criteria, maxScore, obtainedScore}
        public decimal? AcceptableLimitMin { get; set; }

        public string? Address { get; set; } // Approved/ConditionalApproval/Rejected
        public string? ContactPerson { get; set; } // Approved/ConditionalApproval/Rejected
        public string? ServiceProvider { get; set; }
        public bool? ToContinued { get; set; }
        public bool? ToRemoved { get; set; }
        public string? Recommendation { get; set; }
        public string? RegisterNo { get; set; }
        public string? GstNo { get; set; }
        [NotMapped]
        public List<Criteria>? Criteria { get; set; }

        [NotMapped]
        public List<IncomingPlan>? IncomingPlan { get; set; }

        [NotMapped]
        public List<PurchaseOrders>? PurchaseOrders { get; set; }

    }

    [NotMapped]
    public class Criteria
    {
        public int MaxScore { get; set; }
        public int ScoreObtained { get; set; }
        public string Parameter { get; set; }
        public string Remarks { get; set; }
    }
    [NotMapped]
    public class IncomingPlan
    {
        public DateTime Date { get; set; }
        public string InspectionPlanNoName { get; set; }
        public string InspectionResult { get; set; }
        public string PurchaseOrderNo { get; set; }
        public string SupplierName { get; set; }
    }
    [NotMapped]
    public class PurchaseOrders
    {
        public DateTime DeliveryDate{ get; set; }
        public DateTime PoDate { get; set; }
        public string PoNo { get; set; }
        public string ReferenceIndentNo { get; set; }
        public string SupplierName { get; set; }
    }
}
