using System.ComponentModel.DataAnnotations;

namespace LIMSApi.Models
{
    public class VendorMaster : AuditProperty
    {
        [Key]
        public long ID { get; set; }

        [Required]
        [StringLength(100)]
        public string Code { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string TellyLedgerName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public required string GSTNo { get; set; }

        [Required]
        [StringLength(100)]
        public required string PANNo { get; set; }

        [StringLength(100)]
        public string? ContactPersonName { get; set; }

        [Phone, StringLength(100)]
        public string? MobileNo { get; set; }

        [StringLength(100)]
        [EmailAddress]
        public string? EmailID { get; set; }
        [StringLength(250)]
        public string? Address { get; set; }
    }
}
