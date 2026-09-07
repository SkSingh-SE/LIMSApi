using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LIMSApi.Dtos
{
    public class ParameterUnitEquivalentDto
    {
        public long ID { get; set; }
        public long BaseParameterUnitID { get; set; }

        [Required(ErrorMessage = "Equivalent Name is required.")]
        [StringLength(50, ErrorMessage = "Equivalent Name cannot exceed 50 characters.")]
        public string Name { get; set; } = string.Empty;

        [Range(0.000001, 1000000000, ErrorMessage = "Conversion factor must be greater than 0.")]
        public decimal? ConversionFactor { get; set; }

        public int? DisplayOrder { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class ParameterUnitListItemDto
    {
        public long ID { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Symbol { get; set; } = string.Empty;
        public string? QuantityType { get; set; }
        public string? Description { get; set; }
        public decimal? ConversionFactor { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string? CreatedByName { get; set; }
        public List<ParameterUnitEquivalentDto> Equivalents { get; set; } = new();
    }

    public class ParameterUnitDetailDto
    {
        public long ID { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Symbol { get; set; } = string.Empty;
        public string? QuantityType { get; set; }
        public string? Description { get; set; }
        public decimal? ConversionFactor { get; set; }
        public bool IsActive { get; set; }
        public List<ParameterUnitEquivalentDto> Equivalents { get; set; } = new();
    }

    public class ParameterUnitCreateDto
    {
        [Required(ErrorMessage = "Unit Code is required.")]
        [StringLength(50, ErrorMessage = "Unit Code cannot exceed 50 characters.")]
        public string Code { get; set; } = string.Empty;

        [Required(ErrorMessage = "Unit Name is required.")]
        [StringLength(100, ErrorMessage = "Unit Name cannot exceed 100 characters.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Symbol is required.")]
        [StringLength(50, ErrorMessage = "Symbol cannot exceed 50 characters.")]
        public string Symbol { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "Quantity Type cannot exceed 50 characters.")]
        public string? QuantityType { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string? Description { get; set; }

        public decimal? ConversionFactor { get; set; } = 1.0m;

        public bool IsActive { get; set; } = true;

        public List<ParameterUnitEquivalentDto> Equivalents { get; set; } = new();
    }

    public class ParameterUnitUpdateDto
    {
        [Required(ErrorMessage = "Unit ID is required.")]
        public long ID { get; set; }

        [Required(ErrorMessage = "Unit Code is required.")]
        [StringLength(50, ErrorMessage = "Unit Code cannot exceed 50 characters.")]
        public string Code { get; set; } = string.Empty;

        [Required(ErrorMessage = "Unit Name is required.")]
        [StringLength(100, ErrorMessage = "Unit Name cannot exceed 100 characters.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Symbol is required.")]
        [StringLength(50, ErrorMessage = "Symbol cannot exceed 50 characters.")]
        public string Symbol { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "Quantity Type cannot exceed 50 characters.")]
        public string? QuantityType { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string? Description { get; set; }

        public decimal? ConversionFactor { get; set; } = 1.0m;

        public bool IsActive { get; set; } = true;

        public List<ParameterUnitEquivalentDto> Equivalents { get; set; } = new();
    }
}
