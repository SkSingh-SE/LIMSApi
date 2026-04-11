using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    /// <summary>
    /// Junction table mapping a Role → Permission.
    /// Permissions granted via this table are inherited by ALL users with that role.
    /// Per-user overrides are stored in UserPermission (additive or revoking).
    ///
    /// Resolution order (enforced in RequirePermissionAttribute):
    ///   1. Admin role → bypass all checks
    ///   2. UserPermission override (user-specific grant/deny)
    ///   3. RolePermission default (from user's role)
    ///   4. Deny
    /// </summary>
    public class RolePermission : AuditProperty
    {
        [Key]
        public long ID { get; set; }

        public long RoleID { get; set; }
        public long PermissionID { get; set; }
        public bool IsGranted { get; set; } = true;

        [ForeignKey("RoleID")]
        public virtual RoleMaster? Role { get; set; }

        [ForeignKey("PermissionID")]
        public virtual PermissionMaster? Permission { get; set; }
    }
}
