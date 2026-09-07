using System.ComponentModel.DataAnnotations;

namespace LIMSApi.Dtos
{
    public class ConditionMasterDto
    {
        public long ID { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = "General";
        public string ValueType { get; set; } = "Decimal";
        public long? ParameterUnitID { get; set; }
        public string? ParameterUnitName { get; set; }
        public string? ParameterUnitSymbol { get; set; }
        public List<string> AllowedOperators { get; set; } = new();
        public List<string>? AllowedValues { get; set; }
        public string? AllowedValuesJson { get; set; }
        public string? DefaultValue { get; set; }
        public string? Description { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
        public long CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public long? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string CompanyCode { get; set; } = string.Empty;
        public int ReferenceCount { get; set; }
    }

    public class ConditionMasterCreateDto
    {
        [Required]
        [StringLength(50)]
        public string Code { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Category { get; set; } = "General";

        [Required]
        [StringLength(20)]
        public string ValueType { get; set; } = "Decimal";

        public long? ParameterUnitID { get; set; }

        public List<string> AllowedOperators { get; set; } = new();

        public List<string>? AllowedValues { get; set; }

        [StringLength(100)]
        public string? DefaultValue { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        public int DisplayOrder { get; set; } = 0;

        public bool IsActive { get; set; } = true;
    }

    public class ConditionMasterUpdateDto
    {
        [Required]
        public long ID { get; set; }

        [Required]
        [StringLength(50)]
        public string Code { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Category { get; set; } = "General";

        [Required]
        [StringLength(20)]
        public string ValueType { get; set; } = "Decimal";

        public long? ParameterUnitID { get; set; }

        public List<string> AllowedOperators { get; set; } = new();

        public List<string>? AllowedValues { get; set; }

        [StringLength(100)]
        public string? DefaultValue { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        public int DisplayOrder { get; set; } = 0;

        public bool IsActive { get; set; } = true;
    }

    public class ConditionMasterDropdownDto
    {
        public long ID { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = "General";
        public string ValueType { get; set; } = "Decimal";
        public long? ParameterUnitID { get; set; }
        public string? UnitSymbol { get; set; }
        public List<string> AllowedOperators { get; set; } = new();
        public List<string>? AllowedValues { get; set; }
        public string? DefaultValue { get; set; }
        public bool IsActive { get; set; }
    }
}
