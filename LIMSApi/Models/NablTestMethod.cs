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

        public string TestMethodJson { get; set; }
        public string OrginDocJson { get; set; }
        [NotMapped]
        public List<TestMethod>? TestMethod { get; set; }
        [NotMapped]
        public List<DocEntries>? DocEntries { get; set; }

    }

    [NotMapped]
    public class TestMethod
    {
        public string MethodName { get; set; }
        public string SpecificationCode { get; set; }
        public string ReferenceStandard { get; set; }
        public string RevisionNo { get; set; }
        public DateTime EffectiveDate { get; set; }
        public string Status { get; set; }
        public bool IsVerified { get; set; }
        public bool IsValidated { get; set; }
    }
    [NotMapped]
    public class DocEntries
    {
        public string DocId { get; set; }
        public string Description { get; set; }
        public string SpecificationCode { get; set; }
        public string DocSource { get; set; }
        public string DocType { get; set; }
        public string Issue { get; set; }
        public DateTime MonthYear { get; set; }
    }

}
