using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    [Table("GstConfigs")]
    public class GstConfig : AuditProperty
    {
        [Key]
        public long Id { get; set; }

        [Required, StringLength(50)]
        public string GstNumber { get; set; } = null!;

        [Required, StringLength(50)]
        public string State { get; set; } = null!;

        [StringLength(500)]
        public string? Address { get; set; }
        public long OrganizationId { get; set; }
    }
}
