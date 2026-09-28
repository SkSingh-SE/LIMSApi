using System.ComponentModel.DataAnnotations;

namespace LIMSApi.Models
{
    /// <summary>
    /// PHASE 1G — Execution Layout Master (header).
    /// Defines how configured test information is organized and presented to the
    /// analyst during execution. PRESENTATION CONFIGURATION ONLY.
    /// Phase boundary: no UTD association (Phase 2), no Effective Configuration
    /// consumption (Phase 4), no snapshot integration (Phase 6).
    /// R1: RendererType is nullable at database level, has no database default, and is
    /// presented as a controlled dropdown in the UI. Dropdown values come from the verified
    /// renderer catalog (ObservationMatrix, MultiSpecimen, MultiReading, ParameterTable,
    /// Qualitative, Calculation, Graph). Phase 1G does not implement renderer behavior.
    /// R3: no usage counting — there is no authoritative consumer yet.
    /// </summary>
    public class ExecutionLayoutMaster : AuditProperty
    {
        [Key]
        public long ID { get; set; }

        [Required]
        [StringLength(50)]
        public string Code { get; set; } = string.Empty;

        [Required]
        [StringLength(150)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        /// <summary>R1: nullable at DB level, no DB default; controlled UI dropdown from verified catalog; no renderer behavior in Phase 1G.</summary>
        [StringLength(30)]
        public string? RendererType { get; set; }

        public int DisplayOrder { get; set; } = 0;

        public virtual ICollection<ExecutionLayoutSection> Sections { get; set; } = new List<ExecutionLayoutSection>();
    }
}
