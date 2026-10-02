using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    [Table("NablIntermediateChecks")]
    public class NablIntermediateCheck : NablFormBase
    {
        public long? EquipmentId { get; set; }
 

        [MaxLength(100)]
        public string? EquipmentName { get; set; }

        public DateTime? CheckDate { get; set; }

        [MaxLength(200)]
        public string? CheckMethod { get; set; }

        [MaxLength(200)]
        public string? ReferenceStandard { get; set; }

        [Column(TypeName = "decimal(18,6)")]
        public decimal? ObservedValue { get; set; }

        [Column(TypeName = "decimal(18,6)")]
        public decimal? AcceptedValue { get; set; }

        [Column(TypeName = "decimal(18,6)")]
        public decimal? Tolerance { get; set; }

        [MaxLength(50)]
        public string? ResultStatus { get; set; } // Pass/Fail

        [MaxLength(200)]
        public string? CheckedBy { get; set; }

  
        public string? EquipmentNo { get; set; }
        public string? DepartmentName { get; set; }
        public string? EquipmentType { get; set; }
        public string? OEMName { get; set; }
        public string? ModelNumber { get; set; }
        public int? CalibrationFrequencyDays { get; set; }
        public string? IntermediateCheckInterval { get; set; }
        public DateTime? LastCalibrationDate { get; set; }
        public DateTime? NextCalibrationDueDate { get; set; }
        public string? IntermediateCheckLogsJson { get; set; }
        [NotMapped]
        public List<IntermediateCheckLogs>? IntermediateCheckLogs { get; set; }
    }



    [NotMapped]
    public class IntermediateCheckLogs
    {
        public DateTime? CheckDate { get; set; }
        public string? CheckType { get; set; }
        public string? ReferenceStandard { get; set; }
        public decimal? ReferenceValue { get; set; }
        public decimal? ObservedValue { get; set; }
        public decimal? AcceptanceCriteria { get; set; }
        public string? Result { get; set; }
        public string? AuditeeName { get; set; }
        public long? AuditeeId { get; set; }

    }
}
