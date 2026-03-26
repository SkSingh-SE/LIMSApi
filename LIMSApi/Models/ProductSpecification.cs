using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    public class ProductSpecification : AuditProperty
    {
        [Key]
        public long ID { get; set; }
        [StringLength(100),Required]
        public required string SpecificationName { get; set; }
        [StringLength(100)]
        public string AliasName { get; set; } = string.Empty;
        public string? Size { get; set; }
        [StringLength(100)]
        public string SpecificationCode { get; set; } = string.Empty;
        public long GradeID { get; set; }
        public long LaboratoryTestID { get; set; }
        public long MetalClassificationID { get; set; }
        public long TestMethodSpecificationID { get; set; }
        public long? TestMethodSpecificationVersionID { get; set; }

        public bool IsCustom { get; set; }
        [ForeignKey("GradeID")]
        public virtual SpecificationGrade? SpecificationGrade { get; set; }

        [ForeignKey("LaboratoryTestID")]
        public virtual LaboratoryTest? LaboratoryTest { get; set; }
        [ForeignKey("MetalClassificationID")]
        public virtual MetalClassificationMaster? MetalClassification { get; set; }
        [ForeignKey("TestMethodSpecificationID")]
        public virtual TestMethodSpecification? TestMethodSpecification { get; set; }

        [ForeignKey("TestMethodSpecificationVersionID")]
        public virtual TestMethodSpecificationVersion? TestMethodSpecificationVersion { get; set; }

        public virtual ICollection<ProductTestGroup> ProductTestGroups { get; set; } = new List<ProductTestGroup>();
        public virtual ICollection<ProductSpecificationGrade> ProductSpecificationGrades { get; set; } = new List<ProductSpecificationGrade>();
    }
}
