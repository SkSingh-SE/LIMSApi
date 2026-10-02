namespace LIMSApi.Dtos
{
    public class PurchaseMaterialVerificationPrintDto
    {
        public DateTime? Date { get; set; }

        public string? PONo { get; set; }

        public string? SupplierName { get; set; }

        public List<PurchaseMaterialVerificationItemDto> MaterialDetails { get; set; } = new();
    }
}
