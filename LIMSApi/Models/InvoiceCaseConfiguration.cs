using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Models
{
    [Index(nameof(Name), IsUnique = true)]
    public class InvoiceCaseConfiguration : AuditProperty

    {
        [Key]
        public long ID { get; set; }
        [MaxLength(100)]
        public string SelectionType { get; set; } = string.Empty;

        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string AliasName { get; set; } = string.Empty; // comma-separated

        [MaxLength(100)]
        public string Value { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Start { get; set; } = string.Empty;

        [MaxLength(100)]
        public string End { get; set; } = string.Empty;

        [MaxLength(50)]
        public string Unit { get; set; } = string.Empty;

        // Navigation property
        public ICollection<InvoiceCaseAliasName> AliasNames { get; set; } = new List<InvoiceCaseAliasName>();
    }
}
