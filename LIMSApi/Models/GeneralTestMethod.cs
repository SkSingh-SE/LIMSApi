using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LIMSApi.Models
{
    public class GeneralTestMethod
    {
        [Key]
        public long ID { get; set; }
        public long GeneralTestID { get; set; }
        public long TestMethodID { get; set; }
        public long StandardID { get; set; }
        public int Quantity { get; set; }
        public string ReportNo { get; set; }
        public string UlrNo { get; set; }
        public bool Cancel { get; set; }
        [ForeignKey("GeneralTestID"),JsonIgnore]
        public virtual GeneralTest? GeneralTest { get; set; }
    }
}
