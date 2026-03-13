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
    }
}
