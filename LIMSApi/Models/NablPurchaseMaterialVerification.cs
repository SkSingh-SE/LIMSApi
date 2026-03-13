using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    [Table("NablPurchaseMaterialVerifications")]
    public class NablPurchaseMaterialVerification : NablFormBase
    {
        public long? PurchaseOrderId { get; set; }

        [ForeignKey("PurchaseOrderId")]
        public virtual NablPurchaseOrder? PurchaseOrder { get; set; }

        [MaxLength(200)]
        public string? PONumber { get; set; }

        public DateTime? ReceivedDate { get; set; }

        public DateTime? VerificationDate { get; set; }

        [MaxLength(200)]
        public string? VerifiedBy { get; set; }

        public string? ItemsVerificationJson { get; set; } // JSON array of {itemName, expectedQty, receivedQty, condition, status}

        [MaxLength(50)]
        public string? OverallStatus { get; set; } // Accepted/Rejected/PartiallyAccepted

        [MaxLength(200)]
        public string? GRNNumber { get; set; }

        public string? Remarks { get; set; }
    }
}
