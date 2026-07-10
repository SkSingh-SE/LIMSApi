using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    [Table("NablRetestings")]
    public class NablRetesting : NablFormBase
    {
        [MaxLength(200)]
        public string? SampleCode { get; set; }

        public DateTime? OriginalTestDate { get; set; }

        public string? RetestReason { get; set; }

        public DateTime? RetestDate { get; set; }

        [MaxLength(500)]
        public string? TestParameter { get; set; }

        [MaxLength(500)]
        public string? TestMethodName  { get; set; }

        [MaxLength(500)]
        public string? OriginalResult { get; set; }

        [MaxLength(500)]
        public string? RetestResult { get; set; }

        [MaxLength(100)]
        public string? Unit { get; set; }

        [MaxLength(500)]
        public string? AcceptanceCriteria { get; set; }

        [MaxLength(50)]
        public string? RetestConclusion { get; set; } // Consistent/Inconsistent

        [MaxLength(200)]
        public string? TestedBy { get; set; }

        [MaxLength(200)]
        public string? AuthorizedBy { get; set; }

        public string? Remarks { get; set; }
        public int QcPlanNoId { get; set; }
        public int QcPlanActivityId { get; set; }

        public string? PlanNo { get; set; }
        public int? PlanYear { get; set; }
        public string? Discipline { get; set; }
        public string? MaterialProductGroup { get; set; }
        public string? LabIncharge { get; set; }

        public string? QcActivity { get; set; }
        public string? DepartmentName { get; set; }

        public string? ReferenceType { get; set; }
        public string? ReferenceName { get; set; }
        public string? FrequencyType { get; set; }
        public string? ResponsibleEmployee { get; set; }

        public DateTime? EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public DateTime? NextDueDate { get; set; }

        public ICollection<RetestingInitialTestLog> InitialTestingLogs { get; set; } = new List<RetestingInitialTestLog>();
        public ICollection<RetestingComparisonLog> RetestingLogs { get; set; } = new List<RetestingComparisonLog>();
    }
}

