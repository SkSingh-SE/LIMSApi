using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    [Table("NablPurchaseIndents")]
    public class NablPurchaseIndent : NablFormBase
    {
        public DateTime? IndentDate { get; set; }

        [MaxLength(200)]
        public string? RequestedBy { get; set; }

        public long? DepartmentId { get; set; }

        [ForeignKey("DepartmentId")]
        public virtual DepartmentMaster? Department { get; set; }

        [MaxLength(200)]
        public string? DepartmentName { get; set; }

        public string? ItemsJson { get; set; } // JSON array of {itemName, quantity, unit, specification}

        public string? Justification { get; set; }

        public DateTime? RequiredByDate { get; set; }

        [MaxLength(50)]
        public string? Priority { get; set; } // Normal/Urgent

        [MaxLength(200)]
        public string? ApprovedBy { get; set; }
        public string TechnicalSpecification { get; set; }
        public string IndentorName { get; set; }
        public string? Remarks { get; set; }
        public int? Quantity { get; set; }
        public DateTime? ExpectedDate { get; set; }
        public string? PINo { get; set; }
        public string? UnitOfMeasure { get; set; }
    }
}
