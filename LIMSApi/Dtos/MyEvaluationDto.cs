namespace LIMSApi.Dtos
{
    public class MyEvaluationDto
    {
        public long? EvaluationId { get; set; }
        public long AttendanceId { get; set; }
        public long TrainingPlanId { get; set; }
        public long? ParticipantId { get; set; }
        public long? QuestionSetId { get; set; }
        public string? TrainingPlanName { get; set; }
        public DateTime? TrainingDate { get; set; }
        public string? QuestionSetName { get; set; }
        public string? ParticipantName { get; set; }
        public string? Designation { get; set; }
        public bool IsEvaluationCompleted { get; set; }
    }
}
