using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LIMSApi.Models
{
    public class EquipmentSOP
    {
        [Key]
        public long ID { get; set; }
        public long EquipmentID { get; set; }
        public string Title { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string? FilePath { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; // e.g., "attachment", "video", etc.
        public long? UploadReferenceID { get; set; }

        [NotMapped]
        public IFormFile? File { get; set; }
    }
}
