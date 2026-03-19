using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    public class ProductSpecificationGrade : AuditProperty
    {
        [Key]
        public long ID { get; set; }

        [Required]
        public long ProductSpecificationID { get; set; }

        [Required]
        public long SpecificationGradeID { get; set; }

        [MaxLength(200)]
        public string? AliasName { get; set; }

        [ForeignKey("ProductSpecificationID")]
        public virtual ProductSpecification? ProductSpecification { get; set; }

        [ForeignKey("SpecificationGradeID")]
        public virtual SpecificationGrade? SpecificationGrade { get; set; }
    }
}
