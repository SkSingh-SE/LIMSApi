namespace LIMSApi.Helpers.Enums
{
    public enum PaymentType
    {
        PIInvoice = 1,
        Invoice = 2,
        AmendmentInvoice = 3,
        Advance = 4
    }
    public enum PaymentStatus
    {
        Pending = 1,
        Paid = 2,
        Failed = 3,
        Expired = 4
    }
}
