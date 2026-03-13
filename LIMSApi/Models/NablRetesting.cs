using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    [Table("NablRetestings")]
    public class NablRetesting : NablFormBase
    {
        [MaxLength(200)]
        public string? SampleCode { get; set; }

        public DateTime? OriginalTestDate { get; set; }

        public string? RetestReason { get; set; }

        public DateTime? RetestDate { get; set; }

        [MaxLength(500)]
        public string? TestParameter { get; set; }

        [MaxLength(500)]
        public string? TestMethod { get; set; }

        [MaxLength(500)]
        public string? OriginalResult { get; set; }

        [MaxLength(500)]
        public string? RetestResult { get; set; }

        [MaxLength(100)]
        public string? Unit { get; set; }

        [MaxLength(500)]
        public string? AcceptanceCriteria { get; set; }

        [MaxLength(50)]
        public string? RetestConclusion { get; set; } // Consistent/Inconsistent

        [MaxLength(200)]
        public string? TestedBy { get; set; }

        [MaxLength(200)]
        public string? AuthorizedBy { get; set; }

        public string? Remarks { get; set; }
    }
}
