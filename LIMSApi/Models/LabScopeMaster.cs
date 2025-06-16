using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    public class LabScopeMaster : AuditProperty
    {
        [Key]
        public long ID { get; set; }
        public long LaboratoryTestID { get; set; }

        [ForeignKey("LaboratoryTestID")]
        public virtual LaboratoryTest? LaboratoryTest { get; set; }
        public ICollection<LabScopeSpecification> Specifications { get; set; } = new List<LabScopeSpecification>();
    }
   
}
