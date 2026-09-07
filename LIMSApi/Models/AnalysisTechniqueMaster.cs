using System.ComponentModel.DataAnnotations;

namespace LIMSApi.Models
{
    /// <summary>
    /// Enterprise Scientific Master representing the scientific/analytical approach used by a laboratory test.
    /// Reusable across Soil, Mechanical, Chemical, Electrical, Civil, Environmental, Metrology, etc.
    /// Not branch-owned, not department-owned, not discipline-owned.
    /// </summary>
    public class AnalysisTechniqueMaster : AuditProperty
    {
        [Key]
        public long ID { get; set; }

        /// <summary>Stable uppercase business identifier (e.g. OES, ICP, WET, GRAV_METRIC).</summary>
        [Required]
        [StringLength(50)]
        public string Code { get; set; } = string.Empty;

        /// <summary>Display name — e.g. "OES", "ICP", "Wet Analysis", "Gravimetric Analysis".</summary>
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        /// <summary>Comma-separated alternate names — e.g. "Spectro Test, Metal Analysis".</summary>
        [StringLength(500)]
        public string? AliasNames { get; set; }

        /// <summary>Optional notes on purpose / typical scientific use.</summary>
        [StringLength(1000)]
        public string? Description { get; set; }
    }
}
