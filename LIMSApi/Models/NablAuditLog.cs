using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    [Table("NablAuditLogs")]
    public class NablAuditLog
    {
        [Key]
        public long ID { get; set; }

        [Required, MaxLength(50)]
        public string FormType { get; set; } = string.Empty;

        public long FormDataId { get; set; }

        [Required, MaxLength(50)]
        public string Action { get; set; } = string.Empty; // Created, Updated, Submitted, Reviewed, Approved, Rejected, Deleted

        public string? OldValues { get; set; } // JSON of changed fields (before)

        public string? NewValues { get; set; } // JSON of changed fields (after)

        public long PerformedBy { get; set; }

        [MaxLength(200)]
        public string? PerformedByName { get; set; }

        public DateTime PerformedOn { get; set; } = DateTime.UtcNow;

        [MaxLength(100)]
        public string? IpAddress { get; set; }

        [MaxLength(20)]
        public string CompanyCode { get; set; } = "LIMS";
    }
}
