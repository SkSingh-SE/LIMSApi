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

        public string? AgendaItemsJson { get; set; } // JSON array of {agendaItem, discussion, decision, actionRequired, responsiblePerson, targetDate}
        public string? ActionPlanJson { get; set; } // JSON array of {agendaItem, discussion, decision, actionRequired, responsiblePerson, targetDate}

        public DateTime? NextMeetingDate { get; set; }

        public string? NextMeetingAgenda { get; set; }

        [MaxLength(200)]
        public string? ActionClosureStatus { get; set; }
        public long? MeetingId { get; set; }
        public string? MeetingNo { get; set; }
        public string? MeetingVenue { get; set; }
        public TimeOnly? MeetingTime { get; set; }
        public string? OverallConclusion { get; set; }
        [NotMapped]
        public List<AgendaList> AgendaList { get; set; }
        [NotMapped]
        public List<ParticipantItems> ParticipantItems { get; set; }
        [NotMapped]
        public List<ActionItems> ActionItems { get; set; }
    }
    [NotMapped]
    public class ActionItems
    {
        public string Action { get; set; }
        public string Responsibility { get; set; }
        public DateTime TargetDate { get; set; }
        public string Priority { get; set; }
        public string Status { get; set; }
    }
    [NotMapped]
    public class AgendaList
    {
        public string AgendaItem { get; set; }
        public string Discussion { get; set; }
        public string Decisiontaken { get; set; }
    }
    [NotMapped]
    public class ParticipantItems
    {
        public string Name { get; set; }
        public string Designation { get; set; }
        public string Department { get; set; }

    }
}
