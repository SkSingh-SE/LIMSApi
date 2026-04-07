namespace LIMSApi.Dtos
{
    /// <summary>
    /// Result of checking whether testing is allowed for a sample inward.
    /// Walk-in customers are hard-blocked until payment; Credit customers get warnings.
    /// </summary>
    public class PaymentGateResult
    {
        public bool IsBlocked { get; set; }
        public string? BlockReason { get; set; }
        public string CustomerType { get; set; } = string.Empty;
        public decimal? AmountDue { get; set; }
        public decimal? CreditLimit { get; set; }
        public decimal? OutstandingBalance { get; set; }

        /// <summary>
        /// Credit escalation level: None, Warning (80%), HardBlock (100%), AutoBlock (120%)
        /// </summary>
        public string? CreditEscalationLevel { get; set; }

        /// <summary>
        /// If true, a LabManager can override this block with documented justification
        /// </summary>
        public bool AllowOverride { get; set; }
    }

    /// <summary>
    /// Result of checking a customer's credit limit against their outstanding balance.
    /// </summary>
    public class CreditCheckResult
    {
        public bool IsOverLimit { get; set; }
        public decimal CreditLimit { get; set; }
        public decimal OutstandingBalance { get; set; }
        public decimal AvailableCredit { get; set; }
        public string? Warning { get; set; }
    }
}
