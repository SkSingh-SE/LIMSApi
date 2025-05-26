using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    public class SubGroupMaster : AuditProperty
    {
        [Key]
        public long ID { get; set; }
        [Required, StringLength(100)]
        public string Name { get; set; } = string.Empty;
        [StringLength(250)]
        public string? Description { get; set; }

        public long GroupID { get; set; }
        [ForeignKey("GroupID")]
        public virtual GroupMaster? Group { get; set; }
    }
}
