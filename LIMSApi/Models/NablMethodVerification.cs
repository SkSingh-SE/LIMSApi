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
    }
}
