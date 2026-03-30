using LIMSApi.Models;

namespace LIMSApi.Dtos
{
    public class SettingsDto
    {
    }
    public sealed class OrganizationDto
    {
        public long? Id { get; set; }
        public string LabName { get; set; } = string.Empty;
        public string LabCode { get; set; } = string.Empty;
        public string LabAddress { get; set; } = string.Empty;
        public string ContactEmail { get; set; } = string.Empty;
        public string ContactPhone { get; set; } = string.Empty;
        public string? OrganizationLogo { get; set; }
        public string? CIN { get; set; }
        public string? Website { get; set; }
        public string? MobileNo { get; set; }
        public string? UlrPrefix { get; set; }
        public string? LabLocationCode { get; set; }
    }
    public sealed class NablDto
    {
        public bool NablEnabled { get; set; }
        public string? NablTcNumber { get; set; }
        public string? NablCertificate { get; set; }
        public string? NablLogo { get; set; }
    }
    public sealed class NumberingDto
    {
        public string TcBaseNumber { get; set; } = string.Empty;
        public string ReportNumberPrefix { get; set; } = string.Empty;
        // system generated / read-only fields used by frontend UI
        public string? YearCode { get; set; }
        public int? RunningCounter { get; set; }
    }
    public sealed class GstDto
    {
        public bool GstApplicable { get; set; }
        public string? Gstin { get; set; }
        public string? PanNumber { get; set; }
        public string? StateCode { get; set; }
        public int DefaultGstRate { get; set; }
        public bool PIGstApplicable { get; set; }
    }
    public sealed class FinancialYearDto
    {
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
    }
    public sealed class SignatoryDto
    {
        public long? Id { get; set; }
        public string SignatoryName { get; set; } = string.Empty;
        public string Designation { get; set; } = string.Empty;
        public bool ApplicableFor { get; set; }
        public string? SignatureImage { get; set; }
        public bool Status { get; set; }
    }
    public sealed class SettingsSaveAllDto
    {
        public OrganizationDto OrganizationInfo { get; set; } = new();
        public NablDto NablAccreditation { get; set; } = new();
        public NumberingDto Numbering { get; set; } = new();
        public GstDto GstConfig { get; set; } = new();
        public FinancialYearDto FinancialYear { get; set; } = new();
        public List<SignatoryDto> Signatories { get; set; } = new();
    }

    // Requests received from frontend (DTOs) - map in controller to EF entities
    public sealed class SaveAllRequest
    {
        public SettingsSaveAllDto Payload { get; set; } = new();
    }
    public sealed class SaveSignatoriesRequest
    {
        public SignatoryDto[] Signatories { get; set; } = [];
    }
}
