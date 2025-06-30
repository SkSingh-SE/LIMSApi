using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace LIMSApi.Models
{
    public class InvoiceCaseAliasName
    {
        public long ID { get; set; }

        public long InvoiceConfigurationID { get; set; }

        [ForeignKey(nameof(InvoiceConfigurationID)), JsonIgnore]
        public virtual InvoiceCaseConfiguration? InvoiceConfiguration { get; set; } = null!;

        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
    }
}
