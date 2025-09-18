namespace LIMSApi.Dtos
{
    public class PlanDto
    {
        public long ID { get; set; }
        public string CaseNo { get; set; }
        public long CustomerID { get; set; }

        public string StatementOfConformity { get; set; }
        public string DecisionRule { get; set; }
        public string ReviewStatus { get; set; } = "Pending";
        public long? ReviewedBy { get; set; }
        public DateTime? ReviewedOn { get; set; }

        public ICollection<SampleDetailDto> SampleDetails { get; set; } = new List<SampleDetailDto>();
    }

}
