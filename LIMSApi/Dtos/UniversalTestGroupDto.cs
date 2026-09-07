namespace LIMSApi.Dtos
{
    public class UniversalTestGroupDto
    {
        public long ID { get; set; }
        public long BranchID { get; set; }
        public long SampleTestPlanID { get; set; }
        public long LaboratoryTestID { get; set; }
        public string? LaboratoryTestName { get; set; }
        public long? DepartmentID { get; set; }
        public string? DepartmentName { get; set; }
        public long? TestMethodSpecificationID { get; set; }
        public string? TestMethodName { get; set; }
        public long? TestMethodSpecificationVersionID { get; set; }
        public string? TestMethodVersion { get; set; }
        public long? SpecificationHeaderID { get; set; }
        public string? SpecificationName { get; set; }
        public long? SpecificationVersionID { get; set; }
        public string? SpecificationVersionNumber { get; set; }
        public long? SpecificationGradeID { get; set; }
        public string? SpecificationGradeName { get; set; }
        public long OrganizationID { get; set; }
        public string Status { get; set; } = "Pending";
        public long? TestExecutionID { get; set; }
        public string? ExecutionStatus { get; set; }
    }
}
