using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    [Table("NablSkillMatrices")]
    public class NablSkillMatrix : NablFormBase
    {
        // FK to existing Designation master
        public long DesignationId { get; set; }

        [ForeignKey("DesignationId")]
        public virtual DesignationMaster? Designation { get; set; }

        [MaxLength(100)]
        public string? DesignationName { get; set; }

        [MaxLength(200)]
        public string? Title { get; set; }

        [MaxLength(100)]
        public string? Decision { get; set; }

        // Skills list stored as JSON array of skill names
        public string? SkillsJson { get; set; }

        // Employee skills stored as JSON array of { employeeId, employeeName, designationName, skills: {} }
        public string? EmployeeSkillsJson { get; set; }

        // Approval
        [MaxLength(200)]
        public string? IssuedBy { get; set; }

        [MaxLength(200)]
        public string? ReviewedApprovedBy { get; set; }

        public DateTime? LastUpdated { get; set; }
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
}
