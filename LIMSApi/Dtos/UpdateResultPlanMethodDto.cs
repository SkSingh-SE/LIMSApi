namespace LIMSApi.Dtos
{
    public class UpdateResultPlanMethodDto
    {
        public long HeaderId { get; set; }
        public long? TestMethodSpecificationID { get; set; }
        public long? TestMethodSpecificationVersionID { get; set; }
        public long? Specification1 { get; set; }
    }

    public class UpdateResultPlanMethodResult
    {
        public long HeaderId { get; set; }
        public long? TestMethodSpecificationID { get; set; }
        public long? TestMethodSpecificationVersionID { get; set; }
        public string? MethodName { get; set; }
        public long? Specification1 { get; set; }
        public string? SpecName { get; set; }
    }
}
