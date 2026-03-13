using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    [Table("NablSampleLabels")]
    public class NablSampleLabel : NablFormBase
    {
        [MaxLength(100)]
        public string? SampleCode { get; set; }

        [MaxLength(500)]
        public string? SampleDescription { get; set; }

        [MaxLength(200)]
        public string? CustomerName { get; set; }

        public DateTime? ReceivedDate { get; set; }

        [MaxLength(500)]
        public string? TestParameter { get; set; }

        [MaxLength(100)]
        public string? LabelNo { get; set; }

        [MaxLength(200)]
        public string? StorageCondition { get; set; }

        public DateTime? ExpiryDate { get; set; }

        [MaxLength(200)]
        public string? LabelledBy { get; set; }
    }
}
