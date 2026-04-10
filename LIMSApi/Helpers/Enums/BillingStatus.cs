namespace LIMSApi.Helpers.Enums
{
    /// <summary>
    /// Global Billing Status Model (MANDATORY) - tracks billing lifecycle
    /// </summary>
    public enum BillingStatus
    {
        PRICE_DRAFTED,
        PI_GENERATED,
        ADVANCE_PAID,
        PRICE_SNAPSHOT,
        INVOICE_GENERATED,
        PAYMENT_PENDING,
        PAYMENT_PARTIAL,
        PAYMENT_COMPLETED
    }
}

