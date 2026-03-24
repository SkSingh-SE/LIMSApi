namespace LIMSApi.Helpers.Enums
{
    /// <summary>
    /// Status values for ReportHeader.Status field.
    /// Use these constants instead of raw strings to prevent typos.
    /// </summary>
    public static class ReportStatus
    {
        public const string Pending = "Pending";
        public const string ReportGenerated = "Report Generated";
        public const string Completed = "Completed";
        public const string UnderAmendmentReview = "Under Amendment Review";
        public const string SendBack = "Send Back";
        public const string Rejected = "Rejected";
        public const string Approved = "Approved";
    }
}
