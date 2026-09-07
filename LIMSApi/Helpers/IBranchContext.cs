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
    }
}
