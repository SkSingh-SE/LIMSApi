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
    }
}
