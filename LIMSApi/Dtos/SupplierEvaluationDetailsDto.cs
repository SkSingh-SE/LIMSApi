namespace LIMSApi.Dtos
{
    public class SupplierEvaluationDetailsDto
    {
        public List<NablPurchaseOrderDto>? PurchaseOrders{ get; set; }
        public List<NablIncomingMaterialDto>? IncomingMaterials{ get; set; }
    }
}
