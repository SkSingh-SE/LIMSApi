using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    public class EquipmentReferenceMaterial : AuditProperty
    {
        [Key]
        public long ID { get; set; }

        [Required]
        public long EquipmentMasterID { get; set; }

        [Required, MaxLength(200)]
        public string MaterialName { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string Type { get; set; } = string.Empty; // "CRM" or "SUS"

        [MaxLength(100)]
        public string? LotNumber { get; set; }

        [MaxLength(100)]
        public string? CertificateNumber { get; set; }

        public DateTime? ManufactureDate { get; set; }
        public DateTime? ExpiryDate { get; set; }

        [MaxLength(200)]
        public string? Supplier { get; set; }

        [MaxLength(50)]
        public string? Status { get; set; } // "Active", "Expired", "Consumed"

        [MaxLength(500)]
        public string? Remark { get; set; }

        [ForeignKey("EquipmentMasterID")]
        public virtual EquipmentMaster? Equipment { get; set; }
    }
}
