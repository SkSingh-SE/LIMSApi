namespace LIMSApi.Dtos
{
    public class TrainingPlanDetailsDto
    {
        public long? TrainingPlanId { get; set; }
        public string TrainingTopic { get; set; }
        public int? PlanningYear { get; set; }
        public string Month { get; set; }
        public string Audience { get; set; } // Map to 'To be attended by'
        public string ProviderAgencyName { get; set; }
        public long? ProviderAgencyId { get; set; }
        public bool? EvaluationRequired { get; set; }
        public string Agency { get; set; }
        public string QuestionSet { get; set; }
        public long? QuestionSetId { get; set; }
        public DateTime? PlanDate { get; set; }
    }
}
