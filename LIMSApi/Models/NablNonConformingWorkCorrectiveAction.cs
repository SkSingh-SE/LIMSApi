using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    [Table("NablNonConformingWorkCorrectiveActions")]
    public class NablNonConformingWorkCorrectiveAction
    {
        [Key]
        public long Id { get; set; }
        public long NablNonConformingWorkId { get; set; }

        [MaxLength(50)]
        public string? ActionNo { get; set; }
        public string? CorrectiveAction { get; set; }
        public long? ResponsiblePersonId { get; set; }

        [MaxLength(200)]
        public string? ResponsiblePersonName { get; set; }
        public DateTime? TargetDate { get; set; }
        public DateTime? CompletionDate { get; set; }
        public string? ResourcesRequired { get; set; }
        public string? PreventiveAction { get; set; }

        [ForeignKey("NablNonConformingWorkId")]
        public virtual NablNonConformingWork? NonConformingWork { get; set; }
    }
}
