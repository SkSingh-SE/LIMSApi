using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    [Table("Branches")]
    public class Branch : AuditProperty
    {
        [Key]
        public long ID { get; set; }

        public long OrganizationID { get; set; }
        [ForeignKey("OrganizationID")]
        public virtual Organization? Organization { get; set; }

        [Required, StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required, StringLength(50)]
        public string Code { get; set; } = string.Empty;

        public bool IsHeadOffice { get; set; } = false;

        [StringLength(500)]
        public string? Address { get; set; }

        [StringLength(200)]
        public string? ContactEmail { get; set; }

        [StringLength(50)]
        public string? ContactPhone { get; set; }

        public virtual ICollection<UserBranch> UserBranches { get; set; } = new List<UserBranch>();
    }
}
