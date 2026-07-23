using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    [Table("NablMeasurementUncertainties")]
    public class NablMeasurementUncertainty : NablFormBase
    {
        [MaxLength(500)]
        public string? TestParameter { get; set; }

        [MaxLength(500)]
        public string? TestMethodName { get; set; }

        [MaxLength(200)]
        public string? MatrixType { get; set; }

        [MaxLength(50)]
        public string? UncertaintyType { get; set; } // Type_A/Type_B/Combined

        public string? SourcesJson { get; set; } // JSON array of {source, distribution, value, sensitivity, contribution}

        [Column(TypeName = "decimal(18,6)")]
        public decimal? CombinedUncertainty { get; set; }

        [Column(TypeName = "decimal(18,6)")]
        public decimal? ExpandedUncertainty { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal? CoverageFactor { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal? ConfidenceLevel { get; set; }

        [MaxLength(100)]
        public string? Unit { get; set; }

        [MaxLength(200)]
        public string? ValidatedBy { get; set; }
        public string? MUCode { get; set; }
        public long? LaboratoryTestID { get; set; }
        public long? TestMethodID { get; set; }
        public long? EquipmentID { get; set; }
        public string? EquipmentName { get; set; }
        public string? LaboratoryTestName { get; set; }
        public string? Version { get; set; }
        public string? Remarks { get; set; }
        public decimal? SumOfSquares { get; set; }

        public DateTime? ReviewDate { get; set; }
        public DateTime? EffectiveDate { get; set; }
        [NotMapped]
        public List<UncertaintySources>? UncertaintySources { get; set; }
    }
    [NotMapped]
    public class UncertaintySources
    {
        public string? Source { get; set; }
        public string? Type { get; set; }
        public string? Distribution { get; set; }
        public decimal? InputValue { get; set; }
        public decimal? Divisor { get; set; }
        public decimal? SensitivityCoefficient { get; set; }
        public decimal? StandardUncertainty { get; set; }
        public string? Unit { get; set; }
        public string? Remarks { get; set; }

    }
}
