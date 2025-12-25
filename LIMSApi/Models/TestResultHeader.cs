using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LIMSApi.Models
{
    [Table("TestResultHeaders")]
    public class TestResultHeader : AuditProperty
    {
        [Key]
        public long ID { get; set; }

        [Required]
        public long SampleID { get; set; }

        [Required]
        public long LaboratoryTestID { get; set; }

        [Required]
        public long TestPlanID { get; set; }
        public long? TestID { get; set; }

        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = "Pending";

        // 🔍 Pass/Fail for the entire test
        public bool? IsOverallPass { get; set; }

        // 🧪 Is this test NABL accredited or not?
        public bool IsNabl { get; set; } = false;
        public string? LabNo { get; set; }
        [MaxLength(128)]
        public string? CertificateNo { get; set; }
        public long? EquipmentID { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }

        public long? StartedBy { get; set; }
        // 🔗 Navigation Property
        public ICollection<TestResultParameter> Parameters { get; set; } = new List<TestResultParameter>();
        public ICollection<TestResultImage> Images { get; set; } = new List<TestResultImage>();
        public ICollection<LongTermTest> LongTermTests { get; set; } = new List<LongTermTest>();

        [ForeignKey(nameof(LaboratoryTestID))]
        public virtual LaboratoryTest? LaboratoryTest { get; set; }
        [ForeignKey(nameof(SampleID))]
        public virtual SampleDetail? Sample { get; set; }
    }

}
