using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    [Table("NablMeetingMinutes")]
    public class NablMeetingMinutes : NablFormBase
    {
        public long? AgendaId { get; set; }

        [ForeignKey("AgendaId")]
        public virtual NablMeetingAgenda? Agenda { get; set; }

        public DateTime? MeetingDate { get; set; }

        [MaxLength(50)]
        public string? MeetingType { get; set; }

        [MaxLength(200)]
        public string? ChairpersonName { get; set; }

        public string? AttendeesJson { get; set; } // JSON array of {employeeId, name, designation, signature}

        public string? MinutesJson { get; set; } // JSON array of {agendaItem, discussion, decision, actionRequired, responsiblePerson, targetDate}

        public DateTime? NextMeetingDate { get; set; }

        public string? NextMeetingAgenda { get; set; }

        [MaxLength(200)]
        public string? ActionClosureStatus { get; set; }
    }
}
