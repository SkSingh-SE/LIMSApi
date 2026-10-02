using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    [Table("NablComplaints")]
    public class NablComplaint : NablFormBase
    {
        public long? CustomerId { get; set; }

        [ForeignKey("CustomerId")]
        public virtual Customer? Customer { get; set; }

        [MaxLength(200)]
        public string? CustomerName { get; set; }

        public DateTime? ComplaintDate { get; set; }

        public string? ComplaintDescription { get; set; }

        [MaxLength(50)]
        public string? ComplaintCategory { get; set; } // Report/Sample/Service/Other

        [MaxLength(200)]
        public string? SampleCode { get; set; }

        [MaxLength(200)]
        public string? ReportNo { get; set; }

        [MaxLength(200)]
        public string? ReceivedBy { get; set; }

        public DateTime? InvestigationDate { get; set; }

        public string? RootCause { get; set; }

        public string? CorrectiveAction { get; set; }

        public string? PreventiveAction { get; set; }

        public DateTime? ClosureDate { get; set; }

        [MaxLength(200)]
        public string? ClosedBy { get; set; }

        public DateTime? CustomerInformedDate { get; set; }

        public bool? CustomerSatisfied { get; set; }

        public string? Remarks { get; set; }
        public DateTime? MonthYear { get; set; }
        public DateTime? ReferenceNoDate { get; set; }
        public string? ComplaintNo { get; set; }
        public string? ComplainantName { get; set; }
        public string? ValidationOfComplaint { get; set; }
        public string? OutcomeOfInvestigation { get; set; }
        public string? SignatureQM { get; set; }
    }
}
