using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    [Table("ConfigurationAdjustmentItems")]
    public class ConfigurationAdjustmentItem : AuditProperty
    {
        [Key]
        public long ID { get; set; }

        public long ConfigurationAdjustmentID { get; set; }

        public long UniversalTestGroupID { get; set; }

        [Required]
        [StringLength(50)]
        public string Section { get; set; } = string.Empty; // Parameters, Conditions, Equipment, Factors, Uncertainty, AcceptanceCriteria, Layout

        [Required]
        [StringLength(50)]
        public string EntityType { get; set; } = string.Empty; // Parameter, Requirement, Condition, Equipment, Factor, Uncertainty, AcceptanceCriteria, Layout

        public long? EntityID { get; set; }

        [StringLength(100)]
        public string? EntityCode { get; set; }

        [StringLength(250)]
        public string? EntityName { get; set; }

        [Required]
        [StringLength(100)]
        public string FieldName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string ChangeType { get; set; } = string.Empty; // ADD, REMOVE, CHANGE, REPLACE, OVERRIDE

        public string? PlannedValue { get; set; }

        public string? EffectiveValue { get; set; }

        public string? PreviousAdjustedValue { get; set; }

        public string? NewAdjustedValue { get; set; }

        [Required]
        [StringLength(500)]
        public string Reason { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string AuthorizationStatus { get; set; } = "Pending"; // Pending, Approved, Rejected

        public long? AuthorizedBy { get; set; }
        public DateTime? AuthorizedOn { get; set; }

        public long BranchID { get; set; }

        [ForeignKey("ConfigurationAdjustmentID")]
        public virtual ConfigurationAdjustment ConfigurationAdjustment { get; set; } = null!;
    }
}
