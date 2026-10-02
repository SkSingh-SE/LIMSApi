namespace LIMSApi.Dtos
{
    public class NablIncomingMaterialDto
    {
        public long Id { get; set; }
        public string PurchaseOrderNo { get; set; } = "";
        public string SupplierName { get; set; } = "";
        public string InspectionPlanNoName { get; set; } = "";
        public DateTime Date { get; set; }
        public string InspectionResult { get; set; } = ""; 
    }
}
