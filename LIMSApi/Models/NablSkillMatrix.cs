using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    [Table("NablSkillMatrices")]
    public class NablSkillMatrix : NablFormBase
    {
        // FK to existing Designation master
        public long DesignationId { get; set; }

        [MaxLength(100)]
        public string? DesignationName { get; set; }

        // Skills list stored as JSON array of skill names
        public string? SkillsJson { get; set; }

        // Employee skills stored as JSON array of { employeeId, employeeName, designationName, skills: {} }\
        // Approval
        public string? AverageRequiredSkillLevel { get; set; }
        public decimal? AverageRequiredSkill { get; set; }
        public string? EmployeeName { get; set; }
        public long? EmployeeId { get; set; }
        public DateTime? EvaluationDate { get; set; }

        [NotMapped]
        public List<EmployeeSkills> EmployeeSkills { get; set; } = new();
    }

    [Table("NablSkillMatrixDecisions")]
    public class NablSkillMatrixDecision : NablFormBase
    {
        // FK to existing Designation master
        public long DesignationId { get; set; }

        [ForeignKey("DesignationId")]
        public virtual DesignationMaster? Designation { get; set; }

        [MaxLength(100)]
        public string? DesignationName { get; set; }

        [MaxLength(200)]
        public string? Title { get; set; }

        // Rows stored as JSON array of { skillArea, competencyRequirement, evaluationMethod, authorized }
        public string? RowsJson { get; set; }

        // Approval
        [MaxLength(200)]
        public string? IssuedBy { get; set; }

        [MaxLength(200)]
        public string? ReviewedApprovedBy { get; set; }

        public DateTime? LastUpdated { get; set; }
    }

    [NotMapped]
    public class EmployeeSkills
    {
        public string? SkillName { get; set; }
        public string? SkillLevel { get; set; }
        public string? Level{ get; set; }
        public bool? Required { get; set; }
    }
}
