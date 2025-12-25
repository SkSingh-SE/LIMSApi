using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LIMSApi.Models
{
    public class SampleInwardDispatchMode
    {
        [Key]
        public long ID { get; set; }
        
        public long InwardID { get; set; }
        public long DispatchModeID { get; set; }
        [ForeignKey("InwardID"), JsonIgnore]
        public virtual SampleInward? SampleInward { get; set; } = null!;
    }
}
