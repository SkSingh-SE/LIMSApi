using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    [Table("NablFormRevisionHistory")]
    public class NablFormRevisionHistory : AuditProperty
    {
        [Key]
        public long ID { get; set; }

        public long FormDataId { get; set; }

        [Required, MaxLength(50)]
        public string FormType { get; set; } = string.Empty;

        [MaxLength(10)]
        public string? IssueNo { get; set; }

        [MaxLength(10)]
        public string? RevNo { get; set; }

        // Full JSON snapshot of previous version
        public string? SnapshotJson { get; set; }

        [MaxLength(500)]
        public string? ChangeReason { get; set; }

        public DateTime RevisionDate { get; set; } = DateTime.UtcNow;

        public long RevisedBy { get; set; }

        [MaxLength(200)]
        public string? RevisedByName { get; set; }
    }
}
