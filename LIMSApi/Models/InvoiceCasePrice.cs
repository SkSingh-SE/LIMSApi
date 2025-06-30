using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    public class InvoiceCasePrice
    {
        [Key]
        public long ID { get; set; }
        public long InvoiceCaseID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string AliasName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public long InvoiceCaseConfigID { get; set; }

        [ForeignKey(nameof(InvoiceCaseConfigID))]
        public virtual InvoiceCaseConfiguration? Configuration { get; set; }

    }
}
