namespace LIMSApi.Dtos
{
    /// <summary>
    /// One year-version of an InvoiceCase for a LaboratoryTest, with its price rows.
    /// </summary>
    public class InvoiceCaseVersionDto
    {
        public long ID { get; set; }
        public DateTime EffectiveFrom { get; set; }
        public long? FinancialYearId { get; set; }
        public string? FinancialYear { get; set; }
        public string? DefaultPricingType { get; set; }
        public bool IsCurrentFy { get; set; }
        public List<InvoiceCasePriceDto> Prices { get; set; } = new();
    }

    public class InvoiceCasePriceDto
    {
        public long ID { get; set; }
        public long InvoiceCaseConfigID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string AliasName { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }

    /// <summary>
    /// Payload for POST /save-all — all year versions for a single LaboratoryTest.
    /// </summary>
    public class SaveAllInvoiceCaseDto
    {
        public long LaboratoryTestID { get; set; }
        public List<InvoiceCaseVersionDto> Versions { get; set; } = new();
    }

    /// <summary>
    /// Response for GET /by-lab-test/{labTestId} — all year versions for a LaboratoryTest.
    /// </summary>
    public class InvoiceCaseByLabTestDto
    {
        public long LaboratoryTestID { get; set; }
        public string? LaboratoryTest { get; set; }
        public List<InvoiceCaseVersionDto> Versions { get; set; } = new();
    }
}
