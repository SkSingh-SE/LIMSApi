using System.Collections.Generic;

namespace LIMSApi.Dtos
{
    public class ConfiguredParameterDto
    {
        public long ParameterID { get; set; }
        public string ParameterName { get; set; } = string.Empty;
        public decimal? MinValue { get; set; }
        public decimal? MaxValue { get; set; }
        public long ParameterUnitID { get; set; }
        public string ParameterUnit { get; set; } = string.Empty;
        public bool Selected { get; set; } = true;
    }

    public class ConfiguredTestDto
    {
        public long LaboratoryTestID { get; set; }
        public string LaboratoryTestName { get; set; } = string.Empty;
        public string TestType { get; set; } = "General"; // General or Chemical
        public string SubGroup { get; set; } = string.Empty;
        public long? TestMethodStandardID { get; set; }
        public string TestMethodStandardName { get; set; } = string.Empty;
        public int Quantity { get; set; } = 1;
    }

    public class ConfiguredGradeDto
    {
        public long SpecificationGradeID { get; set; }
        public string GradeName { get; set; } = string.Empty;
        public long? SpecificationID { get; set; }
        public string SpecificationName { get; set; } = string.Empty;
        public string MetalClassificationName { get; set; } = string.Empty;
        public List<ConfiguredTestDto> ConfiguredTests { get; set; } = new List<ConfiguredTestDto>();
        public List<ConfiguredParameterDto> ChemicalElements { get; set; } = new List<ConfiguredParameterDto>();
    }

    public class ProductMasterExplorerDto
    {
        public long ProductMasterID { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string DisplayTitle { get; set; } = string.Empty;
        public long? MetalClassificationID { get; set; }
        public string MetalClassificationName { get; set; } = string.Empty;
        public long? ProductSizeMasterID { get; set; }
        public string ProductSizeDisplayName { get; set; } = string.Empty;
        public List<ConfiguredGradeDto> Grades { get; set; } = new List<ConfiguredGradeDto>();
    }

    public class MetalExplorerDto
    {
        public long MetalClassificationID { get; set; }
        public string MetalClassificationName { get; set; } = string.Empty;
        public List<ConfiguredGradeDto> Grades { get; set; } = new List<ConfiguredGradeDto>();
    }

    public class LabTestExplorerDto
    {
        public long LaboratoryTestID { get; set; }
        public string LaboratoryTestName { get; set; } = string.Empty;
        public string Category { get; set; } = "General";
        public List<ConfiguredTestDto> Standards { get; set; } = new List<ConfiguredTestDto>();
    }
}
