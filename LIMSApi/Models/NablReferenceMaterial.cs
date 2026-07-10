using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    [Table("NablReferenceMaterials")]
    public class NablReferenceMaterial : NablFormBase
    {
        [MaxLength(100)]
        public string? RMCode { get; set; }

        [MaxLength(200)]
        public string? RMName { get; set; }

        [MaxLength(200)]
        public string? Manufacturer { get; set; }

        [MaxLength(100)]
        public string? BatchNo { get; set; }

        [MaxLength(100)]
        public string? CertificateNo { get; set; }

        public DateTime? ReceivedDate { get; set; }

        public DateTime? ExpiryDate { get; set; }

        [MaxLength(200)]
        public string? StorageCondition { get; set; }

        [Column(TypeName = "decimal(18,6)")]
        public decimal? CertifiedValue { get; set; }

        [Column(TypeName = "decimal(18,6)")]
        public decimal? Uncertainty { get; set; }

        [MaxLength(50)]
        public string? Unit { get; set; }

        [MaxLength(500)]
        public string? Purpose { get; set; }

        public long? SupplierId { get; set; } // FK to Supplier (not yet in codebase)

        [Column(TypeName = "decimal(18,4)")]
        public decimal? RemainingQuantity { get; set; }

        [MaxLength(50)]
        public string? QuantityUnit { get; set; }
        public string? MaterialDescription { get; set; }
        public string? Type { get; set; }
        public string? Supplier { get; set; }
        public string? MatrixType { get; set; }
        public string? StorageLocation { get; set; }
        public string? Traceability { get; set; }
        public DateTime? CertificationDate { get; set; }
        public DateTime? ValidityDate { get; set; }
        public decimal InitialQuantity { get; set; }
        public decimal AvailableQuantity { get; set; }
        public decimal MinimumQuantity { get; set; }
        public string? UnitOfMeasure { get; set; }
        public string? Specifications { get; set; }
        public string? ItemId { get; set; }
        public long? DepartmentID { get; set; }
        public long? InventoryId { get; set; }
        public string? ParameterJson { get; set; }
        public string? ItemCode { get; set; }
        public string? ItemName{ get; set; }
        [NotMapped]
        public List<Parameters> Parameters{ get; set; }
    }
    [NotMapped]
    public class Parameters
    {
        public string ParameterName { get; set; }
        public decimal CertifiedValue { get; set; }
        public decimal UpperLimit { get; set; }
        public decimal LowerLimit { get; set; }
        public string Unit { get; set; }
        public decimal MeasurementUncertainty { get; set; }
        public string? Remarks { get; set; }

    }
}
