using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    [Table("NablEquipmentHistories")]
    public class NablEquipmentHistory : NablFormBase
    {
        public long? EquipmentId { get; set; }

        [ForeignKey("EquipmentId")]
        public virtual EquipmentMaster? Equipment { get; set; }

        [MaxLength(100)]
        public string? EquipmentCode { get; set; }

        [MaxLength(200)]
        public string? EquipmentName { get; set; }

        [MaxLength(200)]
        public string? Manufacturer { get; set; }

        [MaxLength(100)]
        public string? ModelNo { get; set; }

        [MaxLength(100)]
        public string? SerialNo { get; set; }

        public DateTime? PurchaseDate { get; set; }

        public DateTime? InstallationDate { get; set; }

        [MaxLength(200)]
        public string? Location { get; set; }

        [MaxLength(100)]
        public string? CalibrationFrequency { get; set; }

        public DateTime? LastCalibrationDate { get; set; }

        public DateTime? NextCalibrationDate { get; set; }

        [MaxLength(200)]
        public string? CalibrationAgency { get; set; }

        public string? MaintenanceRecordsJson { get; set; } // JSON array of {date, type, remarks, performedBy}

        [MaxLength(50)]
        public string? CurrentStatus { get; set; } // Active/UnderMaintenance/Decommissioned
    }
}
