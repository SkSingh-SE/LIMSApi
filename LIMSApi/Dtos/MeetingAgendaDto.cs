using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Dtos
{
    public class MeetingAgendaDto
    {
        public long MeetingId { get; set; }
        public string? MeetingNo { get; set; }
        public TimeOnly? MeetingTime { get; set; }
        public string? ChairpersonName { get; set; }
        public DateTime? MeetingDate { get; set; }
        public string? MeetingType { get; set; }
        public string? MeetingVenue { get; set; }
        public List<AgendaItemsDto>? Agendalist { get; set; }= new List<AgendaItemsDto>();
        public List<ParticipantsDto>? ParticipantItems { get; set; }= new List<ParticipantsDto>();


    }
    public class AgendaItemsDto
    {
        public string? AgendaItem { get; set; }
    }

    public class ParticipantsDto
    {
        public string? Name { get; set; }
        public string? Designation { get; set; }
        public string? Department { get; set; }
    }
}
