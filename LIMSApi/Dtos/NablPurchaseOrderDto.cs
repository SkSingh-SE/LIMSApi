namespace LIMSApi.Dtos
{
    public class NablPurchaseOrderDto
    {
        public long Id { get; set; }
        public string PONo { get; set; } = "";
        public DateTime? PODate { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public string SupplierName { get; set; } = "";
        public string ReferenceIndentNo { get; set; } = "";
    }
}
