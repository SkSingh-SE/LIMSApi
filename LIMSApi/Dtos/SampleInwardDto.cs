using System.ComponentModel.DataAnnotations;
using LIMSApi.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Dtos
{
    public class SampleInwardDto
    {

        public long ID { get; set; }
        public string CaseNo { get; set; } = string.Empty;

        // Customer + Address
        public long CustomerID { get; set; }
        public string? CustomerName { get; set; }
        public string Address { get; set; } = string.Empty;
        public string Area { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string PinCode { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string GstNo { get; set; } = string.Empty;

        // PO Linkage
        public long? PurchaseOrderId { get; set; }

        // Flags
        public decimal AdvancePayment { get; set; }
        public bool BillRequired { get; set; }
        public bool AdvancePIRequired { get; set; }
        public bool HoldTesting { get; set; }
        public bool HoldTestingUntilPIApproved { get; set; }
        public bool Urgent { get; set; }
        public bool ReturnSample { get; set; }
        public bool NotDestroyed { get; set; }

        public string? SampleReceiptNote { get; set; }
        public string? StatementOfConformity { get; set; } = "Not Applicable";
        public string? DecisionRule { get; set; } = "Not Applicable";

        public string? RequestFilePath { get; set; }
        public string? RequestFileName { get; set; }
        public long? UploadReferenceID { get; set; } = null;
        public string Status { get; set; } = "Sample Received";
        public bool IsReportStopped { get; set; } = false;
        public string? StopReportReason { get; set; }
        public DateTime CollectionTime { get; set; } = DateTime.Now;
        public string ReviewStatus { get; set; } = "Pending";
        public long? ReviewedBy { get; set; }
        public DateTime? ReviewedOn { get; set; }
        // Navigation Properties
        public virtual ICollection<DispatchModeDto> DispatchModes { get; set; } = new List<DispatchModeDto>();
        public virtual ICollection<ContactDto> Contacts { get; set; } = new List<ContactDto>();
        //public virtual ICollection<SampleInwardAddressInfo> Addresses { get; set; }
        //= new List<SampleInwardAddressInfo>();
        public PartyAddressDto? ReportingTo { get; set; }
        public PartyAddressDto? BillingTo { get; set; }
        public virtual ICollection<SampleDetailDto> SampleDetails { get; set; } = new List<SampleDetailDto>();
        public virtual ICollection<SampleAdditionalDetailDto> SampleAdditionalDetails { get; set; } = new List<SampleAdditionalDetailDto>();
        public ICollection<SampleTestPlanDto> SampleTestPlans { get; set; } = new List<SampleTestPlanDto>();

        [NotMapped]
        public IFormFile? File { get; set; } = null!;

        public bool CanTakeAction { get; set; } = false;
        public List<ActionDto> Actions { get; set; } = new();
    }

    public class DispatchModeDto
    {
        public long ID { get; set; }

        public long InwardID { get; set; }
        public long DispatchModeID { get; set; }
    }

    public class ContactDto
    {
        public long ID { get; set; }
        public bool Selected { get; set; }
        public long ContactID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string MobileNo { get; set; } = string.Empty;
        public string EmailId { get; set; } = string.Empty;
        public bool SendBill { get; set; }
        public bool SendReport { get; set; }
        public long InwardID { get; set; }
    }

    public class PartyAddressDto
    {
        public long ID { get; set; }
        public long? ContactPersonID { get; set; }
        public string? ContactPersonName { get; set; } = string.Empty;
        public string? Address { get; set; } = string.Empty;
        public string? PinCode { get; set; } = string.Empty;
        public string? Area { get; set; } = string.Empty;
        public string? City { get; set; } = string.Empty;
        public string? State { get; set; } = string.Empty;
        public string? Country { get; set; } = string.Empty;
        public string? Type { get; set; } = string.Empty;
        public long InwardID { get; set; }
        public string? EmailId { get; set; } = string.Empty;
        public string? MobileNo { get; set; } = string.Empty;
        public long? CustomerID { get; set; }
    }

    public class SampleDetailDto
    {
        public long ID { get; set; }
        public string SampleNo { get; set; }
        public string Details { get; set; }
        public long? MetalClassificationID { get; set; }
        public string? MetalClassificationName { get; set; }
        public long? ProductConditionID { get; set; }
        public string? ProductConditionName { get; set; }
        // UI hidden per client requirement — retained for data integrity
        public long? ProductFormID { get; set; }
        // UI hidden per client requirement — retained for data integrity
        public long? SpecimenOrientationID { get; set; }
        public string? Remarks { get; set; }
        public int Quantity { get; set; }
        public bool IsCancelled { get; set; }
        public string? SampleStatus { get; set; }
        public DateTime? CancelledOn { get; set; }
        public long? CancelledBy { get; set; }
        public string? CancellationReason { get; set; }

        // Prep flags
        public bool PreparationRequired { get; set; }
        public bool MachiningRequired { get; set; }
        public decimal? MachiningAmount { get; set; }
        public bool OtherPreparation { get; set; }
        public decimal? OtherPreparationCharge { get; set; }
        public bool TpiRequired { get; set; }
        public long? TpiAgencyID { get; set; }
        public string? Specimen { get; set; }
        public string? TestInstructions { get; set; }

        // TPI joined fields (for display in review screens)
        public string? TpiAgencyName { get; set; }
        public string? TpiEmailId { get; set; }
        public string? TpiContactNo { get; set; }

        // File
        public long? UploadReferenceID { get; set; }
        public string? SampleFilePath { get; set; }
        public string? FileName { get; set; }
        public IFormFile? File { get; set; }
        public long InwardID { get; set; }

        // Physical Dimensions
        public decimal? Thickness { get; set; }
        public decimal? Diameter { get; set; }
        public decimal? Width { get; set; }
        public decimal? Length { get; set; }

        // Product Master & Size Cascade linkage
        public long? ProductMasterID { get; set; }
        public string? ProductMasterName { get; set; }
        public long? ProductSizeMasterID { get; set; }
        public long? ProductSizeID { get; set; }
        public string? ProductSizeName { get; set; }
        public long? SpecificationGradeID { get; set; }
        public bool IsUnknownSample { get; set; }
        public long? AssignedGradeID { get; set; }
        public string? AssignedGradeName { get; set; }
        public string? AssignedGradeNote { get; set; }

        public ICollection<SampleAdditionalDetailDto> AdditionalDetails { get; set; } = new List<SampleAdditionalDetailDto>();
        public ICollection<SampleTestPlanDto> TestPlans { get; set; } = new List<SampleTestPlanDto>();
    }

    public class SampleAdditionalDetailDto
    {
        public long ID { get; set; }
        public string SampleNo { get; set; }
        public string Label { get; set; }
        public string Value { get; set; }
        public long SampleID { get; set; }
    }

    public class SampleTestPlanDto
    {
        public long ID { get; set; }
        public string SampleNo { get; set; }
        public long SampleID { get; set; }
        public int Version { get; set; } = 1;
        public int ReplanCount { get; set; } = 0;
        public string PlanStatus { get; set; } = "Draft";
        public long? ApprovedById { get; set; }
        public string? ApprovedByName { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public List<GeneralTestDto> GeneralTests { get; set; } = new();
        public List<ChemicalTestDto> ChemicalTests { get; set; } = new();
    }

    public class GeneralTestDto
    {
        public long ID { get; set; }
        public long SampleTestPlanID { get; set; }
        public string? SampleNo { get; set; }
        public long? Specification1 { get; set; }
        public long? Specification2 { get; set; }
        public long? LaboratoryTestSubGroupID { get; set; }
        public string? SubGroupName { get; set; }
        public List<GeneralTestMethodDto> Methods { get; set; } = new();
    }

    public class GeneralTestMethodDto
    {
        public long ID { get; set; }
        public long GeneralTestID { get; set; }
        public long? TestMethodID { get; set; }
        public long? TestCaseID { get; set; }
        public int Quantity { get; set; }
        public string? ReportNo { get; set; }
        public string? UlrNo { get; set; }
        public bool Cancel { get; set; }
        public long? StandardID { get; set; }
        public string? StandardName { get; set; }
    }

    public class ChemicalTestMethodDto
    {
        public long ID { get; set; }
        public long ChemicalTestID { get; set; }
        public long? TestMethodID { get; set; }
        public long? LaboratoryTestAnalysisTypeID { get; set; }
        public string? AnalysisTypeName { get; set; }
        public List<string> TechniqueCodes { get; set; } = new();
        public List<string> TechniqueNames { get; set; } = new();
        public long? TestMethodSpecificationID { get; set; }
        public long? StandardID { get; set; }
        public string? StandardName { get; set; }
        public int Quantity { get; set; } = 1;
        public string? ReportNo { get; set; }
        public string? UlrNo { get; set; }
        public bool Cancel { get; set; }
    }

    public class ChemicalTestDto
    {
        public long ID { get; set; }
        public long SampleTestPlanID { get; set; }
        public string? SampleNo { get; set; }
        public string? ReportNo { get; set; } = "";
        public string? UlrNo { get; set; } = "";
        public List<long> AnalysisTypeIds { get; set; } = new();
        public List<long> TestTypeIds
        {
            get => AnalysisTypeIds;
            set => AnalysisTypeIds = value ?? new();
        }
        public List<string> TechniqueCodes { get; set; } = new();
        public List<string> TechniqueNames { get; set; } = new();
        public long? LaboratoryTestAnalysisTypeID { get; set; }
        public string? AnalysisTypeName { get; set; }
        public long? MetalClassificationID { get; set; }
        public string? MetalClassificationName { get; set; }
        public long? Specification1 { get; set; }
        public long? Specification2 { get; set; }
        public List<ChemicalTestMethodDto> Methods { get; set; } = new();
        public List<ChemicalTestElementDto> Elements { get; set; } = new();
    }

    public class ChemicalTestElementDto
    {
        public long ID { get; set; }
        public long ChemicalTestID { get; set; }
        public long ParameterID { get; set; }
        public string? ParameterName { get; set; }
        public long? SpecificationLineID { get; set; }
        public long? LaboratoryTestAnalysisTypeID { get; set; }
        public string? LaboratoryTestAnalysisTypeName { get; set; }
        public string? AnalysisTypeName
        {
            get => LaboratoryTestAnalysisTypeName;
            set => LaboratoryTestAnalysisTypeName = value;
        }
        public string? SourceType { get; set; } // "Specification", "AnalysisType", "MachineSpectro"
        public long ParameterUnitID { get; set; }
        public string ParameterUnit { get; set; } = "";
        public decimal? MinValue { get; set; }
        public decimal? MaxValue { get; set; }
        public bool Selected { get; set; } = true;
    }

    public enum SampleWorkflowStatus
    {
        INWARD_REGISTERED,
        INWARD_VERIFIED,
        PLAN_DRAFT,
        PLAN_SUBMITTED,
        TECHNICAL_REVIEW,
        QUALITY_REVIEW,
        AWAITING_L1_APPROVAL,
        APPROVED_L1,
        AWAITING_L2_APPROVAL,
        APPROVED_L2,
        FINAL_APPROVED,
        REJECTED,
        RETURNED_TO_ORIGIN,
        PI_GENERATION_PENDING,
        PI_GENERATED,
        WORK_ASSIGNED,
        TESTING_IN_PROGRESS,
        TESTING_COMPLETED,
        ARCHIVED
    }

    public class ActionDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Action { get; set; }
    }

    public class PaymentInfoDto
    {
        public long? PurchaseOrderId { get; set; }
        public decimal AdvancePayment { get; set; }
        public bool BillRequired { get; set; }
        public bool AdvancePIRequired { get; set; }
        public bool HoldTestingUntilPIApproved { get; set; }
    }

    public class SamplePrepReviewDto
    {
        public bool PreparationRequired { get; set; }
        public bool MachiningRequired { get; set; }
        public decimal MachiningAmount { get; set; }
        public string? Specimen { get; set; }
        public bool OtherPreparation { get; set; }
        public decimal OtherPreparationCharge { get; set; }
        public string? TestInstructions { get; set; }
        public bool TpiRequired { get; set; }
        public long? TpiAgencyID { get; set; }
    }
}


