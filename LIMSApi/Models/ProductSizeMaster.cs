using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    /// <summary>
    /// Reusable named size band (e.g. "Up to 16mm Dia", "17-50mm Dia").
    /// FK source for specification size-wise limits (MS4) and product master (PM1).
    /// </summary>
    public class ProductSizeMaster : AuditProperty
    {
        [Key]
        public long ID { get; set; }

        /// Dimension this band measures: Diameter / Thickness / Length / Width.
        [Required]
        [StringLength(50)]
        public string SizeType { get; set; } = string.Empty;

        /// Lower bound of the band (inclusive), in Unit.
        [Column(TypeName = "decimal(18,4)")]
        public decimal? MinValue { get; set; }

        /// Upper bound of the band, in Unit.
        [Column(TypeName = "decimal(18,4)")]
        public decimal? MaxValue { get; set; }

        /// Unit of measure for Min/Max — FK to ParameterUnitMaster (mm / inch / cm …).
        public long? ParameterUnitID { get; set; }

        /// Human-readable label shown in dropdowns and on certificates.
        [Required]
        [StringLength(100)]
        public string DisplayName { get; set; } = string.Empty;

        [ForeignKey(nameof(ParameterUnitID))]
        public virtual ParameterUnitMaster? ParameterUnit { get; set; }
    }
}
