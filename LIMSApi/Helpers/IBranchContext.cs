namespace LIMSApi.Helpers
{
    public enum BranchAction
    {
        View,
        Create,
        Edit,
        Execute,
        Approve,
        Delete
    }

    /// <summary>
    /// Authoritative tenant context resolved from JWT + Branches table.
    /// NEVER falls back to a default organization/branch/company code.
    /// </summary>
    public class TenantContext
    {
        public long OrganizationID { get; set; }
        public long BranchID { get; set; }
        public string CompanyCode { get; set; } = string.Empty;
    }

    public interface IBranchContext
    {
        long? CurrentOrganizationID { get; }
        long? CurrentBranchID { get; }
        bool CanViewAllBranches { get; }
        IReadOnlyList<long> AuthorizedBranchIDs { get; }
        IReadOnlyList<long> AuthorizedExecutionBranchIDs { get; }

        long RequireCurrentBranchId();
        long RequireOrganizationId();
        bool IsAuthorizedForBranch(long branchId, BranchAction action = BranchAction.View);

        /// <summary>
        /// Resolves the authoritative tenant context for the given execution branch.
        /// Uses JWT (loggedInUser.OrganizationID + CompanyCode) and cross-validates against the Branches table.
        /// FAILS LOUD with InvalidOperationException if any tenant value cannot be resolved.
        /// </summary>
        TenantContext ResolveTenantContext(long executionBranchId);
    }
}
