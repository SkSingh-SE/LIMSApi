using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    public class ReportTemplate : AuditProperty
    {
        [Key]
        public long ID { get; set; }

        [Required, MaxLength(100)]
        public string TemplateCode { get; set; } = string.Empty;
        // DEFAULT_TEST_CERTIFICATE, CHEMICAL_ANALYSIS, etc.

        [Required, MaxLength(200)]
        public string DisplayName { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? TestType { get; set; }   // Informational only

        public long? TestTypeID { get; set; }   // NULL = generic template

        public int Version { get; set; } = 1;
        public bool IsLocked { get; set; } = false;
        public bool IsDefault { get; set; } = false;

        public ICollection<ReportTemplateBlock> Blocks { get; set; } = new List<ReportTemplateBlock>();

        [ForeignKey(nameof(TestTypeID))]
        public LaboratoryTest? Test { get; set; }
    }

}
