using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LIMSApi.Models
{
    [Table("RetestingComparisonLogs")]
    public class RetestingComparisonLog
    {
        public long Id { get; set; }

        public long RetestingRetainedSampleId { get; set; }
        [JsonIgnore]
        public NablRetesting? NablRetesting { get; set; }

        public long? InitialTestLogId { get; set; }

        public string? QcMonth { get; set; }
        public DateTime? DateOfRetesting { get; set; }

        public string? SampleId { get; set; }

        public string? PreviousPrefix { get; set; }
        public decimal? PreviousValue { get; set; }

        public string? RetestPrefix { get; set; }
        public decimal? RetestValue { get; set; }

        public decimal? Difference { get; set; }
        public decimal? AcceptableLimit { get; set; }

        public string? ResultStatus { get; set; }

        public int? TestedById { get; set; }
        public string? TestedByName { get; set; }

        public string? QmSignature { get; set; }
        public string? Remarks { get; set; }
    }
}