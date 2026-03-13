using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    [Table("NablMasterDocuments")]
    public class NablMasterDocument : NablFormBase
    {
        [MaxLength(200)]
        public string? DocumentCode { get; set; }

        [MaxLength(500)]
        public string? DocumentTitle { get; set; }

        [MaxLength(50)]
        public string? DocumentType { get; set; } // SOP/Form/Policy/WorkInstruction/Other

        [MaxLength(50)]
        public string? CurrentIssue { get; set; }

        [MaxLength(50)]
        public string? CurrentRevision { get; set; }

        public DateTime? EffectiveDate { get; set; }

        [MaxLength(200)]
        public string? ReviewFrequency { get; set; }

        [MaxLength(200)]
        public string? DocumentOwner { get; set; }

        [MaxLength(500)]
        public string? StorageLocation { get; set; }

        public string? ControlledCopiesJson { get; set; } // JSON array of {copyNo, holder, location, dateIssued}

        public DateTime? ObsoleteDate { get; set; }

        public string? ObsoleteReason { get; set; }
    }
}
