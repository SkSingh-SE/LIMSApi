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
        public string? MaterialName { get; set; }
        public string? MaterialCode { get; set; }
        public string? LotNo { get; set; }
        public string? InvoiceNo { get; set; }
        public string? GrnNo { get; set; }
        [NotMapped]
        public List<InspectionParameters>? InspectionParameters { get; set; }
        [NotMapped]
        public List<ItemsParameters>? ItemsParameters { get; set; }
        public string? InspectionParameterJson { get; set; }
        public string? Deviations { get; set; }
        public string? CorrectiveActions { get; set; }
        public string? RiskLevel { get; set; }
        public string? InspectionStage { get; set; }
        public string? ProductName { get; set; }
        public string? Category { get; set; }
        public string? ProductCode { get; set; }
        public string? InspectionPlanNo { get; set; }
        public string? ItemsParametersJson { get; set; }
        public string? PoNo { get; set; }
        public string? IndentNoPoNo { get; set; }
        public string? Email { get; set; }
        public string? PhoneNo { get; set; }
        public string? Address { get; set; }
        public string? GstNo { get; set; } 
        public string? GeneralRemarks { get; set; }
        public string? OrderType { get; set; }
        public string? InspectionPlanNoName { get; set; }
    }
    [NotMapped]
    public class InspectionParameters
    {
        public string? ParameterName { get; set; }
        public string? Requirement { get; set; }
        public string? ReferenceStandard { get; set; }
        public string? MethodOfCheck { get; set; }
        public string? Frequency { get; set; }
        public string? AcceptanceCriteria { get; set; }
        public string? Result { get; set; }
    }

    [NotMapped]
    public class ItemsParameters
    {
        public string? MaterialName { get; set; }
        public string? MaterialCode { get; set; }
        public decimal? OrderedQty { get; set; }
        public decimal? Unit { get; set; }
        public decimal? ReceviceQty { get; set; }
        public string? BatchNo { get; set; }
        public string? LotNo { get; set; }
        public string? InvoiceNo { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string? CoaAvailable { get; set; }
        public string? Result { get; set; }

    }
}
