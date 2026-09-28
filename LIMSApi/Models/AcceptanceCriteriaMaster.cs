using System.ComponentModel.DataAnnotations;

namespace LIMSApi.Models
{
    public class AcceptanceCriteriaMaster : AuditProperty
    {
        [Key]
        public long ID { get; set; }

        [Required]
        [StringLength(50)]
        public string Code { get; set; } = string.Empty;

        [StringLength(150)]
        public required string Name { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        [Required]
        [StringLength(20)]
        public string EvaluationType { get; set; } = "TEST";

        [Required]
        [StringLength(30)]
        public string ComparisonType { get; set; } = "RANGE";

        [Required]
        [StringLength(50)]
        public string DecisionRule { get; set; } = "ALL_REQUIRED_PASS";

        [Required]
        [StringLength(30)]
        public string RoundingRule { get; set; } = "ROUND_NEAREST";

        public int DisplayOrder { get; set; } = 0;
    }
}
