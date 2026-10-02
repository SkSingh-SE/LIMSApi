using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    [Table("NablEmployeeAuthorizations")]
    public class NablEmployeeAuthorization : NablFormBase
    {
        // FK to existing Department master
        public long? DepartmentId { get; set; }

        [ForeignKey("DepartmentId")]
        public virtual DepartmentMaster? Department { get; set; }

        [MaxLength(100)]
        public string? DepartmentName { get; set; }

        // FK to existing Employee master
        public long? EmployeeId { get; set; }

        [ForeignKey("EmployeeId")]
        public virtual EmployeeMaster? Employee { get; set; }

        [MaxLength(200)]
        public string? PersonnelName { get; set; }

        [MaxLength(50)]
        public string? Uid { get; set; }

        [MaxLength(200)]
        public string? Equipment { get; set; }

        [MaxLength(500)]
        public string? TestMethodAuthorization { get; set; }

        [MaxLength(500)]
        public string? TestAuthorization { get; set; }

       
        public List<NablEmployeeEquipmentAuthrization>? EmployeeEquipmentAuth { get; set; }

        public List<NablEmployeeTestMethodAuthorization>? TestMethodAuth { get; set; }
        
        public List<NablEmployeeLaborartyTestAuthorization>? LabTestAuth { get; set; }
    }

}
