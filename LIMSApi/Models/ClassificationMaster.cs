using System.ComponentModel.DataAnnotations;

namespace LIMSApi.Models
{
    public class ClassificationMaster : AuditProperty
    {
        [Key]
        public long ID { get; set; }

        [Required]
        [StringLength(50)]
        public string Code { get; set; } = string.Empty;

        [StringLength(100)]
        public required string Name { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        public int DisplayOrder { get; set; } = 0;
    }
}
