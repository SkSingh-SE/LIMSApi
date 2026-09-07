using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    [Table("UserBranches")]
    public class UserBranch : AuditProperty
    {
        [Key]
        public long ID { get; set; }

        public long UserID { get; set; }
        [ForeignKey("UserID")]
        public virtual UserMaster? User { get; set; }

        public long BranchID { get; set; }
        [ForeignKey("BranchID")]
        public virtual Branch? Branch { get; set; }

        public bool IsDefault { get; set; } = false;

        // Granular Permission Flags
        public bool CanView { get; set; } = true;
        public bool CanCreate { get; set; } = true;
        public bool CanEdit { get; set; } = true;
        public bool CanExecute { get; set; } = true;
        public bool CanApprove { get; set; } = false;
        public bool CanDelete { get; set; } = false;
    }
}
