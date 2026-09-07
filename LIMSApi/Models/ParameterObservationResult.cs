using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    public class ParameterObservationResult : AuditProperty
    {
        [Key]
        public long ID { get; set; }

        public long TestObservationID { get; set; }

        public long ParameterMasterID { get; set; }

        [StringLength(255)]
        public string? RawValue { get; set; }

        [Column(TypeName = "decimal(18, 6)")]
        public decimal? NumericValue { get; set; }

        [StringLength(255)]
        public string? CalculatedValue { get; set; }

        public bool IsFormulaCalculated { get; set; }

        [Column(TypeName = "decimal(18, 6)")]
        public decimal? SpecMin { get; set; }

        [Column(TypeName = "decimal(18, 6)")]
        public decimal? SpecMax { get; set; }

        [StringLength(50)]
        public string? ResultStatus { get; set; }

        [ForeignKey("TestObservationID")]
        public virtual TestObservation TestObservation { get; set; } = null!;

        [ForeignKey("ParameterMasterID")]
        public virtual ParameterMaster ParameterMaster { get; set; } = null!;
    }
}
