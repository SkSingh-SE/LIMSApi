using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    public class ExecutionConfigSnapshot : AuditProperty
    {
        [Key]
        public long ID { get; set; }

        [Required]
        public string ConfigJson { get; set; } = null!;

        [StringLength(256)]
        public string? SnapshotHash { get; set; }
    }
}
