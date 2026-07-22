namespace LIMSApi.Dtos
{
    public class PurchaseMaterialVerificationItemDto
    {
        public string? MaterialName { get; set; }

        public decimal ReceviceQty { get; set; }

        public string? VerificationDetails { get; set; }

        public string? InspectionQtyStatus { get; set; }

        public string? VerificationDone { get; set; }
    }
}
