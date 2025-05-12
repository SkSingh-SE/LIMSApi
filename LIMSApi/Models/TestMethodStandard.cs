using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    public class TestMethodStandard : AuditProperty
    {
        [Key]
        public long ID { get; set; }

        [MaxLength(255)]
        public string? Name { get; set; } // Organisation + Test Method SpecificationCode + Year

        [MaxLength(500)]
        public string? Caption { get; set; }

        [Required]
        public long StandardOrganisationID { get; set; }

        [MaxLength(50)]
        public string? TestMethodCode { get; set; }

        [Required]
        public int Year { get; set; }
        [MaxLength(500)]
        public string? Description { get; set; }
        public bool UnderNABL {  get; set; }
        [Required]
        [MaxLength(250)]
        public string Group {  get; set; } = string.Empty;
        [MaxLength(250)]
        public string? SubGroup { get; set; }
        [Required]
        [Column(TypeName = "varchar(20)")]
        [EnumDataType(typeof(TestType))]
        public TestType TestCategory { get; set; }
        [MaxLength(500)]
        public string? DocumentPath { get; set; }
        public string? Parameters { get; set; }
        public string? ParameterUnits { get; set; }
        public long EquipmentID { get; set; }



        [ForeignKey("StandardOrganisationID")]
        public virtual StandardOrganizationMaster? StandardOrganisation { get; set; }

        [ForeignKey("EquipmentID")]
        public virtual EquipmentMaster? Equipment {  get; set; }
    }
    public enum TestType
    {
        Qualitative = 1,
        Quantitative = 2
    }
}
