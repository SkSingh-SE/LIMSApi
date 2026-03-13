using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    [Table("NablSampleMusterRegisters")]
    public class NablSampleMusterRegister : NablFormBase
    {
        [MaxLength(100)]
        public string? SampleCode { get; set; }

        [MaxLength(500)]
        public string? SampleDescription { get; set; }

        public DateTime? MusteringDate { get; set; }

        [MaxLength(200)]
        public string? MusteredBy { get; set; }

        public int? NumberOfPieces { get; set; }

        [MaxLength(200)]
        public string? SampleDimensions { get; set; }

        [MaxLength(500)]
        public string? CuttingInstructions { get; set; }

        public int? PreparedSamples { get; set; }

        [MaxLength(200)]
        public string? WasteGenerated { get; set; }

        [MaxLength(200)]
        public string? DisposalMethod { get; set; }

        [MaxLength(500)]
        public string? Remarks { get; set; }
    }
}
