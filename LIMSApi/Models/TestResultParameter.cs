using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LIMSApi.Models
{
    [Table("TestResultParameters")]
    public class TestResultParameter
    {
        [Key]
        public long ID { get; set; }

        [Required]
        public long TestResultHeaderID { get; set; }

        [Required]
        public long ParameterID { get; set; }

        [MaxLength(200)]
        public string ParameterName { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Unit { get; set; } = string.Empty;
        public string? Remarks { get; set; }

        [Column(TypeName = "decimal(18,6)")]
        public decimal? Value { get; set; }
        public bool IsAdditional { get; set; } = false;

        public bool IsCalculated { get; set; } = false;
        public string? Formula { get; set; }

        // Chemical Specification Range
        [Column(TypeName = "decimal(18,6)")]
        public decimal? MinValue { get; set; }

        [Column(TypeName = "decimal(18,6)")]
        public decimal? MaxValue { get; set; }

        // PASS / FAIL for this parameter
        public bool? IsWithinLimit { get; set; }

        public long? SpecificationLineID { get; set; }

        // ----- Formula / Calculation enhancements -----
        [MaxLength(500)]
        public string? FormulaExpression { get; set; }
        public string? DependsOnParamsJson { get; set; }

        // ----- Specification range enhancements -----
        [Column(TypeName = "decimal(18,6)")]
        public decimal? SpecMinValue { get; set; }
        [Column(TypeName = "decimal(18,6)")]
        public decimal? SpecMaxValue { get; set; }
        [MaxLength(200)]
        public string? AcceptanceCriteria { get; set; }

        // ----- Standalone / cross-method parameters -----
        public bool IsStandalone { get; set; } = false;
        public long? SourceTestMethodId { get; set; }

        // ----- Billable flag (observed parameters excluded from pricing) -----
        public bool IsBillable { get; set; } = true;

        // ----- Result status -----
        [MaxLength(50)]
        public string? ResultStatus { get; set; }

        // ----- NABL Scope Validation (Phase 1) -----
        public bool? IsWithinNablScope { get; set; }
        [MaxLength(50)]
        public string? NablScopeStatus { get; set; }
        public long? LabScopeSpecParamId { get; set; }

        // ----- Uncertainty Integration (Phase 2) -----
        [Column(TypeName = "decimal(18,6)")]
        public decimal? ExpandedUncertainty { get; set; }
        [Column(TypeName = "decimal(10,2)")]
        public decimal? CoverageFactor { get; set; }
        [Column(TypeName = "decimal(18,6)")]
        public decimal? CombinedUncertainty { get; set; }
        public long? NablMeasurementUncertaintyId { get; set; }

        // ----- Parameter Type & Test Method Used -----
        [Column(TypeName = "varchar(20)")]
        public string? ParameterType { get; set; }  // "Qualitative" or "Quantitative"

        [MaxLength(200)]
        public string? TestMethodUsed { get; set; }  // e.g., "OES", "ICP", "Wet Chemistry"

        // ----- Unit Conversion & Display (for Report) -----
        public int DecimalPrecision { get; set; } = 2;

        [Column(TypeName = "decimal(18,6)")]
        public decimal ConversionFactor { get; set; } = 1;

        [Column(TypeName = "decimal(18,6)")]
        public decimal? ConvertedValue { get; set; }

        [MaxLength(50)]
        public string? SelectedUnit { get; set; }

        [ForeignKey(nameof(TestResultHeaderID))]
        public TestResultHeader? TestResultHeader { get; set; }

    }

}
