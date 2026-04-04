using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    /// <summary>
    /// LabRoom: Physical lab room/area where tests are performed.
    /// Links to Equipment (which room equipment is in) and Environment Monitoring (room conditions).
    /// </summary>
    public class LabRoom : AuditProperty
    {
        [Key]
        public long ID { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(20)]
        public string? Code { get; set; }

        public long? DepartmentID { get; set; }

        [MaxLength(200)]
        public string? Building { get; set; }

        [MaxLength(50)]
        public string? Floor { get; set; }

        [MaxLength(500)]
        public string? Purpose { get; set; }

        // Default acceptable environment limits for this room
        [Column(TypeName = "decimal(6,2)")]
        public decimal? DefaultTempMin { get; set; }

        [Column(TypeName = "decimal(6,2)")]
        public decimal? DefaultTempMax { get; set; }

        [Column(TypeName = "decimal(6,2)")]
        public decimal? DefaultHumidityMin { get; set; }

        [Column(TypeName = "decimal(6,2)")]
        public decimal? DefaultHumidityMax { get; set; }

        // Is environment monitoring enabled for this room?
        public bool MonitoringEnabled { get; set; } = true;

        [ForeignKey(nameof(DepartmentID))]
        public virtual DepartmentMaster? Department { get; set; }
    }
}
