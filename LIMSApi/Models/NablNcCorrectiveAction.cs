using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    [Table("NablNcCorrectiveActions")]
    public class NablNcCorrectiveAction : NablFormBase
    {
        public long? NCId { get; set; }

        [ForeignKey("NCId")]
        public virtual NablNonConformingWork? NC { get; set; }

        [MaxLength(200)]
        public string? NCRef { get; set; }

        public DateTime? CADate { get; set; }

        public string? RootCause { get; set; }

        public string? CorrectiveAction { get; set; }

        public string? PreventiveAction { get; set; }

        [MaxLength(200)]
        public string? ImplementedBy { get; set; }

        public DateTime? ImplementationDate { get; set; }

        public DateTime? VerificationDate { get; set; }

        [MaxLength(200)]
        public string? VerifiedBy { get; set; }

        public bool? EffectivenessEvaluated { get; set; }

        public string? EffectivenessResult { get; set; }

        public bool? Closed { get; set; }

        public DateTime? ClosureDate { get; set; }

        [MaxLength(200)]
        public string? ClosedBy { get; set; }
        public string? ActivityAssessed { get; set; }
        public string? AuditNo { get; set; }
        public string? Auditee { get; set; }
        public string? Auditor { get; set; }
        public string? CorrectiveActionProposed { get; set; }
        public long? DepartmentID { get; set; }
        public long? ImplementedById { get; set; }
        public long? ObservedByID { get; set; }
        public long? ProposedById { get; set; }
        public long? SignOfAuditorID { get; set; }
        public long? SignatureOfQMID { get; set; }
        public long? VerifiedById { get; set; }
        public string? ClauseNo { get; set; }
        public string? VerifiedByName { get; set; }
        public string? SignatureOfQMName { get; set; }
        public string? TimeRequirement { get; set; }
        public string? ProposedByName { get; set; }
        public string? ObservedByName { get; set; }
        public string? SignOfAuditorName { get; set; }
        public string? ImplementedByName { get; set; }
        public string? DepartmentName { get; set; }
        public string? EffectivenessOfAction { get; set; }
        public string? NcNo { get; set; }
        public string? NcObserved { get; set; }
        public DateTime? CorrectiveActionDate { get; set; }
        public DateTime? ImplementedDate { get; set; }
        public DateTime? VerifiedDate { get; set; }
        public string? CorrectiveActionTaken { get; set; }
        public long? AuditorId { get; set; }
        public long? AuditeeId { get; set; }
    }
}
