using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LIMSApi.Models
{
    public class SampleDetail
    {
        [Key]
        public long ID { get; set; }
        public string SampleNo { get; set; }
        public string Details { get; set; }
        public string Nature { get; set; }
        public string Category { get; set; }
        public string Remarks { get; set; }
        public int Quantity { get; set; }
        public bool Disabled { get; set; }

        public long? UploadReferenceID { get; set; }
        [StringLength(255)]
        public string? SampleFilePath { get; set; }
        public string? FileName { get; set; }
        public long InwardID { get; set; }
        [ForeignKey("InwardID"), JsonIgnore]
        public virtual SampleInward? SampleInward { get; set; } = null!;

        public virtual ICollection<SampleAdditionalDetail> AdditionalDetails { get; set; } = new List<SampleAdditionalDetail>();

        [NotMapped]
        public IFormFile File { get; set; } = null!;
    }
}
