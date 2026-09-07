using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    public class LaboratoryTestMethod : AuditProperty
    {
        [Key]
        public long ID { get; set; }

        [Required]
        public long LaboratoryTestID { get; set; }

        [Required]
        public long TestMethodSpecificationID { get; set; }

        public bool IsDefault { get; set; } = false;

        public int DisplayOrder { get; set; } = 0;

        [ForeignKey(nameof(LaboratoryTestID))]
        public virtual LaboratoryTest? LaboratoryTest { get; set; }

        [ForeignKey(nameof(TestMethodSpecificationID))]
        public virtual TestMethodSpecification? TestMethodSpecification { get; set; }
    }
}
