using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LIMSApi.Models
{
    [Table("LongTermRecords")]
    public class LongTermRecord : AuditProperty
    {
        [Key]
        public long ID { get; set; }

        [Required]
        public long LongTermTestID { get; set; }

        // Store intermediate measurement payload as JSON (flexible)
        [Column(TypeName = "nvarchar(max)")]
        public string DataJson { get; set; } = string.Empty;

        // When this intermediate data was recorded (could be UTC)
        public DateTime RecordedAt { get; set; } = DateTime.UtcNow;

        // Soft delete or active flag if not in AuditProperty
        public bool IsActive { get; set; } = true;

        // Navigation
        [ForeignKey(nameof(LongTermTestID))]
        public virtual LongTermTest? LongTermTest { get; set; }
    }
}
