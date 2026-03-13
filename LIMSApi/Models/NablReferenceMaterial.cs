using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    [Table("NablReferenceMaterials")]
    public class NablReferenceMaterial : NablFormBase
    {
        [MaxLength(100)]
        public string? RMCode { get; set; }

        [MaxLength(200)]
        public string? RMName { get; set; }

        [MaxLength(200)]
        public string? Manufacturer { get; set; }

        [MaxLength(100)]
        public string? BatchNo { get; set; }

        [MaxLength(100)]
        public string? CertificateNo { get; set; }

        public DateTime? ReceivedDate { get; set; }

        public DateTime? ExpiryDate { get; set; }

        [MaxLength(200)]
        public string? StorageCondition { get; set; }

        [Column(TypeName = "decimal(18,6)")]
        public decimal? CertifiedValue { get; set; }

        [Column(TypeName = "decimal(18,6)")]
        public decimal? Uncertainty { get; set; }

        [MaxLength(50)]
        public string? Unit { get; set; }

        [MaxLength(500)]
        public string? Purpose { get; set; }

        public long? SupplierId { get; set; } // FK to Supplier (not yet in codebase)

        [Column(TypeName = "decimal(18,4)")]
        public decimal? RemainingQuantity { get; set; }

        [MaxLength(50)]
        public string? QuantityUnit { get; set; }
    }
}
