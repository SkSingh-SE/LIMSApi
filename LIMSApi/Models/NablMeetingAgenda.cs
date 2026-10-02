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
        public string? MeetingNo { get; set; }
        public TimeOnly? MeetingTime { get; set; }
        public string? ParticipantsJson { get; set; } // JSON array of {serialNo, topic, presenter, allocatedTime}
        [NotMapped]
        public List<AgendaItems>? AgendaItems { get; set; }
        [NotMapped]
        public List<Participants>? Participants { get; set; }
    }
    [NotMapped]
    public class AgendaItems
    {
        public string? AgendaItem { get; set; }
        public string? Presenter { get; set; }
        public string? Remarks { get; set; }
    }
    [NotMapped]
    public class Participants
    {
        public string? Name { get; set; }
        public string? Designation { get; set; }
        public string? Department { get; set; }
        public string? Attendance { get; set; }
    }
}
