using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    /// <summary>
    /// Immutable audit log for every sample and inward status change.
    /// Required for NABL ISO 17025 traceability — answers:
    ///   "Who changed the status, when, from what, to what?"
    ///
    /// Records are NEVER updated or deleted.
    /// </summary>
    [Table("SampleStatusHistories")]
    public class SampleStatusHistory
    {
        [Key]
        public long ID { get; set; }

        /// <summary>"Sample" or "Inward"</summary>
        [Required, StringLength(20)]
        public string EntityType { get; set; } = string.Empty;

        /// <summary>SampleDetail.ID or SampleInward.ID</summary>
        public long EntityID { get; set; }

        [StringLength(50)]
        public string? PreviousStatus { get; set; }

        [Required, StringLength(50)]
        public string NewStatus { get; set; } = string.Empty;

        public long ChangedBy { get; set; }

        [StringLength(100)]
        public string? ChangedByName { get; set; }

        public DateTime ChangedOn { get; set; } = DateTime.UtcNow;

        /// <summary>Which service/method triggered this change</summary>
        [StringLength(200)]
        public string? Source { get; set; }

        [StringLength(500)]
        public string? Remarks { get; set; }
    }
}
