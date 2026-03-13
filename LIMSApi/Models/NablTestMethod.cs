using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    [Table("NablTestMethods")]
    public class NablTestMethod : NablFormBase
    {
        public long? TestMethodStandardId { get; set; }

        [ForeignKey("TestMethodStandardId")]
        public virtual TestMethodStandard? TestMethodStandard { get; set; }

        [MaxLength(100)]
        public string? TestMethodCode { get; set; }

        [MaxLength(500)]
        public string? TestMethodTitle { get; set; }

        [MaxLength(500)]
        public string? TestParameter { get; set; }

        [MaxLength(200)]
        public string? TestMatrix { get; set; }

        [MaxLength(500)]
        public string? Scope { get; set; }

        [MaxLength(500)]
        public string? Principle { get; set; }

        [MaxLength(500)]
        public string? ApplicableStandard { get; set; }

        [MaxLength(500)]
        public string? EquipmentRequired { get; set; }

        [MaxLength(500)]
        public string? ReagentsRequired { get; set; }

        [MaxLength(500)]
        public string? SamplePreparation { get; set; }

        public string? Procedure { get; set; } // long text

        [MaxLength(500)]
        public string? CalibrationRequirements { get; set; }

        [MaxLength(500)]
        public string? QualityControlRequirements { get; set; }

        [MaxLength(500)]
        public string? AcceptanceCriteria { get; set; }

        [MaxLength(500)]
        public string? UncertaintyStatement { get; set; }

        [MaxLength(200)]
        public string? DetectionLimit { get; set; }
    }
}
