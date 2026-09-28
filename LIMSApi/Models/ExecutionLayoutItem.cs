using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    /// <summary>
    /// PHASE 1G — Execution Layout Item (aggregate-owned by section).
    /// C3: (ReferenceType, ReferenceID) is a CONTROLLED APPLICATION-LEVEL
    /// reference — NOT a relational FK. ReferenceType determines the only legal
    /// target table; validation (exists + IsActive + same CompanyCode) is applied
    /// at create/update/activation. Cross-company references are always rejected.
    /// C5: IsEditable means "can the user edit the UI input value during
    /// execution" ONLY — never configuration editability (Phase 5).
    /// R2: IsRequired is presentation-only.
    /// </summary>
    public class ExecutionLayoutItem : AuditProperty
    {
        [Key]
        public long ID { get; set; }

        public long ExecutionLayoutSectionID { get; set; }

        /// <summary>ParameterMaster | ConditionMaster | EquipmentRequirementMaster | FactorConversionMaster | MeasurementUncertaintyMaster | AcceptanceCriteriaMaster | None</summary>
        [Required]
        [StringLength(30)]
        public string ReferenceType { get; set; } = "None";

        /// <summary>C3: controlled application-level reference (NO database FK).</summary>
        public long? ReferenceID { get; set; }

        [StringLength(150)]
        public string? DisplayLabel { get; set; }

        public int DisplayOrder { get; set; } = 0;

        public bool IsVisible { get; set; } = true;

        /// <summary>C5: execution UI input editability only — not configuration editability.</summary>
        public bool IsEditable { get; set; } = true;

        /// <summary>R2: presentation requirement only — never execution/business mandatory.</summary>
        public bool IsRequired { get; set; } = false;

        [ForeignKey(nameof(ExecutionLayoutSectionID))]
        public virtual ExecutionLayoutSection ExecutionLayoutSection { get; set; } = null!;
    }
}
