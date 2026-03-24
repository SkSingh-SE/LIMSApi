namespace LIMSApi.Helpers.Enums
{
    /// <summary>
    /// Status values for TestResultHeader.Status field.
    /// Use these constants instead of raw strings to prevent typos.
    /// </summary>
    public static class TestResultStatus
    {
        public const string Pending = "Pending";
        public const string InProgress = "In-Progress";
        public const string Started = "Started";
        public const string Completed = "Completed";
        public const string PendingVerification = "PendingVerification";
        public const string Verified = "Verified";
        public const string VerificationRejected = "VerificationRejected";
        public const string LongTerm = "Long-Term";
    }
}
