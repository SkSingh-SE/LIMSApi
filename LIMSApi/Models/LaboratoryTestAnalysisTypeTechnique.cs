using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    public class LaboratoryTestAnalysisTypeTechnique
    {
        [Key]
        public long ID { get; set; }

        [Required]
        public long LaboratoryTestAnalysisTypeID { get; set; }

        [Required]
        public long AnalysisTechniqueID { get; set; }

        [ForeignKey(nameof(LaboratoryTestAnalysisTypeID))]
        public virtual LaboratoryTestAnalysisType? AnalysisType { get; set; }

        [ForeignKey(nameof(AnalysisTechniqueID))]
        public virtual AnalysisTechniqueMaster? AnalysisTechnique { get; set; }
    }
}
