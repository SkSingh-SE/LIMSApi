using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    [Table("CustomerPurchaseOrders")]
    public class CustomerPurchaseOrder : AuditProperty
    {
        [Key]
        public long ID { get; set; }

        public long CustomerId { get; set; }

        [Required, MaxLength(100)]
        public string PONumber { get; set; } = string.Empty;

        public DateTime PODate { get; set; }
        public DateTime? ValidUntil { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal POAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal UtilizedAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal RemainingAmount { get; set; }

        [MaxLength(50)]
        public string Status { get; set; } = "Active"; // Active, Exhausted, Expired, Cancelled

        [MaxLength(500)]
        public string? Terms { get; set; }

        [MaxLength(500)]
        public string? FilePath { get; set; }

        [MaxLength(200)]
        public string? FileName { get; set; }

        public long? UploadReferenceID { get; set; }

        // Navigation properties
        [ForeignKey(nameof(CustomerId))]
        public virtual Customer? Customer { get; set; }

        public virtual ICollection<TaxInvoice> Invoices { get; set; } = new List<TaxInvoice>();
    }
}
