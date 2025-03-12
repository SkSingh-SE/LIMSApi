using LIMSApi.Dtos;

namespace LIMSApi.Models
{
    public class UploadFile : AuditProperty
    {
        public long ID { get; set; }
        public required string OriginalFileName { get; set; }
        public required string StoredFileName { get; set; }
        public FileType FileType { get; set; }
        public string? FileExtension { get; set; }
        public string? FilePath { get; set; }
        public long? FileSize { get; set; }
        public int? Year { get; set; }

    }
}
