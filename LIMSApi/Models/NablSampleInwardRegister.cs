using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    [Table("NablSampleInwardRegisters")]
    public class NablSampleInwardRegister : NablFormBase
    {
        public long? SampleInwardId { get; set; }

        [MaxLength(100)]
        public string? SampleCode { get; set; }

        [MaxLength(200)]
        public string? CustomerName { get; set; }

        [MaxLength(500)]
        public string? SampleDescription { get; set; }

        public DateTime? ReceivedDate { get; set; }

        [MaxLength(200)]
        public string? ReceivedBy { get; set; }

        [MaxLength(200)]
        public string? SampleCondition { get; set; }

        [MaxLength(200)]
        public string? StorageLocation { get; set; }

        [MaxLength(500)]
        public string? TestsRequested { get; set; }

        public DateTime? TargetCompletionDate { get; set; }

        [MaxLength(500)]
        public string? Remarks { get; set; }
    }
}
