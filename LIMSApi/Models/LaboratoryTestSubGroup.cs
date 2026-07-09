using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    /// <summary>
    /// A named section under a LaboratoryTest that groups AnalysisTypes (formerly SubTypes) by metal base.
    /// </summary>
    public class LaboratoryTestSubGroup : AuditProperty
    {
        [Key]
        public long ID { get; set; }

        [Required]
        public long LaboratoryTestID { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string ReportTestName { get; set; } = string.Empty;

        public int? TestDuration { get; set; }

        public long? MetalClassificationID { get; set; }

        public int DisplayOrder { get; set; } = 0;

        [ForeignKey(nameof(LaboratoryTestID))]
        public virtual LaboratoryTest? LaboratoryTest { get; set; }

        [ForeignKey(nameof(MetalClassificationID))]
        public virtual MetalClassificationMaster? MetalClassification { get; set; }

        public virtual ICollection<LaboratoryTestAnalysisType> AnalysisTypes { get; set; } = new List<LaboratoryTestAnalysisType>();
        public virtual ICollection<LaboratoryTestSubGroupParameter> Parameters { get; set; } = new List<LaboratoryTestSubGroupParameter>();
        public virtual ICollection<LaboratoryTestSubGroupMethod> TestMethods { get; set; } = new List<LaboratoryTestSubGroupMethod>();
        public virtual ICollection<LaboratoryTestSubGroupEquipment> Equipments { get; set; } = new List<LaboratoryTestSubGroupEquipment>();
        public virtual ICollection<LaboratoryTestSubGroupSpecification> Specifications { get; set; } = new List<LaboratoryTestSubGroupSpecification>();
        public virtual ICollection<LaboratoryTestSubGroupInvoiceCase> InvoiceCases { get; set; } = new List<LaboratoryTestSubGroupInvoiceCase>();
    }
}
