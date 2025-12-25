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

        public decimal? Value { get; set; }
        public bool IsAdditional { get; set; } = false;

        public bool IsCalculated { get; set; } = false;
        public string? Formula { get; set; }

        // 🧪 Chemical Specification Range
        public decimal? MinValue { get; set; }
        public decimal? MaxValue { get; set; }

        // ✔ PASS / FAIL for this parameter
        public bool? IsWithinLimit { get; set; }

        public long? SpecificationLineID { get; set; }

        [ForeignKey(nameof(TestResultHeaderID))]
        public TestResultHeader? TestResultHeader { get; set; }

    }

}
