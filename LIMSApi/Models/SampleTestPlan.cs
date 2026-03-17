using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LIMSApi.Models
{
    public class SampleTestPlan
    {
        [Key]
        public long ID { get; set; }
        public long SampleID { get; set; }
        public string SampleNo { get; set; }
        public List<GeneralTest> GeneralTests { get; set; } = new List<GeneralTest>();
        public List<ChemicalTest> ChemicalTests { get; set; } = new List<ChemicalTest>();

        // Versioning & Status
        public int Version { get; set; } = 1;
        public int ReplanCount { get; set; } = 0;

        [MaxLength(50)]
        public string PlanStatus { get; set; } = "Draft";  // Draft, Submitted, UnderReview, Approved, ReplanRequested, ReplanApproved

        public long? ApprovedById { get; set; }

        [MaxLength(200)]
        public string? ApprovedByName { get; set; }

        public DateTime? ApprovedAt { get; set; }

        [ForeignKey("SampleID"), JsonIgnore]
        public virtual SampleDetail? SampleDetail { get; set; }

        // Navigation properties for history & replan
        public virtual ICollection<PlanHistory> Histories { get; set; } = new List<PlanHistory>();
        public virtual ICollection<ReplanRequest> ReplanRequests { get; set; } = new List<ReplanRequest>();
    }
}
