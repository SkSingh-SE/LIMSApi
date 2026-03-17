using System.ComponentModel.DataAnnotations;

namespace LIMSApi.Models
{
    public class PriceDimensionType : AuditProperty
    {
        [Key]
        public long ID { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? Unit { get; set; }

        public bool IsRange { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        public int SortOrder { get; set; }
    }
}
