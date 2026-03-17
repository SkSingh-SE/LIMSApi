using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LIMSApi.Models
{
    public class TaxInvoice
    {
        [Key]
        public long ID { get; set; }

        public string InvoiceNo { get; set; } = null!;
        public DateTime InvoiceDate { get; set; }

        // Business linkage
        public long InwardID { get; set; }
        public long CustomerID { get; set; }

        // Amounts (finalized)
        public decimal SubTotal { get; set; } = 0;
        public decimal CGST { get; set; } = 0;
        public decimal SGST { get; set; } = 0;
        public decimal IGST { get; set; } = 0;
        public decimal GrandTotal { get; set; } = 0;

        public string Status { get; set; } = "Generated";
        // Generated | Sent | Paid | PartiallyPaid

        // PO linkage
        public long? PurchaseOrderId { get; set; }

        [MaxLength(50)]
        public string? PaymentTerms { get; set; } // Immediate, Net30, Net60, Net90, OnPO

        public DateTime? PaymentDueDate { get; set; }

        // PDF
        public string? PdfPath { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(InwardID))]
        public SampleInward? Inward { get; set; }

        [ForeignKey(nameof(CustomerID))]
        public Customer? Customer { get; set; }

        [ForeignKey(nameof(PurchaseOrderId))]
        public CustomerPurchaseOrder? PurchaseOrder { get; set; }
    }
}
