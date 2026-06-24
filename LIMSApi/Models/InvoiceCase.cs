using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    public class InvoiceCase : AuditProperty
    {
        [Key]
        public long ID { get; set; }

        public long? FinancialYearId { get; set; }

        [ForeignKey(nameof(FinancialYearId))]
        public virtual FinancialYear? FinancialYearEntity { get; set; }

        // Date this price version takes effect. Resolution key for all test-charge lookups
        // (greatest EffectiveFrom <= sample inward date). FinancialYearId is derived from this on save.
        public DateTime EffectiveFrom { get; set; }

        public long LaboratoryTestID { get; set; }

        [ForeignKey(nameof(LaboratoryTestID))]
        public virtual LaboratoryTest? LaboratoryTest { get; set; }

        [StringLength(50)]
        public string? DefaultPricingType { get; set; }

        public ICollection<InvoiceCasePrice> InvoiceCasePrices { get; set; } = new List<InvoiceCasePrice>();
    }
}
