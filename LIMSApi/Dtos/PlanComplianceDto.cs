using System.Collections.Generic;

namespace LIMSApi.Dtos
{
    public class PlanComplianceRequestDto
    {
        public long? ProductMasterID { get; set; }
        public long? MetalClassificationID { get; set; }
        public long? SpecificationGradeID { get; set; }
        public long? LaboratoryTestSubGroupID { get; set; }
        public long? LaboratoryTestAnalysisTypeID { get; set; }
        public long? TestMethodSpecificationID { get; set; }
        public bool IsUnknownSample { get; set; } = false;
    }

    public class PlanComplianceResultDto
    {
        public bool IsFullyCompliant { get; set; } = true;
        public bool IsScopeConfigured { get; set; } = true;
        public bool IsUnknownSampleWorkflow { get; set; } = false;
        public string ComplianceBadge { get; set; } = "Configured"; // "Configured" | "Custom Selection" | "Scope Not Configured"
        public string Message { get; set; } = "";
        public List<string> DeviationNotes { get; set; } = new List<string>();
        public List<DropdwonSelector> RecommendedStandards { get; set; } = new List<DropdwonSelector>();
    }
}
