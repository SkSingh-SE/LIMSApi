using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    /// <summary>
    /// Simple machining charge line item per sample.
    /// No master price lookup — free-text description + manual amount.
    /// Total machining = SUM(Amount) of all active items for a sample.
    /// </summary>
    public class MachiningChargeItem : AuditProperty
    {
        [Key]
        public long ID { get; set; }

        public long SampleID { get; set; }

        [Required, MaxLength(200)]
        public string Description { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [MaxLength(500)]
        public string? Remark { get; set; }

        [ForeignKey(nameof(SampleID))]
        public virtual SampleDetail? Sample { get; set; }
    }
}
