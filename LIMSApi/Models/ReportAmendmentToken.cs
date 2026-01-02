using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    public class ReportAmendmentToken
    {
        [Key]
        public long ID { get; set; }
        public Guid Token { get; set; }
        public long ReportID { get; set; }
        public long SampleID { get; set; }
        public DateTime LinkExpiryOn { get; set; }
        public DateTime FreeUntil { get; set; }
        public bool IsUsed { get; set; }
        public DateTime CreatedOn { get; set; }

        [ForeignKey(nameof(ReportID))]
        public virtual Report? Report { get; set; }
    }

}
