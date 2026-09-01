using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LIMSApi.Models
{
    public class ChemicalTestMethod
    {
        [Key]
        public long ID { get; set; }

        [Required]
        public long ChemicalTestID { get; set; }

        public long LaboratoryTestAnalysisTypeID { get; set; }

        public long? TestMethodSpecificationID { get; set; }

        public int Quantity { get; set; } = 1;

        [MaxLength(100)]
        public string? ReportNo { get; set; }

        [MaxLength(100)]
        public string UlrNo { get; set; } = string.Empty;

        public bool Cancel { get; set; }
        public bool PreparationRequired { get; set; } = false;

        [ForeignKey("ChemicalTestID"), JsonIgnore]
        public virtual ChemicalTest? ChemicalTest { get; set; }

        [ForeignKey(nameof(LaboratoryTestAnalysisTypeID))]
        public virtual LaboratoryTestAnalysisType? AnalysisType { get; set; }

        [ForeignKey(nameof(TestMethodSpecificationID))]
        public virtual TestMethodSpecification? TestMethodSpecification { get; set; }
    }
}
