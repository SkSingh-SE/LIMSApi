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
        public string? GSTNo { get; set; }
        public string? SupplierAddress { get; set; }
        public string? OrderType { get; set; }
        public string? TearmCondition { get; set; }
        [NotMapped]
        public List<Items>? Items { get; set; }
        public int? GstPercentage { get; set; }
        public decimal? GstAmount { get; set; }
        public decimal? GrandTotal { get; set; }
        public string? ReferenceIndentNo { get; set; }
        public string? PONo { get; set; }
        public string? Email { get; set; }
        public string? PhoneNo { get; set; }
        public string? AuthorizedBy { get; set; }
        public int? RequestedQuantity { get; set; }
        public long? ApprovedSupplierId { get; set; }
        [ForeignKey("ApprovedSupplierId")]
        public NablApprovedSupplier? NablApprovedSupplier { get; set; }
        public string? ReferenceIndentName { get; set; }
    }
    [NotMapped]
    public class Items
    {
        public string? Description { get; set; }
        public int? Quantity { get; set; }
        public string? UnitOfMeasure { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? TotalAmount { get; set; }
    }
}
