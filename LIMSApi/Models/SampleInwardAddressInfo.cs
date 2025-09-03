using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LIMSApi.Models
{
    public class SampleInwardAddressInfo
    {
        [Key]
        public long ID { get; set; }
        public long ContactPersonID { get; set; }
        public string ContactPersonName { get; set; }
        public string Address { get; set; }
        public string PinCode { get; set; }
        public string Area { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string Type { get; set; }
        public long InwardID { get; set; }
        [ForeignKey("InwardID"), JsonIgnore]
        public virtual SampleInward? SampleInward { get; set; } = null!;
    }
}
