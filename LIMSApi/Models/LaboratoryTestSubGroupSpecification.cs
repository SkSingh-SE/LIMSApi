using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    public class LaboratoryTestSubGroupSpecification
    {
        [Key]
        public long ID { get; set; }

        [Required]
        public long LaboratoryTestSubGroupID { get; set; }

        public long? SpecificationHeaderID { get; set; }

        public long? SpecificationGradeID { get; set; }

        public long? ProductSpecificationID { get; set; }

        [ForeignKey(nameof(LaboratoryTestSubGroupID))]
        public virtual LaboratoryTestSubGroup? SubGroup { get; set; }

        [ForeignKey(nameof(SpecificationHeaderID))]
        public virtual SpecificationHeader? MaterialSpecification { get; set; }

        [ForeignKey(nameof(SpecificationGradeID))]
        public virtual SpecificationGrade? SpecificationGrade { get; set; }

        [ForeignKey(nameof(ProductSpecificationID))]
        public virtual ProductSpecification? ProductSpecification { get; set; }
    }
}
