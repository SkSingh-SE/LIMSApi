using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LIMSApi.Models
{
    public class SampleAdditionalDetail
    {
        [Key]
        public long ID { get; set; }
        public string SampleNo { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public long SampleID { get; set; }
        [ForeignKey("SampleID"), JsonIgnore]
        public virtual SampleDetail? SampleDetail { get; set; }
    }
}
