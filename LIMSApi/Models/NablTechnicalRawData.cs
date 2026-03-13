using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    [Table("NablTechnicalRawDatas")]
    public class NablTechnicalRawData : NablFormBase
    {
        [MaxLength(100)]
        public string? SampleCode { get; set; }

        [MaxLength(500)]
        public string? TestParameter { get; set; }

        [MaxLength(500)]
        public string? TestMethod { get; set; }

        public long? EquipmentId { get; set; }

        [ForeignKey("EquipmentId")]
        public virtual EquipmentMaster? Equipment { get; set; }

        public DateTime? ObservationDate { get; set; }

        public string? ObservationsJson { get; set; } // JSON array of readings

        [MaxLength(200)]
        public string? CalculatedResult { get; set; }

        [MaxLength(50)]
        public string? Unit { get; set; }

        [MaxLength(200)]
        public string? Uncertainty { get; set; }

        [MaxLength(500)]
        public string? RawDataFile { get; set; }

        [MaxLength(200)]
        public string? TestedBy { get; set; }

        [MaxLength(200)]
        public string? CheckedBy { get; set; }

        [MaxLength(500)]
        public string? Remarks { get; set; }
    }
}
