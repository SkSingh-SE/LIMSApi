using System.ComponentModel.DataAnnotations;

namespace LIMSApi.Dtos
{
    public class LaboratoryTestListDto
    {
        public long ID { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public long? DisciplineID { get; set; }
        public string? DisciplineName { get; set; }
        public long? LabDepartmentID { get; set; }
        public string? DepartmentName { get; set; }
        public int? TestDuration { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public int ParameterCount { get; set; }
        public int MethodCount { get; set; }
        public int ConditionCount { get; set; }
        public string CompanyCode { get; set; } = "LIMS";
        public DateTime CreatedOn { get; set; }
    }

    public class LaboratoryTestDetailDto
    {
        public long ID { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public long? DisciplineID { get; set; }
        public string? DisciplineName { get; set; }
        public long? LabDepartmentID { get; set; }
        public string? DepartmentName { get; set; }
        public int? TestDuration { get; set; }
        public string? Equation { get; set; } // Read-only informational legacy text
        public bool IsChemicalTest { get; set; }
        public bool IsMechanical { get; set; }
        public bool IsActive { get; set; }
        public string CompanyCode { get; set; } = "LIMS";
        public long CreatedBy { get; set; }
        public string? CreatedByName { get; set; }
        public DateTime CreatedOn { get; set; }
        public long? ModifiedBy { get; set; }
        public string? ModifiedByName { get; set; }
        public DateTime? ModifiedOn { get; set; }

        public List<LaboratoryTestParameterItemDto> Parameters { get; set; } = new();
        public List<LaboratoryTestMethodItemDto> Methods { get; set; } = new();
        public List<LaboratoryTestConditionItemDto> Conditions { get; set; } = new();
    }

    public class LaboratoryTestParameterItemDto
    {
        public long ID { get; set; }
        public long LaboratoryTestID { get; set; }
        public long ParameterID { get; set; }
        public string ParameterCode { get; set; } = string.Empty;
        public string ParameterName { get; set; } = string.Empty;
        public string? ParameterUnit { get; set; }
        public string? InputType { get; set; }
        public bool IsMandatory { get; set; } = true;
        public bool IsReportable { get; set; } = true;
        public int DisplayOrder { get; set; } = 0;
        public bool IsActive { get; set; } = true;
    }

    public class LaboratoryTestMethodItemDto
    {
        public long ID { get; set; }
        public long LaboratoryTestID { get; set; }
        public long TestMethodSpecificationID { get; set; }
        public string MethodCode { get; set; } = string.Empty;
        public string MethodName { get; set; } = string.Empty;
        public string DisplayTitle { get; set; } = string.Empty;
        public string? StandardReference { get; set; }
        public string? AnalysisTechniqueName { get; set; }
        public bool IsDefault { get; set; } = false;
        public int DisplayOrder { get; set; } = 0;
        public bool IsActive { get; set; } = true;
    }

    public class LaboratoryTestConditionItemDto
    {
        public long ID { get; set; }
        public long LaboratoryTestID { get; set; }
        public long ConditionMasterID { get; set; }
        public string ConditionCode { get; set; } = string.Empty;
        public string ConditionName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string ValueType { get; set; } = string.Empty;
        public string? ParameterUnit { get; set; }
        public bool IsMandatory { get; set; } = false;
        public int DisplayOrder { get; set; } = 0;
        public bool IsActive { get; set; } = true;
    }

    public class LaboratoryTestCreateDto
    {
        [Required]
        [StringLength(50)]
        [RegularExpression(@"^[A-Z0-9_]+$", ErrorMessage = "Code must contain only uppercase letters, numbers, and underscores.")]
        public string Code { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        public long? DisciplineID { get; set; }
        public long? LabDepartmentID { get; set; }

        [Range(1, 365)]
        public int? TestDuration { get; set; }

        public bool IsActive { get; set; } = false; // Draft allowed on creation

        public List<LaboratoryTestParameterItemDto> Parameters { get; set; } = new();
        public List<LaboratoryTestMethodItemDto> Methods { get; set; } = new();
        public List<LaboratoryTestConditionItemDto> Conditions { get; set; } = new();
    }

    public class LaboratoryTestUpdateDto
    {
        [Required]
        public long ID { get; set; }

        [Required]
        [StringLength(50)]
        [RegularExpression(@"^[A-Z0-9_]+$", ErrorMessage = "Code must contain only uppercase letters, numbers, and underscores.")]
        public string Code { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        public long? DisciplineID { get; set; }
        public long? LabDepartmentID { get; set; }

        [Range(1, 365)]
        public int? TestDuration { get; set; }

        public bool IsActive { get; set; }

        public List<LaboratoryTestParameterItemDto> Parameters { get; set; } = new();
        public List<LaboratoryTestMethodItemDto> Methods { get; set; } = new();
        public List<LaboratoryTestConditionItemDto> Conditions { get; set; } = new();
    }

    public class LaboratoryTestDropdownDto
    {
        public long ID { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public long? DisciplineID { get; set; }
        public string? DisciplineName { get; set; }
    }
}
