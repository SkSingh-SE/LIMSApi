using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    [Table("NumberingConfigs")]
    public class NumberingConfig : AuditProperty
    {
        [Key]
        public long Id { get; set; }

        [Required, StringLength(100)]
        public string ModuleName { get; set; } = null!;

        [Required, StringLength(100)]
        public string Prefix { get; set; } = null!;

        public int StartNumber { get; set; }
        public int CurrentNumber { get; set; }
        public long OrganizationId { get; set; }
        [ForeignKey("OrganizationId")]
        public virtual Organization? Organization { get; set; }

        public long? BranchId { get; set; }
        [ForeignKey("BranchId")]
        public virtual Branch? Branch { get; set; }
    }
}
