namespace LIMSApi.Models
{
    public class ChemicalTest
    {
        public long ID { get; set; }
        public string ReportNo { get; set; }
        public string UrlNo { get; set; }
        //public Dictionary<string, bool> TestTypes { get; set; }
        public long MetalClassificationID { get; set; }
        public long Specification1 { get; set; }
        public long? Specification2 { get; set; }
        public long TestMethod { get; set; }
        public ICollection<ChemicalTestElement> Elements { get; set; }
    }
}
