using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    [Table("MachineDataLogs")]
    public class MachineDataLog : AuditProperty
    {
        [Key]
        public long ID { get; set; }
        public long? EquipmentId { get; set; }
        public long? TestResultHeaderId { get; set; }

        [Required]
        public string RawPayloadJson { get; set; } = string.Empty;

        public int MatchedCount { get; set; }
        public int UnmatchedCount { get; set; }
        public DateTime ReceivedAt { get; set; } = DateTime.UtcNow;

        [MaxLength(100)]
        public string? MachineSerialNo { get; set; }

        [MaxLength(50)]
        public string? Source { get; set; } // "API" | "FileImport" | "DirectConnect"
    }
}
