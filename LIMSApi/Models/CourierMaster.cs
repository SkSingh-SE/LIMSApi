using System.ComponentModel.DataAnnotations;

namespace LIMSApi.Models
{
    public class CourierMaster : AuditProperty
    {
        [Key]
        public long ID { get; set; }
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string? ContactNo { get; set; }

    }
}
