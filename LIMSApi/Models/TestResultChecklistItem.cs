using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    [Table("TestResultChecklistItems")]
    public class TestResultChecklistItem : AuditProperty
    {
        [Key]
        public long ID { get; set; }

        [Required]
        public long TestResultHeaderID { get; set; }

        [Required]
        [MaxLength(50)]
        public string ItemCode { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string ItemDescription { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = "NA";

        [MaxLength(500)]
        public string? Remarks { get; set; }

        public long? CheckedBy { get; set; }

        public DateTime? CheckedAt { get; set; }

        public bool IsReportable { get; set; } = false;

        public bool IsVoided { get; set; } = false;

        [Timestamp]
        public byte[]? RowVersion { get; set; }

        [ForeignKey(nameof(TestResultHeaderID))]
        public virtual TestResultHeader? TestResultHeader { get; set; }
    }
}
