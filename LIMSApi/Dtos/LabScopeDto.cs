using System.ComponentModel.DataAnnotations;

namespace LIMSApi.Dtos
{
    public class ScopeListRequest
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SearchTerm { get; set; }
        public string? SortByColumn { get; set; }
        public string? SortOrder { get; set; }
        public long? LaboratoryTestID { get; set; }
        public long? BranchID { get; set; }
        public string Status { get; set; } = "active";
        public string Validity { get; set; } = "all";
        public string Accreditation { get; set; } = "all";
    }

    public class ScopeListItemDto
    {
        public long ID { get; set; }
        public long LaboratoryTestID { get; set; }
        public string LaboratoryTestName { get; set; } = string.Empty;
        public long? BranchID { get; set; }
        public string BranchName { get; set; } = string.Empty;
        public string MethodsSummary { get; set; } = string.Empty;
        public int ParameterCount { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidUntil { get; set; }
        public string ValidityState { get; set; } = string.Empty;
        public bool AccreditationCovered { get; set; }
        public bool IsActive { get; set; }
        public string CreatedByName { get; set; } = string.Empty;
        public DateTime CreatedOn { get; set; }
        public string ModifiedByName { get; set; } = string.Empty;
        public DateTime? ModifiedOn { get; set; }
    }

    public class ScopeParameterDto
    {
        public long ID { get; set; }
        public long ParameterID { get; set; }
        public string ParameterName { get; set; } = string.Empty;
        public long ParameterUnitID { get; set; }
        public string ParameterUnitName { get; set; } = string.Empty;
        [Required]
        public string ScopeType { get; set; } = "Quantitative";
        public bool IsUnderISO { get; set; }
        public string? LowerOperator { get; set; }
        public decimal? LowerLimitValue { get; set; }
        public string? UpperOperator { get; set; }
        public decimal? UpperLimitValue { get; set; }
        public List<long> EquipmentIDs { get; set; } = new();
        public List<string> EquipmentNames { get; set; } = new();
    }

    public class ScopeMethodDto
    {
        public long ID { get; set; }
        public long TestMethodSpecificationID { get; set; }
        public string TestMethodName { get; set; } = string.Empty;
        public long? TestMethodSpecificationVersionID { get; set; }
        public string? TestMethodVersion { get; set; }
        public List<ScopeParameterDto> Parameters { get; set; } = new();
    }

    public class AccreditationContextDto
    {
        public long? AccreditationID { get; set; }
        public string? CertificateNumber { get; set; }
        public DateTime? IssueDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string? LogoPath { get; set; }
        public bool IsEffective { get; set; }
    }

    public class ScopeChangeHistoryDto
    {
        public long ID { get; set; }
        public string ChangeType { get; set; } = string.Empty;
        public string? EntityName { get; set; }
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
        public long ChangedBy { get; set; }
        public string ChangedByName { get; set; } = string.Empty;
        public DateTime ChangedOn { get; set; }
    }

    public class ScopeDetailDto
    {
        public long ID { get; set; }
        public long LaboratoryTestID { get; set; }
        public string LaboratoryTestName { get; set; } = string.Empty;
        public long? BranchID { get; set; }
        public string BranchName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidUntil { get; set; }
        public DateTime? NextReviewDate { get; set; }
        public string? ScopeRemarks { get; set; }
        public string ValidityState { get; set; } = string.Empty;
        public List<ScopeMethodDto> Methods { get; set; } = new();
        public AccreditationContextDto Accreditation { get; set; } = new();
        public string CreatedByName { get; set; } = string.Empty;
        public DateTime CreatedOn { get; set; }
        public string ModifiedByName { get; set; } = string.Empty;
        public DateTime? ModifiedOn { get; set; }
        public string CompanyCode { get; set; } = string.Empty;
        public List<ScopeChangeHistoryDto> ChangeHistory { get; set; } = new();
    }

    public class ScopeCreateDto
    {
        [Required]
        public long LaboratoryTestID { get; set; }
        public long? BranchID { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidUntil { get; set; }
        public DateTime? NextReviewDate { get; set; }
        [MaxLength(500)]
        public string? ScopeRemarks { get; set; }
        public List<ScopeMethodDto> Methods { get; set; } = new();
    }

    public class ScopeUpdateDto : ScopeCreateDto
    {
        [Required]
        public long ID { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class ScopeValidateRequest
    {
        [Required]
        public long LaboratoryTestID { get; set; }
        public long? TestMethodSpecificationID { get; set; }
        public long? TestMethodSpecificationVersionID { get; set; }
        [Required]
        public long ParameterID { get; set; }
        public decimal? Value { get; set; }
        public long? BranchID { get; set; }
        public DateTime? ReferenceDate { get; set; }
    }

    public class ScopeDecisionDto
    {
        public string ScopeStatus { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public long? LabScopeID { get; set; }
        public long? LabScopeSpecParamId { get; set; }
        public string? MethodName { get; set; }
        public string? MethodVersion { get; set; }
        public decimal? ScopeLowerLimit { get; set; }
        public decimal? ScopeUpperLimit { get; set; }
        public bool AccreditationEffective { get; set; }
        public string? AccreditationCertificate { get; set; }
    }

    public class ScopePreviewRequest
    {
        [Required]
        public long LaboratoryTestID { get; set; }
        public long? BranchID { get; set; }
        public DateTime? ReferenceDate { get; set; }
    }

    public class ScopePreviewParameterDto
    {
        public long ParameterID { get; set; }
        public string ParameterName { get; set; } = string.Empty;
        public string ParameterUnitName { get; set; } = string.Empty;
        public string ScopeType { get; set; } = string.Empty;
        public string ScopeStatus { get; set; } = string.Empty;
        public long? LabScopeID { get; set; }
        public string? MethodName { get; set; }
        public string? MethodVersion { get; set; }
    }

    public class ScopePreviewDto
    {
        public long LaboratoryTestID { get; set; }
        public int InScopeCount { get; set; }
        public int OutOfScopeCount { get; set; }
        public List<ScopePreviewParameterDto> Parameters { get; set; } = new();
        public AccreditationContextDto Accreditation { get; set; } = new();
    }
}
