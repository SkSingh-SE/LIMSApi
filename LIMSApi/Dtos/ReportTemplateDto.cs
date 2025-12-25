using System.Text.Json;

namespace LIMSApi.Dtos
{
    public class ReportTemplateDto
    {
        public long Id { get; set; }
        public string TemplateCode { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string TestType { get; set; } = string.Empty;
        public long? TestTypeID { get; set; }

        public int Version { get; set; }
        public bool IsLocked { get; set; }
        public bool IsDefault { get; set; }
        public bool IsActive { get; set; }

        public List<ReportTemplateBlockDto> Blocks { get; set; } = new();
    }
    public class ReportTemplateBlockDto
    {
        public long Id { get; set; }
        public string BlockType { get; set; } = string.Empty;
        public int SortOrder { get; set; }
        public JsonElement? Config { get; set; }

    }
}
