using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    [Table("UniversalResultAudits")]
    public class UniversalResultAudit : AuditProperty
    {
        [Key]
        public long ID { get; set; }

        public long UniversalTestResultID { get; set; }

        public long TestExecutionID { get; set; }

        public int RevisionNo { get; set; } = 1;

        [Required]
        [StringLength(50)]
        public string EventType { get; set; } = string.Empty;

        public long ActorID { get; set; }

        [StringLength(200)]
        public string? ActorName { get; set; }

        public DateTime EventOn { get; set; } = DateTime.UtcNow;

        [StringLength(256)]
        public string? SnapshotHash { get; set; }

        public string? DetailsJson { get; set; }

        [ForeignKey("UniversalTestResultID")]
        public virtual UniversalTestResult? UniversalTestResult { get; set; }
    }
}
