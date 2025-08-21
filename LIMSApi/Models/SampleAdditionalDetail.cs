namespace LIMSApi.Models
{
    public class SampleAdditionalDetail
    {
        public long ID { get; set; }
        public string Label { get; set; }
        public bool Enabled { get; set; }
        public ICollection<string> Values { get; set; } = new List<string>();
    }
}
