using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    public class InventoryQuantityLog
    {
        public long ID { get; set; }
        public long InventoryId { get; set; }

        public decimal AddedQuantity { get; set; }

        public decimal PreviousQuantity { get; set; }

        public decimal NewQuantity { get; set; }

        public DateTime AddedDate { get; set; }

        public long? AddedBy { get; set; }

        public string? Remarks { get; set; }

        public bool IsActive { get; set; } = true;

        [ForeignKey("InventoryId")]
        public InventoryManagement? InventoryManagement { get; set; }

    }
}
