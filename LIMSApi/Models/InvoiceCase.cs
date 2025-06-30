using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    public class InvoiceCase : AuditProperty
    {
        [Key]
        public long ID { get; set; }
        [Required, StringLength(20)]
        public required string FinancialYear { get; set; }
        public long LaboratoryTestID { get; set; }

        [ForeignKey(nameof(LaboratoryTestID))]
        public virtual LaboratoryTest? LaboratoryTest { get; set; }

        public ICollection<InvoiceCasePrice> InvoiceCasePrices { get; set; } = new List<InvoiceCasePrice>();
    }
}
