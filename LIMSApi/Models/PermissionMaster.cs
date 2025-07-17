using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Org.BouncyCastle.Bcpg.OpenPgp;

namespace LIMSApi.Models
{
    public class PermissionMaster
    {
        [Key]
        public long ID { get; set; }
        [Required, StringLength(100)]
        public string Name { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public long? MenuID { get; set; }
        public string Type { get; set; } = string.Empty; // "Action", "Read" "View", etc.
        public string? Description { get; set; }

        [ForeignKey("MenuID")]
        public virtual MenuMaster? Menu { get; set; }
    }
}
