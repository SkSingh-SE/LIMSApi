using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    [Table("AuthorizedSignatories")]
    public class AuthorizedSignatory : AuditProperty
    {
        [Key]
        public long Id { get; set; }

        [Required, StringLength(200)]
        public string Name { get; set; } = null!;

        [Required, StringLength(200)]
        public string Designation { get; set; } = null!;

        [StringLength(500)]
        public string? SignaturePath { get; set; }

        public bool ApplicableFor { get; set; } = true;

        public long OrganizationId { get; set; }
    }
}
