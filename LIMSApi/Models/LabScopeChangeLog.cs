using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    /// <summary>
    /// Tracks scope changes for NABL audit compliance (ISO 17025 Clause 7.1.1).
    /// Records when parameters are added, removed, or limits changed.
    /// </summary>
    [Table("LabScopeChangeLogs")]
    public class LabScopeChangeLog
    {
        [Key]
        public long ID { get; set; }

        public long LabScopeID { get; set; }

        [Required, MaxLength(50)]
        public string ChangeType { get; set; } = string.Empty; // Created, ParameterAdded, ParameterRemoved, LimitsChanged, SpecificationAdded, SpecificationRemoved

        [MaxLength(200)]
        public string? EntityName { get; set; } // e.g., "UTS", "IS 1608"

        [MaxLength(500)]
        public string? OldValue { get; set; }

        [MaxLength(500)]
        public string? NewValue { get; set; }

        [MaxLength(500)]
        public string? Remarks { get; set; }

        public long ChangedBy { get; set; }
        public DateTime ChangedOn { get; set; } = DateTime.UtcNow;
    }
}
