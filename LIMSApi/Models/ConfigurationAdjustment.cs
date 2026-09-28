using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    [Table("ConfigurationAdjustments")]
    public class ConfigurationAdjustment : AuditProperty
    {
        [Key]
        public long ID { get; set; }

        public long UniversalTestGroupID { get; set; }

        public int AdjustmentNumber { get; set; } = 1;

        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "Draft"; // Draft, Applied, Approved

        [Required]
        [StringLength(500)]
        public string OverallReason { get; set; } = string.Empty;

        public long? AppliedBy { get; set; }
        public DateTime? AppliedOn { get; set; }

        public long? ApprovedBy { get; set; }
        public DateTime? ApprovedOn { get; set; }

        [StringLength(500)]
        public string? ApprovalRemarks { get; set; }

        public long BranchID { get; set; }

        [Required]
        [StringLength(64)]
        public string ConcurrencyToken { get; set; } = Guid.NewGuid().ToString("N");

        /// <summary>
        /// Immutable, applied adjustment result for Phase 6 handoff.
        /// Derived from Phase 4 Effective + Applied Adjustments via BuildAdjustedConfiguration().
        /// Never overwrites PlannedConfigurationJson or Phase 4 logic.
        /// </summary>
        public string? AdjustedConfigurationJson { get; set; }

        [ForeignKey("UniversalTestGroupID")]
        public virtual UniversalTestGroup UniversalTestGroup { get; set; } = null!;

        [ForeignKey("BranchID")]
        public virtual Branch Branch { get; set; } = null!;

        public virtual ICollection<ConfigurationAdjustmentItem> Items { get; set; } = new List<ConfigurationAdjustmentItem>();
    }
}
