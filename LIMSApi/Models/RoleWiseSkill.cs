using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    public class RoleWiseSkill: AuditProperty
    {
        public long Id { get; set; }
        public string? RoleWiseSkillNo { get; set; }
       public DateTime? Date { get; set; }
        public long? DesignationId { get; set; }
        public long? DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
        public string? DesignationName { get; set; }
        public string? Remarks { get; set; }
        // Database column
        public string? SkillJson { get; set; }
        // Not stored in database
        [NotMapped]
        public List<Skills> Skills { get; set; } = new();
    }
    [NotMapped]
    public class Skills
    {
        public string? SkillName { get; set; }
        public string? Leveltype { get; set; }
        public bool? Required { get; set; }
    }
}
