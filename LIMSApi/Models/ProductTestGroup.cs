using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    public class ProductTestGroup : AuditProperty
    {
        [Key]
        public long ID { get; set; }

        [Required]
        public long ProductSpecificationID { get; set; }

        [Required]
        public long LaboratoryTestID { get; set; }

        public long? TestMethodStandardID { get; set; }

        public bool IsPerBatch { get; set; }

        public int? Year { get; set; }

        [MaxLength(500)]
        public string? Remark { get; set; }

        [ForeignKey("ProductSpecificationID")]
        public virtual ProductSpecification? ProductSpecification { get; set; }

        [ForeignKey("LaboratoryTestID")]
        public virtual LaboratoryTest? LaboratoryTest { get; set; }

        [ForeignKey("TestMethodStandardID")]
        public virtual TestMethodStandard? TestMethodStandard { get; set; }
    }
}
