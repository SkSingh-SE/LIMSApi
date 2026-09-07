using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    /// <summary>
    /// Enterprise Scientific / Technical Master representing a universal condition or dimension
    /// applicable across laboratory disciplines (Material, Mechanical, Chemical, Soil, Civil, Electrical, Environmental).
    /// Defines reusable technical identity, data type, parameter unit, allowed operators, and discrete allowed values.
    /// Does NOT store requirement-specific limits, planning values, or execution observations.
    /// </summary>
    public class ConditionMaster : AuditProperty
    {
        [Key]
        public long ID { get; set; }

        /// <summary>Stable uppercase business identifier (e.g. TEST_TEMP, SPEC_THICKNESS, CURING_AGE, HUMIDITY).</summary>
        [Required]
        [StringLength(50)]
        public string Code { get; set; } = string.Empty;

        /// <summary>Display name (e.g. "Test Temperature", "Specimen Thickness", "Curing Age", "Relative Humidity").</summary>
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Discipline-neutral technical category metadata (e.g. "Thermal", "Dimensional", "Environmental", "Temporal", "Operational", "State").
        /// Metadata only; does NOT control runtime branching.
        /// </summary>
        [Required]
        [StringLength(50)]
        public string Category { get; set; } = "General";

        /// <summary>
        /// Value semantics: "Decimal" | "Integer" | "Text" | "Boolean" | "Date" | "DateTime" | "Selection"
        /// </summary>
        [Required]
        [StringLength(20)]
        public string ValueType { get; set; } = "Decimal";

        /// <summary>FK to ParameterUnitMaster if applicable (e.g. °C, mm, %, days).</summary>
        public long? ParameterUnitID { get; set; }

        /// <summary>JSON array of valid operators, e.g. ["=", "!=", ">", ">=", "<", "<=", "BETWEEN"].</summary>
        [Required]
        [StringLength(200)]
        public string AllowedOperators { get; set; } = "[\"=\"]";

        /// <summary>JSON array of allowed discrete values for Selection type, e.g. ["Option1", "Option2"]. Nullable.</summary>
        [StringLength(2000)]
        public string? AllowedValuesJson { get; set; }

        /// <summary>Optional default reference value (e.g. "23" for ambient room temperature).</summary>
        [StringLength(100)]
        public string? DefaultValue { get; set; }

        /// <summary>Technical description or standard notes.</summary>
        [StringLength(500)]
        public string? Description { get; set; }

        /// <summary>Display order sequence.</summary>
        public int DisplayOrder { get; set; } = 0;

        [ForeignKey("ParameterUnitID")]
        public virtual ParameterUnitMaster? ParameterUnit { get; set; }
    }
}
