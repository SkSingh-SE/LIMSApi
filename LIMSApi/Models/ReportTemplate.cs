using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LIMSApi.Helpers.Enums;

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

        // ── Multi-Format Fields ──
        public ReportFormatType FormatType { get; set; } = ReportFormatType.TestCertificate;

        [Column(TypeName = "nvarchar(max)")]
        public string? HeaderConfigJson { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string? SectionVisibilityJson { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string? SignatoryConfigJson { get; set; }

        [MaxLength(20)]
        public string? PageLayout { get; set; } = "Portrait"; // Portrait | Landscape

        public bool ShowCalibrationTable { get; set; } = false;
        public bool ShowSpecificationRow { get; set; } = true;
        public bool ShowNablColumn { get; set; } = false;

        [MaxLength(50)]
        public string? WatermarkText { get; set; } // DRAFT | AMENDED | COPY | null

        public ICollection<ReportTemplateBlock> Blocks { get; set; } = new List<ReportTemplateBlock>();

        [ForeignKey(nameof(TestTypeID))]
        public LaboratoryTest? Test { get; set; }
    }

}
