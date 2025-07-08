using System.ComponentModel.DataAnnotations;

namespace LIMSApi.Models
{
    public class SampleInward
    {
        [Key]
        public long ID { get; set; }
        public long CustomerID { get; set; }
    }
}
