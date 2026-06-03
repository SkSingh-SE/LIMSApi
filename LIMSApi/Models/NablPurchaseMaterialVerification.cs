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
        public string? SupplierName { get; set; }
        public string? InvoiceNo { get; set; }
        public string? InvoiceDate { get; set; }
        public string? PhoneNo { get; set; }
        public string? Email { get; set; }
        public string? GstNo { get; set; }
        public string? Address { get; set; }
        public string? OrderType { get; set; }
        public string? PODate { get; set; }
        public string? CorrectiveActions { get; set; }
        public string? Deviations { get; set; }
        public string? PurchaseOrderNo { get; set; }
        public string? PoNo { get; set; }
        public string? InspectionBy { get; set; }
        [NotMapped]
        public List<DescriptionParameters>? ItemsParameters{ get; set; }

    }
    [NotMapped]
    public class DescriptionParameters
    {
        public string? MaterialName { get; set; }
        public int? OrderedQty { get; set; }
        public int? ReceviceQty { get; set; }
        public int? ApprovedQty { get; set; }
        public int? RejectedQty { get; set; }
        public string? VerificationDetails { get; set; }
        public string? InspectionQtyStatus { get; set; }
        public string? VerificationDone { get; set; }
    }
}
