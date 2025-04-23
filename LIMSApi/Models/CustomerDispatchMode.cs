using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    public class CustomerDispatchMode
    {
        [Key]
        public long ID { get; set; }
        public long CustomerID { get; set; }
        [ForeignKey("CustomerID")]
        public Customer Customer { get; set; }

        public long DispatchModeID { get; set; }
        [ForeignKey("DispatchModeID")]
        public DispatchModeMaster DispatchMode { get; set; }
        public virtual ICollection<CustomerDispatchMode> CustomerDispatchModes { get; set; } = new List<CustomerDispatchMode>();
    }
}
