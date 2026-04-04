using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    [Table("NablEnvironmentMonitorings")]
    public class NablEnvironmentMonitoring : NablFormBase
    {
        public long? DepartmentId { get; set; }

        [ForeignKey("DepartmentId")]
        public virtual DepartmentMaster? Department { get; set; }

        [MaxLength(200)]
        public string? DepartmentName { get; set; }

        // Lab Room linkage (Phase 8)
        public long? LabRoomId { get; set; }

        [ForeignKey(nameof(LabRoomId))]
        public virtual LabRoom? LabRoom { get; set; }

        [MaxLength(200)]
        public string? RoomName { get; set; }

        // Monthly monitoring period
        public int? MonitoringMonth { get; set; }
        public int? MonitoringYear { get; set; }

        public DateTime? MonitoringDate { get; set; }

        [MaxLength(50)]
        public string? TimeOfReading { get; set; }

        // Single reading (legacy — kept for backward compat)
        [Column(TypeName = "decimal(6,2)")]
        public decimal? Temperature { get; set; }

        [Column(TypeName = "decimal(6,2)")]
        public decimal? Humidity { get; set; }

        [Column(TypeName = "decimal(6,2)")]
        public decimal? AcceptableTemperatureMin { get; set; }

        [Column(TypeName = "decimal(6,2)")]
        public decimal? AcceptableTemperatureMax { get; set; }

        [Column(TypeName = "decimal(6,2)")]
        public decimal? AcceptableHumidityMin { get; set; }

        [Column(TypeName = "decimal(6,2)")]
        public decimal? AcceptableHumidityMax { get; set; }

        public bool IsWithinLimits { get; set; }

        [MaxLength(500)]
        public string? CorrectiveAction { get; set; }

        [MaxLength(200)]
        public string? RecordedBy { get; set; }

        // Daily records (Phase 8 — monthly header with child records)
        public virtual ICollection<EnvironmentDailyRecord> DailyRecords { get; set; } = new List<EnvironmentDailyRecord>();
    }
}
