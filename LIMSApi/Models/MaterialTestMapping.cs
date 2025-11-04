namespace LIMSApi.Models
{
    public class MaterialTestMapping : AuditProperty
    {
        public long ID { get; set; }
        public long? MetalClassificationID { get; set; }
        public long? ProductConditionID { get; set; }
        public long? GradeID { get; set; }
        public long LaboratoryTestID { get; set; }
        public bool IsDefault { get; set; }

        // Navigation properties
        public virtual MetalClassificationMaster? MetalClassification { get; set; }
        public virtual ProductConditionMaster? ProductCondition { get; set; }
        public virtual SpecificationGrade? Grade { get; set; }
        public virtual LaboratoryTest? LaboratoryTest { get; set; }
    }
}
