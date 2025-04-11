using System.ComponentModel.DataAnnotations;

namespace LIMSApi.Models
{
    public class BankMaster : AuditProperty
    {
        [Key]
        public long ID { get; set; }
        [Required]
        [MaxLength(100)]
        public string BankName { get; set; } = string.Empty;
        [MaxLength(100)]
        [Required]
        public string AccountHolderName { get; set; } = string.Empty;
        [MaxLength(50)]
        [Required]
        public string AccountNumber { get; set; } = string.Empty;
        [MaxLength(50)]
        [Required]
        public string AccountType { get; set; } = string.Empty;
        public string? BranchName { get; set; } = string.Empty;
        [MaxLength(50)]
        [Required]
        public string IFSCCode { get; set; } = string.Empty;
    }
}
