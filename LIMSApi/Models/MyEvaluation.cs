using Microsoft.VisualBasic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    public class MyEvaluation: AuditProperty
    {
        [Key]
        public long Id { get; set; }
        public long AttendanceId { get; set; }
        public long TrainingPlanId { get; set; }
        public long? QuestionSetId { get; set; }
        public long? ParticipantId { get; set; }
        public string? TrainingPlan { get; set; }
        public string? TrainingVenue { get; set; }
        public DateTime? TrainingDate { get; set; }
        public DateTime? TrainingDatetime { get; set; }
        public string? QuestionSetName { get; set; }
        public string? ParticipantName { get; set; }
        public string? Designation { get; set; }
        public string? QuestionSetJson { get; set; }
        public string? Agency { get; set; }
        public string? FacultyName { get; set; }
        public string? Month { get; set; }
        public int? PlanningYear { get; set; }
        public string? ProviderName { get; set; }
        public string? QuestionSet { get; set; }
        public string? Status { get; set; }
        public string? Result { get; set; }
        public int? TotalQuestions { get; set; }
        public int? CorrectAnswers { get; set; }
        public DateTime? EvaluationDate { get; set; }
        [NotMapped]
        public List<QuestionSet> Questions { get; set; } = new();
    }
    [NotMapped]
    public class QuestionSet
    {
        public int QuestionNo { get; set; }

        public string? Question { get; set; }

        public string? OptionA { get; set; }

        public string? OptionB { get; set; }

        public string? OptionC { get; set; }

        public string? OptionD { get; set; }

        // A / B / C / D
        public string? SelectedAnswer { get; set; }
    }
}
