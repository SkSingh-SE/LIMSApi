using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    /// <summary>
    /// PHASE 1G — Execution Layout Section (aggregate-owned by header).
    /// Ordered presentation section of the execution workspace.
    /// R2: IsRequired is a PRESENTATION requirement only — it never drives
    /// execution/business mandatory semantics (LaboratoryTestParameter.IsMandatory
    /// remains the sole execution-mandatory authority).
    /// </summary>
    public class ExecutionLayoutSection : AuditProperty
    {
        [Key]
        public long ID { get; set; }

        public long ExecutionLayoutID { get; set; }

        [Required]
        [StringLength(100)]
        public string SectionCode { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string SectionName { get; set; } = string.Empty;

        /// <summary>Preparation | Conditions | Equipment | Parameters | Factors | MeasurementUncertainty | AcceptanceCriteria | Attachments | Remarks</summary>
        [Required]
        [StringLength(100)]
        public string SectionType { get; set; } = string.Empty;

        public int DisplayOrder { get; set; } = 0;

        public bool IsVisible { get; set; } = true;

        public bool IsCollapsible { get; set; } = false;

        /// <summary>R2: presentation requirement only — never execution/business mandatory.</summary>
        public bool IsRequired { get; set; } = false;

        [ForeignKey(nameof(ExecutionLayoutID))]
        public virtual ExecutionLayoutMaster ExecutionLayout { get; set; } = null!;

        public virtual ICollection<ExecutionLayoutItem> Items { get; set; } = new List<ExecutionLayoutItem>();
    }
}
