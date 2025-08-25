using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    public class SampleAdditionalDetail
    {
        [Key]
        public long ID { get; set; }
        public string SampleNo { get; set; }
        public string Label { get; set; }
        public string Value { get; set; }
        public long SampleID { get; set; }
        [ForeignKey("SampleID")]
        public virtual SampleDetail? SampleDetail { get; set; }
    }
}
