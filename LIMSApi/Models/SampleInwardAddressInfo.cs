using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LIMSApi.Models
{
    public class SampleInwardAddressInfo
    {
        private long? _contactPersonID;
        private string _contactPersonName = string.Empty;
        private string _address = string.Empty;
        private string _pinCode = string.Empty;
        private string _area = string.Empty;
        private string _city = string.Empty;
        private string _state = string.Empty;
        private string _country = string.Empty;
        private string _type = string.Empty;
        private string _mobileNo = string.Empty;
        private string _emailId = string.Empty;

        [Key]
        public long ID { get; set; }
        public long? ContactPersonID { get => _contactPersonID ?? 0; set => _contactPersonID = value ?? 0; }
        public string ContactPersonName { get => _contactPersonName ?? string.Empty; set => _contactPersonName = value ?? string.Empty; }
        public string Address { get => _address ?? string.Empty; set => _address = value ?? string.Empty; }
        public string PinCode { get => _pinCode ?? string.Empty; set => _pinCode = value ?? string.Empty; }
        public string Area { get => _area ?? string.Empty; set => _area = value ?? string.Empty; }
        public string City { get => _city ?? string.Empty; set => _city = value ?? string.Empty; }
        public string State { get => _state ?? string.Empty; set => _state = value ?? string.Empty; }
        public string Country { get => _country ?? string.Empty; set => _country = value ?? string.Empty; }
        public string Type { get => _type ?? string.Empty; set => _type = value ?? string.Empty; }
        public string MobileNo { get => _mobileNo ?? string.Empty; set => _mobileNo = value ?? string.Empty; }
        public string EmailId { get => _emailId ?? string.Empty; set => _emailId = value ?? string.Empty; }
        public long InwardID { get; set; }
        [ForeignKey("InwardID"), JsonIgnore]
        public virtual SampleInward? SampleInward { get; set; } = null!;

        public long? CustomerID { get; set; }
        [ForeignKey("CustomerID"), JsonIgnore]
        public virtual Customer? OverrideCustomer { get; set; }
    }
}
