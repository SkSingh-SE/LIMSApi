using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    public class NablNonConformingWorkInvestigation
    {
        [Key]
        public long Id { get; set; }
        public long NablNonConformingWorkId { get; set; }
        public long? AssignedToEmployeeId { get; set; }

        [MaxLength(200)]
        public string? AssignedToEmployeeName { get; set; }
        public DateTime? InvestigationDate { get; set; }

        [MaxLength(100)]
        public string? InvestigationMethod { get; set; }
        public string? RootCause { get; set; }
        public string? ContributingFactors { get; set; }
        public string? InvestigationDetails { get; set; }
        public string? RecommendedAction { get; set; }

        [ForeignKey("NablNonConformingWorkId")]
        public virtual NablNonConformingWork? NonConformingWork { get; set; }
    }
}
