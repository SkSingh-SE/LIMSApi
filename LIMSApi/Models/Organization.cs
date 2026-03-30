using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    [Table("Organizations")]
    public class Organization : AuditProperty
    {
        [Key]
        public long Id { get; set; }

        [Required, StringLength(200)]
        public string LabName { get; set; } = null!;

        [Required, StringLength(50)]
        public string LabCode { get; set; } = null!;

        [Required, StringLength(500)]
        public string LabAddress { get; set; } = null!;

        [Required, StringLength(200)]
        public string ContactEmail { get; set; } = null!;

        [Required, StringLength(50)]
        public string ContactPhone { get; set; } = null!;

        [StringLength(500)]
        public string? OrganizationLogo { get; set; }

        [StringLength(100)]
        public string? CIN { get; set; }

        [StringLength(100)]
        public string? Website { get; set; }

        [StringLength(50)]
        public string? MobileNo { get; set; }
    }
}
