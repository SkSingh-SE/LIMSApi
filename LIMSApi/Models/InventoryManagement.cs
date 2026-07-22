using System.ComponentModel.DataAnnotations;

namespace LIMSApi.Models
{
    public partial class InventoryManagement: AuditProperty
    {
        [Key]
        public long ID { get; set; }
        public string ItemCode { get; set; }
        public string ItemCategory { get; set; }
        public string ItemName{ get; set; }
        public string? ItemDescription { get; set; }
        public long? DepartmentID { get; set; }
        public long? SupplierId { get; set; }
        public string? Manufacturer { get; set; }
        public string? BatchNo { get; set; }
        public string? Unit { get; set; }
        public decimal Quantity { get; set; }
        public decimal MinimumQuantity { get; set; }
        public string? StorageLocation { get; set; }
        public DateTime Date{ get; set; }
        public string? Remarks { get; set; }
        public string? SupplierName { get; set; }

    }
}
