using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    public class LaboratoryTestAnalysisTypeSpecification
    {
        [Key]
        public long ID { get; set; }

        [Required]
        public long LaboratoryTestAnalysisTypeID { get; set; }

        public long? SpecificationHeaderID { get; set; }

        public long? SpecificationGradeID { get; set; }

        public long? ProductMasterID { get; set; }

        [ForeignKey(nameof(LaboratoryTestAnalysisTypeID))]
        public virtual LaboratoryTestAnalysisType? AnalysisType { get; set; }

        [ForeignKey(nameof(SpecificationHeaderID))]
        public virtual SpecificationHeader? MaterialSpecification { get; set; }

        [ForeignKey(nameof(SpecificationGradeID))]
        public virtual SpecificationGrade? SpecificationGrade { get; set; }

        [ForeignKey(nameof(ProductMasterID))]
        public virtual ProductMaster? ProductMaster { get; set; }
    }
}

