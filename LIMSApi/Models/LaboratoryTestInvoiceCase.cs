using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LIMSApi.Models
{
    public class LaboratoryTestInvoiceCase
    {
        [Key]
        public long ID { get; set; }
        public long LabTestID {  get; set; }
        public long InvoiceCaseConfigID {  get; set; }
        [ForeignKey("InvoiceCaseConfigID")]
        public virtual InvoiceCaseConfiguration? InvoiceCaseConfiguration { get; set; }


    }
}
