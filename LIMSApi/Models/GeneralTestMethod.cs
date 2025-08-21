using System.ComponentModel.DataAnnotations;

namespace LIMSApi.Models
{
    public class GeneralTestMethod
    {
        [Key]
        public long ID { get; set; }
        public long TestMethodID { get; set; }
        public long StandardID { get; set; }
        public string Quantity { get; set; }
        public string ReportNo { get; set; }
        public string UlrNo { get; set; }
        public bool Cancel { get; set; }
    }
}
