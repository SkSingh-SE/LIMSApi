using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    public class TestConditionDimension : AuditProperty
    {
        [Key]
        public long ID { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = null!;

        [StringLength(50)]
        public string? Unit { get; set; }

        [StringLength(20)]
        public string DataType { get; set; } = "Decimal";
    }
}
