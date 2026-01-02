using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    public class CustomerAmendment
    {
        [Key]
        public long ID { get; set; }

        public long ReportID { get; set; }
        public long TokenID { get; set; }

        public string CustomerName { get; set; } = string.Empty;
        public string Reason { get; set; }
        public string? Remarks { get; set; }

        public string Status { get; set; } // Pending / PaymentPending / Approved / Rejected / Completed

        public bool IsChargeable { get; set; }
        public decimal? Amount { get; set; }
        public long? PaymentOrderID { get; set; }
        public DateTime CreatedOn { get; set; }
        [ForeignKey(nameof(ReportID))]
        public virtual Report? Report { get; set; }
        [ForeignKey(nameof(TokenID))]
        public virtual ReportAmendmentToken? Token { get; set; }
    }

}
