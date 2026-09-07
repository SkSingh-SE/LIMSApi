using System.Collections.Generic;

namespace LIMSApi.Dtos
{
    public class UserBranchDetailDto
    {
        public long Id { get; set; }
        public long BranchId { get; set; }
        public string BranchCode { get; set; } = string.Empty;
        public string BranchName { get; set; } = string.Empty;
        public bool IsHeadOffice { get; set; }
        public bool IsDefault { get; set; }
        public bool CanView { get; set; } = true;
        public bool CanCreate { get; set; } = true;
        public bool CanEdit { get; set; } = true;
        public bool CanExecute { get; set; } = true;
        public bool CanApprove { get; set; } = false;
        public bool CanDelete { get; set; } = false;
        public bool IsActive { get; set; } = true;
    }

    public class UserBranchAccessDto
    {
        public long UserId { get; set; }
        public long EmployeeId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public long? OrganizationId { get; set; }
        public string OrganizationName { get; set; } = string.Empty;
        public long? DefaultBranchId { get; set; }
        public string DefaultBranchName { get; set; } = string.Empty;
        public bool CanViewAllBranches { get; set; }
        public List<UserBranchDetailDto> AssignedBranches { get; set; } = new();
        public List<DropdwonSelector> AvailableBranches { get; set; } = new();
    }

    public class UpdateUserBranchItemDto
    {
        public long BranchId { get; set; }
        public bool IsDefault { get; set; }
        public bool CanView { get; set; } = true;
        public bool CanCreate { get; set; } = true;
        public bool CanEdit { get; set; } = true;
        public bool CanExecute { get; set; } = true;
        public bool CanApprove { get; set; } = false;
        public bool CanDelete { get; set; } = false;
    }

    public class UpdateUserBranchAccessDto
    {
        public long? OrganizationId { get; set; }
        public bool CanViewAllBranches { get; set; }
        public long? DefaultBranchId { get; set; }
        public List<UpdateUserBranchItemDto> Branches { get; set; } = new();
    }

    public class SetDefaultBranchDto
    {
        public long UserId { get; set; }
        public long BranchId { get; set; }
    }
}
