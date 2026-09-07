using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    [Table("BranchDisciplines")]
    public class BranchDiscipline : AuditProperty
    {
        [Key]
        public long ID { get; set; }

        public long BranchID { get; set; }
        [ForeignKey("BranchID")]
        public virtual Branch? Branch { get; set; }

        public long DisciplineID { get; set; }
        [ForeignKey("DisciplineID")]
        public virtual DisciplineMaster? Discipline { get; set; }

        public bool IsAccredited { get; set; } = false;
    }
}
