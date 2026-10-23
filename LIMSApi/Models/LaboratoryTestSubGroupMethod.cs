using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    public class LaboratoryTestSubGroupMethod
    {
        [Key]
        public long ID { get; set; }

        [Required]
        public long LaboratoryTestSubGroupID { get; set; }

        [Required]
        public long TestMethodSpecificationID { get; set; }

        public bool IsDefault { get; set; } = false;

        [ForeignKey(nameof(LaboratoryTestSubGroupID))]
        public virtual LaboratoryTestSubGroup? SubGroup { get; set; }

        [ForeignKey(nameof(TestMethodSpecificationID))]
        public virtual TestMethodSpecification? TestMethodSpecification { get; set; }

        public long? TestMethodSpecificationVersionID { get; set; }

        [ForeignKey(nameof(TestMethodSpecificationVersionID))]
        public virtual TestMethodSpecificationVersion? TestMethodSpecificationVersion { get; set; }
    }
}
