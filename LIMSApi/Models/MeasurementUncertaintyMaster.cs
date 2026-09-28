using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    public class MeasurementUncertaintyMaster : AuditProperty
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

        public long? LaboratoryTestID { get; set; }

        public long? ParameterID { get; set; }

        public long? TestMethodSpecificationID { get; set; }

        public long? TestMethodSpecificationVersionID { get; set; }

        public long? ParameterUnitID { get; set; }

        [Required]
        [StringLength(20)]
        public string UncertaintyType { get; set; } = "EXPANDED";

        [Required]
        [StringLength(20)]
        public string Basis { get; set; } = "Type B";

        [Column(TypeName = "decimal(18,6)")]
        public decimal? CombinedUncertainty { get; set; }

        [Column(TypeName = "decimal(18,6)")]
        public decimal? ExpandedUncertainty { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal CoverageFactor { get; set; } = 2.0m;

        [Column(TypeName = "decimal(5,2)")]
        public decimal? ConfidenceLevel { get; set; }

        public string? ComponentsJson { get; set; }

        public int DisplayOrder { get; set; } = 0;

        [ForeignKey(nameof(LaboratoryTestID))]
        public virtual LaboratoryTest? LaboratoryTest { get; set; }

        [ForeignKey(nameof(ParameterID))]
        public virtual ParameterMaster? Parameter { get; set; }

        [ForeignKey(nameof(TestMethodSpecificationID))]
        public virtual TestMethodSpecification? TestMethodSpecification { get; set; }

        [ForeignKey(nameof(TestMethodSpecificationVersionID))]
        public virtual TestMethodSpecificationVersion? TestMethodSpecificationVersion { get; set; }

        [ForeignKey(nameof(ParameterUnitID))]
        public virtual ParameterUnitMaster? ParameterUnit { get; set; }
    }
}
