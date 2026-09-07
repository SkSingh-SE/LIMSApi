using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using LIMSApi.Helpers.Enums;
using Microsoft.AspNetCore.Http;

namespace LIMSApi.Dtos
{
    public class SpecificationVersionFilterDto
    {
        public long? SpecificationHeaderID { get; set; }
        public string? Version { get; set; }
        public string? Year { get; set; }
        public VersionStatus? Status { get; set; }
        public bool? IsDefault { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SortColumn { get; set; } = "ID";
        public string? SortDirection { get; set; } = "desc";
    }

    public class SpecificationVersionListItemDto
    {
        public long ID { get; set; }
        public long SpecificationHeaderID { get; set; }
        public string SpecificationCode { get; set; } = string.Empty;
        public string SpecificationName { get; set; } = string.Empty;
        public string? StandardReference { get; set; }
        public string? StandardOrganizationName { get; set; }
        public string Version { get; set; } = string.Empty;
        public string? Year { get; set; }
        public VersionStatus Status { get; set; }
        public string StatusName => Status.ToString();
        public DateTime? EffectiveDate { get; set; }
        public DateTime? SupersededDate { get; set; }
        public DateTime? ReviewDate { get; set; }
        public string? ChangeReason { get; set; }
        public bool IsDefault { get; set; }
        public string? StandardFile { get; set; }
        public string? StandardFilePath { get; set; }
        public long? UploadReferenceID { get; set; }
        public int ParametersCount { get; set; }
        public bool IsParentActive { get; set; }
    }

    public class SpecificationVersionDetailDto : SpecificationVersionListItemDto
    {
        public DateTime CreatedOn { get; set; }
        public long CreatedBy { get; set; }
        public string? CreatedByName { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public long? ModifiedBy { get; set; }
        public string? ModifiedByName { get; set; }
        public string CompanyCode { get; set; } = "LIMS";
        public List<SpecificationVersionParameterDto> Parameters { get; set; } = new();
    }

    public class SpecificationVersionCreateDto
    {
        [Required(ErrorMessage = "Parent Specification is required.")]
        public long SpecificationHeaderID { get; set; }

        [Required(ErrorMessage = "Version / Edition is required.")]
        [StringLength(50, ErrorMessage = "Version cannot exceed 50 characters.")]
        public string Version { get; set; } = string.Empty;

        [StringLength(20, ErrorMessage = "Year cannot exceed 20 characters.")]
        public string? Year { get; set; }

        public VersionStatus Status { get; set; } = VersionStatus.Draft;

        public DateTime? EffectiveDate { get; set; }
        public DateTime? SupersededDate { get; set; }
        public DateTime? ReviewDate { get; set; }

        [StringLength(500, ErrorMessage = "Change reason cannot exceed 500 characters.")]
        public string? ChangeReason { get; set; }

        public bool IsDefault { get; set; }

        public IFormFile? File { get; set; }
    }

    public class SpecificationVersionUpdateDto
    {
        [Required(ErrorMessage = "Version ID is required.")]
        public long ID { get; set; }

        [Required(ErrorMessage = "Version / Edition is required.")]
        [StringLength(50, ErrorMessage = "Version cannot exceed 50 characters.")]
        public string Version { get; set; } = string.Empty;

        [StringLength(20, ErrorMessage = "Year cannot exceed 20 characters.")]
        public string? Year { get; set; }

        public VersionStatus Status { get; set; }

        public DateTime? EffectiveDate { get; set; }
        public DateTime? SupersededDate { get; set; }
        public DateTime? ReviewDate { get; set; }

        [StringLength(500, ErrorMessage = "Change reason cannot exceed 500 characters.")]
        public string? ChangeReason { get; set; }

        public bool IsDefault { get; set; }

        public IFormFile? File { get; set; }
    }

    public class SpecificationVersionParameterDto
    {
        public long ID { get; set; }

        [Required(ErrorMessage = "Parameter is required.")]
        public long ParameterID { get; set; }

        public string? ParameterCode { get; set; }
        public string? ParameterName { get; set; }

        public long? UnitID { get; set; }
        public string? UnitName { get; set; }
        public string? UnitSymbol { get; set; }

        public int SortOrder { get; set; } = 1;

        [StringLength(1000, ErrorMessage = "Comment cannot exceed 1000 characters.")]
        public string? Comment { get; set; }
    }

    public class SaveSpecificationVersionParametersDto
    {
        public List<SpecificationVersionParameterDto> Parameters { get; set; } = new();
    }

    public class SpecificationVersionDropdownDto
    {
        public long ID { get; set; }
        public string Version { get; set; } = string.Empty;
        public string? Year { get; set; }
        public bool IsDefault { get; set; }
        public VersionStatus Status { get; set; }
        public string DisplayText => string.IsNullOrWhiteSpace(Year)
            ? (IsDefault ? $"{Version} (Default)" : Version)
            : (IsDefault ? $"{Version} ({Year}) (Default)" : $"{Version} ({Year})");
    }
}
