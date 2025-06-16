using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LIMSApi.Models
{
    public class EquipmentMaintenance
    {
        [Key]
        public long ID { get; set; }
        public long EquipmentID { get; set; }
        public DateTime MaintenanceDate { get; set; }
        public string Certificate { get; set; } = string.Empty;
        public string? CertificatePath { get; set; } = string.Empty;
        public long? UploadReferenceID { get; set; }

        [NotMapped]
        public IFormFile? File { get; set; }
    }
}
