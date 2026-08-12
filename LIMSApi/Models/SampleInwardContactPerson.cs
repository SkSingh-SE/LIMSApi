using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LIMSApi.Models
{
    public class SampleInwardContactPerson
    {
        private string _name = string.Empty;
        private string _mobileNo = string.Empty;
        private string _emailId = string.Empty;

        [Key]
        public long ID { get; set; }
        public bool Selected { get; set; }
        public long ContactID { get; set; }
        public string Name { get => _name ?? string.Empty; set => _name = value ?? string.Empty; }
        public string MobileNo { get => _mobileNo ?? string.Empty; set => _mobileNo = value ?? string.Empty; }
        public string EmailId { get => _emailId ?? string.Empty; set => _emailId = value ?? string.Empty; }
        public bool SendBill { get; set; }
        public bool SendReport { get; set; }
        public long InwardID { get; set; }
        [ForeignKey("InwardID"), JsonIgnore]
        public virtual SampleInward? SampleInward { get; set; } = null!;
    }
}
