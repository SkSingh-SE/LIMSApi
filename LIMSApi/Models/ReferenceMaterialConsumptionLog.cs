namespace LIMSApi.Models
{
    public class ReferenceMaterialConsumptionLog
    {
        public long Id { get; set; }

        // F-18 Header FK
        public long ReferenceMaterialConsumptionId { get; set; }

        // F-17 Reference Material FK
        public long? ReferenceMaterialId { get; set; }

        public DateTime ConsumptionDate { get; set; }

        public decimal QuantityConsumed { get; set; }

        public decimal PreviousBalanceQty { get; set; }

        public decimal BalanceQty { get; set; }

        public string Purpose { get; set; } = string.Empty;

        public string? EquipmentOrTest { get; set; }

        public string? UsedBy { get; set; }

        public string? Remarks { get; set; }

        // Audit
        public bool IsActive { get; set; } = true;
        public long? CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }

        // Navigation
        public NablCrmConsumption? ReferenceMaterialConsumption { get; set; }
        public NablReferenceMaterial? ReferenceMaterial { get; set; }
    }
}
