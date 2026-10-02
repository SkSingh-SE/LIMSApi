using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LIMSApi.Models
{
    [Table("RetestingInitialTestLogs")]
    public class RetestingInitialTestLog
    {
        public long Id { get; set; }

        public long RetestingRetainedSampleId { get; set; }

        [JsonIgnore]
        public NablRetesting? NablRetesting { get; set; }

        public DateTime? DateOfTesting { get; set; }
        public string? SampleId { get; set; }

        public string? ResultPrefix { get; set; }
        public decimal? ResultValue { get; set; }

        public int? TestedById { get; set; }
        public string? TestedByName { get; set; }

        public string? Remarks { get; set; }
        public string? LatestResultPrefix { get; set; }
        public decimal? LatestResultValue { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}