using System.ComponentModel.DataAnnotations;

namespace LIMSApi.Models
{
    public class ProductSpecification : AuditProperty
    {
        [Key]
        public long ID { get; set; }
        [StringLength(100),Required]
        public required string Name { get; set; }
        [StringLength(100)]
        public string? AliasName { get; set; }
        [StringLength(100)]
        public string? Code { get; set; }
        public string ? MaterialSpecification { get; set; } = string.Empty;
    }
}
