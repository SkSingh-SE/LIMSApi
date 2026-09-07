using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LIMSApi.Dtos
{
    public class BranchAdminListRequestDto
    {
        [Required]
        public long OrganizationId { get; set; }
        public int PageNo { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SearchTerm { get; set; }
        public bool? IsActive { get; set; }
    }

    public class BranchAdminItemDto
    {
        public long ID { get; set; }
        public long OrganizationID { get; set; }
        public string OrganizationName { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public bool IsHeadOffice { get; set; }
        public string? Address { get; set; }
        public string? ContactEmail { get; set; }
        public string? ContactPhone { get; set; }
        public bool IsActive { get; set; }
        public int DisciplineCount { get; set; }
        public int AssignedUserCount { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
    }

    public class BranchAdminListResponseDto
    {
        public int TotalCount { get; set; }
        public List<BranchAdminItemDto> Items { get; set; } = new();
    }

    public class BranchCreateDto
    {
        [Required]
        public long OrganizationID { get; set; }

        [Required, StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required, StringLength(50)]
        public string Code { get; set; } = string.Empty;

        public bool IsHeadOffice { get; set; } = false;

        [StringLength(500)]
        public string? Address { get; set; }

        [StringLength(200), EmailAddress]
        public string? ContactEmail { get; set; }

        [StringLength(50)]
        public string? ContactPhone { get; set; }
    }

    public class BranchUpdateDto
    {
        [Required]
        public long ID { get; set; }

        [Required]
        public long OrganizationID { get; set; }

        [Required, StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required, StringLength(50)]
        public string Code { get; set; } = string.Empty;

        public bool IsHeadOffice { get; set; } = false;

        [StringLength(500)]
        public string? Address { get; set; }

        [StringLength(200), EmailAddress]
        public string? ContactEmail { get; set; }

        [StringLength(50)]
        public string? ContactPhone { get; set; }

        public bool IsActive { get; set; } = true;
    }

    public class BranchDependencyDto
    {
        public long BranchId { get; set; }
        public string BranchName { get; set; } = string.Empty;
        public int ActiveUsersCount { get; set; }
        public int DefaultUsersCount { get; set; }
        public int EquipmentCount { get; set; }
        public int DepartmentsCount { get; set; }
        public int OpenSamplesCount { get; set; }
        public bool CanDeactivate { get; set; } = true;
        public string? BlockReason { get; set; }
    }
}
