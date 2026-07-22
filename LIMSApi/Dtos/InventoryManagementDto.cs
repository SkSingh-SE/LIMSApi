namespace LIMSApi.Dtos
{
    public class InventoryManagementDto
    {
        public long InventoryId { get; set; }
        public string ItemCode { get; set; }
        public string ItemCategory { get; set; }
        public string ItemName { get; set; }
        public long? SupplierId { get; set; }
        public long? DepartmentID { get; set; }
        public string? Manufacturer { get; set; }
        public string? BatchNo { get; set; }
        public string? Unit { get; set; }
        public decimal Quantity { get; set; }
        public decimal MinimumQuantity { get; set; }
        public string? StorageLocation { get; set; }
        public DateTime Date { get; set; }
        public string? SupplierName { get; set; }
    }
}
