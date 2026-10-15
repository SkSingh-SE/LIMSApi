namespace LIMSApi.Dtos
{
    public class UnifiedTestHierarchyDto
    {
        public string Key { get; set; } = string.Empty;
        public string Id { get; set; } = string.Empty;
        public long LaboratoryTestID { get; set; }
        public string TestName { get; set; } = string.Empty;
        public bool IsChemicalTest { get; set; }
        public long? LaboratoryTestSubGroupID { get; set; }
        public string? SubGroupName { get; set; }
        public long? AnalysisTypeID { get; set; }
        public string? AnalysisTypeName { get; set; }
        public string Name { get; set; } = string.Empty;
        public string HierarchyPath { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public bool IsHeader { get; set; } = false;
        public int Level { get; set; } = 0;
        public bool Selectable { get; set; } = true;
        public Dictionary<string, object>? AdditionalValues { get; set; }
    }

    public class NablValidityDto
    {
        public string ValidFrom { get; set; } = string.Empty;
        public string ValidUntil { get; set; } = string.Empty;
        public string NextReviewDate { get; set; } = string.Empty;
        public string CertificateNumber { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public string Status { get; set; } = "Certificate Effective";
        public string ValidityPeriodFormatted { get; set; } = string.Empty;
    }
}
