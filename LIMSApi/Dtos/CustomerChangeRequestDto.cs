namespace LIMSApi.Dtos
{
    /// <summary>
    /// Snapshot of all Level 2 fields — used as old/new values in a change request.
    /// Null on a field means "not part of this change".
    /// </summary>
    public class CustomerChangeValuesDto
    {
        public string? CustomerType { get; set; }
        public decimal? CreditLimitAmount { get; set; }
        public int? CreditLimitTime { get; set; }
        public bool? ConstantDiscount { get; set; }
        public decimal? ConstantDiscountPercentage { get; set; }
        public bool? WeeklyBillingCustomer { get; set; }
        public bool? MonthlyBillingCustomer { get; set; }
        public bool? BillingEvery { get; set; }
        public int? BillingEveryDays { get; set; }
    }

    public class CustomerChangeRequestResponseDto
    {
        public long ID { get; set; }
        public long CustomerID { get; set; }
        public CustomerChangeValuesDto OldValues { get; set; } = new();
        public CustomerChangeValuesDto NewValues { get; set; } = new();
        public string Status { get; set; } = string.Empty;
        public string? RejectionReason { get; set; }
        public string? ReviewedByName { get; set; }
        public DateTime? ReviewedOn { get; set; }
        public DateTime CreatedOn { get; set; }
        public string? RequestedByName { get; set; }
        public long? WorkflowInstanceID { get; set; }
    }

    public class ReviewChangeRequestDto
    {
        public long ChangeRequestId { get; set; }
        /// <summary>Approve or Reject</summary>
        public string Action { get; set; } = string.Empty;
        public string? Remarks { get; set; }
    }
}
