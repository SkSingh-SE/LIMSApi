using System.ComponentModel.DataAnnotations;

namespace LIMSApi.Dtos
{
    public class TestMethodSpecificationDto
    {
        public long ID { get; set; }
        public long StandardOrganizationID { get; set; }
        public string TestMethodStandard { get; set; } = string.Empty;
        public required string Name { get; set; }
        public string? Part { get; set; }
        public string? DisplayTitle { get; set; }
        public bool IsDisabled { get; set; } = false;
        public string? LinkedStandard { get; set; }
        public string? FormulaExpression { get; set; }
        public string? DefaultParameters { get; set; }
        public string Versions { get; set; }
        // JSON array of metal classification IDs: e.g. "[1,2,3]"
        public string? MetalClassificationIDs { get; set; }
    }

    public class TestMethodSpecParameterDto
    {
        public long ID { get; set; }
        public long ParameterID { get; set; }
        public long? ParameterUnitID { get; set; }
        public long? ParameterUnitEquivalentID { get; set; }
        public string? Comment { get; set; }
        public int SortOrder { get; set; }
    }
    public class VersionDto
    {
        public long ID { get; set; }
        public long TestMethodSpecificationID { get; set; }
        public int Status { get; set; }
        public string Version { get; set; }
        public string? Year { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public DateTime? SupersededDate { get; set; }
        public DateTime? ReviewDate { get; set; }
        public string? ChangeReason { get; set; }
        public string StandardFile { get; set; }
        public string StandardFilePath { get; set; }
        public long? UploadReferenceID { get; set; }
        // True if this version is the spec's default (auto-selected in inward/plan).
        public bool IsDefault { get; set; }
        // Parameters this version reports (version-level).
        public List<TestMethodSpecParameterDto>? Parameters { get; set; }
    }

    public class SetDefaultVersionDto
    {
        public long SpecificationId { get; set; }
        public long VersionId { get; set; }
    }

    public class VersionActionDto
    {
        public long SpecificationId { get; set; }
        public long VersionId { get; set; }
    }

    public class VersionWithdrawDto
    {
        public long SpecificationId { get; set; }
        public long VersionId { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}
