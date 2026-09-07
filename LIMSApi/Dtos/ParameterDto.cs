using System.ComponentModel.DataAnnotations;

namespace LIMSApi.Dtos;

public class ParameterListItemDto
{
    public long ID { get; set; }
    public string? Code { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Symbol { get; set; }
    public string? ParameterType { get; set; }
    public string? InputType { get; set; }
    public long? ParameterUnitID { get; set; }
    public long? UnitMasterId { get => ParameterUnitID; set => ParameterUnitID = value ?? ParameterUnitID; }
    public string? UnitName { get; set; }
    public string? UnitSymbol { get; set; }
    public int DecimalPrecision { get; set; }
    public int Precision { get => DecimalPrecision; set => DecimalPrecision = value; }
    public bool IsCalculated { get; set; }
    public string? CalculationRole { get; set; }
    public string? Formula { get; set; }
    public string? FormulaDisplay { get; set; }
    public int? Sequence { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime? ModifiedOn { get; set; }
    public string? CreatedByName { get; set; }
    public string? ModifiedByName { get; set; }
}

public class ParameterDetailDto
{
    public long ID { get; set; }
    public string? Code { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Symbol { get; set; }
    public string? ParameterType { get; set; }
    public string? InputType { get; set; }
    public long? ParameterUnitID { get; set; }
    public long? UnitMasterId => ParameterUnitID;
    public string? UnitName { get; set; }
    public string? UnitSymbol { get; set; }
    public long? ParameterUnitEquivalentID { get; set; }
    public decimal? UnitConversionFactor { get; set; }
    public int DecimalPrecision { get; set; }
    public int Precision => DecimalPrecision;
    public bool IsCalculated { get; set; }
    public string? CalculationRole { get; set; }
    public string? Formula { get; set; }
    public string? FormulaDisplay { get; set; }
    public int? Sequence { get; set; }
    public string? Description { get; set; }
    public string? ElementType { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime? ModifiedOn { get; set; }
    public List<ParameterDropdownOptionDto> DropdownOptions { get; set; } = new();
}

public class ParameterCreateDto
{
    [Required(ErrorMessage = "Parameter Code is required.")]
    [StringLength(100, ErrorMessage = "Parameter Code cannot exceed 100 characters.")]
    public string Code { get; set; } = string.Empty;

    [Required(ErrorMessage = "Parameter Name is required.")]
    [StringLength(200, ErrorMessage = "Parameter Name cannot exceed 200 characters.")]
    public string Name { get; set; } = string.Empty;

    [StringLength(50)]
    public string? Symbol { get; set; }

    public string? ParameterType { get; set; } = "Reported";

    [StringLength(20)]
    public string? InputType { get; set; } = "Decimal";

    public long? ParameterUnitID { get; set; }
    public long? UnitMasterId { get => ParameterUnitID; set => ParameterUnitID = value ?? ParameterUnitID; }
    public long? ParameterUnitEquivalentID { get; set; }
    public decimal? UnitConversionFactor { get; set; }

    [Range(0, 6, ErrorMessage = "Decimal precision must be between 0 and 6.")]
    public int DecimalPrecision { get; set; } = 2;
    public int? Precision { get => DecimalPrecision; set { if (value.HasValue) DecimalPrecision = value.Value; } }

    public string? CalculationRole { get; set; } = "Input";
    public bool IsCalculated { get; set; } = false;

    public string? Formula { get; set; }
    public string? FormulaDisplay { get; set; }

    public int? Sequence { get; set; }

    [StringLength(500)]
    public string? Description { get; set; }

    public string? ElementType { get; set; } = "normal";

    public bool IsActive { get; set; } = true;

    public List<ParameterDropdownOptionDto>? DropdownOptions { get; set; }
}

public class ParameterUpdateDto
{
    [Required(ErrorMessage = "Parameter ID is required.")]
    public long ID { get; set; }

    [Required(ErrorMessage = "Parameter Code is required.")]
    [StringLength(100, ErrorMessage = "Parameter Code cannot exceed 100 characters.")]
    public string Code { get; set; } = string.Empty;

    [Required(ErrorMessage = "Parameter Name is required.")]
    [StringLength(200, ErrorMessage = "Parameter Name cannot exceed 200 characters.")]
    public string Name { get; set; } = string.Empty;

    [StringLength(50)]
    public string? Symbol { get; set; }

    public string? ParameterType { get; set; } = "Reported";

    [StringLength(20)]
    public string? InputType { get; set; } = "Decimal";

    public long? ParameterUnitID { get; set; }
    public long? UnitMasterId { get => ParameterUnitID; set => ParameterUnitID = value ?? ParameterUnitID; }
    public long? ParameterUnitEquivalentID { get; set; }
    public decimal? UnitConversionFactor { get; set; }

    [Range(0, 6, ErrorMessage = "Decimal precision must be between 0 and 6.")]
    public int DecimalPrecision { get; set; } = 2;
    public int? Precision { get => DecimalPrecision; set { if (value.HasValue) DecimalPrecision = value.Value; } }

    public string? CalculationRole { get; set; } = "Input";
    public bool IsCalculated { get; set; } = false;

    public string? Formula { get; set; }
    public string? FormulaDisplay { get; set; }

    public int? Sequence { get; set; }

    [StringLength(500)]
    public string? Description { get; set; }

    public string? ElementType { get; set; } = "normal";

    public bool IsActive { get; set; } = true;

    public List<ParameterDropdownOptionDto>? DropdownOptions { get; set; }
}

public class ParameterDropdownOptionDto
{
    public long ID { get; set; }
    public long ParameterID { get; set; }
    public string DisplayText { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public int DisplayOrder { get; set; } = 0;
    public bool IsDefault { get; set; } = false;
    public bool IsActive { get; set; } = true;
}
