using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    [Table("NablCrmConsumptions")]
    public class NablCrmConsumption : NablFormBase
    {
        public long? ReferenceMaterialId { get; set; }

        [ForeignKey("ReferenceMaterialId")]
        public virtual NablReferenceMaterial? ReferenceMaterial { get; set; }

        [MaxLength(100)]
        public string? RMCode { get; set; }

        [MaxLength(200)]
        public string? RMName { get; set; }

        public DateTime? UsageDate { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal? QuantityUsed { get; set; }

        [MaxLength(50)]
        public string? QuantityUnit { get; set; }

        [MaxLength(500)]
        public string? PurposeOfUse { get; set; }

        [MaxLength(200)]
        public string? UsedBy { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal? RemainingAfterUse { get; set; }

        public bool IsExhausted { get; set; }
    }
}
