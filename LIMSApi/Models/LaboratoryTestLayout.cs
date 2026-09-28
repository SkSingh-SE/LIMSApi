using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    /// <summary>
    /// PHASE 2 — LaboratoryTestLayout (assignment of a presentation layout to a test).
    /// Answers ONLY "which execution layout is applicable to this test".
    /// Presentation-only: never owns formula, limits, MQU, calibration, results, compliance.
    /// Scope: enterprise/company (no BranchID — same scope as LaboratoryTest).
    /// Precedence: Test+Version &gt; Test+Method &gt; Test &gt; unassigned.
    /// Same level: Priority, then IsDefault, then exactly-one-winner else BLOCK.
    /// IsDefault is per assignment scope (version/method/test), not global per test.
    /// Guards: Version requires Method; Version must belong to Method;
    /// Method/Version must exist in LaboratoryTestMethod for the test.
    /// </summary>
    public class LaboratoryTestLayout : AuditProperty
    {
        [Key]
        public long ID { get; set; }

        [Required]
        public long LaboratoryTestID { get; set; }

        [Required]
        public long ExecutionLayoutID { get; set; }

        public long? TestMethodSpecificationID { get; set; }

        public long? TestMethodSpecificationVersionID { get; set; }

        public int Priority { get; set; } = 0;

        public bool IsDefault { get; set; } = false;

        [ForeignKey(nameof(LaboratoryTestID))]
        public virtual LaboratoryTest? LaboratoryTest { get; set; }

        [ForeignKey(nameof(ExecutionLayoutID))]
        public virtual ExecutionLayoutMaster? ExecutionLayout { get; set; }

        [ForeignKey(nameof(TestMethodSpecificationID))]
        public virtual TestMethodSpecification? TestMethodSpecification { get; set; }

        [ForeignKey(nameof(TestMethodSpecificationVersionID))]
        public virtual TestMethodSpecificationVersion? TestMethodSpecificationVersion { get; set; }
    }
}
