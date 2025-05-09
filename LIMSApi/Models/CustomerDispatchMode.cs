using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LIMSApi.Models
{
    public class CustomerDispatchMode
    {
        [Key]
        public long ID { get; set; }
        public long CustomerID { get; set; }

        public long DispatchModeID { get; set; }
        [ForeignKey("DispatchModeID")]
        public virtual DispatchModeMaster? DispatchMode { get; set; }
    }
}
