using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    public class ReportFormatMapping : AuditProperty
    {
        [Key]
        public long ID { get; set; }

        [Required]
        public long ReportFormatID { get; set; }

        public long? LaboratoryTestID { get; set; }

        public long? TestMethodID { get; set; }

        public int Priority { get; set; } = 0;

        [ForeignKey(nameof(ReportFormatID))]
        public ReportFormat? ReportFormat { get; set; }

        [ForeignKey(nameof(LaboratoryTestID))]
        public LaboratoryTest? LaboratoryTest { get; set; }

        [ForeignKey(nameof(TestMethodID))]
        public TestMethodSpecification? TestMethod { get; set; }
    }
}
