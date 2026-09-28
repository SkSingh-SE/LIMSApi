using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    public class UniversalTestGroup : AuditProperty
    {
        [Key]
        public long ID { get; set; }

        public long BranchID { get; set; }

        public long SampleTestPlanID { get; set; }

        public long LaboratoryTestID { get; set; }

        public long? TestMethodSpecificationID { get; set; }

        public long? TestMethodSpecificationVersionID { get; set; }

        public long? SpecificationHeaderID { get; set; }

        public long? SpecificationGradeID { get; set; }

        public long? SpecificationVersionID { get; set; }

        public long OrganizationID { get; set; }

        [StringLength(50)]
        public string Status { get; set; } = "Pending";

        public long? ExecutionLayoutID { get; set; }

        public long? DepartmentID { get; set; }

        public string? PlannedConfigurationJson { get; set; }

        public int ExecutionCount { get; set; } = 0;

        [ForeignKey("BranchID")]
        public virtual Branch? Branch { get; set; }

        [ForeignKey("SampleTestPlanID")]
        public virtual SampleTestPlan SampleTestPlan { get; set; } = null!;

        [ForeignKey("LaboratoryTestID")]
        public virtual LaboratoryTest LaboratoryTest { get; set; } = null!;

        [ForeignKey("ExecutionLayoutID")]
        public virtual ExecutionLayoutMaster? ExecutionLayout { get; set; }

        [ForeignKey("DepartmentID")]
        public virtual DepartmentMaster? Department { get; set; }

        [ForeignKey("TestMethodSpecificationID")]
        public virtual TestMethodSpecification? TestMethodSpecification { get; set; }

        [ForeignKey("TestMethodSpecificationVersionID")]
        public virtual TestMethodSpecificationVersion? TestMethodSpecificationVersion { get; set; }

        [ForeignKey("SpecificationHeaderID")]
        public virtual SpecificationHeader? SpecificationHeader { get; set; }

        [ForeignKey("SpecificationGradeID")]
        public virtual SpecificationGrade? SpecificationGrade { get; set; }

        [ForeignKey("SpecificationVersionID")]
        public virtual SpecificationVersion? SpecificationVersion { get; set; }

        [ForeignKey("OrganizationID")]
        public virtual Organization Organization { get; set; } = null!;

        public virtual ICollection<TestExecution> TestExecutions { get; set; } = new List<TestExecution>();
    }
}
