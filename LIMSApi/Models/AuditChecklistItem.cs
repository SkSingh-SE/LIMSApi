using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LIMSApi.Models
{
    public class AuditChecklistItem
    {
        [Key]
        public long ID { get; set; }

        public long ChecklistId { get; set; }

        [ForeignKey(nameof(ChecklistId))]
        [JsonIgnore]
        public virtual NablAuditChecklist? Checklist { get; set; }
        public int? IsoClauseId { get; set; }
        [MaxLength(100)]
        public string? IsoClauseName { get; set; }

        [MaxLength(1000)]
        public string? AuditQuestion { get; set; }
        public string? ObjectiveEvidence { get; set; }

        [MaxLength(100)]
        public string? FindingType { get; set; }
        public string? Remarks { get; set; }
        public long? NcId { get; set; }

        [MaxLength(100)]
        public string? NcNo { get; set; }
        public bool IsActive { get; set; } = true;

        [NotMapped]
        public int? NcCurrentStep { get; set; }

        [NotMapped]
        public string? NcStatus { get; set; }

    }
}
