using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LIMSApi.Models
{
    [Table("LongTermTests")]
    public class LongTermTest : AuditProperty
    {
        [Key]
        public long ID { get; set; }

        // Link to the test header (the test that was moved to long-term)
        [Required]
        public long TestResultHeaderID { get; set; }

        // Link to sample for quick querying
        [Required]
        public long SampleID { get; set; }

        // Duration in hours (or change to TimeSpan if you prefer)
        public int DurationHours { get; set; }

        // When the long-term test started
        public DateTime? StartedAt { get; set; }

        // When it ended / was completed
        public DateTime? EndedAt { get; set; }

        // Running / Completed / Cancelled etc.
        [MaxLength(50)]
        public string Status { get; set; } = "Running";

        // Optional free-text notes
        [MaxLength(1000)]
        public string? Notes { get; set; }

        // Navigation properties
        [ForeignKey(nameof(TestResultHeaderID))]
        public virtual TestResultHeader? TestResultHeader { get; set; }

        [ForeignKey(nameof(SampleID))]
        public virtual SampleDetail? Sample { get; set; }

        // Child records (intermediate measurements)
        public ICollection<LongTermRecord> Records { get; set; } = new List<LongTermRecord>();
    }
}
