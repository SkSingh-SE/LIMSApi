using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    public class LaboratoryTestAnalysisTypeInvoiceCase
    {
        [Key]
        public long ID { get; set; }

        [Required]
        public long LaboratoryTestAnalysisTypeID { get; set; }

        [Required]
        public long InvoiceCaseConfigID { get; set; }

        [ForeignKey(nameof(LaboratoryTestAnalysisTypeID))]
        public virtual LaboratoryTestAnalysisType? AnalysisType { get; set; }

        [ForeignKey(nameof(InvoiceCaseConfigID))]
        public virtual InvoiceCaseConfiguration? InvoiceCaseConfiguration { get; set; }
    }
}
