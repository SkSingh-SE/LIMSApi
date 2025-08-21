namespace LIMSApi.Models
{
    public class SampleTestPlan
    {
        public long ID { get; set; }
        public long SampleID { get; set; }
        public string SampleNo { get; set; }
        public ICollection<GeneralTest> GeneralTests { get; set; }
        public ICollection<ChemicalTest> ChemicalTests { get; set; }
    }
}
