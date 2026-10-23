using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    public class LaboratoryTestAnalysisType : AuditProperty
    {
        [Key]
        public long ID { get; set; }

        [Required]
        public long LaboratoryTestSubGroupID { get; set; }

        public long? MetalClassificationID { get; set; }

        [Required]
        [StringLength(150)]
        public string Name { get; set; } = string.Empty;

        public int? TestDuration { get; set; } = 1;

        [ForeignKey(nameof(LaboratoryTestSubGroupID))]
        public virtual LaboratoryTestSubGroup? SubGroup { get; set; }

        [ForeignKey(nameof(MetalClassificationID))]
        public virtual MetalClassificationMaster? MetalClassification { get; set; }

        public virtual ICollection<LaboratoryTestAnalysisTypeTechnique> AllowedTechniques { get; set; } = new List<LaboratoryTestAnalysisTypeTechnique>();
        public virtual ICollection<LaboratoryTestAnalysisTypeParameter> Parameters { get; set; } = new List<LaboratoryTestAnalysisTypeParameter>();
        public virtual ICollection<LaboratoryTestAnalysisTypeMethod> TestMethods { get; set; } = new List<LaboratoryTestAnalysisTypeMethod>();
        public virtual ICollection<LaboratoryTestAnalysisTypeEquipment> Equipments { get; set; } = new List<LaboratoryTestAnalysisTypeEquipment>();
        public virtual ICollection<LaboratoryTestAnalysisTypeSpecification> Specifications { get; set; } = new List<LaboratoryTestAnalysisTypeSpecification>();
        public virtual ICollection<LaboratoryTestAnalysisTypeInvoiceCase> InvoiceCases { get; set; } = new List<LaboratoryTestAnalysisTypeInvoiceCase>();
    }
}
