using System.Text.Json;

namespace LIMSApi.Dtos
{
    public class ReportFormatDto
    {
        public long Id { get; set; }
        public string FormatCode { get; set; } = string.Empty;
        public string FormatName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string PageLayout { get; set; } = "Portrait";
        public string PageSize { get; set; } = "A4";
        public JsonElement? HeaderConfig { get; set; }
        public bool IsDefault { get; set; }
        public bool IsLocked { get; set; }
        public bool IsActive { get; set; }
        public List<ReportFormatSectionDto> Sections { get; set; } = new();
        public List<ReportFormatMappingDto> Mappings { get; set; } = new();
    }

    public class ReportFormatSectionDto
    {
        public long Id { get; set; }
        public int SectionType { get; set; }
        public string SectionTypeName { get; set; } = string.Empty;
        public int SortOrder { get; set; }
        public JsonElement? Config { get; set; }
        public bool IsVisible { get; set; } = true;
    }

    public class ReportFormatMappingDto
    {
        public long Id { get; set; }
        public long? LaboratoryTestID { get; set; }
        public string? LaboratoryTestName { get; set; }
        public long? TestMethodID { get; set; }
        public string? TestMethodName { get; set; }
        public int Priority { get; set; }
    }

    public class GeneratedReportDto
    {
        public long Id { get; set; }
        public long ReportFormatID { get; set; }
        public string FormatName { get; set; } = string.Empty;
        public long SampleID { get; set; }
        public string SampleNo { get; set; } = string.Empty;
        public string ReportNo { get; set; } = string.Empty;
        public string? CertificateNo { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? PdfPath { get; set; }
        public string GeneratedBy { get; set; } = string.Empty;
        public DateTime GeneratedAt { get; set; }
    }
}
