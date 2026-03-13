using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    [Table("NablIncomingMaterials")]
    public class NablIncomingMaterial : NablFormBase
    {
        public long? SupplierId { get; set; }

        [ForeignKey("SupplierId")]
        public virtual SupplierMaster? Supplier { get; set; }

        [MaxLength(200)]
        public string? SupplierName { get; set; }

        [MaxLength(200)]
        public string? PurchaseOrderNo { get; set; }

        public DateTime? ReceivedDate { get; set; }

        [MaxLength(200)]
        public string? ReceivedBy { get; set; }

        [MaxLength(500)]
        public string? ItemDescription { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal? Quantity { get; set; }

        [MaxLength(50)]
        public string? Unit { get; set; }

        [MaxLength(200)]
        public string? BatchNo { get; set; }

        public DateTime? InspectionDate { get; set; }

        [MaxLength(200)]
        public string? InspectionBy { get; set; }

        [MaxLength(50)]
        public string? InspectionResult { get; set; } // Accepted/Rejected/ConditionalAcceptance

        public string? Remarks { get; set; }

        [MaxLength(500)]
        public string? StorageLocation { get; set; }
    }
}
