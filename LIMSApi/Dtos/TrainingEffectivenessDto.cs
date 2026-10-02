namespace LIMSApi.Dtos
{
    public class TrainingEffectivenessDto
    {
        public long AttendanceId { get; set; }

        public long TrainingPlanId { get; set; }

        public long QuestionSetId { get; set; }
 
        public string? TrainingPlan { get; set; }

        public DateTime? TrainingDate { get; set; }

        public int? PlanningYear { get; set; }

        public string? Month { get; set; }

        public string? Agency { get; set; }

        public string? ProviderName { get; set; }

        public string? TrainingVenue { get; set; }

        public string? FacultyName { get; set; }

        public DateTime? TrainingDatetime { get; set; }

        public string? QuestionSet { get; set; }
        public DateTime? EvaluationDate { get; set; }
         
        public List<TrainingEffectivenessQuestionDto> Questions { get; set; }
            = new();
    }


    public class TrainingEffectivenessQuestionDto
    {
        public int QuestionNo { get; set; }

        public string? Question { get; set; }

        public string? OptionA { get; set; }

        public string? OptionB { get; set; }

        public string? OptionC { get; set; }

        public string? OptionD { get; set; }
    }
}

