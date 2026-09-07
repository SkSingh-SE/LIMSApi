using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    public class TestObservation : AuditProperty
    {
        [Key]
        public long ID { get; set; }

        public long TestSpecimenID { get; set; }

        public int ReadingNo { get; set; } = 1;

        [ForeignKey("TestSpecimenID")]
        public virtual TestSpecimen TestSpecimen { get; set; } = null!;
        
        public virtual ICollection<ParameterObservationResult> ParameterObservationResults { get; set; } = new List<ParameterObservationResult>();
    }
}
