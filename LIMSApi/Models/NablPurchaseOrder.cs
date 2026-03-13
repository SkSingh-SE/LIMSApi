using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    [Table("NablPurchaseOrders")]
    public class NablPurchaseOrder : NablFormBase
    {
        public long? SupplierId { get; set; }

        [ForeignKey("SupplierId")]
        public virtual SupplierMaster? Supplier { get; set; }

        [MaxLength(200)]
        public string? SupplierName { get; set; }

        public long? PurchaseIndentId { get; set; }

        [ForeignKey("PurchaseIndentId")]
        public virtual NablPurchaseIndent? PurchaseIndent { get; set; }

        public DateTime? PODate { get; set; }

        public DateTime? DeliveryDate { get; set; }

        [MaxLength(500)]
        public string? PaymentTerms { get; set; }

        public string? ItemsJson { get; set; } // JSON array of {itemName, quantity, unit, unitPrice, total}

        [Column(TypeName = "decimal(18,2)")]
        public decimal? TotalAmount { get; set; }

        [MaxLength(10)]
        public string? Currency { get; set; }

        public string? SpecialInstructions { get; set; }

        [MaxLength(200)]
        public string? IssuedBy { get; set; }
    }
}
