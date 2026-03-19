using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    public class PurchaseOrderItem : AuditProperty
    {
        [Key]
        public long ID { get; set; }

        [Required]
        public long CustomerPurchaseOrderID { get; set; }

        [Required, MaxLength(300)]
        public string Description { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? ItemCode { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Quantity { get; set; }

        [MaxLength(50)]
        public string? Unit { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal BilledAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal RemainingAmount { get; set; }

        [MaxLength(500)]
        public string? Remark { get; set; }

        [ForeignKey("CustomerPurchaseOrderID")]
        public virtual CustomerPurchaseOrder? CustomerPurchaseOrder { get; set; }
    }
}
