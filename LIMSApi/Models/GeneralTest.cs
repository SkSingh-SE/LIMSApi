namespace LIMSApi.Models
{
    public class GeneralTest
    {
        public long ID { get; set; }
        public long Specification1 { get; set; }
        public long Specification2 { get; set; }
        public string Parameter { get; set; }
        public ICollection<GeneralTestMethod> Methods { get; set; }
    }
}
