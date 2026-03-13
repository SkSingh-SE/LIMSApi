using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    [Table("NablApprovedSuppliers")]
    public class NablApprovedSupplier : NablFormBase
    {
        public long? SupplierId { get; set; } // FK to Supplier (not yet in codebase)

        [MaxLength(200)]
        public string? SupplierName { get; set; }

        [MaxLength(500)]
        public string? ItemsApproved { get; set; }

        public DateTime? ApprovalDate { get; set; }

        public DateTime? ApprovalValidUpto { get; set; }

        [MaxLength(100)]
        public string? ApprovalCategory { get; set; }

        [MaxLength(100)]
        public string? PerformanceRating { get; set; }

        public DateTime? LastReviewDate { get; set; }

        [MaxLength(200)]
        public string? ApprovedBy { get; set; }
    }
}
