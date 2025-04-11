using System.ComponentModel.DataAnnotations;

namespace LIMSApi.Models
{
    public class TPIMaster : AuditProperty
    {
        [Key]
        public long ID { get; set; }

        [Required, StringLength(100)]
        public required string AgencyName { get; set; }

        [Required, EmailAddress, StringLength(100)]
        public required string EmailId { get; set; }

        [Required, StringLength(50)]
        public required string ContactNo { get; set; }
    }
}
