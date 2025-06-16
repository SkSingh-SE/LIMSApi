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
        public bool IsDisabled { get; set; } = false;
        public string Versions { get; set; }
    }
    public class VersionDto
    {
        public long ID { get; set; }
        public long TestMethodSpecificationID { get; set; }
        public bool Default { get; set; }
        public string Version { get; set; }
        public string Year { get; set; }
        public string StandardFile { get; set; } // match by name if needed
        public string StandardFilePath { get; set; }
        public long? UploadReferenceID { get; set; }
    }
}
