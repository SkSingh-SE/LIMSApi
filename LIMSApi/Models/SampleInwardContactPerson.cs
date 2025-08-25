using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    public class SampleInwardContactPerson
    {
        [Key]
        public long ID { get; set; }
        public bool Selected { get; set; }
        public long ContactID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string MobileNo { get; set; } = string.Empty;
        public string EmailId { get; set; } = string.Empty;
        public bool SendBill { get; set; }
        public bool SendReport { get; set; }
        public long InwardID { get; set; }
        [ForeignKey("InwardID")]
        public virtual SampleInward? SampleInward { get; set; } = null!;
    }
}
