using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    public class UserPermission : AuditProperty
    {
        [Key]
        public long ID { get; set; }
        public long UserID { get; set; }
        public long PermissionID { get; set; }
        public bool IsGranted { get; set; } = false;
        public bool IsOverride { get; set; } = false;


        [ForeignKey("UserID")]
        public virtual UserMaster? User { get; set; }
        [ForeignKey("PermissionID")]
        public virtual PermissionMaster? Permission { get; set; }
    }
}
