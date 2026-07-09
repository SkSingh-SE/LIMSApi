using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    public class LaboratoryTestSubGroupInvoiceCase
    {
        [Key]
        public long ID { get; set; }

        [Required]
        public long LaboratoryTestSubGroupID { get; set; }

        [Required]
        public long InvoiceCaseConfigID { get; set; }

        [ForeignKey(nameof(LaboratoryTestSubGroupID))]
        public virtual LaboratoryTestSubGroup? SubGroup { get; set; }

        [ForeignKey(nameof(InvoiceCaseConfigID))]
        public virtual InvoiceCaseConfiguration? InvoiceCaseConfiguration { get; set; }
    }
}
