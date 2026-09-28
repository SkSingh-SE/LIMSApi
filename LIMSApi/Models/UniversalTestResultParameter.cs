using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    [Table("UniversalTestResultParameters")]
    public class UniversalTestResultParameter : AuditProperty
    {
        [Key]
        public long ID { get; set; }

        public long UniversalTestResultID { get; set; }

        public long ParameterMasterID { get; set; }

        [Required]
        [StringLength(100)]
        public string ParameterCode { get; set; } = string.Empty;

        [StringLength(200)]
        public string? ParameterName { get; set; }

        [StringLength(20)]
        public string InputType { get; set; } = "Decimal";

        [StringLength(50)]
        public string? Unit { get; set; }

        public int DecimalPrecision { get; set; } = 2;

        public bool IsCalculated { get; set; }
        public bool IsMandatory { get; set; } = true;
        public bool IsReportable { get; set; } = true;

        public int DisplayOrder { get; set; }

        [StringLength(255)]
        public string? RawValue { get; set; }

        [Column(TypeName = "decimal(18,6)")]
        public decimal? RawNumericValue { get; set; }

        [StringLength(100)]
        public string? AppliedFactorCode { get; set; }

        [StringLength(100)]
        public string? AppliedFactorOperation { get; set; }

        [Column(TypeName = "decimal(18,6)")]
        public decimal? FactoredValue { get; set; }

        public string? Formula { get; set; }

        public string? SubstitutionTrace { get; set; }

        [Column(TypeName = "decimal(18,6)")]
        public decimal? CalculatedValue { get; set; }

        [Column(TypeName = "decimal(18,6)")]
        public decimal? ComplianceValue { get; set; }

        [StringLength(100)]
        public string? DisplayValue { get; set; }

        [StringLength(100)]
        public string? ReportedValue { get; set; }

        [Column(TypeName = "decimal(18,6)")]
        public decimal? SpecMin { get; set; }

        [Column(TypeName = "decimal(18,6)")]
        public decimal? SpecMax { get; set; }

        [Column(TypeName = "decimal(18,6)")]
        public decimal? SpecTarget { get; set; }

        [Column(TypeName = "decimal(18,6)")]
        public decimal? MinTolerance { get; set; }

        [Column(TypeName = "decimal(18,6)")]
        public decimal? MaxTolerance { get; set; }

        [Column(TypeName = "decimal(18,6)")]
        public decimal? EffectiveMin { get; set; }

        [Column(TypeName = "decimal(18,6)")]
        public decimal? EffectiveMax { get; set; }

        [StringLength(50)]
        public string? ToleranceSource { get; set; }

        [StringLength(50)]
        public string? ToleranceType { get; set; }

        [Column(TypeName = "decimal(18,6)")]
        public decimal? AppliedTolerance { get; set; }

        [StringLength(50)]
        public string? ComplianceValueSource { get; set; }

        [Required]
        [StringLength(40)]
        public string RequirementStatus { get; set; } = "RESOLVED";

        [Required]
        [StringLength(30)]
        public string Verdict { get; set; } = "NOT_EVALUATED";

        [Column(TypeName = "decimal(18,6)")]
        public decimal? CombinedUncertainty { get; set; }

        [Column(TypeName = "decimal(18,6)")]
        public decimal? ExpandedUncertainty { get; set; }

        [Column(TypeName = "decimal(10,4)")]
        public decimal? CoverageFactor { get; set; }

        [StringLength(50)]
        public string? MUSource { get; set; }

        public bool GuardBandApplied { get; set; }

        [StringLength(500)]
        public string? EvaluationNote { get; set; }

        [ForeignKey("UniversalTestResultID")]
        public virtual UniversalTestResult? UniversalTestResult { get; set; }
    }
}
