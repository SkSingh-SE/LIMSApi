using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    public class TpiInspection : AuditProperty
    {
        [Key]
        public long ID { get; set; }

        [Required]
        public long SampleInwardID { get; set; }

        public long? SampleDetailID { get; set; }

        [Required]
        public long TPIMasterID { get; set; }

        [Required]
        [Column(TypeName = "varchar(30)")]
        public string Stage { get; set; } = string.Empty; // BeforePreparation, BeforeTesting

        [Required]
        [Column(TypeName = "varchar(30)")]
        public string Status { get; set; } = "Pending";

        public DateTime? ScheduledDate { get; set; }
        public DateTime? InspectionDate { get; set; }

        [MaxLength(2000)]
        public string? Remarks { get; set; }

        [MaxLength(2000)]
        public string? InspectorComments { get; set; }

        [MaxLength(500)]
        public string? DocumentPath { get; set; }

        [ForeignKey("SampleInwardID")]
        public virtual SampleInward? SampleInward { get; set; }

        [ForeignKey("SampleDetailID")]
        public virtual SampleDetail? SampleDetail { get; set; }

        [ForeignKey("TPIMasterID")]
        public virtual TPIMaster? TPIMaster { get; set; }
    }

    public enum TpiStage
    {
        BeforePreparation = 1,
        BeforeTesting = 2
    }

    public enum TpiStatus
    {
        Pending = 1,
        Scheduled = 2,
        InProgress = 3,
        Approved = 4,
        Rejected = 5,
        Waived = 6
    }
}
