using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    [Table("NablNonConformingWorkClosures")]
    public class NablNonConformingWorkClosure
    {
        [Key]
        public long Id { get; set; }

        public long NablNonConformingWorkId { get; set; }

        public DateTime? ClosureDate { get; set; }

        public long? ClosedByEmployeeId { get; set; }

        [MaxLength(200)]
        public string? ClosedByEmployeeName { get; set; }

        public string? FinalRemarks { get; set; }

        [MaxLength(50)]
        public string? Status { get; set; }

        [ForeignKey("NablNonConformingWorkId")]
        public virtual NablNonConformingWork? NonConformingWork { get; set; }
    }
}
