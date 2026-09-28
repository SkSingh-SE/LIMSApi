using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    public class FactorConversionMaster : AuditProperty
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
        [StringLength(20)]
        public string FactorType { get; set; } = "MULTIPLICATION";

        [Column(TypeName = "decimal(18,6)")]
        public decimal FactorValue { get; set; } = 1.0m;

        [Required]
        public long InputParameterID { get; set; }

        public long? OutputParameterID { get; set; }

        public long? LaboratoryTestID { get; set; }

        public long? TestMethodSpecificationID { get; set; }

        public long? TestMethodSpecificationVersionID { get; set; }

        [StringLength(100)]
        public string AppliedOn { get; set; } = "Measured Value";

        public bool IsMandatory { get; set; } = true;

        public int DisplayOrder { get; set; } = 0;

        [ForeignKey(nameof(InputParameterID))]
        public virtual ParameterMaster? InputParameter { get; set; }

        [ForeignKey(nameof(OutputParameterID))]
        public virtual ParameterMaster? OutputParameter { get; set; }

        [ForeignKey(nameof(LaboratoryTestID))]
        public virtual LaboratoryTest? LaboratoryTest { get; set; }

        [ForeignKey(nameof(TestMethodSpecificationID))]
        public virtual TestMethodSpecification? TestMethodSpecification { get; set; }

        [ForeignKey(nameof(TestMethodSpecificationVersionID))]
        public virtual TestMethodSpecificationVersion? TestMethodSpecificationVersion { get; set; }
    }
}
