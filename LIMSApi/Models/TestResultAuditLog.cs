using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    [Table("TestResultAuditLogs")]
    public class TestResultAuditLog
    {
        [Key]
        public long ID { get; set; }

        [Required]
        public long TestResultHeaderID { get; set; }

        [Required]
        [MaxLength(100)]
        public string EntityName { get; set; } = string.Empty;

        public long EntityID { get; set; }

        [Required]
        [MaxLength(100)]
        public string FieldName { get; set; } = string.Empty;

        public string? OldValue { get; set; }

        public string? NewValue { get; set; }

        [MaxLength(500)]
        public string? ChangeReason { get; set; }

        public long ChangedBy { get; set; }

        public DateTime ChangedOn { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(TestResultHeaderID))]
        public virtual TestResultHeader? TestResultHeader { get; set; }
    }
}
