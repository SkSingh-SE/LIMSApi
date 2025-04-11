using System.ComponentModel.DataAnnotations;

namespace LIMSApi.Models
{
    public class SubContractorMaster : AuditProperty
    {
        [Key]
        public long ID { get; set; }

        [Required, StringLength(100)]
        public required string Name { get; set; }

        [StringLength(100)]
        public string? Alias { get; set; }

        [Required, StringLength(500)]
        public required string Address { get; set; }

        [EmailAddress, StringLength(100)]
        public string? EmailID { get; set; }

        [Required, Phone, StringLength(15)]
        public required string MobileNo { get; set; }

        [Phone, StringLength(15)]
        public string? PhoneNo { get; set; }

        [Required, StringLength(50)]
        public required string GSTNo { get; set; }
    }
}
