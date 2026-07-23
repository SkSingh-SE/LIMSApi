namespace LIMSApi.Dtos
{
    public class ImportTestMethodSpecItemDto
    {
        public int RowNumber { get; set; }
        public string StandardOrganization { get; set; } = string.Empty;
        public string TestMethodStandard { get; set; } = string.Empty;
        public string? Part { get; set; }
        public string OfficialTitle { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public string Year { get; set; } = string.Empty;
    }

    public class ImportValidationResultDto
    {
        public int RowNumber { get; set; }
        public string StandardOrganization { get; set; } = string.Empty;
        public string TestMethodStandard { get; set; } = string.Empty;
        public string? Part { get; set; }
        public string OfficialTitle { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public string Year { get; set; } = string.Empty;
        public string Status { get; set; } = "ok";       // "ok" | "warning" | "error"
        public List<string> Messages { get; set; } = new();
        public long? StandardOrganizationID { get; set; }
        public bool? Exists { get; set; }
        public long? ExistingSpecId { get; set; }
        // PDF matching info
        public string? PdfFileName { get; set; }
        public bool PdfFound { get; set; }
    }

    public class BulkImportRequestDto
    {
        public List<ImportTestMethodSpecItemDto> Items { get; set; } = new();
    }

    public class BulkImportResultDto
    {
        public int TotalRows { get; set; }
        public int Imported { get; set; }
        public int Skipped { get; set; }
        public List<string> Errors { get; set; } = new();
        public int PdfMatched { get; set; }
        public int PdfUploaded { get; set; }
    }
}
