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

    /// <summary>Universal code for formula evaluation (e.g. "SOIL_LL")</summary>
    [StringLength(100)]
    public string? Code { get; set; }

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

    /// <summary>FK to ParameterUnitEquivalent. Set when a specific equivalent unit is selected.</summary>
    public long? ParameterUnitEquivalentID { get; set; }

    /// <summary>Conversion factor relative to base unit at the time of selection/save.</summary>
    [Column(TypeName = "decimal(18, 6)")]
    public decimal? UnitConversionFactor { get; set; }

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

    /// <summary>Calculation role: "Input" | "Calculated" | "Derived"</summary>
    [StringLength(50)]
    public string? CalculationRole { get; set; } = "Input";

    /// <summary>Master display order / sequence hint.</summary>
    public int? Sequence { get; set; }

    public string? Note { get; set; }

    /// <summary>Alias for Note / scientific description.</summary>
    [NotMapped]
    public string? Description
    {
        get => Note;
        set => Note = value;
    }

    /// <summary>Billing tier. "normal" | "special" | "super". No type change.</summary>
    public string? ElementType { get; set; } = "normal";

    // ─── Navigation ───────────────────────────────────
    [ForeignKey("ParameterUnitID")]
    public virtual ParameterUnitMaster? ParameterUnit { get; set; }

    [ForeignKey("ParameterUnitEquivalentID")]
    public virtual ParameterUnitEquivalent? ParameterUnitEquivalent { get; set; }

    /// <summary>Options for Dropdown / MultiSelect InputType.</summary>
    public virtual ICollection<ParameterDropdownOption> DropdownOptions { get; set; }
        = new List<ParameterDropdownOption>();
}
