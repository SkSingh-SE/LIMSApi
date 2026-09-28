using System.ComponentModel.DataAnnotations;

namespace LIMSApi.Dtos
{
    public class AcceptanceCriteriaMasterDto
    {
        public long ID { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string EvaluationType { get; set; } = string.Empty;
        public string ComparisonType { get; set; } = string.Empty;
        public string DecisionRule { get; set; } = string.Empty;
        public string RoundingRule { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
        public string CompanyCode { get; set; } = string.Empty;
    }

    public class AcceptanceCriteriaMasterCreateDto
    {
        [Required]
        [StringLength(50)]
        public string Code { get; set; } = string.Empty;

        [Required]
        [StringLength(150)]
        public string Name { get; set; } = string.Empty;

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

    public class AcceptanceCriteriaMasterUpdateDto
    {
        [Required]
        public long ID { get; set; }

        [Required]
        [StringLength(50)]
        public string Code { get; set; } = string.Empty;

        [Required]
        [StringLength(150)]
        public string Name { get; set; } = string.Empty;

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
        public bool IsActive { get; set; } = true;
    }

    public class AcceptanceCriteriaMasterDropdownDto
    {
        public long ID { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }
}
