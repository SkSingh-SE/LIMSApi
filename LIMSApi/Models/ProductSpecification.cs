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
        public string? AliasName { get; set; }
        [StringLength(100)]
        public string? SpecificationCode { get; set; }
        public long MateriaSpecificationID { get; set; }
        public bool IsCustom { get; set; }
        [ForeignKey("MateriaSpecificationID")]
        public virtual SpecificationHeader? SpecificationHeader { get; set; }
    }
}
