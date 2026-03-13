using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    [Table("NablMeetingAgendas")]
    public class NablMeetingAgenda : NablFormBase
    {
        public DateTime? MeetingDate { get; set; }

        [MaxLength(50)]
        public string? MeetingType { get; set; } // MRM/ReviewMeeting/AuditFollowup

        [MaxLength(500)]
        public string? MeetingVenue { get; set; }

        public long? ChairpersonId { get; set; }

        [ForeignKey("ChairpersonId")]
        public virtual EmployeeMaster? Chairperson { get; set; }

        [MaxLength(200)]
        public string? ChairpersonName { get; set; }

        public string? AgendaItemsJson { get; set; } // JSON array of {serialNo, topic, presenter, allocatedTime}

        public string? AttendeeIds { get; set; }

        public string? AttendeeNames { get; set; }

        [MaxLength(200)]
        public string? PreviousMOMRef { get; set; }
    }
}
