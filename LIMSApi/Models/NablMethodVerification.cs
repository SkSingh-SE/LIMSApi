using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    [Table("NablMethodVerifications")]
    public class NablMethodVerification : NablFormBase
    {
        [MaxLength(100)]
        public string? TestMethodCode { get; set; }

        [MaxLength(500)]
        public string? TestParameter { get; set; }

        [MaxLength(200)]
        public string? TestMatrix { get; set; }

        public DateTime? VerificationDate { get; set; }

        [MaxLength(200)]
        public string? VerificationType { get; set; }

        [MaxLength(500)]
        public string? LinearityResults { get; set; }

        [MaxLength(500)]
        public string? PrecisionResults { get; set; }

        [MaxLength(500)]
        public string? BiasResults { get; set; }

        [MaxLength(500)]
        public string? UncertaintyResults { get; set; }

        [MaxLength(50)]
        public string? OverallConclusion { get; set; } // Verified/NotVerified

        [MaxLength(200)]
        public string? VerifiedBy { get; set; }

        public DateTime? NextVerificationDate { get; set; }
        public string? TestMethodName { get; set; }
        public string? RevIssue { get; set; }
        public string? ReferenceStandard { get; set; }
        public string? Humidity { get; set; }
        public string? Temperature { get; set; }
        public string? EquipmentId { get; set; }
        public string? EquipmentName { get; set; }
        public string? Conclusion { get; set; }
        public string? VerificationStatus { get; set; }
        public string? ReasonNotVerified { get; set; }
        public decimal? RecoveryMin { get; set; }
        public decimal? RecoveryMax { get; set; }
        public decimal? RsdMax { get; set; }
        public decimal? BiasMax { get; set; }
        public DateTime? CalibrationDueDate { get; set; }
        public string? CrmParametersJson { get; set; }
        public string? VerificationDataJson { get; set; }

        [NotMapped]
        public List<CrmParameters>? CrmParameters { get; set; }

        [NotMapped]
        public List<VerificationData>? VerificationData { get; set; }
    }
    [NotMapped]
    public class CrmParameters
    {
        public string CrmSampleId { get; set; }
        public string CertificateNo { get; set; }
        public decimal ReferenceValue { get; set; }
        public string Unit { get; set; }
        public string MeasurementUncertainty { get; set; }
    }
    [NotMapped]
    public class VerificationData
    {
        public string CrmSampleId { get; set; }
        public decimal ReferenceValue { get; set; }
        public string Unit { get; set; }
        public decimal ObservationValue { get; set; }
        public decimal Difference { get; set; }
        public decimal Recovery { get; set; }
        public string Status { get; set; }
    }
}
