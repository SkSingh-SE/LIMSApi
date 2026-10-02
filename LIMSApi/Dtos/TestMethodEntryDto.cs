namespace LIMSApi.Dtos
{
    public class TestMethodEntryDto
    {
        public int SrNo { get; set; }   
        public string? MethodName { get; set; }
        public string? SpecificationCode { get; set; }
        public string? ReferenceStandard { get; set; }
        public string? RevisionNo { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public string Status { get; set; }
    }
}
