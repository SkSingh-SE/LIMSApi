using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    public class TestUsageStats
    {
        [Key]
        public long ID { get; set; }
        public long LaboratoryTestID { get; set; }
        public long? MetalClassificationID { get; set; }
        public long? ProductConditionID { get; set; }
        public long? CustomerID { get; set; }
        public int UsageCount { get; set; }
        public int RecentUsageCount { get; set; }
        public DateTime? LastUsedDate { get; set; }
        public DateTime ComputedAt { get; set; }

        [ForeignKey("LaboratoryTestID")]
        public virtual LaboratoryTest? LaboratoryTest { get; set; }
    }
}
