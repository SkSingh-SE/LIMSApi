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
        public string? TestMethod { get; set; }

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

        public DateTime? ReviewDate { get; set; }
    }
}
