using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    public class SampleDetail
    {
        public long ID { get; set; }
        public string LabNo { get; set; }
        public string Details { get; set; }
        public string Nature { get; set; }
        public string Remarks { get; set; }
        public int Quantity { get; set; }
        public bool AttachPhoto { get; set; }
        public bool Disabled { get; set; }

        public long? UploadReferenceID { get; set; }
        [StringLength(255)]
        public string? SampleFilePath { get; set; }
        public string? FileName { get; set; }

        [NotMapped]
        public IFormFile File { get; set; } = null!;
    }
}
