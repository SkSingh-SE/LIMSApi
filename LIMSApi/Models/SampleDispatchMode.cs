using System.ComponentModel.DataAnnotations;

namespace LIMSApi.Models
{
    public class SampleDispatchMode
    {
        [Key]
        public long ID { get; set; }
        public long SampleID { get; set; }
        public long DispatchModeID { get; set; }

    }
}
