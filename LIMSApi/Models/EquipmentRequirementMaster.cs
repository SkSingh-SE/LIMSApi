using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    public class EquipmentRequirementMaster : AuditProperty
    {
        [Key]
        public long ID { get; set; }

        [Required]
        [StringLength(50)]
        public string Code { get; set; } = string.Empty;

        [Required]
        [StringLength(150)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        [Required]
        public long LaboratoryTestID { get; set; }

        public long? TestMethodSpecificationID { get; set; }

        public long? TestMethodSpecificationVersionID { get; set; }

        public long? ParameterID { get; set; }

        [Required]
        public long EquipmentTypeID { get; set; }

        public long? EquipmentID { get; set; }

        [StringLength(500)]
        public string? RequiredCapability { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal? MinimumRange { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal? MaximumRange { get; set; }

        public long? RangeUnitID { get; set; }

        [StringLength(200)]
        public string? AccuracyRequirement { get; set; }

        [StringLength(200)]
        public string? ResolutionRequirement { get; set; }

        public bool IsMandatory { get; set; } = true;

        public int DisplayOrder { get; set; } = 0;

        [ForeignKey(nameof(LaboratoryTestID))]
        public virtual LaboratoryTest? LaboratoryTest { get; set; }

        [ForeignKey(nameof(TestMethodSpecificationID))]
        public virtual TestMethodSpecification? TestMethodSpecification { get; set; }

        [ForeignKey(nameof(TestMethodSpecificationVersionID))]
        public virtual TestMethodSpecificationVersion? TestMethodSpecificationVersion { get; set; }

        [ForeignKey(nameof(ParameterID))]
        public virtual ParameterMaster? Parameter { get; set; }

        [ForeignKey(nameof(EquipmentTypeID))]
        public virtual EquipmentTypeMaster? EquipmentType { get; set; }

        [ForeignKey(nameof(EquipmentID))]
        public virtual EquipmentMaster? Equipment { get; set; }

        [ForeignKey(nameof(RangeUnitID))]
        public virtual ParameterUnitMaster? RangeUnit { get; set; }
    }
}
