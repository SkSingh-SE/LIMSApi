using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models;

public partial class ParameterMaster : AuditProperty
{
    public long ID { get; set; }

    [StringLength(200)]
    public required string Name { get; set; }

    /// <summary>Chemical symbol or short notation (e.g. "C", "Mn", "UTS")</summary>
    [StringLength(50)]
    public string? Symbol { get; set; }

    /// <summary>
    /// "Chemical" | "Mechanical" | "Observation"
    /// Kept as string for backward compatibility — no type change.
    /// </summary>
    public string? ParameterType { get; set; }

    /// <summary>
    /// Input type controlling UI behavior and value storage.
    /// "Decimal" | "Integer" | "Boolean" | "Dropdown" | "MultiSelect" | "Text"
    /// Default: "Decimal" (backward compatible with all existing Chemical/Mechanical params)
    /// </summary>
    [StringLength(20)]
    public string? InputType { get; set; } = "Decimal";

    /// <summary>FK to ParameterUnitMaster. Applicable only for Decimal / Integer InputType.</summary>
    public long? ParameterUnitID { get; set; }

    /// <summary>Number of decimal places. Applicable only for Decimal InputType.</summary>
    public int DecimalPrecision { get; set; } = 2;

    /// <summary>True when the parameter value is computed from a formula.</summary>
    public bool IsCalculated { get; set; } = false;

    /// <summary>
    /// Stored formula in {Px} token format: e.g. "{P12}+({P15}/6)".
    /// Applicable only when IsCalculated=true AND InputType is Decimal or Integer.
    /// </summary>
    public string? Formula { get; set; }

    /// <summary>
    /// Human-readable display of the formula: e.g. "C + Mn/6".
    /// Stored alongside Formula for display without re-resolving parameter names.
    /// </summary>
    public string? FormulaDisplay { get; set; }

    public string? Note { get; set; }

    /// <summary>Billing tier. "normal" | "special" | "super". No type change.</summary>
    public string? ElementType { get; set; } = "normal";

    // ─── Navigation ───────────────────────────────────
    [ForeignKey("ParameterUnitID")]
    public virtual ParameterUnitMaster? ParameterUnit { get; set; }

    /// <summary>Options for Dropdown / MultiSelect InputType.</summary>
    public virtual ICollection<ParameterDropdownOption> DropdownOptions { get; set; }
        = new List<ParameterDropdownOption>();
}
