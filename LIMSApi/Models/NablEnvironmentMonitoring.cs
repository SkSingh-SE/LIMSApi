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

        public DateTime? MonitoringDate { get; set; }

        [MaxLength(50)]
        public string? TimeOfReading { get; set; }

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
    }
}
