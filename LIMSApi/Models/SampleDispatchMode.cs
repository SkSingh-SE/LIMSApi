using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    public class SampleDispatchMode
    {
        [Key]
        public long ID { get; set; }
        
        public long InwardID { get; set; }
        public long DispatchModeID { get; set; }
        [ForeignKey("InwardID")]
        public virtual SampleInward? SampleInward { get; set; } = null!;
    }
}
