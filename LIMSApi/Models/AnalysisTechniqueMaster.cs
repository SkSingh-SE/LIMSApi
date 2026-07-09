using System.ComponentModel.DataAnnotations;

namespace LIMSApi.Models
{
    /// <summary>
    /// Chemical analysis technique / instrument family used to perform chemical analysis
    /// (NOT a test method). e.g. OES, ICP, Wet Analysis, LECO, WDXRF, EDXRF.
    /// Sits in the chemical hierarchy: Chemical Test → Analysis Technique → Method → Instrument → Parameters.
    /// </summary>
    public class AnalysisTechniqueMaster : AuditProperty
    {
        [Key]
        public long ID { get; set; }

        /// Display name — e.g. "OES", "ICP", "Wet Analysis", "LECO", "WDXRF", "EDXRF".
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        /// Short unique code — e.g. OES / ICP / WET / LECO / WDXRF / EDXRF.
        [StringLength(40)]
        public string? Code { get; set; }

        /// Comma-separated alternate names — e.g. "Spectro Test, Metal Analysis, Spectrometer Test".
        [StringLength(500)]
        public string? AliasNames { get; set; }

        /// True for spectrometric techniques (OES / LECO / WDXRF / EDXRF) — drives spectro-style pricing.
        public bool IsSpectro { get; set; } = false;

        /// Optional notes on purpose / typical use.
        [StringLength(1000)]
        public string? Description { get; set; }

        public int SortOrder { get; set; } = 0;
    }
}
