using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    public class TestSpecimen : AuditProperty
    {
        [Key]
        public long ID { get; set; }

        public long TestExecutionID { get; set; }

        public int SequenceNo { get; set; }

        [StringLength(100)]
        public string? SpecimenIdentifier { get; set; }

        public bool IsDiscarded { get; set; } = false;

        [ForeignKey("TestExecutionID")]
        public virtual TestExecution TestExecution { get; set; } = null!;
        
        public virtual ICollection<TestObservation> TestObservations { get; set; } = new List<TestObservation>();
    }
}
