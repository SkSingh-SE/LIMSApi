namespace LIMSApi.Dtos
{
    public class CrmConsumptionLogDto
    {
        public long Id { get; set; }
        public long ReferenceMaterialId { get; set; }
        public DateTime ConsumptionDate { get; set; }
        public decimal QuantityConsumed { get; set; }
        public decimal PreviousBalanceQty { get; set; }
        public decimal BalanceQty { get; set; }
        public string? Purpose { get; set; }
        public string? EquipmentOrTest { get; set; }
        public string? UsedBy { get; set; }
        public string? Remarks { get; set; }
    }
}
