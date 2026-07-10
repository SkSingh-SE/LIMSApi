using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    [Table("NablNonConformingWorks")]
    public class NablNonConformingWork : NablFormBase
    {
        public DateTime? NCDate { get; set; }

        [MaxLength(200)]
        public string? SampleCode { get; set; }

        [MaxLength(500)]
        public string? TestParameter { get; set; }

        public string? NCDescription { get; set; }

        [MaxLength(50)]
        public string? NCSource { get; set; } // InternalAudit/CustomerComplaint/EquipmentFailure/Personnel/Method/Other

        [MaxLength(200)]
        public string? DetectedBy { get; set; }

        [MaxLength(200)]
        public string? IdentifiedBy { get; set; }

        public bool? SuspendedWork { get; set; }

        public string? AffectedResults { get; set; }

        public string? ImmediateAction { get; set; }

        [MaxLength(50)]
        public string? NCCategory { get; set; } // Minor/Major

        public string? RootCauseAnalysis { get; set; }
        public string? CorrectiveAction { get; set; }
        public string? SignatureTDQM { get; set; }
        public DateTime? CloserDate { get; set; }
    }
}
