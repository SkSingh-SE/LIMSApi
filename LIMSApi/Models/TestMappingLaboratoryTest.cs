using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    public class TestMappingLaboratoryTest
    {
        [Key]
        public long ID { get; set; }
        public long TestMappingID { get; set; }
        public long LaboratoryTestID { get; set; }

        [ForeignKey("LaboratoryTestID")]
        public virtual LaboratoryTest? LaboratoryTest { get; set; }
    }
}
