using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    [Table("NablProductInspections")]
    public class NablProductInspection : NablFormBase
    {
        public long? SupplierId { get; set; }

        [ForeignKey("SupplierId")]
        public virtual SupplierMaster? Supplier { get; set; }

        [MaxLength(200)]
        public string? SupplierName { get; set; }

        [MaxLength(500)]
        public string? ItemDescription { get; set; }

        [MaxLength(200)]
        public string? PurchaseOrderNo { get; set; }

        public DateTime? InspectionDate { get; set; }

        [MaxLength(200)]
        public string? InspectionBy { get; set; }

        public int? SampleSize { get; set; }

        public int? DefectsFound { get; set; }

        [MaxLength(500)]
        public string? InspectionCriteria { get; set; }

        public string? InspectionResultsJson { get; set; } // JSON array

        [MaxLength(50)]
        public string? OverallResult { get; set; } // Pass/Fail

        public string? CorrectiveAction { get; set; }
        public string? ProductName { get; set; }
        public string? ProductCode { get; set; }
        public string? Category { get; set; }
        public string? InspectionStage { get; set; }
        public string? Remarks { get; set; }
        public string? PlanNo { get; set; }
        public string? Risklevel { get; set; }
        [NotMapped]
        public List<Inspectionparameters>? Parameters { get; set; }
    }
    public class Inspectionparameters
    {
        public string? ParameterName { get; set; }
        public string? Requirement { get; set; }
        public string? ReferenceStandard { get; set; }
        public string? MethodOfCheck { get; set; }
        public string? Frequency { get; set; }
        public string? AcceptanceCriteria { get; set; }
    }
}
