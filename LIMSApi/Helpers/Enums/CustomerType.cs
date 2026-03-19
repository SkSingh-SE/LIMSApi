namespace LIMSApi.Helpers.Enums
{
    /// <summary>
    /// Defines the accounting classification for customers.
    /// Maps to Customer.CustomerType string field values.
    /// </summary>
    public enum CustomerType
    {
        WalkIn = 1,
        Credit = 2,
        PurchaseOrder = 3
    }

    /// <summary>
    /// String constants matching the values stored in Customer.CustomerType.
    /// Use these for consistent comparisons throughout the codebase.
    /// </summary>
    public static class CustomerTypeConstants
    {
        public const string WalkIn = "Walk-in";
        public const string Credit = "Credit";
        public const string PurchaseOrder = "PurchaseOrder";

        public static bool IsWalkIn(string? customerType)
            => string.Equals(customerType, WalkIn, StringComparison.OrdinalIgnoreCase);

        public static bool IsCredit(string? customerType)
            => string.Equals(customerType, Credit, StringComparison.OrdinalIgnoreCase);

        public static bool IsPurchaseOrder(string? customerType)
            => string.Equals(customerType, PurchaseOrder, StringComparison.OrdinalIgnoreCase);
    }
}
