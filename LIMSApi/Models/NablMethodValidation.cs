using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    [Table("NablMethodValidations")]
    public class NablMethodValidation : NablFormBase
    {
        [MaxLength(100)]
        public string? TestMethodCode { get; set; }

        [MaxLength(500)]
        public string? TestParameter { get; set; }

        [MaxLength(200)]
        public string? TestMatrix { get; set; }

        public DateTime? ValidationDate { get; set; }

        [MaxLength(500)]
        public string? ValidationScope { get; set; }

        [MaxLength(500)]
        public string? SelectivityResults { get; set; }

        [MaxLength(200)]
        public string? LinearityRange { get; set; }

        [MaxLength(200)]
        public string? DetectionLimit { get; set; }

        [MaxLength(200)]
        public string? QuantificationLimit { get; set; }

        [Column(TypeName = "decimal(8,4)")]
        public decimal? PrecisionRSD { get; set; }

        [Column(TypeName = "decimal(8,4)")]
        public decimal? BiasPercentage { get; set; }

        [MaxLength(500)]
        public string? RobustnessResults { get; set; }

        [MaxLength(500)]
        public string? UncertaintyResults { get; set; }

        [MaxLength(50)]
        public string? OverallConclusion { get; set; } // Validated/NotValidated

        [MaxLength(200)]
        public string? ValidatedBy { get; set; }

        public DateTime? NextValidationDate { get; set; }
        public DateTime? VerificationDate { get; set; }
        public string? VerifiedBy { get; set; }
        public string? ValidationType { get; set; }
        public string? ValidStatus { get; set; }
        public string? TestMethodName { get; set; }
        public string? RevIssue { get; set; }
        public string? ReferenceStandard { get; set; }
        public string? Humidity { get; set; }
        public string? Temperature { get; set; }
        public string? EquipmentId { get; set; }
        public string? EquipmentName { get; set; }
        public string? Conclusion { get; set; }
        public string? ReasonNotValid { get; set; }
        public string? ReasonForValidation { get; set; }
        public decimal? RecoveryMin { get; set; }
        public decimal? RecoveryMax { get; set; }
        public decimal? RsdMax { get; set; }
        public decimal? BiasMax { get; set; }
        public decimal? ConfidenceLevel { get; set; }
        public decimal? CoverageFactor { get; set; }
        public decimal? ExpandedUncertainty { get; set; }
        public decimal? MeasurementUncertainty { get; set; }
        public bool? Precision { get; set; }
        public bool? Recovery { get; set; }
        public bool? Repeatability { get; set; }
        public bool? Robustness { get; set; }
        public bool? Measurement { get; set; }
        public bool? Accuracy { get; set; }
        public string? PrecisionStudyJson { get; set; }
        public string? CrmMaterialParametersJson { get; set; }
        public string? AccuracyStudyJson { get; set; }
        [NotMapped]
        public List<PrecisionStudy>? PrecisionStudy { get; set; }
        [NotMapped]
        public List<AccuracyStudy>? AccuracyStudy{ get; set; }
        [NotMapped]
        public List<CrmMaterialParameters>? CrmMaterialParameters { get; set; }
    }
    [NotMapped]
    public class PrecisionStudy
    {
        public string CrmSampleId { get; set; }
        public decimal ReferenceValue { get; set; }
        public string Unit { get; set; }
        public decimal? Reading1 { get; set; }
        public decimal? Reading2 { get; set; }
        public decimal? Reading3 { get; set; }
        public decimal? Reading4 { get; set; }
        public decimal Reading5 { get; set; }
        public decimal Mean { get; set; }
        public decimal SD { get; set; }
        public decimal RSD { get; set; }
        public string Status { get; set; }
    }
    [NotMapped]
    public class CrmMaterialParameters
    {
        public string CrmSampleId { get; set; }
        public string CertificateNo { get; set; }
        public decimal ReferenceValue { get; set; }
        public string Unit { get; set; }
        public string MeasurementUncertainty { get; set; }
    }
    [NotMapped]
    public class AccuracyStudy
    {
        public string CrmSampleId { get; set; }
        public decimal ReferenceValue { get; set; }
        public decimal Difference { get; set; }
        public decimal ObservationValue { get; set; }
        public decimal Recovery { get; set; }
        public string Unit { get; set; }
        public string Status { get; set; }
    }
}
