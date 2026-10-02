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
        [MaxLength(200)]
        public string ContactPerson { get; set; }
        public string MobileNo { get; set; }
        public string Email { get; set; }
        public string ServiceProviderName { get; set; }
        public int? LastScore { get; set; }
        public bool? IsPresentStatus { get; set; }
        public DateTime? EnlistmentDate { get; set; }
        public bool? ProductApproved { get; set; }
        public string? Remarks { get; set; }
        public string? BlacklistReason { get; set; }
        public DateTime? AgreementDate { get; set; }
        public DateTime? BlacklistDate { get; set; }
        public bool? IsBlacklisted { get; set; }
        public long? SupplierRegisterId { get; set; }
        [ForeignKey("SupplierRegisterId")]
        public NablSupplierRegistration? SupplierRegistration { get; set; }
        public string? RegisterNo { get; set; }
        [MaxLength(20)]
        public string? GstNo { get; set; }
        [MaxLength(500)]
        public string? Address { get; set; }

    }
}
