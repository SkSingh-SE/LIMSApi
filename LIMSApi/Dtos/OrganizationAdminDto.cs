using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LIMSApi.Dtos
{
    public class OrganizationAdminListRequestDto
    {
        public int PageNo { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SearchTerm { get; set; }
        public bool? IsActive { get; set; }
    }

    public class OrganizationAdminItemDto
    {
        public long Id { get; set; }
        public string LabName { get; set; } = string.Empty;
        public string LabCode { get; set; } = string.Empty;
        public string LabAddress { get; set; } = string.Empty;
        public string ContactEmail { get; set; } = string.Empty;
        public string ContactPhone { get; set; } = string.Empty;
        public string? OrganizationLogo { get; set; }
        public string? CIN { get; set; }
        public string? Website { get; set; }
        public string? MobileNo { get; set; }
        public string? UlrPrefix { get; set; }
        public string? LabLocationCode { get; set; }
        public bool IsMultiBranch { get; set; }
        public bool IsActive { get; set; }
        public int BranchCount { get; set; }
        public int ActiveUserCount { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
    }

    public class OrganizationAdminListResponseDto
    {
        public int TotalCount { get; set; }
        public List<OrganizationAdminItemDto> Items { get; set; } = new();
    }

    public class OrganizationCreateDto
    {
        [Required, StringLength(200)]
        public string LabName { get; set; } = null!;

        [Required, StringLength(50)]
        public string LabCode { get; set; } = null!;

        [Required, StringLength(500)]
        public string LabAddress { get; set; } = null!;

        [Required, StringLength(200), EmailAddress]
        public string ContactEmail { get; set; } = null!;

        [Required, StringLength(50)]
        public string ContactPhone { get; set; } = null!;

        [StringLength(500)]
        public string? OrganizationLogo { get; set; }

        [StringLength(100)]
        public string? CIN { get; set; }

        [StringLength(100)]
        public string? Website { get; set; }

        [StringLength(50)]
        public string? MobileNo { get; set; }

        [StringLength(50)]
        public string? UlrPrefix { get; set; }

        [StringLength(20)]
        public string? LabLocationCode { get; set; }

        public bool IsMultiBranch { get; set; } = false;
    }

    public class OrganizationUpdateDto
    {
        [Required]
        public long Id { get; set; }

        [Required, StringLength(200)]
        public string LabName { get; set; } = null!;

        [Required, StringLength(50)]
        public string LabCode { get; set; } = null!;

        [Required, StringLength(500)]
        public string LabAddress { get; set; } = null!;

        [Required, StringLength(200), EmailAddress]
        public string ContactEmail { get; set; } = null!;

        [Required, StringLength(50)]
        public string ContactPhone { get; set; } = null!;

        [StringLength(500)]
        public string? OrganizationLogo { get; set; }

        [StringLength(100)]
        public string? CIN { get; set; }

        [StringLength(100)]
        public string? Website { get; set; }

        [StringLength(50)]
        public string? MobileNo { get; set; }

        [StringLength(50)]
        public string? UlrPrefix { get; set; }

        [StringLength(20)]
        public string? LabLocationCode { get; set; }

        public bool IsMultiBranch { get; set; } = false;
    }

    public class OrganizationDependencyDto
    {
        public long OrganizationId { get; set; }
        public string LabName { get; set; } = string.Empty;
        public int ActiveBranchesCount { get; set; }
        public int ActiveUsersCount { get; set; }
        public int OpenSamplesCount { get; set; }
        public bool CanDeactivate { get; set; } = true;
        public string? BlockReason { get; set; }
    }
}
