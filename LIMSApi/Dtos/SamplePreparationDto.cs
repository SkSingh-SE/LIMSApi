namespace LIMSApi.Dtos
{
    public class SamplePreparationListDto
    {
        public long Id { get; set; }
        public long SampleID { get; set; }
        public string SampleNo { get; set; } = string.Empty;
        public long InwardID { get; set; }
        public string CaseNo { get; set; } = string.Empty;
        public string? CustomerName { get; set; }
        public string? SampleDescription { get; set; }
        public string? MaterialClassification { get; set; }
        public string Status { get; set; } = "Pending";
        public string? AssignedToName { get; set; }
        public string? PreparedByName { get; set; }
        public DateTime? StartedOn { get; set; }
        public DateTime? CompletedOn { get; set; }

        // Charge totals
        public decimal CuttingChargesTotal { get; set; }
        public decimal MachiningChargesTotal { get; set; }
        public decimal OtherChargesTotal { get; set; }
        public decimal TotalCharges => CuttingChargesTotal + MachiningChargesTotal + OtherChargesTotal;

        // Flags
        public bool IsInvoiced { get; set; }
        public bool PreparationRequired { get; set; }
    }

    public class SamplePreparationDetailDto
    {
        public long Id { get; set; }
        public long SampleID { get; set; }
        public string SampleNo { get; set; } = string.Empty;
        public long InwardID { get; set; }
        public string CaseNo { get; set; } = string.Empty;
        public string? CustomerName { get; set; }
        public string? SampleDescription { get; set; }

        // Status & Workflow
        public string Status { get; set; } = "Pending";
        public long? AssignedToEmployeeID { get; set; }
        public string? AssignedToName { get; set; }
        public long? PreparedByEmployeeID { get; set; }
        public string? PreparedByName { get; set; }
        public long? VerifiedByEmployeeID { get; set; }
        public string? VerifiedByName { get; set; }

        // Timestamps
        public DateTime? AssignedOn { get; set; }
        public DateTime? StartedOn { get; set; }
        public DateTime? CompletedOn { get; set; }
        public DateTime? VerifiedOn { get; set; }

        // NABL
        public long? EquipmentID { get; set; }
        public string? EquipmentName { get; set; }
        public string? PreparationMethod { get; set; }
        public decimal? Temperature { get; set; }
        public decimal? Humidity { get; set; }

        // Notes
        public string? PreparationInstructions { get; set; }
        public string? PreConditionNotes { get; set; }
        public string? PostConditionNotes { get; set; }
        public string? VerificationRemarks { get; set; }

        // Charges
        public decimal CuttingChargesTotal { get; set; }
        public decimal MachiningChargesTotal { get; set; }
        public decimal OtherChargesTotal { get; set; }
        public decimal TotalCharges => CuttingChargesTotal + MachiningChargesTotal + OtherChargesTotal;

        // Sample info for context
        public decimal? Thickness { get; set; }
        public decimal? Diameter { get; set; }
        public decimal? Width { get; set; }
        public decimal? Length { get; set; }
        public bool IsInvoiced { get; set; }

        // Plan-level fields (auto-filled from SampleDetail)
        public bool PreparationRequired { get; set; }
        public bool MachiningRequired { get; set; }
        public decimal MachiningAmountFromPlan { get; set; }
        public bool OtherPreparation { get; set; }
        public decimal OtherPreparationCharge { get; set; }
        public string? Specimen { get; set; }
        public string? TestInstructions { get; set; }
    }

    public class SamplePreparationCreateDto
    {
        public long SampleID { get; set; }
        public long InwardID { get; set; }
        public long? AssignedToEmployeeID { get; set; }
        public long? EquipmentID { get; set; }
        public string? PreparationMethod { get; set; }
        public string? PreparationInstructions { get; set; }
        public string? PreConditionNotes { get; set; }
    }

    public class SamplePreparationUpdateDto
    {
        public long ID { get; set; }
        public string? Status { get; set; }
        public long? AssignedToEmployeeID { get; set; }
        public long? PreparedByEmployeeID { get; set; }
        public long? VerifiedByEmployeeID { get; set; }
        public long? EquipmentID { get; set; }
        public string? PreparationMethod { get; set; }
        public decimal? Temperature { get; set; }
        public decimal? Humidity { get; set; }
        public string? PreparationInstructions { get; set; }
        public string? PreConditionNotes { get; set; }
        public string? PostConditionNotes { get; set; }
        public string? VerificationRemarks { get; set; }
    }

    public class SamplePreparationStatusDto
    {
        public string Status { get; set; } = string.Empty;
        public string? Remarks { get; set; }
    }
}
