using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    public class LabScopeMaster : AuditProperty
    {
        [Key]
        public long ID { get; set; }
        public long LaboratoryTestID { get; set; }

        public long? LaboratoryTestSubGroupID { get; set; }
        public long? AnalysisTypeID { get; set; }

        // G17: Scope validity and review tracking
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidUntil { get; set; }
        public DateTime? NextReviewDate { get; set; }

        [MaxLength(500)]
        public string? ScopeRemarks { get; set; }

        [ForeignKey("LaboratoryTestID")]
        public virtual LaboratoryTest? LaboratoryTest { get; set; }

        [ForeignKey("LaboratoryTestSubGroupID")]
        public virtual LaboratoryTestSubGroup? SubGroup { get; set; }

        [ForeignKey("AnalysisTypeID")]
        public virtual LaboratoryTestAnalysisType? AnalysisType { get; set; }

        public ICollection<LabScopeSpecification> Specifications { get; set; } = new List<LabScopeSpecification>();
    }
   
}
