using System.ComponentModel.DataAnnotations;

namespace LIMSApi.Models
{
    public class ContactPerson
    {
        [Key]
        public long ID { get; set; }

        [MaxLength(10)]
        public string? Salutation { get; set; }

        [Required]
        [StringLength(100)]
        public required string Name { get; set; }

        [MaxLength(100)]
        public string? Department { get; set; }

        [EmailAddress]
        [MaxLength(100)]
        public string? EmailId { get; set; }

        [Phone]
        [MaxLength(15)]
        public string? MobileNo { get; set; }

        public bool IsWhatsappNo { get; set; }

        [MaxLength(15)]
        public string? TelephoneNo { get; set; }

        public bool SendBill { get; set; }

        public bool SendReport { get; set; }

        [MaxLength(255)]
        public string Address { get; set; } = string.Empty;

        public string Country { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;

        public long AreaID { get; set; }
        [StringLength(10)]
        public required string PinCode { get; set; }

        [Required, StringLength(50)]
        public required string Type { get; set; }

        public long CustomerID { get; set; }
    }
}
